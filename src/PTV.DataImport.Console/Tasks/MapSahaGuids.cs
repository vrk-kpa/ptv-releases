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
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using PTV.DataImport.ConsoleApp.Services;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PTV.DataImport.ConsoleApp.Models;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.ExternalSources;
using PTV.Framework;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    public class MapSahaGuids
    {
        private static readonly string PostalCodesStartDataFile = Path.Combine("ImportSources", "PCF_20170407.dat");

        private static readonly string PostalCodesGeneratedFile = Path.Combine("Generated", "PostalCode.json");

        private IServiceProvider _serviceProvider;
        
        public MapSahaGuids(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;
        }

        public void RemoveOldMappings()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedMigrationService = serviceScope.ServiceProvider.GetService<IMigrationService>();
                scopedMigrationService.RemoveOldSahaGuids();
            }
        }
        
        public void MapSahaGuidsTemp()
        {
            var resourceManager = new ResourceManager();
            var sahaGuids = resourceManager.GetDesrializedJsonResource<List<VmTempSahaGuids>>(JsonResources.TempSahaMapping);
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var sahaOrgRep = unitOfWork.CreateRepository<ISahaOrganizationInformationRepository>();
                    var allNewSahaMapping = sahaOrgRep.All()
                        .Where(x => sahaGuids.Select(y => y.NewSahaGuid).Contains(x.SahaId)).ToList();
                    
                    var allOldSahaMapping = sahaOrgRep.All()
                        .Where(x => sahaGuids.Select(y => y.OldSahaGuid).Contains(x.SahaId) && !allNewSahaMapping.Select(y => y.OrganizationId).Contains(x.OrganizationId)).ToList();

                    if (sahaGuids.Count != (allOldSahaMapping.Count + allNewSahaMapping.Count))
                    {
                        var missingMappingInPtv = sahaGuids.Where(x => !allNewSahaMapping.Select(y => y.SahaId).Contains(x.NewSahaGuid)).Where(x => !allOldSahaMapping.Select(y => y.SahaId).Contains(x.OldSahaGuid)).ToList();
                        Console.WriteLine();
                        missingMappingInPtv.ForEach(x => Console.WriteLine(@"{0};{1};{2}", x.Name, x.OldSahaGuid.ToString(), x.NewSahaGuid.ToString()));
                    }
                    
                    var dateToSave = DateTime.UtcNow;
                    var operationId = dateToSave.ToString("O") + "-" + Guid.NewGuid();
                    
                    var newMapping = allOldSahaMapping.Join(sahaGuids, f => f.SahaId, s => s.OldSahaGuid, (f, s) =>
                       new SahaOrganizationInformation()
                       {
                           OrganizationId = f.OrganizationId,
                           SahaId = s.NewSahaGuid,
                           SahaParentId = s.NewSahaGuid,
                           Name = f.Name,
                           LastOperationId = operationId
                       }
                    ).ToList();

                    if (!newMapping.Any()) return;
                    Console.WriteLine("Adding duplicates with new saha guid");
                    sahaOrgRep.BatchInsert(newMapping, "PTVapp");
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
        }
        
        /// <summary>
        /// Generates a list of postal codes in JSON format to applications file generation folder.
        /// </summary>
        public void MapGuids()
        {
            if (File.Exists(@"sahaGuids.txt"))
            {
                var lines = File.ReadAllText(@"sahaGuids.txt");
                var sahaItmes = JsonConvert.DeserializeObject<List<VmJsonSahaGuids>>(lines);
                if (sahaItmes.Any(x => string.IsNullOrEmpty(x.BusinessId) || x.BusinessId.IsNullOrWhitespace()))
                {
                    Console.WriteLine("There are items without business code in the input json.");
                }

                var loadedCount = sahaItmes.Count;
                Console.WriteLine($"Count of loaded saha items {loadedCount}");
                sahaItmes = sahaItmes.Where(x => x.AccountData.Country.Code == "fi").ToList();
                var filteredForFiCode = sahaItmes.Count;
                var languageCache = _serviceProvider.GetRequiredService<ILanguageCache>();
                var typesCache = _serviceProvider.GetRequiredService<ITypesCache>();
                var alternateNameType = typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString());
                var nameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                var publishedTypeID = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                var organizationsWithBusinessCode = _serviceProvider.GetRequiredService<IOrganizationVersionedRepository>().All().Include(x => x.Business).Include(x => x.OrganizationNames).Where(x => x.BusinessId.HasValue && x.ParentId == null && x.PublishingStatusId == publishedTypeID).ToList();
                var allOrganizationsNames = _serviceProvider.GetRequiredService<IOrganizationNameRepository>().All().Include(x => x.OrganizationVersioned).Where(x => x.OrganizationVersioned.PublishingStatusId == publishedTypeID).ToList();

                
                var sahaItemsWithBusinessCode = sahaItmes.Where(x => !string.IsNullOrEmpty(x.BusinessId)).ToList();
                var sahaItemsWithoutBusinessCode = sahaItmes.Where(x => x.BusinessId.IsNullOrEmpty() || x.BusinessId.IsNullOrWhitespace()).ToList();
                var foundInPTV = sahaItemsWithBusinessCode.Join(organizationsWithBusinessCode, x => x.BusinessId, y => y.Business.Code, (sI, org) =>
                    new VmSahaPtvMap
                    {
                        PtvOrgName = org.OrganizationNames.FirstOrDefault(x => x.LocalizationId == languageCache.Get(sI.AccountData.Country.Code) && x.TypeId == nameType)?.Name ?? string.Empty,
                        PtvOrgAltName = org.OrganizationNames.FirstOrDefault(x => x.LocalizationId == languageCache.Get(sI.AccountData.Country.Code) && x.TypeId == alternateNameType)?.Name ?? string.Empty,
                        ONames = org.OrganizationNames.Select(x => new Tuple<Guid, string>(x.LocalizationId, x.Name)).ToList(),
                        SahaOrgName = sI.OrganizationName,
                        SahaId = sI.Id,
                        PtvUnificRootId = org.UnificRootId,
                        PtvBusinessCode = org.Business.Code
                    }).DistinctBy(x => x.PtvUnificRootId).ToList();
                var checkedNames =
                    foundInPTV.Where(x => x.SahaOrgName == x.PtvOrgName || x.SahaOrgName == x.PtvOrgAltName).ToList();
                var g = checkedNames.GroupBy(x => new {x.SahaId, x.PtvBusinessCode}).Where(x => x.Count() > 1).ToList();
                
                var foundNameAndBcodeEqual = checkedNames.DistinctBy(x => x.PtvBusinessCode, x => x.SahaId).ToList();

                var sahaItemsNotFoundInPtv =
                    sahaItmes.Where(x => !foundNameAndBcodeEqual.Select(y => y.SahaId).Contains(x.Id)).ToList();

                var sahaItemsNotFoundNames =
                    sahaItemsNotFoundInPtv.Where(x => allOrganizationsNames.Select(y => y.Name).Contains(x.OrganizationName)).Select(x => x.OrganizationName).ToList();

                var allFoundNames = allOrganizationsNames.Where(x => !foundNameAndBcodeEqual.Select(y => y.PtvUnificRootId).Contains(x.OrganizationVersioned.UnificRootId) && sahaItemsNotFoundNames.Contains(x.Name)).DistinctBy(x => x.OrganizationVersionedId).ToList();

                var foundNamesInPTV = allFoundNames.Join(sahaItemsNotFoundInPtv, x => x.Name, x => x.OrganizationName,
                    (oN, sI) =>
                        new VmSahaPtvMap()
                        {
                            PtvUnificRootId = oN.OrganizationVersioned.UnificRootId,
                            SahaId = sI.Id,
                            SahaOrgName = sI.OrganizationName
                        }).ToList();
                
                foundNameAndBcodeEqual.AddRange(foundNamesInPTV);
                var c = foundNameAndBcodeEqual.GroupBy(x => x.PtvUnificRootId).Where(x => x.Count() > 1);
                if (c.Any())
                {
                    foundNameAndBcodeEqual = foundNameAndBcodeEqual.DistinctBy(x => x.PtvUnificRootId).ToList();
                    Console.WriteLine($"Found duplicities({c.Count()}) in source, that have different saha id, but same ptv id, distinct done, but could not be correct");
                }
                var seedingService = _serviceProvider.GetService<ISeedingService>();
                seedingService.MapSahaGuid(foundNameAndBcodeEqual);
            }
            else
            {
                Console.WriteLine("File not exist.");
            }
//            List<object> postalCodes = new List<object>();
//
//            // Read postal codes from file (line by line)
//            var lines = File.ReadLines(CreatePostalCodesJsonTask.PostalCodesStartDataFile, System.Text.Encoding.GetEncoding("ISO-8859-1"));
//
//            foreach (var line in lines)
//            {
//                postalCodes.Add(new
//                {
//                    //Id = Guid.NewGuid(),
//                    Code = line.Substring(13, 5).Trim(),
//                    Type = line.Substring(110, 1).Trim(),
//                    MunicipalityCode = line.Substring(176, 3).Trim(),
//                    Names = new[] {
//                        new { Language = "fi", Name = line.Substring(18, 30).Trim() },
//                        new { Language = "sv", Name = line.Substring(48, 30).Trim() }
//                    }
//                });
//            }
//
//            postalCodes.Add(new {Code = "Undefined", Type="Undefined", MunicipalityCode = "Undefined",
//                Names = new[] {
//                        new { Language = "fi", Name = "Undefined" },
//                        new { Language = "sv", Name = "Undefined" }
//                    }
//            });
//            // Convert data to json
            // var json = JsonConvert.SerializeObject(users, Formatting.Indented);
//
//            // Overwrite the file always
            // File.WriteAllText("UserOrganization.json", json, System.Text.Encoding.UTF8);
        }

    }
}
