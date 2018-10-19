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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V7;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceConnectionListValidatorTests : ValidatorTestBase
    {
        private Guid _id;
        private List<Guid> _idList;

        public ServiceConnectionListValidatorTests()
        {
            channelServiceMockSetup = new Mock<IChannelService>();
            channelService = channelServiceMockSetup.Object;
            _id = Guid.NewGuid();
            _idList = new List<Guid> { _id };
        }

        [Fact]
        public void CheckVisibility_UserOrganizationsNotSet()
        {
            // Arrange
            Action act = () => new ServiceConnectionListValidator<V8VmOpenApiServiceServiceChannelAstiInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(null, channelService, codeService, true);
            
            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new ServiceConnectionListValidator<V8VmOpenApiServiceServiceChannelAstiInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(null, channelService, codeService, false);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NoChannelsAttached()
        {
            // Arrange
            var validator = new ServiceConnectionListValidator<V8VmOpenApiServiceServiceChannelAstiInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>
                (new List<V8VmOpenApiServiceServiceChannelAstiInBase>() { new V8VmOpenApiServiceServiceChannelAstiInBase() }, channelService, codeService, false);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void CheckChannelsReturnsNull()
        {
            // Arrange
            channelServiceMockSetup.Setup(x => x.CheckChannels(_idList, null)).Returns((VmOpenApiConnectionChannels)null);
            var vm = new List<V8VmOpenApiServiceServiceChannelAstiInBase> { new V8VmOpenApiServiceServiceChannelAstiInBase { ChannelGuid = _id } };
            var validator = new ServiceConnectionListValidator<V8VmOpenApiServiceServiceChannelAstiInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NotExistingChannelsFound()
        {
            // Arrange
            channelServiceMockSetup.Setup(x => x.CheckChannels(_idList, null)).Returns(new VmOpenApiConnectionChannels { NotExistingChannels = _idList } );
            var vm = new List<V8VmOpenApiServiceServiceChannelAstiInBase> { new V8VmOpenApiServiceServiceChannelAstiInBase { ChannelGuid = _id } };
            var validator = new ServiceConnectionListValidator<V8VmOpenApiServiceServiceChannelAstiInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ChannelRelations").Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue("ChannelRelations", out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().StartWith("Some of the channels were not found:");
        }

        [Fact]
        public void AstiConnectionsForOtherThanServiceLocationChannelFound()
        {
            // Arrange
            channelServiceMockSetup.Setup(x => x.CheckChannels(_idList, null))
                .Returns(new VmOpenApiConnectionChannels());
            var vm = new List<V8VmOpenApiServiceServiceChannelAstiInBase> { new V8VmOpenApiServiceServiceChannelAstiInBase
            {
                ChannelGuid = _id,
                IsASTIConnection = true,
            } };
            var validator = new ServiceConnectionListValidator<V8VmOpenApiServiceServiceChannelAstiInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false, isASTI: true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ChannelRelations").Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue("ChannelRelations", out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().StartWith("Asti connections are only allowed for Service location channel!");
        }

        [Fact]
        public void AstiConnectionsForOtherThanServiceLocationChannelFound_AlsoServiceLocationChannelAttached()
        {
            // Arrange
            var channels = new List<Guid> { _id, Guid.NewGuid() };
            channelServiceMockSetup.Setup(x => x.CheckChannels(channels, null))
                .Returns(new VmOpenApiConnectionChannels { ServiceLocationChannels = _idList });
            var vm = new List<V8VmOpenApiServiceServiceChannelAstiInBase> { new V8VmOpenApiServiceServiceChannelAstiInBase
            {
                ChannelGuid = _id,
                IsASTIConnection = true,
            } };
            var validator = new ServiceConnectionListValidator<V8VmOpenApiServiceServiceChannelAstiInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false, isASTI: true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ChannelRelations").Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue("ChannelRelations", out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().StartWith("Asti connections are only allowed for Service location channel!");
        }

        [Fact]
        public void AstiConnectionsForOtherThanServiceLocationChannelFound_CheckChannelsReturnsNull()
        {
            // Arrange
            channelServiceMockSetup.Setup(x => x.CheckChannels(_idList, null))
                .Returns((VmOpenApiConnectionChannels)null);
            var vm = new List<V8VmOpenApiServiceServiceChannelAstiInBase> { new V8VmOpenApiServiceServiceChannelAstiInBase
            {
                ChannelGuid = _id,
                IsASTIConnection = true,
            } };
            var validator = new ServiceConnectionListValidator<V8VmOpenApiServiceServiceChannelAstiInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false, isASTI: true);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ChannelRelations").Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue("ChannelRelations", out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().StartWith("Asti connections are only allowed for Service location channel!");
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public void AdditionalInfoForOtherThanServiceLocationChannelFound(bool serviceHoursSet, bool contactDetailsSet)
        {
            // Arrange
            channelServiceMockSetup.Setup(x => x.CheckChannels(_idList, null))
                .Returns(new VmOpenApiConnectionChannels());
            var vm = new List<V8VmOpenApiServiceServiceChannelAstiInBase> { new V8VmOpenApiServiceServiceChannelAstiInBase
            {
                ChannelGuid = _id,
                ServiceHours = serviceHoursSet ? GetServiceHours() : null,
                ContactDetails = contactDetailsSet ? new V8VmOpenApiContactDetailsInBase{ Addresses = new List<V7VmOpenApiAddressContactIn>()} : null
            } };
            var validator = new ServiceConnectionListValidator<V8VmOpenApiServiceServiceChannelAstiInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ChannelRelations").Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue("ChannelRelations", out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().StartWith("ServiceHours and ContactDetails are only allowed for Service location channel!");
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public void AdditionalInfoForServiceLocationChannelFound(bool serviceHoursSet, bool contactDetailsSet)
        {
            // Arrange
            channelServiceMockSetup.Setup(x => x.CheckChannels(_idList, null))
                .Returns(new VmOpenApiConnectionChannels() { ServiceLocationChannels = _idList });
            var vm = new List<V8VmOpenApiServiceServiceChannelAstiInBase> { new V8VmOpenApiServiceServiceChannelAstiInBase
            {
                ChannelGuid = _id,
                ServiceHours = serviceHoursSet ? GetServiceHours() : null,
                ContactDetails = contactDetailsSet ? new V8VmOpenApiContactDetailsInBase{ Addresses = new List<V7VmOpenApiAddressContactIn>()} : null
            } };
            var validator = new ServiceConnectionListValidator<V8VmOpenApiServiceServiceChannelAstiInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        private List<V8VmOpenApiServiceHour> GetServiceHours()
        {
            return new List<V8VmOpenApiServiceHour>{ new V8VmOpenApiServiceHour
            {
                ServiceHourType = ServiceHoursTypeEnum.Standard.ToString(),                
            } };
        }
    }
}
