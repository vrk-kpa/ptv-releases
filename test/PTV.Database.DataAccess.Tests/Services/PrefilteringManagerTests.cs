﻿/**
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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Filters;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Attributes;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services
{
    public class PrefilteringManagerTests : TranslatorTestBase
    {
        private Mock<IServiceVersionedRepository> serviceRepositoryMockSetup;
        private Mock<ILockingRepository> lockingRepositoryMockSetup;
        private Mock<IUserIdentification> userIdentificationMockSetup;
        private Mock<IPublishingStatusCache> publishingStatusCache;
        private List<ServiceVersioned> servicesDbSet;
        private List<Locking> lockingDbSet;

        public PrefilteringManagerTests()
        {
            var service = new ServiceVersioned() {Id = Guid.NewGuid()};
            servicesDbSet = new List<ServiceVersioned>() {service};
            lockingDbSet = new List<Locking>();
            unitOfWorkMockSetup = new Mock<IUnitOfWorkWritable>();
            publishingStatusCache = new Mock<IPublishingStatusCache>();
            publishingStatusCache.Setup(i => i.Get(It.IsAny<PublishingStatus>())).Returns<PublishingStatus>(j => j.ToString().GetGuid());
            publishingStatusCache.Setup(i => i.Get(It.IsAny<string>())).Returns<string>(j => j.GetGuid());
            CacheManagerMock.Setup(i => i.PublishingStatusCache).Returns(publishingStatusCache.Object);
            serviceRepositoryMockSetup = new Mock<IServiceVersionedRepository>();
            serviceRepositoryMockSetup.Setup(i => i.All()).Returns(servicesDbSet.AsQueryable());
            serviceRepositoryMockSetup.Setup(i => i.Add(It.IsAny<ServiceVersioned>())).Returns<ServiceVersioned>(i =>
            {
                servicesDbSet.Add(i);
                return i;
            });
            lockingRepositoryMockSetup = new Mock<ILockingRepository>();
            lockingRepositoryMockSetup.Setup(i => i.All()).Returns(lockingDbSet.AsQueryable());
            lockingRepositoryMockSetup.Setup(i => i.Add(It.IsAny<Locking>())).Returns<Locking>(i =>
            {
                lockingDbSet.Add(i);
                return i;
            });
            lockingRepositoryMockSetup.Setup(i => i.Remove(It.IsAny<Locking>())).Callback<Locking>(i => { lockingDbSet.Remove(i); });
            userIdentificationMockSetup = new Mock<IUserIdentification>();

            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceVersionedRepository>()).Returns(serviceRepositoryMockSetup.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<ILockingRepository>()).Returns(lockingRepositoryMockSetup.Object);
            userIdentificationMockSetup.Setup(ui => ui.UserName).Returns("TestUser");

            RegisterDbSet(servicesDbSet, unitOfWorkMockSetup);
            RegisterDbSet(lockingDbSet, unitOfWorkMockSetup);
        }

        [Fact]
        public void PrefilteringServiceTest()
        {
            // THIS FILTER IS DISABLED FOR NOW
//            ResolveManager.RegisterInstance(new TestRegisterServiceInfo() { RegisterAs = typeof(IQueryFilter), Instance = new PublishingStatusFilter(CacheManager.PublishingStatusCache) });
//            var prefilteringManager = new PrefilteringManager(ResolveManager);
//            var service = servicesDbSet.First();
//            service.PublishingStatusId = PublishingStatus.Published.ToString().GetGuid();
//            IQueryable<ServiceVersioned> query = servicesDbSet.AsQueryable();
//            var result = prefilteringManager.ApplyFilters(query).ToList();
//            Assert.Equal(1, result.Count);
//            service.PublishingStatusId = PublishingStatus.Deleted.ToString().GetGuid();
//            result = prefilteringManager.ApplyFilters(query).ToList();
//            Assert.Equal(0, result.Count);
        }
    }
}
