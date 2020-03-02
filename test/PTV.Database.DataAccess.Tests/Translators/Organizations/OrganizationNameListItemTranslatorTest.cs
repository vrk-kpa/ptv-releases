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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;

using Xunit;
using PTV.Database.DataAccess.Translators.Organizations;
using System;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Tests.Translators.Organizations
{
    public class OrganizationNameListItemTranslatorTest : TranslatorTestBase
    {
        private OrganizationNameListItemTranslator organizationNameListItemTranslator;

        public OrganizationNameListItemTranslatorTest()
        {
            organizationNameListItemTranslator = new OrganizationNameListItemTranslator(ResolveManager, TranslationPrimitives);
        }

        /// <summary>
        /// test for OrganizationNameTranslator entity - > vm
        /// </summary>
        [Fact]
        public void TranslateOrganizationNameToViewModel()
        {
            var organizationName = CreateOrganizationName();
            var toTranslate = new List<OrganizationName> { organizationName };

            var translations = RunTranslationEntityToModelTest<OrganizationName, VmListItem>(new List<ITranslator> { organizationNameListItemTranslator }, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(organizationName.Name, translation.Name);
            Assert.Equal(organizationName.OrganizationVersionedId, translation.Id);

        }

        private OrganizationName CreateOrganizationName()
        {
            return new OrganizationName
            {
                Name = "testOrganizationTypeName",
                OrganizationVersionedId =  Guid.NewGuid()
            };
        }
    }
}
