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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceConnectionListValidatorTests : ValidatorTestBase
    {
        private Guid _id;
        private List<Guid> _idList;
        private readonly int openApiVersion = 11;

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
            Action act = () => new ServiceConnectionListValidator<V11VmOpenApiServiceServiceChannelAstiInBase, V9VmOpenApiContactDetailsInBase, V11VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(null, channelService, codeService, true, openApiVersion);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new ServiceConnectionListValidator<V11VmOpenApiServiceServiceChannelAstiInBase, V9VmOpenApiContactDetailsInBase, V11VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(null, channelService, codeService, false, openApiVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NoChannelsAttached()
        {
            // Arrange
            var validator = new ServiceConnectionListValidator<V11VmOpenApiServiceServiceChannelAstiInBase, V9VmOpenApiContactDetailsInBase, V11VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>
                (new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase() }, channelService, codeService, false, openApiVersion);

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
            var vm = new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase { ChannelGuid = _id } };
            var validator = new ServiceConnectionListValidator<V11VmOpenApiServiceServiceChannelAstiInBase, V9VmOpenApiContactDetailsInBase, V11VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false, openApiVersion);

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
            var vm = new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase { ChannelGuid = _id } };
            var validator = new ServiceConnectionListValidator<V11VmOpenApiServiceServiceChannelAstiInBase, V9VmOpenApiContactDetailsInBase, V11VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false, openApiVersion);

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
            var vm = new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase
            {
                ChannelGuid = _id,
                IsASTIConnection = true,
            } };
            var validator = new ServiceConnectionListValidator<V11VmOpenApiServiceServiceChannelAstiInBase, V9VmOpenApiContactDetailsInBase, V11VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false, openApiVersion, isASTI: true);

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
            var vm = new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase
            {
                ChannelGuid = _id,
                IsASTIConnection = true,
            } };
            var validator = new ServiceConnectionListValidator<V11VmOpenApiServiceServiceChannelAstiInBase, V9VmOpenApiContactDetailsInBase, V11VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false, openApiVersion, isASTI: true);

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
            var vm = new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase
            {
                ChannelGuid = _id,
                IsASTIConnection = true,
            } };
            var validator = new ServiceConnectionListValidator<V11VmOpenApiServiceServiceChannelAstiInBase, V9VmOpenApiContactDetailsInBase, V11VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false, openApiVersion, isASTI: true);

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
            var vm = new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase
            {
                ChannelGuid = _id,
                ServiceHours = serviceHoursSet ? GetServiceHours() : null,
                ContactDetails = contactDetailsSet ? new V9VmOpenApiContactDetailsInBase{Addresses = new List<V7VmOpenApiAddressContactIn>()} : null
            } };
            var validator = new ServiceConnectionListValidator<V11VmOpenApiServiceServiceChannelAstiInBase, V9VmOpenApiContactDetailsInBase, V11VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false, openApiVersion);

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
                .Returns(new VmOpenApiConnectionChannels { ServiceLocationChannels = _idList });
            var vm = new List<V11VmOpenApiServiceServiceChannelAstiInBase> { new V11VmOpenApiServiceServiceChannelAstiInBase
            {
                ChannelGuid = _id,
                ServiceHours = serviceHoursSet ? GetServiceHours() : null,
                ContactDetails = contactDetailsSet ? new V9VmOpenApiContactDetailsInBase{ Addresses = new List<V7VmOpenApiAddressContactIn>()} : null
            } };
            var validator = new ServiceConnectionListValidator<V11VmOpenApiServiceServiceChannelAstiInBase, V9VmOpenApiContactDetailsInBase, V11VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, codeService, false, openApiVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        private static List<V11VmOpenApiServiceHour> GetServiceHours()
        {
            return new List<V11VmOpenApiServiceHour>{ new V11VmOpenApiServiceHour
            {
                ServiceHourType = ServiceHoursTypeEnum.Standard.GetOpenApiValue(),
                IsAlwaysOpen = true
            } };
        }
    }
}
