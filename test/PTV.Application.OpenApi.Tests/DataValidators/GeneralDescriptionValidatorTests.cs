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
using System;
using PTV.Domain.Model.Enums;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class GeneralDescriptionValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new GeneralDescriptionValidator(null, codeService, fintoService, new List<string> { "language" }, true);

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void DescriptionOrBackGroundDescriptionNotSet()
        {
            // Arrange
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Descriptions = new List<VmOpenApiLocalizedListItem>()
            };
            var validator = new GeneralDescriptionValidator(vm, codeService, fintoService, new List<string> { "language" }, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Descriptions").Should().BeTrue();
        }

        [Fact]
        public void ShortDescriptionNotSet()
        {
            // Arrange
            var language = "language";
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Descriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = DescriptionTypeEnum.Description.ToString(), Value = "TestValue"
                    },
                }
            };
            var validator = new GeneralDescriptionValidator(vm, codeService, fintoService, new List<string> { language }, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Descriptions").Should().BeTrue();
        }

        [Fact]
        public void ModelIsValid_DescriptionSet()
        {
            // Arrange
            var language = "language";
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Names = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = NameTypeEnum.Name.ToString(), Value = "TestValue"
                    }
                },
                Descriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = DescriptionTypeEnum.Description.ToString(), Value = "TestValue"
                    },
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = DescriptionTypeEnum.ShortDescription.ToString(), Value = "TestValue"
                    }
                }
            };
            var validator = new GeneralDescriptionValidator(vm, codeService, fintoService, new List<string> { language }, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelIsValid_BackgroundDescriptionSet()
        {
            // Arrange
            var language = "language";
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Names = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = NameTypeEnum.Name.ToString(), Value = "TestValue"
                    }
                },
                Descriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = DescriptionTypeEnum.BackgroundDescription.ToString(), Value = "TestValue"
                    },
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = DescriptionTypeEnum.ShortDescription.ToString(), Value = "TestValue"
                    }
                }
            };
            var validator = new GeneralDescriptionValidator(vm, codeService, fintoService, new List<string> { language }, true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
    }
}
