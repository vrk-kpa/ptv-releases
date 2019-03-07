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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models;
using System;
using Moq;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Services
{
    public class ChannelServiceTests : TestBase
    {
        private const string test = "test";

        private Mock<IOrganizationNameRepository> organizationNameRepositoryMockSetup;
        private Mock<ICommonService> commonServiceMockSetup;
        private Mock<IServiceChargeTypeRepository> serviceChargeTypeRepositoryMockSetup;
        private Mock<IOrganizationRepository> organizationRepositoryMockSetup;
        private Mock<ILanguageRepository> languageRepositoryMockSetup;
        private Mock<IUserIdentification> userIdentificationMockSetup;

        /// <summary>
        /// Constructor tests setup
        /// </summary>
        public ChannelServiceTests()
        {
            organizationNameRepositoryMockSetup = new Mock<IOrganizationNameRepository>();
            serviceChargeTypeRepositoryMockSetup = new Mock<IServiceChargeTypeRepository>();
            organizationRepositoryMockSetup = new Mock<IOrganizationRepository>();
            languageRepositoryMockSetup = new Mock<ILanguageRepository>();
            userIdentificationMockSetup = new Mock<IUserIdentification>();
            commonServiceMockSetup = new Mock<ICommonService>();

//            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationNameRepository>()).Returns(organizationNameRepositoryMockSetup.Object);
//            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChargeTypeRepository>()).Returns(serviceChargeTypeRepositoryMockSetup.Object);
//            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationRepository>()).Returns(organizationRepositoryMockSetup.Object);
//            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<ILanguageRepository>()).Returns(languageRepositoryMockSetup.Object);

//            userIdentificationMockSetup.Setup(ui => ui.UserName).Returns(test);
//            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(It.IsAny<IQueryable<Language>>(), It.IsAny<Func<IQueryable<Language>, IQueryable<Language>>>(), It.IsAny<bool>())).Returns((Func<IQueryable<Language>, Func<IQueryable<Language>, IQueryable<Language>>, IQueryable<Language>>)((i, j) => i));
            RegisterDbSet(new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// Test GetElectronicChannelStep1
        /// </summary>
        [Fact]
        public void GetElectronicChannelStep1Test()
        {
            var organizations = new List<VmListItem>() { new VmListItem { Name = test } };
            commonServiceMockSetup.Setup(sr => sr.GetOrganizationNames(null, true)).Returns(organizations);


            var serviceChargeTypes = new VmListItemsData<VmEnumType>(new List<VmEnumType>() { new VmEnumType() { Code = test } });
            commonServiceMockSetup.Setup(sr => sr.GetPhoneChargeTypes()).Returns(serviceChargeTypes);

//            var userOrganizations = new List<UserOrganization>() { new UserOrganization {  } };
//            userOrganizationRepositoryMockSetup.Setup(sr => sr.All()).Returns(userOrganizations.AsQueryable());

//            listItemLogicMockSetup.Setup(l => l.SelectByIds(It.IsAny<List<VmSelectableItem>>(), It.IsAny<List<Guid>>(), false));
            //var dataUtils = new Mock<DataUtils>();
            //var channelService = new ChannelService(contextManager, null, translationManagerMock, translationManagerViewModelMockSetup.Object, logger, null, serviceUtilitiesMock, null, commonServiceMock, listItemLogic, dataUtils.Object, ownerReferenceLogic.Object, null, cacheManagerMock.Object, null);
            //var result = channelService.GetElectronicChannelStep1(new VmChannelBasic());

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
