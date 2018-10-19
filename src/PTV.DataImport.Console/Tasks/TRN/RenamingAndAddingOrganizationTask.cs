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
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Framework;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.DirectRaw;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Organization;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    [RegisterService(typeof(RenamingAndAddingOrganizationTask), RegisterType.Transient)]
    public class RenamingAndAddingOrganizationTask
    {
        private IServiceProvider _serviceProvider;

        public RenamingAndAddingOrganizationTask(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;
        }

        private static readonly Dictionary<string, string> OrganizationNewNames = new Dictionary<string, string>()
        {
            {"fi", "Testiorganisaatio"},
            {"sv", "Testorganisation"},
            {"en", "Testorganization"}
        };
        
        private static readonly Dictionary<string, string> OrganizationNewSummaryDescriptions = new Dictionary<string, string>()
        {
            {"fi", "Testiorganisaatio {0} on varattu koulutusympäristön testejä varten."},  
            {"sv", "Testorganisation {0} är reserverad för träningsmiljöprov."},       
            {"en", "Testorganization {0} for test users."}   
        };
        
        private const string testEmail = "test.person@test.com";
        
        public void UpdateOrganizations()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                var languageCache = _serviceProvider.GetService<ILanguageCache>();
                var typesCache = _serviceProvider.GetService<ITypesCache>();
                var organizationService = _serviceProvider.GetService<IOrganizationService>();
                var databaseRawContext = serviceScope.ServiceProvider.GetService<IDatabaseRawContext>();
                
                
                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    //1
                    //RenameOrganizations(unitOfWork, languageCache, typesCache);
                    //unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    
                    //2
                    //AddNewOrganizations(unitOfWork, organizationService, typesCache, languageCache);
                    
                    //3
                    //RemoveAllNotNeededEntiites(databaseRawContext, unitOfWork, typesCache);
                    //unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    
                    //4
                    //ChangeAllModifiedByAndCreatedBy(unitOfWork, testEmail);
                    //unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                });
            }
        }

        private void RemoveAllNotNeededEntiites(IDatabaseRawContext databaseRawContext, IUnitOfWorkWritable unitOfWork, ITypesCache typesCache)
        {
            //Draft organization 
            RemoveData(databaseRawContext, "OrganizationVersioned","Id", new List<Guid>{ new Guid("b176b485-b8da-47bb-9211-530c5f72a586")});

            var publishingStatusDeletedId = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var organizationVersionedRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var removeAllArchivedOrganizationList = organizationVersionedRepository.All().Where(x => x.PublishingStatusId == publishingStatusDeletedId)
                .Select(y => y.Id).ToList();
            
            //Remove all old archived orgnizations
            RemoveData(databaseRawContext, "OrganizationVersioned","Id", removeAllArchivedOrganizationList);
        }

        private void RenameOrganizations(IUnitOfWorkWritable unitOfWork, ILanguageCache languageCache, ITypesCache typesCache)
        {
            var renameOrganizationRootDictionary = new Dictionary<Guid, int>()
            {
                {new Guid("b458d033-042d-4cbe-b030-34e81da28821"), 1},
                {new Guid("df499a95-3f53-4a4c-b794-015b25710ee8"), 2},
                {new Guid("746538f1-6ddc-4042-bd7b-923d6401ecae"), 3},
                {new Guid("c60381d6-fbd7-45d7-994e-c99ec0fc8f3f"), 4},
                {new Guid("011154df-3726-461e-ae9c-a1182d1746de"), 5},
                {new Guid("3d1759a2-e47a-4947-9a31-cab1c1e2512b"), 6},
                {new Guid("6745e341-be2a-45a4-b184-bbc2f8465615"), 7},
                {new Guid("92374b0f-7d3c-4017-858e-666ee3ca2761"), 8},
                {new Guid("7fdd7f84-e52a-4c17-a59a-d7c2a3095ed5"), 9},
                {new Guid("52e0f6dc-ec1f-48d5-a0a2-7a4d8b657d53"), 10},
                {new Guid("0d34eb3a-d7e5-4af0-aa0d-009b5fb0e91c"), 11},
                {new Guid("e9d022bc-97b8-41c6-b953-d052ad53bc91"), 12},
                {new Guid("3e8356bd-377f-4cad-97b1-a027bd4bbf25"), 13},
                {new Guid("4bc4fad0-84fe-412f-8fb9-c431f4ba48b2"), 14},
                {new Guid("ae788356-6950-48fc-b3ff-63243f74fe53"), 15},
                {new Guid("c225a17a-b767-4148-80ae-b78506275534"), 16},
                {new Guid("269685e8-9c00-4f2e-b6c7-e61d94e13b96"), 17}
            };
            
            var organizationTypeCacheData = typesCache.GetCacheData<OrganizationType>();
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());

            var organizationVersionedRepository = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();

            var renameOrganizations = organizationVersionedRepository.All()
                .Where(x => renameOrganizationRootDictionary.Keys.Contains(x.UnificRootId));

            var organizations = unitOfWork.ApplyIncludes(renameOrganizations, q => q.Include(i => i.OrganizationNames).Include(i => i.OrganizationDisplayNameTypes))
                .ToList();

            foreach (var organization in organizations)
            {
                var orgNumber = renameOrganizationRootDictionary.ContainsKey(organization.UnificRootId)
                    ? renameOrganizationRootDictionary[organization.UnificRootId]
                    : 0;
                
                var organizationTypeNames = organizationTypeCacheData.First(x => x.Id == organization.TypeId).Names;

                foreach (var orgName in organization.OrganizationNames) //jenom typu name
                {    
                    if (orgName.TypeId == nameTypeId)
                    {
                        var orgLanguageCode = languageCache.GetByValue(orgName.LocalizationId);
                        var orgNewName = OrganizationNewNames.ContainsKey(orgLanguageCode)
                            ? OrganizationNewNames[orgLanguageCode]
                            : "no-exist translation";

                        var organizationTypeName =
                            organizationTypeNames.FirstOrDefault(x => x.LocalizationId == orgName.LocalizationId)
                                ?.Name ?? "no-exist translation";

                        orgName.Name = $"{orgNewName} {orgNumber} ({organizationTypeName})";
                    }
                    else
                    {
                        unitOfWork.DetachOrRemoveEntity(orgName);
                    }
                }
                
                //Change all names 
                if (organization.OrganizationNames.Any(x => x.TypeId == nameTypeId))
                {
                    foreach (var nameType in organization.OrganizationDisplayNameTypes)
                    {
                        nameType.DisplayNameTypeId = nameTypeId;
                    }
                }
            }
        }
        
        private void AddNewOrganizations(IUnitOfWorkWritable unitOfWork, IOrganizationService organizationService, ITypesCache typesCache, ILanguageCache languageCache)
        {
            var newOrganizationInputs = GetOrganizationsInputData(unitOfWork, typesCache, languageCache);
            foreach (var input in newOrganizationInputs)
            {
                var newOrganization = organizationService.SaveOrganization(input);
                if (newOrganization.Id.HasValue)
                {
                    var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                    input.LanguagesAvailabilities.ForEach(x =>
                    {
                        x.StatusId = publishingStatusId;
                    });
                    
                    organizationService.PublishOrganization(new VmPublishingModel()
                    {
                        Id = newOrganization.Id.Value,
                        LanguagesAvailabilities = input.LanguagesAvailabilities
                    });
                }
            }
        }

        private List<VmOrganizationInput> GetOrganizationsInputData(IUnitOfWorkWritable unitOfWork, ITypesCache typesCache, ILanguageCache languageCache)
        {
            var models = new List<VmOrganizationInput>();
            var organizationTypeCacheData = typesCache.GetCacheData<OrganizationType>();
            
            //18
            var orgNumber = 18;
            var orgLanguageCodes = new List<string>(){"fi","en"};
            var org18 = CreateEmptyOrganizationInput();
            
            org18.OrganizationType = typesCache.Get<OrganizationType>(OrganizationTypeEnum.Company.ToString());
            org18.AreaInformation = new VmAreaInformation()
            {
                AreaInformationTypeId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString()), // new Guid("df57ce06-b661-d647-60df-3385b2ae7ffc"),
                Provinces = new List<Guid>()
                {
                    new Guid("911d51dd-f843-41da-bb99-9ab42deacfc5"),
                    new Guid("70ab9aed-8554-467d-a1f7-26d83867b6a4")
                } 
            };
            
            foreach (var orgLanguageCode in orgLanguageCodes)
            {
                var organizationTypeNames = organizationTypeCacheData.First(x => x.Id == org18.OrganizationType).Names;
                var localizationId = languageCache.Get(orgLanguageCode);
                var organizationTypeName = organizationTypeNames.FirstOrDefault(x => x.LocalizationId == localizationId)?.Name ?? "no-exist translation";
                var orgNewName = OrganizationNewNames.ContainsKey(orgLanguageCode)
                    ? OrganizationNewNames[orgLanguageCode]
                    : "no-exist translation";
                
                org18.Name.Add(orgLanguageCode, $"{orgNewName} {orgNumber} ({organizationTypeName})");
                org18.ShortDescription.Add(orgLanguageCode, string.Format(OrganizationNewSummaryDescriptions[orgLanguageCode], orgNumber));
            }
      
            SetLanguageAvailabilities(languageCache, org18);
            models.Add(org18);
            
            //19
            orgNumber = 19;
            orgLanguageCodes = new List<string>(){"fi","sv","en"};
            var org19 = CreateEmptyOrganizationInput();
            
            org19.OrganizationType = typesCache.Get<OrganizationType>(OrganizationTypeEnum.Company.ToString());
            org19.AreaInformation = new VmAreaInformation()
            {
                AreaInformationTypeId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString()), // new Guid("df57ce06-b661-d647-60df-3385b2ae7ffc"),
                Provinces = new List<Guid>()
                {
                    new Guid("6be7c2d3-e8f1-427c-996d-e1d98363f0d3")
                } 
            };
           
            foreach (var orgLanguageCode in orgLanguageCodes)
            {
                var organizationTypeNames = organizationTypeCacheData.First(x => x.Id == org19.OrganizationType).Names;
                var localizationId = languageCache.Get(orgLanguageCode);
                var organizationTypeName = organizationTypeNames.FirstOrDefault(x => x.LocalizationId == localizationId)?.Name ?? "no-exist translation";
                var orgNewName = OrganizationNewNames.ContainsKey(orgLanguageCode)
                    ? OrganizationNewNames[orgLanguageCode]
                    : "no-exist translation";
                
                org19.Name.Add(orgLanguageCode, $"{orgNewName} {orgNumber} ({organizationTypeName})");
                org19.ShortDescription.Add(orgLanguageCode, string.Format(OrganizationNewSummaryDescriptions[orgLanguageCode], orgNumber));
            }
      
            SetLanguageAvailabilities(languageCache, org19);
            models.Add(org19);
            
            //20
            orgNumber = 20;
            orgLanguageCodes = new List<string>(){"fi","sv"};
            var org20 = CreateEmptyOrganizationInput();
            
            org20.OrganizationType = typesCache.Get<OrganizationType>(OrganizationTypeEnum.State.ToString());
            org20.AreaInformation = new VmAreaInformation()
            {
                AreaInformationTypeId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString()),
            };
           
            foreach (var orgLanguageCode in orgLanguageCodes)
            {
                var organizationTypeNames = organizationTypeCacheData.First(x => x.Id == org20.OrganizationType).Names;
                var localizationId = languageCache.Get(orgLanguageCode);
                var organizationTypeName = organizationTypeNames.FirstOrDefault(x => x.LocalizationId == localizationId)?.Name ?? "no-exist translation";
                var orgNewName = OrganizationNewNames.ContainsKey(orgLanguageCode)
                    ? OrganizationNewNames[orgLanguageCode]
                    : "no-exist translation";
                
                org20.Name.Add(orgLanguageCode, $"{orgNewName} {orgNumber} ({organizationTypeName})");
                org20.ShortDescription.Add(orgLanguageCode, string.Format(OrganizationNewSummaryDescriptions[orgLanguageCode], orgNumber));
            }
      
            SetLanguageAvailabilities(languageCache, org20);
            models.Add(org20);
            
            //21
            orgNumber = 21;
            orgLanguageCodes = new List<string>(){"fi","sv","en"};
            var org21 = CreateEmptyOrganizationInput();
            
            org21.OrganizationType = typesCache.Get<OrganizationType>(OrganizationTypeEnum.Municipality.ToString());
            var helsinkiMunicipalityId = unitOfWork.CreateRepository<IMunicipalityRepository>().All().FirstOrDefault(m => m.Code == "091")?.Id;
            org21.Municipality = helsinkiMunicipalityId;
           
            foreach (var orgLanguageCode in orgLanguageCodes)
            {
                var organizationTypeNames = organizationTypeCacheData.First(x => x.Id == org21.OrganizationType).Names;
                var localizationId = languageCache.Get(orgLanguageCode);
                var organizationTypeName = organizationTypeNames.FirstOrDefault(x => x.LocalizationId == localizationId)?.Name ?? "no-exist translation";
                var orgNewName = OrganizationNewNames.ContainsKey(orgLanguageCode)
                    ? OrganizationNewNames[orgLanguageCode]
                    : "no-exist translation";
                
                org21.Name.Add(orgLanguageCode, $"{orgNewName} {orgNumber} ({organizationTypeName})");
                org21.ShortDescription.Add(orgLanguageCode, string.Format(OrganizationNewSummaryDescriptions[orgLanguageCode], orgNumber));
            }
      
            SetLanguageAvailabilities(languageCache, org21);
            models.Add(org21);
            
            //22
            orgNumber = 22;
            orgLanguageCodes = new List<string>(){"fi","sv","en"};
            var org22 = CreateEmptyOrganizationInput();
            
            org22.OrganizationType = typesCache.Get<OrganizationType>(OrganizationTypeEnum.Municipality.ToString());
            var akkaMunicipalityId = unitOfWork.CreateRepository<IMunicipalityRepository>().All().FirstOrDefault(m => m.Code == "020")?.Id;
            org22.Municipality = akkaMunicipalityId;
           
            foreach (var orgLanguageCode in orgLanguageCodes)
            {
                var organizationTypeNames = organizationTypeCacheData.First(x => x.Id == org22.OrganizationType).Names;
                var localizationId = languageCache.Get(orgLanguageCode);
                var organizationTypeName = organizationTypeNames.FirstOrDefault(x => x.LocalizationId == localizationId)?.Name ?? "no-exist translation";
                var orgNewName = OrganizationNewNames.ContainsKey(orgLanguageCode)
                    ? OrganizationNewNames[orgLanguageCode]
                    : "no-exist translation";
                
                org22.Name.Add(orgLanguageCode, $"{orgNewName} {orgNumber} ({organizationTypeName})");
                org22.ShortDescription.Add(orgLanguageCode, string.Format(OrganizationNewSummaryDescriptions[orgLanguageCode], orgNumber));
            }
      
            SetLanguageAvailabilities(languageCache, org22);
            models.Add(org22);
            
            //23
            orgNumber = 23;
            orgLanguageCodes = new List<string>(){"fi","sv","en"};
            var org23 = CreateEmptyOrganizationInput();
            
            org23.OrganizationType = typesCache.Get<OrganizationType>(OrganizationTypeEnum.State.ToString());
            org20.AreaInformation = new VmAreaInformation()
            {
                AreaInformationTypeId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString()),
            };
           
            foreach (var orgLanguageCode in orgLanguageCodes)
            {
                var organizationTypeNames = organizationTypeCacheData.First(x => x.Id == org23.OrganizationType).Names;
                var localizationId = languageCache.Get(orgLanguageCode);
                var organizationTypeName = organizationTypeNames.FirstOrDefault(x => x.LocalizationId == localizationId)?.Name ?? "no-exist translation";
                var orgNewName = OrganizationNewNames.ContainsKey(orgLanguageCode)
                    ? OrganizationNewNames[orgLanguageCode]
                    : "no-exist translation";
                
                org23.Name.Add(orgLanguageCode, $"{orgNewName} {orgNumber} ({organizationTypeName})");
                org23.ShortDescription.Add(orgLanguageCode, string.Format(OrganizationNewSummaryDescriptions[orgLanguageCode], orgNumber));
            }
      
            SetLanguageAvailabilities(languageCache, org23);
            models.Add(org23);
            
            return models;
        }
        
        private VmOrganizationInput CreateEmptyOrganizationInput()
        {
            return new VmOrganizationInput()
            {
                Action = ActionTypeEnum.Save,
                Name = new Dictionary<string, string>(),
                ShortDescription = new Dictionary<string, string>(),
                LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo>(),
                VisitingAddresses = new List<VmAddressSimple>(),
                PostalAddresses = new List<VmAddressSimple>(),
                Emails = new Dictionary<string, List<VmEmailData>>(),
                PhoneNumbers = new Dictionary<string, List<VmPhone>>(),
                WebPages = new  Dictionary<string, List<VmWebPage>>(),
                ElectronicInvoicingAddresses = new List<VmElectronicInvoicingAddress>()
            };
        }

        private static void SetLanguageAvailabilities(ILanguageCache languageCache, VmOrganizationInput firstOrg)
        {
            var langAvailabilities = new List<VmLanguageAvailabilityInfo>();
            foreach (var languageKey in firstOrg.Name.Keys)
            {
                var langAvailability = new VmLanguageAvailabilityInfo()
                {
                    LanguageId = languageCache.Get(languageKey)
                };
                langAvailabilities.Add(langAvailability);
            }

            firstOrg.LanguagesAvailabilities = langAvailabilities;
        }
      
        private void ChangeAllModifiedByAndCreatedBy(IUnitOfWorkWritable unitOfWork, string testEmail)
        {
            ChangeAllCreatedByAndModifiedBy<Address>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AddressStreet>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AddressPostOfficeBox>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AddressForeign>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AddressForeignTextName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AddressCharacter>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AddressCharacterName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AddressOther>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AddressType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AddressTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AppEnvironmentData>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AppEnvironmentDataType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AppEnvironmentDataTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AttachmentType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AttachmentTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<Attachment>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<Business>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<Country>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<CountryName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<DescriptionType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<DescriptionTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ElectronicChannel>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ElectronicChannelUrl>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ExceptionHoursStatusType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ExceptionHoursStatusTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ExternalSource>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<PrintableFormChannel>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<PrintableFormChannelUrl>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<PrintableFormChannelIdentifier>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<Form>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<FormState>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<FormType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<FormTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<Keyword>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<Language>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<LanguageName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<LifeEvent>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<LifeEventName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<Municipality>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<MunicipalityName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<NameType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<NameTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OntologyTerm>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OntologyTermName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OntologyTermDescription>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationVersioned>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationAddress>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationEInvoicing>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationEInvoicingAdditionalInformation>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationEmail>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationWebPage>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationDisplayNameType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationDescription>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationPhone>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationService>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceCollectionName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceCollectionDescription>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceCollectionService>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<PhoneNumberType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<PhoneNumberTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<PrintableFormChannelUrlType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<PrintableFormChannelUrlTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ProvisionType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ProvisionTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<PublishingStatusType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<PublishingStatusTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceVersioned>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceClass>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceClassName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceClassDescription>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<IndustrialClass>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<IndustrialClassName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelVersioned>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelAttachment>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChargeType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChargeTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelDescription>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelEmail>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelKeyword>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelOntologyTerm>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelPhone>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelServiceClass>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelServiceHours>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<DailyOpeningTime>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelTargetGroup>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelWebPage>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelLanguage>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceDescription>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceHourType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceHourTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceElectronicCommunicationChannel>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceElectronicNotificationChannel>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceHoursAdditionalInformation>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceKeyword>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceLanguage>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceLaw>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceLifeEvent>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<GeneralDescriptionType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceOntologyTerm>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceRequirement>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceServiceClass>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceIndustrialClass>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceTargetGroup>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<StreetName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<PostOfficeBoxName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<StatutoryServiceDescription>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<StatutoryServiceLanguage>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<StatutoryServiceLifeEvent>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<StatutoryServiceName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<StatutoryServiceOntologyTerm>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<StatutoryServiceServiceClass>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<StatutoryServiceTargetGroup>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<StatutoryServiceIndustrialClass>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<StatutoryServiceLaw>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<StatutoryServiceRequirement>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<TargetGroup>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<TargetGroupName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<WebpageChannel>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<WebpageChannelUrl>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<WebPage>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<WebPageType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<WebPageTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<Phone>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<Versioning>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<Law>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<LawName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<LawWebPage>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<DialCode>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<GeneralDescriptionLanguageAvailability>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceLanguageAvailability>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelLanguageAvailability>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationLanguageAvailability>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceCollectionLanguageAvailability>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<PostalCodeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<Coordinate>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<CoordinateType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<CoordinateTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OntologyTermExactMatch>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ExactMatch>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AreaInformationType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AreaInformationTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AreaType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AreaTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<Area>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AreaMunicipality>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AreaName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationArea>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationAreaMunicipality>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceArea>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceAreaMunicipality>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelArea>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelAreaMunicipality>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelConnectionType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelConnectionTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceFundingType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceFundingTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ExtraType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ExtraTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ExtraSubType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ExtraSubTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceServiceChannelExtraType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceServiceChannelExtraTypeDescription>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<GeneralDescriptionServiceChannelExtraType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<GeneralDescriptionServiceChannelExtraTypeDescription>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<BugReport>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceBlockedAccessRight>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ChannelBlockedAccessRight>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<OrganizationBlockedAccessRight>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<GeneralDescriptionBlockedAccessRight>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceCollectionBlockedAccessRight>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AccessRightType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AccessRightName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceProducer>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceProducerAdditionalInformation>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceProducerOrganization>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceHours>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceServiceChannelServiceHours>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceServiceChannelAddress>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceServiceChannelEmail>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceServiceChannelPhone>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceServiceChannelWebPage>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<SahaOrganizationInformation>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<TrackingServiceServiceChannel>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<TrackingServiceCollectionService>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<TrackingGeneralDescriptionServiceChannel>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<GeneralDescriptionTranslationOrder>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelTranslationOrder>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceTranslationOrder>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<TranslationCompany>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<TranslationOrder>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<TranslationOrderState>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<TranslationStateType>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<TranslationStateTypeName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<UserAccessRightsGroup>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<TasksFilter>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<TasksConfiguration>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<UserAccessRightsGroupName>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<UserOrganization>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<ServiceChannelAddress>(unitOfWork);
            ChangeAllCreatedByAndModifiedBy<AddressReceiver>(unitOfWork);
        }
        
        private void ChangeAllCreatedByAndModifiedBy<TEntity>(IUnitOfWork unitOfWork) where TEntity : class, IAuditing
        {
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entities = repository.All();

            foreach (var entity in entities)
            {
                SetModifiedAndCreatedBy(entity, testEmail);
            }
        }
        
        private static void SetModifiedAndCreatedBy<T>(T entity, string testEmail) where T : IAuditing
        {
            entity.ModifiedBy = testEmail;
            entity.CreatedBy = testEmail;
        }

        private void RemoveData(IDatabaseRawContext databaseRawContext, string tableName, string columnName, List<Guid> removeIds)
        {
            databaseRawContext.ExecuteWriter(unitOfDbWork =>
            {
                try
                {
                    var command = $"DELETE FROM \"{tableName}\" WHERE \"{columnName}\" = ANY(@data);";
                    unitOfDbWork.Command(command, new { data = removeIds});
                    unitOfDbWork.Save();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }

    }
}