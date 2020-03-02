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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Finto;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{

    internal abstract class FintoDataJob<TEntity> : BaseJob
        where TEntity : IFintoItemBase, new()
    {
        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<FintoDataJobDataConfiguration>(context.JobDetail);

            var urlConfiguration = GetKapaConfiguration<ApplicationFintoKapaConfiguration>(context);
            if (urlConfiguration == null)
            {
                TaskSchedulerLogger.LogJobWarn(context.FireInstanceId, JobKey,
                    $"Url configuration for {JobKey} is not found.");
                return $"Url configuration for {JobKey} is not found.";
            }

            var environment = serviceProvider.GetService<IWebHostEnvironment>();

            var settings = FintoJobExtension.GetSettings(jobData.DownloadType, environment);
            settings.Params["baseUri"] = urlConfiguration.UrlBase;
            settings.QueryParams["apikey"] = urlConfiguration.ApiKey;
            var typeSettings = FintoJobExtension.SetDownloadDefinition(settings, jobData.DownloadType);

            var downloadFintoTask = serviceProvider.GetService<DownloadFintoDataTask>();
            downloadFintoTask.Init(ProxyDownload, urlConfiguration);
            downloadFintoTask.DownloadData(settings);
            if (typeSettings.Content == null)
            {
                return "No data downloaded";
            }

            var service = serviceProvider.GetService<IFintoImportService<TEntity>>();
            service?.SeedFintoItems(typeSettings.Content, context.JobDetail.Key.Name);
            return $"Downloaded data for {JobKey}: {typeSettings.Content.Count}";
        }
    }
}
