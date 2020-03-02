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
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Interfaces;
using PTV.TaskScheduler.Models;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    /// <summary>
    /// Handle codes of area types
    /// </summary>
    internal abstract class AreaCodesJob : BaseJob
    {
        private AreaTypeEnum AreaType { get; }

        protected AreaCodesJob(AreaTypeEnum areaType)
        {
            AreaType = areaType;
        }

        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<AreaCodeJobDataConfiguration>(context.JobDetail);

            var typesCache = serviceProvider.GetRequiredService<ITypesCache>();
            var areaTypeId = typesCache.Get<AreaType>(AreaType.ToString());

			var kapaConfiguration = GetKapaConfiguration<ApplicationKapaConfiguration>(context);
            var url = ParseKapaConfiguration(jobData.Url, kapaConfiguration);
            var content = Download(url, kapaConfiguration);
            var downloadedAreas = Parse(content).ToList();

            return contextManager.ExecuteWriter(unitOfWork =>
            {

                var existingAreas = unitOfWork.CreateRepository<IAreaRepository>()
                    .All()
                    .Where(a => a.AreaTypeId == areaTypeId/* && a.IsValid*/)
                    .Include(a => a.AreaNames).ThenInclude(b => b.Localization)
                    .Include(a => a.AreaMunicipalities).ThenInclude(m => m.Municipality)
                    .ToList();

                var vmToEntity = serviceProvider.GetRequiredService<ITranslationViewModel>();

                // get municipalities from repository
                var municipalities = unitOfWork.CreateRepository<IMunicipalityRepository>().All().ToDictionary(m => m.Code, m => m.Id);

                var missingAreaCodes = downloadedAreas.Where(pc => !existingAreas.Select(epc => epc.Code).Contains(pc.Code)).ToList();
                if (missingAreaCodes.Any())
                {

                    // add missing municipalities
                    missingAreaCodes.ForEach(mpc =>
                    {
                        mpc.Municipalities = GetRegionMunicipalities(jobData.MunicipalityUrl, kapaConfiguration, mpc.Code);
                        mpc.Municipalities.ForEach(m =>
                        {
                            mpc.AreaTypeId = areaTypeId;
                            var municipalityId = municipalities.ContainsKey(m) ? municipalities[m] : (Guid?)null;
                            if (municipalityId == null)
                            {
                                throw new Exception($"Municipality of code: {m} doesn't exist.");
                            }
                            mpc.AreaMunicipalities.Add(new VmJsonAreaMunicipality { MunicipalityId = municipalityId.Value });
                        });
                    });

                    var rep = unitOfWork.CreateRepository<IAreaRepository>();
                    var toSave = vmToEntity.TranslateAll<VmJsonArea, Area>(missingAreaCodes, unitOfWork);
                    toSave.ForEach(a =>
                    {
                        a.IsValid = true;
                        rep.Add(a);
                    });
                }

                // invalidate not exist areas
                var downloadedAreaCodes = downloadedAreas.Select(a => a.Code).ToList();
                var areasToInvalidate = existingAreas.Where(ea => !downloadedAreaCodes.Contains(ea.Code));
                areasToInvalidate.ForEach(a => a.IsValid = false);

                // validate existing areas
                var areasToValidate = existingAreas.Where(ea => !ea.IsValid && downloadedAreaCodes.Contains(ea.Code)).ToList();
                areasToValidate.ForEach(a => a.IsValid = true);

                // handle names
                var nameRep = unitOfWork.CreateRepository<IAreaNameRepository>();
                var namesToAdd = HandleCodeNames(unitOfWork, nameRep, existingAreas.Where(pc => pc.IsValid), downloadedAreas.OfType<IJsonCodeNamesItem>().ToList());
                namesToAdd.ForEach(nta =>
                {
                    var itemName = vmToEntity.Translate<VmJsonName, AreaName>(nta, unitOfWork);
                    if (!nta.OwnerReferenceId.HasValue) return;
                    itemName.AreaId = nta.OwnerReferenceId.Value;
                    nameRep.Add(itemName);
                });

                // handle municipalities
                var areaMunicipalityRep = unitOfWork.CreateRepository<IAreaMunicipalityRepository>();
                existingAreas.ForEach(ea =>
                {

                    var existingMunicipalities = ea.AreaMunicipalities.ToList();
                    var downloadedMunicipalities = ParseMunicipalities(Download(ParseKapaConfigurationForMunicipality(jobData.MunicipalityUrl, kapaConfiguration, ea.Code), kapaConfiguration));

                    // municipalities to delete
                    var municipalitiesToDelete = existingMunicipalities.Where(em => !downloadedMunicipalities.Contains(em.Municipality?.Code));
                    municipalitiesToDelete.ForEach(am =>
                    {
                        areaMunicipalityRep.Remove(am);
                    });

                    // municipalities to add
                    var municipalitiesToAdd = downloadedMunicipalities.Where(dm => !existingMunicipalities.Select(m => m.Municipality?.Code).Contains(dm));
                    municipalitiesToAdd.ForEach(m =>
                    {
                        if (!municipalities.ContainsKey(m)) return;
                        areaMunicipalityRep.Add(new AreaMunicipality
                        {
                            AreaId = ea.Id,
                            MunicipalityId = municipalities[m]
                        });
                    });
                });

                // save
                unitOfWork.Save(SaveMode.AllowAnonymous, userName: context.JobDetail.Key.Name);
                return $"Downloaded: {downloadedAreaCodes.Count}, invalidated: {areasToInvalidate.Count()}.";
            });
        }

        private IEnumerable<VmKapaJsonArea> Parse(JArray areaCodes)
        {
            return areaCodes.Select(entry => new VmKapaJsonArea
            {
                Code = ParseCode(entry),
                Names = ParseNames(entry),
                IsValid = true
            }).ToList();
        }

        private IList<string> ParseMunicipalities(JArray municipalityCodes)
        {
            return municipalityCodes.Select(ParseCode).ToList();
        }

        private List<string> GetRegionMunicipalities(string municipalityUrl, ApplicationKapaConfiguration kapaConfiguration, string areaCode)
        {
            var url = ParseKapaConfigurationForMunicipality(municipalityUrl, kapaConfiguration, areaCode);
            var content = Download(url, kapaConfiguration);
            return content.Select(ParseCode).Where(code => !code.IsNullOrEmpty()).ToList();
        }

        private static string ParseKapaConfigurationForMunicipality(string url, ApplicationKapaConfiguration kapaConfiguration, string areaCode)
        {
            if (kapaConfiguration == null) throw new Exception("Kapa configuration is not set");
            return string.Format(url, kapaConfiguration.UrlBase.TrimEnd('/'), kapaConfiguration.Version, areaCode, kapaConfiguration.ApiKey);
        }

    }
}
