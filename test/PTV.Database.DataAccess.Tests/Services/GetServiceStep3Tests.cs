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

namespace PTV.Database.DataAccess.Tests.Services
{
    public class GetServiceStep3Tests
    {
        private const string test = "test";

        private TestContextManager contextManager;
        private ILogger<ServiceService> logger;

        /// <summary>
        /// Constructor tests setup
        /// </summary>
        public GetServiceStep3Tests()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWorkWritable>();
            var translationManagerMockSetup = new Mock<ITranslationEntity>();

            var serviceClassRepositoryMockSetup = new Mock<IServiceClassRepository>();
            var organizationRepositoryMockSetup = new Mock<IOrganizationNameRepository>();
            var publishingStatusTypeRepositoryMockSetup = new Mock<IPublishingStatusTypeRepository>();
            var serviceRepositoryMockSetup = new Mock<IServiceRepository>();
            var ontologyTermRepositoryMockSetup = new Mock<IOntologyTermRepository>();
            var lifeEventRepositoryMockSetup = new Mock<ILifeEventRepository>();
            var keywordRepositoryMockSetup = new Mock<IKeywordRepository>();
            var targetGroupRepositoryMockSetup = new Mock<ITargetGroupRepository>();
            logger = new Mock<ILogger<ServiceService>>().Object;
            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceClassRepository>()).Returns(serviceClassRepositoryMockSetup.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationNameRepository>()).Returns(organizationRepositoryMockSetup.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceRepository>()).Returns(serviceRepositoryMockSetup.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IPublishingStatusTypeRepository>()).Returns(publishingStatusTypeRepositoryMockSetup.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOntologyTermRepository>()).Returns(ontologyTermRepositoryMockSetup.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<ILifeEventRepository>()).Returns(lifeEventRepositoryMockSetup.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IKeywordRepository>()).Returns(keywordRepositoryMockSetup.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<ITargetGroupRepository>()).Returns(targetGroupRepositoryMockSetup.Object);


            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(It.IsAny<IQueryable<Service>>(), It.IsAny<Func<IQueryable<Service>, IQueryable<Service>>>(), It.IsAny<bool>())).Returns((Func<IQueryable<Service>, Func<IQueryable<Service>, IQueryable<Service>>, IQueryable<Service>>)((i,j) => i));


            contextManager = new TestContextManager(unitOfWorkMock, unitOfWorkMock);
        }

        //[Fact]
        public void GetAddServiceDataForStep3Test()
        {
            //var serviceClasses = new List<ServiceClass>() { new ServiceClass() { Label = test } };
            //serviceClassRepositoryMock.Expect(sr => sr.All()).Return(serviceClasses.AsQueryable()).Repeat.Any();
            //translationManagerMock.Expect(tm => tm.TranslateAll<ServiceClass, VmTreeItem>(serviceClasses.AsQueryable().OrderBy(x => x.Label))).IgnoreArguments().Return(CreateTreeList()).Repeat.Once();
            //translationManagerMock.Expect(tm => tm.TranslateAll<ServiceClass, VmTreeItem>(serviceClasses.AsQueryable().OrderBy(x => x.Label))).IgnoreArguments().Return(new List<VmTreeItem>()).Repeat.Once();

            //var ontologyTerm = new List<OntologyTerm>() { new OntologyTerm() { Label = test } };
            //ontologyTermRepositoryMock.Expect(sr => sr.All()).Return(ontologyTerm.AsQueryable()).Repeat.Any();
            //translationManagerMock.Expect(tm => tm.TranslateAll<OntologyTerm, VmTreeItem>(ontologyTerm.AsQueryable().OrderBy(x => x.Label))).IgnoreArguments().Return(CreateTreeList()).Repeat.Once();
            //translationManagerMock.Expect(tm => tm.TranslateAll<OntologyTerm, VmTreeItem>(ontologyTerm.AsQueryable().OrderBy(x => x.Label))).IgnoreArguments().Return(new List<VmTreeItem>()).Repeat.Once();

            //var lifeEvent = new List<LifeEvent>() { new LifeEvent() { Label = test } };
            //lifeEventRepositoryMock.Expect(sr => sr.All()).Return(lifeEvent.AsQueryable()).Repeat.Any();
            //translationManagerMock.Expect(tm => tm.TranslateAll<LifeEvent, VmTreeItem>(lifeEvent.AsQueryable().OrderBy(x => x.Label))).IgnoreArguments().Return(CreateTreeList()).Repeat.Once();
            //translationManagerMock.Expect(tm => tm.TranslateAll<LifeEvent, VmTreeItem>(lifeEvent.AsQueryable().OrderBy(x => x.Label))).IgnoreArguments().Return(new List<VmTreeItem>()).Repeat.Once();

            //var keyWord = new List<Keyword>() { new Keyword() { Name = test } };
            //keywordRepositoryMock.Expect(sr => sr.All()).Return(keyWord.AsQueryable()).Repeat.Once();
            //translationManagerMock.Expect(tm => tm.TranslateAll<Keyword, VmListItem>(keyWord.AsQueryable().OrderBy(x => x.Name))).IgnoreArguments().Return(CreateList()).Repeat.Once();

            //var targetGroupK1 = new List<TargetGroup>() { new TargetGroup() { Code = "KR1" } };
            //targetGroupRepositoryMock.Expect(sr => sr.All()).Return(targetGroupK1.AsQueryable()).Repeat.Times(2);
            //translationManagerMock.Expect(tm => tm.Translate<TargetGroup, VmCheckBox>(targetGroupK1.First())).IgnoreArguments().Return(CreateCheckBox()).Repeat.Once();
            //translationManagerMock.Expect(tm => tm.TranslateAll<TargetGroup, VmCheckBox>(targetGroupK1.AsQueryable())).IgnoreArguments().Return(new List<VmCheckBox>()).Repeat.Once();

            //var targetGroupK2 = new List<TargetGroup>() { new TargetGroup() { Code = "KR2" } };
            //targetGroupRepositoryMock.Expect(sr => sr.All()).Return(targetGroupK2.AsQueryable()).Repeat.Times(2);
            //translationManagerMock.Expect(tm => tm.Translate<TargetGroup, VmCheckBox>(targetGroupK2.First())).IgnoreArguments().Return(CreateCheckBox()).Repeat.Once();
            //translationManagerMock.Expect(tm => tm.TranslateAll<TargetGroup, VmCheckBox>(targetGroupK2.AsQueryable())).IgnoreArguments().Return(new List<VmCheckBox>()).Repeat.Once();

            //var targetGroupK3 = new List<TargetGroup>() { new TargetGroup() { Code = "KR3" } };
            //targetGroupRepositoryMock.Expect(sr => sr.All()).Return(targetGroupK3.AsQueryable()).Repeat.Times(2);
            //translationManagerMock.Expect(tm => tm.Translate<TargetGroup, VmCheckBox>(targetGroupK3.First())).IgnoreArguments().Return(CreateCheckBox()).Repeat.Once();
            //translationManagerMock.Expect(tm => tm.TranslateAll<TargetGroup, VmCheckBox>(targetGroupK3.AsQueryable())).IgnoreArguments().Return(new List<VmCheckBox>()).Repeat.Once();

            //var serviceService = new ServiceService(contextManager, translationManagerMock, null, logger);
            //var result = serviceService.GetServiceStep3((Guid?)null);

            //Assert.True(result.Organizations.Children.Any());
            //Assert.False(result.Organizers.Children.Any());

            //Assert.True(result.MunicipalitiesSource.Any());
            //Assert.False(result.MunicipalitiesTarget.Any());

            //Assert.True(result.ServiceProducers.Any());
            //Assert.True(result.ServiceCoverageTypes.Any());

            //Assert.True(result.ProvisionTypes.Any());

        }

        //private IQueryable<Service> CreateServiceList(VmServiceSearch vmServiceSearch)
        //{
        //    return new List<Service>()
        //    {
        //        new Service() { ServiceKeywords = new List<ServiceKeyword>() { new ServiceKeyword() { Keyword = new Keyword() { Name = vmServiceSearch.OntologyWord} } } },
        //        new Service() { ServiceNames = new List<ServiceName>() { new ServiceName() { Name = vmServiceSearch.ServiceName} } },
        //        new Service() { OrganizationId = vmServiceSearch.OrganizationId.HasValue ? vmServiceSearch.OrganizationId.Value : Guid.Empty },
        //        new Service() { ServiceServiceClasses = new List<ServiceServiceClass>() { new ServiceServiceClass() { ServiceClassId = vmServiceSearch.ServiceClassId ?? Guid.Empty } } }

        //    }.AsQueryable();
        //}
        //private IReadOnlyList<VmListItem> CreateList()
        //{
        //    return new List<VmListItem>() { new VmListItem() { Name = test } };
        //}
        //private IReadOnlyList<VmTreeItem> CreateTreeList()
        //{
        //    return new List<VmTreeItem>() { new VmTreeItem() { Module = test } };
        //}
        //private VmCheckBox CreateCheckBox()
        //{
        //    return new VmCheckBox() { Name = test };
        //}
    }
}
