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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services.V2;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Connection
{
    public class GetAllServiceChannelRelationsTest : TestBase
    {
        private readonly ConnectionsService _connectionsService;
        private readonly Guid _includedId = Guid.Parse("e85da738-b3d1-4299-a744-d7fa4d2c8f51");
        private readonly Guid _excludedId = Guid.Parse("2a8770b0-0e52-4fa5-82ce-fa6f25388496");
        private readonly Dictionary<bool, Guid> _unificRootDictionary;

        public GetAllServiceChannelRelationsTest()
        {
            SetupTypesCacheMock<PublishingStatusType>();
            RegisterRepository<IServiceServiceChannelRepository, ServiceServiceChannel>(ServiceServiceChannels
                .AsQueryable());
            _connectionsService = SetupConnectionService();
            _unificRootDictionary = SetupUnificRootDictionary();
        }

        private IEnumerable<PublishingStatusType> PublishingStatusTypes =>
            Enum.GetNames(typeof(PublishingStatus)).Select(pst => CreatePublishingStatusType(pst, pst.GetGuid()));

        private IEnumerable<ServiceServiceChannel> ServiceServiceChannels => PublishingStatusTypes.SelectMany(pst =>
            _unificRootDictionary.Select(uri => CreateServiceServiceChannel(pst, uri.Value)));

        private PublishingStatusType CreatePublishingStatusType(string publishingStatusName, Guid guid)
        {
            return new PublishingStatusType
            {
                Id = guid,
                Code = publishingStatusName,
                Names = new List<PublishingStatusTypeName>
                {
                    new PublishingStatusTypeName
                    {
                        TypeId = guid,
                        Name = publishingStatusName
                    }
                }
            };
        }

        private ServiceServiceChannel CreateServiceServiceChannel(PublishingStatusType publishingStatusType,
            Guid unificRootId)
        {
            return new ServiceServiceChannel
            {
                ServiceChannelId = unificRootId,
                Service = new Model.Models.Service
                {
                    Versions = new List<ServiceVersioned>
                    {
                        new ServiceVersioned
                        {
                            PublishingStatusId = publishingStatusType.Id,
                            PublishingStatus = publishingStatusType,
                            ServiceNames = new List<ServiceName>(),
                            LanguageAvailabilities = new List<ServiceLanguageAvailability>(),
                            StatutoryServiceGeneralDescription = new StatutoryServiceGeneralDescription
                            {
                                Versions = new List<StatutoryServiceGeneralDescriptionVersioned>()
                            }
                        }
                    }
                }
            };
        }

        private ConnectionsService SetupConnectionService()
        {
            return new ConnectionsService(
                contextManagerMock.Object,
                null,
                null,
                null,
                null,
                null,
                CacheManager,
                null,
                null,
                null
            );
        }

        private Dictionary<bool, Guid> SetupUnificRootDictionary()
        {
            return new Dictionary<bool, Guid>
            {
                {true, _includedId},
                {false, _excludedId}
            };
        }

        [Fact]
        public void AllowedPublishingStatusTypesAreIncluded()
        {
            var unitOfWork = unitOfWorkMockSetup.Object;
            var result = _connectionsService.GetAllServiceChannelRelations(unitOfWork, new List<Guid> {_includedId});
            var returnedPublishingStatusIds = result.SelectMany(dictItem =>
                    dictItem.Value.SelectMany(ssc => ssc.Service.Versions.Select(v => v.PublishingStatusId)))
                .ToList();

            Assert.Contains(PublishingStatus.Draft.ToString().GetGuid(), returnedPublishingStatusIds);
            Assert.Contains(PublishingStatus.Published.ToString().GetGuid(), returnedPublishingStatusIds);
            Assert.Contains(PublishingStatus.Modified.ToString().GetGuid(), returnedPublishingStatusIds);
        }

        [Fact]
        public void UnificRootIdsAreIncluded()
        {
            var unitOfWork = unitOfWorkMockSetup.Object;
            var result = _connectionsService.GetAllServiceChannelRelations(unitOfWork, new List<Guid> {_includedId});
            var returnedServiceChannelIds =
                result.SelectMany(dictItem => dictItem.Value.Select(ssc => ssc.ServiceChannelId)).ToList();

            Assert.Contains(_unificRootDictionary[true], returnedServiceChannelIds);
        }

        [Fact]
        public void NotAllowedPublishingStatusTypesAreExcluded()
        {
            var unitOfWork = unitOfWorkMockSetup.Object;
            var result = _connectionsService.GetAllServiceChannelRelations(unitOfWork, new List<Guid> {_includedId});
            var returnedPublishingStatusIds = result.SelectMany(dictItem =>
                    dictItem.Value.SelectMany(ssc => ssc.Service.Versions.Select(v => v.PublishingStatusId)))
                .ToList();
            
            Assert.DoesNotContain(PublishingStatus.Deleted.ToString().GetGuid(), returnedPublishingStatusIds);
            Assert.DoesNotContain(PublishingStatus.OldPublished.ToString().GetGuid(), returnedPublishingStatusIds);
        }

        [Fact]
        public void IdsNotInUnificRootIdsAreExcluded()
        {
            var unitOfWork = unitOfWorkMockSetup.Object;
            var result = _connectionsService.GetAllServiceChannelRelations(unitOfWork, new List<Guid> {_includedId});
            var returnedServiceChannelIds =
                result.SelectMany(dictItem => dictItem.Value.Select(ssc => ssc.ServiceChannelId)).ToList();

            Assert.DoesNotContain(_unificRootDictionary[false], returnedServiceChannelIds);
        }
    }
}
