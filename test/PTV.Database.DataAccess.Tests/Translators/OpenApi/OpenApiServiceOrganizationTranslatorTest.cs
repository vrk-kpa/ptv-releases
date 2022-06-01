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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.OpenApi.Services;
using PTV.Database.Model.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Database.DataAccess.Translators.OpenApi.Organizations;
using PTV.Domain.Model.Models.OpenApi.V7;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiServiceOrganizationTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public OpenApiServiceOrganizationTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;

            SetupTypesCacheMock<PhoneNumberType>();
            SetupTypesCacheMock<ServiceChargeType>();

            translators = new List<object>
            {
                new OpenApiServiceOrganizationTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new OpenApiServiceOrganizationInTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationSimpleItemTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                RegisterTranslatorMock(new Mock<ITranslator<ProvisionType, string>>(), unitOfWorkMock)
            };
        }

        [Fact]
        public void TranslateEntityToVm()
        {
            var entity = new OrganizationService();
            var toTranslate = new List<OrganizationService> { entity };
            var translations = RunTranslationEntityToModelTest<OrganizationService, V6VmOpenApiServiceOrganization>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
        }

        [Fact]
        public void TranslateVmToEntity()
        {
            var model = new V7VmOpenApiOrganizationServiceIn();
            var toTranslate = new List<V7VmOpenApiOrganizationServiceIn> { model };
            var translations = RunTranslationModelToEntityTest<V7VmOpenApiOrganizationServiceIn, OrganizationService>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
        }
    }
}
