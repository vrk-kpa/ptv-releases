﻿/**
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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class StatutoryServiceOntologyTermTranslatorTest : TranslatorTestBase
    {
        private StatutoryServiceOntologyTermTranslator statutoryServiceOntologyTermTranslator;

        public StatutoryServiceOntologyTermTranslatorTest()
        {
            statutoryServiceOntologyTermTranslator = new StatutoryServiceOntologyTermTranslator(ResolveManager, TranslationPrimitives);
        }

        /// <summary>
        /// test for StatutoryServiceOntologyTermTranslator entity - > vm
        /// </summary>
        [Fact]
        public void TranslateStatutoryServiceOntologyTermToViewModel()
        {
            var statutoryServiceOntologyTerm = CreateStatutoryServiceOntologyTerm();
            var toTranslate = new List<StatutoryServiceOntologyTerm>() { statutoryServiceOntologyTerm };
            var translations = RunTranslationEntityToModelTest<StatutoryServiceOntologyTerm, VmTreeItem>(new List<ITranslator> { statutoryServiceOntologyTermTranslator }, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(statutoryServiceOntologyTerm.OntologyTerm.Label, translation.Name);
            Assert.Equal(statutoryServiceOntologyTerm.OntologyTermId, translation.Id);
            Assert.True(translation.IsSelected);
        }

        private StatutoryServiceOntologyTerm CreateStatutoryServiceOntologyTerm()
        {
            return new StatutoryServiceOntologyTerm()
            {
                OntologyTerm = new OntologyTerm() { Label = "OntologyTermLabel" },
                OntologyTermId = new Guid()
            };
        }

    }
}
