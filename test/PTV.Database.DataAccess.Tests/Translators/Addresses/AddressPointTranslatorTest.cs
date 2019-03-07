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
using Moq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Addresses;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Framework;
using Xunit;
using IName = PTV.Domain.Model.Models.Interfaces.V2.IName;

namespace PTV.Database.DataAccess.Tests.Translators.Addresses
{
    public class AddressPointTranslatorTest : TranslatorTestBase
    {
        [Fact]
        public void TranslateAddressPoint()
        {
            var translators = RegisterTranslators();
            var addressPoint = CreateAddressPoint();
            var toTranslate = new List<ClsAddressPoint> {addressPoint};

            var translations =
                RunTranslationEntityToModelTest<ClsAddressPoint, VmAddressPoint>(translators, toTranslate);
            var translation = translations.FirstOrDefault();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(addressPoint.IsValid, translation.IsValid);
            Assert.Equal(addressPoint.Id, translation.Id);
            Assert.Equal(addressPoint.StreetNumber, translation.StreetNumber);
        }

        private static ClsAddressPoint CreateAddressPoint()
        {
            return new ClsAddressPoint
            {
                IsValid = true,
                Id = Point.GetGuid(),
                MunicipalityId = Municipality.GetGuid(),
                AddressId = Address.GetGuid(),
                StreetNumber = StreetNumber,
                AddressStreetId = Street.GetGuid(),
                AddressStreetNumberId = StreetNumber.GetGuid(),
                PostalCodeId = PostalCode.GetGuid()
            };
        }

        private List<object> RegisterTranslators()
        {
            var unitOfWork = unitOfWorkMockSetup.Object;

            var translators = new List<object>
            {
                new ClsAddressPointTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<ClsAddressStreet, VmStreet>>(), unitOfWork),
                RegisterTranslatorMock(new Mock<ITranslator<ClsAddressStreetNumber, VmStreetNumber>>(), unitOfWork),
                RegisterTranslatorMock(new Mock<ITranslator<PostalCode, VmPostalCode>>(), unitOfWork),
                RegisterTranslatorMock(new Mock<ITranslator<Municipality, VmMunicipality>>(), unitOfWork),
                RegisterTranslatorMock(new Mock<ITranslator<INameReferences, IVmTranslatedItem>>(), unitOfWork,
                    vm => default(INameReferences), entity => new Mock<IVmTranslatedItem>().Object),
                RegisterTranslatorMock(new Mock<ITranslator<IName, string>>(), unitOfWork, vm => default(IName),
                    entity => new Mock<string>().Object),
                RegisterTranslatorMock(new Mock<ITranslator<INameReferences, IVmTranslationItem>>(), unitOfWork,
                    vm => default(INameReferences), entity => new Mock<IVmTranslationItem>().Object)
            };
            return translators;
        }

        private const string Address = "Address";
        private const string Point = "Point";
        private const string Street = "Street";
        private const string Municipality = "Municipality";
        private const string PostalCode = "00001";
        private const string StreetNumber = "7g";
    }
}
