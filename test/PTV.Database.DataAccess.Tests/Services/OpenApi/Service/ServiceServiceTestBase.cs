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

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class ServiceServiceTestBase : ServiceTestBase
    {
        internal ILogger<DataAccess.Services.ServiceService> Logger { get; private set; }

        internal IGeneralDescriptionService gdService { get; private set; }

        internal Mock<IGeneralDescriptionService> gdServiceMock { get; private set; }

        internal Mock<IServiceVersionedRepository> ServiceRepoMock { get; private set; }

        internal Mock<IServiceNameRepository> ServiceNameRepoMock { get; private set; }

        internal Mock<IServiceChannelVersionedRepository> ServiceChannelRepoMock { get; private set; }

        internal Mock<IServiceChannelNameRepository> ServiceChannelNameRepoMock { get; private set; }

        internal Mock<IOrganizationNameRepository> OrganizationNameRepoMock { get; private set; }

        internal Mock<IKeywordRepository> KeywordRepoMock { get; private set; }

        internal Mock<ITargetGroupRepository> TargetGroupRepoMock { get; private set; }

        internal Mock<IServiceCollectionServiceRepository> ServiceCollectionRepoMock { get; private set; }

        public ServiceServiceTestBase()
        {
            Logger = new Mock<ILogger<ServiceService>>().Object;

            gdServiceMock = new Mock<IGeneralDescriptionService>();
            gdService = gdServiceMock.Object;

            ServiceRepoMock = new Mock<IServiceVersionedRepository>();
            ServiceNameRepoMock = new Mock<IServiceNameRepository>();
            ServiceChannelRepoMock = new Mock<IServiceChannelVersionedRepository>();
            ServiceChannelNameRepoMock = new Mock<IServiceChannelNameRepository>();
            OrganizationNameRepoMock = new Mock<IOrganizationNameRepository>();
            KeywordRepoMock = new Mock<IKeywordRepository>();
            TargetGroupRepoMock = new Mock<ITargetGroupRepository>();
            ServiceCollectionRepoMock = new Mock<IServiceCollectionServiceRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceVersionedRepository>()).Returns(ServiceRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceVersioned>>()).Returns(ServiceRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceNameRepository>()).Returns(ServiceNameRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelVersionedRepository>()).Returns(ServiceChannelRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelNameRepository>()).Returns(ServiceChannelNameRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationNameRepository>()).Returns(OrganizationNameRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IExternalSourceRepository>()).Returns(ExternalSourceRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IKeywordRepository>()).Returns(KeywordRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<ITargetGroupRepository>()).Returns(TargetGroupRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceCollectionServiceRepository>()).Returns(ServiceCollectionRepoMock.Object);
        }
    }
}
