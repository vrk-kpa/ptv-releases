﻿/**
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
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;


namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class ServiceTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly IUnitOfWork unitOfWorkMock;
        private readonly TreeModelGenerator treeGenerator;
        private readonly ItemListModelGenerator itemListGenerator;
        private readonly TestConversion conversion;

        public ServiceTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;

            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<PublishingStatusType>(typeof(PublishingStatus));
            SetupTypesCacheMock<ProvisionType>(typeof(ProvisionTypeEnum));

            translators = new List<object>
            {
//                new ServiceTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),
                new ServiceNameTranslator(ResolveManager, TranslationPrimitives),
                new ServiceDescriptionTranslator(ResolveManager, TranslationPrimitives),
                new ServiceRequirementTranslator(ResolveManager, TranslationPrimitives),
                new ServiceProducerTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new ServiceProducerOrganizationTranslator(ResolveManager, TranslationPrimitives),
                new ServiceProducerAdditionalInformationTranslator(ResolveManager, TranslationPrimitives, CacheManager),

                RegisterTranslatorMock(new Mock<ITranslator<Language, string>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<NameType, string>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<DescriptionType, string>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceTargetGroup, VmTargetGroupListItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceOntologyTerm, VmTreeItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceServiceClass, VmTreeItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceLifeEvent, VmTreeItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceKeyword, VmKeywordItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationService, VmTreeItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationService, VmServiceProducer>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceElectronicNotificationChannel, string>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceElectronicCommunicationChannel, string>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceServiceChannel, VmChannelListItem>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, VmElectronicChannelStep1>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, IVmChannelDescription>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceLanguage, VmListItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceKeyword, VmTreeItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceTargetGroup, VmListItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceOntologyTerm, VmListItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceServiceClass, VmListItem>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceLifeEvent, VmListItem>>(), unitOfWorkMock),
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
        [InlineData("name", "name2", "desc1", "desc2", "desc3", "sr", "C83C80FD-60CF-454B-938A-C2EC85AA5657", 0, null, null)]
        [InlineData("name", null, "desc1", "desc2", null, "sr", null, 0, "C83C80FD-60CF-454B-938A-C2EC85AA5657", CoverageTypeEnum.Nationwide)]
        [InlineData(null, null, null, null, null, null, null, 5, "C83C80FD-60CF-454B-938A-C2EC85AA5657", CoverageTypeEnum.Nationwide)]
        [InlineData(null, "name2", "desc1", "desc2", "desc3", null, null, 5, "C83C80FD-60CF-454B-938A-C2EC85AA5657", CoverageTypeEnum.Local)]
        public void TranslateServiceStep1ToEntityTest(string name, string name2, string desc1, string desc2, string desc3, string usage, string language, int municipalities, string serviceCoverageId, CoverageTypeEnum serviceCoverageType)
        {
            Assert.False(true, "redesign needed");
//            var model = CreateModel();
//            model.Step1Form = new VmServiceStep1
//            {
//                ServiceName = name,
//                AlternateServiceName = name2,
//                Description = desc1,
//                ShortDescriptions = desc2,
//                UserInstruction = desc3,
//                AdditionalInformation = "AdditionalInformation",
//                AdditionalInformationDeadLine = "AdditionalInformationDeadLine",
//                AdditionalInformationProcessingTime = "AdditionalInformationProcessingTime",
//                //AdditionalInformationTasks = "AdditionalInformationProcessingTime",
//                AdditionalInformationValidityTime = "AdditionalInformationValidityTime",
//                ServiceUsage = usage,
//                Languages = string.IsNullOrEmpty(language) ? new List<Guid>() : new List<Guid> { Guid.Parse(language) },
//                FundingTypeId = ServiceFundingTypeEnum.MarketFunded.ToString().GetGuid()
//            };

//            var toTranslate = new List<VmService>() { model };
//            Guid? serviceCoverageTypeId = conversion.GetGuid(serviceCoverageId);
////            if (serviceCoverageTypeId != null) model.Step1Form.ServiceCoverageTypeId = serviceCoverageTypeId.Value;
////            model.Step1Form.Municipalities = itemListGenerator.Create<Guid>(municipalities);
//            var translations = RunTranslationModelToEntityTest<VmService, ServiceVersioned>(translators, toTranslate, unitOfWorkMock);
//            var translation = translations.First();
//            Assert.Equal(toTranslate.Count, translations.Count);
////            translation.PublishingStatus.Code.Should().Be(PublishingStatus.Draft.ToString());
////            translation.PublishingStatusId.Should().Be(model.PublishingStatus);
////            translation.PublishingStatusId.Should().Be(PublishingStatus.Draft.ToString().GetGuid());
//            CheckTranslation(model, model.Step1Form, translation);
        }

//        private void CheckTranslation(VmService source, VmServiceStep1 step1, ServiceVersioned target)
//        {
//            //target.Id.Should().NotBe(Guid.Empty);
//            var names = conversion.GetValidTextsSkipEmptyKeepMandatory(step1.ServiceName, step1.AlternateServiceName);
//            target.ServiceNames.Count.Should().Be(names.Count, "ServiceNames");
//            target.ServiceNames.All(n => names.Contains(n.Name)).Should().BeTrue("Not all names are translated correctly");
//            var descriptions = conversion.GetValidTexts(step1.ShortDescriptions, step1.Description, step1.UserInstruction, step1.AdditionalInformation, step1.AdditionalInformationDeadLine, step1.AdditionalInformationProcessingTime, step1.AdditionalInformationValidityTime);
//            target.ServiceDescriptions.Count.Should().Be(descriptions.Count, "ServiceDescriptions");
//            target.ServiceDescriptions.All(n => descriptions.Contains(n.Description)).Should().BeTrue("Not all description are translated correctly");
//            target.ServiceRequirements.Count.Should().Be(1, "ServiceRequirements");
//            target.ServiceLanguages.Count.Should().Be(step1.Languages.Count, "ServiceLanguages");
//   //         target.ServiceCoverageTypeId.Should().Be(step1.ServiceCoverageTypeId);
//            target.FundingTypeId.Should().Be(step1.FundingTypeId);
//        }

        [Theory]
        [InlineData(0,0,0,0,0)]
        [InlineData(1,1,1,1,1)]
        [InlineData(8,15,5,10,13)]
        [InlineData(0,5,0,4,0)]
        public void TranslateServiceStep2ToEntityTest(int lifeEventsTree, int ontologyTermsTree, int serviceClassesTree, int targetGroups, int keewords)
        {
            Assert.False(true, "redesign needed");
            //            var model = CreateModel();
            //            model.Step2Form = new VmServiceStep2
            //            {
            //                TargetGroups = itemListGenerator.Create<Guid>(targetGroups),
            //                LifeEvents = itemListGenerator.Create<VmListItem>(lifeEventsTree),
            //                OntologyTerms = itemListGenerator.Create<VmListItem>(ontologyTermsTree),
            //                ServiceClasses = itemListGenerator.Create<VmListItem>(serviceClassesTree),
            //                KeyWords = itemListGenerator.Create<Guid>(keewords)
            //            };
            //
            //            var toTranslate = new List<VmService>() { model };
            //            var translations = RunTranslationModelToEntityTest<VmService, ServiceVersioned>(translators, toTranslate, unitOfWorkMock);
            //            var translation = translations.First();
            //            Assert.Equal(toTranslate.Count, translations.Count);
            //            //            translation.PublishingStatus.Code.Should().Be(PublishingStatus.Draft.ToString());
            //            //translation.PublishingStatusId.Should().Be(model.PublishingStatus);
            //            //translation.PublishingStatusId.Should().Be(PublishingStatus.Draft.ToString().GetGuid());
            //            CheckTranslation(model, model.Step2Form, translation);
        }

//        private void CheckTranslation(VmService source, VmServiceStep2 step, ServiceVersioned target)
//        {
//            //target.Id.Should().NotBe(Guid.Empty);
//            target.ServiceTargetGroups.Count.Should().Be(step.TargetGroups.Count);
//            target.ServiceServiceClasses.Count.Should().Be(step.ServiceClasses.Count);
//            target.ServiceOntologyTerms.Count.Should().Be(step.OntologyTerms.Count);
//            target.ServiceLifeEvents.Count.Should().Be(step.LifeEvents.Count);
//            target.ServiceKeywords.Count.Should().Be(step.KeyWords.Count);
//        }

        [Theory]
        [InlineData("C83C80FD-60CF-454B-938A-C2EC85AA5657", "text")]
        [InlineData("C83C80FD-60CF-454B-938A-C2EC85AA5657", null)]
        [InlineData(null, "text")]
        [InlineData(null, null)]
        public void TranslateServiceStep3ToEntityTest(string organizationId, string additionalInformation)
        {
            Assert.False(true, "redesign needed");
//            var model = CreateModel();
//            model.Step3Form = new VmServiceStep3
//            {
//                ServiceProducers = new List<VmServiceProducer>
//                {
//                    new VmServiceProducer
//                    {
//                        ProvisionType = ProvisionTypeEnum.SelfProduced.ToString().GetGuid(),
//                        SelfProducers = string.IsNullOrEmpty(organizationId)
//                            ? new List<Guid>()
//                            : new List<Guid>{Guid.Parse(organizationId)}
//                    },
//                    new VmServiceProducer
//                    {
//                        ProvisionType = ProvisionTypeEnum.PurchaseServices.ToString().GetGuid(),
//                        Organization = string.IsNullOrEmpty(organizationId)
//                            ? (Guid?) null
//                            : Guid.Parse(organizationId),
//                        AdditionalInformation = new Dictionary<string, string>() {{"fi", additionalInformation}}
//                    },
//                    new VmServiceProducer
//                    {
//                        ProvisionType = ProvisionTypeEnum.Other.ToString().GetGuid(),
//                        Organization = string.IsNullOrEmpty(organizationId)
//                            ? (Guid?) null
//                            : Guid.Parse(organizationId),
//                        AdditionalInformation = (string.IsNullOrEmpty(organizationId))
//                            ? new Dictionary<string, string>() {{"fi", additionalInformation}}
//                            : null
//                    }
//                }
//            };
//
//            RegisterDbSet(new List<ServiceProducer>(), unitOfWorkMockSetup);
//            RegisterDbSet(new List<ServiceProducerAdditionalInformation>(), unitOfWorkMockSetup);
//            RegisterDbSet(new List<ServiceProducerOrganization>(), unitOfWorkMockSetup);
//
//            var toTranslate = new List<VmService>() { model };
//            var translations = RunTranslationModelToEntityTest<VmService, ServiceVersioned>(translators, toTranslate, unitOfWorkMock);
//            var translation = translations.First();
//            Assert.Equal(toTranslate.Count, translations.Count);
//            CheckTranslation(model, model.Step3Form, translation);
        }

//        private static void CheckTranslation(VmService source, VmServiceStep3 step, ServiceVersioned target)
//        {
//            target.ServiceProducers.Count.Should().Be(step.ServiceProducers.Count);
//
//            // selfProduced
//            var sourceSelfProduced = source.Step3Form.ServiceProducers.First(p => p.ProvisionType == ProvisionTypeEnum.SelfProduced.ToString().GetGuid());
//            var targetSelfProduced = target.ServiceProducers.First(p => p.ProvisionTypeId == ProvisionTypeEnum.SelfProduced.ToString().GetGuid());
//            targetSelfProduced.Organizations.Count.Should().Be(sourceSelfProduced.SelfProducers.Count);
//            if(sourceSelfProduced.SelfProducers.Count > 0) targetSelfProduced.Organizations.First().OrganizationId.Should().Be(sourceSelfProduced.SelfProducers.First());
//            targetSelfProduced.AdditionalInformations.Should().BeEmpty();
//
//            // purchased
//            var sourcePurchased = source.Step3Form.ServiceProducers.First(p => p.ProvisionType == ProvisionTypeEnum.PurchaseServices.ToString().GetGuid());
//            var targetPurchased = target.ServiceProducers.First(p => p.ProvisionTypeId == ProvisionTypeEnum.PurchaseServices.ToString().GetGuid());
//            if (sourcePurchased.Organization.HasValue) targetPurchased.Organizations.First().OrganizationId.Should().Be(sourcePurchased.Organization.Value);
//            if (!string.IsNullOrEmpty(sourcePurchased.AdditionalInformation["fi"])) targetPurchased.AdditionalInformations.First().Text.Should().Be(sourcePurchased.AdditionalInformation["fi"]);
//
//            // other
//            var sourceOther = source.Step3Form.ServiceProducers.First(p => p.ProvisionType == ProvisionTypeEnum.Other.ToString().GetGuid());
//            var targetOther = target.ServiceProducers.First(p => p.ProvisionTypeId == ProvisionTypeEnum.Other.ToString().GetGuid());
//            if (sourceOther.Organization.HasValue) targetOther.Organizations.First().OrganizationId.Should().Be(sourceOther.Organization.Value);
//            if (!string.IsNullOrEmpty(sourceOther.AdditionalInformation["fi"])) targetOther.AdditionalInformations.First().Text.Should().Be(sourceOther.AdditionalInformation["fi"]);
//        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(8)]
        [InlineData(12)]
        public void TranslateServiceStep4ToEntityTest(int attachedServiceChannels)
        {
            Assert.False(true, "redesign needed");
//            var model = CreateModel();
//            model.Step4Form = itemListGenerator.Create<Guid>(attachedServiceChannels);
//
//            var toTranslate = new List<VmService>() { model };
//            var translations = RunTranslationModelToEntityTest<VmService, ServiceVersioned>(translators, toTranslate, unitOfWorkMock);
//            var translation = translations.First();
//            Assert.Equal(toTranslate.Count, translations.Count);
//            //            translation.PublishingStatus.Code.Should().Be(PublishingStatus.Draft.ToString());
//           // translation.PublishingStatusId.Should().Be(model.PublishingStatus);
//            //translation.PublishingStatusId.Should().Be(PublishingStatus.Draft.ToString().GetGuid());
//           // CheckTranslation(model, model.Step4Form, translation);
        }

//        private void CheckTranslation(VmService source, List<Guid> step, ServiceVersioned target)
//        {
//            // target.Id.Should().NotBe(Guid.Empty);
//            // target.ServiceServiceChannels.Count.Should().Be(step.Count);
//        }

//        private VmService CreateModel()
//        {
//            return new VmService()
//            {
//                PublishingStatusId = PublishingStatus.Draft.ToString().GetGuid(),
//                Step1Form = new VmServiceStep1(),
//                Step2Form = new VmServiceStep2(),
//                Step3Form = new VmServiceStep3(),
//                Step4Form = new List<Guid>()
//            };
//        }


    }


}
