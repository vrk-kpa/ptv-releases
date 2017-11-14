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
using PTV.Domain.Model.Enums.Security;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new ServiceValidator(null, generalDescriptionService, codeService, fintoService, commonService,
                channelService, new List<string> { "language" }, UserRoleEnum.Eeva);

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void OnlyPublishingStatusDefined()
        {
            // Arrange & act
            var validator = new ServiceValidator(new VmOpenApiServiceInVersionBase() { PublishingStatus = PublishingStatus.Published.ToString() }, 
                generalDescriptionService, codeService, fintoService, commonService,
                channelService, null, UserRoleEnum.Eeva);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("id")] // not valid Guid
        [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
        public void GeneralDescriptionSet_NotValid(string gdId)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceInVersionBase()
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                StatutoryServiceGeneralDescriptionId = gdId
            };
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, commonService,
                channelService, null, UserRoleEnum.Eeva);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void GeneralDescriptionSet_NotExists()
        {
            // Arrange & act
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase()
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                StatutoryServiceGeneralDescriptionId = gdId.ToString()
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true)).Returns((VmOpenApiGeneralDescriptionVersionBase)null);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, commonService,
                channelService, null, UserRoleEnum.Eeva);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ModelIsValid()
        {
            // Arrange & act
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase()
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                StatutoryServiceGeneralDescriptionId = gdId.ToString()
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase() { Id = Guid.NewGuid() });
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, commonService,
                channelService, null, UserRoleEnum.Eeva);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
    }
}
