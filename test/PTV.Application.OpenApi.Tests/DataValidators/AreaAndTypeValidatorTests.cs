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

using FluentAssertions;
using Moq;
using PTV.Application.OpenApi.DataValidators;
using System;
using Xunit;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class AreaAndTypeValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new AreaAndTypeValidator(null, "", codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void InvalidTypeShouldThrow()
        {
            // Arrange
            var validator = new AreaAndTypeValidator(new List<VmOpenApiAreaIn>(), "InvalidType", codeService);

            // Act
            Action act = () => validator.Validate(controller.ModelState);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void ModelSetWithAreasAndTypeAsWholeCountry()
        {
            // Arrange
            var list = new List<VmOpenApiAreaIn>
            {
                new VmOpenApiAreaIn
                {
                    AreaCodes = new List<string> {"code1", "code2" }
                }
            };
            var validator = new AreaAndTypeValidator(list, AreaInformationTypeEnum.WholeCountry.GetOpenApiValue(), codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ModelSetWithoutAreasAndTypeAsWholeCountry()
        {
            // Arrange
            var validator = new AreaAndTypeValidator(new List<VmOpenApiAreaIn>(), AreaInformationTypeEnum.WholeCountry.GetOpenApiValue(), codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelSetWithMunicipalitiesAndTypeAsAreaType()
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetMunicipalityByCode(It.IsAny<string>(), It.IsAny<bool>())).Returns(new VmListItem { Id = Guid.NewGuid() });
            var list = new List<VmOpenApiAreaIn>
            {
                new VmOpenApiAreaIn
                {
                    Type = AreaTypeEnum.Municipality.GetOpenApiValue(),
                    AreaCodes = new List<string> {"code1", "code2" }
                }
            };
            var validator = new AreaAndTypeValidator(list, AreaInformationTypeEnum.AreaType.GetOpenApiValue(), codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            codeServiceMockSetup.Verify(m => m.GetMunicipalityByCode(It.IsAny<string>(), It.IsAny<bool>()), Times.Exactly(2));
        }

        [Fact]
        public void ModelSetWithAreasAndTypeAsAreaType()
        {
            // Arrange
            codeServiceMockSetup.Setup(c => c.GetAreaByCodeAndType(It.IsAny<string>(), It.IsAny<string>())).Returns((VmOpenApiArea)null);
            var list = new List<VmOpenApiAreaIn>
            {
                new VmOpenApiAreaIn
                {
                    Type = AreaTypeEnum.Province.GetOpenApiValue(),
                    AreaCodes = new List<string> {"code1", "code2" }
                }
            };
            var validator = new AreaAndTypeValidator(list, AreaInformationTypeEnum.AreaType.GetOpenApiValue(), codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            codeServiceMockSetup.Verify(m => m.GetAreaByCodeAndType(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
