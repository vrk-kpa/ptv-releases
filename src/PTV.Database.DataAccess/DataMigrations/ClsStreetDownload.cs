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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.StreetData.Responses;
using PTV.Framework;

namespace PTV.Database.DataAccess.DataMigrations
{
    [RegisterService(typeof(IClsStreetDownload), RegisterType.Transient)]
    public class ClsStreetDownload : IClsStreetDownload
    {
        private readonly IStreetDataService streetDataService;
        private readonly ProxyServerSettings proxyServerSettings;
        private readonly ClsStreetDownloadSettings clsStreetDownloadSettings;
        private readonly ILogger<ClsStreetDownload> logger;
        private const int pageSize = 1000; 

        public ClsStreetDownload(IServiceProvider serviceProvider, ProxyServerSettings proxySettings, ClsStreetDownloadSettings clsStreetDownloadSettings)
        {
            this.streetDataService = serviceProvider.GetService<IStreetDataService>();
            this.proxyServerSettings = proxySettings;
            this.clsStreetDownloadSettings = clsStreetDownloadSettings;
            this.logger = serviceProvider.GetService<ILogger<ClsStreetDownload>>();
        }

        public void DownloadAddresses()
        {
            logger.LogInformation($"Downloading of addresses started. ({DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss.fff", CultureInfo.InvariantCulture)})");
            var streetAddresses = Asyncs.HandleAsyncInSync(() => HttpClientWithProxy.UseAsync(proxyServerSettings.OverrideBy(clsStreetDownloadSettings.ProxyServerSettings), async client =>
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

                var response = await streetDataService.DownloadAll(client, pageSize, from => FormatUrl(from));
                return response;
            }));
//            File.WriteAllText("adresy.dat", JsonConvert.SerializeObject(streetAddresses));
//
//            Console.WriteLine("Loading addresses....");

//            var streetAddresses = JsonConvert.DeserializeObject<VmStreetAddressCollection>(File.ReadAllText("adresy.dat"));
            logger.LogInformation($"Downloading of addresses done. ({DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss.fff", CultureInfo.InvariantCulture)})");
            Console.WriteLine("Importing addresses....");
            var importedStreetAddresses = streetDataService.ImportAndUpdateAddresses(streetAddresses);
            Console.WriteLine("Done.");
        }

        private string FormatUrl(int from)
        {
            Console.WriteLine($"Downloading addresses from {from} to {from + pageSize}");
            return string.Format(
                clsStreetDownloadSettings.UrlTemplate, 
                clsStreetDownloadSettings.UrlBase, 
                clsStreetDownloadSettings.Version, 
                clsStreetDownloadSettings.ApiKey, 
                pageSize, 
                from);
        }
    }
    
    public interface IClsStreetDownload {}
}