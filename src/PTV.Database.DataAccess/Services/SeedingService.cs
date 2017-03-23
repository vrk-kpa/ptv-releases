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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.ExternalSources;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Import;
using PTV.Domain.Logic.Finto;
using PTV.ExternalSources.Resources.Finto;
using PTV.ExternalSources.Resources.Types;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ISeedingService), RegisterType.Transient)]
    internal class SeedingService : ISeedingService
    {
        private IContextManager contextManager;
        private ResourceManager resourceManager;
        private ITranslationViewModel translationVmtoEnt;
        private ResourceLoader resourceLoader;
        private DataUtils dataUtils;
        private IndustrialClassLogic industrialClassLogic;

        private readonly List<string> forDataLanguages = new List<string> {"en", "fi", "sv"};
        private string defaultLanguageCode = "fi";

        private ILogger logger;
        ITypesCache typesCache;

        public SeedingService(IContextManager contextManager, ITranslationViewModel translationVmtoEnt, ResourceLoader resourceLoader, ResourceManager resourceManager, DataUtils dataUtils, IndustrialClassLogic industrialClassLogic, ILoggerFactory loggerFactory, ITypesCache typesCache)
        {
            this.contextManager = contextManager;
            this.resourceManager = new ResourceManager();
            this.translationVmtoEnt = translationVmtoEnt;
            this.resourceLoader = resourceLoader;
            this.resourceManager = resourceManager;
            this.dataUtils = dataUtils;
            this.industrialClassLogic = industrialClassLogic;
            this.typesCache = typesCache;
            this.logger = loggerFactory.CreateLogger("Seeding service");
        }

        private Language SeedWorldLanguages(IUnitOfWorkWritable unitOfWork)
        {
            var langRep = unitOfWork.CreateRepository<ILanguageRepository>();
            var langNameRep = unitOfWork.CreateRepository<ILanguageNameRepository>();
            var languages = resourceManager.GetDesrializedJsonResource<List<VmLanguageCode>>(JsonResources.LanguageCodes);
            var languagesList = new Dictionary<string, Language>();
            foreach (var language in languages)
            {
                var codeToSearch = language.Code.ToLower();
                Language languageEntity = langRep.All().FirstOrDefault(i =>i.Code.ToLower() == codeToSearch);
                if (languageEntity == null)
                {
                    languageEntity = new Language();
                    langRep.Add(languageEntity);
                }

                languageEntity.Code = language.Code;
                languageEntity.OrderNumber = language.Order;
                languageEntity.IsForData = forDataLanguages.Contains(language.Code.ToLower());
                languageEntity.IsDefault = language.Code.ToLower() == defaultLanguageCode.ToLower();
                languagesList.Add(language.Code, languageEntity);
            }
            unitOfWork.Save(SaveMode.AllowAnonymous);
            var eng = languagesList.First(i => i.Key.ToLower() == "en").Value;
            var fi = languagesList.First(i => i.Key.ToLower() == "fi").Value;
            var sv = languagesList.First(i => i.Key.ToLower() == "sv").Value;
            foreach (var language in languages)
            {
                var languageEntity = languagesList[language.Code];
                var langCode = languageEntity.Code;
                LanguageName englishName = langNameRep.All().FirstOrDefault(i => (i.Localization.Code == eng.Code) && (i.Language.Code == langCode));
                if (englishName == null)
                {
                    englishName = new LanguageName();
                    langNameRep.Add(englishName);
                }
                englishName.Language = languageEntity;
                englishName.Localization = eng;
                englishName.Name = language.En;

                LanguageName finnishName = langNameRep.All().FirstOrDefault(i => (i.Localization.Code == fi.Code) && (i.Language.Code == langCode));
                if (finnishName == null)
                {
                    finnishName = new LanguageName();
                    langNameRep.Add(finnishName);
                }
                finnishName.Language = languageEntity;
                finnishName.Localization = fi;
                finnishName.Name = language.Fi;

                LanguageName swedishName = langNameRep.All().FirstOrDefault(i => (i.Localization.Code == sv.Code) && (i.Language.Code == langCode));
                if (swedishName == null)
                {
                    swedishName = new LanguageName();
                    langNameRep.Add(swedishName);
                }
                swedishName.Language = languageEntity;
                swedishName.Localization = sv;
                swedishName.Name = language.Sv;
            }
            unitOfWork.Save(SaveMode.AllowAnonymous);
            return languagesList.First(i => i.Key.ToLower() == defaultLanguageCode).Value;
        }


        public void SeedTypes()
        {
            Console.WriteLine("Inserting or updating types.");
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SeedWorldLanguages(unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SeedType<NameType, NameTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.NameType}, unitOfWork);
                SeedType<PhoneNumberType, PhoneNumberTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.PhoneNumberType},
                    unitOfWork);
                SeedType<DescriptionType, DescriptionTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.DescriptionType},
                    unitOfWork);
                SeedType<AddressType, AddressTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.AddressType}, unitOfWork);
                SeedType<WebPageType, WebPageTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.WebPageType}, unitOfWork);
                SeedType<ServiceCoverageType, ServiceCoverageTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceCoverageType}, unitOfWork);
                SeedType<PublishingStatusType, PublishingStatus>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.PublishingStatus}, unitOfWork);
                SeedType<RoleType, RoleTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.RoleType}, unitOfWork);
                SeedType<ServiceChannelType, ServiceChannelTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceChannelType}, unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                SeedType<ProvisionType, ProvisionTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ProvisionType},
                    unitOfWork);
                SeedType<ServiceChargeType, ServiceChargeTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceChargeType},
                    unitOfWork);
                SeedType<ServiceHourType, ServiceHoursTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceHours},
                    unitOfWork);
                SeedType<ExceptionHoursStatusType, ExceptionHoursStatus>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ExceptionHoursStatus}, unitOfWork);
                SeedType<AttachmentType, AttachmentTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.AttachmentType},
                    unitOfWork);
                SeedType<PrintableFormChannelUrlType, PrintableFormChannelUrlTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.PrintableFormChannelUrlType}, unitOfWork);
                SeedType<ServiceType, ServiceTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceType}, unitOfWork);
                SeedType<CoordinateType, CoordinateTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.CoordinateType}, unitOfWork);

                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
            typesCache.Refresh();
        }


        public void SeedDatabaseEnums()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var municipalities = SeedMunicipality(unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                SeedUpdatePostalCodes(unitOfWork, municipalities);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                SeedCountrylCodes(unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                Console.WriteLine("Done.");
            });
        }

        public void SeedFintoItems()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                // seed finto from service views json
                SeedFintoItems<VmServiceViewsJsonItem, LifeEvent>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.LifeSituations }, unitOfWork);
                SeedFintoItems<VmServiceViewsJsonItem, ServiceClass>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.ServiceClasses }, unitOfWork);
                SeedFintoItems<VmServiceViewsJsonItem, TargetGroup>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.TargetGroups }, unitOfWork);
                SeedFintoItems<VmServiceViewsJsonItem, OrganizationType>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.OrganizationTypes }, unitOfWork);
                Console.WriteLine("Seeding industrial classes...");
                GetFintoItems<VmIndustrialClassJsonItem, IndustrialClass>(industrialClassLogic.BuildTree(GetFintoItems<VmIndustrialClassJsonItem>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.IndustrialClasses })), unitOfWork);
                //SeedDigitalAuthorizations(unitOfWork);

                SeedOntologyTerms(unitOfWork);

                unitOfWork.Save(SaveMode.AllowAnonymous);
                Console.WriteLine("Done.");
            });
        }

        private void SeedOntologyTerms(IUnitOfWorkWritable unitOfWork)
        {
            Console.WriteLine("Loading Ontology terms...");
            var ontologyTermRepository = unitOfWork.CreateRepository<IOntologyTermRepository>();
            var current = ontologyTermRepository.All().ToDictionary(x => x.Uri);

            var ontologyTermParentRepository = unitOfWork.CreateRepository<IOntologyTermParentRepository>();
            var ontologyTermNameRepository = unitOfWork.CreateRepository<IOntologyTermNameRepository>();
            var ontologyItems = GetFintoItems<VmServiceViewsJsonItem>(new FintoItemImportDefinition
            {
                Resource = FintoItemImportDefinition.Resources.OntologyAll
            });

            var sourceItems = ontologyItems.ToDictionary(x => x.Id);
            var forUpdate = new List<OntologyTerm>();
            var forInsert = new List<OntologyTerm>();
            var ontologyYso = GetFintoItems<VmServiceViewsJsonItem, OntologyTerm>(ontologyItems, null, ot =>
            {
                var otExisting = current.TryGet(ot.Uri);
                if (otExisting != null)
                {
                    ot.Id = otExisting.Id;
                    forUpdate.Add(ot);
                }
                else
                {
                    ot.Id = ot.Id.IsAssigned() ? ot.Id : Guid.NewGuid();
                    forInsert.Add(ot);
                }
                ot.Names.ForEach(x => x.OntologyTermId = ot.Id);
            });
            var ontologyTermParents = new List<OntologyTermParent>();
            dataUtils.JoinHierarchy(ontologyYso.ToDictionary(x => x.Uri), item => sourceItems[item.Uri].BroaderURIs, (parent, child) =>
            {
                ontologyTermParents.Add(new OntologyTermParent
                {
                    ChildId = child.Id,
                    ParentId = parent.Id
                });
            });

            Console.WriteLine($"Current ontology terms count: {current.Count}.");
            Console.WriteLine($"Importing ontology terms: {ontologyYso.Count}. Updating {forUpdate.Count}. New {forInsert.Count}. Not matched {current.Count - forUpdate.Count}.");

            Console.WriteLine("Removing old hierarchy..");
            ontologyTermParentRepository.DeleteAll();
            ontologyTermNameRepository.BatchDelete(x => x.OntologyTermId, forUpdate.Select(x => x.Id));

            Console.WriteLine("Inserting Ontology terms...");
            string userName = unitOfWork.GetUserNameForAuditing(SaveMode.AllowAnonymous);
            ontologyTermRepository.BatchInsert(forInsert, userName);
            ontologyTermRepository.BatchUpdate(forUpdate, ot => ot.Id, userName);
            ontologyTermParentRepository.BatchInsert(ontologyTermParents, userName);
            ontologyTermNameRepository.BatchInsert(ontologyYso.SelectMany(x => x.Names), userName);

            SeedExactMatches(ontologyItems, ontologyYso, unitOfWork);
        }

        private void SeedExactMatches(List<VmServiceViewsJsonItem> ontologyItems, List<OntologyTerm> ontologyYso, IUnitOfWorkWritable unitOfWork)
        {
            string userName = unitOfWork.GetUserNameForAuditing(SaveMode.AllowAnonymous);

            //delete ontology exact matches
            Console.WriteLine("Delete old exact matches...");
            var ontologyExactMatchRep = unitOfWork.CreateRepository<IOntologyTermExactMatchRepository>();
            ontologyExactMatchRep.DeleteAll();

            //delete exact matches
            var exactMatchRep = unitOfWork.CreateRepository<IExactMatchRepository>();
            exactMatchRep.DeleteAll();

            Console.WriteLine("Insert new exact matches...");
            //insert new exact matches
            var exactMatches = ontologyItems.SelectMany(x => x.ExactMatchURIs).Distinct().Select(x=>new ExactMatch() { Id = Guid.NewGuid(), Uri = x }).ToList();
            exactMatchRep.BatchInsert(exactMatches, userName);
            Console.Write(exactMatches.Count());

            //insert new ontolgy exact matches
            var ontologyExactMatches = new List<OntologyTermExactMatch>();
            var sourceItems = ontologyItems.ToDictionary(x => x.Id);
            var exactMatchesItems = exactMatches.ToDictionary(x => x.Uri);
            foreach (var ontologyTerm in ontologyYso)
            {
                foreach (var exactMatch in sourceItems[ontologyTerm.Uri].ExactMatchURIs)
                {
                    ontologyExactMatches.Add(new OntologyTermExactMatch()
                    {
                        ExactMatchId = exactMatchesItems[exactMatch].Id,
                        OntologyTermId = ontologyTerm.Id
                    });
                }
            }

            //dataUtils.JoinHierarchy(exactMatches.ToDictionary(x => x.Uri), item => sourceItems[item.Uri].ExactMatchURIs, (parent, child) =>
            //{
            //    ontologyExactMatches.Add(new OntologyTermExactMatch()
            //    {
            //        ExactMatchId = exactMatchesItems[exactMatch].Id,
            //        OntologyTermId = ontologyTerm.Id
            //    });
            //});

            ontologyExactMatchRep.BatchInsert(ontologyExactMatches, userName);
        }

        private void SeedDigitalAuthorizations(IUnitOfWorkWritable unitOfWork)
        {
            Console.WriteLine("Loading Digital authorizations...");
            var sourceItems = GetFintoItems<VmRovaJsonItem>(new FintoItemImportDefinition
            {
                Resource = FintoItemImportDefinition.Resources.DigitalAuthorizations
            });

            var sourceItemsByKey = sourceItems.ToDictionary(x => x.Uri);
            var digitalAuthorizations = GetFintoItems<VmRovaJsonItem, DigitalAuthorization>(sourceItems, unitOfWork);
            dataUtils.JoinHierarchy(digitalAuthorizations.ToDictionary(x => x.Uri), item => sourceItemsByKey[item.Uri].BroaderConcepts, (parent, child) =>
            {
                child.Parent = parent;
                parent.Children.Add(child);
            });
        }

        internal void SeedType<TEntity>(IImportDefinition definition, IUnitOfWork unitOfWork) where TEntity : TypeBase, new()
        {
            int order = 0;
            resourceLoader.GetDesrializedResource<List<VmJsonTypeItem>>(definition).ForEach(fintoItem =>
            {
                fintoItem.OrderNumber = order++;
                var entity = translationVmtoEnt.Translate<VmJsonTypeItem, TEntity>(fintoItem, unitOfWork);
            });
        }


        internal void SeedType<TEntity, TEnumType>(IImportDefinition definition, IUnitOfWorkWritable unitOfWork) where TEntity : TypeBase, new() where TEnumType : struct
        {
            SeedType<TEntity>(definition, unitOfWork);
            unitOfWork.Save(SaveMode.AllowAnonymous);
            var set = unitOfWork.GetSet<TEntity>();
            var allSeededData = set.Select(i => i.Code).ToList();
            var enumData = Enum.GetValues(typeof(TEnumType)).Cast<TEnumType>().Select(i => i.ToString()).ToList();
            var missingInEnum = allSeededData.Except(enumData).ToList();
            var missingInDb = enumData.Except(allSeededData).ToList();
            if (missingInEnum.Any())
            {
                logger.LogCritical($"There is/are missing value(s) in enum type {typeof(TEnumType).Name}\n" + string.Join(",", missingInEnum));
            }
            if (missingInDb.Any())
            {
                logger.LogCritical($"There is/are missing value(s) in DB seed (json resource) of enum type {typeof(TEnumType).Name}\n" + string.Join(",", missingInDb));
            }
        }


        private void SeedLanguageNames(IUnitOfWorkWritable unitOfWork, IDictionary<string, Language> languageList, Dictionary<LanguageCode, List<string>> languageNameList)
        {
            var repository = unitOfWork.CreateRepository<ILanguageNameRepository>();
            var languageArray = new List<LanguageCode> { LanguageCode.fi, LanguageCode.sv, LanguageCode.en };
            foreach (var languageName in languageNameList)
            {
                var localization = languageList[languageName.Key.ToString()];
                int i = 0;
                foreach (var languageNameValue in languageName.Value)
                {
                    var language = languageList[languageArray[i].ToString()];
                    repository.Add(new LanguageName() { Id = new Guid(), Language = language, Name = languageNameValue, Localization = localization });
                    i++;
                }
            }

        }
        private List<TSource> GetFintoItems<TSource>(IImportDefinition definition) where TSource : class//VmFintoJsonItem
        {
            return resourceLoader.GetDesrializedResource<List<TSource>>(definition);
        }

        private List<TEntity> GetFintoItems<TSource, TEntity>(IEnumerable<TSource> source, ITranslationUnitOfWork unitOfWork, Action<TEntity> handleEntityAction = null) where TEntity : FintoItemBase, new() where TSource : class//VmFintoJsonItem
        {
            return source.Select(fintoItem =>
            {
                var entity = translationVmtoEnt.Translate<TSource, TEntity>(fintoItem, unitOfWork);
                handleEntityAction?.Invoke(entity);
                return entity;
            }).ToList();
        }

        private void SeedFintoItems<TSource, TEntity>(IImportDefinition definition, IUnitOfWorkWritable unitOfWork) where TEntity : FintoItemBase<TEntity>, new() where TSource : VmServiceViewsJsonItem
        {
//            Console.WriteLine($"Seeding {typeof(TEntity).Name}.");
//            return SeedFintoItems<TSource, TEntity>(GetFintoItems<TSource>(definition), unitOfWork);

            Console.WriteLine($"Loading {typeof(TEntity).Name}...");
            var sourceItems = GetFintoItems<TSource>(definition);

            var sourceItemsByKey = sourceItems.ToDictionary(x => x.Id);
            var entities = GetFintoItems<TSource, TEntity>(sourceItems, unitOfWork);
            dataUtils.JoinHierarchy(entities.ToDictionary(x => x.Uri), item => sourceItemsByKey[item.Uri].BroaderURIs, (parent, child) =>
            {
                child.Parent = parent;
                parent.Children.Add(child);
            });
            RemoveOldFintoItems(entities, unitOfWork);
        }

        private void RemoveOldFintoItems<TEntity>(List<TEntity> newItems, IUnitOfWorkWritable unitOfWork) where TEntity : FintoItemBase<TEntity>
        {
            var newIds = newItems.Select(x => x.Id);

            switch (typeof(TEntity).Name.ToLower())
            {
                case "targetgroup":
                    var tgRep = unitOfWork.CreateRepository<ITargetGroupRepository>();
                    var tgItems = tgRep.All().Where(x => !newIds.Contains(x.Id))
                   .Include(x => x.ServiceTargetGroups)
                   .Include(x => x.StatutoryServiceTargetGroups)
                   .Include(x => x.Names);
                    tgItems.ForEach(x =>
                    {
                        x.ServiceTargetGroups.Clear();
                        x.StatutoryServiceTargetGroups.Clear();
                        x.Names.Clear();
                        tgRep.Remove(x);
                    });
                    Console.WriteLine($"Delete {tgItems.Count()} {typeof(TEntity).Name}.");
                    break;
                case "serviceclass":
                    var scRep = unitOfWork.CreateRepository<IServiceClassRepository>();
                    var scItems = scRep.All().Where(x => !newIds.Contains(x.Id))
                   .Include(x => x.ServiceServiceClasses)
                   .Include(x => x.StatutoryServiceServiceClasses)
                   .Include(x => x.Names);
                    scItems.ForEach(x =>
                    {
                        x.ServiceServiceClasses.Clear();
                        x.StatutoryServiceServiceClasses.Clear();
                        x.Names.Clear();
                        scRep.Remove(x);
                    });
                    Console.WriteLine($"Delete {scItems.Count()} {typeof(TEntity).Name}.");
                    break;
                case "lifeevent":
                    var leRep = unitOfWork.CreateRepository<ILifeEventRepository>();
                    var leItems = leRep.All().Where(x => !newIds.Contains(x.Id))
                    .Include(x => x.ServiceLifeEvents)
                    .Include(x => x.StatutoryServiceLifeEvents)
                    .Include(x => x.Names);
                    leItems.ForEach(x =>
                    {
                        x.ServiceLifeEvents.Clear();
                        x.StatutoryServiceLifeEvents.Clear();
                        x.Names.Clear();
                        leRep.Remove(x);
                    });
                    Console.WriteLine($"Delete {leItems.Count()} {typeof(TEntity).Name}.");
                    break;
                default:
                    break;
            }
        }

        private IReadOnlyList<PostalCode> SeedUpdatePostalCodes(IUnitOfWorkWritable unitOfWork, IReadOnlyList<Municipality> municipalities)
        {
            var jsonPostalCodes = resourceManager.GetDesrializedJsonResource<List<VmJsonPostalCode>>(JsonResources.PostalCode);

            jsonPostalCodes.ForEach(x => x.MunicipalityId = municipalities.FirstOrDefault(y => y.Code == x.MunicipalityCode)?.Id);
            /*.Select(p => new VmPostalCodeJson()
            {
                Id = Guid.NewGuid(),
                Code = p.Code,
                PostOffice = p.Names.Find(n => n.Language == defaultLanguageCode).Name,
                MunicipalityId = municipalities.FirstOrDefault(x => x.Code == p.MunicipalityCode)?.Id,
            });*/

            return translationVmtoEnt.TranslateAll<VmJsonPostalCode, PostalCode>(jsonPostalCodes, unitOfWork);
        }

        private void SeedCountrylCodes(IUnitOfWorkWritable unitOfWork)
        {
            var result = resourceManager.GetDesrializedJsonResource<List<VmJsonCountry>>(JsonResources.CountryCodes);
            translationVmtoEnt.TranslateAll<VmJsonCountry, Country>(result, unitOfWork);
            unitOfWork.Save(SaveMode.AllowAnonymous);
            var phoneRep = unitOfWork.CreateRepository<IPhoneRepository>();
            var dialCodesRepository = unitOfWork.CreateRepository<IDialCodeRepository>();
            var phonesToFix = phoneRep.All().Where(i => i.PrefixNumber.Country.Code == "FAKE").Include(i => i.PrefixNumber).ToList();
            phonesToFix.ForEach(phone =>
            {
                phone.Number = phone.PrefixNumber.Code + " " + phone.Number;
                if (phone.Number.Length > 20)
                {
                    phone.Number = phone.Number.Substring(0, 19);
                }
                dialCodesRepository.Remove(phone.PrefixNumber);
                phone.PrefixNumber = null;
            });
        }

        private IReadOnlyList<Municipality> SeedMunicipality(IUnitOfWorkWritable unitOfWork)
        {
            var result = resourceManager.GetDesrializedJsonResource<List<VmJsonMunicipality>>(JsonResources.Municipality);
            return translationVmtoEnt.TranslateAll<VmJsonMunicipality, Municipality>(result.Where(x => !x.IsRemoved), unitOfWork);
        }
    }
}
