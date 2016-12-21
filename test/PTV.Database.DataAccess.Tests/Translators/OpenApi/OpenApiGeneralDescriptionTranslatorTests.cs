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
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using FluentAssertions;
using PTV.Database.DataAccess.Translators.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiGeneralDescriptionTranslatorTests : TranslatorTestBase
    {
        private const string GENERAL_DESCRIPTION = "This is general description.";
        private const string NAME = "Statutory name";
        private const string SERVICE_CLASS_URI = "http://urn.fi/URN:NBN:fi:au:ptvl:P22";
        private const string ONTOLOGY_TERM_URI = "http://www.yso.fi/onto/yso/p6548";
        private const string TARGET_GROUP_URI = "http://urn.fi/URN:NBN:fi:au:ptvl:KR3";
        private const string LIFE_EVENT_URI = "http://urn.fi/URN:NBN:fi:au:ptvl:KE5";

        private List<object> translators;
        private readonly ICacheManager cacheManager;

        public OpenApiGeneralDescriptionTranslatorTests()
        {
            var languageCache = SetupLanguageCacheMock().Object;
            var typesCacheMock = SetupTypesCacheMock<DescriptionType>(typesEnum: typeof(DescriptionTypeEnum));
            SetupTypesCacheMock<ServiceType>(typesCacheMock, typeof(ServiceTypeEnum));

            cacheManager = SetupCacheManager(languageCache, typesCacheMock.Object).Object;

            translators = new List<object>()
            {
                new OpenApiGeneralDescriptionInTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiStatutoryServiceNameTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiGeneralDescriptionTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiStatutoryServiceDescriptionTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceClassTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiLifeEventTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOntologyTermTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiTargetGroupTranslator(ResolveManager, TranslationPrimitives),
                new NameTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives)
            };
        }

        [Fact]
        public void TranslateGeneralDescriptionToVm()
        {
            var generalDescription = CreateGeneralDescription();
            var toTranslate = new List<StatutoryServiceGeneralDescription>() { generalDescription };

            var list = new List<StatutoryServiceGeneralDescription>() { generalDescription };
            var translations = RunTranslationEntityToModelTest<StatutoryServiceGeneralDescription, V2VmOpenApiGeneralDescription>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(generalDescription, translation);
        }

        private void CheckTranslations(StatutoryServiceGeneralDescription source, V2VmOpenApiGeneralDescription target)
        {
            target.Names.First().Value.Should().Be(source.Names.First().Name);
            target.Descriptions.First().Value.Should().Be(source.Descriptions.First().Description);
            target.Descriptions.First().Type.Should().Be(cacheManager.TypesCache.GetByValue<DescriptionType>(source.Descriptions.First().TypeId));
            target.ServiceClasses.Count.Should().Be(1);
            target.TargetGroups.First().Name.Should().Be(source.TargetGroups.First().TargetGroup.Names.First().Name);
            target.OntologyTerms.First().Uri.Should().Be(source.OntologyTerms.First().OntologyTerm.Uri);
            target.ServiceClasses.First().Name.Should().Be(source.ServiceClasses.First().ServiceClass.Names.First().Name);
            target.Languages.Count.Should().Be(2);
        }

        [Fact]
        public void TranslateVmGeneralDescriptionToEntity()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            var unitOfWorkMock = unitOfWorkMockSetup.Object;
            var languageCache = SetupLanguageCacheMock().Object;
            var typesCacheMock = SetupTypesCacheMock<NameType>();
            SetupTypesCacheMock<ServiceChargeType>(typesCacheMock);
            var cacheManager = SetupCacheManager(languageCache, typesCacheMock.Object).Object;

            var model = CreateGeneralDescriptionInModel();
            var toTranslate = new List<IVmOpenApiGeneralDescriptionIn>() { model };
            var translators = new List<object>()
            {
                new OpenApiGeneralDescriptionInTranslator(ResolveManager, TranslationPrimitives, cacheManager),

                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceName, VmOpenApiLocalizedListItem>>(), unitOfWorkMock, name => new StatutoryServiceName {Name = NAME}),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceDescription, VmOpenApiLocalizedListItem>>(), unitOfWorkMock, description => new StatutoryServiceDescription {Description = GENERAL_DESCRIPTION}),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceLanguage, string>>(), unitOfWorkMock, languageCode => new StatutoryServiceLanguage {Language = new Language {Code = languageCode} }),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceServiceClass, string>>(), unitOfWorkMock, uri => new StatutoryServiceServiceClass {ServiceClass = new ServiceClass { Uri = uri}}),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceOntologyTerm, string>>(), unitOfWorkMock, uri => new StatutoryServiceOntologyTerm {OntologyTerm = new OntologyTerm {Uri = uri}}),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceTargetGroup, string>>(), unitOfWorkMock, uri => new StatutoryServiceTargetGroup {TargetGroup = new TargetGroup {Uri = uri}}),
                RegisterTranslatorMock(new Mock<ITranslator<StatutoryServiceLifeEvent, string>>(), unitOfWorkMock, uri => new StatutoryServiceLifeEvent {LifeEvent = new LifeEvent {Uri = uri}})
            };

            RegisterDbSet(new List<StatutoryServiceGeneralDescription>(), unitOfWorkMockSetup);

            var translations = RunTranslationModelToEntityTest<IVmOpenApiGeneralDescriptionIn, StatutoryServiceGeneralDescription>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            translation.Should().NotBeNull();
            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(model, translation);
        }

        private void CheckTranslations(IVmOpenApiGeneralDescriptionIn source, StatutoryServiceGeneralDescription target)
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

        private StatutoryServiceGeneralDescription CreateGeneralDescription()
        {
            return new StatutoryServiceGeneralDescription()
            {
                Descriptions = new List<StatutoryServiceDescription>()
                {
                    new StatutoryServiceDescription()
                    {
                        Description = GENERAL_DESCRIPTION,
                        Localization = TestDataFactory.LocalizationFi(),
                        TypeId = cacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()),
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

        private static IVmOpenApiGeneralDescriptionIn CreateGeneralDescriptionInModel()
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