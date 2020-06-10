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
using Xunit;
using System.Collections.Generic;
using Moq;
using PTV.Application.OpenApi.Controllers;
using Microsoft.Extensions.Logging;

using System;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public class ConnectionControllerTests : ControllerTestBase
    {
        private const string TEXT = "TestText";
        private const string LANGUAGE = "fi";
        private const string URL = "Url";

        private ILogger<V9ConnectionController> logger;

        private List<VmOpenApiLanguageItem> _languageItemList = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Value = TEXT, Language = LANGUAGE } };
        private List<VmOpenApiLocalizedListItem> _localizedItemList = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Value = TEXT, Language = LANGUAGE } };

        private DateTime _from;
        private DateTime _to;

        private Guid _channelId;
        private Guid _serviceId;
        private List<Guid> _serviceIdList;
        private List<Guid> _channelIdList;
        private List<Guid> _userOrganizationIdList;

        private V9ConnectionController _controller;

        public ConnectionControllerTests()
        {
            var loggerMock = new Mock<ILogger<V9ConnectionController>>();
            logger = loggerMock.Object;

            _from = DateTime.Now;
            _to = _from.AddDays(3);

            _channelId = Guid.NewGuid();
            _serviceId = Guid.NewGuid();
            _serviceIdList = new List<Guid> { _serviceId };
            _channelIdList = new List<Guid> { _channelId };
            _userOrganizationIdList = new List<Guid> { userOrganizationId };

            _controller = new V9ConnectionController(serviceAndChannelServiceMockSetup.Object, serviceServiceMockSetup.Object, channelServiceMockSetup.Object,
                userService, codeServiceMockSetup.Object, settings, logger);
        }

        #region POST service and channel
        [Fact]
        public void PostServiceAndChannel_ModelIsNull()
        {
            // Act
            var result = _controller.PostServiceAndChannel(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PostServiceAndChannel_ModelIsNotValid()
        {
            // Arrange
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServicesAndChannels(It.IsAny<List<V11VmOpenApiServiceServiceChannelAstiInBase>>())).Returns(new List<string> { "Return message" });
            _controller.ModelState.AddModelError("Validation Error", "Model not valid");
            var model = new List<V9VmOpenApiServiceAndChannelIn>
            {
                new V9VmOpenApiServiceAndChannelIn
                {
                    ServiceId = "1",
                    ServiceChannelId = "2",
                    ServiceChargeType = "Test",
                }
            };

            // Act
            var result = _controller.PostServiceAndChannel(model);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PostServiceAndChannel_ModelIsValid()
        {
            // Arrange
            var model = new List<V9VmOpenApiServiceAndChannelIn>
            {
                new V9VmOpenApiServiceAndChannelIn
                {
                    ServiceId = _serviceId.ToString(),
                    ServiceChannelId = _channelId.ToString(),
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.GetOpenApiValue(),
                    Description = _localizedItemList,
                    ServiceHours = GetServiceHours(),
                    ContactDetails = GetContactDetails<V9VmOpenApiContactDetailsIn>()
                }
            };
            var successMsg = "Save succeeded";
            var errorMsg = "Save did not succeed";
            var errorList = new List<string> { errorMsg };
            codeServiceMockSetup.Setup(c => c.GetDialCode(TEXT)).Returns(new VmDialCode { Id = Guid.NewGuid() });
            channelServiceMockSetup.Setup(x => x.GetServiceChannelByIdSimple(It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceChannel {Id = _channelId});
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServicesAndChannels(It.IsAny<List<V11VmOpenApiServiceServiceChannelAstiInBase>>()))
                .Returns((List<V11VmOpenApiServiceServiceChannelAstiInBase> list) =>
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

                    if (item.ContactDetails.Emails == null || item.ContactDetails.Emails.Count == 0 || item.ContactDetails.PhoneNumbers == null || item.ContactDetails.PhoneNumbers.Count == 0 ||
                    item.ContactDetails.Addresses == null || item.ContactDetails.Addresses.Count == 0 || item.ContactDetails.WebPages == null || item.ContactDetails.WebPages.Count == 0)
                    {
                        return errorList;
                    }

                    return new List<string> { successMsg };
                });

            // Act
            var result = _controller.PostServiceAndChannel(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultList = Assert.IsType<List<string>>(okResult.Value);
            resultList.Count.Should().Be(1);
            resultList.FirstOrDefault().Should().Be(successMsg);
        }

        #endregion
        #region PUT service and channel
        [Fact]
        public void PutServiceAndChannel_ModelIsNull()
        {
            // Act
            var result = _controller.PutServiceAndChannel(_serviceId.ToString(), null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            serviceServiceMockSetup.Verify(x => x.GetLatestVersionPublishingStatus(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public void PutServiceAndChannel_ModelIsNotValid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Validation Error", "Model not valid");

            // Act
            var result = _controller.PutServiceAndChannel(_serviceId.ToString(), new V9VmOpenApiServiceAndChannelRelationInBase());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            serviceServiceMockSetup.Verify(x => x.GetLatestVersionPublishingStatus(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public void PutServiceAndChannel_ServiceIdIsNull()
        {
            // Act
            var result = _controller.PutServiceAndChannel(null, new V9VmOpenApiServiceAndChannelRelationInBase());

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            serviceServiceMockSetup.Verify(x => x.GetLatestVersionPublishingStatus(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public void PutServiceAndChannel_ServiceNotExists()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetLatestVersionPublishingStatus(_serviceId)).Returns((PublishingStatus?)null);

            // Act
            var result = _controller.PutServiceAndChannel(_serviceId.ToString(), new V9VmOpenApiServiceAndChannelRelationInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            serviceServiceMockSetup.Verify(x => x.GetLatestVersionPublishingStatus(It.IsAny<Guid>()), Times.Once);
            var value = Assert.IsType<VmError>(notFoundResult.Value);
            value.ErrorMessage.Should().Be($"Service with id '{_serviceId}' not found.");
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void PutServiceAndChannel_ServiceHasNotValidStatus(PublishingStatus status)
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetLatestVersionPublishingStatus(_serviceId))
                .Returns(status);

            // Act
            var result = _controller.PutServiceAndChannel(_serviceId.ToString(), new V9VmOpenApiServiceAndChannelRelationInBase());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            serviceServiceMockSetup.Verify(x => x.GetLatestVersionPublishingStatus(It.IsAny<Guid>()), Times.Once);
            var value = Assert.IsType<VmError>(notFoundResult.Value);
            value.ErrorMessage.Should().Be($"Service with id '{_serviceId}' not found.");
        }

        [Fact]
        public void PutServiceAndChannel_ChannelNotExists()
        {
            // Arrange
            serviceServiceMockSetup.Setup(s => s.GetLatestVersionPublishingStatus(_serviceId))
                .Returns(PublishingStatus.Published);
            channelServiceMockSetup.Setup(s => s.CheckChannels(_channelIdList, _userOrganizationIdList))
                .Returns(new VmOpenApiConnectionChannels { NotExistingChannels = new List<Guid> { _channelId } });

            // Act
            var result = _controller.PutServiceAndChannel(_serviceId.ToString(), new V9VmOpenApiServiceAndChannelRelationInBase
            {
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelInBase>
                { new V9VmOpenApiServiceServiceChannelInBase
                {
                    ServiceChannelId = _channelId.ToString()
                } }
            });

            // Assert
            var BadRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            serviceServiceMockSetup.Verify(x => x.GetLatestVersionPublishingStatus(It.IsAny<Guid>()), Times.Once);
            channelServiceMockSetup.Verify(x => x.CheckChannels(_channelIdList, _userOrganizationIdList), Times.Once());
            var errors = Assert.IsType<SerializableError>(BadRequestObjectResult.Value);
            errors.Should().NotBeNull();
            var error = errors.First();
            error.Key.Should().Be("ChannelRelations");
        }

        [Fact]
        public void PutServiceAndChannel_CanModifyConnections()
        {
            // Arrange
            var request = new V9VmOpenApiServiceAndChannelRelationInBase
            {
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelInBase>
                { new V9VmOpenApiServiceServiceChannelInBase
                {
                    ServiceChannelId = _channelId.ToString(),
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.GetOpenApiValue(),
                    Description = _localizedItemList,
                    ServiceHours = GetServiceHours(),
                    ContactDetails = GetContactDetails<V9VmOpenApiContactDetailsInBase>(),
                } }
            };

            codeServiceMockSetup.Setup(c => c.GetDialCode(TEXT)).Returns(new VmDialCode { Id = Guid.NewGuid() });
            serviceServiceMockSetup.Setup(s => s.GetLatestVersionPublishingStatus(_serviceId))
                .Returns(PublishingStatus.Published);
            channelServiceMockSetup.Setup(s => s.CheckChannels(_channelIdList, _userOrganizationIdList))
                .Returns(new VmOpenApiConnectionChannels { ServiceLocationChannels = _channelIdList });

            MockSaveServiceConnections();

            // Act
            var result = _controller.PutServiceAndChannel(_serviceId.ToString(), request);

            // Assert
            serviceServiceMockSetup.Verify(x => x.GetLatestVersionPublishingStatus(It.IsAny<Guid>()), Times.Once);
            channelServiceMockSetup.Verify(x => x.CheckChannels(_channelIdList, _userOrganizationIdList), Times.Once());
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceConnections(It.IsAny<V11VmOpenApiServiceAndChannelRelationAstiInBase>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
            CheckServiceResult(result, _channelId, false);
        }

        [Fact]
        public void PutServiceAndChannel_DeleteConnections()
        {
            // Arrange
            var request = new V9VmOpenApiServiceAndChannelRelationInBase
            {
                DeleteAllChannelRelations = true
            };

            serviceServiceMockSetup.Setup(s => s.GetLatestVersionPublishingStatus(_serviceId))
                .Returns(PublishingStatus.Published);
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServiceConnections(It.IsAny<V11VmOpenApiServiceAndChannelRelationAstiInBase>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceVersionBase());

            // Act
            var result = _controller.PutServiceAndChannel(_serviceId.ToString(), request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceVersionBase>(okResult.Value);
            serviceServiceMockSetup.Verify(x => x.GetLatestVersionPublishingStatus(It.IsAny<Guid>()), Times.Once);
            channelServiceMockSetup.Verify(x => x.CheckChannels(It.IsAny<List<Guid>>(), It.IsAny< List<Guid>>()), Times.Never);
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceConnections(It.IsAny<V11VmOpenApiServiceAndChannelRelationAstiInBase>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public void PutServiceAndChannel_DuplicateItemsInRequest()
        {
            // Arrange
            var request = new V9VmOpenApiServiceAndChannelRelationInBase
            {
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelInBase>
                {
                    new V9VmOpenApiServiceServiceChannelInBase
                    {
                        ServiceChannelId = _channelId.ToString(),
                    },
                    new V9VmOpenApiServiceServiceChannelInBase
                    {
                        ServiceChannelId = _channelId.ToString(),
                    },
                }
            };

            serviceServiceMockSetup.Setup(s => s.GetLatestVersionPublishingStatus(_serviceId))
                .Returns(PublishingStatus.Published);

            // Act
            var result = _controller.PutServiceAndChannel(_serviceId.ToString(), request);

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            serviceServiceMockSetup.Verify(x => x.GetLatestVersionPublishingStatus(It.IsAny<Guid>()), Times.Once);
            channelServiceMockSetup.Verify(x => x.CheckChannels(It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>()), Times.Never);
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceConnections(It.IsAny<V11VmOpenApiServiceAndChannelRelationAstiInBase>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        #endregion
        #region POST service and channel by source

        [Fact]
        public void PostServiceAndChannelBySource_ModelIsNull()
        {
            // Act
            var result = _controller.PostServiceAndChannelBySource(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PostServiceAndChannelBySource_ModelIsNotValid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Validation Error", "Model not valid");

            // Act
            var result = _controller.PostServiceAndChannelBySource(new List<V9VmOpenApiServiceAndChannelRelationBySource>());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PostServiceAndChannelBySource_ModelIsValid()
        {
            // Arrange
            var request = new List<V9VmOpenApiServiceAndChannelRelationBySource>();
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServicesAndChannelsBySource(It.IsAny<List<V11VmOpenApiServiceAndChannelRelationBySource>>())).Returns(new List<string>());

            // Act
            var result = _controller.PostServiceAndChannelBySource(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<List<string>>(okResult.Value);
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServicesAndChannelsBySource(It.IsAny<List<V11VmOpenApiServiceAndChannelRelationBySource>>()), Times.Once);
        }

        #endregion
        #region PUT service and channel by source

        [Fact]
        public void PutServiceAndChannelBySource_ModelIsNull()
        {
            // Act
            var result = _controller.PutServiceAndChannelBySource("sourceId", null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceConnectionsBySource(It.IsAny<string>(), It.IsAny<V11VmOpenApiServiceAndChannelRelationBySourceAsti>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public void PutServiceAndChannelBySource_ModelIsNotValid()
        {
            // Arrange
            var serviceSourceId = "serviceSourceId";
            _controller.ModelState.AddModelError("Validation Error", "Model not valid");

            // Act
            var result = _controller.PutServiceAndChannelBySource(serviceSourceId, new V9VmOpenApiServiceAndChannelRelationBySourceInBase());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceConnectionsBySource(It.IsAny<string>(), It.IsAny<V11VmOpenApiServiceAndChannelRelationBySourceAsti>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public void PutServiceAndChannelBySource_ModelIsValid()
        {
            // Arrange
            var serviceSourceId = "serviceSourceId";
            var channelSourceId = "channelSourceId";
            var request = new V9VmOpenApiServiceAndChannelRelationBySourceInBase
            {
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelBySource> { new V9VmOpenApiServiceServiceChannelBySource
                {
                    ServiceChannelSourceId = channelSourceId,
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.GetOpenApiValue(),
                    Description = _localizedItemList,
                    ServiceHours = GetServiceHours(),
                    ContactDetails = GetContactDetails<V9VmOpenApiContactDetailsInBase>()
                } }
            };
            codeServiceMockSetup.Setup(c => c.GetDialCode(TEXT)).Returns(new VmDialCode { Id = Guid.NewGuid() });
            MockSaveServiceConnectionsBySource(serviceSourceId);

            // Act
            var result = _controller.PutServiceAndChannelBySource(serviceSourceId, request);

            // Assert
            CheckServiceResult(result, null, false);
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceConnectionsBySource(It.IsAny<string>(), It.IsAny<V11VmOpenApiServiceAndChannelRelationBySourceAsti>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public void PutServiceAndChannelBySource_DeleteConnections()
        {
            // Arrange
            var serviceSourceId = "serviceSourceId";
            var request = new V9VmOpenApiServiceAndChannelRelationBySourceInBase { DeleteAllChannelRelations = true };
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServiceConnectionsBySource(serviceSourceId, It.IsAny<V11VmOpenApiServiceAndChannelRelationBySourceAsti>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new VmOpenApiServiceVersionBase());

            // Act
            var result = _controller.PutServiceAndChannelBySource(serviceSourceId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceVersionBase>(okResult.Value);
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceConnectionsBySource(It.IsAny<string>(), It.IsAny<V11VmOpenApiServiceAndChannelRelationBySourceAsti>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public void PutServiceAndChannelBySource_DuplicateItemsInRequest()
        {
            // Arrange
            var serviceSourceId = "serviceSourceId";
            var channelSourceId = "channelSourceId";
            var request = new V9VmOpenApiServiceAndChannelRelationBySourceInBase
            {
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelBySource> {
                    new V9VmOpenApiServiceServiceChannelBySource
                    {
                        ServiceChannelSourceId = channelSourceId,
                    },
                    new V9VmOpenApiServiceServiceChannelBySource
                    {
                        ServiceChannelSourceId = channelSourceId,
                    }
                }
            };

            // Act
            var result = _controller.PutServiceAndChannelBySource(serviceSourceId, request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceConnectionsBySource(It.IsAny<string>(), It.IsAny<V11VmOpenApiServiceAndChannelRelationBySourceAsti>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        #endregion
        #region PUT ASTI service and channel

        [Fact]
        public void ASTIPutServiceAndChannel_ModelIsNull()
        {
            // Act
            var result = _controller.ASTIPutServiceAndChannel(_serviceId.ToString(), null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ASTIPutServiceAndChannel_CanModifyConnections()
        {
            // Arrange
            var request = new V9VmOpenApiServiceAndChannelRelationAstiInBase
            {
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelAstiInBase>
                { new V9VmOpenApiServiceServiceChannelAstiInBase
                {
                    ServiceChannelId = _channelId.ToString(),
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.GetOpenApiValue(),
                    Description = _localizedItemList,
                    ExtraTypes = GetExtraTypes(),
                    ServiceHours = GetServiceHours(),
                    ContactDetails = GetContactDetails<V9VmOpenApiContactDetailsInBase>(),
                } }
            };

            codeServiceMockSetup.Setup(c => c.GetDialCode(TEXT)).Returns(new VmDialCode { Id = Guid.NewGuid() });
            serviceServiceMockSetup.Setup(s => s.GetLatestVersionPublishingStatus(_serviceId))
                .Returns(PublishingStatus.Published);
            channelServiceMockSetup.Setup(s => s.CheckChannels(_channelIdList, null))
                .Returns(new VmOpenApiConnectionChannels { ServiceLocationChannels = _channelIdList });

            MockSaveServiceConnections();

            // Act
            var result = _controller.ASTIPutServiceAndChannel(_serviceId.ToString(), request);

            // Assert
            channelServiceMockSetup.Verify(x => x.CheckChannels(_channelIdList, null), Times.Once);
            CheckServiceResult(result, _channelId, true);
        }

        [Fact]
        public void ASTIPutServiceAndChannelBySource_ModelIsNull()
        {
            // Act
            var result = _controller.ASTIPutServiceAndChannelBySource(Guid.NewGuid().ToString(), null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceConnectionsBySource(It.IsAny<string>(), It.IsAny<V11VmOpenApiServiceAndChannelRelationBySourceAsti>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public void ASTIPutServiceAndChannelBySource_ModelIsValid()
        {
            // Arrange
            var serviceSourceId = "serviceSourceId";
            var channelSourceId = "channelSourceId";
            var request = new V9VmOpenApiServiceAndChannelRelationBySourceAsti
            {
                ChannelRelations = new List<V9VmOpenApiServiceServiceChannelBySourceAsti> { new V9VmOpenApiServiceServiceChannelBySourceAsti
                {
                    ServiceChannelSourceId = channelSourceId,
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.GetOpenApiValue(),
                    Description = _localizedItemList,
                    ExtraTypes = GetExtraTypes(),
                    ServiceHours = GetServiceHours(),
                    ContactDetails = GetContactDetails<V9VmOpenApiContactDetailsInBase>()
                } }
            };
            codeServiceMockSetup.Setup(c => c.GetDialCode(TEXT)).Returns(new VmDialCode { Id = Guid.NewGuid() });
            MockSaveServiceConnectionsBySource(serviceSourceId);

            // Act
            var result = _controller.ASTIPutServiceAndChannelBySource(serviceSourceId, request);

            // Assert
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceConnectionsBySource(serviceSourceId, It.IsAny<V11VmOpenApiServiceAndChannelRelationBySourceAsti>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
            CheckServiceResult(result, null, true);
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_ModelIsNull()
        {
            // Act
            var result = _controller.ASTIPutServiceAndChannelByChannel(Guid.NewGuid().ToString(), null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(It.IsAny<Guid>(), true), Times.Never);
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_ModelIsNotValid()
        {
            _controller.ModelState.AddModelError("Validation Error", "Model not valid");

            // Act
            var result = _controller.ASTIPutServiceAndChannelByChannel(Guid.NewGuid().ToString(), new V9VmOpenApiChannelServicesIn());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(It.IsAny<Guid>(), true), Times.Never);
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_ChannelIdIsNull()
        {
            // Act
            var result = _controller.ASTIPutServiceAndChannelByChannel(null, new V9VmOpenApiChannelServicesIn());

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(It.IsAny<Guid>(), true), Times.Never);
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_ChannelNotExists()
        {
            // Arrange
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(_channelId, true))
                .Returns((VmOpenApiServiceChannel)null);

            var vm = new V9VmOpenApiChannelServicesIn
            {
                ServiceRelations = new List<V9VmOpenApiServiceChannelServiceInBase>{ new V9VmOpenApiServiceChannelServiceInBase
                {
                    ServiceId = Guid.NewGuid().ToString()
                } }
            };
            // Act
            var result = _controller.ASTIPutServiceAndChannelByChannel(_channelId.ToString(), vm);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(It.IsAny<Guid>(), true), Times.Once());
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceChannelConnections(It.IsAny<V11VmOpenApiChannelServicesIn>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_ServiceNotExists()
        {
            // Arrange
            var vm = new V9VmOpenApiChannelServicesIn
            {
                ServiceRelations = new List<V9VmOpenApiServiceChannelServiceInBase>
                {
                    new V9VmOpenApiServiceChannelServiceInBase{ServiceId = _serviceId.ToString()}
                }
            };
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(_channelId, true))
                .Returns(new VmOpenApiServiceChannel { Id = _channelId, ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString() });
            serviceServiceMockSetup.Setup(s => s.CheckServices(_serviceIdList))
                .Returns(_serviceIdList);

            // Act
            var result = _controller.ASTIPutServiceAndChannelByChannel(_channelId.ToString(), vm);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(It.IsAny<Guid>(), true), Times.Once());
            serviceServiceMockSetup.Verify(x => x.CheckServices(It.IsAny<List<Guid>>()), Times.Once);
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceChannelConnections(It.IsAny<V11VmOpenApiChannelServicesIn>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_CanModifyConnections()
        {
            // Arrange
            var request = new V9VmOpenApiChannelServicesIn
            {
                ServiceRelations = new List<V9VmOpenApiServiceChannelServiceInBase>
                { new V9VmOpenApiServiceChannelServiceInBase
                {
                    ServiceId = _serviceId.ToString(),
                    ServiceChargeType = ServiceChargeTypeEnum.Charged.GetOpenApiValue(),
                    Description = _localizedItemList,
                    ExtraTypes = GetExtraTypes(),
                    ServiceHours = GetServiceHours(),
                    ContactDetails = GetContactDetails<V9VmOpenApiContactDetailsInBase>(),
                } }
            };
            codeServiceMockSetup.Setup(c => c.GetDialCode(TEXT)).Returns(new VmDialCode { Id = Guid.NewGuid() });
            channelServiceMockSetup.Setup(s => s.GetServiceChannelByIdSimple(_channelId, true))
                .Returns(new VmOpenApiServiceChannel { Id = _channelId, IsVisibleForAll = true, ServiceChannelType = ServiceChannelTypeEnum.ServiceLocation.ToString() });
            serviceServiceMockSetup.Setup(s => s.CheckServices(_serviceIdList))
                .Returns((List<Guid>)null);
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServiceChannelConnections(It.IsAny<V11VmOpenApiChannelServicesIn>(), It.IsAny<int>()))
                .Returns((V11VmOpenApiChannelServicesIn relations, int version) =>
                {
                    var channel = new V8VmOpenApiServiceLocationChannel();
                    if (relations.ServiceRelations?.Count > 0)
                    {
                        channel.Services = new List<V8VmOpenApiServiceChannelService>();
                        relations.ServiceRelations.ForEach(service =>
                        {
                            var model = new V8VmOpenApiServiceChannelService
                            {
                                Service = new VmOpenApiItem { Id = service.ServiceGuid },
                                ServiceChargeType = service.ServiceChargeType,
                                Description = service.Description,
                                ExtraTypes = service.ExtraTypes,
                                ContactDetails = GetContacDetailsResultList<V8VmOpenApiContactDetails>(service.ContactDetails),
                                IsASTIConnection = service.IsASTIConnection
                            };
                            service.ServiceHours?.ForEach(h => model.ServiceHours.Add(h.ConvertToPreviousVersion()));
                            channel.Services.Add(model);
                        });
                    }
                    return channel;
                });

            // Act
            var result = _controller.ASTIPutServiceAndChannelByChannel(_channelId.ToString(), request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var channelResult = Assert.IsType<V8VmOpenApiServiceLocationChannel>(okResult.Value);
            channelResult.Should().NotBeNull();
            channelResult.Services.Should().NotBeNull();
            channelResult.Services.Count().Should().Be(1);
            var connectionResult = channelResult.Services.First();
            connectionResult.Service.Id.Should().Be(_serviceId);
            CheckConnectionResult(connectionResult, true);
            CheckExtraTypesResult(connectionResult.ExtraTypes, true);
            CheckContactDetailsResult(connectionResult.ContactDetails);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(It.IsAny<Guid>(), true), Times.Once());
            serviceServiceMockSetup.Verify(x => x.CheckServices(It.IsAny<List<Guid>>()), Times.Once);
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceChannelConnections(It.IsAny<V11VmOpenApiChannelServicesIn>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_DeleteConnections()
        {
            // Arrange
            var request = new V9VmOpenApiChannelServicesIn
            {
                DeleteAllServiceRelations = true
            };
            channelServiceMockSetup.Setup(s => s.ChannelExists(_channelId)).Returns(true);
            serviceServiceMockSetup.Setup(s => s.CheckServices(_serviceIdList))
                .Returns((List<Guid>)null);
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServiceChannelConnections(It.IsAny<V11VmOpenApiChannelServicesIn>(), It.IsAny<int>()))
                .Returns(new VmOpenApiServiceChannel());

            // Act
            var result = _controller.ASTIPutServiceAndChannelByChannel(_channelId.ToString(), request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<VmOpenApiServiceChannel>(okResult.Value);
            channelServiceMockSetup.Verify(x => x.ChannelExists(It.IsAny<Guid>()), Times.Once());
            serviceServiceMockSetup.Verify(x => x.CheckServices(It.IsAny<List<Guid>>()), Times.Never);
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceChannelConnections(It.IsAny<V11VmOpenApiChannelServicesIn>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void ASTIPutServiceAndChannelByChannel_DuplicateItemsInRequest()
        {
            // Arrange
            var request = new V9VmOpenApiChannelServicesIn
            {
                ServiceRelations = new List<V9VmOpenApiServiceChannelServiceInBase>
                {
                    new V9VmOpenApiServiceChannelServiceInBase
                    {
                        ServiceId = _serviceId.ToString(),
                    },
                    new V9VmOpenApiServiceChannelServiceInBase
                    {
                        ServiceId = _serviceId.ToString(),
                    }
                }
            };

            // Act
            var result = _controller.ASTIPutServiceAndChannelByChannel(_channelId.ToString(), request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            channelServiceMockSetup.Verify(x => x.GetServiceChannelByIdSimple(It.IsAny<Guid>(), true), Times.Never);
            serviceServiceMockSetup.Verify(x => x.CheckServices(It.IsAny<List<Guid>>()), Times.Never);
            serviceAndChannelServiceMockSetup.Verify(x => x.SaveServiceChannelConnections(It.IsAny<V11VmOpenApiChannelServicesIn>(), defaultVersion), Times.Never);
        }
        #endregion

        private TModel GetContactDetails<TModel>() where TModel : IVmOpenApiContactDetailsInVersionBase, new()
        {
            return new TModel
            {
                Emails = _languageItemList,
                PhoneNumbers = new List<V4VmOpenApiPhone>{ new V4VmOpenApiPhone
                        { AdditionalInformation = TEXT, ServiceChargeType = ServiceChargeTypeEnum.Free.GetOpenApiValue(), ChargeDescription = TEXT, PrefixNumber = TEXT, Number = TEXT, Language = LANGUAGE} },
                Addresses = new List<V7VmOpenApiAddressContactIn>{ new V7VmOpenApiAddressContactIn
                        { Type = AddressConsts.POSTAL, SubType = AddressConsts.POSTOFFICEBOX, PostOfficeBoxAddress = new VmOpenApiAddressPostOfficeBoxIn{ PostOfficeBox = _languageItemList} } },
                WebPages = new List<V9VmOpenApiWebPage> { new V9VmOpenApiWebPage { Url = URL, Value = TEXT, Language = LANGUAGE } }

            };
        }

        private List<VmOpenApiExtraType> GetExtraTypes()
        {
            return new List<VmOpenApiExtraType> { new VmOpenApiExtraType
            {
                Type = ExtraTypeEnum.Asti.ToString(),
                Code = ExtraSubTypeEnum.DocumentReceived.ToString(),
                Description = _languageItemList
            } };
        }

        private List<V8VmOpenApiServiceHour> GetServiceHours()
        {
            return new List<V8VmOpenApiServiceHour>{ new V8VmOpenApiServiceHour
            {
                ServiceHourType = ServiceHoursTypeEnum.Standard.GetOpenApiValue(),
                ValidFrom = _from,
                ValidTo = _to,
                OpeningHour = new List<V8VmOpenApiDailyOpeningTime>{new V8VmOpenApiDailyOpeningTime { DayFrom = TEXT, DayTo = TEXT } },
                AdditionalInformation = _languageItemList
            } };
        }

        private TModel GetContacDetailsResultList<TModel>(V9VmOpenApiContactDetailsInBase detailsRequest) where TModel : IVmOpenApiContactDetailsVersionBase, new()
        {
            if (detailsRequest == null) return default(TModel);

            var result = new TModel
            {
                Emails = detailsRequest.Emails,
                WebPages = detailsRequest.WebPages,
            };

            if (detailsRequest.PhoneNumbers?.Count > 0)
            {
                result.PhoneNumbers = new List<V4VmOpenApiPhoneWithType>();
                detailsRequest.PhoneNumbers.ForEach(p => result.PhoneNumbers.Add(new V4VmOpenApiPhoneWithType
                {
                    Type = PhoneNumberTypeEnum.Phone.ToString(),
                    PrefixNumber = p.PrefixNumber,
                    Number = p.Number,
                    Language = p.Language,
                    IsFinnishServiceNumber = p.IsFinnishServiceNumber,
                    AdditionalInformation = p.AdditionalInformation,
                    ServiceChargeType = p.ServiceChargeType,
                    ChargeDescription = p.ChargeDescription,
                }));
            }

            if (detailsRequest.Addresses?.Count > 0)
            {
                result.Addresses = new List<V7VmOpenApiAddressContact>();
                detailsRequest.Addresses.ForEach(a =>
                {
                    var vmAddress = new V7VmOpenApiAddressContact { Type = a.Type, SubType = a.SubType, LocationAbroad = a.ForeignAddress, Country = a.Country };
                    if (a.StreetAddress != null)
                    {
                        var street = a.StreetAddress;
                        vmAddress.StreetAddress = new VmOpenApiAddressStreetWithCoordinates
                        {
                            Street = street.Street,
                            StreetNumber = street.StreetNumber,
                            PostalCode = street.PostalCode,
                            Latitude = street.Latitude,
                            Longitude = street.Longitude,
                            AdditionalInformation = street.AdditionalInformation
                        };
                        if (street.Municipality != null)
                        {
                            vmAddress.StreetAddress.Municipality = new VmOpenApiMunicipality { Code = street.Municipality };
                        }
                    }
                    if (a.PostOfficeBoxAddress != null)
                    {
                        vmAddress.PostOfficeBoxAddress = new VmOpenApiAddressPostOfficeBox
                        {
                            PostOfficeBox = a.PostOfficeBoxAddress.PostOfficeBox,
                            PostalCode = a.PostOfficeBoxAddress.PostalCode,
                            AdditionalInformation = a.PostOfficeBoxAddress.AdditionalInformation
                        };
                    }
                    result.Addresses.Add(vmAddress);
                });
            }

            return result;
        }

        private void MockSaveServiceConnections()
        {
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServiceConnections(It.IsAny<V11VmOpenApiServiceAndChannelRelationAstiInBase>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns((V11VmOpenApiServiceAndChannelRelationAstiInBase relations, int version, bool isASTI) =>
                {
                    var service = new V8VmOpenApiService();
                    if (relations.ChannelRelations?.Count > 0)
                    {
                        service.ServiceChannels = new List<V8VmOpenApiServiceServiceChannel>();
                        relations.ChannelRelations.ForEach(channel =>
                        {
                            var model = new V8VmOpenApiServiceServiceChannel
                            {
                                ServiceChannel = new VmOpenApiItem { Id = channel.ChannelGuid },
                                ServiceChargeType = channel.ServiceChargeType,
                                Description = channel.Description,
                                ExtraTypes = channel.ExtraTypes,
                                ContactDetails = GetContacDetailsResultList<V8VmOpenApiContactDetails>(channel.ContactDetails),
                                IsASTIConnection = channel.IsASTIConnection
                            };
                            channel.ServiceHours?.ForEach(h => model.ServiceHours.Add(h.ConvertToPreviousVersion()));
                            service.ServiceChannels.Add(model);
                        });
                    }
                    return service;
                });
        }

        private void MockSaveServiceConnectionsBySource(string serviceSourceId)
        {
            serviceAndChannelServiceMockSetup.Setup(s => s.SaveServiceConnectionsBySource(serviceSourceId, It.IsAny<V11VmOpenApiServiceAndChannelRelationBySourceAsti>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns((string sourceId, V11VmOpenApiServiceAndChannelRelationBySourceAsti relations, int version, bool isASTI) =>
                {
                    var service = new V8VmOpenApiService();
                    if (relations.ChannelRelations?.Count > 0)
                    {
                        service.ServiceChannels = new List<V8VmOpenApiServiceServiceChannel>();
                        relations.ChannelRelations.ForEach(channel =>
                        {
                            var channelResult = new V8VmOpenApiServiceServiceChannel
                            {
                                ServiceChannel = new VmOpenApiItem(),
                                ServiceChargeType = channel.ServiceChargeType,
                                IsASTIConnection = channel.IsASTIConnection,
                                Description = channel.Description,
                                ExtraTypes = channel.ExtraTypes,
                                ContactDetails = GetContacDetailsResultList<V8VmOpenApiContactDetails>(channel.ContactDetails)
                            };
                            channel.ServiceHours?.ForEach(h => channelResult.ServiceHours.Add(h.ConvertToPreviousVersion()));
                            service.ServiceChannels.Add(channelResult);
                        });
                    }
                    return service;
                });
        }

        private void CheckServiceResult(IActionResult result, Guid? channelId, bool isAsti)
        {
            var okResult = Assert.IsType<OkObjectResult>(result);
            var serviceResult = Assert.IsType<V8VmOpenApiService>(okResult.Value);
            serviceResult.Should().NotBeNull();
            serviceResult.ServiceChannels.Should().NotBeNull();
            serviceResult.ServiceChannels.Count().Should().Be(1);
            var connectionResult = serviceResult.ServiceChannels.First();
            connectionResult.ServiceChannel.Id.Should().Be(channelId);
            CheckConnectionResult(connectionResult, isAsti);
            CheckExtraTypesResult(connectionResult.ExtraTypes, isAsti);
            CheckContactDetailsResult(connectionResult.ContactDetails);
        }

        private void CheckConnectionResult(V8VmOpenApiServiceChannelService result, bool isAsti)
        {
            CheckConnectionResultBase(result, isAsti);
            var serviceHour = result.ServiceHours.First();
            serviceHour.ValidFrom.Should().Be(_from);
            serviceHour.ValidTo.Should().Be(_to);
            serviceHour.AdditionalInformation.First().Value.Should().Be(TEXT);
        }
        private void CheckConnectionResult(V8VmOpenApiServiceServiceChannel result, bool isAsti)
        {
            CheckConnectionResultBase(result, isAsti);
            var serviceHour = result.ServiceHours.First();
            serviceHour.ValidFrom.Should().Be(_from);
            serviceHour.ValidTo.Should().Be(_to);
            serviceHour.AdditionalInformation.First().Value.Should().Be(TEXT);
        }
        private void CheckConnectionResultBase(IVmOpenApiServiceServiceChannelBase result, bool isAsti)
        {
            result.IsASTIConnection.Should().Be(isAsti);
            result.ServiceChargeType.Should().Be(ServiceChargeTypeEnum.Charged.GetOpenApiValue());
            result.Description.First().Value.Should().Be(TEXT);
        }

        private void CheckExtraTypesResult(IList<VmOpenApiExtraType> result, bool isAsti)
        {
            if (isAsti)
            {
                result.Count().Should().Be(1);
                var extraType = result.First();
                extraType.Type.Should().Be(ExtraTypeEnum.Asti.ToString());
                extraType.Code.Should().Be(ExtraSubTypeEnum.DocumentReceived.ToString());
                extraType.Description.Should().BeSameAs(_languageItemList);
            }
            else
            {
                result.Count().Should().Be(0);
            }
        }

        private void CheckContactDetailsResult(IVmOpenApiContactDetailsVersionBase details)
        {
            details.Emails.Should().BeSameAs(_languageItemList);
            details.PhoneNumbers.Count().Should().Be(1);
            details.PhoneNumbers.First().Number.Should().Be(TEXT);
            details.WebPages.Count().Should().Be(1);
            details.WebPages.First().Url.Should().Be(URL);
            details.Addresses.Count().Should().Be(1);
            var address = details.Addresses.First();
            address.Type.Should().Be(AddressConsts.POSTAL);
            address.SubType.Should().Be(AddressConsts.POSTOFFICEBOX);
            address.PostOfficeBoxAddress.PostOfficeBox.Should().BeSameAs(_languageItemList);
        }
    }
}
