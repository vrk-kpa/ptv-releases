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
using PTV.Domain.Logic.Utilities;
using PTV.Domain.Model.Models;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using Quartz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class LanguageCodesJob : BaseJob, IJob
    {
        private const string ResourceFileName = @"languageCodes.json";
        private const string DefaultLanguageCode = "fi";

        //private readonly List<string> forDataLanguages = new List<string> { "en", "fi", "sv" };

        public override JobTypeEnum JobType => JobTypeEnum.LanguageCodes;

        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<LanguageJobDataConfiguration>(context.JobDetail);
            var cultureUtility = serviceProvider.GetService<LanguageStateCultureUtility>();
            var applicationKapaConfiguration = GetKapaConfiguration<ApplicationKapaConfiguration>(context);
            var url = ParseKapaConfiguration(jobData.Url, applicationKapaConfiguration);
            var content = Download(url, applicationKapaConfiguration); // roughly 20.55kb
            var forDataLanguages = jobData.AllowedForData;
            var forTranslationLanguages = jobData.AllowedForTranslation;
            var languageOrders = GetLanguageOrders();
            var maxLanguageOrderNumberJobSetting = languageOrders.Values.Max();
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var languageRep = unitOfWork.CreateRepository<ILanguageRepository>();
                var languageNameRep = unitOfWork.CreateRepository<ILanguageNameRepository>();
                
                var existingLanguages = languageRep.All().ToList().ToDictionary(lang => lang.Code.ToLower(), lang =>
                {
                    lang.IsValid = false;
                    return lang;
                });
                
                var maxLanguageOrderNumber = existingLanguages.Values.Max(l => l.OrderNumber);
                var maxOrderNumber = maxLanguageOrderNumber > maxLanguageOrderNumberJobSetting
                    ? maxLanguageOrderNumber
                    : maxLanguageOrderNumberJobSetting;
                

                foreach (var language in content)
                {
                    var code = ParseCode(language).ToLower();
                    var newLanguage = false;
                    if (!existingLanguages.TryGetValue(code, out var languageEntity))
                    {
                        newLanguage = true;

                        languageEntity = new Language
                        {
                            OrderNumber = languageOrders != null && languageOrders.TryGetValue(code, out var orderNumber) ? orderNumber : ++maxOrderNumber
                        };

                        languageRep.Add(languageEntity);
                        existingLanguages.Add(code, languageEntity);
                    }

                    languageEntity.Code = code;
                    languageEntity.IsForData = forDataLanguages.Contains(code);
                    languageEntity.IsForTranslation = forTranslationLanguages.Contains(code);
                    languageEntity.IsDefault = code == DefaultLanguageCode.ToLower();
                    languageEntity.IsValid = true;
                    if (string.IsNullOrEmpty(languageEntity.LanguageStateCulture))
                    {
                        languageEntity.LanguageStateCulture = cultureUtility.GetLanguageStateCulture(code);
                    }

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
                return $"Downloaded: {existingLanguages.Count(x => x.Value.IsValid)}, invalidated: {existingLanguages.Count(x => !x.Value.IsValid)}.";

            });
        }

        private static Dictionary<string, int> GetLanguageOrders()
        {
            var path = Path.Combine(AppContext.BaseDirectory, ResourceFileName);
            return File.Exists(path)
                ? JsonConvert.DeserializeObject<List<VmLanguage>>(File.ReadAllText(path)).ToDictionary(l => l.Code.ToLower(), l => l.Order)
                : null;
        }
    }
}
