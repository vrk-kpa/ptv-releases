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
using Microsoft.EntityFrameworkCore;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.ServiceManager;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class ServiceNameTranslatorTest : TranslatorTestBase
    {
        private ServiceNameTranslator serviceNameTranslator;
        private NameTypeCodeTranslator nameTypeCodeTranslator;
        private LanguageCodeTranslator languageCodeTranslator;
        private IUnitOfWork unitOfWorkMock;

        public ServiceNameTranslatorTest()
        {
            serviceNameTranslator = new ServiceNameTranslator(ResolveManager, TranslationPrimitives);
            nameTypeCodeTranslator = new NameTypeCodeTranslator(ResolveManager, TranslationPrimitives);
            languageCodeTranslator = new LanguageCodeTranslator(ResolveManager, TranslationPrimitives);
            unitOfWorkMock = unitOfWorkMockSetup.Object;
        }

        /// <summary>
        /// test for ServiceNameTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(NameTypeEnum.Name)]
        [InlineData(NameTypeEnum.AlternateName)]
        public void TranslateServiceNAmeToEntity(NameTypeEnum nameType)
        {
            var vmName = CreateVmName(nameType);
            var toTranslate = new List<VmName>() { vmName };

            RegisterDbSet(CreateCodeData<NameType>(typeof(NameTypeEnum)), unitOfWorkMockSetup);
            string[] codes = {"fi", "sv", "en"}; 
            RegisterDbSet(codes.Select(type => new Language { Code = type }).ToList(), unitOfWorkMockSetup);

            var translations = RunTranslationModelToEntityTest<VmName, ServiceName>(new List<ITranslator> { serviceNameTranslator, nameTypeCodeTranslator, languageCodeTranslator }, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(vmName.Name, translation.Name);
            Assert.Equal(vmName.TypeId, translation.TypeId);
        }

        private VmName CreateVmName(NameTypeEnum descriptionType)
        {
            return new VmName()
            {
                Name = "name",
                TypeId = descriptionType.ToString().GetGuid()
            };
        }

    }

}
