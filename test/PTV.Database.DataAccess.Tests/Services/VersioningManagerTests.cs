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
using System.Threading.Tasks;
using Xunit;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models;
using Microsoft.Extensions.Logging;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Domain.Logic;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Services
{
    public class VersioningManagerTests : TranslatorTestBase
    {
//        private Mock<IUnitOfWorkWritable> unitOfWorkMockSetup;
//        private Mock<IServiceRepository> serviceRepositoryMockSetup;
//        private ILogger<ChannelService> logger;
//        private Mock<IVersioningRepository> versioningRepositoryMockSetup;
//        private Mock<IUserIdentification> userIdentificationMockSetup;
//        private TestContextManager contextManager;
//        private Mock<ICacheManager> cacheManagerMockSetup;
//        private Mock<IPublishingStatusCache> publishingStatusCache;
//        private ICacheManager cacheManagerMock;
//        private List<Service> servicesDbSet;
//        private List<Versioning> versioningDbSet;
//        private IUnitOfWorkWritable unitOfWorkMock;
//
//        public VersioningManagerTests()
//        {
//            var service = new Service() { Id = Guid.NewGuid() };
//            servicesDbSet = new List<Service>() { service };
//            versioningDbSet = new List<Versioning>();
//            unitOfWorkMockSetup = new Mock<IUnitOfWorkWritable>();
//
//            publishingStatusCache = new Mock<IPublishingStatusCache>();
//            publishingStatusCache.Setup(i => i.Get(It.IsAny<PublishingStatus>())).Returns<PublishingStatus>(j => j.ToString().GetGuid());
//            publishingStatusCache.Setup(i => i.Get(It.IsAny<string>())).Returns<string>(j => j.GetGuid());
//            CacheManagerMock.Setup(i => i.PublishingStatusCache).Returns(publishingStatusCache.Object);
//            serviceRepositoryMockSetup = new Mock<IServiceRepository>();
//            serviceRepositoryMockSetup.Setup(i => i.All()).Returns(servicesDbSet.AsQueryable());
//            serviceRepositoryMockSetup.Setup(i => i.Add(It.IsAny<Service>())).Returns<Service>(i => { servicesDbSet.Add(i);return i;});
//            versioningRepositoryMockSetup = new Mock<IVersioningRepository>();
//            versioningRepositoryMockSetup.Setup(i => i.All()).Returns(versioningDbSet.AsQueryable());
//            versioningRepositoryMockSetup.Setup(i => i.Add(It.IsAny<Versioning>())).Returns<Versioning>(i => { versioningDbSet.Add(i); return i; });
//            logger = new Mock<ILogger<ChannelService>>().Object;
//            userIdentificationMockSetup = new Mock<IUserIdentification>();
//
//            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceRepository>()).Returns(serviceRepositoryMockSetup.Object);
//            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IVersioningRepository>()).Returns(versioningRepositoryMockSetup.Object);
//            userIdentificationMockSetup.Setup(ui => ui.UserName).Returns("TestUser");
//            unitOfWorkMock = unitOfWorkMockSetup.Object;
//            contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);
//
//            RegisterDbSet(servicesDbSet, unitOfWorkMockSetup);
//            RegisterDbSet(versioningDbSet, unitOfWorkMockSetup);
//        }
//
//        [Fact]
//        public void ServiceVersioningFirstVersionTest()
//        {
            //var verManager = new VersioningManager(cacheManager);
//            var service = servicesDbSet.First();
//            verManager.CreateNewVersion(unitOfWorkMock, service);
//            Assert.Equal(1, versioningDbSet.Count);
//            var version = versioningDbSet.First();
//            Assert.Equal(1, version.VersionMajor);
//            Assert.Equal(0, version.VersionMinor);
//            Assert.Equal(PublishingStatus.Draft.ToString().GetGuid(), service.PublishingStatusId);
//            Assert.NotNull(service.Versioning);
//            Assert.Null(service.Versioning.PreviousVersionId);
//            Assert.Null(service.Versioning.PreviousVersion);
//        }
//
//        [Fact]
//        public void ServiceVersioningCreateNextVersionTest()
//        {
            //var verManager = new VersioningManager(CacheManager);
//            var service = servicesDbSet.First();
//            verManager.CreateNewVersion(unitOfWorkMock, service);
//            verManager.CreateNewVersion(unitOfWorkMock, service);
//            Assert.Equal(2, versioningDbSet.Count);
//            var version = versioningDbSet.Last();
//            Assert.Equal(1, version.VersionMajor);
//            Assert.Equal(1, version.VersionMinor);
//            Assert.Equal(PublishingStatus.Modified.ToString().GetGuid(), service.PublishingStatusId);
//            Assert.NotNull(service.Versioning);
//            Assert.NotNull(service.Versioning.PreviousVersionId);
//        }
//
//
//        [Fact]
//        public void ServiceVersioningPublishVersionTest()
//        {
            //var verManager = new VersioningManager(CacheManager);
//            var service = servicesDbSet.First();
//            verManager.CreateNewVersion(unitOfWorkMock, service);
//            verManager.CreateNewVersion(unitOfWorkMock, service);
//            verManager.PublishVersion(unitOfWorkMock, service);
//            Assert.Equal(3, versioningDbSet.Count);
//            var version = versioningDbSet.Last();
//            Assert.Equal(2, version.VersionMajor);
//            Assert.Equal(0, version.VersionMinor);
//            Assert.Equal(PublishingStatus.Published.ToString().GetGuid(), service.PublishingStatusId);
//            Assert.NotNull(service.Versioning);
//            Assert.NotNull(service.Versioning.PreviousVersionId);
//        }
    }
}
