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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services.V2;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Mass;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using Xunit;
using IChannelService = PTV.Database.DataAccess.Interfaces.Services.V2.IChannelService;
using IServiceService = PTV.Database.DataAccess.Interfaces.Services.V2.IServiceService;

namespace PTV.Database.DataAccess.Tests.Services.MassTool
{
    public class MassServiceMessageResultTests : TestBase
    {
        private IMassService massService;
        private Mock<ICommonServiceInternal> commonService;
        private Mock<ITranslationEntity> translateToVm;
        private Mock<ITranslationViewModel> translateToEntity;
        private ItemListModelGenerator listGenerator = new ItemListModelGenerator();
        private MassToolConfiguration massToolConfiguration;

        private void SetupCommonService<TEntity, TLanguageAvail>() where TEntity : class, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailabilityBase, new()
        {
            commonService.Setup(
                x => x.ExecutePublishEntities<TEntity, TLanguageAvail>(It.IsAny<IUnitOfWorkWritable>(),
                    It.IsAny<IReadOnlyList<IVmLocalizedEntityModel>>(), It.IsAny<bool>())
            ).Returns(new List<PublishingResult>());
            
            commonService.Setup(
                x => x.ExecuteArchiveEntities<TEntity, TLanguageAvail>(It.IsAny<IUnitOfWorkWritable>(),
                    It.IsAny<IReadOnlyList<Guid>>(), It.IsAny<Action<IUnitOfWorkWritable, Guid>>())
            );
            commonService.Setup(
                x => x.UpdateHistoryMetaData<TEntity, TLanguageAvail>(It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<List<TLanguageAvail>>(), unitOfWorkMockSetup.Object)
            );
            SetupTranslationEntity<VmMassLanguageAvailabilityModel, TLanguageAvail>();
        }

        private void SetupCommonService<TEntity, TRoot, TLanguageAvail>() where TEntity : class, IMultilanguagedEntity<TLanguageAvail>, IVersionedVolume<TRoot>, IOriginalEntity, IOrganizationInfo, INameReferences, new() where TLanguageAvail : class, ILanguageAvailability where TRoot : IVersionedRoot, new()
        {
            commonService.Setup(
                x => x.ExecuteCopyEntities<TEntity, TRoot, TLanguageAvail>(It.IsAny<IUnitOfWorkWritable>(),
                    It.IsAny<IReadOnlyList<Guid>>(), It.IsAny<Guid>())
            );
        }
        
        
        private void SetupTranslation<TSource, TOut>() where TSource : class where TOut : class, new()
        {
            translateToVm
                .Setup(x => x.TranslateAll<TSource, TOut>(It.IsAny<IEnumerable<TSource>>()))
                .Returns((IEnumerable<TSource> source) => source.Select(x => new TOut()).ToList());
        }
        
        private void SetupTranslationEntity<TSource, TOut>() where TSource : class where TOut : class, new()
        {
            translateToEntity
                .Setup(x => x.TranslateAll<TSource, TOut>(It.IsAny<List<TSource>>(), It.IsAny<IUnitOfWorkWritable>()))
                .Returns((List<TSource> source, IUnitOfWorkWritable unit) => source.Select(x => new TOut()).ToList());
        }

        public MassServiceMessageResultTests()
        {
            var configuration = new Mock<IOptions<MassToolConfiguration>>(MockBehavior.Strict);
            massToolConfiguration = new MassToolConfiguration();
            configuration.SetupGet(x => x.Value).Returns(massToolConfiguration);
            translateToEntity = new Mock<ITranslationViewModel>(MockBehavior.Strict);
            translateToVm = new Mock<ITranslationEntity>(MockBehavior.Strict);
                        
            commonService = new Mock<ICommonServiceInternal>(MockBehavior.Strict);
            
            massService = new MassService
            (
                contextManagerMock.Object,
                CacheManager,
                null,
                null,
                commonService.Object, 
                translateToVm.Object,
                translateToEntity.Object, 
                configuration.Object,
                new Mock<ILogger<MassService>>().Object, 
                null,
                null,
                new Mock<IChannelService>().Object, 
                new Mock<IGeneralDescriptionService>().Object, 
                new Mock<IServiceService>().Object, 
                null
            );
            
            SetupContextManager<IMessage, object>();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(1, null)]
        [InlineData(null, 2)]
        [InlineData(1, 2)]
        public void ResultMessageForPublish(long? publishAt, long? archiveAt)
        {
            var model = new VmMassDataModel<VmPublishingModel>
            {
                PublishAt = publishAt, ArchiveAt = archiveAt, Id = Guid.NewGuid()
            };
            
            SetupCommonService<ServiceChannelVersioned, ServiceChannelLanguageAvailability>();
            SetupCommonService<ServiceVersioned, ServiceLanguageAvailability>();
            SetupCommonService<OrganizationVersioned, OrganizationLanguageAvailability>();
            SetupCommonService<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>();
            SetupCommonService<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>();
            commonService.Setup(x =>
                x.RemoveNotCommonConnections(It.IsAny<IEnumerable<Guid>>(), It.IsAny<IUnitOfWorkWritable>(),
                It.IsAny<bool>())
            );
            var result = massService.PublishEntities(model);
            CheckResult(result, model, publishAt.HasValue);
        }
        
        [Theory]
        [InlineData(null, null)]
        [InlineData(1, null)]
        [InlineData(null, 2)]
        [InlineData(1, 2)]
        public void ResultMessageForArchive(long? publishAt, long? archiveAt)
        {
            var model = new VmMassDataModel<VmArchivingModel>
            {
                PublishAt = publishAt, ArchiveAt = archiveAt, Id = Guid.NewGuid()
            };
            
            SetupTranslation<IBaseInformation, VmArchivingModel>();
            SetupCommonService<ServiceChannelVersioned, ServiceChannelLanguageAvailability>();            
            SetupCommonService<ServiceVersioned, ServiceLanguageAvailability>();
            SetupCommonService<OrganizationVersioned, OrganizationLanguageAvailability>();
            SetupCommonService<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>();
            SetupCommonService<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>();
            var result = massService.ArchiveEntities(model);
            CheckResult(result, model, archiveAt.HasValue);
        }

        [Fact]
        public void ResultMessageForCopy()
        {
            var model = new VmMassDataModel<VmCopyingModel> { Id = Guid.NewGuid(), OrganizationId = Guid.NewGuid() };

            SetupCommonService<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>();
            SetupContextManager<VmMassToolProcessMessage, object>();
            
            var result = massService.CopyEntities(model);
            CheckResult(result, model, false);
        }

        private void CheckResult<T>(IMessage result, VmMassDataModel<T> model, bool isScheduled) where T : IVmLocalizedEntityModel
        {
            if (isScheduled)
            {
                result.Should().BeNull();
            }
            else
            {
                result.Should().NotBeNull();
                var message = result as VmMassToolProcessMessage;
                message.Should().NotBeNull();
                message.Id.Should().Be(model.Id);
                message.Code.Should().NotBeEmpty();
            }
        }

        private IReadOnlyList<T> CreateListModel<T>(int entityCount, int languageCount) where T : IVmLocalizedEntityModel, new()
        {
            return listGenerator.Create(entityCount,
                i => new T
                    {LanguagesAvailabilities = listGenerator.Create<VmLanguageAvailabilityInfo>(languageCount).ToList()});
        }

        [Fact]
        public void PtvMaxCountValidationExceptionPublishThrowTest()
        {
            var model = new VmMassDataModel<VmPublishingModel>
            {
                Channels = CreateListModel<VmPublishingModel>(1, 2),
                Services = CreateListModel<VmPublishingModel>(1, 2),
                Organizations = CreateListModel<VmPublishingModel>(1, 2),
                GeneralDescriptions = CreateListModel<VmPublishingModel>(1, 2),
                ServiceCollections = CreateListModel<VmPublishingModel>(1, 2)
            };
            
            massToolConfiguration.MaxArchiveEntities = 4;
            massToolConfiguration.MaxCopyEntities = 4;
            massToolConfiguration.MaxPublishLanguageVersions = 9;
            Action callPublishLanguage = () => massService.PublishEntities(model);
            callPublishLanguage.Should().ThrowExactly<PtvMaxCountLanguageVersionsValidationException>();

        }
        
        [Fact]
        public void PtvMaxCountValidationExceptionArchiveThrowTest()
        {
            var model = new VmMassDataModel<VmArchivingModel>
            {
                Channels = CreateListModel<VmArchivingModel>(1, 2),
                Services = CreateListModel<VmArchivingModel>(1, 2),
                Organizations = CreateListModel<VmArchivingModel>(1, 2),
                GeneralDescriptions = CreateListModel<VmArchivingModel>(1, 2),
                ServiceCollections = CreateListModel<VmArchivingModel>(1, 2)
            };
            
            massToolConfiguration.MaxArchiveEntities = 4;
            massToolConfiguration.MaxCopyEntities = 3;
            massToolConfiguration.MaxPublishLanguageVersions = 5;
            Action callPublishLanguage = () => massService.ArchiveEntities(model);
            callPublishLanguage.Should().ThrowExactly<PtvMaxCountEntitiesValidationException>();
        }
        
        [Fact]
        public void PtvMaxCountValidationExceptionCopyThrowTest()
        {
            var model = new VmMassDataModel<VmCopyingModel>
            {
                OrganizationId = Guid.NewGuid(),
                Channels = CreateListModel<VmCopyingModel>(1, 2),
                Services = CreateListModel<VmCopyingModel>(1, 2),
                Organizations = CreateListModel<VmCopyingModel>(1, 2),
                GeneralDescriptions = CreateListModel<VmCopyingModel>(1, 2),
                ServiceCollections = CreateListModel<VmCopyingModel>(1, 2)
            };
            
            massToolConfiguration.MaxArchiveEntities = 3;
            massToolConfiguration.MaxCopyEntities = 4;
            massToolConfiguration.MaxPublishLanguageVersions = 5;
            Action callPublishLanguage = () => massService.CopyEntities(model);
            callPublishLanguage.Should().ThrowExactly<PtvMaxCountEntitiesValidationException>();
        }
        
        [Fact]
        public void PtvMandatoryOrganizationValidationExceptionThrowTest()
        {
            var model = new VmMassDataModel<VmCopyingModel>();
            
            Action callPublishLanguage = () => massService.CopyEntities(model);
            callPublishLanguage.Should().ThrowExactly<PtvMandatoryOrganizationValidationException>();
        }
        
        [Fact]
        public void PublishArchiveDatesValidationTest()
        {
            Action callPublishLanguage = () => massService.PublishEntities(new VmMassDataModel<VmPublishingModel>
            {
                PublishAt = 3,
                ArchiveAt = 2
            });
            callPublishLanguage.Should().ThrowExactly<PtvAppException>();
            
            SetupTranslation<IBaseInformation, VmArchivingModel>();
            Action callArchiveLanguage = () => massService.ArchiveEntities(new VmMassDataModel<VmArchivingModel>
            {
                PublishAt = 3,
                ArchiveAt = 2
            });
            callArchiveLanguage.Should().ThrowExactly<PtvAppException>();
        }
    }
}
