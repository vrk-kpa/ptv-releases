﻿/**
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

using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.OpenApi.Services;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiServiceAdditionalInformationTranslatorTest : TranslatorTestBase
    {
        private const string ADDITIONAL_INFORMATION = "AdditionalInformation";
        private const string SERVICE_DESRIPTION = "ServiceDescription";
        private const string SERVICE_DESRIPTION_MARKDOWN = SERVICE_DESRIPTION+"\n";
        private const string SERVICE_DESRIPTION_JSON = "{\"entityMap\":{},\"blocks\":[{\"text\":\"" + SERVICE_DESRIPTION + "\",\"type\":\"unstyled\",\"key\":\"FUU4G\",\"depth\":0,\"inlineStyleRanges\":[],\"entityRanges\":[]}]}";

        private readonly List<object> translators;

        public OpenApiServiceAdditionalInformationTranslatorTest()
        {
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));

            translators = new List<object>
            {
                new OpenApiServiceTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceInTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceDescriptionTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//                new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
//                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceChargeTypeCodeTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiServiceLanguageAvailabilityTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiOrganizationItemTranslator(ResolveManager, TranslationPrimitives, CacheManager)
            };
        }

        [Fact]
        public void TranslateEntityToVm()
        {
            var entity = CreateEntity();
            var toTranslate = new List<ServiceVersioned> { entity };
            var translations = RunTranslationEntityToModelTest<ServiceVersioned, VmOpenApiServiceVersionBase>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(entity, translation);
        }

        private void CheckTranslations(ServiceVersioned source, IVmOpenApiServiceBase target)
        {
            target.ServiceDescriptions.Count.Should().Be(2);

            // additionalInfo
            var additionalInfoTarget = target.ServiceDescriptions.Where(d => d.Type == DescriptionTypeEnum.ServiceTypeAdditionalInfo.ToString().GetOpenApiEnumValue<DescriptionTypeEnum>()).First();
            var additionalInfoSource = source.ServiceDescriptions.First(d => d.TypeId == CacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceTypeAdditionalInfo.ToString()));
            additionalInfoTarget.Language.Should().Be(CacheManager.LanguageCache.GetByValue(additionalInfoSource.LocalizationId));
            additionalInfoTarget.Value.Should().Be(additionalInfoSource.Description);

            // serviceDescription
            var serviceDescriptionTarget = target.ServiceDescriptions.First();
            var serviceDescriptionSource = source.ServiceDescriptions.First(d => d.TypeId == CacheManager.TypesCache.Get<DescriptionType>(serviceDescriptionTarget.Type));
            serviceDescriptionTarget.Language.Should().Be(CacheManager.LanguageCache.GetByValue(serviceDescriptionSource.LocalizationId));
            serviceDescriptionTarget.Value.Should().NotBeNull();
        }

        private ServiceVersioned CreateEntity()
        {

            return new ServiceVersioned
            {
                ServiceDescriptions = new List<ServiceDescription>
                {
                    new ServiceDescription
                    {
                        Description = SERVICE_DESRIPTION_JSON,
                        LocalizationId = CacheManager.LanguageCache.Get("fi".ToString()),
                        TypeId = CacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString())
                    },
                    new ServiceDescription
                    {
                        Description = ADDITIONAL_INFORMATION,
                        LocalizationId = CacheManager.LanguageCache.Get("fi".ToString()),
                        TypeId = CacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceTypeAdditionalInfo.ToString())
                    },
                }
            };
        }

        [Fact]
        public void TranslateVmToEntity()
        {
            var model = CreateViewModel();
            var toTranslate = new List<IVmOpenApiServiceInVersionBase> { model };
            var translations = RunTranslationModelToEntityTest<IVmOpenApiServiceInVersionBase, ServiceVersioned>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslations(model, translation);
        }

        private void CheckTranslations(IVmOpenApiServiceInVersionBase source, ServiceVersioned target)
        {
            target.ServiceDescriptions.Count.Should().Be(2);

            // additionalInfo
            var additionalInfoSource = source.ServiceDescriptions.Where(d => d.Type == DescriptionTypeEnum.ServiceTypeAdditionalInfo.GetOpenApiValue()).First();
            var additionalInfoTarget = target.ServiceDescriptions.First(d => d.TypeId == CacheManager.TypesCache.Get<DescriptionType>(additionalInfoSource.Type.GetEnumValueByOpenApiEnumValue(typeof(DescriptionTypeEnum))));
            additionalInfoTarget.LocalizationId.Should().Be(CacheManager.LanguageCache.Get(additionalInfoSource.Language));
            additionalInfoTarget.Description.Should().Be(additionalInfoSource.Value);

            // serviceDescription
            var serviceDescriptionSource = source.ServiceDescriptions.First();
            var serviceDescriptionTarget = target.ServiceDescriptions.First(d => d.TypeId == CacheManager.TypesCache.Get<DescriptionType>(serviceDescriptionSource.Type));
            serviceDescriptionTarget.LocalizationId.Should().Be(CacheManager.LanguageCache.Get(serviceDescriptionSource.Language));
            serviceDescriptionTarget.Description.Should().NotBeNull();
        }

        private IVmOpenApiServiceInVersionBase CreateViewModel()
        {
            return new VmOpenApiServiceInVersionBase
            {
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = "fi",
                        Type = DescriptionTypeEnum.Description.GetOpenApiValue(),
                        Value = SERVICE_DESRIPTION_MARKDOWN
                    },
                    new VmOpenApiLocalizedListItem
                    {
                        Language = "fi",
                        Type = DescriptionTypeEnum.ServiceTypeAdditionalInfo.GetOpenApiValue(),
                        Value = ADDITIONAL_INFORMATION
                    }
                }
            };
        }
    }
}
