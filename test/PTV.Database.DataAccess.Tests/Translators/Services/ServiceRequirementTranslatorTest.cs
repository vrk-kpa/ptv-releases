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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework.ServiceManager;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class ServiceRequirementTranslatorTest : TranslatorTestBase
    {
        private ServiceRequirementTranslator serviceRequirementTranslator;
        private LanguageCodeTranslator languageCodeTranslator;
        private IUnitOfWork unitOfWorkMock;

        public ServiceRequirementTranslatorTest()
        {
            serviceRequirementTranslator = new ServiceRequirementTranslator(ResolveManager, TranslationPrimitives);
            languageCodeTranslator = new LanguageCodeTranslator(ResolveManager, TranslationPrimitives);
            unitOfWorkMock = unitOfWorkMockSetup.Object;
        }

        /// <summary>
        /// test for ServiceRequirementTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateServiceNameToEntity()
        {
            var vmServiceRequirement = CreateVmServiceRequirement();
            var toTranslate = new List<VmServiceRequirement>() { vmServiceRequirement };

            RegisterDbSet(CreateCodeData<Language>(typeof(LanguageCode)), unitOfWorkMockSetup);

            var translations = RunTranslationModelToEntityTest<VmServiceRequirement, ServiceRequirement>(new List<ITranslator> { serviceRequirementTranslator, languageCodeTranslator }, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(vmServiceRequirement.Requirement, translation.Requirement);
        }

        private VmServiceRequirement CreateVmServiceRequirement()
        {
            return new VmServiceRequirement()
            {
                Requirement="requrement"
            };
        }

    }

}
