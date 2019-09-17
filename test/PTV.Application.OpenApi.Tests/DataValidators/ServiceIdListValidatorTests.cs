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
using Moq;
using PTV.Application.OpenApi.DataValidators;
using Xunit;
using System.Collections.Generic;
using System;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceIdListValidatorTests : ValidatorTestBase
    {
        public ServiceIdListValidatorTests()
        { 
            serviceServiceMockSetup.Setup(c => c.CheckServices(It.IsAny<List<Guid>>()))
                .Returns((List<Guid> idList) =>
                {
                    if (idList == null) return null;

                    var list = new List<Guid>();
                    idList.ForEach(i =>
                    {
                        if (i == null || i == Guid.Empty)
                        {
                            list.Add(i);
                        }
                    });
                    return list;
                });
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new ServiceIdListValidator(null, serviceService, "PropertyName");

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelListIncludesEmpty()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var list = new List<Guid>() { Guid.Empty, serviceId };
            var validator = new ServiceIdListValidator(list, serviceService, "PropertyName");

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            serviceServiceMockSetup.Verify(x => x.CheckServices(list), Times.Once());
        }

        [Fact]
        public void ModelIsValid()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var list = new List<Guid>() { serviceId };
            var validator = new ServiceIdListValidator(list, serviceService, "PropertyName");

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            serviceServiceMockSetup.Verify(x => x.CheckServices(list), Times.Once());
        }
    }
}
