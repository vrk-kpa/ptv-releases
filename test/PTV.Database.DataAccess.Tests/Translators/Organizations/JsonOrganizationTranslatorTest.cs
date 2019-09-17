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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

using Xunit;
using PTV.Database.DataAccess.Translators.Organizations;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Domain.Model.Models.Import;
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Import;
using PTV.Database.DataAccess.Translators.Finto;

namespace PTV.Database.DataAccess.Tests.Translators.Organizations
{
//    public class JsonOrganizationTranslatorTest : TranslatorTestBase
//    {
//        private List<object> translators;
//        private IUnitOfWork unitOfWorkMock;
//
//        public JsonOrganizationTranslatorTest()
//        {
//            unitOfWorkMock = unitOfWorkMockSetup.Object;
//            translators = new List<object>()
//            {
//                new JsonOrganizationTranslator(ResolveManager, TranslationPrimitives),
//                new OrganizationNameTextTranslator(ResolveManager, TranslationPrimitives),               
////                new NameTypeCodeTranslator(ResolveManager, TranslationPrimitives),
//                new MunicipalityCodeTranslator(ResolveManager, TranslationPrimitives),
//                new BusinessCodeTranslator(ResolveManager, TranslationPrimitives),
//                new OrganizationTypeStringTranslator(ResolveManager, TranslationPrimitives),
//                RegisterTranslatorMock(new Mock<ITranslator<Language, string>>(), unitOfWorkMock),                
//            };
//            RegisterDbSet(CreateCodeData<NameType>(typeof(NameTypeEnum)), unitOfWorkMockSetup);
//            RegisterDbSet(CreateCodeData<OrganizationType>(typeof(OrganizationTypeEnum)), unitOfWorkMockSetup);
//            RegisterDbSet(new List<Municipality>() { new Municipality() { Code = "00123" } }, unitOfWorkMockSetup);
//        }
//
//        /// <summary>
//        /// test for JsonOrganizationTranslator vm - > entity
//        /// </summary>
//        [Fact]
//        public void TranslateJsonOrganizationToEntity()
//        {
//            var vmJson = TestHelper.CreateVmJsonOrganizationModel();
//            var toTranslate = new List<VmJsonOrganization>() { vmJson };
//
//            var translations = RunTranslationModelToEntityTest<VmJsonOrganization, OrganizationVersioned>(translators, toTranslate, unitOfWorkMock);
//            var translation = translations.First();
//
//            Assert.Equal(toTranslate.Count, translations.Count);
//            Assert.Equal(vmJson.BusinessID, translation.Business.Code.ToString());
//            Assert.Equal(vmJson.BusinessID, translation.Business.Name.ToString());
//            Assert.Equal(vmJson.Identifier, translation.Oid);
//            Assert.Equal(vmJson.Name, translation.OrganizationNames.Single().Name);
//            Assert.Equal(vmJson.OrganizationType, translation.Type.Code);
//            Assert.Equal(vmJson.MunicipalityCode, translation.Municipality.Code);
//        }
//    }
}
