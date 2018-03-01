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

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.Domain.Logic.Address;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Services.Validation;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi
{
    public abstract class ServiceTestBase : TestBase
    {
        internal int DefaultVersion = 8;

        internal Mock<IVersioningManager> VersioningManagerMock { get; private set; }
        internal Mock<IUserIdentification> UserIdentificationMock { get; private set; }
        internal Mock<ICommonServiceInternal> CommonServiceMock { get; private set; }
        internal Mock<IExternalSourceRepository> ExternalSourceRepoMock { get; private set; }
        internal Mock<IServiceServiceChannelRepository> ConnectionRepoMock { get; private set; }
        internal ApplicationConfiguration ApplicationConfigurationMock { get; private set; }

        internal ILockingManager LockingManager { get; private set; }
        internal IVersioningManager VersioningManager { get; private set; }
        internal IUserOrganizationChecker UserOrganizationChecker { get; private set; }
        internal IUserIdentification UserIdentification { get; private set; }
        internal ITranslationViewModel TranslationManagerVModel { get; private set; }        
        internal AddressLogic AddressLogic { get; private set; }

        // Services
        internal IAddressService AddressService { get; private set; }
        internal ICommonServiceInternal CommonService { get; private set; }
        internal IUserInfoService UserInfoService { get; private set; }
        internal IUserOrganizationService UserOrganizationService { get; private set; }
        internal ITranslationService TranslationService { get; private set; }
        internal DataUtils DataUtils { get; private set; }
        internal IValidationManager ValidationManagerMock { get; private set; }

        protected Guid PublishedId;
        protected Guid DeletedId;
        protected Guid OldPublishedId;

        protected Mock<ITranslationEntity> translationManagerMockSetup;
        protected Mock<ITranslationViewModel> translationManagerVModelMockSetup;

        public ServiceTestBase()
        {
            ApplicationConfigurationMock = new ApplicationConfiguration((new Mock<IConfigurationRoot>()).Object);

            VersioningManagerMock = new Mock<IVersioningManager>();
            UserIdentificationMock = new Mock<IUserIdentification>();
            CommonServiceMock = new Mock<ICommonServiceInternal>();

            ExternalSourceRepoMock = new Mock<IExternalSourceRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IExternalSourceRepository>()).Returns(ExternalSourceRepoMock.Object);

            ConnectionRepoMock = new Mock<IServiceServiceChannelRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceServiceChannelRepository>()).Returns(ConnectionRepoMock.Object);

            translationManagerMockSetup = new Mock<ITranslationEntity>();
            translationManagerVModelMockSetup = new Mock<ITranslationViewModel>();

            LockingManager = (new Mock<ILockingManager>()).Object;
            VersioningManager = VersioningManagerMock.Object;
            UserOrganizationChecker = (new Mock<IUserOrganizationChecker>()).Object;
            UserIdentification = UserIdentificationMock.Object;
            TranslationManagerVModel = translationManagerVModelMockSetup.Object;

            AddressService = (new Mock<IAddressService>()).Object;
            CommonService = CommonServiceMock.Object;
            UserInfoService = (new Mock<IUserInfoService>()).Object;
            UserOrganizationService = (new Mock<IUserOrganizationService>()).Object;
            TranslationService = (new Mock<ITranslationService>()).Object;
            DataUtils = new DataUtils();
            ValidationManagerMock = (new Mock<IValidationManager>()).Object;
            PublishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            DeletedId = PublishingStatusCache.Get(PublishingStatus.Deleted);
            OldPublishedId = PublishingStatusCache.Get(PublishingStatus.OldPublished);

            var mapServiceProviderMock = new MapServiceProvider(
                (new Mock<IHostingEnvironment>()).Object,
                new ApplicationConfiguration((new Mock<IConfigurationRoot>()).Object),
                (new Mock<IOptions<ProxyServerSettings>>()).Object,
                new Mock<ILogger<MapServiceProvider>>().Object);

            AddressLogic = new AddressLogic(mapServiceProviderMock);
        }
    }
}
