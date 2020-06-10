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
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.TaskScheduler.Configuration;
using Quartz;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Cloud;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Jobs;
using PTV.Domain.Model.Models.PostiStreets;
using PTV.Domain.Model.Models.StreetData;
using PTV.Domain.Model.Models.StreetData.Responses;
using PTV.Framework;

namespace PTV.TaskScheduler.Jobs
{
    internal class PostiStreetLoaderJob : BaseJob
    {
        private const int pageSize = 1000;
       // public override JobTypeEnum JobType => JobTypeEnum.PostiStreetLoader;
        private ResultJob result = new ResultJob();

        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider,
            IContextManager contextManager, VmJobSummary jobSummary)
        {
            var jobData = GetApplicationJobConfiguration<StreetImportConfiguration>(context);

            var languageCache = serviceProvider.GetService<ILanguageCache>();
            var storage = serviceProvider.GetService<IStorageService>();
            var finnishId = languageCache.Get("fi");

            var stopwatchDownloader = Stopwatch.StartNew();
            var postiStreetLines = Asyncs.HandleAsyncInSync(() => PtvHttpClient.UseAsync(async client =>
            {
                var datFileUrl = await GetDatFileUrl(client, jobData.UrlBase, jobSummary);
                return await ProcessPostiData(client, datFileUrl);
            }));
            stopwatchDownloader.Stop();
            var downloadingPosti = stopwatchDownloader.Elapsed;

            DebuggerConsoleWriteLine("Load PTV data for address cache....");
            var stopwatchLoader = Stopwatch.StartNew();
            var clsStreetCache = LoadClsStreetCache(contextManager, finnishId);
            stopwatchLoader.Stop();
            var loadingDbStreets = stopwatchLoader.Elapsed;
            result.PtvAddressesCount = clsStreetCache.Count;

            var stopwatchMatcher = Stopwatch.StartNew();
            MatchStreetIds(postiStreetLines, clsStreetCache);
            stopwatchLoader.Stop();
            var matchingStreets = stopwatchMatcher.Elapsed;

            clsStreetCache.Clear();
            GC.Collect();

            var now = DateTime.UtcNow;
            var jobStatus = new StringBuilder();
            var stopwatchSaver = Stopwatch.StartNew();

            using (var client = storage.GetClient())
            {
                ArchiveOldFiles(jobData.Folder, jobData.ArchiveFolder, client);
                for (var i = 0; i < postiStreetLines.Keys.Count; i += pageSize)
                {
                    var jsonData = ConvertToJsonData(postiStreetLines, i, now);
                    SaveToFile(jsonData, i/pageSize, now, jobData.Folder, jobStatus, client);
                }
            }
            stopwatchSaver.Stop();
            var savingStreetFiles = stopwatchSaver.Elapsed;

            DebuggerConsoleWriteLine("Done.");

            jobStatus.AppendLine($"Downloading street file from Posti: {downloadingPosti}");
            jobStatus.AppendLine($"Loading street data from DB: {loadingDbStreets}");
            jobStatus.AppendLine($"Matching of street data: {matchingStreets}" );
            jobStatus.AppendLine($"Writing results to files: {savingStreetFiles}");

            jobStatus.AppendLine($"Number of valid Posti lines: {result.PostiLinesCount}");
            jobStatus.AppendLine($"Number of PTV streets: {result.PtvAddressesCount}");
            jobStatus.AppendLine($"Number of match addresses: {result.MatchAddressesCount}");
            jobStatus.AppendLine($"Number of updated addresses: {result.UpdatedAddressesCount}");
            jobStatus.AppendLine($"Number of new addresses: {result.NewAddressesCount}");
            jobStatus.AppendLine($"Number of result json files: {result.FilesCount}");

            return jobStatus.ToString();
        }

        private static Dictionary<(string Name, string MunicipalityCode), ClsStreetCacheItem> LoadClsStreetCache(IContextManager contextManager, Guid finnishId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var streetRepo = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
                var municipalityRepo = unitOfWork.CreateRepository<IMunicipalityRepository>();
                var streetNamesRepo = unitOfWork.CreateRepository<IClsAddressStreetNameRepository>();
                var streetNumbersRepo = unitOfWork.CreateRepository<IClsAddressStreetNumberRepository>();
                var postalCodeRepo = unitOfWork.CreateRepository<IPostalCodeRepository>();

                var municipalities = municipalityRepo.All()
                    .Select(x => new { x.Id, x.Code })
                    .ToDictionary(x => x.Id, x => x.Code);
                var finnishNames = streetNamesRepo.All()
                    .Where(x => x.LocalizationId == finnishId)
                    .ToDictionary(x => x.ClsAddressStreetId, x => x.Name);
                var postalCodes = postalCodeRepo.All()
                    .Select(x => new {x.Id, x.Code})
                    .ToDictionary(x => x.Id, x => x.Code);
                var streetNumbers = streetNumbersRepo.All()
                    .ToList()
                    .GroupBy(x => x.ClsAddressStreetId)
                    .ToDictionary(x => x.Key, x => x.ToList());
                
                return streetRepo.All()
                    .ToList()
                    .Select(s => new ClsStreetCacheItem
                    {
                        Id = s.Id,
                        MunicipalityCode = municipalities.GetValueOrDefault(s.MunicipalityId),
                        Name = finnishNames.GetValueOrDefault(s.Id),
                        NonCls = s.NonCls,
                        IsValid = s.IsValid,
                        StreetNumbers = SelectStreetNumbers(streetNumbers, s.Id, postalCodes)
                    })
                    .GroupBy(x => new {x.Name, x.MunicipalityCode})
                    .ToDictionary(x => (x.Key.Name, x.Key.MunicipalityCode), x => x.OrderBy(y => y.IsValid ? 0 : 1).FirstOrDefault());
            });
        }

        private static IEnumerable<ClsStreetNumberCacheItem> SelectStreetNumbers(Dictionary<Guid, List<ClsAddressStreetNumber>> streetNumbers, Guid streetId, Dictionary<Guid, string> postalCodes)
        {
            var collection = streetNumbers.GetValueOrDefault(streetId);
            if (collection == null)
            {
                return new List<ClsStreetNumberCacheItem>();
            }
            
            return collection.Select(sn => new ClsStreetNumberCacheItem
                {
                    Id = sn.Id,
                    StartNumber = sn.StartNumber,
                    EndNumber = sn.EndNumber,
                    IsEven = sn.IsEven,
                    NonCls = sn.NonCls,
                    IsValid = sn.IsValid,
                    PostalCode = postalCodes.GetValueOrDefault(sn.PostalCodeId)
                });
        }

        private void SaveToFile(
            VmStreetAddressCollection jsonData, 
            int partNumber, 
            DateTime created, 
            string folder, 
            StringBuilder jobStatus, 
            IStorageClient client)
        {
            try
            {
                result.FilesCount++;
                var fileName = "postiStreet" + "_" + created.ToString("MM_dd_yyyy") + "_Part_" + partNumber + ".json";
                client.SaveFile(folder, fileName, JsonConvert.SerializeObject(jsonData));
            }
            catch (Exception ex)
            {
                jobStatus.AppendLine($"Exception during creation json files: {ex}");
                RemoveOldFiles(fileModified=>fileModified > created.AddHours(-1), folder, client);
            }
        }

        private void RemoveOldFiles(Func<DateTime, bool> fileFilter, string folder, IStorageClient client)
        {
            var toRemove = client.ListFiles(folder).Where(file => fileFilter(file.LastModified));
            foreach (var file in toRemove)
            {
                var filename = Path.GetFileName(file.Name);
                client.DeleteFile(folder, filename);
            }
        }

        private void ArchiveOldFiles(string folder, string archiveFolder, IStorageClient client)
        {
            var files = client.ListFiles(folder);
            foreach (var file in files)
            {
                var filename = Path.GetFileName(file.Name);
                client.DeleteFile(archiveFolder, filename);
                client.MoveFile(folder, archiveFolder, filename);
            }
            RemoveOldFiles(fileModified=>fileModified<DateTime.UtcNow.AddDays(-14), archiveFolder, client);
        }

        private VmStreetAddressCollection ConvertToJsonData(Dictionary<(string Name, string Code), List<PostiStreetLine>> postiStreetLines, int start, DateTime created)
        {
            var result = new VmStreetAddressCollection();
            result.Meta = new VmMeta
            {
                Code = 200,
                From = start,
                PageSize = pageSize,
                ResultCount = Math.Min(pageSize, postiStreetLines.Count - start),
                TotalResults = postiStreetLines.Count
            };
            result.Results = postiStreetLines.Skip(start).Take(pageSize).Select(streetGroup => new VmStreetAddress
            {
                Created = created,
                Id = streetGroup.Value.First().StreetId,
                Modified = created,
                Municipality = new VmMunicipality
                {
                    Code = streetGroup.Key.Code
                },
                Names = new VmStreetNamesJson
                {
                    Fi = streetGroup.Key.Name,
                    Sv = streetGroup.Value.First().StreetNameSv
                },
                StreetNumbers = streetGroup.Value.Select(streetNumber => new VmStreetNumber
                {
                    Created = created,
                    Id = streetNumber.StreetNumberId,
                    Modified = created,
                    EndNumber = streetNumber.EndNumber ?? 0,
                    IsEven = streetNumber.IsEven ?? false,
                    PostalCode = new VmPostalCode
                    {
                        Code = streetNumber.PostalCode
                    },
                    StartCharacter = streetNumber.StartCharacter,
                    StartNumber = streetNumber.StartNumber ?? 0,
                    EndCharacter = streetNumber.EndCharacter,
                    EndCharacterEnd = streetNumber.EndCharacterDual,
                    EndNumberEnd = streetNumber.EndNumberDual ?? 0,
                    StartCharacterEnd = streetNumber.StartCharacterDual,
                    StartNumberEnd = streetNumber.StartNumberDual ?? 0
                }).ToList()
            }).ToList();

            return result;
        }

        private void MatchStreetIds(
        Dictionary<(string Name, string Code), List<PostiStreetLine>> postiStreetLines,
        Dictionary<(string Name, string Code), ClsStreetCacheItem> clsStreetCache)
        {
            var stopCount = 0;
            foreach (var postiStreetLine in postiStreetLines)
            {
                stopCount++;
                if(stopCount % 20 == 0) DebuggerConsoleWriteLine($"Match line....{stopCount}");
                var key = postiStreetLine.Key;
                if (clsStreetCache.TryGetValue(key, out var clsStreet))
                {
                    foreach (var line in postiStreetLine.Value)
                    {
                        line.StreetId = clsStreet.Id;
                        MatchStreetNumberIds(line, clsStreet);
                    }
                }
                else
                {
                    var newStreetId = Guid.NewGuid();
                    postiStreetLine.Value.ForEach(line =>
                    {
                        line.StreetNumberId = Guid.NewGuid();
                        line.StreetId = newStreetId;
                        result.NewAddressesCount++;
                    });
                }
            }
        }

        private void MatchStreetNumberIds(PostiStreetLine postiLine, ClsStreetCacheItem clsStreet)
        {
            var possibleRanges =
                clsStreet.StreetNumbers.Where(sn => sn.IsEven == postiLine.IsEven && sn.PostalCode == postiLine.PostalCode);

            var exactMatch = possibleRanges.FirstOrDefault(x => x.StartNumber == postiLine.SmallestNumber
                                                               && x.EndNumber == postiLine.HighestNumber);
            if (exactMatch != null)
            {
                postiLine.StreetNumberId = exactMatch.Id;
                result.MatchAddressesCount++;
                return;
            }

            var startMatch = possibleRanges.Where(x => x.StartNumber == postiLine.StartNumber)
                .OrderBy(x => Math.Abs(x.EndNumber - postiLine.HighestNumber))
                .FirstOrDefault();

            if (startMatch != null)
            {
                postiLine.StreetNumberId = startMatch.Id;
                result.UpdatedAddressesCount++;
                return;
            }

            var endMatch = possibleRanges.Where(x => x.EndNumber == postiLine.HighestNumber)
                .OrderBy(x => Math.Abs(x.StartNumber - postiLine.SmallestNumber))
                .FirstOrDefault();

            if (endMatch != null)
            {
                postiLine.StreetNumberId = endMatch.Id;
                result.UpdatedAddressesCount++;
                return;
            }

            postiLine.StreetNumberId = Guid.NewGuid();
            result.NewAddressesCount++;
        }

        private async Task<Dictionary<(string Name, string Code), List<PostiStreetLine>>> ProcessPostiData(
            HttpClient client, string url)
        {
            var dictionary = new Dictionary<(string Name, string Code), List<PostiStreetLine>>();
            using (var httpStream = await client.GetStreamAsync(url))
            using (var textReader = new StreamReader(httpStream, Encoding.GetEncoding("iso-8859-1")))
            {
                var counter = 0;
                string line;
                while ((line = await textReader.ReadLineAsync()) != null)
                {
                    result.PostiLinesCount++;
                    if(counter++ % 50 == 0) DebuggerConsoleWriteLine($"Get file line {counter}");
                    var lineData = new PostiStreetLine(line);

                    // If street is empty or contains no numbers, skip it
                    if (lineData.IsEven == null)
                    {
                        continue;
                    }

                    var key = (lineData.StreetNameFi, lineData.MunicipalityCode);
                    if (dictionary.ContainsKey(key))
                    {
                        dictionary[key].Add(lineData);
                    }
                    else
                    {
                        dictionary.Add(key, new List<PostiStreetLine>{ lineData });
                    }
                }
            }
            
            return dictionary;
        }

        /// <summary>
        /// Gets the most recent .dat file from Posti
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetDatFileUrl(HttpClient client, string url, VmJobSummary jobSummary)
        {
            TaskSchedulerLogger.LogJobInfo(jobSummary,"Get file name from Posti....", JobStatusService);
            var text = await client.GetStringAsync(url);
            var httpParts = text.Split("http");
            var correctPart = httpParts.FirstOrDefault(part => part.Contains("unzip/BAF"));
            var dataUrl = correctPart.Split(".dat").FirstOrDefault();
            TaskSchedulerLogger.LogJobInfo(jobSummary, $"File name {dataUrl}", JobStatusService);
            return $"http{dataUrl}.dat";
        }

        private class ResultJob
        {
            public ResultJob()
            {
                PostiLinesCount = 0;
                PtvAddressesCount = 0;
                NewAddressesCount = 0;
                MatchAddressesCount = 0;
                UpdatedAddressesCount = 0;
                FilesCount = 0;
            }
            public int PostiLinesCount { get; set; }
            public int PtvAddressesCount { get; set; }
            public int NewAddressesCount { get; set; }
            public int MatchAddressesCount { get; set; }
            public int UpdatedAddressesCount { get; set; }
            public int FilesCount { get; set; }
        }
    }
}
