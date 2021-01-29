/**
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
using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using FluentAssertions;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class ServiceChannelCommonTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private readonly ItemListModelGenerator itemListGenerator;
        private VmAreaInformation areaModel = null;

        public ServiceChannelCommonTranslatorTest()
        {
            var entity = new ServiceServiceChannel();
            SetupTypesCacheMock<DescriptionType>();
            SetupTypesCacheMock<NameType>();
            VmConnectionLightBasics vmclb = null;
            translators = new List<object>
            {
                new ServiceChannelCommonTranslator(ResolveManager, TranslationPrimitives, CacheManager, new CommonTranslatorHelper(CacheManager), new EntityDefinitionHelper(CacheManager)),
                RegisterTranslatorMock<ServiceChannelName, VmName>(),
                RegisterTranslatorMock<ServiceChannelVersioned, VmChannelHeader>(),
                RegisterTranslatorMock<IDescription, string>(text => new ServiceChannelDescription { Description = text}),
                RegisterTranslatorMock<IName, string>(text => new ServiceChannelName { Name = text}),
                RegisterTranslatorMock<ServiceChannelDisplayNameType, VmDispalyNameType>(),
                RegisterTranslatorMock<ServiceChannelDescription, VmDescription>(),
                RegisterTranslatorMock<ServiceChannelPhone, VmPhone>(),
                RegisterTranslatorMock<ServiceChannelEmail, VmEmailData>(),
                RegisterTranslatorMock<ServiceServiceChannel, VmConnectionLightBasics>((m) => new ServiceServiceChannel(),
                    (e) =>
                    {
                        var result = vmclb ?? new VmConnectionLightBasics();
                        result.ChannelId = e.ServiceChannelId;
                        result.ServiceId = e.ServiceId;
                       return result;
                    }, setTargetViewAction: (vm, v) => vmclb = vm),
                RegisterTranslatorMock<ServiceChannelVersioned, VmAreaInformation>(model =>
                {
                    areaModel = model;
                    return new ServiceChannelVersioned();
                })
            };

            itemListGenerator = new ItemListModelGenerator();
        }

        /// <summary>
        /// test for PrintableFormChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("fi;sv;en", "organizationId", "connectionTypeId", "name", "name2", "desc1", "desc2", 8, 6, true)]
        [InlineData("fi;sv;en", "organizationId", null, "name", "name2", "desc1", "desc2", 8, 6, true)]
        [InlineData("", "organizationId", null, "", "", "", "", 0, 0, false)]
        public void TranslateToEntity(string list, string organizationId, string connectionTypeId, string name, string name2, string desc1, string desc2, int selectedEmails, int selectedPhoneNumbers, bool isAreaFilled)
        {
            RegisterDbSet(new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
            var model = new VmServiceChannel
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId.GetGuid(),
                Name = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + name),
                AlternateName = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + name2),
                Description = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + desc1),
                ShortDescription = itemListGenerator.CreateList(list, l => l)?.ToDictionary(x => x, x => x + desc2),
                IsAlternateNameUsedAsDisplayName = itemListGenerator.CreateList(list, x => x)?.ToList(),
                Emails = itemListGenerator
                    .CreateList(list, l => itemListGenerator.Create(selectedEmails, i => new VmEmailData {Email = l}))
                    ?.ToDictionary(x => x.First().Email),
                PhoneNumbers = itemListGenerator
                    .CreateList(list, l => itemListGenerator.Create(selectedPhoneNumbers, i => new VmPhone {Number = l}))
                    ?.ToDictionary(x => x.First().Number),
                AreaInformation = isAreaFilled ? new VmAreaInformation() : null,
                ConnectionTypeId = connectionTypeId.GetGuid()
            };
            var translation = RunTranslationModelToEntityTest<VmServiceChannel, ServiceChannelVersioned>(translators, model, unitOfWorkMockSetup.Object);
            CheckTranslation(model, translation);
        }

        /// <summary>
        /// test for PrintableFormChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("fi;sv;en")]
        [InlineData("")]
        public void TranslateToViewModel(string localizationList)
        {
            var localization = itemListGenerator.CreateList(localizationList, l => l.GetGuid());
            var descriptions = new List<ServiceChannelDescription>();
            var alternateNames = new List<ServiceChannelName>();
            var displayNameTypes = new List<ServiceChannelDisplayNameType>();
            var descriptionTypeId = CacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString());
            var shortDescriptionTypeId = CacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString());
            var alternateNameTypeId = CacheManager.TypesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString());
            var serviceChannelEmails = new List<ServiceChannelEmail>();
            var serviceChannelPhones = new List<ServiceChannelPhone>();
            foreach (var l in localization)
            {
                descriptions.Add(new ServiceChannelDescription
                {
                    LocalizationId = l,
                    TypeId = descriptionTypeId
                });

                descriptions.Add(new ServiceChannelDescription
                {
                    LocalizationId = l,
                    TypeId = shortDescriptionTypeId
                });


                displayNameTypes.Add(new ServiceChannelDisplayNameType
                {
                    LocalizationId = l,
                    DisplayNameTypeId = alternateNameTypeId
                });

                serviceChannelEmails.Add(new ServiceChannelEmail
                {
                    Email = new Email { LocalizationId = l}
                });

                serviceChannelPhones.Add(new ServiceChannelPhone
                {
                    Phone = new Phone { LocalizationId = l}
                });

                alternateNames.Add(new ServiceChannelName
                {
                    LocalizationId = l,
                    TypeId = alternateNameTypeId,
                    Name = "TestName"
                });
            }

            var entity = new ServiceChannelVersioned
            {
                Id = Guid.NewGuid(),
                UnificRootId = Guid.NewGuid(),
                ConnectionTypeId = Guid.NewGuid(),
                OrganizationId = Guid.NewGuid(),
                ServiceChannelDescriptions = descriptions,
                DisplayNameTypes = displayNameTypes,
                ServiceChannelNames = alternateNames,
                UnificRoot = new ServiceChannel
                {
                    ServiceServiceChannels = new List<ServiceServiceChannel>
                    {
                        new ServiceServiceChannel
                        {
                            ServiceId = Guid.NewGuid(),
                            ServiceChannelId = Guid.NewGuid(),
                        }
                    }
                }
            };
            var translation = RunTranslationEntityToModelTest<ServiceChannelVersioned, VmServiceChannel>(translators, entity);
            CheckTranslation(entity, translation);
        }

        private void CheckTranslation(VmServiceChannel source, ServiceChannelVersioned translation)
        {
            translation.OrganizationId.Should().Be(source.OrganizationId);
            translation.Languages.Count.Should().Be(source.Languages?.Count ?? 0, "Languages");
            translation.ServiceChannelNames.Should().HaveCount((source.Name?.Count + source.AlternateName?.Count) ?? 0, "ServiceChannelNames");
            translation.ServiceChannelDescriptions.Should().HaveCount(
                (source.Description?.Count +
                 source.ShortDescription?.Count
                ) ?? 0, "ServiceChannelDescriptions");
            translation.Emails.Should().HaveCount(source.Emails?.Sum(x => x.Value.Count) ?? 0, "Emails");
            translation.Phones.Should().HaveCount(source.PhoneNumbers?.Sum(x => x.Value.Count) ?? 0, "Phone");
            translation.ConnectionTypeId.Should().Be(source.ConnectionTypeId ?? CacheManager.TypesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString()));
            translation.DisplayNameTypes.Should().HaveCount(source.IsAlternateNameUsedAsDisplayName.Count);
            areaModel.Should().NotBeNull();
            areaModel.OwnerReferenceId.Should().Be(source.AreaInformation != null ? source.Id : null);
        }

        private void CheckTranslation(ServiceChannelVersioned source, VmServiceChannel translation)
        {
            translation.Id.Should().Be(source.Id);
            translation.OrganizationId.Should().Be(source.OrganizationId);
            translation.ConnectionTypeId.Should().Be(source.ConnectionTypeId);
            translation.UnificRootId.Should().Be(source.UnificRootId);
            translation.ShortDescription.Should().HaveCount(source.ServiceChannelDescriptions.Count(x => x.TypeId == CacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString())));
            translation.Description.Should().HaveCount(source.ServiceChannelDescriptions.Count(x => x.TypeId == CacheManager.TypesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString())));
            translation.IsAlternateNameUsedAsDisplayName.Should().HaveCount(source.DisplayNameTypes.Count(x => x.DisplayNameTypeId == CacheManager.TypesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString())));
            translation.Emails.Should().HaveCount(source.Emails.Count);
            translation.PhoneNumbers.Should().HaveCount(source.Phones.Count);
            translation.AlternateName.Should().HaveCount(source.ServiceChannelNames.Count(x =>
                x.TypeId == CacheManager.TypesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString())));
            translation.ConnectedServicesUnific.Should().HaveCount(1);
            var sc = source.UnificRoot.ServiceServiceChannels.First();
            translation.ConnectedServicesUnific.First().ChannelId.Should().Be(sc.ServiceChannelId);
            translation.ConnectedServicesUnific.First().ServiceId.Should().Be(sc.ServiceId);
        }
    }
}
