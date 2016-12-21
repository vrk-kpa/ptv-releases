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
using Xunit;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models;
using System;
using Microsoft.Extensions.Logging;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Logic;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Services
{
    public class ChannelServiceTests : TestBase
    {
        private const string test = "test";

        private Mock<IUnitOfWorkWritable> unitOfWorkMockSetup;
        private Mock<ITranslationEntity> translationManagerEntityMockSetup;
        private Mock<ITranslationViewModel> translationManagerViewModelMockSetup;
        private ITranslationEntity translationManagerMock;
        private TestContextManager contextManager;
        private Mock<IOrganizationNameRepository> organizationNameRepositoryMockSetup;
        private IOrganizationNameRepository organizationNameRepositoryMock;
        private Mock<ICommonService> commonServiceMockSetup;
        private Mock<IServiceChargeTypeRepository> serviceChargeTypeRepositoryMockSetup;
        private Mock<IUserOrganizationRepository> userOrganizationRepositoryMockSetup;
        private Mock<IOrganizationRepository> organizationRepositoryMockSetup;
        private Mock<ILanguageRepository> languageRepositoryMockSetup;
        private Mock<IUserIdentification> userIdentificationMockSetup;
        private Mock<ServiceUtilities> serviceUtilitiesMockSetup;
        private Mock<VmListItemLogic> listItemLogicMockSetup;
        private VmListItemLogic listItemLogic;
        private ServiceUtilities serviceUtilitiesMock;
        private ICommonService commonServiceMock;
        private Mock<VmOwnerReferenceLogic> ownerReferenceLogic;
        private ILogger<ChannelService> logger;
        private Mock<ICacheManager> cacheManagerMock;

        /// <summary>
        /// Constructor tests setup
        /// </summary>
        public ChannelServiceTests()
        {
            unitOfWorkMockSetup = new Mock<IUnitOfWorkWritable>();
            translationManagerEntityMockSetup = new Mock<ITranslationEntity>();
            translationManagerViewModelMockSetup = new Mock<ITranslationViewModel>();
            cacheManagerMock = SetupCacheManager();


            organizationNameRepositoryMockSetup = new Mock<IOrganizationNameRepository>();
            serviceChargeTypeRepositoryMockSetup = new Mock<IServiceChargeTypeRepository>();
            userOrganizationRepositoryMockSetup = new Mock<IUserOrganizationRepository>();
            organizationRepositoryMockSetup = new Mock<IOrganizationRepository>();
            languageRepositoryMockSetup = new Mock<ILanguageRepository>();
            userIdentificationMockSetup = new Mock<IUserIdentification>();
            commonServiceMockSetup = new Mock<ICommonService>();
            ownerReferenceLogic = new Mock<VmOwnerReferenceLogic>();
            listItemLogic = new VmListItemLogic();
            listItemLogicMockSetup = new Mock<VmListItemLogic>();

            logger = new Mock<ILogger<ChannelService>>().Object;

            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IUserOrganizationRepository>()).Returns(userOrganizationRepositoryMockSetup.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationNameRepository>()).Returns(organizationNameRepositoryMockSetup.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChargeTypeRepository>()).Returns(serviceChargeTypeRepositoryMockSetup.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationRepository>()).Returns(organizationRepositoryMockSetup.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<ILanguageRepository>()).Returns(languageRepositoryMockSetup.Object);

            userIdentificationMockSetup.Setup(ui => ui.UserName).Returns(test);

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(It.IsAny<IQueryable<Language>>(), It.IsAny<Func<IQueryable<Language>, IQueryable<Language>>>(), It.IsAny<bool>())).Returns((Func<IQueryable<Language>, Func<IQueryable<Language>, IQueryable<Language>>, IQueryable<Language>>)((i, j) => i));
            serviceUtilitiesMockSetup = new Mock<ServiceUtilities>(userIdentificationMockSetup.Object, commonServiceMockSetup.Object, listItemLogicMockSetup.Object);
            contextManager = new TestContextManager(unitOfWorkMockSetup.Object, unitOfWorkMockSetup.Object);
            translationManagerMock = translationManagerEntityMockSetup.Object;
            serviceUtilitiesMock = serviceUtilitiesMockSetup.Object;
            organizationNameRepositoryMock = organizationNameRepositoryMockSetup.Object;
            commonServiceMock = commonServiceMockSetup.Object;

            RegisterDbSet(new List<ServiceChannel>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// Test GetElectronicChannelStep1
        /// </summary>
        [Fact]
        public void GetElectronicChannelStep1Test()
        {
            var organizations = new List<VmListItem>() { new VmListItem { Name = test } };
            commonServiceMockSetup.Setup(sr => sr.GetOrganizationNames(unitOfWorkMockSetup.Object, null, true)).Returns(organizations);


            var serviceChargeTypes = new List<VmSelectableItem>() { new VmSelectableItem { Code = test } };
            commonServiceMockSetup.Setup(sr => sr.GetPhoneChargeTypes(unitOfWorkMockSetup.Object)).Returns(serviceChargeTypes);

            var userOrganizations = new List<UserOrganization>() { new UserOrganization { UserName = "test user" } };
            userOrganizationRepositoryMockSetup.Setup(sr => sr.All()).Returns(userOrganizations.AsQueryable());

//            listItemLogicMockSetup.Setup(l => l.SelectByIds(It.IsAny<List<VmSelectableItem>>(), It.IsAny<List<Guid>>(), false));
            //var dataUtils = new Mock<DataUtils>();
            //var channelService = new ChannelService(contextManager, null, translationManagerMock, translationManagerViewModelMockSetup.Object, logger, null, serviceUtilitiesMock, null, commonServiceMock, listItemLogic, dataUtils.Object, ownerReferenceLogic.Object, null, cacheManagerMock.Object, null);
            //var result = channelService.GetElectronicChannelStep1(new VmGetChannelStep());

            //organizationNameRepositoryMockSetup.AssertWasCalled(x => x.All(), x => x.Repeat.Once());
            //serviceChargeTypeRepositoryMockSetup.AssertWasCalled(x => x.All(), x => x.Repeat.Once());

            //Assert.True(result.Description.Organizations.Any());
            //Assert.True(result.Support.PhoneChargeTypes.Any());
        }

        /// <summary>
        /// Test GetWebPageChannelStep1
        /// </summary>
        [Fact]
        public void GetWebPageChannelStep1Test()
        {
//            var organizations = new List<OrganizationName>() { new OrganizationName() { Name = test, Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } };
//            organizationNameRepositoryMockSetup.Setup(sr => sr.All()).Returns(organizations.AsQueryable());
//            translationManagerMockSetup.Setup(tm => tm.TranslateAll<OrganizationName, VmListItem>(organizations.AsQueryable().OrderBy(x => x.Name))).Returns(CreateList());
//
//
//            var serviceChargeTypes = new List<ServiceChargeType>() { new ServiceChargeType() { Code = test } };
//            serviceChargeTypeRepositoryMockSetup.Setup(sr => sr.All()).Returns(serviceChargeTypes.AsQueryable());
//            translationManagerMockSetup.Setup(tm => tm.TranslateAll<TypeBase, VmSelectableItem>(serviceChargeTypes.AsQueryable().OrderBy(x => x.OrderNumber))).Returns(CreateCheckBoxList());
//
//            var languages = new List<Language>() { new Language() { Code = test } };
//            languageRepositoryMockSetup.Setup(sr => sr.All()).Returns(languages.AsQueryable());
//            translationManagerMockSetup.Setup(tm => tm.TranslateAll<Language, VmListItem>(languages.AsQueryable().OrderBy(x => x.Code))).Returns(CreateList());
//
//
//            var userOrganizations = new List<UserOrganization>() { new UserOrganization { UserName = "test user" } };
//            userOrganizationRepositoryMockSetup.Setup(sr => sr.All()).Returns(userOrganizations.AsQueryable());
//
//            var channelService = new ChannelService(contextManager, null, translationManagerMock, null, logger, null, serviceUtilitiesMock, null);
//            var result = channelService.GetWebPageChannelStep1((Guid?)null);
//
//            //organizationNameRepositoryMock.AssertWasCalled(x => x.All(), x => x.Repeat.Once());
//            //serviceChargeTypeRepositoryMock.AssertWasCalled(x => x.All(), x => x.Repeat.Once());
//
//            Assert.True(result.Description.Organizations.Any());
//            Assert.True(result.Support.PhoneChargeTypes.Any());
//            Assert.True(result.WorldLanguages.Any());
        }

        private IReadOnlyList<VmSelectableItem> CreateCheckBoxList()
        {
            return new List<VmSelectableItem>() { CreateCheckBox()  };
        }
        private VmSelectableItem CreateCheckBox()
        {
            return new VmSelectableItem() { Name = test };
        }
        private IReadOnlyList<VmListItem> CreateList()
        {
            return new List<VmListItem>() { new VmListItem() { Name = test } };
        }
    }
}
