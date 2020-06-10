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
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.VersioningManager
{
    public class ChangeStatusOfLanguageVersionTest : TestBase
    {
        [Fact]
        public void LastFailedPublishDateIsRemoved()
        {
            var typesCacheMock = new Mock<ITypesCache>();
            typesCacheMock.Setup(x => x.Get<PublishingStatusType>(It.IsAny<string>()))
                .Returns<string>(x => x.GetGuid());
            CacheManagerMock.Setup(x => x.TypesCache).Returns(typesCacheMock.Object);

            var versioningManager = new DataAccess.Services.VersioningManager(
                CacheManager,
                CloningManager,
                new NullLogger<DataAccess.Services.VersioningManager>());

            var entity = CreateOrganization();
            var languageAvailabilities = CreateLanguageAvailabilities();
            versioningManager.ChangeStatusOfLanguageVersion<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWorkMockSetup.Object, entity, languageAvailabilities);

            Assert.Null(entity.LanguageAvailabilities.First().LastFailedPublishAt);
        }

        private IEnumerable<VmLanguageAvailabilityInfo> CreateLanguageAvailabilities()
        {
            yield return new VmLanguageAvailabilityInfo
            {
                StatusId = PublishingStatus.Published.ToString().GetGuid(),
                LanguageId = "fi".GetGuid()
            };
        }

        private OrganizationVersioned CreateOrganization()
        {
            return new OrganizationVersioned
            {
                LanguageAvailabilities = new List<OrganizationLanguageAvailability>
                {
                    new OrganizationLanguageAvailability
                    {
                        LanguageId = "fi".GetGuid(),
                        LastFailedPublishAt = new DateTime(2016, 6, 6)
                    }
                }
            };
        }
    }
}
