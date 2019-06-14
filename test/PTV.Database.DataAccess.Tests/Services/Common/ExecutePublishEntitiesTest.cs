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
using Microsoft.Extensions.Logging;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Common
{
    public class ExecutePublishEntitiesTest : TestBase
    {
        [Fact]
        public void InvalidEntitiesFailToPublish()
        {
            var channelsToBePublished = CreateChannels().ToList();
            var validationMessages = CreateValidationMessages(channelsToBePublished);

            RegisterRepository<IRepository<ServiceChannelVersioned>, ServiceChannelVersioned>(channelsToBePublished
                .AsQueryable());

            var validationManagerMock = new Mock<IValidationManager>();
            validationManagerMock
                .Setup(x => x.CheckEntity<ServiceChannelVersioned>(
                    It.IsAny<Guid>(),
                    It.IsAny<IUnitOfWorkWritable>(), 
                    It.IsAny<IVmLocalizedEntityModel>()))
                .Returns(validationMessages);
            var logger = new Mock<ILogger<CommonService>>();
            
            var commonService = new CommonService(
                null,
                null,
                null,
                null,
                CacheManager.PublishingStatusCache,
                null,
                null,
                null,
                null,
                validationManagerMock.Object,
                null,
                null,
                null,
                null,
                null,
                logger.Object,
                null,
                null);

            var model = new List<VmPublishingModel>
            {
                new VmPublishingModel
                {
                    Id = (ChannelName + 1).GetGuid(),
                    LanguagesAvailabilities = new List<VmLanguageAvailabilityInfo>
                    {
                        new VmLanguageAvailabilityInfo
                        {
                            LanguageId = FinnishName.GetGuid(),
                            StatusId = PublishingStatus.Published.ToString().GetGuid()
                        }
                    }
                }
            };

            var result = commonService
                .ExecutePublishEntities<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                    unitOfWorkMockSetup.Object,
                    model,
                    true);

            var channelEntity = channelsToBePublished.First();
            
            Assert.Null(channelEntity.LanguageAvailabilities.First().PublishAt);
            Assert.Equal(scheduledPublishDate, channelEntity.LanguageAvailabilities.First().LastFailedPublishAt);
        }

        private Dictionary<Guid, List<ValidationMessage>> CreateValidationMessages(List<ServiceChannelVersioned> channels)
        {
            return channels
                .SelectMany(c => c.LanguageAvailabilities)
                .ToDictionary(c => c.LanguageId, c => new List<ValidationMessage>
                {
                    new ValidationMessage
                    {
                        Key = "Name",
                        ErrorType = "Cannot be null"
                    }
                });
        }

        private IEnumerable<ServiceChannelVersioned> CreateChannels()
        {
            yield return new ServiceChannelVersioned
            {
                Id = (ChannelName + 1).GetGuid(),
                LanguageAvailabilities = new List<ServiceChannelLanguageAvailability>
                {
                    new ServiceChannelLanguageAvailability
                    {
                        LanguageId = FinnishName.GetGuid(),
                        PublishAt = scheduledPublishDate
                    }
                }
            };
        }

        private const string ChannelName = "channel";
        private const string FinnishName = "finnish";
        private readonly DateTime scheduledPublishDate = new DateTime(2020, 3, 3);
    }
}