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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.OpenApi.Finto;
using PTV.Database.DataAccess.Translators.OpenApi.Services;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using PTV.Database.DataAccess.Caches;
using System;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Models.OpenApi;
using FluentAssertions;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiGeneralDescriptionTranslatorTests : TranslatorTestBase
    {
        private const string GENERAL_DESCRIPTION = "This is general description.";
        private const string GENERAL_DESCRIPTION_MARKDOWN = GENERAL_DESCRIPTION + "\n";
        private const string GENERAL_DESCRIPTION_JSON = "{\"entityMap\":{},\"blocks\":[{\"text\":\"" + GENERAL_DESCRIPTION + "\",\"type\":\"unstyled\",\"key\":\"FUU4G\",\"depth\":0,\"inlineStyleRanges\":[],\"entityRanges\":[]}]}";

        private const string NAME = "Statutory name";
        private const string SERVICE_CLASS_URI = "http://urn.fi/URN:NBN:fi:au:ptvl:P22";
        private const string ONTOLOGY_TERM_URI = "http://www.yso.fi/onto/yso/p6548";
        private const string TARGET_GROUP_URI = "http://urn.fi/URN:NBN:fi:au:ptvl:KR3";
        private const string LIFE_EVENT_URI = "http://urn.fi/URN:NBN:fi:au:ptvl:KE5";

        private List<object> translators;

        public OpenApiGeneralDescriptionTranslatorTests()
        {
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));
            SetupTypesCacheMock<ServiceType>(typeof(ServiceTypeEnum));

            translators = new List<object>()
            {
                new OpenApiGeneralDescriptionInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiStatutoryServiceNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiGeneralDescriptionTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiStatutoryServiceDescriptionTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceClassTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceClassNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiLifeEventTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiLifeEventNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiOntologyTermTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOntologyTermNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiTargetGroupTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiTargetGroupNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new NameTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
            };
        }

        [Fact]
        public void TranslateGeneralDescriptionToVm()
        {
            var unitOfWorkMock = unitOfWorkMockSetup.Object;
            var generalDescription = CreateGeneralDescription();
            var toTranslate = new List<StatutoryServiceGeneralDescriptionVersioned>() { generalDescription };

            var translators = new List<object>()
            {
                new OpenApiGeneralDescriptionInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiStatutoryServiceNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiGeneralDescriptionTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceClassTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceClassNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiLifeEventTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiLifeEventNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiOntologyTermTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOntologyTermNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiTargetGroupTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiTargetGroupNameTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new NameTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceClassWithDescriptionTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceDescription, VmOpenApiLocalizedListItem>>(), unitOfWorkMock, description => new VmOpenApiLocalizedListItem() { Value = GENERAL_DESCRIPTION_MARKDOWN, Type= DescriptionTypeEnum.Description.ToString() }  )
            };

            RegisterDbSet(new List<VmOpenApiGeneralDescriptionVersionBase>(), unitOfWorkMockSetup);

            var translations = RunTranslationEntityToModelTest<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(generalDescription, translation);
        }

        private void CheckTranslations(StatutoryServiceGeneralDescriptionVersioned source, VmOpenApiGeneralDescriptionVersionBase target)
        {
            target.Names.First().Value.Should().Be(source.Names.First().Name);
            target.Descriptions.First().Value.Should().NotBeSameAs(source.Descriptions.First().Description);
            target.Descriptions.First().Type.Should().Be(CacheManager.TypesCache.GetByValue<DescriptionType>(source.Descriptions.First().TypeId));
            target.ServiceClasses.Count.Should().Be(1);
            target.TargetGroups.First().Name.First().Value.Should().Be(source.TargetGroups.First().TargetGroup.Names.First().Name);
            target.OntologyTerms.First().Uri.Should().Be(source.OntologyTerms.First().OntologyTerm.Uri);
            target.ServiceClasses.First().Name.First().Value.Should().Be(source.ServiceClasses.First().ServiceClass.Names.First().Name);
            target.Languages.Count.Should().Be(2);
        }

        [Fact]
        public void TranslateVmGeneralDescriptionToEntity()
        {
            var unitOfWorkMock = unitOfWorkMockSetup.Object;
            SetupTypesCacheMock<NameType>();
            SetupTypesCacheMock<ServiceChargeType>();

            var model = CreateGeneralDescriptionInModel();
            var toTranslate = new List<IVmOpenApiGeneralDescriptionInVersionBase>() { model };
            var translators = new List<object>()
            {
                new OpenApiGeneralDescriptionInTranslator(ResolveManager, TranslationPrimitives, CacheManager),

                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceName, VmOpenApiLocalizedListItem>>(), unitOfWorkMock, name => new StatutoryServiceName {Name = NAME}),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceDescription, VmOpenApiLocalizedListItem>>(), unitOfWorkMock, description => new StatutoryServiceDescription {Description = GENERAL_DESCRIPTION}),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceLanguage, VmOpenApiStringItem>>(), unitOfWorkMock, item => new StatutoryServiceLanguage {Language = new Language {Code = item.Value} }),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceServiceClass, VmOpenApiStringItem>>(), unitOfWorkMock, item => new StatutoryServiceServiceClass {ServiceClass = new ServiceClass { Uri = item.Value}}),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceOntologyTerm, VmOpenApiStringItem>>(), unitOfWorkMock, item => new StatutoryServiceOntologyTerm {OntologyTerm = new OntologyTerm {Uri = item.Value}}),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceTargetGroup, VmOpenApiStringItem>>(), unitOfWorkMock, item => new StatutoryServiceTargetGroup {TargetGroup = new TargetGroup {Uri = item.Value}}),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceLifeEvent, VmOpenApiStringItem>>(), unitOfWorkMock, item => new StatutoryServiceLifeEvent {LifeEvent = new LifeEvent {Uri = item.Value}}),
                RegisterTranslatorMock(new Mock<ITranslator<GeneralDescriptionLanguageAvailability, VmOpenApiLanguageAvailability>>(), unitOfWorkMock, item => new GeneralDescriptionLanguageAvailability {Language = new Language {Code = item.Language} }),
            };

            RegisterDbSet(new List<StatutoryServiceGeneralDescriptionVersioned>(), unitOfWorkMockSetup);

            var translations = RunTranslationModelToEntityTest<IVmOpenApiGeneralDescriptionInVersionBase, StatutoryServiceGeneralDescriptionVersioned>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            translation.Should().NotBeNull();
            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(model, translation);
        }

        private void CheckTranslations(IVmOpenApiGeneralDescriptionInVersionBase source, StatutoryServiceGeneralDescriptionVersioned target)
        {
            target.Names.First().Name.Should().Be(source.Names.First().Value);
            target.Descriptions.First().Description.Should().Be(source.Descriptions.First().Value);
            target.Languages.FirstOrDefault(lc => lc.Language.Code == LanguageCode.fi.ToString()).Should().NotBeNull();
            target.Languages.FirstOrDefault(lc => lc.Language.Code == LanguageCode.sv.ToString()).Should().NotBeNull();
            target.ServiceClasses.First().ServiceClass.Uri.Should().Be(source.ServiceClasses.First());
            target.OntologyTerms.First().OntologyTerm.Uri.Should().Be(source.OntologyTerms.First());
            target.TargetGroups.First().TargetGroup.Uri.Should().Be(source.TargetGroups.First());
            target.LifeEvents.First().LifeEvent.Uri.Should().Be(source.LifeEvents.First());
        }

        private StatutoryServiceGeneralDescriptionVersioned CreateGeneralDescription()
        {
            return new StatutoryServiceGeneralDescriptionVersioned()
            {
                Descriptions = new List<StatutoryServiceDescription>()
                {
                    new StatutoryServiceDescription()
                    {
                        Description = GENERAL_DESCRIPTION_JSON,
                        Localization = TestDataFactory.LocalizationFi(),
                        TypeId = CacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()),
                    }
                },
                Names = new List<StatutoryServiceName>()
                {
                    new StatutoryServiceName()
                    {
                        Name = NAME,
                        Localization = TestDataFactory.LocalizationFi(),
                        Type = new NameType()
                        {
                            Code = NameTypeEnum.Name.ToString()
                        }
                    }
                },
                ServiceClasses = new List<StatutoryServiceServiceClass>()
                {
                    new StatutoryServiceServiceClass()
                    {
                        ServiceClass = TestDataFactory.CreateServiceClass()
                    }
                },
                TargetGroups = new List<StatutoryServiceTargetGroup>()
                {
                    new StatutoryServiceTargetGroup()
                    {
                       TargetGroup = TestDataFactory.CreateTargetGroup()
                    }
                },
                OntologyTerms = new List<StatutoryServiceOntologyTerm>()
                {
                    new StatutoryServiceOntologyTerm()
                    {
                        OntologyTerm = TestDataFactory.CreateOntologyTerm()
                    }
                },
                LifeEvents = new List<StatutoryServiceLifeEvent>()
                {
                    new StatutoryServiceLifeEvent()
                    {
                        LifeEvent = TestDataFactory.CreateLifeEvent()
                    }
                },
                Languages = new List<StatutoryServiceLanguage>() {
                    new StatutoryServiceLanguage() { Language = TestDataFactory.LocalizationFi() },
                    new StatutoryServiceLanguage() { Language = TestDataFactory.LocalizationSv() }
                }
            };
        }

        private static IVmOpenApiGeneralDescriptionInVersionBase CreateGeneralDescriptionInModel()
        {
            return new VmOpenApiGeneralDescriptionIn()
            {

                Names = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{Language = LanguageCode.fi.ToString(), Type =  NameTypeEnum.Name.ToString(), Value = NAME}
                },

                Descriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{Language = LanguageCode.fi.ToString(), Type =  DescriptionTypeEnum.Description.ToString(), Value = GENERAL_DESCRIPTION}
                },

                Languages = new List<string>
                {
                    LanguageCode.fi.ToString(),
                    LanguageCode.sv.ToString()
                },

                ServiceClasses = new List<string> {SERVICE_CLASS_URI},
                OntologyTerms = new List<string> {ONTOLOGY_TERM_URI},
                TargetGroups = new List<string> {TARGET_GROUP_URI},
                LifeEvents = new List<string> {LIFE_EVENT_URI}
            };
        }
    }
}