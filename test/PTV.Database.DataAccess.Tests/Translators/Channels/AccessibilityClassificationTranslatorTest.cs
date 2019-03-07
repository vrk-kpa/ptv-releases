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
using FluentAssertions;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class AccessibilityClassificationTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly IUnitOfWork unitOfWorkMock;

        public AccessibilityClassificationTranslatorTest()
        {
            SetupTypesCacheMock<AccessibilityClassificationLevelType>();
            SetupTypesCacheMock<WcagLevelType>();
            
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<object>()  
            {
                new AccessibilityClassificationTranslator(ResolveManager, TranslationPrimitives, CacheManager)
            };
            RegisterDbSet(new List<AccessibilityClassification>(), unitOfWorkMockSetup);
        }
        
        /// <summary>
        /// test for AccessibilityClassificationTranslatorTest entity - > vm
        /// </summary>
        [Fact]
        public void TranslateServiceChargeTypeToViewModel()
        {
            var toTranslate = new AccessibilityClassification()
            {
                Id = "Id".GetGuid(), 
                AccessibilityClassificationLevelTypeId = AccessibilityClassificationLevelTypeEnum.Unknown.ToString().GetGuid(),
                WcagLevelTypeId = WcagLevelTypeEnum.LevelA.ToString().GetGuid(),
                Name = "name",
                Url = "https://www.urlAddress.com/"
            };
            
            var translation = RunTranslationEntityToModelTest<AccessibilityClassification, VmAccessibilityClassification>(translators, toTranslate);
            CheckTranslation(toTranslate, translation); 
        }

        private void CheckTranslation(AccessibilityClassification source, VmAccessibilityClassification target)
        {
            Assert.Equal(source.Id, target.Id);
            Assert.Equal(source.AccessibilityClassificationLevelTypeId, target.AccessibilityClassificationLevelTypeId);
            Assert.Equal(source.WcagLevelTypeId, target.WcagLevelTypeId);
            Assert.Equal(source.Name, target.Name);
            Assert.Equal(source.Url, target.UrlAddress);
        }
        
        /// <summary>
        /// test for AccessibilityClassificationTranslatorTest vm - > entity
        /// </summary>
        [Theory]
        [InlineData(null, null)]
        [InlineData(AccessibilityClassificationLevelTypeEnum.Unknown, null )]
        [InlineData(AccessibilityClassificationLevelTypeEnum.PartiallyCompliant, WcagLevelTypeEnum.LevelA )]
        [InlineData(AccessibilityClassificationLevelTypeEnum.FullyCompliant, WcagLevelTypeEnum.LevelAA )]
        public void TranslateToEntity(AccessibilityClassificationLevelTypeEnum? levelType, WcagLevelTypeEnum? wcagLevelType)
        {
            var model = new VmAccessibilityClassification
            {
                AccessibilityClassificationLevelTypeId = levelType.HasValue ? levelType.ToString().GetGuid() : (Guid?)null,
                WcagLevelTypeId = wcagLevelType.HasValue ? wcagLevelType.ToString().GetGuid() : (Guid?)null,
                Name = "name",
                UrlAddress = "https://www.urlAddress.com/",
                LocalizationId = "localizationId".GetGuid(),
                OwnerReferenceId = "ownerReference".GetGuid()
            };
           
            var translation = RunTranslationModelToEntityTest<VmAccessibilityClassification, AccessibilityClassification>(translators, model, unitOfWorkMock);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmAccessibilityClassification source, AccessibilityClassification translation)
        {
            translation.Id.Should().NotBe(Guid.Empty);
            translation.AccessibilityClassificationLevelTypeId.Should().NotBe(Guid.Empty);
            translation.AccessibilityClassificationLevelTypeId.Should().NotBeEmpty();
            Assert.Equal(source.Name, translation.Name);
            Assert.Equal(source.UrlAddress, translation.Url);
        }
    }
}
