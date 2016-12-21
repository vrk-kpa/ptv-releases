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
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Database.DataAccess.Translators.OpenApi.Finto;
using PTV.Database.DataAccess.Translators.OpenApi.Services;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Tests.TestHelpers;
using FluentAssertions;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiServiceAdditionalInformationTranslatorTest : TranslatorTestBase
    {


        private const string ADDITIONAL_INFORMATION = "AdditionalInformation";
        private const string SERVICE_DESRIPTION = "ServiceDescription";


        private readonly List<object> translators;
        private readonly ICacheManager cacheManager;

        public OpenApiServiceAdditionalInformationTranslatorTest()
        {
            var languageCache = SetupLanguageCacheMock().Object;
            var typesCacheMock = SetupTypesCacheMock<DescriptionType>(typesEnum: typeof(DescriptionTypeEnum));
            SetupTypesCacheMock<ServiceCoverageType>(typesCacheMock, typeof(ServiceCoverageTypeEnum));
            cacheManager = SetupCacheManager(languageCache, typesCacheMock.Object).Object;

            translators = new List<object>
            {
                new OpenApiServiceTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceInTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new OpenApiServiceDescriptionTranslator(ResolveManager, TranslationPrimitives, cacheManager),
                new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new ServiceCoverageTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChargeTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new ServiceTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives)
            };
        }

        [Fact]
        public void TranslateEntityToVm()
        {
            var entity = CreateEntity();
            var toTranslate = new List<Service>() { entity };
            var translations = RunTranslationEntityToModelTest<Service, V2VmOpenApiService>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(entity, translation);
        }

        private void CheckTranslations(Service source, IVmOpenApiServiceBase target)
        {
            target.ServiceDescriptions.Count.Should().Be(1);
            target.ServiceAdditionalInformations.Count.Should().Be(1);

            // additionalInfo
            var additionalInfoTarget = target.ServiceAdditionalInformations.First();
            var additionalInfoSource = source.ServiceDescriptions.First(d => d.TypeId == cacheManager.TypesCache.Get<DescriptionType>(additionalInfoTarget.Type));
            additionalInfoTarget.Language.Should().Be(cacheManager.LanguageCache.GetByValue(additionalInfoSource.LocalizationId));
            additionalInfoTarget.Value.Should().Be(additionalInfoSource.Description);

            // serviceDescription
            var serviceDescriptionTarget = target.ServiceDescriptions.First();
            var serviceDescriptionSource = source.ServiceDescriptions.First(d => d.TypeId == cacheManager.TypesCache.Get<DescriptionType>(serviceDescriptionTarget.Type));
            serviceDescriptionTarget.Language.Should().Be(cacheManager.LanguageCache.GetByValue(serviceDescriptionSource.LocalizationId));
            serviceDescriptionTarget.Value.Should().Be(serviceDescriptionSource.Description);
        }

        private Service CreateEntity()
        {

            return new Service
            {
                ServiceCoverageTypeId = cacheManager.TypesCache.Get<ServiceCoverageType>(ServiceCoverageTypeEnum.Local.ToString()),
                ServiceDescriptions = new List<ServiceDescription>
                {
                    new ServiceDescription
                    {
                        Description = SERVICE_DESRIPTION,
                        LocalizationId = cacheManager.LanguageCache.Get(LanguageCode.fi.ToString()),
                        TypeId = cacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString())
                    },
                    new ServiceDescription
                    {
                        Description = ADDITIONAL_INFORMATION,
                        LocalizationId = cacheManager.LanguageCache.Get(LanguageCode.fi.ToString()),
                        TypeId = cacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceTypeAdditionalInfo.ToString())
                    },
                }
            };
        }

        [Fact]
        public void TranslateVmToEntity()
        {
            var model = CreateViewModel();
            var toTranslate = new List<IV2VmOpenApiServiceInBase> { model };
            var translations = RunTranslationModelToEntityTest<IV2VmOpenApiServiceInBase, Service>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(model, translation);
        }

        private void CheckTranslations(IV2VmOpenApiServiceInBase source, Service target)
        {
            target.ServiceDescriptions.Count.Should().Be(2);

            // additionalInfo
            var additionalInfoSource = source.ServiceAdditionalInformations.First();
            var additionalInfoTarget = target.ServiceDescriptions.First(d => d.TypeId == cacheManager.TypesCache.Get<DescriptionType>(additionalInfoSource.Type));
            additionalInfoTarget.LocalizationId.Should().Be(cacheManager.LanguageCache.Get(additionalInfoSource.Language));
            additionalInfoTarget.Description.Should().Be(additionalInfoSource.Value);

            // serviceDescription
            var serviceDescriptionSource = source.ServiceDescriptions.First();
            var serviceDescriptionTarget = target.ServiceDescriptions.First(d => d.TypeId == cacheManager.TypesCache.Get<DescriptionType>(serviceDescriptionSource.Type));
            serviceDescriptionTarget.LocalizationId.Should().Be(cacheManager.LanguageCache.Get(serviceDescriptionSource.Language));
            serviceDescriptionTarget.Description.Should().Be(serviceDescriptionSource.Value);
        }

        private IVmOpenApiServiceInBase CreateViewModel()
        {
            return new VmOpenApiServiceIn
            {
                ServiceAdditionalInformations = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = LanguageCode.fi.ToString(),
                        Type = DescriptionTypeEnum.ServiceTypeAdditionalInfo.ToString(),
                        Value = ADDITIONAL_INFORMATION
                    }
                },

                ServiceDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = LanguageCode.fi.ToString(),
                        Type = DescriptionTypeEnum.Description.ToString(),
                        Value = SERVICE_DESRIPTION
                    }
                }
            };
        }
    }
}
