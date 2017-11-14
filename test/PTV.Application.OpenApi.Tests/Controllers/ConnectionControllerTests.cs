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
using Xunit;
using System.Collections.Generic;
using Moq;
using PTV.Application.OpenApi.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using PTV.Domain.Model.Models.OpenApi.V2;
using System;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using System.Linq;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class ConnectionControllerTests : ControllerTestBase
    {
        private ILogger<V7ConnectionController> logger;

        public ConnectionControllerTests()
        {
            var loggerMock = new Mock<ILogger<V7ConnectionController>>();
            logger = loggerMock.Object;
        }

        [Fact]
        public void PostServiceAndChannel_ModelIsNull()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);

            // Act
            var result = controller.PostServiceAndChannel(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PostServiceAndChannel_ModelIsValid()
        {
            // Arrange
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServicesAndChannels(It.IsAny<List<V7VmOpenApiServiceServiceChannelAstiInBase>>())).Returns(new List<string>() { "Return message" });
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);
            var model = new List<V7VmOpenApiServiceAndChannelIn>()
            {
                new V7VmOpenApiServiceAndChannelIn()
                {
                    ServiceId = Guid.NewGuid().ToString(),
                    ServiceChannelId = Guid.NewGuid().ToString(),
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(),
                }
            };

            // Act
            var result = controller.PostServiceAndChannel(model);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var resultOk = result as OkObjectResult;
            resultOk.Value.Should().BeOfType<List<string>>();
            ((List<string>)resultOk.Value).Count.ShouldBeEquivalentTo(1);
        }
        
        [Fact]
        public void PostServiceAndChannel_ModelIsNotValid()
        {
            // Arrange
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServicesAndChannels(It.IsAny<List<V7VmOpenApiServiceServiceChannelAstiInBase>>())).Returns(new List<string>() { "Return message" });
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);
            controller.ModelState.AddModelError("Validation Error", "Model not valid");
            var model = new List<V7VmOpenApiServiceAndChannelIn>()
            {
                new V7VmOpenApiServiceAndChannelIn()
                {
                    ServiceId = "1",
                    ServiceChannelId = "2",
                    ServiceChargeType = "Test",
                }
            };

            // Act
            var result = controller.PostServiceAndChannel(model);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PostServiceAndChannel_AllPropertiesAreSet()
        {
            // Arrange
            var text = "TestText";
            var type = "Type";
            var from = DateTime.Now;
            var to = from.AddDays(3);
            var language = "fi";
            var languageItemList = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Value = text, Language = language } };
            var model = new List<V7VmOpenApiServiceAndChannelIn>()
            {
                new V7VmOpenApiServiceAndChannelIn()
                {
                    ServiceId = Guid.NewGuid().ToString(),
                    ServiceChannelId = Guid.NewGuid().ToString(),
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(),
                    Description = new List<VmOpenApiLocalizedListItem>{ new VmOpenApiLocalizedListItem { Value = text, Language = language } },
                    ServiceHours = new List<V4VmOpenApiServiceHour>{ new V4VmOpenApiServiceHour
                    {
                        ServiceHourType = ServiceHoursTypeEnum.Standard.ToString(),
                        ValidFrom = from,
                        ValidTo = to,
                        OpeningHour = new List<V2VmOpenApiDailyOpeningTime>{new V2VmOpenApiDailyOpeningTime { DayFrom = text, DayTo = text} },
                        AdditionalInformation = languageItemList
                    } },
                    ContactDetails = new VmOpenApiContactDetailsIn
                    {
                        Emails = languageItemList,
                        Phones = new List<V4VmOpenApiPhone>{ new V4VmOpenApiPhone
                        { AdditionalInformation = text, ServiceChargeType = type, ChargeDescription = text, PrefixNumber = text, Number = text, Language = language} },
                        Addresses = new List<V7VmOpenApiAddressIn>{ new V7VmOpenApiAddressIn
                        { Type = AddressConsts.VISITING, SubType = AddressConsts.POSTOFFICEBOX, PostOfficeBoxAddress = new VmOpenApiAddressPostOfficeBoxIn{ PostOfficeBox = languageItemList} } },
                        WebPages = new List<VmOpenApiWebPageWithOrderNumber>{ new VmOpenApiWebPageWithOrderNumber { Url = text, Value = text, Language = language} }
                    },
                }
            };
            var successMsg = "Save succeeded";
            var errorMsg = "Save did not succeed";
            var errorList = new List<string> { errorMsg };
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServicesAndChannels(It.IsAny<List<V7VmOpenApiServiceServiceChannelAstiInBase>>()))
                .Returns((List<V7VmOpenApiServiceServiceChannelAstiInBase> list) =>
                {
                    if (list == null || list.Count == 0)
                    {
                        return errorList;
                    }
                    var item = list.FirstOrDefault();

                    if (string.IsNullOrEmpty(item.ServiceId) || string.IsNullOrEmpty(item.ServiceChannelId) || string.IsNullOrEmpty(item.ServiceChargeType) ||
                        item.Description == null || item.Description.Count == 0 || item.ServiceHours == null || item.ServiceHours.Count == 0 || item.ContactDetails == null)
                    {
                        return errorList;
                    }

                    var description = item.Description.FirstOrDefault();
                    if (string.IsNullOrEmpty(description.Value) || string.IsNullOrEmpty(description.Language))
                    {
                        return errorList;
                    }

                    var hour = item.ServiceHours.FirstOrDefault();
                    if (string.IsNullOrEmpty(hour.ServiceHourType) || !hour.ValidFrom.HasValue || !hour.ValidTo.HasValue || hour.OpeningHour == null ||
                        hour.OpeningHour.Count == 0 || hour.AdditionalInformation == null || hour.AdditionalInformation.Count == 0)
                    {
                        return errorList;
                    }

                    if (item.ContactDetails.Emails == null || item.ContactDetails.Emails.Count == 0 || item.ContactDetails.Phones == null || item.ContactDetails.Phones.Count == 0 ||
                    item.ContactDetails.Addresses == null || item.ContactDetails.Addresses.Count == 0 || item.ContactDetails.WebPages == null || item.ContactDetails.WebPages.Count == 0)
                    {
                        return errorList;
                    }

                    return new List<string> { successMsg };
                });
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);
            
            // Act
            var result = controller.PostServiceAndChannel(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultList = Assert.IsType<List<string>>(okResult.Value);
            resultList.Count.ShouldBeEquivalentTo(1);
            resultList.FirstOrDefault().Should().Be(successMsg);
        }

        [Fact]
        public void PutServiceAndChannel_ModelIsNull()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);

            // Act
            var result = controller.PutServiceAndChannel(Guid.NewGuid().ToString(), null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PutServiceAndChannel_ModelIsNotValid()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);
            controller.ModelState.AddModelError("Validation Error", "Model not valid");

            // Act
            var result = controller.PutServiceAndChannel(Guid.NewGuid().ToString(), new V7VmOpenApiServiceAndChannelRelationInBase());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PutServiceAndChannel_ServiceIdIsNull()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);

            // Act
            var result = controller.PutServiceAndChannel(null, new V7VmOpenApiServiceAndChannelRelationInBase());

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void PutServiceAndChannel_ServiceNotExists()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            serviceServiceMockSetup.Setup(s => s.ServiceExists(serviceId)).Returns(false);
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceServiceMockSetup.Object, channelService, userService, settings, logger);

            // Act
            var result = controller.PutServiceAndChannel(serviceId.ToString(), new V7VmOpenApiServiceAndChannelRelationInBase());

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void PutServiceAndChannel_CurrentVersionNotExists()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            serviceServiceMockSetup.Setup(s => s.ServiceExists(serviceId)).Returns(true);
            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(serviceId, true)).Returns((VmOpenApiServiceVersionBase)null);
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceServiceMockSetup.Object, channelService, userService, settings, logger);

            // Act
            var result = controller.PutServiceAndChannel(serviceId.ToString(), new V7VmOpenApiServiceAndChannelRelationInBase());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PutServiceAndChannel_ChannelNotExists()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            serviceServiceMockSetup.Setup(s => s.ServiceExists(serviceId)).Returns(true);
            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(serviceId, true))
                .Returns(new VmOpenApiServiceVersionBase() { PublishingStatus = PublishingStatus.Published.ToString() });
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(channelId, true)).Returns((VmOpenApiServiceChannel)null);
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceServiceMockSetup.Object, channelServiceMockSetup.Object,
                userService, settings, logger);

            // Act
            var result = controller.PutServiceAndChannel(serviceId.ToString(), new V7VmOpenApiServiceAndChannelRelationInBase()
            {
                ChannelRelations = new List<V7VmOpenApiServiceServiceChannelInBase>() { new V7VmOpenApiServiceServiceChannelInBase
                {
                    ServiceChannelId = channelId.ToString()
                } }
            });

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(channelId, true), Times.Once());
        }

        [Fact]
        public void PutServiceAndChannel_CanModifyConnections()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var request = new V7VmOpenApiServiceAndChannelRelationInBase()
            {
                ChannelRelations = new List<V7VmOpenApiServiceServiceChannelInBase>() { new V7VmOpenApiServiceServiceChannelInBase
                {
                    ServiceChannelId = channelId.ToString()
                } }
            };
            
            serviceServiceMockSetup.Setup(s => s.ServiceExists(serviceId)).Returns(true);
            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(serviceId, true))
                .Returns(new VmOpenApiServiceVersionBase() { PublishingStatus = PublishingStatus.Published.ToString() });
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel() { Id = channelId, IsVisibleForAll = true, ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString() });
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServiceConnections(It.IsAny<V7VmOpenApiServiceAndChannelRelationAstiInBase>(), defaultVersion))
                .Returns(new VmOpenApiServiceVersionBase());
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceServiceMockSetup.Object, channelServiceMockSetup.Object,
                userService, settings, logger);

            // Act
            var result = controller.PutServiceAndChannel(serviceId.ToString(), request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceVersionBase>(okResult.Value);
        }

        [Fact]
        public void PutServiceAndChannel_DeleteConnections()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var request = new V7VmOpenApiServiceAndChannelRelationInBase()
            {
                DeleteAllChannelRelations = true                
            };

            serviceServiceMockSetup.Setup(s => s.ServiceExists(serviceId)).Returns(true);
            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(serviceId, true))
                .Returns(new VmOpenApiServiceVersionBase() { PublishingStatus = PublishingStatus.Published.ToString() });
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel() { Id = channelId, IsVisibleForAll = true, ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString() });
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServiceConnections(It.IsAny<V7VmOpenApiServiceAndChannelRelationAstiInBase>(), defaultVersion))
                .Returns(new VmOpenApiServiceVersionBase());
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceServiceMockSetup.Object, channelServiceMockSetup.Object,
                userService, settings, logger);

            // Act
            var result = controller.PutServiceAndChannel(serviceId.ToString(), request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceVersionBase>(okResult.Value);
        }

        [Fact]
        public void PostServiceAndChannelBySource_ModelIsNull()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);

            // Act
            var result = controller.PostServiceAndChannelBySource(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PostServiceAndChannelBySource_ModelIsNotValid()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);
            controller.ModelState.AddModelError("Validation Error", "Model not valid");

            // Act
            var result = controller.PostServiceAndChannelBySource(new List<V7VmOpenApiServiceAndChannelRelationBySource>());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PostServiceAndChannelBySource_ModelIsValid()
        {
            // Arrange
            var request = new List<V7VmOpenApiServiceAndChannelRelationBySource>();
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServicesAndChannelsBySource(request)).Returns(new List<string>());
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);
            
            // Act
            var result = controller.PostServiceAndChannelBySource(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<List<string>>(okResult.Value);
        }

        [Fact]
        public void PutServiceAndChannelBySource_ModelIsNull()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);

            // Act
            var result = controller.PutServiceAndChannelBySource(Guid.NewGuid().ToString(), null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PutServiceAndChannelBySource_ModelIsNotValid()
        {
            // Arrange
            var serviceSourceId = "serviceSourceId";
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);
            controller.ModelState.AddModelError("Validation Error", "Model not valid");

            // Act
            var result = controller.PutServiceAndChannelBySource(serviceSourceId, new V7VmOpenApiServiceAndChannelRelationBySourceInBase());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PutServiceAndChannelBySource_ModelIsValid()
        {
            // Arrange
            var serviceSourceId = "serviceSourceId";
            var request = new V7VmOpenApiServiceAndChannelRelationBySourceInBase();
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServiceConnectionsBySource(serviceSourceId, It.IsAny< V7VmOpenApiServiceAndChannelRelationBySourceAsti>(), defaultVersion))
                .Returns(new VmOpenApiServiceVersionBase());
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);

            // Act
            var result = controller.PutServiceAndChannelBySource(serviceSourceId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceVersionBase>(okResult.Value);
        }

        [Fact]
        public void PutServiceAndChannelBySource_DeleteConnections()
        {
            // Arrange
            var serviceSourceId = "serviceSourceId";
            var request = new V7VmOpenApiServiceAndChannelRelationBySourceInBase { DeleteAllChannelRelations = true };
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServiceConnectionsBySource(serviceSourceId, It.IsAny<V7VmOpenApiServiceAndChannelRelationBySourceAsti>(), defaultVersion))
                .Returns(new VmOpenApiServiceVersionBase());
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);

            // Act
            var result = controller.PutServiceAndChannelBySource(serviceSourceId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceVersionBase>(okResult.Value);
        }

        [Fact]
        public void ASTIPutServiceAndChannel_ModelIsNull()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);

            // Act
            var result = controller.ASTIPutServiceAndChannel(Guid.NewGuid().ToString(), null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ASTIPutServiceAndChannelBySource_ModelIsNull()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);

            // Act
            var result = controller.ASTIPutServiceAndChannelBySource(Guid.NewGuid().ToString(), null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_ModelIsNull()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);

            // Act
            var result = controller.ASTIPutServiceAndChannelByChannel(Guid.NewGuid().ToString(), null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_ModelIsNotValid()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);
            controller.ModelState.AddModelError("Validation Error", "Model not valid");

            // Act
            var result = controller.ASTIPutServiceAndChannelByChannel(Guid.NewGuid().ToString(), new V7VmOpenApiChannelServicesIn());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_ChannelIdIsNull()
        {
            // Arrange
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceService, channelService, userService, settings, logger);

            // Act
            var result = controller.ASTIPutServiceAndChannelByChannel(null, new V7VmOpenApiChannelServicesIn());

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_ChannelNotExists()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            channelServiceMockSetup.Setup(s => s.ChannelExists(channelId)).Returns(false);
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceServiceMockSetup.Object, channelServiceMockSetup.Object,
                userService, settings, logger);

            // Act
            var result = controller.ASTIPutServiceAndChannelByChannel(channelId.ToString(), new V7VmOpenApiChannelServicesIn());

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_ServiceNotExists()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var vm = new V7VmOpenApiChannelServicesIn()
            {
                ServiceRelations = new List<V7VmOpenApiServiceChannelServiceInBase>
                {
                    new V7VmOpenApiServiceChannelServiceInBase{ServiceId = serviceId.ToString()}
                }
            };
            channelServiceMockSetup.Setup(s => s.ChannelExists(channelId)).Returns(true);
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel() { Id = channelId });
            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(serviceId, true))
                .Returns((VmOpenApiServiceVersionBase)null);
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceServiceMockSetup.Object, channelServiceMockSetup.Object,
                userService, settings, logger);

            // Act
            var result = controller.ASTIPutServiceAndChannelByChannel(channelId.ToString(), vm);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_CanModifyConnections()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var request = new V7VmOpenApiChannelServicesIn()
            {
                ServiceRelations = new List<V7VmOpenApiServiceChannelServiceInBase>() { new V7VmOpenApiServiceChannelServiceInBase
                {
                    ServiceId = serviceId.ToString()
                } }
            };
            channelServiceMockSetup.Setup(s => s.ChannelExists(channelId)).Returns(true);
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel { Id = channelId, IsVisibleForAll = true, ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString() });
            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(serviceId, true))
                .Returns(new VmOpenApiServiceVersionBase() { Id = serviceId });
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServiceChannelConnections(It.IsAny<V7VmOpenApiChannelServicesIn>(), defaultVersion))
                .Returns(new VmOpenApiServiceChannel());
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceServiceMockSetup.Object, channelServiceMockSetup.Object,
                userService, settings, logger);

            // Act
            var result = controller.ASTIPutServiceAndChannelByChannel(channelId.ToString(), request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceChannel>(okResult.Value);
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_DeleteConnections()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var request = new V7VmOpenApiChannelServicesIn()
            {
                DeleteAllServiceRelations = true
            };
            channelServiceMockSetup.Setup(s => s.ChannelExists(channelId)).Returns(true);
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(channelId, true))
                .Returns(new VmOpenApiServiceChannel { Id = channelId, IsVisibleForAll = true, ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString() });
            serviceServiceMockSetup.Setup(s => s.GetServiceByIdSimple(serviceId, true))
                .Returns(new VmOpenApiServiceVersionBase() { Id = serviceId });
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServiceChannelConnections(It.IsAny<V7VmOpenApiChannelServicesIn>(), defaultVersion))
                .Returns(new VmOpenApiServiceChannel());
            var controller = new V7ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceServiceMockSetup.Object, channelServiceMockSetup.Object,
                userService, settings, logger);

            // Act
            var result = controller.ASTIPutServiceAndChannelByChannel(channelId.ToString(), request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceChannel>(okResult.Value);
        }
    }
}
