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

using FluentAssertions;
using Moq;
using PTV.Application.OpenApi.DataValidators;
using Xunit;
using System.Collections.Generic;
using System;
using PTV.Domain.Model.Enums;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class OrganizationIdListValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new OrganizationIdListValidator(null, commonService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelListIncludesNull()
        {
            // Arrange
            commonServiceMockSetup.Setup(c => c.OrganizationExists(It.IsAny<Guid>(), PublishingStatus.Published)).Returns(true);
            var list = new List<string>() { null, Guid.NewGuid().ToString() };
            var validator = new OrganizationIdListValidator(list, commonService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelListIncludesInvalidOrganizationId()
        {
            // Arrange
            var validGuid = Guid.NewGuid();
            commonServiceMockSetup.Setup(c => c.OrganizationExists(validGuid, PublishingStatus.Published)).Returns(true);
            var list = new List<string>() { null, validGuid.ToString(), "invalidGuid" };
            var validator = new OrganizationIdListValidator(list, commonService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ModelListIncludesInvalidOrganization()
        {
            // Arrange
            var validGuid = Guid.NewGuid();
            var invalidGuid = Guid.NewGuid();
            commonServiceMockSetup.Setup(c => c.OrganizationExists(validGuid, PublishingStatus.Published)).Returns(true);
            commonServiceMockSetup.Setup(c => c.OrganizationExists(invalidGuid, PublishingStatus.Published)).Returns(false);
            var list = new List<string>() { null, validGuid.ToString(), invalidGuid.ToString() };
            var validator = new OrganizationIdListValidator(list, commonService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
