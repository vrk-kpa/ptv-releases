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
using Xunit;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceHoursIsExtraValidatorTests : ValidatorTestBase
    {
        private const string PROPERTY_NAME = "Property name";

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new ServiceHoursIsExtraValidator(null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void OnlyIsExtraTrueSet_NotValid()
        {
            var vm = new List<V4VmOpenApiServiceHour> { new V4VmOpenApiServiceHour
            {
                //ServiceHourType = type.ToString(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V2VmOpenApiDailyOpeningTime>{
                    new V2VmOpenApiDailyOpeningTime { IsExtra = true } }
            } };

            // Arrange
            var validator = new ServiceHoursIsExtraValidator(vm, PROPERTY_NAME);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            var propertyName = $"{PROPERTY_NAME}[0].OpeningHour";
            controller.ModelState.ContainsKey(propertyName).Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue(propertyName, out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().BeEquivalentTo("Invalid list. One item with 'isExtra=false' is required!");
        }

        [Fact]
        public void OnlyIsExtraTrueSetForADay_NotValid()
        {
            var vm = new List<V4VmOpenApiServiceHour> { new V4VmOpenApiServiceHour
            {
                //ServiceHourType = type.ToString(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V2VmOpenApiDailyOpeningTime>
                {
                    new V2VmOpenApiDailyOpeningTime { DayFrom = DayOfWeek.Monday.ToString(), IsExtra = false },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = DayOfWeek.Monday.ToString(), IsExtra = true },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = DayOfWeek.Tuesday.ToString(), IsExtra = true } // Only IsExtra = true for Tuesday
                }
            } };

            // Arrange
            var validator = new ServiceHoursIsExtraValidator(vm, PROPERTY_NAME);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            var propertyName = $"{PROPERTY_NAME}[0].OpeningHour";
            controller.ModelState.ContainsKey(propertyName).Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue(propertyName, out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().BeEquivalentTo($"Invalid list. One item with 'isExtra=false' where 'DayFrom={DayOfWeek.Tuesday.ToString()}' is required!");
        }

        [Fact]
        public void MultipleIsExtraFalseSetForADay_NotValid()
        {
            var vm = new List<V4VmOpenApiServiceHour> { new V4VmOpenApiServiceHour
            {
                //ServiceHourType = type.ToString(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V2VmOpenApiDailyOpeningTime>
                {
                    new V2VmOpenApiDailyOpeningTime { DayFrom = DayOfWeek.Monday.ToString(), IsExtra = false },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = DayOfWeek.Monday.ToString(), IsExtra = false },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = DayOfWeek.Tuesday.ToString(), IsExtra = false }
                }
            } };

            // Arrange
            var validator = new ServiceHoursIsExtraValidator(vm, PROPERTY_NAME);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            var propertyName = $"{PROPERTY_NAME}[0].OpeningHour";
            controller.ModelState.ContainsKey(propertyName).Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue(propertyName, out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().BeEquivalentTo("Invalid list. Only one item with 'isExtra=false' for a day is accepted!");
        }

        [Fact]
        public void MultipleOpeningHours_Valid()
        {
            // Arrange
            var vm = new List<V4VmOpenApiServiceHour> { new V4VmOpenApiServiceHour
            {
                ServiceHourType = ServiceHoursTypeEnum.Standard.ToString(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V2VmOpenApiDailyOpeningTime>{
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "0", IsExtra = false },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "0", IsExtra = true },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "1", IsExtra = false },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "1", IsExtra = true },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "1", IsExtra = true }}
            } };
            var validator = new ServiceHoursIsExtraValidator(vm, PROPERTY_NAME);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
    }
}
