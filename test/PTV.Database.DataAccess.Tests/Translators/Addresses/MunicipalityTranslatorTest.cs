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
    public class MunicipalityTranslatorTest : TranslatorTestBase
    {
        [Fact]
        public void TranslateMunicipality()
        {
            var translators = RegisterTranslators();
            var municipality = CreateMunicipality();
            var toTranslate = new List<Municipality> {municipality};

            var translations = RunTranslationEntityToModelTest<Municipality, VmMunicipality>(translators, toTranslate);
            var translation = translations.FirstOrDefault();
            
            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(municipality.Id, translation.Id);
            Assert.Equal(municipality.Code, translation.MunicipalityCode);
            Assert.Equal(municipality.Description, translation.Description);
        }

        private List<object> RegisterTranslators()
        {
            var unitOfWork = unitOfWorkMockSetup.Object;

            var translators = new List<object>
            {
                new MunicipalityTranslator(ResolveManager, TranslationPrimitives, new TranslatedItemDefinitionHelper(),
                    CacheManager),
                RegisterTranslatorMock(new Mock<ITranslator<INameReferences, IVmTranslatedItem>>(), unitOfWork,
                    vm => default(INameReferences), entity => new Mock<IVmTranslatedItem>().Object),
                RegisterTranslatorMock(new Mock<ITranslator<IName, string>>(), unitOfWork, vm => default(IName),
                    entity => new Mock<string>().Object),
                RegisterTranslatorMock(new Mock<ITranslator<INameReferences, IVmTranslationItem>>(), unitOfWork,
                    vm => default(INameReferences), entity => new Mock<IVmTranslationItem>().Object)
            };
            return translators;
        }

        private static Municipality CreateMunicipality()
        {
            return new Municipality
            {
                Id = MunicipalityCode.GetGuid(),
                Code = MunicipalityCode,
                Description = MunicipalityDescription
            };
        }

        private const string MunicipalityCode = "963";
        private const string MunicipalityDescription = "Municipality Description";
    }
}
