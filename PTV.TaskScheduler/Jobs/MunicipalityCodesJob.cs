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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class MunicipalityCodesJob : BaseJob, IJob
    {
        public override JobTypeEnum JobType => JobTypeEnum.MunicipalityCodes;
       
        public Task Execute(IJobExecutionContext context)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<MunicipalityCodeJobDataConfiguration>(context.JobDetail);
            var serviceProvider = context.Scheduler.Context.Get(QuartzScheduler.SERVICE_PROVIDER) as IServiceProvider;

            ExecuteInternal(unitOfWork =>
            {

                var content = Download(jobData.Url, GetKapaConfiguration(context)); // roughly 300k
                var downloadedMunicipalityCodes = Parse(content);

                var existingMunicipalityCodes = unitOfWork.CreateRepository<IMunicipalityRepository>()
                    .All()
                    .Include(a => a.MunicipalityNames).ThenInclude(b => b.Localization)
                    .ToList();

                // add new postal codes and update existing
                var vmToEntity = serviceProvider.GetRequiredService<ITranslationViewModel>();
                vmToEntity.TranslateAll<VmJsonMunicipality, Municipality>(downloadedMunicipalityCodes, unitOfWork);

                // invalidate not exist postal codes
                var municipalityCodesToInvalidate = existingMunicipalityCodes.Where(em => !downloadedMunicipalityCodes.Select(a => a.MunicipalityCode).Contains(em.Code)).ToList();
                municipalityCodesToInvalidate.ForEach(m => m.IsValid = false);

                unitOfWork.Save(SaveMode.AllowAnonymous, userName: context.JobDetail.Key.Name);
            }, context);
            return Task.CompletedTask;
        }

        private IList<VmJsonMunicipality> Parse(JArray municipalities)
        {
            return municipalities.Select(municipality => new VmJsonMunicipality
            {
                MunicipalityCode = ParseCode(municipality),
                Names = ParseNames(municipality),
                IsRemoved = false
            }).ToList();
        }
    }
}