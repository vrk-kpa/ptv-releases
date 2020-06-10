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
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.YPlatform;
using PTV.Framework;
using PTV.TaskScheduler.Configuration;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    /// <summary>
    /// Handle codes of area types
    /// </summary>
    internal abstract class AreaCodesJob : BaseJob
    {
        protected const string CodesEndpoint = "/codes/";
        protected const string VersionsEndpoint = "/versions/";

        protected List<VmYCodedArea> DownloadAreas(string urlBase, YPlatformConfiguration yPlatformConfiguration)
        {
            var codesUrls = urlBase + CodesEndpoint;
            var content = DownloadWithToken(codesUrls, yPlatformConfiguration);
            return JsonConvert.DeserializeObject<VmYResponse<VmYCodedArea>>(content)?.Results ?? new List<VmYCodedArea>();
        }

        protected Dictionary<string, List<string>> DownloadRelations(string relationsUrl, YPlatformConfiguration yPlatformConfiguration)
        {
            var content = DownloadWithToken(relationsUrl, yPlatformConfiguration);
            var relations = JsonConvert.DeserializeObject<VmYResponse<VmYRelation>>(content)?.Results ?? new List<VmYRelation>();
            return relations
                .Where(x => x.RelatedMember != null)
                .Select(x => new {Municipality = x.Code.CodeValue, Area = x.RelatedMember.Code.CodeValue})
                .GroupBy(x => x.Area)
                .ToDictionary(x => x.Key, x => x.Select(y => y.Municipality).ToList());
        }

        protected string ImportAreas(List<VmYCodedArea> validAreas, IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            var typesCache = serviceProvider.GetService<ITypesCache>();
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var existingCodes = unitOfWork.CreateRepository<IAreaRepository>()
                    .All()
                    .ToList();
                var areaTypeId = typesCache.Get<AreaType>(validAreas.First().AreaType.ToString());

                // add new postal codes and update existing which are still valid
                var vmToEntity = serviceProvider.GetRequiredService<ITranslationViewModel>();
                vmToEntity.TranslateAll<VmYCodedArea, Area>(validAreas, unitOfWork);

                // invalidate not exist postal codes
                var codesToInvalidate = existingCodes.Where(em =>
                    !validAreas.Select(a => a.CodeValue).Contains(em.Code) && em.AreaTypeId == areaTypeId).ToList();
                codesToInvalidate.ForEach(m => m.IsValid = false);

                unitOfWork.Save(SaveMode.AllowAnonymous, userName: context.JobDetail.Key.Name);
                return $"Downloaded: {validAreas.Count}, invalidated: {codesToInvalidate.Count}.";
            });
        }

        protected string ImportRelations(List<VmYCodedArea> validAreas, Dictionary<string, List<string>> relations, IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            var typesCache = serviceProvider.GetService<ITypesCache>();
            
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var relationsRepo = unitOfWork.CreateRepository<IAreaMunicipalityRepository>();
                var municipalityRepo = unitOfWork.CreateRepository<IMunicipalityRepository>();
                var areaRepo = unitOfWork.CreateRepository<IAreaRepository>();
                var addedSum = 0;
                var removedSum = 0;
                
                foreach (var vmArea in validAreas)
                {
                    var areaTypeId = typesCache.Get<AreaType>(vmArea.AreaType.ToString());
                    var importedRelations = relations.TryGetOrDefault(vmArea.CodeValue);
                    var dbArea = areaRepo.All()
                        .Include(x => x.AreaMunicipalities).ThenInclude(x => x.Municipality)
                        .FirstOrDefault(x => x.Code == vmArea.CodeValue && x.AreaTypeId == areaTypeId);
                    var dbMunicipalityCodes = dbArea.AreaMunicipalities.Select(x => x.Municipality.Code).ToList();

                    var newRelations = importedRelations.Where(x => !dbMunicipalityCodes.Contains(x)).ToList();
                    var municipalities = municipalityRepo.All().Where(x => newRelations.Contains(x.Code));
                    var oldRelations = dbArea.AreaMunicipalities.Where(x => !importedRelations.Contains(x.Municipality.Code)).ToList();

                    foreach (var municipality in municipalities)
                    {
                        var relation = new AreaMunicipality {Municipality = municipality, Area = dbArea};
                        relationsRepo.Add(relation);
                    }
                    
                    relationsRepo.Remove(oldRelations);
                    unitOfWork.Save(SaveMode.AllowAnonymous, userName: context.JobDetail.Key.Name);

                    addedSum += newRelations.Count;
                    removedSum += oldRelations.Count;
                }

                return $"Added relations: {addedSum}, removed relations: {removedSum}.";
            });
        }

        protected bool IsConfigurationValid(string urlBase, string testedVersion, YPlatformConfiguration yPlatformConfiguration)
        {
            var versionsUrl = urlBase + VersionsEndpoint;
            var content = DownloadWithToken(versionsUrl, yPlatformConfiguration);
            var versionInfo = JsonConvert.DeserializeObject<VmYResponse<VmYVersion>>(content);
            var latestVersion = versionInfo?.Results?.FirstOrDefault();

            return latestVersion != null 
                   && latestVersion.CodeValue == testedVersion 
                   && latestVersion.EndDate >= DateTime.Today;
        }
        
        private string DownloadWithToken(string url, YPlatformConfiguration configuration)
        {
            return PtvHttpClient.Use(client =>
            {
                if (JobSchedulingConfiguration.Timeout > 0)
                {
                    var timeout = new TimeSpan(0, 0, JobSchedulingConfiguration.Timeout);
                    client.Timeout = timeout;
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration.Token);
                return client.GetStringAsync(url).GetAwaiter().GetResult();
            });
        }
        
        protected List<VmYCodedArea> GetValidAreas(List<VmYCodedArea> municipalities, AreaTypeEnum areaType)
        {
            return municipalities.Where(x => x.IsValid)
                .Select(x =>
                {
                    x.AreaType = areaType;
                    return x;
                })
                .ToList();
        }
    }
}
