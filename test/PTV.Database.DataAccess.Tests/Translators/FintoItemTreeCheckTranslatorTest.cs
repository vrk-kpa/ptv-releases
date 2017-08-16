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
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Finto;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using Xunit;
using PTV.Database.Model.Models.Base;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public class FintoItemTreeCheckTranslatorTest : TranslatorTestBase
    {
        private TargetGroupCheckTranslator targetGroupCheckTranslator;

        public FintoItemTreeCheckTranslatorTest()
        {
            targetGroupCheckTranslator = new TargetGroupCheckTranslator(ResolveManager, TranslationPrimitives);
        }

        /// <summary>
        /// test for ServiceClassTreeTranslator entity - > vm
        /// </summary>
        //[Fact]
        //public void TranslateServiceClassesToVmTreeItem()
        //{
        //    var serviceClassName = CreateFintoClass<ServiceClass>();
        //    var toTranslate = new List<ServiceClass>() { serviceClassName };
        //    var translations = RunTranslationEntityToModelTest<ServiceClass, VmTreeItem>(new List<ITranslator> { serviceClassTreeTranslator }, toTranslate);
        //    var translation = translations.First();

        //    Assert.Equal(toTranslate.Count, translations.Count);
        //    Assert.Equal(serviceClassName.Label, translation.Module);
        //    Assert.Equal(serviceClassName.Id, translation.Id);
        //}

        /// <summary>
        /// test for TargetGroupCheckTranslator entity - > vm
        /// </summary>
        [Fact]
        public void TranslateTargetGroupToVmCheckItem()
        {
            var targetGroupItem = CreateFintoClass<TargetGroup>();
            var toTranslate = new List<TargetGroup>() { targetGroupItem };
            var translations = RunTranslationEntityToModelTest<TargetGroup, VmSelectableItem>(new List<ITranslator> { targetGroupCheckTranslator }, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(targetGroupItem.Label, translation.Name);
            Assert.Equal(targetGroupItem.Id, translation.Id);
        }

        private T CreateFintoClass<T>() where T : FintoItemBase, new()
        {
            return new T()
            {
                Label = "testFintoClass",
                Id = Guid.NewGuid()
            };
        }

    }
}
