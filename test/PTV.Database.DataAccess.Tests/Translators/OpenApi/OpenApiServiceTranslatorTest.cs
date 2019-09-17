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

using PTV.Database.DataAccess.Translators.OpenApi.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.DataAccess.Translators.OpenApi.Common;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiServiceTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public OpenApiServiceTranslatorTest()
        {
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));

            translators = new List<object>
            {
                new OpenApiServiceTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceChargeTypeCodeTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new OpenApiOrganizationItemTranslator(ResolveManager, TranslationPrimitives, CacheManager)
            };
        }

        /// <summary>
        /// Test for OpenApiServiceTranslator entity -> vm
        /// </summary>
        [Fact]
        public void TranslateServiceToVm()
        {
            var service = new ServiceVersioned();
            var toTranslate = new List<ServiceVersioned>() { service };
            var translations = RunTranslationEntityToModelTest<ServiceVersioned, VmOpenApiServiceVersionBase>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
        }
    }
}