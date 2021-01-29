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
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Domain.Model.Models.Jobs;
using PTV.ExternalData.Pharmacy;
using PTV.Framework;
using PTV.Framework.Extensions;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    /// <summary>
    /// TargetGroup FINTO job
    /// </summary>
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class PharmacyImportJob : BaseJob
    {
        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager,
            VmJobSummary jobSummary)
        {
            var configuration = serviceProvider.GetService<ApplicationConfiguration>();
            var languageCache = serviceProvider.GetService<ILanguageCache>();
            var pharmacyConfiguration = configuration?.GetPharmacyImportConfiguration();
            
            if (pharmacyConfiguration?.ApiUserName?.IsNullOrWhitespace() ?? true)
            {
                return $"{nameof(PharmacyImportJob)} has invalid configuration."; 
            }

            var secret = configuration.GetPharmacyImportConfiguration().GetSecret();
            var enumCache = CreateEnumCache(contextManager, languageCache);
            var builder = new StringBuilder();
            try
            {
                var result = Importer.import(secret, enumCache, builder).GetAwaiter().GetResult();
                return result;
            }
            catch (Exception ex)
            {
                builder.Append(ex.FlattenWithInnerExceptions());
                return builder.ToString();
            }
        }

        private EnumCache CreateEnumCache(IContextManager contextManager, ILanguageCache languageCache)
        {
            var finnish = languageCache.Get("fi");   
            var languages = new Dictionary<string, string>();
            var postalCodeMunicipalities = new Dictionary<string, List<string>>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var languageNameRepo = unitOfWork.CreateRepository<ILanguageNameRepository>();
                languages = languageNameRepo.All()
                    .Where(x => x.LocalizationId == finnish)
                    .Include(x => x.Language)
                    .Select(x => new {x.Name, x.Language.Code})
                    .Distinct()
                    .ToDictionary(x => x.Name, x => x.Code);

                var postalCodeNameRepo = unitOfWork.CreateRepository<IPostalCodeNameRepository>();
                postalCodeMunicipalities = postalCodeNameRepo.All()
                    .Where(x => x.LocalizationId == finnish && x.PostalCode.IsValid)
                    .Include(x => x.PostalCode).ThenInclude(x => x.Municipality)
                    .Select(x => new {x.Name, x.PostalCode.Municipality.Code})
                    .ToList()
                    .GroupBy(x => x.Name)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Code).ToList());
            });
            return new EnumCache(languages, postalCodeMunicipalities);
        }
    }
}
