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
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class LanguageCodesJob : BaseJob, IJob
    {
        private const string ResourceFileName = @"languageCodes.json";
        private const string DefaultLanguageCode = "fi";

        private readonly List<string> forDataLanguages = new List<string> { "en", "fi", "sv" };

        public override JobTypeEnum JobType => JobTypeEnum.LanguageCodes;

        public Task Execute(IJobExecutionContext context)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<UrlJobDataConfiguration>(context.JobDetail);

            ExecuteInternal(unitOfWork =>
            {
                var url = ParseKapaConfiguration(jobData.Url, GetKapaConfiguration(context));
                var content = Download(url); // roughly 20.55kb

                var languageRep = unitOfWork.CreateRepository<ILanguageRepository>();
                var languageNameRep = unitOfWork.CreateRepository<ILanguageNameRepository>();

                var existingLanguages = languageRep.All().ToDictionary(lang => lang.Code.ToLower(), lang =>
                {
                    lang.IsValid = false;
                    return lang;
                });
                var maxLanguageOrderNumber = existingLanguages.Values.Max(l => l.OrderNumber);

                var languageOrders = GetLanguageOrders();

                foreach (var language in content)
                {
                    var code = ParseCode(language).ToLower();
                    var newLanguage = false;
                    if (!existingLanguages.TryGetValue(code, out var languageEntity))
                    {
                        newLanguage = true;

                        languageEntity = new Language
                        {
                            OrderNumber = languageOrders != null && languageOrders.TryGetValue(code, out var orderNumber) ? orderNumber : ++maxLanguageOrderNumber
                        };

                        languageRep.Add(languageEntity);
                        existingLanguages.Add(code, languageEntity);
                    }

                    languageEntity.Code = code;
                    languageEntity.IsForData = forDataLanguages.Contains(code);
                    languageEntity.IsDefault = code == DefaultLanguageCode.ToLower();
                    languageEntity.IsValid = true;

                    // handle names
                    var names = ParseNames(language);
                    var languageNames = languageNameRep.All().Include(l => l.Language).Include(l => l.Localization);
                    foreach (var languageName in names)
                    {
                        if (!existingLanguages.TryGetValue(languageName.Language, out var localization)) continue;
                        var languageNameEntity = newLanguage ? null : languageNames.SingleOrDefault(l => l.Language.Code.ToLower() == code && l.Localization.Code == localization.Code);
                        if (languageNameEntity == null)
                        {
                            languageNameEntity = new LanguageName {Localization = localization, Language = languageEntity};
                            languageNameRep.Add(languageNameEntity);
                        }

                        languageNameEntity.Name = languageName.Name;
                    }
                }

                // save
                unitOfWork.Save(SaveMode.AllowAnonymous, userName: context.JobDetail.Key.Name);
            }, context);
            return Task.CompletedTask;
        }

        private static Dictionary<string, int> GetLanguageOrders()
        {
            var path = Path.Combine(AppContext.BaseDirectory, ResourceFileName);
            return File.Exists(path)
                ? JsonConvert.DeserializeObject<List<VmLanguageCode>>(File.ReadAllText(path)).ToDictionary(l => l.Code.ToLower(), l => l.Order)
                : null;
        }
    }
}
