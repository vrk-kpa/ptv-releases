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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class OntologyTermListValidatorTests : FintoItemValidatorTestBase
    {
        private Mock<IOntologyTermDataCache> ontologyTermDataCacheMock;
        private IOntologyTermDataCache ontologyTermDataCache;
        
        private IList<string> list;
        public OntologyTermListValidatorTests() : base()
        {
            list = new List<string> { null, "uri" };
            ontologyTermDataCacheMock = new Mock<IOntologyTermDataCache>();
            ontologyTermDataCache = ontologyTermDataCacheMock.Object;
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new OntologyTermListValidator(null, ontologyTermDataCache);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelSetAndFintoItemsNotExist()
        {
            // Arrange  
            ontologyTermDataCacheMock.Setup(o => o.HasUris(list)).Returns(new List<string>());
            var validator = new OntologyTermListValidator(list, ontologyTermDataCache);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ModelSetAndFintoItemsExist()
        {
            // Arrange        
            ontologyTermDataCacheMock.Setup(o => o.HasUris(It.IsAny<List<string>>())).Returns(list);
            var validator = new OntologyTermListValidator(list, ontologyTermDataCache);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
        
        [Fact]
        public void ModelIncludesNullAndFintoItemsExist()
        {
            // Arrange       
            ontologyTermDataCacheMock.Setup(o => o.HasUris(It.IsAny<List<string>>()))
                .Returns((List<string> list) => list.Where(i => !string.IsNullOrEmpty(i)).ToList());
            var validator = new OntologyTermListValidator(list, ontologyTermDataCache);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void AmountOfItemsGreaterThanValidCount()
        {
            // Arrange  
            ontologyTermDataCacheMock.Setup(o => o.HasUris(It.IsAny<List<string>>()))
                .Returns((List<string> list) =>
                {
                    return list;
                });
            var validator = new OntologyTermListValidator(new List<string> { "uri1", "uri2" }, ontologyTermDataCache, validCount: 1);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void AmountOfItemsSameThanValidCount()
        {
            // Arrange  
            ontologyTermDataCacheMock.Setup(o => o.HasUris(It.IsAny<List<string>>()))
                .Returns((List<string> list) =>
                {
                    return list;
                });
            var validator = new OntologyTermListValidator(new List<string> { "uri1", "uri2" }, ontologyTermDataCache, validCount: 2);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
    }
}
