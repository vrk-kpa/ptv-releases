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
using Newtonsoft.Json;
using PTV.Database.Model.Models.Privileges;
using PTV.Framework.Enums;

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
                SeedType<PhoneNumberType, PhoneNumberTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.PhoneNumberType}, unitOfWork);
                SeedType<DescriptionType, DescriptionTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.DescriptionType}, unitOfWork);
                SeedType<AddressType, AddressTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.AddressType}, unitOfWork);
                SeedType<WebPageType, WebPageTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.WebPageType}, unitOfWork);
                SeedType<PublishingStatusType, PublishingStatus>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.PublishingStatus}, unitOfWork);
                SeedType<RoleType, RoleTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.RoleType}, unitOfWork);
                SeedType<ServiceChannelType, ServiceChannelTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceChannelType}, unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                SeedType<ProvisionType, ProvisionTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ProvisionType}, unitOfWork);
                SeedType<ServiceChargeType, ServiceChargeTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceChargeType}, unitOfWork);
                SeedType<ServiceHourType, ServiceHoursTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceHours}, unitOfWork);
                SeedType<ExceptionHoursStatusType, ExceptionHoursStatus>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ExceptionHoursStatus}, unitOfWork);
                SeedType<AttachmentType, AttachmentTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.AttachmentType}, unitOfWork);
                SeedType<PrintableFormChannelUrlType, PrintableFormChannelUrlTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.PrintableFormChannelUrlType}, unitOfWork);
                SeedType<ServiceType, ServiceTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceType}, unitOfWork);
                SeedType<CoordinateType, CoordinateTypeEnum>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.CoordinateType}, unitOfWork);
                SeedType<AppEnvironmentDataType, AppEnvironmentDataTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AppEnvironmentDataType }, unitOfWork);
                SeedType<AreaInformationType, AreaInformationTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AreaInformationType }, unitOfWork);
                SeedType<AreaType, AreaTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AreaType }, unitOfWork);
                SeedType<ServiceChannelConnectionType, ServiceChannelConnectionTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceChannelConnectionType }, unitOfWork);
                SeedType<ServiceFundingType, ServiceFundingTypeEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.ServiceFundingType }, unitOfWork);
                SeedType<AccessRightType, AccessRightEnum>(new TypeItemImportDefinition { Resource = TypeItemImportDefinition.Resources.AccessRightType }, unitOfWork);
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
                SeedAndUpdateAllAreaCodes(unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);

                Console.WriteLine("Done.");
            });
        }

        public void SeedFintoItems()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                // seed finto from service views json
                SeedFintoItems<LifeEvent>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.LifeSituations }, unitOfWork);
                SeedFintoItems<ServiceClass>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.ServiceClasses }, unitOfWork);
                SeedFintoItems<TargetGroup>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.TargetGroups }, unitOfWork);
                SeedFintoItems<OrganizationType>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.OrganizationTypes }, unitOfWork);
                Console.WriteLine("Seeding industrial classes...");
                TranslateItems<VmIndustrialClassJsonItem, IndustrialClass>(industrialClassLogic.BuildTree(GetFintoItems<VmIndustrialClassJsonItem>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.IndustrialClasses })), unitOfWork);
                SeedOntologyTerms(unitOfWork);

                unitOfWork.Save(SaveMode.AllowAnonymous);
                Console.WriteLine("Done.");
            });
        }

        private OntologyTerm GetOntologyTermEntity(Dictionary<string, OntologyTerm> current, VmServiceViewsJsonItem main, VmServiceViewsJsonItem replaced, List<OntologyTerm> forUpdate, List<OntologyTerm> forInsert)
        {
            var entity = translationVmtoEnt.Translate<VmServiceViewsJsonItem, OntologyTerm>(main);

            var otExisting = current.TryGet(replaced.Id) ?? current.TryGet(main.Id);
            if (otExisting != null)
            {
                entity.Id = otExisting.Id;
                entity.Created = otExisting.Created;
                entity.CreatedBy = otExisting.CreatedBy;
                forUpdate.Add(entity);
            }
            else
            {
                entity.Id = entity.Id.IsAssigned() ? entity.Id : Guid.NewGuid();
                forInsert.Add(entity);
            }
            entity.Names.ForEach(x => x.OntologyTermId = entity.Id);
            return entity;
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

            var main = GetMigratedFintoItems(ontologyItems);
            var sourceItems = ontologyItems.ToDictionary(x => x.Id);
            var forUpdate = new List<OntologyTerm>();
            var forInsert = new List<OntologyTerm>();

            var ontologyYso = main.Select(fintoItem =>
            {

                var replacedBy = fintoItem.Value.Main;
                OntologyTerm entity = null;
                if (fintoItem.Value.Replaced.Count == 0)
                {
                    entity = GetOntologyTermEntity(current, replacedBy, replacedBy, forUpdate, forInsert);
                }
                else
                {
                    List<OntologyTerm> existing = new List<OntologyTerm>();
                    List<OntologyTerm> inserting = new List<OntologyTerm>();
                    GetOntologyTermEntity(current, replacedBy, replacedBy, existing, new List<OntologyTerm>());

                    foreach (var item in fintoItem.Value.Replaced)
                    {
                        GetOntologyTermEntity(current, replacedBy, item.Item, existing, inserting);
                    }
                    existing.ForEach(ot =>
                    {
                        if (entity == null)
                        {
                            entity = ot;
                        }
                        else
                        {
                            ot.Code = "duplicated";
                        }
                    });
                    forUpdate.AddRange(existing);
                    if (entity == null && inserting.Count > 0)
                    {
                        entity = inserting.First();
                        forInsert.Add(entity);
                    }
                    else if (entity == null)
                    {
                        throw new Exception("Nothing to update for ontology");
                    }
                }
                return entity;
            }).ToList();

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

        public void SeedDigitalAuthorizations(string jsonData = null)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SeedDigitalAuthorizations(unitOfWork, jsonData);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                Console.WriteLine("Seeding of digital autorizations done.");
            });
        }

        private void SeedDigitalAuthorizations(ITranslationUnitOfWork unitOfWork, string data = null)
        {
            var sourceItems = (data == null)
                ? resourceManager.GetDesrializedJsonResource<List<VmRovaJsonItem>>(JsonResources.DigitalAuthorizations)
                : JsonConvert.DeserializeObject<List<VmRovaJsonItem>>(data);

            var sourceItemsByKey = sourceItems.ToDictionary(x => x.Uri);
            var digitalAuthorizations = sourceItems.Select(item => translationVmtoEnt.Translate<VmRovaJsonItem, DigitalAuthorization>(item, unitOfWork)).ToList();

            dataUtils.JoinHierarchy(digitalAuthorizations.ToDictionary(x => x.Uri), item => sourceItemsByKey[item.Uri].BroaderConcepts, (parent, child) =>
            {
                child.Parent = parent;
                parent.Children.Add(child);
            });
        }

        internal void SeedType<TEntity>(IImportDefinition definition, IUnitOfWork unitOfWork) where TEntity : TypeBase, new()
        {
            int order = 0;
            resourceLoader.GetDeserializedResource<List<VmJsonTypeItem>>(definition).ForEach(fintoItem =>
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
            return resourceLoader.GetDeserializedResource<List<TSource>>(definition);
        }

        private List<TEntity> TranslateItems<TSource, TEntity>(IEnumerable<TSource> source, ITranslationUnitOfWork unitOfWork) where TEntity : FintoItemBase, new() where TSource : class//VmFintoJsonItem
        {
            return source.Select(fintoItem =>
            {
                var entity = translationVmtoEnt.Translate<TSource, TEntity>(fintoItem, unitOfWork);
                return entity;
            }).ToList();
        }

        private void SeedFintoItems<TEntity>(IImportDefinition definition, IUnitOfWorkWritable unitOfWork) where TEntity : FintoItemBase<TEntity>, new()
        {
//            Console.WriteLine($"Seeding {typeof(TEntity).Name}.");
//            return SeedFintoItems<TSource, TEntity>(TranslateItems<TSource>(definition), unitOfWork);

            Console.WriteLine($"Loading {typeof(TEntity).Name}...");
            var sourceItems = GetFintoItems<VmServiceViewsJsonItem>(definition);
            var main = GetMigratedFintoItems(sourceItems);

            var sourceItemsByKey = sourceItems.ToDictionary(x => x.Id);
            var entities = TranslateItems<VmReplaceItemServiceViewsJsonItem, TEntity>(main.Values.Where(x => x.Replaced.Count > 0).SelectMany(x => x.Replaced), unitOfWork);
            entities.AddRange(TranslateItems<VmServiceViewsJsonItem, TEntity>(
                main.Values.Where(x => x.Replaced.Count == 0).Select(x => x.Main), unitOfWork));
            dataUtils.JoinHierarchy(entities.GroupBy(x => x.Uri).ToDictionary(x => x.Key, x => x.FirstOrDefault()), item => sourceItemsByKey[item.Uri].BroaderURIs, (parent, child) =>
            {
                child.Parent = parent;
                parent.Children.Add(child);
            });
            RemoveOldFintoItems(entities, unitOfWork);
        }

        public void FixDuplicatedFintoItems()
        {
            FixFintoMigration();
            FixOntologyTermMigration();
        }

        private void FixFintoMigration()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IRepository<ServiceClass>>();
                var repSsc = unitOfWork.CreateRepository<IRepository<ServiceServiceClass>>();
                var repGdSc = unitOfWork.CreateRepository<IRepository<StatutoryServiceServiceClass>>();
                var uris = rep.All().GroupBy(x => x.Uri).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                var items = unitOfWork.ApplyIncludes(rep.All().Where(x => uris.Contains(x.Uri)),
                    q => q.Include(x => x.Parent)
                    .Include(x => x.ServiceServiceClasses)
                    .Include(x => x.StatutoryServiceServiceClasses)
                ).ToList();
                items.GroupBy(x => x.Uri).ForEach(x =>
                    {
                        ServiceClass toKeep = null;
                        Queue<ServiceClass> toRemove = new Queue<ServiceClass>();
                        x.ForEach(sc =>
                        {
                            if (sc.ParentUri == sc.Parent.Uri)
                            {
                                toKeep = sc;
                            }
                            else
                            {
                                toRemove.Enqueue(sc);
                            }
                        });
                        if (toKeep == null)
                        {
                            toKeep = toRemove.Dequeue();
                        }
                        foreach (var serviceClass in toRemove)
                        {
                            rep.Remove(serviceClass);
                            serviceClass.ServiceServiceClasses.ForEach(ssc => repSsc.Add(
                                new ServiceServiceClass
                                {
                                    ServiceVersionedId = ssc.ServiceVersionedId,
                                    ServiceClass = toKeep
                                }));
                            serviceClass.StatutoryServiceServiceClasses.ForEach(ssc => repGdSc.Add(
                                new StatutoryServiceServiceClass
                                {
                                    StatutoryServiceGeneralDescriptionVersionedId = ssc.StatutoryServiceGeneralDescriptionVersionedId,
                                    ServiceClass = toKeep
                                }));
                        }
                    }
                );
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }
        private void FixOntologyTermMigration()
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IRepository<OntologyTerm>>();
                var repSsc = unitOfWork.CreateRepository<IRepository<ServiceOntologyTerm>>();
                var repGdSc = unitOfWork.CreateRepository<IRepository<StatutoryServiceOntologyTerm>>();
                var items = unitOfWork.ApplyIncludes(rep.All().Where(x => x.Code == "duplicated"),
                    q => q
                    .Include(x => x.ServiceOntologyTerms)
                    .Include(x => x.StatutoryServiceOntologyTerms)
                ).ToList();
                Console.WriteLine($"Ontology to remove {items.Count}.");

                var duplicatedUris = items.Select(x => x.Uri).Distinct().ToList();
                var replacingItems = unitOfWork.ApplyIncludes(rep.All().Where(x => duplicatedUris.Contains(x.Uri) && x.Code != "duplicated"),
                    q => q.Include(x => x.ServiceOntologyTerms)
                        .Include(x => x.StatutoryServiceOntologyTerms)
                    ).ToDictionary(x => x.Uri);
                items.ForEach(x =>
                    {
                        var toKeep = replacingItems.TryGet(x.Uri);
                        rep.Remove(x);
                        x.ServiceOntologyTerms.Where(ot => !toKeep.ServiceOntologyTerms.Select(j => j.ServiceVersionedId).Contains(ot.ServiceVersionedId)).ForEach(ssc => repSsc.Add(
                            new ServiceOntologyTerm
                            {
                                ServiceVersionedId = ssc.ServiceVersionedId,
                                OntologyTerm = toKeep
                            }));
                        x.StatutoryServiceOntologyTerms.Where(ot => !toKeep.StatutoryServiceOntologyTerms.Select(j => j.StatutoryServiceGeneralDescriptionVersionedId).Contains(ot.StatutoryServiceGeneralDescriptionVersionedId)).ForEach(ssc => repGdSc.Add(
                            new StatutoryServiceOntologyTerm
                            {
                                StatutoryServiceGeneralDescriptionVersionedId =
                                    ssc.StatutoryServiceGeneralDescriptionVersionedId,
                                OntologyTerm = toKeep
                            }));

                    }
                );
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }

        private static Dictionary<string, VmReplaceGroupServiceViewsJsonItem> GetMigratedFintoItems(List<VmServiceViewsJsonItem> sourceItems)
        {
            var replaced = sourceItems.Where(x => !string.IsNullOrEmpty(x.ReplacedBy)).GroupBy(x => x.ReplacedBy).ToList();
            var main = sourceItems.Where(x => string.IsNullOrEmpty(x.ReplacedBy))
                .ToDictionary(x => x.Id, x => new VmReplaceGroupServiceViewsJsonItem {Main = x});
            replaced.ForEach(x =>
            {
                var item = main.TryGet(x.Key);
                item?.Replaced.AddRange(x.Select(i => new VmReplaceItemServiceViewsJsonItem
                {
                    Item = i,
                    ReplacedBy = item.Main
                }));
            });
            return main;
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

        private void SeedAndUpdateAllAreaCodes(IUnitOfWorkWritable unitOfWork)
        {
            SeedAndUpdateAreaCodes(unitOfWork, AreaTypeEnum.Province, JsonResources.Province);
            SeedAndUpdateAreaCodes(unitOfWork, AreaTypeEnum.BusinessRegions, JsonResources.BusinessRegion);
            SeedAndUpdateAreaCodes(unitOfWork, AreaTypeEnum.HospitalRegions, JsonResources.HospitalRegion);
            unitOfWork.Save(SaveMode.AllowAnonymous);
            InvalidateAreas(unitOfWork);
        }

        private void SeedAndUpdateAreaCodes(IUnitOfWorkWritable unitOfWork, AreaTypeEnum areaType, JsonResources jsonResource)
        {
            var municipalityRep = unitOfWork.CreateRepository<IMunicipalityRepository>();
            var areaCodes = resourceManager.GetDesrializedJsonResource<List<VmJsonArea>>(jsonResource);
            var areaTypeId = typesCache.Get<AreaType>(areaType.ToString());

            // invalidate not exist areas
            var existingAreas = unitOfWork.CreateRepository<IAreaRepository>().All().Where(a => a.AreaTypeId == areaTypeId);
            var areasToInvalidate = existingAreas.Where(ea => !(areaCodes.Select(a => a.Code)).Contains(ea.Code));
            areasToInvalidate.ForEach(a => a.IsValid = false);

            areaCodes.ForEach(area =>
            {
                area.IsValid = true;
                area.AreaTypeId = areaTypeId;
                area.Municipalities.ForEach(municipalityCode => {
                    var municipalityId = municipalityRep.All().FirstOrDefault(x => x.Code == municipalityCode)?.Id;
                    if (municipalityId == null)
                    {
                        throw new Exception($"Municipality of code: {municipalityCode} doesn't exist.");
                    }
                    area.AreaMunicipalities.Add(new VmJsonAreaMunicipality() { MunicipalityId = municipalityId.Value });
                });
            });

            var areas = translationVmtoEnt.TranslateAll<VmJsonArea, Area>(areaCodes, unitOfWork);
            areas.ForEach(a => a.AreaMunicipalities = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, a.AreaMunicipalities, x => x.AreaId == a.Id, x => x.MunicipalityId));
        }

        private void InvalidateAreas(IUnitOfWorkWritable unitOfWork)
        {
            var invalidAreas = resourceManager.GetDesrializedJsonResource<List<VmInvalidArea>>(JsonResources.InvalidAreas);
            foreach (var invalidArea in invalidAreas)
            {
                InvalidateArea(unitOfWork, invalidArea.Type, invalidArea.Codes);
            }
        }

        private void InvalidateArea(IUnitOfWorkWritable unitOfWork, string areaType, IEnumerable<string> codes)
        {
            AreaTypeEnum type;
            if (!Enum.TryParse(areaType, true, out type)) return;

            var areaTypeId = typesCache.Get<AreaType>(areaType);
            var areas = unitOfWork.CreateRepository<IAreaRepository>().All().Where(a => a.AreaTypeId == areaTypeId && codes.Contains(a.Code));
            areas.ForEach(a => a.IsValid = false);
        }

        internal class VmInvalidArea
        {
            public string Type { get; set; }

            public List<string> Codes { get; set; }

        }
    }
}
