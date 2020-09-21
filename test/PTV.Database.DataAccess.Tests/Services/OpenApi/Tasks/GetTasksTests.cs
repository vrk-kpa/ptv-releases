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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Database.Model.Views;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Enums;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Tasks
{
    public class GetTasksTests : ServiceTestBase
    {
        private Mock<IServiceService> _serviceServiceMock;
        private Mock<IChannelService> _channelServiceMock;
        private TasksService _service;
        private Mock<IVTasksConfigurationRepository> taskRepoMock;
        private Mock<IServiceServiceChannelRepository> connectionRepoMock;

        public GetTasksTests()
        {
            _serviceServiceMock = new Mock<IServiceService>();
            _channelServiceMock = new Mock<IChannelService>();

            var unitOfWorkMock = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);
            ExpirationTimeCacheMock.Setup(x => x.GetExpirationTimes(It.IsAny<Type>(), It.IsAny<Guid>()))
                .Returns(null as (DateTime, DateTime)?);
            ExpirationServiceMock.Setup(x => x
                    .GetExpirationTasks<ServiceChannelVersioned, ServiceChannelLanguageAvailability>
                    (It.IsAny<IUnitOfWork>(), It.IsAny<Guid>(), It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>(),
                        It.IsAny<int?>()))
                .Returns(new List<VmTaskEntity>());
            ExpirationServiceMock.Setup(x => x
                    .GetExpirationTasks<ServiceVersioned, ServiceLanguageAvailability>
                    (It.IsAny<IUnitOfWork>(), It.IsAny<Guid>(), It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>(),
                        It.IsAny<int?>()))
                .Returns(new List<VmTaskEntity>());

            _service = new TasksService(
                translationManagerMockSetup.Object,
                TranslationManagerVModel,
                PublishingStatusCache,
                UserOrganizationChecker,
                contextManager,
                _serviceServiceMock.Object,
                _channelServiceMock.Object,
                CommonService,
                null,
                serviceUtilities,
                null,
                null,
                null,
                null,
                ExpirationServiceMock.Object,
                ExpirationTimeCacheMock.Object,
                CacheManager);

            taskRepoMock = new Mock<IVTasksConfigurationRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IVTasksConfigurationRepository>()).Returns(taskRepoMock.Object);
            taskRepoMock.Setup(x => x.All()).Returns(new List<VTasksConfiguration> { new VTasksConfiguration() }.AsQueryable());

            connectionRepoMock = new Mock<IServiceServiceChannelRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceServiceChannelRepository>()).Returns(connectionRepoMock.Object);
        }

        [Theory]
        [InlineData(TasksIdsEnum.OutdatedDraftServices)]
        [InlineData(TasksIdsEnum.OutdatedPublishedServices)]
        [InlineData(TasksIdsEnum.OutdatedDraftChannels)]
        [InlineData(TasksIdsEnum.OutdatedPublishedChannels)]
        public void CanGetTasks(TasksIdsEnum taskId)
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 1;
            IVmOpenApiModelWithPagingBase<VmOpenApiExpiringTask> vm = new VmOpenApiExpiringTasks();
            _serviceServiceMock.Setup(x => x.GetTaskServices(pageNumber, pageSize, It.IsAny<List<Guid>>(), It.IsAny<DateTime>(), It.IsAny<List<Guid>>())).Returns(vm);
            _channelServiceMock.Setup(x => x.GetTaskServiceChannels(pageNumber, pageSize, It.IsAny<List<Guid>>(), It.IsAny<DateTime>(), It.IsAny<List<Guid>>())).Returns(vm);

            // Act
            var result = _service.GetTasks(taskId, pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void CanGetOrphanServicesTasks()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 1;
            Guid userOrganizationId = Guid.NewGuid();
            var entityList = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            entityList.ForEach(e => e.OrganizationId = userOrganizationId);

            UserOrganizationServiceMock.Setup(x => x.GetAllUserOrganizationIds(It.IsAny<List<PublishingStatus>>())).Returns(new List<Guid> { userOrganizationId });

            Mock<IRepository<ServiceVersioned>> repoMock = new Mock<IRepository<ServiceVersioned>>();
            repoMock.Setup(x => x.All()).Returns(entityList.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceVersioned>>()).Returns(repoMock.Object);

            _serviceServiceMock.Setup(x => x.GetTaskServices(pageNumber, pageSize, It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>()))
                .Returns((int pn, int ps, List<Guid> ids, List<Guid> statusIds) =>
                {
                    IVmOpenApiModelWithPagingBase<VmOpenApiTask> vm = new VmOpenApiTasks { PageNumber = pn, PageSize = ps };
                    if (ids?.Count > 0)
                    {
                        vm.ItemList = new List<VmOpenApiTask>();
                        ids.ForEach(i => vm.ItemList.Add(new VmOpenApiTask { Id = i }));
                    }
                    return vm;
                });

            // Act
            var result = _service.GetOrphanItemsTasks(TasksIdsEnum.ServicesWithoutChannels, pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.ItemList.Should().NotBeNull();
            result.ItemList.Count().Should().Be(3);
        }

        [Fact]
        public void CanGetOrphanChannelsTasks()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 1;
            Guid userOrganizationId = Guid.NewGuid();
            var entityList = EntityGenerator.GetServiceChannelEntityList(1, PublishingStatusCache);
            entityList.ForEach(e => e.OrganizationId = userOrganizationId);

            UserOrganizationServiceMock.Setup(x => x.GetAllUserOrganizationIds(It.IsAny<List<PublishingStatus>>())).Returns(new List<Guid> { userOrganizationId });

            Mock<IRepository<ServiceChannelVersioned>> repoMock = new Mock<IRepository<ServiceChannelVersioned>>();
            repoMock.Setup(x => x.All()).Returns(entityList.AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<ServiceChannelVersioned>>()).Returns(repoMock.Object);

            _channelServiceMock.Setup(x => x.GetTaskServiceChannels(pageNumber, pageSize, It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>()))
                .Returns((int pn, int ps, List<Guid> ids, List<Guid> statusIds) =>
                {
                    IVmOpenApiModelWithPagingBase<VmOpenApiTask> vm = new VmOpenApiTasks { PageNumber = pn, PageSize = ps };
                    if (ids?.Count > 0)
                    {
                        vm.ItemList = new List<VmOpenApiTask>();
                        ids.ForEach(i => vm.ItemList.Add(new VmOpenApiTask { Id = i }));
                    }
                    return vm;
                });

            // Act
            var result = _service.GetOrphanItemsTasks(TasksIdsEnum.ChannelsWithoutServices, pageNumber, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.ItemList.Should().NotBeNull();
            result.ItemList.Count().Should().Be(3);
        }
    }
}
