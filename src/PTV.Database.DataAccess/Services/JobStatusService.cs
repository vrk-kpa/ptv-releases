/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IJobStatusService), RegisterType.Transient)]
    internal class JobStatusService : IJobStatusService
    {
        private readonly IContextManager contextManager;
        private readonly ITranslationEntity translationManagerToVm;
        private readonly ITranslationViewModel translationManagerToEntity;
        private readonly ApplicationConfiguration applicationConfiguration;
        private readonly IJobExecutionCache jobExecutionCache;
        private readonly IJobStatusCache jobStatusCache;

        public JobStatusService(
            IContextManager contextManager, 
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ApplicationConfiguration applicationConfiguration,
            ICacheManager cacheManager)
        {
            this.contextManager = contextManager;
            this.translationManagerToVm = translationManagerToVm;
            this.translationManagerToEntity = translationManagerToEntity;
            this.applicationConfiguration = applicationConfiguration;
            this.jobExecutionCache = cacheManager.JobExecutionCache;
            this.jobStatusCache = cacheManager.JobStatusCache;
        }
        
        public IEnumerable<VmJobSummary> GetJobsSummary(bool archive = false)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var jobSummaryRepo = unitOfWork.CreateRepository<IJobSummaryRepository>();
                var result = new List<VmJobSummary>();
                var groupedSummaries = jobSummaryRepo.All()
                    .Include(x => x.JobLogs)
                    .ToList()
                    .GroupBy(x => x.JobType);

                if (archive)
                {
                    foreach (var group in groupedSummaries)
                    {
                        var archiveForJob = group.OrderByDescending(x => x.StartUtc).Skip(1).ToList();
                        var translated = translationManagerToVm.TranslateAll<JobSummary, VmJobSummary>(archiveForJob);
                        result.AddRange(translated);
                    }
                }
                else
                {

                    foreach (var group in groupedSummaries)
                    {
                        var currentSummary = group.OrderByDescending(x => x.StartUtc).FirstOrDefault();
                        if (currentSummary == null)
                        {
                            continue;
                        }

                        var translated = translationManagerToVm.Translate<JobSummary, VmJobSummary>(currentSummary);
                        result.Add(translated);
                    }
                }

                return result;
            });
        }

        public void Save(VmJobSummary summary, VmJobLog log, bool allowAnonymous)
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;

            if (!summary.Id.IsAssigned())
            {
                CheckAndShrinkArchive(summary.JobType, saveMode);
                SaveSummary(summary, saveMode);
            }

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var translatedLog = translationManagerToEntity.Translate<VmJobLog, JobLog>(log, unitOfWork);
                translatedLog.JobSummaryId = summary.Id.Value;

                unitOfWork.Save(saveMode);
            });
        }

        private void SaveSummary(VmJobSummary summary, SaveMode saveMode)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var translatedSummary =
                    translationManagerToEntity.Translate<VmJobSummary, JobSummary>(summary, unitOfWork);
                unitOfWork.Save(saveMode);
                summary.Id = translatedSummary.Id;
            });
        }

        public void CheckAndShrinkArchive(string jobType, SaveMode saveMode)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var jobSummaryRepo = unitOfWork.CreateRepository<IJobSummaryRepository>();

                var existingRecordsCount = jobSummaryRepo.All().Count(x => x.JobType == jobType);
                if (existingRecordsCount > applicationConfiguration.LogArchiveThreshold)
                {
                    var toRemove = jobSummaryRepo.All()
                        .Where(x => x.JobType == jobType)
                        .OrderByDescending(x => x.StartUtc)
                        .Skip(applicationConfiguration.LogArchiveThreshold)
                        .ToList();
                    jobSummaryRepo.Remove(toRemove);
                    unitOfWork.Save(saveMode);
                }
            });
        }

        public IEnumerable<VmJobSummary> GetJobsSummary(string jobType, bool archive = false)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var jobSummaryRepo = unitOfWork.CreateRepository<IJobSummaryRepository>();
                var summaries = jobSummaryRepo.All()
                    .Include(x => x.JobLogs)
                    .Where(x => x.JobType == jobType)
                    .OrderByDescending(x => x.StartUtc)
                    .ToList();

                if (archive)
                {
                    var archiveForJob = summaries.Skip(1);
                    var translated = translationManagerToVm.TranslateAll<JobSummary, VmJobSummary>(archiveForJob);
                    return translated;
                }
                else
                {
                    var currentLog = summaries.FirstOrDefault();
                    var translated = translationManagerToVm.Translate<JobSummary, VmJobSummary>(currentLog);
                    return new List<VmJobSummary> {translated};
                }
            });
        }

        public FileResult GetAllLogsFilesZip()
        {
            byte[] compressedBytes = new byte[]{0x00};
            var allEntities = contextManager.ExecuteReader(unitOfWork =>
            {
                var jobSummaryRepo = unitOfWork.CreateRepository<IJobSummaryRepository>();
                var summaries = jobSummaryRepo.All()
                    .Include(x => x.JobLogs)
                    .ToList();

                return summaries;
            });

            var jobSummaries = translationManagerToVm.TranslateAll<JobSummary, VmJobSummary>(allEntities);
            
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var jobSummary in jobSummaries)
                    {
                        var entryName = $"{jobSummary.JobType}-{jobSummary.StartUtc:s}.json";
                        var archiveEntry = archive.CreateEntry(entryName);
                        var json = JsonConvert.SerializeObject(jobSummary);
                        var bytes = Encoding.UTF8.GetBytes(json);
                        using (var entryStream = archiveEntry.Open())
                        {
                            entryStream.Write(bytes);
                        }
                    }
            
                }
                compressedBytes = memoryStream.ToArray();
            }
            
            return new FileContentResult(compressedBytes, "application/zip");
        }

        public IEnumerable<VmJsonJobLog> GetAllJobsLog()
        {
            var allEntities = contextManager.ExecuteReader(unitOfWork =>
            {
                var jobSummaryRepo = unitOfWork.CreateRepository<IJobSummaryRepository>();
                var summaries = jobSummaryRepo.All()
                    .Include(x => x.JobLogs)
                    .ToList();

                return summaries;
            });

            var logs = allEntities.SelectMany(x => x.JobLogs)
                .OrderByDescending(x => x.TimeUtc)
                .ThenByDescending(x => x.NlogLevel);
            foreach (var log in logs)
            {
                yield return new VmJsonJobLog
                {
                    Exception = $"{log.Exception} -> {log.StackTrace}",
                    Level = log.NlogLevel,
                    Message = log.Message,
                    Time = log.TimeUtc.ToString("s"),
                    ExecutionType =  log.JobSummary.JobExecutionTypeId.HasValue 
                        ? jobExecutionCache.GetEnumByValue(log.JobSummary.JobExecutionTypeId.Value).ToString() 
                        : null,
                    JobStatus = log.JobStatusTypeId.HasValue
                        ? jobStatusCache.GetEnumByValue(log.JobStatusTypeId.Value).ToString()
                        : null,
                    JobType = log.JobSummary.JobType,
                    OperationId = log.JobSummary.OperationName
                };
            }
        }

        public void ClearAllLogs()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var jobSummaryRepo = unitOfWork.CreateRepository<IJobSummaryRepository>();
                var toRemove = jobSummaryRepo.All().ToList();
                jobSummaryRepo.Remove(toRemove);
                unitOfWork.Save();
            });
        }
    }
}
