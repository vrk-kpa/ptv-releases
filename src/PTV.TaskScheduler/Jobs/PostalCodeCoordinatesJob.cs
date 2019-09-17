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
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    internal class PostalCodeCoordinatesJob : BaseJob
    {
        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            var jobData =
                QuartzScheduler.GetJobDataConfiguration<PostalCodeCoordinatesJobConfiguration>(context.JobDetail);
            var urlConfiguration = GetApplicationJobConfiguration<PostalCodeCoordinatesConfiguration>(context);
            
            if (jobData == null || urlConfiguration == null)
            {
                TaskSchedulerLogger.LogJobWarn(context.FireInstanceId, JobKey,
                    $"Url configuration for {JobKey} is not found.");
                return null;
            }

            var defaultProxy = context.Scheduler.Context.Get(QuartzScheduler.PROXY_SERVER_SETTINGS) as ProxyServerSettings;
            var proxySettings = defaultProxy.OverrideBy(urlConfiguration.ProxyServerSettings);

            jobData.UrlBase = urlConfiguration.UrlBase;
            
            return jobData.DownloadOneByOne
                ? ProcessOneByOne(proxySettings, serviceProvider, contextManager, jobData)
                : ProcessBatches(proxySettings, serviceProvider, contextManager, jobData);
        }

        /// <summary>
        /// Download data by moving through paged responses from geo.stat.fi. The server contains around 3000 postal
        /// codes, while our database has more than 4500 postal codes. This method is much faster than ProcessOneByOne,
        /// but cannot determine which postal codes were skipped.
        /// </summary>
        /// <param name="proxySettings"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="contextManager"></param>
        /// <param name="jobData"></param>
        /// <returns></returns>
        private string ProcessBatches(ProxyServerSettings proxySettings, IServiceProvider serviceProvider,
            IContextManager contextManager, PostalCodeCoordinatesJobConfiguration jobData)
        {
            var service = serviceProvider.GetService<IPostalCodeCoordinatesService>();
            var jobStatus = new StringBuilder();
            var downloadWatch = new Stopwatch();
            var importWatch = new Stopwatch();
            var downloadTime = TimeSpan.Zero;
            var importTime = TimeSpan.Zero;
            var batchStart = 0;
            var lastBatchSize = jobData.BatchSize;

            while (lastBatchSize == jobData.BatchSize)
            {
                Console.WriteLine($"Processing batch from {batchStart}");
            
                var batchCoordinates = Asyncs.HandleAsyncInSync(() => HttpClientWithProxy.UseAsync(proxySettings, async client =>
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Xml));

                    downloadWatch.Restart();
                    var response = await service.DownloadBatch(client, batchStart, jobData.BatchSize, new PostalCodeCoordinatesSettings()
                    {
                        BatchUrl = jobData.BatchUrl,
                        UrlBase = jobData.UrlBase
                    }, jobStatus);
                    downloadWatch.Stop();
                    downloadTime += downloadWatch.Elapsed;

                    return response;
                }));

                importWatch.Restart();
                contextManager.ExecuteWriter(unitOfWork => service.UpdateCoordinates(unitOfWork, batchCoordinates));
                importWatch.Stop();
                importTime += importWatch.Elapsed;

                lastBatchSize = batchCoordinates.Count;
                batchStart += lastBatchSize;
                
                Console.WriteLine($"Downloaded {lastBatchSize} coordinates.");
            }

            jobStatus.AppendLine($"Downloaded coordinates count {batchStart}");
            jobStatus.AppendLine($"Download time {downloadTime}");
            jobStatus.AppendLine($"Import time {importTime}");
            return jobStatus.ToString();
        }

        /// <summary>
        /// Download data by querying each individual postal code from our database. The geo.stat.fi server contains
        /// around 3000 postal codes, while our database has more than 4500 postal codes. This method is much slower
        /// than ProcessBatches, but 
        /// </summary>
        /// <param name="proxySettings"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="contextManager"></param>
        /// <param name="jobData"></param>
        /// <returns></returns>
        private string ProcessOneByOne(ProxyServerSettings proxySettings, IServiceProvider serviceProvider,
            IContextManager contextManager, PostalCodeCoordinatesJobConfiguration jobData)
        {
            var service = serviceProvider.GetService<IPostalCodeCoordinatesService>();
            var postalCodes = GetPostalCodes(contextManager);
            var jobStatus = new StringBuilder();
            var downloadWatch = new Stopwatch();
            var importWatch = new Stopwatch();
            var downloadTime = TimeSpan.Zero;
            var importTime = TimeSpan.Zero;
            var batchCounter = 1;
            var totalBatches = postalCodes.Count / jobData.BatchSize;
            var totalCount = 0;

            postalCodes.Batch(jobData.BatchSize).ForEach(batch =>
            {
                Console.WriteLine($"Processing batch {batchCounter} / {totalBatches}.");

                downloadWatch.Restart();
                var batchCoordinates = Asyncs.HandleAsyncInSync(() => HttpClientWithProxy.UseAsync(proxySettings, async client =>
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Xml));

                    downloadWatch.Restart();
                    var response = await service.DownloadCoordinates(client, batch, new PostalCodeCoordinatesSettings()
                    {
                        SingleUrl = jobData.SingleUrl,
                        UrlBase = jobData.UrlBase
                    }, jobStatus);
                    downloadWatch.Stop();
                    downloadTime += downloadWatch.Elapsed;

                    return response;
                }));
                downloadWatch.Stop();
                downloadTime += downloadWatch.Elapsed;
                totalCount += batchCoordinates.Count;

                importWatch.Restart();
                contextManager.ExecuteWriter(unitOfWork => service.UpdateCoordinates(unitOfWork, batchCoordinates));
                importWatch.Stop();
                importTime += importWatch.Elapsed;

                batchCounter++;
            });

            jobStatus.AppendLine($"Downloaded coordinates count {totalCount}");
            jobStatus.AppendLine($"Download time {downloadTime}");
            jobStatus.AppendLine($"Import time {importTime}");
            return jobStatus.ToString();
        }

        private List<VmPostalCode> GetPostalCodes(IContextManager contextManager)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var postalCodeRepo = unitOfWork.CreateRepository<IPostalCodeRepository>();
                return postalCodeRepo.All().Select(postalCode => new VmPostalCode
                {
                    Id = postalCode.Id,
                    Code = postalCode.Code
                }).ToList();
            });
        }
    }
}