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
using PTV.Framework;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class PostalCodesJob : BaseJob, IJob
    {
        public override JobTypeEnum JobType => JobTypeEnum.PostalCodes;

        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<UrlJobDataConfiguration>(context.JobDetail);
            var applicationKapaConfiguration = GetKapaConfiguration<ApplicationKapaConfiguration>(context);
 			var url = ParseKapaConfiguration(jobData.Url, applicationKapaConfiguration);
            var content = Download(url, applicationKapaConfiguration); // roughly 2.23MB
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var municipalities = unitOfWork.CreateRepository<IMunicipalityRepository>().All().ToDictionary(m => m.Code, m => m.Id);
                var downloadedPostalCodes = Parse(content, municipalities);

                var postalCodeRep = unitOfWork.CreateRepository<IPostalCodeRepository>();
                
                string userName = unitOfWork.GetUserNameForAuditing(SaveMode.AllowAnonymous);
                postalCodeRep.BatchUpdate(new PostalCode(), ot => ot.IsValid, null, null, userName);
                // add new postal codes and update existing
                var vmToEntity = serviceProvider.GetRequiredService<ITranslationViewModel>();
                vmToEntity.TranslateAll<VmJsonPostalCode, PostalCode>(downloadedPostalCodes, unitOfWork);

                // save
                unitOfWork.Save(SaveMode.AllowAnonymous, userName: context.JobDetail.Key.Name);
                return $"Downloaded: {downloadedPostalCodes.Count}, invalidated: {postalCodeRep.All().Count(x => !x.IsValid)}.";
            });
        }

        private IList<VmJsonPostalCode> Parse(JArray pcEntries, Dictionary<string, Guid> municipalities)
        {
            return pcEntries.Select(pcEntry => new VmJsonPostalCode
            {
                Code = ParseCode(pcEntry),
                MunicipalityId = ParseMunicipalityId(pcEntry, municipalities),
                Names = ParseNames(pcEntry),
                IsValid = true
            }).ToList();
        }

        private static Guid? ParseMunicipalityId(JToken token, Dictionary<string, Guid> municipalities)
        {
            var municipalityCode = ParseMunicipalityCodeUrl(token.Value<JObject>("municipality")?.GetValue("url")?.ToObject<string>());
            return municipalityCode != null && municipalities.ContainsKey(municipalityCode)
                ? municipalities[municipalityCode]
                : (Guid?) null;
        }

        private static string ParseMunicipalityCodeUrl(string url)
        {
            if (url.IsNullOrEmpty()) return null;
            var uri = new Uri(url);
            return uri.Segments.Last().Trim('/');
        }
    }
}
