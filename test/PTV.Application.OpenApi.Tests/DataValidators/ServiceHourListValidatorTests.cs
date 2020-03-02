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
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceHourListValidatorTests : ValidatorTestBase
    {
        private const string PROPERTY_NAME = "Property name";

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new ServiceHourListValidator<V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ValidToEarlierThanValidFrom()
        {
            // Arrange
            var vm = new List<V8VmOpenApiServiceHour> { new V8VmOpenApiServiceHour
            {
                ServiceHourType = ServiceHoursTypeEnum.Standard.GetOpenApiValue(),
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddDays(-1)
            } };
            var validator = new ServiceHourListValidator<V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, PROPERTY_NAME);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            var propertyName = $"{PROPERTY_NAME}[0]";
            controller.ModelState.ContainsKey(propertyName).Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue(propertyName, out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().BeEquivalentTo("ValidTo cannot be earlier than ValidFrom.");
        }

        [Fact]
        public void NoOpeningHoursSet_TypeStandard_Valid()
        {
            // Arrange
            var vm = new List<V8VmOpenApiServiceHour> { new V8VmOpenApiServiceHour
            {
                ServiceHourType = ServiceHoursTypeEnum.Standard.GetOpenApiValue(),
                ValidForNow = true
            } };
            var validator = new ServiceHourListValidator<V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, PROPERTY_NAME);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NoOpeningHoursSet_TypeException_Valid()
        {
            // Arrange
            var vm = new List<V8VmOpenApiServiceHour> { new V8VmOpenApiServiceHour
            {
                ServiceHourType = ServiceHoursTypeEnum.Exception.GetOpenApiValue(),
                IsClosed = true,
                ValidForNow = true
            } };
            var validator = new ServiceHourListValidator<V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, PROPERTY_NAME);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(ServiceHoursTypeEnum.Special)]
        [InlineData(ServiceHoursTypeEnum.Exception)]
        public void NoOpeningHoursSet_NotValid(ServiceHoursTypeEnum type)
        {
            // Arrange
            var vm = new List<V8VmOpenApiServiceHour> { new V8VmOpenApiServiceHour
            {
                ServiceHourType = type.GetOpenApiValue(),
                IsClosed = false,
                ValidForNow = true
            } };
            var validator = new ServiceHourListValidator<V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, PROPERTY_NAME);

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
            error.ErrorMessage.Should().BeEquivalentTo("The OpeningHour field is required.");
        }

        [Theory]
        [InlineData(ServiceHoursTypeEnum.Special)]
        [InlineData(ServiceHoursTypeEnum.Exception)]
        public void TypeNotStandard_MultipleOpeningHours_NotValid(ServiceHoursTypeEnum type)
        {
            // Arrange
            var vm = new List<V8VmOpenApiServiceHour> { new V8VmOpenApiServiceHour
            {
                ServiceHourType = type.GetOpenApiValue(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V8VmOpenApiDailyOpeningTime>{
                    new V8VmOpenApiDailyOpeningTime { DayFrom = "0" } ,
                    new V8VmOpenApiDailyOpeningTime { DayFrom = "1" }}
            } };
            var validator = new ServiceHourListValidator<V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, PROPERTY_NAME);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            var propertyName = $"{PROPERTY_NAME}[0]";
            controller.ModelState.ContainsKey(propertyName).Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue(propertyName, out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().BeEquivalentTo("ServiceHour with ServiceHourType 'Exceptional' or 'OverMidnight' can only contain one OpeningHour item.");
        }

        [Fact]
        public void TypeStandard_MultipleOpeningHours_Valid()
        {
            // Arrange
            var vm = new List<V8VmOpenApiServiceHour> { new V8VmOpenApiServiceHour
            {
                ServiceHourType = ServiceHoursTypeEnum.Standard.GetOpenApiValue(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V8VmOpenApiDailyOpeningTime>{
                    new V8VmOpenApiDailyOpeningTime { DayFrom = "0" },
                    new V8VmOpenApiDailyOpeningTime { DayFrom = "0" },
                    new V8VmOpenApiDailyOpeningTime { DayFrom = "1" },
                    new V8VmOpenApiDailyOpeningTime { DayFrom = "1" }}
            } };
            var validator = new ServiceHourListValidator<V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, PROPERTY_NAME);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ToEarlierThanFrom_NotValid()
        {
            // Arrange
            var vm = new List<V8VmOpenApiServiceHour> { new V8VmOpenApiServiceHour
            {
                ServiceHourType = ServiceHoursTypeEnum.Standard.GetOpenApiValue(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V8VmOpenApiDailyOpeningTime>{
                    new V8VmOpenApiDailyOpeningTime { DayFrom = "0", From = "14:00", To = "10:00" }}
            } };
            var validator = new ServiceHourListValidator<V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, PROPERTY_NAME);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            var propertyName = $"{PROPERTY_NAME}[0].OpeningHour[0].To";
            controller.ModelState.ContainsKey(propertyName).Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue(propertyName, out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().BeEquivalentTo("To cannot be earlier than From.");
        }

    }
}
