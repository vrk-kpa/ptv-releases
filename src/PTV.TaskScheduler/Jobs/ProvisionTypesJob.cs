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
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Jobs;
using PTV.Domain.Model.Models.YPlatform;
using PTV.TaskScheduler.Configuration;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    /// <summary>
    /// ProvisionType FINTO job
    /// </summary>
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class ProvisionTypesJob : YPlatformCodesJob
    {
        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager,
            VmJobSummary jobSummary)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<UrlJobDataConfiguration>(context.JobDetail);
            var yPlatformConfiguration = GetApplicationJobConfiguration<YPlatformConfiguration>(context);

            var urlBase = string.Format(jobData.Url, yPlatformConfiguration.UrlBase, yPlatformConfiguration.ApiVersion,
                yPlatformConfiguration.ProvisionType);
            if (!IsConfigurationValid(urlBase, yPlatformConfiguration.ProvisionType, yPlatformConfiguration))
            {
                return $"{nameof(ProvisionTypesJob)} has invalid configuration, tested URL {urlBase}.";
            }

            var provisionTypes = DownloadCodes(urlBase, yPlatformConfiguration);
            ImportEntities(provisionTypes, context, serviceProvider, contextManager);
            var rearranged = RearrangeParents<ProvisionType>(provisionTypes, context, contextManager, x => x.YCode);
            
            return $"Downloaded: {provisionTypes.Count}, invalidated: {0}, rearranged: {rearranged}.";
        }
        
        private void ImportEntities(List<VmYCodeData> yCodes, IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                // add new codes and update existing which are still valid
                var vmToEntity = serviceProvider.GetRequiredService<ITranslationViewModel>();
                vmToEntity.TranslateAll<VmYCodeData, ProvisionType>(yCodes, unitOfWork);

                unitOfWork.Save(SaveMode.AllowAnonymous, userName: context.JobDetail.Key.Name);
            });
        }
    }
}
