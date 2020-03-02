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
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Common
{
    public class PublishEntityTest : TestBase
    {
        [Fact]
        public void LastFailedPublishAtIsRemovedForAffectedEntities()
        {
            var versionings = CreateVersionings().ToList();
            var oldServices = CreateOldServices(versionings).ToList();

            RegisterDbSet(versionings, unitOfWorkMockSetup);
            RegisterDbSet(oldServices, unitOfWorkMockSetup);
            RegisterRepository<IServiceVersionedRepository, ServiceVersioned>(oldServices.AsQueryable());
            RegisterRepository<IVersioningRepository, Versioning>(new List<Versioning>().AsQueryable());

            var typesCacheMock = new Mock<ITypesCache>();
            typesCacheMock.Setup(x => x.Get<PublishingStatusType>(It.IsAny<string>()))
                .Returns<string>(x => x.GetGuid());
            CacheManagerMock.Setup(x => x.TypesCache).Returns(typesCacheMock.Object);

            var versioningManager = new DataAccess.Services.VersioningManager(CacheManager, CloningManager, new NullLogger<DataAccess.Services.VersioningManager>());

            var commonService = new CommonService(
                null,
                null,
                null,
                typesCacheMock.Object,
                null,
                null,
                null,
                null,
                versioningManager,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            var publishingModel = new VmPublishingModel
            {
                Id = (ServiceName + 2).GetGuid(),
                LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo>
                {
                    new VmLanguageAvailabilityInfo
                    {
                        StatusId = PublishingStatus.Published.ToString().GetGuid()
                    }
                }
            };

            var result = commonService.PublishEntity<ServiceVersioned, ServiceLanguageAvailability>(
                unitOfWorkMockSetup.Object,
                publishingModel,
                HistoryAction.Publish);

            var firstOldService = oldServices[0];
            var secondOldService = oldServices[1];

            Assert.Null(firstOldService.LanguageAvailabilities.First().LastFailedPublishAt);
            Assert.Null(secondOldService.LanguageAvailabilities.First().LastFailedPublishAt);
        }

        private IEnumerable<ServiceVersioned> CreateOldServices(List<Versioning> versionings)
        {
            var versioning1 = (VersioningName + 1).GetGuid();
            var versioning2 = (VersioningName + 2).GetGuid();

            yield return new ServiceVersioned
            {
                UnificRootId = ServiceName.GetGuid(),
                Id = (ServiceName + 2).GetGuid(),
                PublishingStatusId = PublishingStatus.Modified.ToString().GetGuid(),
                LanguageAvailabilities = new List<ServiceLanguageAvailability>
                {
                    new ServiceLanguageAvailability
                    {
                        LastFailedPublishAt = new DateTime(2019, 2, 2),
                        StatusId = PublishingStatus.Modified.ToString().GetGuid(),
                    }
                },
                VersioningId = versioning2,
                Versioning = versionings.FirstOrDefault(v => v.Id == versioning2)
            };

            yield return new ServiceVersioned
            {
                UnificRootId = ServiceName.GetGuid(),
                Id = (ServiceName + 1).GetGuid(),
                PublishingStatusId = PublishingStatus.Published.ToString().GetGuid(),
                LanguageAvailabilities = new List<ServiceLanguageAvailability>
                {
                    new ServiceLanguageAvailability
                    {
                        LastFailedPublishAt = new DateTime(2018, 1, 1),
                        StatusId = PublishingStatus.Modified.ToString().GetGuid(),
                    }
                },
                VersioningId = versioning1,
                Versioning = versionings.FirstOrDefault(v => v.Id == versioning1)
            };
        }

        private IEnumerable<Versioning> CreateVersionings()
        {
            yield return new Versioning
            {
                Id = (VersioningName + 2).GetGuid(),
                VersionMajor = 2,
                VersionMinor = 2,
                Modified = new DateTime(2019, 2, 2)
            };

            yield return new Versioning
            {
                Id = (VersioningName + 1).GetGuid(),
                VersionMajor = 1,
                VersionMinor = 1,
                Modified = new DateTime(2018, 1, 1)
            };
        }

        private const string VersioningName = "versioning";
        private const string ServiceName = "service";
    }
}
