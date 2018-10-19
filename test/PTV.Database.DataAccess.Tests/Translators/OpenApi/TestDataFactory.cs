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
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    internal static class TestDataFactory
    {
        public const string STREET_ADDRESS = "Mannerheimintie 5";
        public const string MUNICIPALITY = "Mikkeli";
        public const string WEBPAGE_NAME = "WebPageName";
        public const string URI = "TestUri";
        public const string LIFE_EVENT_NAME = "TestLifeEvent";
        public const string INDUSTRIAL_CLASS_NAME = "TestIndustrialClass";
        public const string ONTOLOGY_TERM_NAME = "OntologyName";
        public const string ONTOLOGY_TYPE = "skos:Concept";
        public const string SERVICE_CLASS_NAME = "Service class name";
        public const string TARGET_GROUP_NAME = "Target group name";
        public const string ATTACHMENT_NAME = "Attachment name";
        public const string TEXT = "Text";
        public const string POSTAL_CODE = "00100";
        public const string EMAIL = "some@one.com";
        public const string PHONE_NUMBER = "1234567890";
        public const string FAX_NUMBER = "0987654321";
        public const string SERVICE_CHARGE_TYPE = "Free";
        public const string SERVICE_TYPE = "Service";

        public static Language LocalizationFi()
        {
            return new Language() { Code = "fi" };
        }

        public static Language LocalizationSv()
        {
            return new Language() { Code = "sv" };
        }

        public static Address CreateAddress(Guid typeId)
        {
            return new Address()
            {
                TypeId = typeId,
                AddressStreets = new List<AddressStreet>()
                {
                    new AddressStreet()
                    {
                        StreetNames = new List<StreetName>()
                        {
                            new StreetName()
                            {
                                Name = STREET_ADDRESS,
                            }
                        },
                        PostalCode = new PostalCode()
                        {
                            Code = POSTAL_CODE,
                            PostalCodeNames = new List<PostalCodeName>()
                            {
                                new PostalCodeName
                                {
                                    Name = "Helsinki",
                                    Localization = LocalizationFi()
                                },
                                new PostalCodeName
                                {
                                    Name = "Helsingfors",
                                    Localization = LocalizationSv()
                                }
                            }
                        },
                        Address = new Address()
                    }
                }
                ,
                Country = new Country()
                {
                    Code = "fi",
                    CountryNames = new List<CountryName>()
                            {
                                new CountryName()
                                {
                                    Name = "Suomi",
                                }
                            }
                }
            };
        }

        public static Municipality CreateMunicipality()
        {
            return new Municipality() { MunicipalityNames = new List<MunicipalityName>() { new MunicipalityName { Name = MUNICIPALITY } } };
        }

        public static WebPage CreateWebPage()
        {
            return new WebPage()
            {
                Name = WEBPAGE_NAME,
                Localization = LocalizationFi(),
                Url = "http://webpage.com"
            };
        }

        public static LifeEvent CreateLifeEvent()
        {
            var lifeEvent = CreateFintoItemBase<LifeEvent>(LIFE_EVENT_NAME);
            lifeEvent.Names = new List<LifeEventName>() { CreateFiName<LifeEventName>(LIFE_EVENT_NAME) };
            return lifeEvent;
        }

        public static IndustrialClass CreateIndustrialClass()
        {
            var industrialClass = CreateFintoItemBase<IndustrialClass>(INDUSTRIAL_CLASS_NAME);
            industrialClass.Names = new List<IndustrialClassName>() { CreateFiName<IndustrialClassName>(INDUSTRIAL_CLASS_NAME) };
            return industrialClass;
        }

        public static OntologyTerm CreateOntologyTerm()
        {
            var term = CreateFintoItemBase<OntologyTerm>(ONTOLOGY_TERM_NAME);
            term.Names = new List<OntologyTermName>() { CreateFiName<OntologyTermName>(ONTOLOGY_TERM_NAME) };
            return term;
        }

        public static ServiceClass CreateServiceClass()
        {
            var serviceClass = CreateFintoItemBase<ServiceClass>(SERVICE_CLASS_NAME);
            serviceClass.Names = new List<ServiceClassName>() { CreateFiName<ServiceClassName>(SERVICE_CLASS_NAME) };
            return serviceClass;
        }

        public static TargetGroup CreateTargetGroup()
        {
            var group = CreateFintoItemBase<TargetGroup>(TARGET_GROUP_NAME);
            group.Names = new List<TargetGroupName>() { CreateFiName<TargetGroupName>(TARGET_GROUP_NAME) };
            return group;
        }

        public static Attachment CreateAttachment()
        {
            return new Attachment()
            {
                Name = ATTACHMENT_NAME,
                Description = "Description",
                Type = new AttachmentType() { Code = AttachmentTypeEnum.Attachment.ToString() },
                Url = "http://someurl.com/folder/file.pdf",
                Localization = LocalizationFi()
            };
        }

        public static VmOpenApiAttachment CreateAttachmentVm()
        {
            return new VmOpenApiAttachment()
            {
                Name = ATTACHMENT_NAME,
                Description = "Description",
                Url = "http://someurl.com/folder/file.pdf",
                Language = "fi"
            };
        }

        public static IList<VmOpenApiLocalizedListItem> CreateLocalizedList(string type)
        {
            return new List<VmOpenApiLocalizedListItem>()
            {
                CreateLocalizedListItem(type)
                //new VmOpenApiLocalizedListItem() { Value = TEXT, Type = type, Language = "fi" }
            };
        }

        public static IList<VmOpenApiLanguageItem> CreateLanguageItemList()
        {
            return new List<VmOpenApiLanguageItem>()
            {
                new VmOpenApiLanguageItem() { Value = TEXT, Language = "fi" }
            };
        }

        public static IList<VmOpenApiListItem> CreateListItemList(string type)
        {
            return new List<VmOpenApiListItem>() { CreateListItem(type) };
        }

        public static V7VmOpenApiAddressDeliveryIn CreateAddressVm()
        {
            return new V7VmOpenApiAddressDeliveryIn()
            {
                SubType = AddressTypeEnum.Street.ToString(),
                StreetAddress = new VmOpenApiAddressStreetIn()
                {
                    Street = CreateLanguageItemList(),
                    StreetNumber = "1",
                    PostalCode = POSTAL_CODE,
                    Municipality = MUNICIPALITY,
                    AdditionalInformation = CreateLanguageItemList()
                }
            };
        }
        
        public static IList<V8VmOpenApiAddressDeliveryIn> CreateListOfAddressVm()
        {
            return new List<V8VmOpenApiAddressDeliveryIn>
            {
                new V8VmOpenApiAddressDeliveryIn
                {
                    SubType = AddressTypeEnum.Street.ToString(),
                    StreetAddress = new VmOpenApiAddressStreetIn
                    {
                        Street = CreateLanguageItemList(),
                        StreetNumber = "1",
                        PostalCode = POSTAL_CODE,
                        Municipality = MUNICIPALITY,
                        AdditionalInformation = CreateLanguageItemList()
                    },
                   FormReceiver = CreateLanguageItemList()
                }
            };
        }

        public static TModel CreateAddressVm<TModel>(AddressTypeEnum addressType)
            where TModel : IVmOpenApiAddressInVersionBase, new()
        {
            var vm = new TModel()
            {
                Type = AddressCharacterEnum.Postal.ToString(),
                SubType = addressType.ToString(),
                Country = "fi",
            };

            switch (addressType)
            {
                case AddressTypeEnum.Street:
                    vm.StreetAddress = new VmOpenApiAddressStreetWithCoordinatesIn
                    {
                        Street = CreateLanguageItemList(),
                        StreetNumber = "1",
                        PostalCode = POSTAL_CODE,
                        Municipality = MUNICIPALITY,
                        AdditionalInformation = CreateLanguageItemList() 
                    };
                    break;
                case AddressTypeEnum.PostOfficeBox:
                    vm.PostOfficeBoxAddress = new VmOpenApiAddressPostOfficeBoxIn
                    {
                        PostOfficeBox = CreateLanguageItemList(),
                        PostalCode = POSTAL_CODE,
                        Municipality = MUNICIPALITY,
                        AdditionalInformation = CreateLanguageItemList()
                    };
                    break;
                case AddressTypeEnum.Foreign:
                    vm.ForeignAddress = CreateLanguageItemList();
                    break;
                default:
                    break;
            }

            return vm;
        }

        public static IList<TModel> CreateWebPageListVm<TModel>() where TModel : IVmOpenApiWebPageVersionBase, new()
        {
            return new List<TModel>()
            {
                new TModel()
                {
                    Url = TEXT,
                    Value = TEXT,
                    Language = "fi",
                    //Type = WebPageTypeEnum.HomePage.ToString()
                }
            };
        }

        public static List<VmOpenApiItem> CreateVmItemList(int count)
        {
            var list = new List<VmOpenApiItem>();
            for (var i = 0; i < count; i++)
            {
                list.Add(new VmOpenApiItem());
            }

            return list;
        }

        
        private static T CreateFintoItemBase<T>(string name) where T : FintoItemBase, new()
        {
            return new T()
            {
                Label = name,
                OntologyType = ONTOLOGY_TYPE,
                Code = "TestCode",
                Uri = URI,
                IsValid = true
            };
        }

        private static T CreateFiName<T>(string name) where T: NameBase, new()
        {
            return new T()
            {
                Name = name,
                Localization = LocalizationFi()
            };
        }

        private static VmOpenApiListItem CreateListItem(string type)
        {
            return new VmOpenApiListItem()
            {
                Value = TEXT,
                Type = type
            };
        }

        private static VmOpenApiLocalizedListItem CreateLocalizedListItem(string type)
        {
            return new VmOpenApiLocalizedListItem()
            {
                Value = TEXT,
                Type = type,
                Language = "fi"
            };
        }
    }
}
