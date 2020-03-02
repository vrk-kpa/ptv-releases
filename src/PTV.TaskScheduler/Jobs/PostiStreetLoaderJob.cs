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
using PTV.TaskScheduler.Enums;
using Quartz;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Repositories;
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
            IContextManager contextManager)
        {
            var jobData = GetApplicationJobConfiguration<StreetImportConfiguration>(context);

            var defaultProxy = context.Scheduler.Context.Get(QuartzScheduler.PROXY_SERVER_SETTINGS) as ProxyServerSettings;
            var proxySettings = defaultProxy.OverrideBy(jobData.ProxyServerSettings);

            var languageCache = serviceProvider.GetService<ILanguageCache>();
            var finnishId = languageCache.Get("fi");

            var stopwatchDownloader = Stopwatch.StartNew();
            var postiStreetLines = Asyncs.HandleAsyncInSync(() => HttpClientWithProxy.UseAsync(proxySettings, async client =>
            {
                var datFileUrl = await GetDatFileUrl(client, jobData.UrlBase);
                return await ProcessPostiData(client, datFileUrl);
            }));
            stopwatchDownloader.Stop();
            var downloadingPosti = stopwatchDownloader.Elapsed;

            Console.WriteLine("Load PTV data for address cache....");
            var stopwatchLoader = Stopwatch.StartNew();
            var clsStreetCache = contextManager.ExecuteReader(unitOfWork =>
            {
                var streetRepo = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
                return streetRepo.All()
                    .Include(s => s.Municipality)
                    .Include(s => s.StreetNames)
                    .Include(s => s.StreetNumbers).ThenInclude(sn => sn.PostalCode)
                    .ToList()
                    //.Where(s => s.IsValid)
                    .Select(s => new ClsStreetCacheItem
                    {
                        Id = s.Id,
                        MunicipalityCode = s.Municipality.Code,
                        Name = s.StreetNames.Where(sn => sn.LocalizationId == finnishId)
                            .Select(sn => sn.Name)
                            .FirstOrDefault(),
                        NonCls = s.NonCls,
                        IsValid = s.IsValid,
                        StreetNumbers = s.StreetNumbers.Select(sn => new ClsStreetNumberCacheItem
                        {
                            Id = sn.Id,
                            StartNumber = sn.StartNumber,
                            EndNumber = sn.EndNumber,
                            IsEven = sn.IsEven,
                            NonCls = sn.NonCls,
                            IsValid = sn.IsValid,
                            PostalCode = sn.PostalCode.Code
                        })
                    })
                    .GroupBy(x => new {x.Name, x.MunicipalityCode})
                    .ToDictionary(x => (x.Key.Name, x.Key.MunicipalityCode), x => x.OrderBy(y => y.IsValid ? 0 : 1).FirstOrDefault());
            });
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

            ArchiveOldFiles(jobData.Folder, jobData.ArchiveFolder);
            var jobStatus = new StringBuilder();
            var stopwatchSaver = Stopwatch.StartNew();
            for (var i = 0; i < postiStreetLines.Keys.Count; i += pageSize)
            {
                var jsonData = ConvertToJsonData(postiStreetLines, i, now);
                SaveToFile(jsonData, i/pageSize, now, jobData.Folder, jobStatus);
            }
            stopwatchSaver.Stop();
            var savingStreetFiles = stopwatchSaver.Elapsed;

            Console.WriteLine("Done.");


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

        private void SaveToFile(VmStreetAddressCollection jsonData, int partNumber, DateTime created, string folder, StringBuilder jobStatus)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                result.FilesCount++;
                var fileName = Path.Combine(folder,
                    "postiStreet" + "_" + created.ToString("MM_dd_yyyy") + "_Part_" + partNumber + ".json");
                File.WriteAllText(fileName, JsonConvert.SerializeObject(jsonData));
            }
            catch (Exception ex)
            {
                jobStatus.AppendLine($"Exception during creation json files: {ex}");
                RemoveOldFiles(fileModified=>fileModified > created.AddHours(-1), folder);
            }
        }

        private void RemoveOldFiles(Func<DateTime, bool> fileFilter, string folder)
        {
            if (Directory.Exists(folder))
            {
                var directory = new DirectoryInfo(folder);
                var files = directory.GetFiles()
                    .Where(file=>fileFilter(file.LastWriteTime));
                files.ForEach(file => file.Delete());
            }
        }

        private void ArchiveOldFiles(string folder, string archiveFolder)
        {
            if (!Directory.Exists(archiveFolder))
            {
                Directory.CreateDirectory(archiveFolder);
            }

            if (Directory.Exists(folder))
            {
                var directory = new DirectoryInfo(folder);
                var files = directory.GetFiles();
                files.ForEach(file =>
                {
                    var destFile = Path.Combine(archiveFolder, file.Name);
                    if (File.Exists(destFile))
                    {
                        File.Delete(destFile);
                    }
                    file.MoveTo(destFile);
                });
            }
            RemoveOldFiles(fileModified=>fileModified<DateTime.UtcNow.AddDays(-14), archiveFolder);
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
                if(stopCount % 20 == 0) Console.WriteLine("Match line....{0}", stopCount);
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

        private async Task<Dictionary<(string Name, string Code), List<PostiStreetLine>>> ProcessPostiData(HttpClient client, string url)
        {
            var dictionary = new Dictionary<(string Name, string Code), List<PostiStreetLine>>();
            var httpStream = await client.GetStreamAsync(url);
            using (var textReader = new StreamReader(httpStream, Encoding.GetEncoding("iso-8859-1")))
            {
                var counter = 0;
                string line;
                while ((line = await textReader.ReadLineAsync()) != null)
                {
                    result.PostiLinesCount++;
                    if(counter++ % 50 == 0) Console.WriteLine("Get file line {0}", counter);
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

            httpStream.Dispose();
            return dictionary;
        }

        /// <summary>
        /// Gets the most recent .dat file from Posti
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetDatFileUrl(HttpClient client, string url)
        {
            Console.WriteLine("Get file name from Posti....");
            var text = await client.GetStringAsync(url);
            var httpParts = text.Split("http");
            var correctPart = httpParts.FirstOrDefault(part => part.Contains("unzip/BAF"));
            var dataUrl = correctPart.Split(".dat").FirstOrDefault();
            Console.WriteLine("File name {0}",dataUrl);
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
