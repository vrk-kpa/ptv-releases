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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

using Xunit;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.DataAccess.Translators.Channels.V2.ServiceLocation;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class LocationChannelMainTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly TestConversion conversion;
        private readonly ItemListModelGenerator itemListGenerator;

        public LocationChannelMainTranslatorTest()
        {
            ServiceChannelVersioned duplicatedEntity = null;
            
            translators = new List<object>()
            {
                new ServiceLocationChannelMainTranslator(ResolveManager, TranslationPrimitives, new ServiceChannelTranslationDefinitionHelper(CacheManager)),

                RegisterTranslatorMock<ServiceChannelVersioned, VmServiceChannel>
                (
                    model =>
                    {
                        var x = duplicatedEntity ?? new ServiceChannelVersioned();
                        x.OrganizationId = model.OrganizationId;
                        return x;
                    },
                    setTargetEntityAction: (sc, vm) => duplicatedEntity = sc
                ),
                RegisterTranslatorMock<ServiceChannelVersioned, VmOpeningHours>(model =>
                {      
                    var result = duplicatedEntity ?? new ServiceChannelVersioned();
                    result.ServiceChannelServiceHours = new List<ServiceChannelServiceHours> { new ServiceChannelServiceHours() };
                    return result;
                }, setTargetEntityAction: (e, vm) => duplicatedEntity = e),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, IPhoneNumberAndFax>>(), unitOfWorkMock,
//                    phones =>
//                    {
//                        var x = TranslatedInstanceStorage.ProcessInstance(phones, duplicatedEntity);
//                        duplicatedEntity.Phones.Add(new ServiceChannelPhone { CreatedBy = phones.PhoneNumber.Number });
//                        return duplicatedEntity;
//                    },
//                    setTargetAction: sc => duplicatedEntity = sc
//                    ),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, IEmail>>(), unitOfWorkMock,
//                    email =>
//                    {
//                        var x = TranslatedInstanceStorage.ProcessInstance(email, duplicatedEntity);
//                        duplicatedEntity.Emails.Add(new ServiceChannelEmail { Email = new Email { Value = email.Email.Email} });
//                        return duplicatedEntity;
//                    },
//                    setTargetAction: sc => duplicatedEntity = sc
//                    ),
                RegisterTranslatorMock<ServiceChannelWebPage, VmWebPage>(),
                RegisterTranslatorMock<ServiceChannelLanguage, VmListItem>(),
                RegisterTranslatorMock<ServiceChannelAddress, VmAddressSimple>(model => new ServiceChannelAddress
                {
                    CharacterId = model.AddressCharacter.ToString().GetGuid()
                }),
//                RegisterTranslatorMock<ServiceChannelEmail, VmEmailData>(),
//                RegisterTranslatorMock<ServiceChannelPhone, VmPhone>()
            };
            SetupTypesCacheMock<ServiceChannelType>(typeof(ServiceChannelTypeEnum));

            RegisterDbSet(new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
            RegisterEntityForVersionManager<ServiceChannelVersioned, ServiceChannel>();

            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
            itemListGenerator = new ItemListModelGenerator();
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("fi")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("fi;sv;en")]
        public void TranslateToEntity(string list)
        {
            var model = new VmServiceLocationChannel
            {
                OrganizationId = "Partial".GetGuid(),
                Languages = itemListGenerator.CreateList(list, x => x.GetGuid())?.ToList(),
                WebPages = itemListGenerator.CreateList(list, key => itemListGenerator.Create(list.Length, i => new VmWebPage { UrlAddress = key }))?.ToDictionary(x => x.First().UrlAddress),
                VisitingAddresses = itemListGenerator.CreateList(list, key => new VmAddressSimple { StreetNumber = key })?.ToList(),
                PostalAddresses = itemListGenerator.CreateList(list, key => new VmAddressSimple { StreetNumber = key })?.ToList(),
            };

            var translation = RunTranslationModelToEntityTest<VmServiceLocationChannel, ServiceChannelVersioned>(translators, model, unitOfWorkMockSetup.Object);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmServiceLocationChannel source, ServiceChannelVersioned target)
        {
            target.OrganizationId.Should().Be(source.OrganizationId);
            
            target.WebPages.Count.Should().Be(source.WebPages?.Sum(x => x.Value.Count) ?? 0, "WebPages");
            target.Languages.Count.Should().Be(source.Languages?.Count ?? 0, "Languages");
            target.ServiceChannelServiceHours.Count.Should().Be(1);
            target.Id.Should().NotBe(Guid.Empty);
            // checks that special translator was called
            target.Addresses.Count.Should().Be((source.VisitingAddresses?.Count + source.PostalAddresses?.Count) ?? 0);
            target.Addresses.Count(x => x.CharacterId == AddressCharacterEnum.Visiting.ToString().GetGuid()).Should().Be(source.VisitingAddresses?.Count ?? 0);
            target.Addresses.Count(x => x.CharacterId == AddressCharacterEnum.Postal.ToString().GetGuid()).Should().Be(source.PostalAddresses?.Count ?? 0);
            target.TypeId.Should().Be(ServiceChannelTypeEnum.ServiceLocation.ToString().GetGuid());
        }
    }
}
