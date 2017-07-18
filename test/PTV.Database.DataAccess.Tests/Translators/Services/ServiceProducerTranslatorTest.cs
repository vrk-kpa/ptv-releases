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
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.DataAccess.Translators.Types;
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
            SetupTypesCacheMock<RoleType>(typeof(RoleTypeEnum));

            translators = new List<object>
            {
                new ServiceProducerTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
                new ProvisionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new RoleTypeCodeTranslator(ResolveManager, TranslationPrimitives),
            };

        }

        /// <summary>
        /// test for ServiceNameTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(ProvisionTypeEnum.SelfProduced, "54029E6A-2914-43DC-8816-12B190E056AD", null, "text", "link")]
        [InlineData(ProvisionTypeEnum.SelfProduced, "54029E6A-2914-43DC-8816-12B190E056AD", "A39E9CB2-7B0C-4452-A7C0-AAB2C9B12F59", "", "")]
        [InlineData(ProvisionTypeEnum.SelfProduced, null, null, null, null)]
        [InlineData(ProvisionTypeEnum.PurchaseServices, "54029E6A-2914-43DC-8816-12B190E056AD", null, "text", "link")]
        [InlineData(ProvisionTypeEnum.PurchaseServices, "54029E6A-2914-43DC-8816-12B190E056AD", "A39E9CB2-7B0C-4452-A7C0-AAB2C9B12F59", null, "link")]
        [InlineData(ProvisionTypeEnum.PurchaseServices, null, null, null, null)]
        [InlineData(ProvisionTypeEnum.PurchaseServices, null, null, "text", null)]
        [InlineData(ProvisionTypeEnum.Other, "54029E6A-2914-43DC-8816-12B190E056AD", null, "text", "link")]
        [InlineData(ProvisionTypeEnum.Other, "54029E6A-2914-43DC-8816-12B190E056AD", "A39E9CB2-7B0C-4452-A7C0-AAB2C9B12F59", "text", "link")]
        [InlineData(ProvisionTypeEnum.Other, null, null, null, null)]
        public void TranslateServiceProducerEntityTest(ProvisionTypeEnum provisionType, string organization, string subOrganization, string freeText, string link)
        {
            var detail = new VmServiceProducerDetail
            {
                ProducerOrganizationId = string.IsNullOrEmpty(organization) ? (Guid?)null : Guid.Parse(organization),
                SubProducerOrganizationId = string.IsNullOrEmpty(subOrganization) ? (Guid?)null : Guid.Parse(subOrganization),
                FreeDescription = freeText,
                Link = link
            };
            var model = CreateModel(provisionType, detail);
            var toTranslate = new List<VmServiceProducer>() { model };

            RegisterDbSet(new List<OrganizationService>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<WebPage>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<OrganizationServiceAdditionalInformation>(), unitOfWorkMockSetup);

            translators.Add(RegisterTranslatorMock(new Mock<ITranslator<OrganizationServiceAdditionalInformation, VmServiceProducerDetail>>(), unitOfWorkMock,
                vm => new OrganizationServiceAdditionalInformation { Text = vm.FreeDescription }));

            var translations = RunTranslationModelToEntityTest<VmServiceProducer, OrganizationService>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();
            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(provisionType, model, translation);

        }

        private void CheckTranslation(ProvisionTypeEnum type, VmServiceProducer source, OrganizationService target)
        {
            target.Id.Should().NotBe(Guid.Empty);
            target.RoleTypeId.Should().Be(CacheManager.TypesCache.Get<RoleType>(RoleTypeEnum.Producer.ToString()));
            target.ProvisionTypeId.Should().Be(source.ProvisionTypeId);
            switch (type)
            {
                case ProvisionTypeEnum.SelfProduced:
                    if (source.SelfProduced.SubProducerOrganizationId.HasValue)
                    {
                        target.OrganizationId.Should().Be(source.SelfProduced.SubProducerOrganizationId);
                    }
                    else
                    {
                        target.OrganizationId.Should().Be(source.SelfProduced.ProducerOrganizationId);
                    }
                    target.AdditionalInformations.Count.Should().Be(0);
                    break;

                case ProvisionTypeEnum.PurchaseServices:
                    target.OrganizationId.Should().Be(source.PurchaseServices.ProducerOrganizationId);
                    if (!source.PurchaseServices.ProducerOrganizationId.HasValue)
                    {
                        if (string.IsNullOrEmpty(source.PurchaseServices.FreeDescription))
                        {
                            target.AdditionalInformations.Count.Should().Be(0);
                        }
                        else
                        {
                            target.AdditionalInformations.Count.Should().Be(1);
                            var ai = target.AdditionalInformations.First();
                            ai.Text.Should().Be(source.PurchaseServices.FreeDescription);
                        }
                    }
                    else
                    {
                        target.AdditionalInformations.Count.Should().Be(0);
                    }
                    break;
                case ProvisionTypeEnum.Other:
                    target.OrganizationId.Should().Be(source.Other.ProducerOrganizationId);
                    target.AdditionalInformations.Count.Should().Be(0);
                    break;
            }
        }

        private VmServiceProducer CreateModel(ProvisionTypeEnum provisionType, VmServiceProducerDetail detailTemplate)
        {
            return new VmServiceProducer()
            {
//                Id = "test".GetGuid(),
//                ProvisionTypeId = Guid.NewGuid(),
                ProvisionTypeId = provisionType.ToString().GetGuid(),
                SelfProduced = CreateModelDetail(ProvisionTypeEnum.SelfProduced, detailTemplate),
                PurchaseServices = CreateModelDetail(ProvisionTypeEnum.PurchaseServices, detailTemplate),
                Other = CreateModelDetail(ProvisionTypeEnum.Other, detailTemplate)

            };
        }

        private VmServiceProducerDetail CreateModelDetail(ProvisionTypeEnum type, VmServiceProducerDetail template)
        {
            return new VmServiceProducerDetail
            {
                FreeDescription = template.FreeDescription,
                Link = template.Link,
                ProducerOrganizationId = template.ProducerOrganizationId,
                SubProducerOrganizationId = template.SubProducerOrganizationId
            };
        }

    }
}
