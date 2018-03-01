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
using System;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceHourListValidatorTests : ValidatorTestBase
    {
        private const string PROPERTY_NAME = "Property name";

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new ServiceHourListValidator<VmOpenApiServiceHourBase>(null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ValidToEarlierThanValidFrom()
        {
            // Arrange
            var vm = new List<VmOpenApiServiceHourBase> { new VmOpenApiServiceHourBase
            {
                ServiceHourType = ServiceHoursTypeEnum.Standard.ToString(),
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddDays(-1)
            } };
            var validator = new ServiceHourListValidator<VmOpenApiServiceHourBase>(vm, PROPERTY_NAME);

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
            var vm = new List<VmOpenApiServiceHourBase> { new VmOpenApiServiceHourBase
            {
                ServiceHourType = ServiceHoursTypeEnum.Standard.ToString(),
                ValidForNow = true
            } };
            var validator = new ServiceHourListValidator<VmOpenApiServiceHourBase>(vm, PROPERTY_NAME);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NoOpeningHoursSet_TypeException_Valid()
        {
            // Arrange
            var vm = new List<VmOpenApiServiceHourBase> { new VmOpenApiServiceHourBase
            {
                ServiceHourType = ServiceHoursTypeEnum.Exception.ToString(),
                IsClosed = true,
                ValidForNow = true
            } };
            var validator = new ServiceHourListValidator<VmOpenApiServiceHourBase>(vm, PROPERTY_NAME);

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
            var vm = new List<VmOpenApiServiceHourBase> { new VmOpenApiServiceHourBase
            {
                ServiceHourType = type.ToString(),
                IsClosed = false,
                ValidForNow = true
            } };
            var validator = new ServiceHourListValidator<VmOpenApiServiceHourBase>(vm, PROPERTY_NAME);

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
            var vm = new List<VmOpenApiServiceHourBase> { new VmOpenApiServiceHourBase
            {
                ServiceHourType = type.ToString(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V2VmOpenApiDailyOpeningTime>{
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "0" } ,
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "1" }}
            } };
            var validator = new ServiceHourListValidator<VmOpenApiServiceHourBase>(vm, PROPERTY_NAME);

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
            error.ErrorMessage.Should().BeEquivalentTo("ServiceHour with ServiceHourType 'Exception' or 'Special' can only contain one OpeningHour item.");
        }

        [Theory]
        [InlineData(ServiceHoursTypeEnum.Standard)]
        [InlineData(ServiceHoursTypeEnum.Special)]
        [InlineData(ServiceHoursTypeEnum.Exception)]
        public void OnlyIsExtraDefined_NotValid(ServiceHoursTypeEnum type)
        {
            // Arrange
            var vm = new List<VmOpenApiServiceHourBase> { new VmOpenApiServiceHourBase
            {
                ServiceHourType = type.ToString(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V2VmOpenApiDailyOpeningTime>{
                    new V2VmOpenApiDailyOpeningTime { IsExtra = true } }
            } };
            var validator = new ServiceHourListValidator<VmOpenApiServiceHourBase>(vm, PROPERTY_NAME);

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

        [Theory]
        [InlineData(ServiceHoursTypeEnum.Standard)]
        [InlineData(ServiceHoursTypeEnum.Special)]
        [InlineData(ServiceHoursTypeEnum.Exception)]
        public void NotExtraDefined_ButForOtherDay_NotValid(ServiceHoursTypeEnum type)
        {
            // Arrange
            var vm = new List<VmOpenApiServiceHourBase> { new VmOpenApiServiceHourBase
            {
                ServiceHourType = type.ToString(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V2VmOpenApiDailyOpeningTime>{
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "0", IsExtra = false },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "1", IsExtra = true }}
            } };
            var validator = new ServiceHourListValidator<VmOpenApiServiceHourBase>(vm, PROPERTY_NAME);

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
            error.ErrorMessage.Should().StartWith("Invalid list. One item with 'isExtra=false' where");
        }

        [Fact]
        public void TypeStandard_MultipleOpeningHours_Valid()
        {
            // Arrange
            var vm = new List<VmOpenApiServiceHourBase> { new VmOpenApiServiceHourBase
            {
                ServiceHourType = ServiceHoursTypeEnum.Standard.ToString(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V2VmOpenApiDailyOpeningTime>{
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "0", IsExtra = false },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "0", IsExtra = true },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "1", IsExtra = false },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "1", IsExtra = true }}
            } };
            var validator = new ServiceHourListValidator<VmOpenApiServiceHourBase>(vm, PROPERTY_NAME);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(ServiceHoursTypeEnum.Standard)]
        [InlineData(ServiceHoursTypeEnum.Special)]
        [InlineData(ServiceHoursTypeEnum.Exception)]
        public void MultipleOpeningHours_MultipleNotExtraHoursForDay_NotValid(ServiceHoursTypeEnum type)
        {
            // Arrange
            var vm = new List<VmOpenApiServiceHourBase> { new VmOpenApiServiceHourBase
            {
                ServiceHourType = type.ToString(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V2VmOpenApiDailyOpeningTime>{
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "0", IsExtra = false },
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "0", IsExtra = false }}
            } };
            var validator = new ServiceHourListValidator<VmOpenApiServiceHourBase>(vm, PROPERTY_NAME);

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
        public void ToEarlierThanFrom_NotValid()
        {
            // Arrange
            var vm = new List<VmOpenApiServiceHourBase> { new VmOpenApiServiceHourBase
            {
                ServiceHourType = ServiceHoursTypeEnum.Standard.ToString(),
                ValidFrom = DateTime.Now,
                OpeningHour = new List<V2VmOpenApiDailyOpeningTime>{
                    new V2VmOpenApiDailyOpeningTime { DayFrom = "0", From = "14:00", To = "10:00" }}
            } };
            var validator = new ServiceHourListValidator<VmOpenApiServiceHourBase>(vm, PROPERTY_NAME);

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
