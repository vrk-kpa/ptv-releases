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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Domain.Model.Models.Jobs;
using PTV.Domain.Model.Models.PostiStreets;
using PTV.Framework;
using PTV.TaskScheduler.Configuration;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class PostiPostalCodesJob : BaseJob
    {
        private const string LanguageCodeFi = "fi";
        private const string LanguageCodeSv = "sv";
        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager, VmJobSummary jobSummary)
        {
            var jobData = GetApplicationJobConfiguration<StreetImportConfiguration>(context);

            // load data from Posti
            var stopwatchDownloader = Stopwatch.StartNew();
            var postiStreetLines = Asyncs.HandleAsyncInSync(() => PtvHttpClient.UseAsync(async client =>
            {
                var datFileUrl = await GetDatFileUrl(client, jobData.UrlBase, jobSummary);
                return await ProcessPostiData(client, datFileUrl);
            }));
            stopwatchDownloader.Stop();
            var downloadingPosti = stopwatchDownloader.Elapsed;

            return contextManager.ExecuteWriter(unitOfWork =>
            {
                // set municipality Id from PTV to Posti data
                var municipalities = unitOfWork.CreateRepository<IMunicipalityRepository>().All().ToDictionary(m => m.Code, m => m.Id);
                FillMunicipalityId(postiStreetLines, municipalities);

                var postalCodeRep = unitOfWork.CreateRepository<IPostalCodeRepository>();

                string userName = unitOfWork.GetUserNameForAuditing(SaveMode.AllowAnonymous);
                postalCodeRep.BatchUpdate(new PostalCode(), ot => ot.IsValid, null, null, userName);
                // add new postal codes and update existing
                var vmToEntity = serviceProvider.GetRequiredService<ITranslationViewModel>();
                vmToEntity.TranslateAll<VmJsonPostalCode, PostalCode>(postiStreetLines, unitOfWork);

                // save
                unitOfWork.Save(SaveMode.AllowAnonymous, userName: context.JobDetail.Key.Name);

                // job status result
                var jobStatus = new StringBuilder();
                jobStatus.AppendLine($"Downloading file from Posti: {downloadingPosti}");
                jobStatus.AppendLine($"Downloaded: {postiStreetLines.Count}");
                jobStatus.AppendLine($"Invalidated: {postalCodeRep.All().Count(x => !x.IsValid)}.");
                return jobStatus.ToString();
            });
        }

        /// <summary>
        /// Gets the most recent .dat file from Posti
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetDatFileUrl(HttpClient client, string url, VmJobSummary jobSummary)
        {
            TaskSchedulerLogger.LogJobInfo(jobSummary, $"Get file name from Posti....", JobStatusService);
            var text = await client.GetStringAsync(url);
            var httpParts = text.Split("http");
            var correctPart = httpParts.FirstOrDefault(part => part.Contains("unzip/PCF"));
            var dataUrl = correctPart.Split(".dat").FirstOrDefault();
            TaskSchedulerLogger.LogJobInfo(jobSummary, $"File name {dataUrl}", JobStatusService);
            return $"http{dataUrl}.dat";
        }

        private async Task<List<VmJsonPostalCode>> ProcessPostiData(HttpClient client, string url)
        {
            var codeList = new List<VmJsonPostalCode>();
            var httpStream = await client.GetStreamAsync(url);
            using (var textReader = new StreamReader(httpStream, Encoding.GetEncoding("iso-8859-1")))
            {
                var counter = 0;
                string line;
                while ((line = await textReader.ReadLineAsync()) != null)
                {
                    codeList.Add(ProcessPostiLine(line));
                }
            }

            httpStream.Dispose();
            return codeList;
        }

        private static VmJsonPostalCode ProcessPostiLine(string line)
        {
            var result = new VmJsonPostalCode
            {
                Code = line.Substring(13, 5).Trim(),
                Names = new List<VmJsonName>
                {
                    new VmJsonName {Language = LanguageCodeFi, Name = line.Substring(18, 30).Trim()},
                    new VmJsonName {Language = LanguageCodeSv, Name = line.Substring(48, 30).Trim()}
                },
                MunicipalityCode = line.Substring(176, 3).Trim(),
                IsValid = true
            };

            return result;
        }

        private static void FillMunicipalityId(List<VmJsonPostalCode> codes, IReadOnlyDictionary<string, Guid> municipalities)
        {
            codes.ForEach(code =>
            {
                code.MunicipalityId = code.MunicipalityCode != null && municipalities.ContainsKey(code.MunicipalityCode)
                ? municipalities[code.MunicipalityCode]
                : (Guid?) null;
            });
        }
    }
}
