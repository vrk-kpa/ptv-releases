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
**/

 using System;
using System.Collections.Generic;
using System.Linq;
 using FluentAssertions;
 using Moq;
 using PTV.Database.DataAccess.Interfaces.DbContext;
 using PTV.Database.DataAccess.Interfaces.Translators;
 using PTV.Database.DataAccess.Translators.Common;
 using PTV.Database.Model.Models;
 using PTV.Domain.Model.Enums;
 using PTV.Domain.Model.Models;
 using PTV.Framework;
 using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Common
{
    public class PhoneTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly IUnitOfWork unitOfWorkMock;

        private readonly Guid DefaultServiceChargeTypeId;
        private readonly Guid DefaultTypeId;
        private readonly Guid DefaultLangugeId;

        public PhoneTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;


            SetupTypesCacheMock<PhoneNumberType>(typeof(PhoneNumberTypeEnum));
            SetupTypesCacheMock<ServiceChargeType>(typeof(ServiceChargeTypeEnum));

            translators = new List<object>
            {
                new PhoneTranslator(ResolveManager, TranslationPrimitives, CacheManager.TypesCache),
                RegisterTranslatorMock(new Mock<ITranslator<DialCode, VmDialCode>>(), unitOfWorkMock),
            };

            RegisterDbSet(new List<Phone>(), unitOfWorkMockSetup);

            DefaultServiceChargeTypeId = ServiceChargeTypeEnum.Charged.ToString().GetGuid();
            DefaultTypeId = PhoneNumberTypeEnum.Phone.ToString().GetGuid();
            DefaultLangugeId = LanguageCode.fi.ToString().GetGuid();
        }

        [Fact]
        public void TranslateEntityToVmTest()
        {
            var entity = CreateEntity();
            var toTranslate = new List<Phone> { entity };

            var translations = RunTranslationEntityToModelTest<Phone, VmPhone>(translators, toTranslate);
            var translation = translations.First();
            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(entity, translation);
        }

        private void CheckTranslation(Phone source, VmPhone target)
        {
            target.Id.Should().Be(source.Id);
            if (source.PrefixNumberId.IsAssigned())
            {
                target.DialCode.Should().NotBeNull();
                target.DialCode.Id.Should().Be(source.PrefixNumberId);
            }
            else
            {
                target.DialCode.Should().BeNull();
            }
            target.Number.Should().Be(source.Number);
            target.LanguageId.Should().Be(source.LocalizationId);
            target.TypeId.Should().Be(source.TypeId);
            target.ChargeType.Should().Be(source.ChargeTypeId);
            target.AdditionalInformation.Should().Be(source.AdditionalInformation);
            target.ChargeDescription.Should().Be(source.ChargeDescription);
        }

        /// <summary>
        /// test for ServiceNameTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(null, null, null)]
        [InlineData(ServiceChargeTypeEnum.Other, PhoneNumberTypeEnum.Fax, LanguageCode.en)]
        public void TranslateVmToEntityTest(ServiceChargeTypeEnum? serviceChargeType, PhoneNumberTypeEnum? phoneNumberType, LanguageCode? languageCode)
        {
            var model = CreateVm(serviceChargeType, phoneNumberType, languageCode);
            var toTranslate = new List<VmPhone> { model };

            var translations = RunTranslationModelToEntityTest<VmPhone, Phone>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();
            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmPhone source, Phone target)
        {
            target.PrefixNumberId.Should().Be(source.DialCode.Id);
            target.Number.Should().Be(source.Number);
            target.ChargeDescription.Should().Be(source.ChargeDescription);
            target.AdditionalInformation.Should().Be(source.AdditionalInformation);

            target.ChargeTypeId.Should().Be(source.ChargeType == Guid.Empty ? DefaultServiceChargeTypeId : source.ChargeType);
            target.TypeId.Should().Be(source.TypeId == Guid.Empty ? DefaultTypeId : source.TypeId);
            target.LocalizationId.Should().Be(source.LanguageId.IsAssigned() ? source.LanguageId.Value : DefaultLangugeId);
        }

        private static VmPhone CreateVm(ServiceChargeTypeEnum? serviceChargeType, PhoneNumberTypeEnum? phoneNumberType, LanguageCode? languageCode)
        {
            return new VmPhone
            {
                ChargeDescription = "description",
                Number = "456789",
                DialCode = new VmDialCode() {Id = new Guid()},
                AdditionalInformation = "info",
                ChargeType = serviceChargeType?.ToString().GetGuid() ?? Guid.Empty,
                TypeId = phoneNumberType?.ToString().GetGuid() ?? Guid.Empty,
                LanguageId= languageCode?.ToString().GetGuid() ?? Guid.Empty
            };
        }

        private static Phone CreateEntity()
        {
            return new Phone
            {
                Id = new Guid(),
                PrefixNumberId = new Guid(),
                Number = "number",
                LocalizationId = LanguageCode.en.ToString().GetGuid(),
                TypeId = PhoneNumberTypeEnum.Sms.ToString().GetGuid(),
                ChargeTypeId = ServiceChargeTypeEnum.Free.ToString().GetGuid(),
                AdditionalInformation = "additional information",
                ChargeDescription = "charge description",
            };
        }

    }
}
