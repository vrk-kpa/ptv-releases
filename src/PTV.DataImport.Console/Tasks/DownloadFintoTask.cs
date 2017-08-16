/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    /// <summary>
    /// Temporary class for getting data from finto.fi
    /// </summary>
    public class DownloadFintoDataTask
    {
        private const string FintoSettingsFile = @"Finto\settings.json";
        private const string OutputFolder = @"Finto";

        public void GetAllFintoData()
        {
            var settings = JsonConvert.DeserializeObject<FintoSettings>(File.ReadAllText(FintoSettingsFile));

            Task t = DownloadData(settings);
            t.Wait();
        }

        private async Task DownloadData(FintoSettings settings)
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
                    await DownloadData(definition, settings);
                }
                Merge(definition);
            }
        }

        private async Task DownloadData(FintoDownloadSettingsItem download, FintoSettings settings)
        {
            foreach (var link in download.Links)
            {
                string action = settings.Endpoints.TryGet(link.Action);
                if (string.IsNullOrEmpty(action))
                {
                    Console.Error.WriteLine($"Action {action} not found. Check settings for {download.Name}.");
                    continue;
                }
                await GetFinto(settings, action, link.Param, link.File, download.DownloadFolder);
            }
        }

        private async Task GetFinto(FintoSettings settings, string action, string param, string path = null, string folder = "")
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = OutputFolder;
            }
            if (string.IsNullOrEmpty(path))
            {
                path = param;
            }
            Console.Write($"Loading data for {param}... started at {DateTime.UtcNow.ToString("HH:mm:ss zz")}");

            string result = await GetFintoJson(action, param, settings);
            if (result == null)
            {
                return;
            }

            var file = new FileInfo(Path.Combine(folder, $"{path}.json"));
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            object x = JsonConvert.DeserializeObject(result);
            string texts = JsonConvert.SerializeObject(x, Formatting.Indented);
            File.WriteAllText(file.FullName, texts);
            Console.WriteLine($@"Data loaded. Result size {result.Length}. File '{file.FullName}'");
        }

        private void Merge(FintoDownloadSettingsItem downloadItem)
        {
            if (string.IsNullOrEmpty(downloadItem.MergeTo) || downloadItem.Links == null || downloadItem.Links.Count < 2)
            {
                return;
            }

            Dictionary<string, VmServiceViewsJsonItem> filtered = new Dictionary<string, VmServiceViewsJsonItem>();
            List<VmServiceViewsJsonItem> duplicated = new List<VmServiceViewsJsonItem>();
            List<VmServiceViewsJsonItem> moreParents = new List<VmServiceViewsJsonItem>();

            foreach (var link in downloadItem.Links)
            {
                try
                {
                    string path = string.IsNullOrEmpty(link.File) ? link.Param : link.File;
                    var file = Path.Combine(downloadItem.DownloadFolder ?? OutputFolder, $"{path}.json");
                    string json = File.ReadAllText(file);
                    Console.WriteLine($"loading file {file}.");
                    List<VmServiceViewsJsonItem> data = new List<VmServiceViewsJsonItem>();
                    switch (link.Action)
                    {
                        case "dataUri":
                        case "broaderUri":
                        case "narrowerUri":
                            data.Add(JsonConvert.DeserializeObject<VmServiceViewsJsonItem>(json));
                            break;
                        default:
                            data = JsonConvert.DeserializeObject<List<VmServiceViewsJsonItem>>(json);
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
                                throw new Exception($"{item.Id} from {link.Param} has different exact matches");
                            }
                            if (!item.NarrowerURIs.All(x => existing.NarrowerURIs.Contains(x)))
                            {
                                throw new Exception($"{item.Id} from {link.Param} has different exact matches");
                            }
                            if (!item.BroaderURIs.All(x => existing.BroaderURIs.Contains(x)))
                            {
                                throw new Exception($"{item.Id} from {link.Param} has different exact matches");
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
                    Console.Error.WriteLine($"Couln't parse json for {link.Action}. {e.Message}");
                }
            }

            if (downloadItem.FixHierarchy)
            {
                Console.WriteLine("Merge hierarchy.");
                foreach (var item in filtered.Values.Concat(duplicated))
                {
                    item.NarrowerURIs.Select(x => filtered.TryGet(x)).Where(x => x != null && !x.BroaderURIs.Contains(item.Id)).ForEach(x => x.BroaderURIs.Add(item.Id));
                    item.BroaderURIs.Select(x => filtered.TryGet(x)).Where(x => x != null && !x.NarrowerURIs.Contains(item.Id)).ForEach(x => x.NarrowerURIs.Add(item.Id));
                }
            }

            string result = JsonConvert.SerializeObject(filtered.Values, Formatting.Indented);
            var outputFile = new FileInfo(Path.Combine(downloadItem.MergeFolder ?? OutputFolder, $"{downloadItem.MergeTo}.json"));
            if (!outputFile.Directory.Exists)
            {
                outputFile.Directory.Create();
            }
            Console.WriteLine($"Merged to {outputFile.FullName}.");
            Console.WriteLine($"Duplicated ids {duplicated.Count}.");
            Console.WriteLine($"Items with more parents {moreParents.Count}.");
            File.WriteAllText(outputFile.FullName, result);
//            File.WriteAllText(Path.Combine(OutputFolder, "marged.duplicated.json"), JsonConvert.SerializeObject(duplicated));
//            File.WriteAllText(Path.Combine(OutputFolder, "broaders.json"), JsonConvert.SerializeObject(moreParents));
        }

        private async Task<string> GetFintoJson(string action, string param, FintoSettings settings)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 30, 0);
                    var response = await client.GetAsync(settings.Endpoints["baseUri"] + string.Format(action, param));
                    string body = await response.Content.ReadAsStringAsync();
                    return body;
                }
                catch (TimeoutException e)
                {
                    Console.Error.WriteLine(e);
                    return null;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    return null;
                }
            }
        }
    }

    internal class FintoSettings
    {
        public Dictionary<string, string> Endpoints { get; set; }
        public List<string> Downloads { get; set; }
        public Dictionary<string, FintoDownloadSettingsItem> DownloadDefinitions { get; set; }

    }

    internal class FintoDownloadSettingsItem
    {
        public bool Enabled { get; set; }
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
    }

    internal class FintoDownloadLink
    {
        public string Action { get; set; }
        public string Param { get; set; }
        public string File { get; set; }
    }
}