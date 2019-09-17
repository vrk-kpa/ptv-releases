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
using System.Diagnostics;
using System.Text;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using Quartz;
using Microsoft.Extensions.DependencyInjection;

namespace PTV.TaskScheduler.Jobs
{
    internal class StreetDataJob : BaseJob
    {
        private const int pageSize = 1000;
        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider,
            IContextManager contextManager)
        {
            var jobData = GetApplicationJobConfiguration<StreetImportConfiguration>(context);
            if (jobData == null)
            {
                TaskSchedulerLogger.LogJobWarn(context.FireInstanceId, JobKey,
                    $"Url configuration for {JobKey} is not found.");
                return null;
            }
            var streetDataService = serviceProvider.GetService<IStreetDataService>();
            TimeSpan downloadTime = TimeSpan.Zero;
                
            var stopwatchHttp = Stopwatch.StartNew();
            var streetAddresses = streetDataService.LoadAll(jobData.Folder, DateTime.UtcNow.AddDays(-2));
            stopwatchHttp.Stop();
            downloadTime = stopwatchHttp.Elapsed;
          
            var stopwatch = Stopwatch.StartNew();
            var streetAddressesResult = streetDataService.ImportAndUpdateAddresses(streetAddresses);
            var importTime = stopwatch.Elapsed;
            stopwatch.Stop();
            var deleteTime = stopwatch.Elapsed - importTime;

            var jobStatus = new StringBuilder();
            jobStatus.AppendLine($"Imported street addresses {streetAddressesResult.ImportedData}.");
            jobStatus.AppendLine($"Deleted street addresses {streetAddressesResult.DeletedData}.");
            jobStatus.AppendLine($"Invalidated existing addresses {streetAddressesResult.MarkedInvalidData}.");
            jobStatus.AppendLine($"Download duration {downloadTime}.");
            jobStatus.AppendLine($"Create and update duration {importTime}.");
            jobStatus.AppendLine($"Delete duration {deleteTime}.");

            return jobStatus.ToString();
        }

        private string FormatUrl(string baseUrl, ApplicationKapaConfiguration urlConfiguration, int from)
        {
            Debug.WriteLine($"Downloading batch from {from} to {from + pageSize}");
            return string.Format(baseUrl, urlConfiguration.UrlBase, urlConfiguration.Version, urlConfiguration.ApiKey, pageSize, from);
        }
    }
}