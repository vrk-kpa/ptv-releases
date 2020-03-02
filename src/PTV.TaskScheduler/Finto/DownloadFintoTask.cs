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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using Microsoft.Extensions.Logging;
using PTV.TaskScheduler.Configuration;
using ILogger = NLog.ILogger;

namespace PTV.TaskScheduler.Finto
{
    /// <summary>
    /// Temporary class for getting data from finto.fi
    /// </summary>
    [RegisterService(typeof(DownloadFintoDataTask), RegisterType.Transient)]
    internal class DownloadFintoDataTask
    {
        private const string FintoSettingsFile = @"Finto\settings.json";
        private const string OutputFolder = @"Finto";

        private UrlBaseConfiguration urlConfiguration;
        private ILogger<DownloadFintoDataTask> logger;

        private Func<string, UrlBaseConfiguration, string> downloadFunc;

        public DownloadFintoDataTask(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<DownloadFintoDataTask>();
        }

        public void Init(Func<string, UrlBaseConfiguration, string> downloadFunc, UrlBaseConfiguration urlConfiguration)
        {
            this.downloadFunc = downloadFunc;
            this.urlConfiguration = urlConfiguration;
        }

        public void GetAllFintoData()
        {
            var settings = JsonConvert.DeserializeObject<FintoSettings>(File.ReadAllText(FintoSettingsFile));
            DownloadData(settings);
        }

        internal void DownloadData(FintoSettings settings)
        {
            foreach (var download in settings.Downloads)
            {
                var definition = settings.DownloadDefinitions.TryGet(download);
                if (definition == null)
                {
                    Console.Error.WriteLine($"Definitoin for key {download} not found.");
                    continue;
                }
                if (!definition.MergeOnly)
                {
                    DownloadData(definition, settings);
                }
                Merge(definition);
            }
        }

        private void DownloadData(FintoDownloadSettingsItem download, FintoSettings settings)
        {
            foreach (var link in download.Links)
            {
//                string action = settings.Actions.TryGet(link.Action);
//                if (string.IsNullOrEmpty(action))
//                {
//                    Console.Error.WriteLine($"Action {action} not found. Check settings for {download.Name}.");
//                    continue;
//                }
                link.Content = GetFinto(settings, link, download.DownloadFolder, download.SaveToFile);
            }
        }

        private List<VmServiceViewsJsonItem> GetFinto(FintoSettings settings, FintoDownloadLink link, string folder = "", bool saveToFile = true)
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = OutputFolder;
            }
            logger.LogInformation($"Loading data for {string.Join("/", link.Path)}... started at {DateTime.UtcNow:HH:mm:ss zz}");
            var languages = new[] {"fi", "sv", "en"};

            var queryParams = new Dictionary<string, string>();
            var allData = new List<VmServiceViewsSearchJsonItem>();

            foreach (var language in languages)
            {
                queryParams["lang"] = language;
                string result = GetFintoJson(link, settings, queryParams);
                if (result == null)
                {
                    continue;
                }

                var searchedData = JsonConvert.DeserializeObject<List<VmServiceViewsSearchJsonItem>>(result);
                allData.AddRange(searchedData);
            }

            var data = allData.GroupBy(x => x.Id).Select(x => new VmServiceViewsJsonItem
            {
                Id = x.First().Id,
                Label = x.GroupBy(l => l.Language).ToDictionary(l => l.Key.ToLowerInvariant(), l => l.First().Label),
                Notes = x.GroupBy(l => l.Language).ToDictionary(l => l.Key.ToLowerInvariant(), l => l.SelectMany(n => n.Notes ?? new List<string>()).ToList()),
                AssociatedURIs = x.First().AssociatedURIs,
                BroaderURIs = x.First().BroaderURIs,
                ConceptType = x.First().ConceptType,
                ConceptTypeURI = x.First().ConceptTypeURI,
                ExactMatchURIs = x.First().ExactMatchURIs,
                NarrowerURIs = x.First().NarrowerURIs,
                Notation = x.First().Notation,
                Scheme = x.First().Scheme,
                ReplacedBy = x.First().ReplacedBy
            }).ToList();

            if (saveToFile)
            {
                var texts = JsonConvert.SerializeObject(data, Formatting.Indented);
                var file = new FileInfo(Path.Combine(folder, link.File ?? $"{string.Join("_", link.Path)}.json"));
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }
                File.WriteAllText(file.FullName, texts);
                logger.LogInformation($@"Data loaded. Result size {texts.Length}. File '{file.FullName}'");
            }
            return data;
        }

        internal void Merge(FintoDownloadSettingsItem downloadItem)
        {
            if (downloadItem.Links == null || downloadItem.Links.Count == 0) return;
            if (string.IsNullOrEmpty(downloadItem.MergeTo) && downloadItem.SaveToFile) return;

            if (downloadItem.Links.Count == 1)
            {
                if (!downloadItem.SaveToFile && downloadItem.MergeTo == null)
                {
                    downloadItem.Content = downloadItem.Links[0].Content;
                }

                return;
            }

            Dictionary<string, VmServiceViewsJsonItem> filtered = new Dictionary<string, VmServiceViewsJsonItem>();
            List<VmServiceViewsJsonItem> duplicated = new List<VmServiceViewsJsonItem>();
            List<VmServiceViewsJsonItem> moreParents = new List<VmServiceViewsJsonItem>();

            foreach (var link in downloadItem.Links)
            {
                try
                {
                    var path = string.IsNullOrEmpty(link.File) ? link.File ?? $"{string.Join("_", link.Path)}.json" : link.File;
                    var file = Path.Combine(downloadItem.DownloadFolder ?? OutputFolder, $"{path}.json");
                    var json = downloadItem.SaveToFile ? File.ReadAllText(file) : null;
                    logger.LogInformation($"loading file {file}.");
                    var data = new List<VmServiceViewsJsonItem>();
                    switch (link.Path.FirstOrDefault())
                    {
                        case "dataUri":
                        case "broaderUri":
                        case "narrowerUri":
                            data.Add(json != null ? JsonConvert.DeserializeObject<VmServiceViewsJsonItem>(json) : link.Content.FirstOrDefault());
                            break;
                        default:
                            data = json != null ? JsonConvert.DeserializeObject<List<VmServiceViewsJsonItem>>(json) : link.Content;
                            break;
                    }

                    foreach (var item in data)
                    {
                        if (filtered.ContainsKey(item.Id))
                        {
                            var existing = filtered[item.Id];
                            duplicated.Add(item);
                            Console.WriteLine($"{path} id {item.Id} already exists.");
                            if (!item.ExactMatchURIs.All(x => existing.ExactMatchURIs.Contains(x)))
                            {
                                throw new Exception($"{item.Id} from {string.Join("/", link.Path)} has different exact matches");
                            }
                            if (!item.NarrowerURIs.All(x => existing.NarrowerURIs.Contains(x)))
                            {
                                throw new Exception($"{item.Id} from {string.Join("/", link.Path)} has different exact matches");
                            }
                            if (!item.BroaderURIs.All(x => existing.BroaderURIs.Contains(x)))
                            {
                                throw new Exception($"{item.Id} from {string.Join("/", link.Path)} has different exact matches");
                            }
                            continue;
                        }
                        if (item.BroaderURIs.Count > 1)
                        {
                            moreParents.Add(item);
                        }
                        filtered.Add(item.Id, item);
                        //item.Children = await GetNarrowers(item);
                    }
                }
                catch(Exception e)
                {
                    logger.LogError($"Couln't parse json for {string.Join("/", link.Path)}. {e.Message}", e);
                }
            }

            if (downloadItem.FixHierarchy)
            {
                logger.LogInformation("Merge hierarchy.");
                foreach (var item in filtered.Values.Concat(duplicated))
                {
                    item.NarrowerURIs.Select(x => filtered.TryGet(x)).Where(x => x != null && !x.BroaderURIs.Contains(item.Id)).ForEach(x => x.BroaderURIs.Add(item.Id));
                    item.BroaderURIs.Select(x => filtered.TryGet(x)).Where(x => x != null && !x.NarrowerURIs.Contains(item.Id)).ForEach(x => x.NarrowerURIs.Add(item.Id));
                }
            }


            var outputFile = new FileInfo(Path.Combine(downloadItem.MergeFolder ?? OutputFolder, $"{downloadItem.MergeTo}.json"));
            if (!outputFile.Directory.Exists)
            {
                outputFile.Directory.Create();
            }
            logger.LogInformation($"Merged to {outputFile.FullName}.");
            logger.LogInformation($"Duplicated ids {duplicated.Count}.");
            logger.LogInformation($"Items with more parents {moreParents.Count}.");

            if (downloadItem.SaveToFile)
            {
                string result = JsonConvert.SerializeObject(filtered.Values, Formatting.Indented);
                File.WriteAllText(outputFile.FullName, result);
            }
            else
            {
                downloadItem.Content = filtered.Values.ToList();
            }
//            File.WriteAllText(Path.Combine(OutputFolder, "marged.duplicated.json"), JsonConvert.SerializeObject(duplicated));
//            File.WriteAllText(Path.Combine(OutputFolder, "broaders.json"), JsonConvert.SerializeObject(moreParents));
        }

        private string BuildUrl(FintoDownloadLink link, FintoSettings settings, Dictionary<string, string> customQueryParams)
        {
            var uri = new Uri(settings.Params.TryGet("baseUri"));
            var relativePath = Path.Combine(settings.RelativePaths.Concat(link.Path).Select
            (
                u => settings.Params.TryGetValue(u, out string value)
                        ? value
                        : u
            ).ToArray());
            //            foreach (var urlPart in settings.RelativePaths.Concat(link.Path))
//            {
//
//                uri = settings.Params.TryGetValue(urlPart, out string value)
//                    ? new Uri(uri.AbsoluteUri, urik)
//                    : new Uri(uri, urlPart);
//            }
            uri = new Uri(uri, relativePath);
            Dictionary<string, string> values = Concat(settings.QueryParams, link.QueryParams, customQueryParams);
//            link.QueryParams.Add("lang", "fi");
//            if (!settings.QueryParams.IsNullOrEmpty() && !link.QueryParams.IsNullOrEmpty() )
//            {
//                values = settings.QueryParams.Concat(link.QueryParams).GroupBy(x => x.Key)
//                    .ToDictionary(x => x.Key, x => x.Last().Value);
//            }
//            else
//            {
//                values = settings.QueryParams.IsNullOrEmpty() ? link.QueryParams : settings.QueryParams;
//            }
            return QueryHelpers.AddQueryString(uri.AbsoluteUri, values ?? new Dictionary<string, string>());
        }

        private Dictionary<string, string> Concat(params IDictionary<string, string>[] data)
        {
            return data
                .WhereNotNull()
                .SelectMany(x => x.AsEnumerable())
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.First().Value);
        }

        internal string GetFintoJson(FintoDownloadLink link, FintoSettings settings, Dictionary<string, string> queryParams)
        {
            try
            {
//                client.Timeout = new TimeSpan(1, 0, 0);
                string url = BuildUrl(link, settings, queryParams);
                logger.LogInformation($"Loading data from {url}.");
                var data = downloadFunc(url, urlConfiguration);
//                throw new Exception(data);
                return data;
            }
            catch (TimeoutException e)
            {
                logger.LogError($"Timout for loading {link}", e);
                return null;
            }
//            catch (Exception e)
//            {
//                logger.LogError(e.Message, e);
//                throw;
//            }

        }
    }

    internal class FintoSettings
    {
        public List<string> RelativePaths { get; set; }
        public Dictionary<string, string> Params { get; set; }
        public Dictionary<string, string> QueryParams { get; set; }

        public List<string> Downloads { get; set; }
        public Dictionary<string, FintoDownloadSettingsItem> DownloadDefinitions { get; set; }

    }

    internal class FintoDownloadSettingsItem
    {
        /// <summary>
        /// Skip download data, used for merging already downloaded data
        /// </summary>
        public bool MergeOnly { get; set; }
        public string Name { get; set; }

        public List<FintoDownloadLink> Links { get; set; }
        public string MergeTo { get; set; }
        public bool FixHierarchy { get; set; }
        public string DownloadFolder { get; set; }
        public string MergeFolder { get; set; }
        public int? Version { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool SaveToFile { get; set; }

        public List<VmServiceViewsJsonItem> Content { get; set; }
    }

    internal class FintoDownloadLink
    {
        public List<string> Path { get; set; }
        public Dictionary<string, string> QueryParams { get; set; } = new Dictionary<string, string>();
        public string File { get; set; }
        public List<VmServiceViewsJsonItem> Content { get; set; }
    }

    internal class FintoSearchResult<T>
    {
        public int TotalHits { get; set; }
        public List<T> Hits { get; set; }
    }
}
