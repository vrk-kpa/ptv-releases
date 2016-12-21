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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.ExternalSources;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Import;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Logic.Finto;
using PTV.ExternalSources.Resources.Finto;
using PTV.ExternalSources.Resources.FintoFi;
using PTV.ExternalSources.Resources.Types;

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

        public SeedingService(IContextManager contextManager, ITranslationViewModel translationVmtoEnt, ResourceLoader resourceLoader, ResourceManager resourceManager, DataUtils dataUtils, IndustrialClassLogic industrialClassLogic, ILoggerFactory loggerFactory)
        {
            this.contextManager = contextManager;
            this.resourceManager = new ResourceManager();
            this.translationVmtoEnt = translationVmtoEnt;
            this.resourceLoader = resourceLoader;
            this.resourceManager = resourceManager;
            this.dataUtils = dataUtils;
            this.industrialClassLogic = industrialClassLogic;
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
                languageEntity.IsForData = forDataLanguages.Contains(language.Code.ToLower());
                languageEntity.IsDefault = language.Code.ToLower() == defaultLanguageCode.ToLower();
                languagesList.Add(language.Code, languageEntity);
            }
            unitOfWork.Save(SaveMode.AllowAnonymous);
            var eng = languagesList.First(i => i.Key.ToLower() == "en").Value;
            var fi = languagesList.First(i => i.Key.ToLower() == "fi").Value;
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
                englishName.Name = language.English;

                LanguageName finnishName = langNameRep.All().FirstOrDefault(i => (i.Localization.Code == fi.Code) && (i.Language.Code == langCode));
                if (finnishName == null)
                {
                    finnishName = new LanguageName();
                    langNameRep.Add(finnishName);
                }
                finnishName.Language = languageEntity;
                finnishName.Localization = fi;
                finnishName.Name = language.Fi;
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
                SeedType<NameType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.NameType}, unitOfWork);
                SeedType<PhoneNumberType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.PhoneNumberType},
                    unitOfWork);
                SeedType<DescriptionType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.DescriptionType},
                    unitOfWork);
                SeedType<AddressType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.AddressType}, unitOfWork);
                SeedType<WebPageType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.WebPageType}, unitOfWork);
                SeedType<ServiceCoverageType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceCoverageType}, unitOfWork);
                SeedType<PublishingStatusType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.PublishingStatus}, unitOfWork);
                SeedType<RoleType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.RoleType}, unitOfWork);
                SeedType<OrganizationType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.OrganizationType},
                    unitOfWork);
                SeedType<ServiceChannelType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceChannelType}, unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                SeedType<ProvisionType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ProvisionType},
                    unitOfWork);
                SeedType<ServiceChargeType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceChargeType},
                    unitOfWork);
                SeedType<ServiceHourType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceHours},
                    unitOfWork);
                SeedType<ExceptionHoursStatusType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ExceptionHoursStatus}, unitOfWork);
                SeedType<AttachmentType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.AttachmentType},
                    unitOfWork);
                SeedType<PrintableFormChannelUrlType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.PrintableFormChannelUrlType}, unitOfWork);
                SeedType<ServiceType>(new TypeItemImportDefinition {Resource = TypeItemImportDefinition.Resources.ServiceType}, unitOfWork);

                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }


        public void SeedDatabaseEnums()
        {
            SeedTypes();
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var municipalities = SeedMunicipality(unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                SeedUpdatePostalCodes(unitOfWork, municipalities);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                SeedCountrylCodes(unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);

                // seed finto from service views json
                SeedFintoItems<VmFintoJsonItem, LifeEvent>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.LifeSituations }, unitOfWork);
                SeedFintoItems<VmFintoJsonItem, ServiceClass>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.ServiceClasses }, unitOfWork);
                SeedFintoItems<VmFintoJsonItem, TargetGroup>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.TargetGroups }, unitOfWork);
                SeedFintoItems<VmIndustrialClassJsonItem, IndustrialClass>(industrialClassLogic.BuildTree(GetFintoItems<VmIndustrialClassJsonItem>(new FintoItemImportDefinition { Resource = FintoItemImportDefinition.Resources.IndustrialClasses })), unitOfWork);

                SeedOntologyTerms(unitOfWork);

                unitOfWork.Save(SaveMode.AllowAnonymous);
                Console.WriteLine("Done.");
            });
        }

        private void SeedOntologyTerms(IUnitOfWorkWritable unitOfWork)
        {
            Console.WriteLine("Loading Ontology terms...");
            var ontologyTermRepository = unitOfWork.CreateRepository<IOntologyTermRepository>();
            if (ontologyTermRepository.All().Any(x => x.OntologyType != null))
            {
                Console.WriteLine("Ontology terms already exists");
                return;
            }

            var ontologyTermParentRepository = unitOfWork.CreateRepository<IOntologyTermParentRepository>();
            var ontologyItems = GetFintoItems<VmFintoJsonItem>(new FintoItemImportDefinition
            {
                Resource = FintoItemImportDefinition.Resources.OntologyAll
            });

            var sourceItems = ontologyItems.ToDictionary(x => x.Id);
            var ontologyYso = SeedFintoItems<VmFintoJsonItem, OntologyTerm>(ontologyItems, null);//.ToDictionary(x => x.Uri);
            var ontologyTermParents = new List<OntologyTermParent>();
            dataUtils.JoinHierarchy(ontologyYso.ToDictionary(x => x.Uri), item => sourceItems[item.Uri].BroaderURIs, (parent, child) =>
            {
                ontologyTermParents.Add(new OntologyTermParent
                {
                    ChildId = child.Id,
                    ParentId = parent.Id
                });
            });

            var sot = unitOfWork.ApplyIncludes(
                ontologyTermRepository.All().Where(x => x.ServiceOntologyTerms.Any() && x.OntologyType == null),
                q => q.Include(x => x.ServiceOntologyTerms).Include(x => x.StatutoryServiceOntologyTerms).Include(x => x.ServiceChannelOntologyTerms)).ToList();
            var sotList = new List<ServiceOntologyTerm>();
            var ssotList = new List<StatutoryServiceOntologyTerm>();
            var scotList = new List<ServiceChannelOntologyTerm>();
            var sotListCheck = new HashSet<string>();
            if (sot.Count > 0)
            {
                foreach (var ontologyTerm in sot)
                {
                    var newOntology = ontologyYso.FirstOrDefault(x => ontologyTerm.Label == x.Label);
                    if (newOntology == null)
                    {
                        logger.LogInformation($"Ontology {ontologyTerm.Id} {ontologyTerm.Label} has not found.");
                    }
                    if (newOntology != null)
                    {

                        sotList.AddRange(ontologyTerm.ServiceOntologyTerms.Select(
                                x => new ServiceOntologyTerm { ServiceId = x.ServiceId, OntologyTermId = newOntology.Id, Created = x.Created, Modified = x.Modified, CreatedBy = x.CreatedBy, ModifiedBy = x.ModifiedBy}
                            ).Where(x =>
                            {
                                string uniqueKey = $"{x.ServiceId}-{x.OntologyTermId}";
                                if (sotListCheck.Contains(uniqueKey))
                                {
                                    return false;
                                }
                                sotListCheck.Add(uniqueKey);
                                return true;
                            })
                        );
                        scotList.AddRange(ontologyTerm.ServiceChannelOntologyTerms.Select(x => new ServiceChannelOntologyTerm { ServiceChannelId = x.ServiceChannelId, OntologyTermId = newOntology.Id, Modified = x.Modified, CreatedBy = x.CreatedBy, ModifiedBy = x.ModifiedBy }));
                        ssotList.AddRange(ontologyTerm.StatutoryServiceOntologyTerms.Select(x => new StatutoryServiceOntologyTerm() { StatutoryServiceGeneralDescriptionId = x.StatutoryServiceGeneralDescriptionId, OntologyTermId = newOntology.Id, Modified = x.Modified, CreatedBy = x.CreatedBy, ModifiedBy = x.ModifiedBy }));
                        ontologyTermRepository.Remove(ontologyTerm);
                    }
                }
            }


            Console.WriteLine("Inserting Ontology terms...");
            ontologyTermRepository.BatchInsert(ontologyYso);
            ontologyTermParentRepository.BatchInsert(ontologyTermParents);

            if (sotList.Count > 0)
            {
                unitOfWork.CreateRepository<IServiceOntologyTermRepository>().BatchInsert(sotList);
            }
            if (scotList.Count > 0)
            {
                unitOfWork.CreateRepository<IServiceChannelOntologyTermRepository>().BatchInsert(scotList);
            }
            if (ssotList.Count > 0)
            {
                unitOfWork.CreateRepository<IStatutoryServiceOntologyTermRepository>().BatchInsert(ssotList);
            }
        }

        internal void SeedType<TEntity>(IImportDefinition definition, IUnitOfWork unitOfWork) where TEntity : TypeBase, new()
        {
            int order = 0;
            resourceLoader.GetDesrializedResource<List<VmJsonTypeItem>>(definition).ForEach(fintoItem =>
            {
                fintoItem.OrderNumber = order++;
                var entity = translationVmtoEnt.Translate<VmJsonTypeItem, TEntity>(fintoItem, unitOfWork);
                //repository.Add(entity);

            });
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

        private List<TEntity> SeedFintoItems<TSource, TEntity>(IEnumerable<TSource> source, ITranslationUnitOfWork unitOfWork) where TEntity : FintoItemBase, new() where TSource : class//VmFintoJsonItem
        {
            return source.Select(fintoItem =>
            {
                var entity = translationVmtoEnt.Translate<TSource, TEntity>(fintoItem, unitOfWork);
                return entity;
            }).ToList();
        }

        private List<TEntity> SeedFintoItems<TSource, TEntity>(IImportDefinition definition, ITranslationUnitOfWork unitOfWork) where TEntity : FintoItemBase, new() where TSource : VmFintoJsonItem
        {
            return SeedFintoItems<TSource, TEntity>(GetFintoItems<TSource>(definition), unitOfWork);
        }

        private Dictionary<string, TEntity> SeedFintoFiItem<TEntity>(IRepository<TEntity> repository, IImportDefinition resource, Func<string, string> manageJsonFunc = null) where TEntity : FintoItemBase, new()
        {
            string jsonFile = resourceLoader.GetResource(resource);
            if (manageJsonFunc != null)
            {
                jsonFile = manageJsonFunc(jsonFile);
            }
            return JsonConvert.DeserializeObject<List<FintoFiJsonItem>>(jsonFile).Where(item => item.Type != "skos:ConceptScheme").Select(fintoItem =>
            {
                var entity = translationVmtoEnt.Translate<FintoFiJsonItem, TEntity>(fintoItem);
                repository.Add(entity);
                return entity;
            }).ToDictionary(x => x.Uri);
        }
//        private Dictionary<string, OntologyTerm> LoadFintoFromNarrowsers(IOntologyTermRepository repository)
//        {
//            string jsonFileOntologyTop = FintoFiJsonFileUpdate(resourceLoader.GetResource(new FintoFiItemImportDefinition { Resource = FintoFiItemImportDefinition.Resources.OntologyTerms}));
//            Dictionary<string, FintoFiJsonItem> items = JsonConvert.DeserializeObject<List<FintoFiJsonItem>>(jsonFileOntologyTop)
//                .Where(item => item.Type != "skos:ConceptScheme")
//                .ToDictionary(x => x.Uri);
//
//            JArray jsonFile = JArray.Parse(resourceLoader.GetResource(new FintoFiItemImportDefinition {Resource = FintoFiItemImportDefinition.Resources.OntologyHierarchyNarrowers}));
//            foreach (var arrayItem in jsonFile)
//            {
//                JToken parentUri = arrayItem["uri"];
//                JToken graphItems = arrayItem["narrower"];
//                foreach (var fintoFiNarrower in JsonConvert.DeserializeObject<List<FintoFiNarrower>>(graphItems.ToString()))
//                {
//                    if (!items.ContainsKey(fintoFiNarrower.Uri))
//                    {
//                        items.Add(fintoFiNarrower.Uri, CreateItem(fintoFiNarrower, parentUri.ToString()));
//                    }
//                }
//            }
//
//            var values =  items.Values.Select(
//                fintoItem => translationVmtoEnt.Translate<FintoFiJsonItem, OntologyTerm>(fintoItem)
//                ).ToDictionary(x => x.Uri);
//            return values;
//        }
//        private FintoFiJsonItem CreateItem(FintoFiNarrower narrower, string parentUri)
//        {
//            var item = new FintoFiJsonItem
//            {
//                Uri = narrower.Uri,
//                Broader = string.IsNullOrEmpty(parentUri) ? null : new FintoFiJsonParent { Uri = parentUri },
//                Type = "Onto",
//            };
//            item.Labels.Add(new FintoFiJsonLabel { Lang = "fi", Value = narrower.Label + narrower.PrefLabel });
//            return item;
        //        }
//        }
//        private string FintoFiJsonFileUpdate(string json)
//        {
//            JObject parsedJson = JObject.Parse(json);
//            JToken graphItems = parsedJson["graph"];
//            return graphItems.ToString();
        //        }
//        }
//        private void SeedFintoItemNames<TEntity>(IRepository<TEntity> repository, IEnumerable<TEntity> nameEntities, Language language) where TEntity : NameBase
//        {
//            foreach (var entity in nameEntities)
//            {
//                entity.Localization = language;
//                repository.Add(entity);
//            }
//        }

        private IReadOnlyList<PostalCode> SeedUpdatePostalCodes(IUnitOfWorkWritable unitOfWork, IReadOnlyList<Municipality> municipalities)
        {
            var jsonPostalCodes = resourceManager.GetDesrializedJsonResource<List<VmJsonPostalCode>>(JsonResources.PostalCode).Select(p => new VmPostalCodeJson()
            {
                Id = Guid.NewGuid(),
                Code = p.Code,
                PostOffice = p.Names.Find(n => n.Language == defaultLanguageCode).Name,
                MunicipalityId = municipalities.FirstOrDefault(x => x.Code == p.MunicipalityCode)?.Id,
            });
            return translationVmtoEnt.TranslateAll<VmPostalCodeJson, PostalCode>(jsonPostalCodes, unitOfWork);
        }

        private void SeedCountrylCodes(IUnitOfWorkWritable unitOfWork)
        {
            var result = resourceManager.GetCsvResource(CvsResources.CountryCode);
            var countryRepository = unitOfWork.CreateRepository<ICountryRepository>();
            if (countryRepository.All().Any())
            {
                return;
            }
            foreach (var countryCode in result.Where(x => x.Count() == 1))
            {
                countryRepository.Add(new Country() { Id = new Guid(), Code = countryCode[0] });
            }
        }

        private IReadOnlyList<Municipality> SeedMunicipality(IUnitOfWorkWritable unitOfWork)
        {
            var result = resourceManager.GetDesrializedJsonResource<List<VmMunicipality>>(JsonResources.Municipality);
            return translationVmtoEnt.TranslateAll<VmMunicipality, Municipality>(result, unitOfWork);
        }
    }
}
