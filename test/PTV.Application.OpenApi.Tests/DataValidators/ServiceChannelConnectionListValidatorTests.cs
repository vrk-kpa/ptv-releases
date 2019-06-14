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
using PTV.Domain.Model.Models.Security;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceChannelConnectionListValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange
            var validator = new ServiceChannelConnectionListValidator<V8VmOpenApiServiceChannelServiceInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(null, channelService, serviceService, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(It.IsAny<Guid>(), true), Times.Never);
        }

        [Fact]
        public void ChannelNotDefined()
        {
            // Arrange
            var list = new List<V8VmOpenApiServiceChannelServiceInBase> { new V8VmOpenApiServiceChannelServiceInBase() };
            var validator = new ServiceChannelConnectionListValidator<V8VmOpenApiServiceChannelServiceInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(list, channelService, serviceService, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(It.IsAny<Guid>(), true), Times.Once);
        }

        [Fact]
        public void ChannelNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var list = new List<V8VmOpenApiServiceChannelServiceInBase> { new V8VmOpenApiServiceChannelServiceInBase { ChannelGuid = id } };
            channelServiceMockSetup.Setup(x => x.GetServiceChannelByIdSimple(id, true)).Returns((VmOpenApiServiceChannel)null);
            var validator = new ServiceChannelConnectionListValidator<V8VmOpenApiServiceChannelServiceInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(list, channelService, serviceService, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, true), Times.Once);
            serviceServiceMockSetup.Verify(x => x.CheckServices(It.IsAny<List<Guid>>()), Times.Never);
        }
        
        [Theory]
        [InlineData(ServiceChannelTypeEnum.EChannel, true, false)]
        [InlineData(ServiceChannelTypeEnum.EChannel, false, true)]
        [InlineData(ServiceChannelTypeEnum.EChannel, true, true)]
        [InlineData(ServiceChannelTypeEnum.Phone, true, false)]
        [InlineData(ServiceChannelTypeEnum.Phone, false, true)]
        [InlineData(ServiceChannelTypeEnum.Phone, true, true)]
        [InlineData(ServiceChannelTypeEnum.PrintableForm, true, false)]
        [InlineData(ServiceChannelTypeEnum.PrintableForm, false, true)]
        [InlineData(ServiceChannelTypeEnum.PrintableForm, true, true)]
        [InlineData(ServiceChannelTypeEnum.WebPage, true, false)]
        [InlineData(ServiceChannelTypeEnum.WebPage, false, true)]
        [InlineData(ServiceChannelTypeEnum.WebPage, true, true)]
        public void ChannelExists_AdditionalDataAttached_NotValid(ServiceChannelTypeEnum type, bool serviceHoursSet, bool contactDetailsSet)
        {
            // Arrange
            var id = Guid.NewGuid();
            var list = new List<V8VmOpenApiServiceChannelServiceInBase> { new V8VmOpenApiServiceChannelServiceInBase
            {
                ChannelGuid = id,
                ServiceHours = serviceHoursSet ? new List<V8VmOpenApiServiceHour>{ new V8VmOpenApiServiceHour { OrderNumber = 1 } } : null,
                ContactDetails = contactDetailsSet ? new V8VmOpenApiContactDetailsInBase{ Addresses = new List<V7VmOpenApiAddressContactIn>()} : null
            } };
            channelServiceMockSetup.Setup(x => x.GetServiceChannelByIdSimple(id, true))
                .Returns(new VmOpenApiServiceChannel { Id = id, ServiceChannelType = type.ToString() });
            var validator = new ServiceChannelConnectionListValidator<V8VmOpenApiServiceChannelServiceInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(list, channelService, serviceService, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(id, true), Times.Once);
            serviceServiceMockSetup.Verify(x => x.CheckServices(It.IsAny<List<Guid>>()), Times.Never);
        }

//        [Theory]
//        [InlineData(true, false)]
//        [InlineData(false, true)]
//        [InlineData(true, true)]
//        public void ChannelExists_AdditionalDataAttached_Valid(bool serviceHoursSet, bool contactDetailsSet)
//        {
//            // Arrange
//            var channelId = Guid.NewGuid();
//            var serviceId = Guid.NewGuid();
//            var list = new List<V8VmOpenApiServiceChannelServiceInBase> { new V8VmOpenApiServiceChannelServiceInBase
//            {
//                ChannelGuid = channelId,
//                ServiceGuid = serviceId,
//                ServiceHours = serviceHoursSet ? new List<V8VmOpenApiServiceHour>{ new V8VmOpenApiServiceHour { OrderNumber = 1 } } : null,
//                ContactDetails = contactDetailsSet ? new V8VmOpenApiContactDetailsInBase{ Addresses = new List<V7VmOpenApiAddressContactIn>()} : null
//            } };
//            channelServiceMockSetup.Setup(x => x.GetServiceChannelByIdSimple(channelId, true))
//                .Returns(new VmOpenApiServiceChannel { Id = channelId, ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString() });
//            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(serviceId, true)).Returns(new VmOpenApiServiceVersionBase { Id = serviceId });
//            var validator = new ServiceChannelConnectionListValidator<V8VmOpenApiServiceChannelServiceInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(list, channelService, serviceService, codeService);
//
//            // Act
//            validator.Validate(controller.ModelState);
//
//            // Assert
//            controller.ModelState.IsValid.Should().BeTrue();
//            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once);
//            serviceServiceMockSetup.Verify(x => x.CheckServices(It.IsAny<List<Guid>>()), Times.Once);
//        }

        [Theory]
        [InlineData(ServiceChannelTypeEnum.EChannel)]
        [InlineData(ServiceChannelTypeEnum.Phone)]
        [InlineData(ServiceChannelTypeEnum.PrintableForm)]
        [InlineData(ServiceChannelTypeEnum.WebPage)]
        public void ChannelExists_AdditionalDataNotAttached_NotValid(ServiceChannelTypeEnum type)
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var list = new List<V8VmOpenApiServiceChannelServiceInBase> { new V8VmOpenApiServiceChannelServiceInBase
            {
                ChannelGuid = channelId,
                ServiceGuid = serviceId,
            } };
            channelServiceMockSetup.Setup(c => c.GetServiceChannelByIdSimple(channelId, It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceChannel()
                {
                    Id = channelId,
                    ServiceChannelType = type.ToString()
                });
            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(serviceId, true)).Returns(new VmOpenApiServiceVersionBase { Id = serviceId });
            var validator = new ServiceChannelConnectionListValidator<V8VmOpenApiServiceChannelServiceInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(list, channelService, serviceService, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once);
            serviceServiceMockSetup.Verify(x => x.CheckServices(It.IsAny<List<Guid>>()), Times.Never);
        }

        [Fact]
        public void ChannelExists_AdditionalDataNotAttached_Valid()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var list = new List<V8VmOpenApiServiceChannelServiceInBase> { new V8VmOpenApiServiceChannelServiceInBase
            {
                ChannelGuid = channelId,
                ServiceGuid = serviceId,
            } };
            channelServiceMockSetup.Setup(c => c.GetServiceChannelByIdSimple(channelId, It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceChannel()
                {
                    Id = channelId,
                    ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString()
                });
            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(serviceId, true)).Returns(new VmOpenApiServiceVersionBase { Id = serviceId });
            var validator = new ServiceChannelConnectionListValidator<V8VmOpenApiServiceChannelServiceInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(list, channelService, serviceService, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once);
            serviceServiceMockSetup.Verify(x => x.CheckServices(It.IsAny<List<Guid>>()), Times.Once);
        }

        [Fact]
        public void InvalidServiceAttached()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var serviceIdList = new List<Guid> { serviceId };
            channelServiceMockSetup.Setup(x => x.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel { Id = channelId, ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString() });
            serviceServiceMockSetup.Setup(x => x.CheckServices(serviceIdList)).Returns(serviceIdList);
            var vm = new List<V8VmOpenApiServiceChannelServiceInBase> { new V8VmOpenApiServiceChannelServiceInBase
            {
                ChannelGuid = channelId,
                ServiceGuid = serviceId
            } };
            var validator = new ServiceChannelConnectionListValidator<V8VmOpenApiServiceChannelServiceInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, serviceService, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once);
            serviceServiceMockSetup.Verify(x => x.CheckServices(serviceIdList), Times.Once);
        }

        [Fact]
        public void ValidServiceAttached()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var serviceIdList = new List<Guid> { serviceId };
            channelServiceMockSetup.Setup(x => x.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel { Id = channelId, ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString() });
            serviceServiceMockSetup.Setup(x => x.CheckServices(serviceIdList)).Returns((List<Guid>)null);
            var vm = new List<V8VmOpenApiServiceChannelServiceInBase> { new V8VmOpenApiServiceChannelServiceInBase
            {
                ChannelGuid = channelId,
                ServiceGuid = serviceId
            } };
            var validator = new ServiceChannelConnectionListValidator<V8VmOpenApiServiceChannelServiceInBase, V8VmOpenApiContactDetailsInBase, V8VmOpenApiServiceHour, V8VmOpenApiDailyOpeningTime>(vm, channelService, serviceService, codeService);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once);
            serviceServiceMockSetup.Verify(x => x.CheckServices(serviceIdList), Times.Once);
        }        
    }
}
