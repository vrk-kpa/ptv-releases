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

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using PTV.Database.DataAccess.Caches;
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

namespace PTV.Database.DataAccess.Tests.Services.Expiration
{
    public class GetExpirationTime : TestBase
    {
        private ExpirationService expirationService;

        private void Init(VersionInfo lastPublishedVersion, List<VTasksConfiguration> taskConfigurations, List<ServiceVersioned> repository)
        {
            var versioningManagerMock = new Mock<IVersioningManager>();
            versioningManagerMock
                .Setup(m => m.GetLastPublishedVersion<ServiceVersioned>(It.IsAny<IUnitOfWork>(), It.IsAny<Guid>()))
                .Returns(lastPublishedVersion);

            RegisterRepository<IRepository<ServiceVersioned>, ServiceVersioned>(repository.AsQueryable());
            RegisterViewRepository<IVTasksConfigurationRepository, VTasksConfiguration>(taskConfigurations.AsQueryable());

            expirationService = new ExpirationService(
                null,
                null,
                PublishingStatusCache,
                null,
                versioningManagerMock.Object,
                null,
                null,
                CacheManager,
                new ExpirationTimeCache(contextManagerMock.Object));
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

            var result = expirationService.GetExpirationDate<ServiceVersioned>(unitOfWorkMockSetup.Object, serviceVersioned.Id);

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

            var result = expirationService.GetExpirationDate<ServiceVersioned>(unitOfWorkMockSetup.Object, serviceVersioned.Id);

            Assert.Null(result);
        }

        [Fact]
        public void CorrectTimeIsCalculatedForDraft()
        {
            var draftId = PublishingStatus.Draft.ToString().GetGuid();
            var modified = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = new DateTime(2018, 1, 12, 0, 0, 0, DateTimeKind.Utc);
            var expirationMonths = 18;
            var expectedResult = now.AddMonths(expirationMonths);

            VTasksConfiguration draftConfiguration = new VTasksConfiguration();
            SetTasksConfiguration(draftConfiguration, nameof(draftConfiguration.Entity), typeof(Model.Models.Service).Name);
            SetTasksConfiguration(draftConfiguration, nameof(draftConfiguration.PublishingStatusId), draftId);
            SetTasksConfiguration(draftConfiguration, nameof(draftConfiguration.Months), (decimal)expirationMonths);

            var tasksConfigurations = new List<VTasksConfiguration>
            {
                draftConfiguration,
                draftConfiguration,
            };
            var serviceVersioned = new ServiceVersioned
            {
                Id = "serviceVersioned".GetGuid(),
                PublishingStatusId = draftId,
                Modified = modified
            };
            Init(null, tasksConfigurations, new List<ServiceVersioned>{ serviceVersioned });

            var result = expirationService.GetExpirationDate<ServiceVersioned>(unitOfWorkMockSetup.Object, serviceVersioned.Id, now);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void CorrectTimeIsCalculatedForModified()
        {
            var draftId = PublishingStatus.Draft.ToString().GetGuid();
            var modifiedId = PublishingStatus.Modified.ToString().GetGuid();

            var modified = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = new DateTime(2018, 1, 12, 0, 0, 0, DateTimeKind.Utc);
            var expirationMonths = 18;
            var expectedResult = now.AddMonths(expirationMonths);

            VTasksConfiguration draftConfiguration = new VTasksConfiguration();
            SetTasksConfiguration(draftConfiguration, nameof(VTasksConfiguration.Entity), typeof(Model.Models.Service).Name);
            SetTasksConfiguration(draftConfiguration, nameof(VTasksConfiguration.PublishingStatusId), draftId);
            SetTasksConfiguration(draftConfiguration, nameof(VTasksConfiguration.Months), (decimal)expirationMonths);

            VTasksConfiguration modifiedConfiguration = new VTasksConfiguration();
            SetTasksConfiguration(modifiedConfiguration, nameof(VTasksConfiguration.Entity), typeof(Model.Models.Service).Name);
            SetTasksConfiguration(modifiedConfiguration, nameof(VTasksConfiguration.PublishingStatusId), modifiedId);
            SetTasksConfiguration(modifiedConfiguration, nameof(VTasksConfiguration.Months),(decimal) expirationMonths);

            var tasksConfigurations = new List<VTasksConfiguration>
            {
                draftConfiguration,
                draftConfiguration,
                modifiedConfiguration,
                modifiedConfiguration,
            };
            var serviceVersioned = new ServiceVersioned
            {
                Id = "serviceVersioned".GetGuid(),
                PublishingStatusId = modifiedId,
                Modified = modified
            };
            Init(null, tasksConfigurations, new List<ServiceVersioned>{ serviceVersioned });

            var result = expirationService.GetExpirationDate<ServiceVersioned>(unitOfWorkMockSetup.Object, serviceVersioned.Id, now);

            Assert.Equal(expectedResult, result);
        }
    }
}
