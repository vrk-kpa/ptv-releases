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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.ExternalSources;
using PTV.Framework;

namespace PTV.DataImport.Console.Tasks
{

    public class DownloadAndImportDigitalAuthorizationsTask
    {
        private readonly IServiceProvider serviceProvider;
        private readonly DigitalAuthorizationsSettings digitalAuthorizationsSettings = new DigitalAuthorizationsSettings();
        private readonly ProxyServerSettings proxyServerSettings = new ProxyServerSettings();

        public DownloadAndImportDigitalAuthorizationsTask(IServiceProvider serviceProvider)
        {
            Program.Configuration.GetSection(ConfigKeys.DigitalAuthorizationsSettings).Bind(digitalAuthorizationsSettings);
            Program.Configuration.GetSection(ConfigKeys.ProxyServerSettings).Bind(proxyServerSettings);

            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            ILogger logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<DownloadAndImportDigitalAuthorizationsTask>();
            logger.LogDebug("DownloadAndImportDigitalAuthorizationsTask .ctor");
        }

        public void DownloadAndImportDigitalAuthorizations()
        {
            var resourceManager = new ResourceManager();
            // ReSharper disable once IdentifierTypo
            var rovaData = resourceManager.GetJsonResource(JsonResources.DigitalAuthorizations);
            if (digitalAuthorizationsSettings.Download)
            {
                var t = DownloadDigitalAuthorizations();
                t.Wait();
            }

            ImportDigitalAuthorizations(rovaData);
        }

        private async Task<string> DownloadDigitalAuthorizations()
        {
            System.Console.WriteLine("Downloading of digital authorizations ...");
            var data = await GetJsonData(digitalAuthorizationsSettings.BaseUri, proxyServerSettings);
            System.Console.WriteLine("Downloading of digital authorizations done.");

            if (digitalAuthorizationsSettings.SaveToResourceFile)
            {
                var fileName = $"{JsonResources.DigitalAuthorizations}.json";
                var file = new FileInfo(Path.Combine(digitalAuthorizationsSettings.ResourceFolder, fileName));
                if (file.Directory != null && file.Directory.Exists)
                {
                    var x = JsonConvert.DeserializeObject(data);
                    var formattedData = JsonConvert.SerializeObject(x, Formatting.Indented);

                    File.WriteAllText(file.FullName, formattedData);
                    System.Console.WriteLine($@"Data stored. Result size {formattedData.Length}. File '{file.FullName}'");
                }
            }

            return data;
        }

        public void ImportDigitalAuthorizations(string jsonData = null)
        {
            System.Console.WriteLine("Seeding digital authorizations..");
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var service = serviceScope.ServiceProvider.GetService<ISeedingService>();
                service?.SeedDigitalAuthorizations(jsonData);
            }
        }


        private static async Task<string> GetJsonData(string uri, ProxyServerSettings proxySettings)
        {

            var proxyUri = $"{proxySettings.Address}:{proxySettings.Port}";
            var proxy = new WebProxy(proxyUri);
            var httpClientHandler = new HttpClientHandler { Proxy = proxy };

            using (var client = new HttpClient(httpClientHandler))
            {
                var response = await client.GetAsync(uri);
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
        }
    }

    internal class DigitalAuthorizationsSettings
    {
        public string BaseUri { get; set; }
        public bool Download { get; set; }
        public bool SaveToResourceFile { get; set; }
        public string ResourceFolder { get; set; }
    }
}
