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
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Common
{
    public class UpdateHistoryMetaDataTest : TestBase
    {
        private readonly CommonService _commonService;

        public UpdateHistoryMetaDataTest()
        {
            _commonService = new CommonService(
                null,
                null,
                contextManagerMock.Object,
                CacheManager.TypesCache,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);
        }

       
        private void RegisterRepositories<TEntity, TLanguageAvail>(List<TEntity> items) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailabilityBase
        {
            RegisterRepository<IRepository<TEntity>, TEntity>(items.AsQueryable());
        }
        
       
        [Fact]
        public void ServiceUpdateHistoryMetaDataTest()
        {
            var dbData = CreateEntities<ServiceVersioned, ServiceLanguageAvailability>();
            var newData = CreateEntities<ServiceVersioned, ServiceLanguageAvailability>(true);
            newData.ForEach(x=>x.LanguageAvailabilities.ForEach(y=>y.ServiceVersionedId = x.Id));
           
            RegisterRepositories<ServiceVersioned, ServiceLanguageAvailability>(dbData);
    
            _commonService.UpdateHistoryMetaData(dbData, newData.SelectMany(x=>x.LanguageAvailabilities).ToList());

            CheckResult(dbData);           
        }
        
        [Fact]
        public void ServiceChannelUpdateHistoryMetaDataTest()
        {
            var dbData = CreateEntities<ServiceChannelVersioned, ServiceChannelLanguageAvailability>();
            var newData = CreateEntities<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(true);
            newData.ForEach(x=>x.LanguageAvailabilities.ForEach(y=>y.ServiceChannelVersionedId = x.Id));
           
            RegisterRepositories<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(dbData);
    
            _commonService.UpdateHistoryMetaData(dbData, newData.SelectMany(x=>x.LanguageAvailabilities).ToList());

            CheckResult(dbData);        
        }
        
        [Fact]
        public void OrganizationUpdateHistoryMetaDataTest()
        {
            var dbData = CreateEntities<OrganizationVersioned, OrganizationLanguageAvailability>();
            var newData = CreateEntities<OrganizationVersioned, OrganizationLanguageAvailability>(true);
            newData.ForEach(x=>x.LanguageAvailabilities.ForEach(y=>y.OrganizationVersionedId = x.Id));
           
            RegisterRepositories<OrganizationVersioned, OrganizationLanguageAvailability>(dbData);
    
            _commonService.UpdateHistoryMetaData(dbData, newData.SelectMany(x=>x.LanguageAvailabilities).ToList());

            CheckResult(dbData);            
        }
        
        [Fact]
        public void GenDescriptionUpdateHistoryMetaDataTest()
        {
            var dbData = CreateEntities<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>();
            var newData = CreateEntities<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(true);
            newData.ForEach(x=>x.LanguageAvailabilities.ForEach(y=>y.StatutoryServiceGeneralDescriptionVersionedId = x.Id));
           
            RegisterRepositories<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(dbData);
    
            _commonService.UpdateHistoryMetaData(dbData, newData.SelectMany(x=>x.LanguageAvailabilities).ToList());

            CheckResult(dbData);          
        }

        [Fact]
        public void ServiceCollectionUpdateHistoryMetaDataTest()
        {
            var dbData = CreateEntities<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>();
            var newData = CreateEntities<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(true);
            newData.ForEach(x => x.LanguageAvailabilities.ForEach(y => y.ServiceCollectionVersionedId = x.Id));

            RegisterRepositories<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(dbData);

            _commonService.UpdateHistoryMetaData(dbData, newData.SelectMany(x => x.LanguageAvailabilities).ToList());

            CheckResult(dbData);
        }


        private void CheckResult<TEntity>(IEnumerable<TEntity> data) where TEntity : IVersionedVolume
        {
            var result = JsonConvert.DeserializeObject<VmHistoryMetaData>(data.First().Versioning.Meta);
            result.LanguagesMetaData.Should().NotBeNull();
            result.LanguagesMetaData.Count.Should().Be(1);
            result.LanguagesMetaData.First().Reviewed.Should().Be(DateTime.MaxValue);
            result.LanguagesMetaData.First().ReviewedBy.Should().Be("new");
            result.LanguagesMetaData.First().PublishedAt.Should().Be(DateTime.MaxValue);                    
        }

        private List<TEntity> CreateEntities<TEntity, TLanguageAvail>(bool update = false) 
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IVersionedTrackedEntity, new()
            where TLanguageAvail : class, ILanguageAvailabilityBase, new() => new List<TEntity>
        {
            new TEntity
            {
                Id = "entityId".GetGuid(),
                Versioning = new Versioning
                {
                    Meta = JsonConvert.SerializeObject(new VmHistoryMetaData
                    {
                        LanguagesMetaData = new List<VmHistoryMetaDataLanguage>
                        {
                            new VmHistoryMetaDataLanguage
                            {
                                LanguageId = "fi".GetGuid()
                            }
                        }
                    })
                },
                LanguageAvailabilities = CreateLanguages<TLanguageAvail>(update)
            }
        };
        private List<TLanguageAvail> CreateLanguages<TLanguageAvail>(bool update) where TLanguageAvail : class, ILanguageAvailabilityBase, new()
        => new List<TLanguageAvail>
        {
            new TLanguageAvail
            {
                LanguageId = "fi".GetGuid(),
                Reviewed = update ? DateTime.MaxValue : DateTime.MinValue,
                PublishAt = update ? DateTime.MaxValue : DateTime.MinValue,
                ReviewedBy = update ? "new" : "old",
            }
        };
        
    }
}
