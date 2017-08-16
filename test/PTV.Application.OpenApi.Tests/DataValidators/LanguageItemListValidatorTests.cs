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

using FluentAssertions;
using PTV.Application.OpenApi.DataValidators;
using Xunit;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class LanguageItemListValidatorTests : ValidatorTestBase
    {
        private IList<VmOpenApiLanguageItem> list;

        public LanguageItemListValidatorTests()
        {
            list = new List<VmOpenApiLanguageItem>()
            {
                new VmOpenApiLanguageItem{ Language = "language1" },
                new VmOpenApiLanguageItem{ Language = "language2" }
            };
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new LanguageItemListValidator(null, "PropertyName", new List<string> { "language" });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
        
        [Fact]
        public void RequiredLanguagesIsNull()
        {
            // Arrange
            var validator = new LanguageItemListValidator(list, "PropertyName", null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelIncludesLanguages()
        {
            // Arrange
            var requiredLanguages = new List<string> { "language1", "language2" };
            var validator = new LanguageItemListValidator(list, "PropertyName", requiredLanguages);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelNotIncludesLanguages()
        {
            // Arrange
            var requiredLanguages = new List<string> { "language1", "language2", "language3" };
            var validator = new LanguageItemListValidator(list, "PropertyName", requiredLanguages);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
