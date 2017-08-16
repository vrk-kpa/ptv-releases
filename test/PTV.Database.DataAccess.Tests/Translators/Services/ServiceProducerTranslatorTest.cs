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
using System.Linq;
using FluentAssertions;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class ServiceProducerTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly IUnitOfWork unitOfWorkMock;

        public ServiceProducerTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            SetupTypesCacheMock<ProvisionType>(typeof(ProvisionTypeEnum));

            translators = new List<object>
            {
                new ServiceProducerTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
                new ServiceProducerOrganizationTranslator(ResolveManager, TranslationPrimitives),
                new ServiceProducerAdditionalInformationTranslator(ResolveManager, TranslationPrimitives)
            };

        }

        /// <summary>
        /// test for ServiceNameTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(ProvisionTypeEnum.SelfProduced, "54029E6A-2914-43DC-8816-12B190E056AD", "text")]
        [InlineData(ProvisionTypeEnum.SelfProduced, "54029E6A-2914-43DC-8816-12B190E056AD", "")]
        [InlineData(ProvisionTypeEnum.SelfProduced, null, null)]
        [InlineData(ProvisionTypeEnum.PurchaseServices, "54029E6A-2914-43DC-8816-12B190E056AD", "text")]
        [InlineData(ProvisionTypeEnum.PurchaseServices, "54029E6A-2914-43DC-8816-12B190E056AD", null)]
        [InlineData(ProvisionTypeEnum.PurchaseServices, null, null)]
        [InlineData(ProvisionTypeEnum.PurchaseServices, null, "text")]
        [InlineData(ProvisionTypeEnum.Other, "54029E6A-2914-43DC-8816-12B190E056AD", "text")]
        [InlineData(ProvisionTypeEnum.Other, null, "text")]
        [InlineData(ProvisionTypeEnum.Other, null, null)]
        public void TranslateServiceProducerEntityTest(ProvisionTypeEnum provisionType, string organizationId, string additionalInformation)
        {
            var model = CreateModel(provisionType, organizationId, additionalInformation);
            var toTranslate = new List<VmServiceProducer> { model };

            RegisterDbSet(new List<ServiceProducer>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceProducerAdditionalInformation>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceProducerOrganization>(), unitOfWorkMockSetup);
            
            var translations = RunTranslationModelToEntityTest<VmServiceProducer, ServiceProducer>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();
            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(provisionType, model, translation);
        }

        private void CheckTranslation(ProvisionTypeEnum provisionType, VmServiceProducer source, ServiceProducer target)
        {
            target.Id.Should().NotBe(Guid.Empty);
            target.ProvisionTypeId.Should().Be(source.ProvisionTypeId);
            switch (provisionType)
            {
                case ProvisionTypeEnum.SelfProduced:
                    target.AdditionalInformations.Count.Should().Be(0);
                    target.Organizations.Count.Should().Be(source.Organizers.Count);
                    if (source.Organizers.Count > 0)
                    {
                        target.Organizations.First().OrganizationId.Should().Be(source.Organizers.First()); 
                    }
                    break;

                case ProvisionTypeEnum.PurchaseServices:
                    if (source.OrganizationId.HasValue)
                    {
                        target.Organizations.Count.Should().Be(1);
                        target.Organizations.First().OrganizationId.Should().Be(source.OrganizationId.Value);
                    }
                    else
                    {
                        target.Organizations.Should().BeEmpty();
                    }

                    if (string.IsNullOrEmpty(source.AdditionalInformation))
                    {
                        target.AdditionalInformations.Should().BeEmpty();
                    }
                    else
                    {
                        var ai = target.AdditionalInformations.FirstOrDefault();
                        ai.Should().NotBeNull();
                        ai.Text.Should().Be(source.AdditionalInformation);
                        ai.LocalizationId.Should().Be(LanguageCode.fi.ToString().GetGuid());
                    }
                    break;

                case ProvisionTypeEnum.Other:
                    if (source.OrganizationId.HasValue)
                    {
                        target.Organizations.Count.Should().Be(1);
                        target.Organizations.First().OrganizationId.Should().Be(source.OrganizationId.Value);
                        target.AdditionalInformations.Should().BeEmpty();
                    }
                    else if (!string.IsNullOrEmpty(source.AdditionalInformation))
                    {
                        target.Organizations.Should().BeEmpty();
                        var ai = target.AdditionalInformations.FirstOrDefault();
                        ai.Should().NotBeNull();
                        ai.Text.Should().Be(source.AdditionalInformation);
                        ai.LocalizationId.Should().Be(LanguageCode.fi.ToString().GetGuid());
                    }
                    else
                    {
                        target.Organizations.Should().BeEmpty();
                        target.AdditionalInformations.Should().BeEmpty();
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(provisionType), provisionType, null);
            }

        }

        private VmServiceProducer CreateModel(ProvisionTypeEnum provisionType, string organizationId, string additionalInformation)
        {
            var producer = new VmServiceProducer
            {
                ProvisionTypeId = CacheManager.TypesCache.Get<ProvisionType>(provisionType.ToString()),
                AdditionalInformation = additionalInformation
            };

            if (provisionType == ProvisionTypeEnum.SelfProduced)
            {
                producer.Organizers = string.IsNullOrEmpty(organizationId)
                    ? new List<Guid>()
                    : new List<Guid>{Guid.Parse(organizationId)};
            }
            else
            {
                producer.OrganizationId = string.IsNullOrEmpty(organizationId)
                    ? (Guid?) null
                    : Guid.Parse(organizationId);
            }

            return producer;
        }
    }
}
