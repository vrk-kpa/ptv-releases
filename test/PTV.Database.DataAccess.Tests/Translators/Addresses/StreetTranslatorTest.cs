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
using System.Collections.Generic;
using System.Linq;
using Moq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Addresses;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Addresses
{
    public class StreetTranslatorTest : TranslatorTestBase
    {
        [Fact]
        public void TranslateStreet()
        {
            var translators = RegisterTranslators();
            var street = CreateStreet();
            var toTranslate = new List<ClsAddressStreet> {street};

            var translations = RunTranslationEntityToModelTest<ClsAddressStreet, VmStreet>(translators, toTranslate);
            var translation = translations.FirstOrDefault();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(street.Id, translation.Id);
            Assert.Equal(street.MunicipalityId, translation.MunicipalityId);
            Assert.Equal(street.Municipality.Code, translation.MunicipalityCode);
        }

        [Fact]
        public void OnlyValidStreetNumbersAreTranslated()
        {
            var translators = RegisterTranslators();
            var street = CreateStreet();
            var streetNumbers = CreateStreetNumbers();
            street.StreetNumbers = streetNumbers;
            var validStreetNumberId = streetNumbers.FirstOrDefault(sn => sn.IsValid)?.Id;
            var toTranslate = new List<ClsAddressStreet> {street};

            var translations = RunTranslationEntityToModelTest<ClsAddressStreet, VmStreet>(translators, toTranslate);
            var translation = translations.FirstOrDefault();
            var translatedStreetNumber = translation.StreetNumbers.FirstOrDefault();

            Assert.Single(translation.StreetNumbers);
            Assert.Equal(validStreetNumberId, translatedStreetNumber.Id);
        }

        private List<ClsAddressStreetNumber> CreateStreetNumbers()
        {
            return new List<ClsAddressStreetNumber>
            {
                new ClsAddressStreetNumber
                {
                    Id = false.ToString().GetGuid(),
                    ClsAddressStreetId = Street.GetGuid(),
                    IsValid = false
                },
                new ClsAddressStreetNumber
                {
                    Id = true.ToString().GetGuid(),
                    ClsAddressStreetId = Street.GetGuid(),
                    IsValid = true
                }
            };
        }

        private ClsAddressStreet CreateStreet()
        {
            return new ClsAddressStreet
            {
                Id = Street.GetGuid(),
                MunicipalityId = MunicipalityCode.GetGuid(),
                Municipality = new Municipality
                {
                    Code = MunicipalityCode
                }
            };
        }

        private List<object> RegisterTranslators()
        {
            var unitOfWork = unitOfWorkMockSetup.Object;

            var translators = new List<object>
            {
                new StreetTranslator(ResolveManager, TranslationPrimitives, new TranslatedItemDefinitionHelper(),
                    CacheManager),
                new StreetNumberTranslator(ResolveManager, TranslationPrimitives),
                new ClsStreetNumberCoordinateTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                    (ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<PostalCode, VmPostalCode>>(), unitOfWork),
                RegisterTranslatorMock(new Mock<ITranslator<INameReferences, IVmTranslatedItem>>(), unitOfWork,
                    vm => default(INameReferences), entity => new Mock<IVmTranslatedItem>().Object),
                RegisterTranslatorMock(new Mock<ITranslator<IName, string>>(), unitOfWork, vm => default(IName),
                    entity => new Mock<string>().Object),
                RegisterTranslatorMock(new Mock<ITranslator<INameReferences, IVmTranslationItem>>(), unitOfWork,
                    vm => default(INameReferences), entity => new Mock<IVmTranslationItem>().Object)
            };

            return translators;
        }

        private const string Street = "Street";
        private const string MunicipalityCode = "852";
    }
}
