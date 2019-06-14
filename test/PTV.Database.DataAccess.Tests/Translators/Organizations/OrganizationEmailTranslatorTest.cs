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
using PTV.Database.Model.Models;

using Xunit;
using PTV.Database.DataAccess.Translators.Organizations;
using System;
using FluentAssertions;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Tests.Translators.Organizations
{
    public class OrganizationEmailTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;

        public OrganizationEmailTranslatorTest()
        {
            translators = new List<object>()
            {
                new OrganizationEmailTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock<Email, VmEmailData>(),
            };

            RegisterDbSet(new List<OrganizationEmail>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<Email>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for OrganizationEmailTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateOrganizationEmailToEntity()
        {
            var vmEmail = "test@test.com";
            var translation = RunTranslationModelToEntityTest<string, OrganizationEmail>(translators, vmEmail, unitOfWorkMockSetup.Object);
            translation.Email.Should().NotBeNull();
        }
    }
}
