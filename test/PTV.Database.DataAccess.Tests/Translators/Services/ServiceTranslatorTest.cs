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
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Database.DataAccess.Translators.Services.V2;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;


namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class ServiceTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly TreeModelGenerator treeGenerator;
        private readonly ItemListModelGenerator itemListGenerator;
        private readonly TestConversion conversion;

        public ServiceTranslatorTest()
        {
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<PublishingStatusType>(typeof(PublishingStatus));
            SetupTypesCacheMock<ProvisionType>(typeof(ProvisionTypeEnum));

            translators = new List<object>
            {
                new ServiceBaseTranslator(ResolveManager, TranslationPrimitives, CacheManager, new EntityDefinitionHelper(CacheManager)),
//                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceNameTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceDescriptionTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceRequirementTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceProducerTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//                new ServiceProducerOrganizationTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceProducerAdditionalInformationTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                RegisterTranslatorMock<ServiceName, VmName>(),
                RegisterTranslatorMock<ServiceDescription, VmDescription>(),
                RegisterTranslatorMock<ServiceVersioned, VmAreaInformation>(),
                RegisterTranslatorMock<ServiceRequirement, VmServiceRequirement>(),
                RegisterTranslatorMock<ServiceProducer, VmServiceProducer>(model => new ServiceProducer { ProvisionTypeId = model.ProvisionType }),
                RegisterTranslatorMock<ServiceTargetGroup, VmTargetGroupListItem>(),
                RegisterTranslatorMock<ServiceOntologyTerm, VmTreeItem>(),
                RegisterTranslatorMock<ServiceServiceClass, VmTreeItem>(),
                RegisterTranslatorMock<ServiceLifeEvent, VmTreeItem>(),
                RegisterTranslatorMock<ServiceKeyword, VmKeywordItem>(),
                RegisterTranslatorMock<OrganizationService, VmTreeItem>(),
                RegisterTranslatorMock<OrganizationService, VmServiceProducer>(),
                RegisterTranslatorMock<ServiceElectronicNotificationChannel, string>(),
                RegisterTranslatorMock<ServiceElectronicCommunicationChannel, string>(),
                RegisterTranslatorMock<ServiceServiceChannel, VmChannelListItem>(),
                RegisterTranslatorMock<ServiceLanguage, VmListItem>(),
                RegisterTranslatorMock<ServiceKeyword, VmTreeItem>(),
                RegisterTranslatorMock<ServiceTargetGroup, VmListItem>(),
                RegisterTranslatorMock<ServiceOntologyTerm, VmListItem>(),
                RegisterTranslatorMock<ServiceServiceClass, VmListItem>(),
                RegisterTranslatorMock<ServiceLifeEvent, VmListItem>(),
            };

            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceVersioned>(), unitOfWorkMockSetup);

            treeGenerator = new TreeModelGenerator();
            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
        }

        /// <summary>
        /// test for ServiceNameTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("fi;sv;en", "name", "name2", "desc1", "desc2", "desc3", "sr", "C83C80FD-60CF-454B-938A-C2EC85AA5657",  0)]
        [InlineData("fi","name", null, "desc1", "desc2", null, "sr", null,  4)]
        [InlineData(null, null, null, null, null, null, null, null,  1)]
        [InlineData("", null, "name2", "desc1", "desc2", "desc3", null, null,  10)]
        public void TranslateServiceStep1ToEntityTest(string list, string name, string name2, string desc1, string desc2, string desc3, string usage, string language, int targetGroups)
        {
            var model = new VmServiceBase
            {
                Organization = "org".GetGuid(),
                Name = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + name),
                AlternateName = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + name2),
                Description = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + desc1),
                ShortDescription = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + desc2),
                UserInstruction = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + desc3),
//                AdditionalInformation = "AdditionalInformation",
                DeadLineInformation = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + "AdditionalInformationDeadLine"),
                ProcessingTimeInformation = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + "AdditionalInformationProcessingTime"),
                //AdditionalInformationTasks = "AdditionalInformationProcessingTime",
                ValidityTimeInformation = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + "AdditionalInformationValidityTime"),
                ConditionOfServiceUsage = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + usage),
                Languages = string.IsNullOrEmpty(language) ? new List<Guid>() : new List<Guid> { Guid.Parse(language) },
                FundingType = ServiceFundingTypeEnum.MarketFunded.ToString().GetGuid(),
                AreaInformation = new VmAreaInformation(),
                ServiceProducers = new List<VmServiceProducer>(),
                TargetGroups = itemListGenerator.Create<Guid>(targetGroups),

            };

//            Guid? serviceCoverageTypeId = conversion.GetGuid(serviceCoverageId);
//            if (serviceCoverageTypeId != null) model.Step1Form.ServiceCoverageTypeId = serviceCoverageTypeId.Value;
//            model.Step1Form.Municipalities = itemListGenerator.Create<Guid>(municipalities);
            var translation = RunTranslationModelToEntityTest<VmServiceBase, ServiceVersioned>(translators, model, unitOfWorkMockSetup.Object);
//            translation.PublishingStatus.Code.Should().Be(PublishingStatus.Draft.ToString());
//            translation.PublishingStatusId.Should().Be(model.PublishingStatus);
//            translation.PublishingStatusId.Should().Be(PublishingStatus.Draft.ToString().GetGuid());
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmServiceBase source, ServiceVersioned target)
        {
            //target.Id.Should().NotBe(Guid.Empty);
            target.OrganizationId.Should().Be(source.Organization ?? Guid.Empty);
            target.ServiceNames.Should().HaveCount((source.Name?.Count + source.AlternateName?.Count) ?? 0, "ServiceNames");
            target.ServiceDescriptions.Should().HaveCount(
                (source.Description?.Count +
                 source.UserInstruction?.Count +
                 source.ShortDescription?.Count +
                 source.DeadLineInformation?.Count +
                 source.ValidityTimeInformation?.Count +
                 source.ProcessingTimeInformation?.Count
                 ) ?? 0, "ServiceDescriptions");
            target.ServiceRequirements.Should().HaveCount(source.ConditionOfServiceUsage?.Count ?? 0, "ServiceRequirements");
            target.ServiceLanguages.Should().HaveCount(source.Languages?.Count ?? 0, "ServiceLanguages");
   //         target.ServiceCoverageTypeId.Should().Be(step1.ServiceCoverageTypeId);
            target.FundingTypeId.Should().Be(source.FundingType);
            target.ServiceTargetGroups.Count.Should().Be(source.TargetGroups.Count);
        }

        [Theory]
        [InlineData("C83C80FD-60CF-454B-938A-C2EC85AA5657", "text")]
        [InlineData("C83C80FD-60CF-454B-938A-C2EC85AA5657", null)]
        [InlineData(null, "text")]
        [InlineData(null, null)]
        public void TranslateServiceStep3ToEntityTest(string organizationId, string additionalInformation)
        {
            var model = new VmServiceBase()
            {
                Organization = "org".GetGuid(),
                ResponsibleOrganizations = new List<Guid>(),
                ServiceProducers = new List<VmServiceProducer>
                {
                    new VmServiceProducer
                    {
                        ProvisionType = ProvisionTypeEnum.SelfProduced.ToString().GetGuid(),
                        SelfProducers = string.IsNullOrEmpty(organizationId)
                            ? new List<Guid>()
                            : new List<Guid>{Guid.Parse(organizationId)}
                    },
                    new VmServiceProducer
                    {
                        ProvisionType = ProvisionTypeEnum.PurchaseServices.ToString().GetGuid(),
                        Organization = string.IsNullOrEmpty(organizationId)
                            ? (Guid?) null
                            : Guid.Parse(organizationId),
                        AdditionalInformation = new Dictionary<string, string>() {{"fi", additionalInformation}}
                    },
                    new VmServiceProducer
                    {
                        ProvisionType = ProvisionTypeEnum.Other.ToString().GetGuid(),
                        Organization = string.IsNullOrEmpty(organizationId)
                            ? (Guid?) null
                            : Guid.Parse(organizationId),
                        AdditionalInformation = (string.IsNullOrEmpty(organizationId))
                            ? new Dictionary<string, string>() {{"fi", additionalInformation}}
                            : null
                    }
                },
                AreaInformation = new VmAreaInformation()
            };

            RegisterDbSet(new List<ServiceProducer>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceProducerAdditionalInformation>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceProducerOrganization>(), unitOfWorkMockSetup);

            var translation = RunTranslationModelToEntityTest<VmServiceBase, ServiceVersioned>(translators, model, unitOfWorkMockSetup.Object);
            CheckTranslation3(model, translation);
        }

        private static void CheckTranslation3(VmServiceBase source, ServiceVersioned target)
        {
            target.ServiceProducers.Count.Should().Be(source.ServiceProducers.Count);

            // selfProduced
//            var sourceSelfProduced = source.ServiceProducers.First(p => p.ProvisionType == ProvisionTypeEnum.SelfProduced.ToString().GetGuid());
//            var targetSelfProduced = target.ServiceProducers.First(p => p.ProvisionTypeId == ProvisionTypeEnum.SelfProduced.ToString().GetGuid());
            var selfProduced = ProvisionTypeEnum.SelfProduced.ToString().GetGuid();
            target.ServiceProducers.Count(x => x.ProvisionTypeId == selfProduced).Should()
                .Be(source.ServiceProducers.Count(x => x.ProvisionType == selfProduced));
//            targetSelfProduced.Organizations.Count.Should().Be(sourceSelfProduced.SelfProducers.Count);
//            if(sourceSelfProduced.SelfProducers.Count > 0) targetSelfProduced.Organizations.First().OrganizationId.Should().Be(sourceSelfProduced.SelfProducers.First());
//            targetSelfProduced.AdditionalInformations.Should().BeEmpty();

            // purchased
            var purchase = ProvisionTypeEnum.PurchaseServices.ToString().GetGuid();
            target.ServiceProducers.Count(x => x.ProvisionTypeId == purchase).Should()
                .Be(source.ServiceProducers.Count(x => x.ProvisionType == purchase));

//            var sourcePurchased = source.ServiceProducers.First(p => p.ProvisionType == ProvisionTypeEnum.PurchaseServices.ToString().GetGuid());
//            var targetPurchased = target.ServiceProducers.First(p => p.ProvisionTypeId == ProvisionTypeEnum.PurchaseServices.ToString().GetGuid());
//            if (sourcePurchased.Organization.HasValue) targetPurchased.Organizations.First().OrganizationId.Should().Be(sourcePurchased.Organization.Value);
//            if (!string.IsNullOrEmpty(sourcePurchased.AdditionalInformation["fi"])) targetPurchased.AdditionalInformations.First().Text.Should().Be(sourcePurchased.AdditionalInformation["fi"]);

            // other
            var other = ProvisionTypeEnum.Other.ToString().GetGuid();
            target.ServiceProducers.Count(x => x.ProvisionTypeId == other).Should()
                .Be(source.ServiceProducers.Count(x => x.ProvisionType == other));

//            var sourceOther = source.ServiceProducers.First(p => p.ProvisionType == ProvisionTypeEnum.Other.ToString().GetGuid());
//            var targetOther = target.ServiceProducers.First(p => p.ProvisionTypeId == ProvisionTypeEnum.Other.ToString().GetGuid());
//            if (sourceOther.Organization.HasValue) targetOther.Organizations.First().OrganizationId.Should().Be(sourceOther.Organization.Value);
//            if (!string.IsNullOrEmpty(sourceOther.AdditionalInformation["fi"])) targetOther.AdditionalInformations.First().Text.Should().Be(sourceOther.AdditionalInformation["fi"]);
        }

    }
}
