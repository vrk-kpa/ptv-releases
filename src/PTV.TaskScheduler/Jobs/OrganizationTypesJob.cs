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
using PTV.Framework;
using PTV.TaskScheduler.Configuration;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    /// <summary>
    /// OrganizationType FINTO job
    /// </summary>
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class OrganizationTypesJob : YPlatformCodesJob
    {
        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager,
            VmJobSummary jobSummary)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<UrlJobDataConfiguration>(context.JobDetail);
            var yPlatformConfiguration = GetApplicationJobConfiguration<YPlatformConfiguration>(context);

            var urlBase = string.Format(jobData.Url, yPlatformConfiguration.UrlBase, yPlatformConfiguration.ApiVersion,
                yPlatformConfiguration.OrganizationType);
            if (!IsConfigurationValid(urlBase, yPlatformConfiguration.OrganizationType, yPlatformConfiguration))
            {
                return $"{nameof(OrganizationTypesJob)} has invalid configuration, tested URL {urlBase}.";
            }

            var organizationTypes = DownloadCodes(urlBase, yPlatformConfiguration);
            var invalidated = ImportEntities(organizationTypes, context, serviceProvider, contextManager);
            var rearranged = RearrangeParents<OrganizationType>(organizationTypes, context, contextManager, x => x.YCode);
            
            return $"Downloaded: {organizationTypes.Count}, invalidated: {invalidated}, rearranged: {rearranged}.";
        }
        
        private int ImportEntities(List<VmYCodeData> yCodes, IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var existingCodes = unitOfWork.CreateRepository<IRepository<OrganizationType>>()
                    .All()
                    .ToList();

                // add new codes and update existing which are still valid
                var vmToEntity = serviceProvider.GetRequiredService<ITranslationViewModel>();
                vmToEntity.TranslateAll<VmYCodeData, OrganizationType>(yCodes, unitOfWork);

                // invalidate not existing codes
                var codesToInvalidate = existingCodes.Where(ec => !yCodes.Select(a => a.CodeValue).Contains(ec.YCode)).ToList();
                codesToInvalidate.ForEach(m => m.IsValid = false);

                unitOfWork.Save(SaveMode.AllowAnonymous, userName: context.JobDetail.Key.Name);
                return codesToInvalidate.Count;
            });
        }
    }
}
