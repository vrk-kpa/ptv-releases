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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.Organizations;
using PTV.Database.DataAccess.Translators.ServiceCollections;
using PTV.Database.DataAccess.Translators.Tasks;
using PTV.Database.DataAccess.Translators.Values;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Notifications;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Tasks
{
    public class TimedPublishFailedTranslatorTest : TranslatorTestBase
    {
        [Fact]
        public void RunGenericTranslationTests()
        {
            TranslateTimedPublishFailedTask<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(AddChannelData);
            TranslateTimedPublishFailedTask<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(AddGeneralDescriptionData);
            TranslateTimedPublishFailedTask<OrganizationVersioned, OrganizationLanguageAvailability>(AddOrganizationData);
            TranslateTimedPublishFailedTask<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(AddServiceCollectionData);
            TranslateTimedPublishFailedTask<ServiceVersioned, ServiceLanguageAvailability>(AddServiceData);
        }

        private ServiceVersioned AddServiceData(ServiceLanguageAvailability entity)
        {
            entity.ServiceVersioned = new ServiceVersioned
            {
                CreatedBy = CreatedByName,
                Created = createdDate,
                PublishingStatusId = draftGuid,
                Id = versionedId,
                UnificRootId = unificRootId,
                ServiceNames = CreateNames<ServiceName>().ToList(),
                LanguageAvailabilities = new List<ServiceLanguageAvailability>{entity}
            };
            entity.ServiceVersionedId = versionedId;
            return entity.ServiceVersioned;
        }

        private ServiceCollectionVersioned AddServiceCollectionData(ServiceCollectionLanguageAvailability entity)
        {
            entity.ServiceCollectionVersioned = new ServiceCollectionVersioned
            {
                CreatedBy = CreatedByName,
                Created = createdDate,
                PublishingStatusId = draftGuid,
                Id = versionedId,
                UnificRootId = unificRootId,
                ServiceCollectionNames = CreateNames<ServiceCollectionName>().ToList(),
                LanguageAvailabilities = new List<ServiceCollectionLanguageAvailability>{entity}
            };
            entity.ServiceCollectionVersionedId = versionedId;
            return entity.ServiceCollectionVersioned;
        }

        private OrganizationVersioned AddOrganizationData(OrganizationLanguageAvailability entity)
        {
            entity.OrganizationVersioned = new OrganizationVersioned
            {
                CreatedBy = CreatedByName,
                Created = createdDate,
                PublishingStatusId = draftGuid,
                Id = versionedId,
                UnificRootId = unificRootId,
                OrganizationNames = CreateNames<OrganizationName>().ToList(),
                LanguageAvailabilities = new List<OrganizationLanguageAvailability>{entity}
            };
            entity.OrganizationVersionedId = versionedId;
            return entity.OrganizationVersioned;
        }

        private StatutoryServiceGeneralDescriptionVersioned AddGeneralDescriptionData(GeneralDescriptionLanguageAvailability entity)
        {
            entity.StatutoryServiceGeneralDescriptionVersioned = new StatutoryServiceGeneralDescriptionVersioned
            {
                CreatedBy = CreatedByName,
                Created = createdDate,
                PublishingStatusId = draftGuid,
                Id = versionedId,
                UnificRootId = unificRootId,
                Names = CreateNames<StatutoryServiceName>().ToList(),
                LanguageAvailabilities = new List<GeneralDescriptionLanguageAvailability>{entity}
            };
            entity.StatutoryServiceGeneralDescriptionVersionedId = versionedId;
            return entity.StatutoryServiceGeneralDescriptionVersioned;
        }

        private ServiceChannelVersioned AddChannelData(ServiceChannelLanguageAvailability entity)
        {
            entity.ServiceChannelVersioned = new ServiceChannelVersioned
            {
                CreatedBy = CreatedByName,
                Created = createdDate,
                PublishingStatusId = draftGuid,
                Id = versionedId,
                UnificRootId = unificRootId,
                ServiceChannelNames = CreateNames<ServiceChannelName>().ToList(),
                Type = new ServiceChannelType {Code = ChannelCode},
                LanguageAvailabilities = new List<ServiceChannelLanguageAvailability>{entity}
            };
            entity.ServiceChannelVersionedId = versionedId;
            return entity.ServiceChannelVersioned;
        }

        private IEnumerable<T> CreateNames<T>()
            where T : IName, new()
        {
            yield return new T
            {
                Name = FinnishName,
                LocalizationId = finnishGuid
            };

            yield return new T
            {
                Name = SwedishName,
                LocalizationId = swedishGuid
            };
    }

        private T CreateLanguageAvailability<T>()
            where T: ILanguageAvailability, new()
        {
            return  new T
            {
                StatusId = draftGuid,
                Created = createdDate,
                CreatedBy = CreatedByName,
                LastFailedPublishAt = lastFailedPublishDate
            };
        }

        private void TranslateTimedPublishFailedTask<TVersioned, TLanguage>(Func<TLanguage, TVersioned> addTypeSpecificData)
            where TVersioned: class
            where TLanguage: class, ILanguageAvailability, new()
        {
            var languageAvailability = CreateLanguageAvailability<TLanguage>();
            var entity = addTypeSpecificData(languageAvailability);
            var translators = RegisterTranslators();

            var translation = RunTranslationEntityToModelTest<TVersioned, VmTimedPublishFailedTask>(
                translators,
                entity);

            var expectedEntityType = GetExpectedEntityType(typeof(TLanguage));
            Assert.Equal(unificRootId, translation.Id);
            Assert.Equal(2, translation.Name.Count);
            Assert.Equal(draftGuid, translation.PublishingStatusId);
            Assert.Equal(createdDate.ToEpochTime(), translation.Created);
            Assert.Equal(CreatedByName, translation.CreatedBy);
            Assert.Equal(expectedEntityType, translation.EntityType);
            Assert.Equal(GetSubentityType(expectedEntityType), translation.SubEntityType);
            Assert.Equal(versionedId, translation.VersionedId);
        }

        private string GetSubentityType(EntityTypeEnum mainEntityType)
        {
            return mainEntityType == EntityTypeEnum.Channel
                ? ChannelCode
                : mainEntityType.ToString();
        }

        private EntityTypeEnum GetExpectedEntityType(Type type)
        {
            if (type == typeof(ServiceChannelLanguageAvailability))
            {
                return EntityTypeEnum.Channel;
            }

            if (type == typeof(GeneralDescriptionLanguageAvailability))
            {
                return EntityTypeEnum.GeneralDescription;
            }

            if (type == typeof(OrganizationLanguageAvailability))
            {
                return EntityTypeEnum.Organization;
            }

            if (type == typeof(ServiceCollectionLanguageAvailability))
            {
                return EntityTypeEnum.ServiceCollection;
            }

            return EntityTypeEnum.Service;
        }

        private List<object> RegisterTranslators()
        {
            var languageCacheMock = new Mock<ILanguageCache>();
            languageCacheMock.Setup(x => x.GetByValue(It.IsAny<Guid>()))
                .Returns<Guid>(id => id.ToString());

            return new List<object>
            {
                new PTV.Database.DataAccess.Translators.Services.ServiceNameStringTranslator(ResolveManager, TranslationPrimitives),
                new ServiceCollectionNameStringTranslator(ResolveManager, TranslationPrimitives),
                new OrganizationNameTextTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                new StatutoryServiceNameStringTranslator(ResolveManager, TranslationPrimitives),
                new PTV.Database.DataAccess.Translators.Channels.ServiceNameStringTranslator(ResolveManager, TranslationPrimitives),
                new DateTimeLongValueTranslator(),
                new LanguageAvailabilityTranslator(ResolveManager, TranslationPrimitives),

                new TimedPublishFailedChannelTranslator(ResolveManager, TranslationPrimitives, languageCacheMock.Object),
                new TimedPublishFailedGeneralDescriptionTranslator(ResolveManager, TranslationPrimitives, languageCacheMock.Object),
                new TimedPublishFailedOrganizationTranslator(ResolveManager, TranslationPrimitives, languageCacheMock.Object, TypeCache),
                new TimedPublishFailedServiceCollectionTranslator(ResolveManager, TranslationPrimitives, languageCacheMock.Object),
                new TimedPublishFailedServiceTranslator(ResolveManager, TranslationPrimitives, languageCacheMock.Object, TypeCache)
            };
        }

        private readonly Guid unificRootId = "unificroot".GetGuid();
        private readonly Guid draftGuid = PublishingStatus.Draft.ToString().GetGuid();
        private readonly DateTime createdDate = new DateTime(2018, 11, 11);
        private const string CreatedByName = "creator";
        private readonly DateTime lastFailedPublishDate = new DateTime(2019, 12, 12);
        private const string ChannelCode = "webchannel";
        private readonly Guid versionedId = "versioned".GetGuid();
        private const string FinnishName = "fi";
        private readonly Guid finnishGuid = FinnishName.GetGuid();
        private const string SwedishName = "sw";
        private readonly Guid swedishGuid = SwedishName.GetGuid();
    }
}
