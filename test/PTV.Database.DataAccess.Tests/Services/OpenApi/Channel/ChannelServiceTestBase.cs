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

using Microsoft.Extensions.Logging;
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public abstract class ChannelServiceTestBase : ServiceTestBase
    {
        internal ILogger<ChannelService> Logger { get; private set; }
        internal ServiceChannelLogic ServiceChannelLogic { get; private set; }
        internal IUrlService UrlService { get; private set; }
        internal Mock<IServiceChannelVersionedRepository> ChannelRepoMock { get; private set; }

        public ChannelServiceTestBase()
        {
            Logger = new Mock<ILogger<ChannelService>>().Object;
            ServiceChannelLogic = new ServiceChannelLogic(new ChannelAttachmentLogic(), new WebPageLogic(), new OpeningHoursLogic(), AddressLogic);
            UrlService = (new Mock<IUrlService>()).Object;
            ChannelRepoMock = new Mock<IServiceChannelVersionedRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelVersionedRepository>()).Returns(ChannelRepoMock.Object);
            var serviceNameRepoMock = new Mock<IServiceNameRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceNameRepository>()).Returns(serviceNameRepoMock.Object);
        }

        internal void ArrangeTranslationManager(ServiceChannelTypeEnum channelType)
        {
            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny< ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType != ServiceChannelTypeEnum.EChannel) return new VmOpenApiElectronicChannelVersionBase();
                    return new VmOpenApiElectronicChannelVersionBase()
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                });

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType != ServiceChannelTypeEnum.Phone) return new VmOpenApiPhoneChannelVersionBase();
                    return new VmOpenApiPhoneChannelVersionBase()
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                });

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType != ServiceChannelTypeEnum.ServiceLocation) return new VmOpenApiServiceLocationChannelVersionBase();
                    return new VmOpenApiServiceLocationChannelVersionBase()
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                });

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType != ServiceChannelTypeEnum.PrintableForm) return new VmOpenApiPrintableFormChannelVersionBase();
                    return new VmOpenApiPrintableFormChannelVersionBase()
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                });

            translationManagerMockSetup.Setup(t => t.Translate<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()))
                .Returns((ServiceChannelVersioned entity) =>
                {
                    if (channelType != ServiceChannelTypeEnum.WebPage) return new VmOpenApiWebPageChannelVersionBase();
                    return new VmOpenApiWebPageChannelVersionBase()
                    {
                        Id = entity.UnificRootId,
                        PublishingStatus = PublishingStatusCache.GetByValue(entity.PublishingStatusId)
                    };
                });
        }
    }
}
