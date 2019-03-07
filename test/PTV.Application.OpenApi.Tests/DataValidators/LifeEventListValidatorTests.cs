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
using System.Linq;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class LifeEventListValidatorTests : FintoItemValidatorTestBase
    {
        private IList<string> list;
        public LifeEventListValidatorTests() : base()
        {
            list = new List<string> { null, "uri" };
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new LifeEventListValidator(null, fintoService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelSetAndFintoItemsNotExist()
        {
            // Arrange            
            fintoServiceMockSetup.Setup(g => g.CheckLifeEvents(It.IsAny<List<string>>())).Returns(list.ToList());
            var validator = new LifeEventListValidator(list, fintoService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ModelSetAndFintoItemsExist()
        {
            // Arrange            
            fintoServiceMockSetup.Setup(g => g.CheckLifeEvents(It.IsAny<List<string>>())).Returns((List<string>)null);
            var validator = new LifeEventListValidator(list, fintoService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
        
        [Fact]
        public void ModelIncludesNullAndFintoItemsExist()
        {
            // Arrange  
            fintoServiceMockSetup.Setup(g => g.CheckLifeEvents(It.IsAny <List<string>>()))
                .Returns((List<string> list) =>
                {
                    if (list.Any(i => string.IsNullOrEmpty(i))) return new List<string> { null };
                    return null;
                });
            var validator = new LifeEventListValidator(list, fintoService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }
    }
}
