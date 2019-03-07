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

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Database.Model.Views;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Tasks
{
    public class GetExpirationTime : TestBase
    {
        private TasksService tasksService;
        
        private void Init(VersionInfo lastPublishedVersion, List<VTasksConfiguration> taskConfigurations, List<ServiceVersioned> repository)
        {
            var versioningManagerMock = new Mock<IVersioningManager>();
            versioningManagerMock
                .Setup(m => m.GetLastPublishedVersion<ServiceVersioned>(It.IsAny<IUnitOfWork>(), It.IsAny<Guid>()))
                .Returns(lastPublishedVersion);
            
            RegisterRepository<IRepository<ServiceVersioned>, ServiceVersioned>(repository.AsQueryable());
            RegisterViewRepository<IVTasksConfigurationRepository, VTasksConfiguration>(taskConfigurations.AsQueryable());

            tasksService = new TasksService(null,
                null,
                CacheManager.PublishingStatusCache,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                versioningManagerMock.Object,
                null,
                null,
                null,
                CacheManager);
        }
        
        [Fact]
        public void OtherPublishedVersionExists()
        {
            var lastPublishedVersion = new VersionInfo();
            var serviceVersioned = new ServiceVersioned
            {
                Id = "serviceVersioned".GetGuid(),
                PublishingStatusId = PublishingStatus.Modified.ToString().GetGuid()
            };
            Init(lastPublishedVersion, new List<VTasksConfiguration>(), new List<ServiceVersioned>{ serviceVersioned });
            
            var result = tasksService.GetExpirationTime<ServiceVersioned>(unitOfWorkMockSetup.Object, serviceVersioned.Id);
            
            Assert.Null(result);
        }

        [Fact]
        public void ConfigurationDoesNotExist()
        {
            var serviceVersioned = new ServiceVersioned
            {
                Id = "serviceVersioned".GetGuid(),
                PublishingStatusId = PublishingStatus.Draft.ToString().GetGuid()
            };
            Init(null, new List<VTasksConfiguration>(), new List<ServiceVersioned>{ serviceVersioned });

            var result = tasksService.GetExpirationTime<ServiceVersioned>(unitOfWorkMockSetup.Object, serviceVersioned.Id);

            Assert.Null(result);
        }

        [Fact]
        public void CorrectTimeIsCalculatedForDraft()
        {
            var draftId = PublishingStatus.Draft.ToString().GetGuid();
            var interval = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var modified = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = new DateTime(2018, 1, 12, 0, 0, 0, DateTimeKind.Utc);
            var expectedResult = now.Add(modified - interval);

            VTasksConfiguration draftConfiguration = new VTasksConfiguration();
            SetTasksConfiguration(draftConfiguration, nameof(draftConfiguration.Entity), typeof(Model.Models.Service).Name);
            SetTasksConfiguration(draftConfiguration, nameof(draftConfiguration.PublishingStatusId), draftId);
            SetTasksConfiguration(draftConfiguration, nameof(draftConfiguration.Interval), interval);
            
            var tasksConfigurations = new List<VTasksConfiguration>
            {
                draftConfiguration,
            };
            var serviceVersioned = new ServiceVersioned
            {
                Id = "serviceVersioned".GetGuid(),
                PublishingStatusId = draftId,
                Modified = modified
            };
            Init(null, tasksConfigurations, new List<ServiceVersioned>{ serviceVersioned });

            var result = tasksService.GetExpirationTime<ServiceVersioned>(unitOfWorkMockSetup.Object, serviceVersioned.Id, now);

            Assert.Equal(expectedResult, result);
        }
        
        [Fact]
        public void CorrectTimeIsCalculatedForModified()
        {
            var draftId = PublishingStatus.Draft.ToString().GetGuid();
            var modifiedId = PublishingStatus.Modified.ToString().GetGuid();
            
            var draftInterval = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var modifiedInterval = new DateTime(2016, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var modified = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = new DateTime(2018, 1, 12, 0, 0, 0, DateTimeKind.Utc);
            var expectedResult = now.Add(modified - draftInterval);

            VTasksConfiguration draftConfiguration = new VTasksConfiguration();
            SetTasksConfiguration(draftConfiguration, nameof(VTasksConfiguration.Entity), typeof(Model.Models.Service).Name);
            SetTasksConfiguration(draftConfiguration, nameof(VTasksConfiguration.PublishingStatusId), draftId);
            SetTasksConfiguration(draftConfiguration, nameof(VTasksConfiguration.Interval), draftInterval);
            
            VTasksConfiguration modifiedConfiguration = new VTasksConfiguration();
            SetTasksConfiguration(modifiedConfiguration, nameof(VTasksConfiguration.Entity), typeof(Model.Models.Service).Name);
            SetTasksConfiguration(modifiedConfiguration, nameof(VTasksConfiguration.PublishingStatusId), modifiedId);
            SetTasksConfiguration(modifiedConfiguration, nameof(VTasksConfiguration.Interval), modifiedInterval);
            
            var tasksConfigurations = new List<VTasksConfiguration>
            {
                draftConfiguration, modifiedConfiguration
            };
            var serviceVersioned = new ServiceVersioned
            {
                Id = "serviceVersioned".GetGuid(),
                PublishingStatusId = modifiedId,
                Modified = modified
            };
            Init(null, tasksConfigurations, new List<ServiceVersioned>{ serviceVersioned });

            var result = tasksService.GetExpirationTime<ServiceVersioned>(unitOfWorkMockSetup.Object, serviceVersioned.Id, now);

            Assert.Equal(expectedResult, result);
        }
    }
}