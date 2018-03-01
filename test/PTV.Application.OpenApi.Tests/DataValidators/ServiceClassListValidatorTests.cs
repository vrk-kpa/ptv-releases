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
using System;
using Xunit;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceClassListValidatorTests : FintoItemValidatorTestBase
    {
        private IList<string> list;
        public ServiceClassListValidatorTests() : base()
        {
            list = new List<string> { null, "uri" };
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new ServiceClassListValidator(null, fintoService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelSetAndFintoItemsNotExist()
        {
            // Arrange            
            fintoServiceMockSetup.Setup(g => g.GetServiceClassByUri(It.IsAny<string>())).Returns((IVmOpenApiFintoItemVersionBase)null);
            var validator = new ServiceClassListValidator(list, fintoService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ModelSetAndFintoItemsExist()
        {
            // Arrange            
            fintoServiceMockSetup.Setup(g => g.GetServiceClassByUri(It.IsAny<string>())).Returns(new VmOpenApiFintoItemVersionBase() { Id = Guid.NewGuid() } );
            var validator = new ServiceClassListValidator(list, fintoService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
        
        [Fact]
        public void ModelIncludesNullAndFintoItemsExist()
        {
            // Arrange  
            fintoServiceMockSetup.Setup(g => g.GetServiceClassByUri(It.IsNotNull<string>())).Returns(new VmOpenApiFintoItemVersionBase() { Id = Guid.NewGuid() });
            var validator = new ServiceClassListValidator(list, fintoService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
