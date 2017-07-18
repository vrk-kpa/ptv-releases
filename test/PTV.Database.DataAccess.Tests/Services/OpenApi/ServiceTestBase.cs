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

using Moq;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using System;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi
{
    public abstract class ServiceTestBase : TestBase
    {
        internal ILockingManager LockingManagerMock { get; private set; }
        internal IVersioningManager VersioningManagerMock { get; private set; }
        internal IUserOrganizationChecker UserOrganizationCheckerMock { get; private set; }
        internal IUserIdentification UserIdentificationMock { get; private set; }
        internal ITranslationViewModel TranslationManagerVModelMock { get; private set; }

        // Services
        internal IAddressService AddressServiceMock { get; private set; }
        internal ICommonServiceInternal CommonServiceMock { get; private set; }
        internal IUserInfoService UserInfoServiceMock { get; private set; }
        internal IUserOrganizationService UserOrganizationServiceMock { get; private set; }
        internal DataUtils DataUtils { get; private set; }

        protected Guid PublishedId;
        protected Mock<ITranslationEntity> translationManagerMockSetup;

        public ServiceTestBase()
        {
            LockingManagerMock = (new Mock<ILockingManager>()).Object;
            VersioningManagerMock = (new Mock<IVersioningManager>()).Object;
            UserOrganizationCheckerMock = (new Mock<IUserOrganizationChecker>()).Object;
            UserIdentificationMock = (new Mock<IUserIdentification>()).Object;
            TranslationManagerVModelMock = (new Mock<ITranslationViewModel>()).Object;

            AddressServiceMock = (new Mock<IAddressService>()).Object;
            CommonServiceMock = (new Mock<ICommonServiceInternal>()).Object;
            UserInfoServiceMock = (new Mock<IUserInfoService>()).Object;
            UserOrganizationServiceMock = (new Mock<IUserOrganizationService>()).Object;
            DataUtils = new DataUtils();

            translationManagerMockSetup = new Mock<ITranslationEntity>();
            PublishedId = PublishingStatusCache.Get(PublishingStatus.Published);
        }
    }
}
