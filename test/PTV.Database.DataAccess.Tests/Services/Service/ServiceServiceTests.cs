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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Service
{
    public class ServiceServiceTests 
    {
        private const string test = "test";

//        private IUnitOfWork unitOfWorkMock;
//        private IUserIdentification userIdentificationMock;
//        private ITranslationEntity translationManagerMock;
//        private TestContextManager contextManager;
//        private IServiceClassRepository serviceClassRepositoryMock;
//        private IOrganizationNameRepository organizationNameRepositoryMock;
//        private IOrganizationRepository organizationRepositoryMock;
//        private IUserOrganizationRepository userOrganizationRepositoryMock;
//        private IServiceRepository serviceRepositoryMock;
//        private IPublishingStatusTypeRepository publishingStatusTypeRepositoryMock;
//        private IOntologyTermRepository ontologyTermRepositoryMock;
//        private ILifeEventRepository lifeEventRepositoryMock;
//        private IKeywordRepository keywordRepositoryMock;
//        private ITargetGroupRepository targetGroupRepositoryMock;
//        private IStatutoryServiceLifeEventRepository statutoryServiceLifeEventRepositoryMock;
//        private IStatutoryServiceOntologyTermRepository statutoryServiceOntologyTermRepositoryMock;
//        private IStatutoryServiceServiceClassRepository statutoryServiceServiceClassRepositoryMock;
//        private IStatutoryServiceTargetGroupRepository statutoryServiceTargetGroupRepositoryMock;
//        private IServiceUtilities serviceUtilitiesMock;
//        private ILogger<ServiceService> logger;

        /// <summary>
        /// Constructor tests setup
        /// </summary>
        public ServiceServiceTests()
        {
//            unitOfWorkMock = MockRepository.GenerateStrictMock<IUnitOfWork>();
//            userIdentificationMock = MockRepository.GenerateStrictMock<IUserIdentification>();
//            translationManagerMock = MockRepository.GenerateStrictMock<ITranslationEntity>();
//            contextManager = new TestContextManager(unitOfWorkMock);
//            serviceClassRepositoryMock = MockRepository.GenerateMock<IServiceClassRepository>();
//            organizationNameRepositoryMock = MockRepository.GenerateMock<IOrganizationNameRepository>();
//            organizationRepositoryMock = MockRepository.GenerateMock<IOrganizationRepository>();
//            userOrganizationRepositoryMock = MockRepository.GenerateMock<IUserOrganizationRepository>();
//            publishingStatusTypeRepositoryMock = MockRepository.GenerateMock<IPublishingStatusTypeRepository>();
//            serviceRepositoryMock = MockRepository.GenerateMock<IServiceRepository>();
//            ontologyTermRepositoryMock = MockRepository.GenerateMock<IOntologyTermRepository>();
//            lifeEventRepositoryMock = MockRepository.GenerateMock<ILifeEventRepository>();
//            keywordRepositoryMock = MockRepository.GenerateMock<IKeywordRepository>();
//            targetGroupRepositoryMock = MockRepository.GenerateMock<ITargetGroupRepository>();
//            statutoryServiceLifeEventRepositoryMock = MockRepository.GenerateMock<IStatutoryServiceLifeEventRepository>();
//            statutoryServiceOntologyTermRepositoryMock = MockRepository.GenerateMock<IStatutoryServiceOntologyTermRepository>();
//            statutoryServiceServiceClassRepositoryMock = MockRepository.GenerateMock<IStatutoryServiceServiceClassRepository>();
//            statutoryServiceTargetGroupRepositoryMock = MockRepository.GenerateMock<IStatutoryServiceTargetGroupRepository>();
//            serviceUtilitiesMock = MockRepository.GenerateMock<ServiceUtilities>(userIdentificationMock);
//
//            logger = MockRepository.GenerateMock<ILogger<ServiceService>>();
//
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<IServiceClassRepository>()).Return(serviceClassRepositoryMock).Repeat.Once();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<IOrganizationNameRepository>()).Return(organizationNameRepositoryMock).Repeat.Once();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<IOrganizationRepository>()).Return(organizationRepositoryMock).Repeat.Any();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<IUserOrganizationRepository>()).Return(userOrganizationRepositoryMock).Repeat.Once();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<IServiceRepository>()).Return(serviceRepositoryMock).Repeat.Once();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<IPublishingStatusTypeRepository>()).Return(publishingStatusTypeRepositoryMock).Repeat.Once();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<IOntologyTermRepository>()).Return(ontologyTermRepositoryMock).Repeat.Once();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<ILifeEventRepository>()).Return(lifeEventRepositoryMock).Repeat.Once();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<IKeywordRepository>()).Return(keywordRepositoryMock).Repeat.Once();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<ITargetGroupRepository>()).Return(targetGroupRepositoryMock).Repeat.Once();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<IStatutoryServiceLifeEventRepository>()).Return(statutoryServiceLifeEventRepositoryMock).Repeat.Once();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<IStatutoryServiceOntologyTermRepository>()).Return(statutoryServiceOntologyTermRepositoryMock).Repeat.Once();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<IStatutoryServiceServiceClassRepository>()).Return(statutoryServiceServiceClassRepositoryMock).Repeat.Once();
//            unitOfWorkMock.Expect(uw => uw.CreateRepository<IStatutoryServiceTargetGroupRepository>()).Return(statutoryServiceTargetGroupRepositoryMock).Repeat.Once();
//
//            userIdentificationMock.Expect(ui => ui.GetUserName()).Return("tExceptionHoursStatus").Repeat.Once();
//
//            unitOfWorkMock.Stub(uw => uw.ApplyIncludes(Arg<IQueryable<Service>>.Is.Anything, Arg<Func<IQueryable<Service>, IQueryable<Service>>>.Is.Anything)).Do((Func<IQueryable<Service>, Func<IQueryable<Service>, IQueryable<Service>>, IQueryable<Service>>)((i,j) => i));
//            unitOfWorkMock.Stub(uw => uw.ApplyIncludes(Arg<IQueryable<StatutoryServiceLifeEvent>>.Is.Anything, Arg<Func<IQueryable<StatutoryServiceLifeEvent>, IQueryable<StatutoryServiceLifeEvent>>>.Is.Anything)).Do((Func<IQueryable<StatutoryServiceLifeEvent>, Func<IQueryable<StatutoryServiceLifeEvent>, IQueryable<StatutoryServiceLifeEvent>>, IQueryable<StatutoryServiceLifeEvent>>)((i, j) => i)).Repeat.Any();
//            unitOfWorkMock.Stub(uw => uw.ApplyIncludes(Arg<IQueryable<StatutoryServiceOntologyTerm>>.Is.Anything, Arg<Func<IQueryable<StatutoryServiceOntologyTerm>, IQueryable<StatutoryServiceOntologyTerm>>>.Is.Anything)).Do((Func<IQueryable<StatutoryServiceOntologyTerm>, Func<IQueryable<StatutoryServiceOntologyTerm>, IQueryable<StatutoryServiceOntologyTerm>>, IQueryable<StatutoryServiceOntologyTerm>>)((i, j) => i)).Repeat.Any();
//            unitOfWorkMock.Stub(uw => uw.ApplyIncludes(Arg<IQueryable<StatutoryServiceServiceClass>>.Is.Anything, Arg<Func<IQueryable<StatutoryServiceServiceClass>, IQueryable<StatutoryServiceServiceClass>>>.Is.Anything)).Do((Func<IQueryable<StatutoryServiceServiceClass>, Func<IQueryable<StatutoryServiceServiceClass>, IQueryable<StatutoryServiceServiceClass>>, IQueryable<StatutoryServiceServiceClass>>)((i, j) => i)).Repeat.Any();
//            unitOfWorkMock.Stub(uw => uw.ApplyIncludes(Arg<IQueryable<LifeEvent>>.Is.Anything, Arg<Func<IQueryable<LifeEvent>, IQueryable<LifeEvent>>>.Is.Anything)).Do((Func<IQueryable<LifeEvent>, Func<IQueryable<LifeEvent>, IQueryable<LifeEvent>>, IQueryable<LifeEvent>>)((i, j) => i)).Repeat.Any();
//            unitOfWorkMock.Stub(uw => uw.ApplyIncludes(Arg<IQueryable<OntologyTerm>>.Is.Anything, Arg<Func<IQueryable<OntologyTerm>, IQueryable<OntologyTerm>>>.Is.Anything)).Do((Func<IQueryable<OntologyTerm>, Func<IQueryable<OntologyTerm>, IQueryable<OntologyTerm>>, IQueryable<OntologyTerm>>)((i, j) => i)).Repeat.Any();
//            unitOfWorkMock.Stub(uw => uw.ApplyIncludes(Arg<IQueryable<ServiceClass>>.Is.Anything, Arg<Func<IQueryable<ServiceClass>, IQueryable<ServiceClass>>>.Is.Anything)).Do((Func<IQueryable<ServiceClass>, Func<IQueryable<ServiceClass>, IQueryable<ServiceClass>>, IQueryable<ServiceClass>>)((i, j) => i)).Repeat.Any();
        }

        /// <summary>
        /// Test for GetServiceSearch
        /// </summary>
        [Fact]
        public void GetServiceSearchTest()
        {
//            var serviceClasses = new List<ServiceClass>() { new ServiceClass() { Label = test } };
//            var organizationNames = new List<OrganizationName>() { new OrganizationName() { Name = test, Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } };
//            var publishingTypes = new List<PublishingStatusType>() { new PublishingStatusType() { Code = "Published" } };
//            var organizations  = new List<Organization>() { new Organization() };
//            var userOrganizations = new List<UserOrganization>() { new UserOrganization { UserName = "test user" } };
//            var userIdentification = new List<UserIdentification>() { new UserIdentification(new HttpContextAccessor()) };
//
//            serviceClassRepositoryMock.Expect(sr => sr.All()).Return(serviceClasses.AsQueryable()).Repeat.Once();
//            organizationNameRepositoryMock.Expect(sr => sr.All()).Return(organizationNames.AsQueryable()).Repeat.Once();
//            organizationRepositoryMock.Expect(sr => sr.All()).Return(organizations.AsQueryable()).Repeat.Once();
//            userOrganizationRepositoryMock.Expect(sr => sr.All()).Return(userOrganizations.AsQueryable()).Repeat.Once();
//            publishingStatusTypeRepositoryMock.Expect(sr => sr.All()).Return(publishingTypes.AsQueryable());
//            userIdentificationMock.Expect(sr => sr.GetUserName()).Return(userIdentification.ToString()).Repeat.Once();
//
//            translationManagerMock.Expect(tm => tm.TranslateAll<ServiceClass, VmListItem>(serviceClasses.AsQueryable().OrderBy(x => x.Label))).IgnoreArguments().Return(CreateList()).Repeat.Once();
//            translationManagerMock.Expect(tm => tm.TranslateAll<OrganizationName, VmListItem>(organizationNames.AsQueryable().OrderBy(x => x.Name))).IgnoreArguments().Return(CreateList()).Repeat.Once();
//            translationManagerMock.Expect(tm => tm.TranslateAll<PublishingStatusType, VmPublishingStatus>(publishingTypes.AsQueryable().OrderBy(x => x.Code))).IgnoreArguments().Return(new List<VmPublishingStatus>() { new VmPublishingStatus() { Type = PublishingStatus.Published } }).Repeat.Once();
//
//            var serviceService = new ServiceService(contextManager, userIdentificationMock, translationManagerMock, null, logger, new VmTreeLogic(), new ServiceLogic(), serviceUtilitiesMock, null);
//            var result = serviceService.GetServiceSearch();
//
//            serviceClassRepositoryMock.AssertWasCalled(x => x.All(), x => x.Repeat.Once());
//            organizationNameRepositoryMock.AssertWasCalled(x => x.All(), x => x.Repeat.Once());
//
//            Assert.True(result.ServiceClasses.Any());
//            Assert.True(result.Organizations.Any());
//            Assert.True(result.ServiceClassId == null);
//            Assert.True(string.IsNullOrEmpty(result.ServiceName));
//            Assert.True(!result.ServiceClassId.HasValue);
//
//            translationManagerMock.VerifyAllExpectations();
        }
//        /// <summary>
//        /// Test for SearchServices
//        /// </summary>
//        /// <param name="ontologyWord">Ontology word is set by user</param>
//        /// <param name="serviceName">Service name is set by user</param>
//        /// <param name="organizationId">Organization is set by user</param>
//        /// <param name="serviceClassId">Service class is set by user></param>
//        /// <param name="count">Number of expected result</param>
//        [Theory]
//        [InlineData(false, false, false, true, 1)]
//        [InlineData(false, false, true, false, 1)]
//        [InlineData(false, true, false, false, 1)]
//        [InlineData(true, false, false, false, 1)]
//        [InlineData(false, false, false, false, 4)]
//        [InlineData(true, true, false, false, 0)]
//        [InlineData(true, true, true, false, 0)]
//        [InlineData(true, true, false, true, 0)]
//        [InlineData(true, true, true, true, 0)]
//        [InlineData(true, false, false, true, 0)]
//        public void SearchServicesTest(bool ontologyWord,bool serviceName, bool organizationId, bool serviceClassId, int count)
//        {
////            var vmServiceSearch = new VmServiceSearch()
////            {
////                OntologyWord = ontologyWord ? test : string.Empty,
////                ServiceName = serviceName ? test : string.Empty,
////                OrganizationId = organizationId ? Guid.NewGuid() : (Guid?)null,
////                ServiceClassId = serviceClassId ? Guid.NewGuid() : (Guid?)null
////            };
////
////            serviceRepositoryMock.Expect(sr => sr.All()).Return(CreateServiceList(vmServiceSearch)).Repeat.Once();
////
////            translationManagerMock.Stub(tm => tm.TranslateAll<Service, VmServiceListItem>(Arg<IQueryable<Service>>.Is.Anything)).Do((Func<IQueryable<Service>, List<VmServiceListItem>>)(id => id.Select(x => new VmServiceListItem()).ToList()));
////
////            var serviceService = new ServiceService(contextManager, null, translationManagerMock, null, logger, new VmTreeLogic(), new ServiceLogic(), null, null);
////            var result = serviceService.SearchServices(vmServiceSearch);
////
////            Assert.Equal(count, result.Services.Count);
////            serviceRepositoryMock.AssertWasCalled(x => x.All(), x => x.Repeat.Once());
//        }


        [Fact]
        public void GetServiceStep2Test()
        {
//            var serviceClasses = new List<ServiceClass>() { new ServiceClass() { Label = test } };
//            serviceClassRepositoryMock.Expect(sr => sr.All()).Return(serviceClasses.AsQueryable()).Repeat.Any();
//            translationManagerMock.Expect(tm => tm.TranslateAll<IFintoItem, VmTreeItem>(serviceClasses.Cast<IFintoItem>().ToList())).IgnoreArguments().Return(CreateTreeList()).Repeat.Once();
//
//            var ontologyTerm = new List<OntologyTerm>() { new OntologyTerm() { Label = test } };
//            ontologyTermRepositoryMock.Expect(sr => sr.All()).Return(ontologyTerm.AsQueryable()).Repeat.Any();
//            translationManagerMock.Expect(tm => tm.TranslateAll<IFintoItem, VmTreeItem>(ontologyTerm.Cast<IFintoItem>().ToList())).IgnoreArguments().Return(CreateTreeList()).Repeat.Once();
//
//            var lifeEvent = new List<LifeEvent>() { new LifeEvent() { Label = test } };
//            lifeEventRepositoryMock.Expect(sr => sr.All()).Return(lifeEvent.AsQueryable()).Repeat.Any();
//            translationManagerMock.Expect(tm => tm.TranslateAll<IFintoItem, VmTreeItem>(lifeEvent.Cast<IFintoItem>().ToList())).IgnoreArguments().Return(CreateTreeList()).Repeat.Once();
//
//            var keyWord = new List<Keyword>() { new Keyword() { Name = test } };
//            keywordRepositoryMock.Expect(sr => sr.All()).Return(keyWord.AsQueryable()).Repeat.Once();
//            translationManagerMock.Expect(tm => tm.TranslateAll<Keyword, VmKeywordItem>(keyWord.AsQueryable().OrderBy(x => x.Name))).IgnoreArguments().Return(CreateKeywordList()).Repeat.Once();
//
//            var targetGroupK1 = new List<TargetGroup>() { new TargetGroup() { Code = "KR1" } };
//            targetGroupRepositoryMock.Expect(sr => sr.All()).Return(targetGroupK1.AsQueryable()).Repeat.Times(2);
//            translationManagerMock.Expect(tm => tm.Translate<TargetGroup, VmSelectableItem>(targetGroupK1.First())).IgnoreArguments().Return(CreateCheckBox()).Repeat.Once();
//            translationManagerMock.Expect(tm => tm.TranslateAll<TargetGroup, VmSelectableItem>(targetGroupK1.AsQueryable())).IgnoreArguments().Return(new List<VmSelectableItem>()).Repeat.Once();
//
//            var targetGroupK2 = new List<TargetGroup>() { new TargetGroup() { Code = "KR2" } };
//            targetGroupRepositoryMock.Expect(sr => sr.All()).Return(targetGroupK2.AsQueryable()).Repeat.Times(2);
//            translationManagerMock.Expect(tm => tm.Translate<TargetGroup, VmSelectableItem>(targetGroupK2.First())).IgnoreArguments().Return(CreateCheckBox()).Repeat.Once();
//            translationManagerMock.Expect(tm => tm.TranslateAll<TargetGroup, VmSelectableItem>(targetGroupK2.AsQueryable())).IgnoreArguments().Return(new List<VmSelectableItem>()).Repeat.Once();
//
//            var targetGroupK3 = new List<TargetGroup>() { new TargetGroup() { Code = "KR3" } };
//            targetGroupRepositoryMock.Expect(sr => sr.All()).Return(targetGroupK3.AsQueryable()).Repeat.Times(2);
//            translationManagerMock.Expect(tm => tm.Translate<TargetGroup, VmSelectableItem>(targetGroupK3.First())).IgnoreArguments().Return(CreateCheckBox()).Repeat.Once();
//            translationManagerMock.Expect(tm => tm.TranslateAll<TargetGroup, VmSelectableItem>(targetGroupK3.AsQueryable())).IgnoreArguments().Return(new List<VmSelectableItem>()).Repeat.Once();
//
//            var serviceService = new ServiceService(contextManager, null, translationManagerMock, null, logger, new VmTreeLogic(), new ServiceLogic(), null, null);
//            var result = serviceService.GetServiceStep2(new VmGetServiceStep() { GeneralDescriptionId = null, ServiceId = null});
//
//            Assert.True(result.ServiceClassesSource.Any());
//            Assert.False(result.ServiceClassesTarget.Any());
//
//            Assert.True(result.OntologyTermsSource.Any());
//            Assert.False(result.OntologyTermsTarget.Any());
//
//            Assert.True(result.LifeEventsSource.Any());
//            Assert.False(result.LifeEventsTarget.Any());
//
//            Assert.True(result.TargetGroupCitizens.IsSelected);
//            Assert.Equal(result.TargetGroupCitizens.Name, test);
//            Assert.False(result.TargetGroupCompanies.IsSelected);
//            Assert.Equal(result.TargetGroupCompanies.Name, test);
//            Assert.False(result.TargetGroupOfficials.IsSelected);
//            Assert.Equal(result.TargetGroupOfficials.Name, test);
        }

        [Fact]
        public void GetServiceStep2WithGeneralDescriptionTest()
        {
//            var serviceClasses = new List<ServiceClass>() { new ServiceClass() { Label = test } };
//            serviceClassRepositoryMock.Expect(sr => sr.All()).Return(serviceClasses.AsQueryable()).Repeat.Any();
//            translationManagerMock.Expect(tm => tm.TranslateAll<IFintoItem, VmTreeItem>(serviceClasses.Cast<IFintoItem>().ToList())).IgnoreArguments().Return(CreateTreeList()).Repeat.Once();
//
//            var ontologyTerm = new List<OntologyTerm>() { new OntologyTerm() { Label = test } };
//            ontologyTermRepositoryMock.Expect(sr => sr.All()).Return(ontologyTerm.AsQueryable()).Repeat.Any();
//            translationManagerMock.Expect(tm => tm.TranslateAll<IFintoItem, VmTreeItem>(ontologyTerm.Cast<IFintoItem>().ToList())).IgnoreArguments().Return(CreateTreeList()).Repeat.Once();
//
//            var lifeEvent = new List<LifeEvent>() { new LifeEvent() { Label = test } };
//            lifeEventRepositoryMock.Expect(sr => sr.All()).Return(lifeEvent.AsQueryable()).Repeat.Any();
//            translationManagerMock.Expect(tm => tm.TranslateAll<IFintoItem, VmTreeItem>(lifeEvent.Cast<IFintoItem>().ToList())).IgnoreArguments().Return(CreateTreeList()).Repeat.Once();
//
//            var keyWord = new List<Keyword>() { new Keyword() { Name = test } };
//            keywordRepositoryMock.Expect(sr => sr.All()).Return(keyWord.AsQueryable()).Repeat.Once();
//            translationManagerMock.Expect(tm => tm.TranslateAll<Keyword, VmKeywordItem>(keyWord.AsQueryable().OrderBy(x => x.Name))).IgnoreArguments().Return(CreateKeywordList()).Repeat.Once();
//
//            var targetGroupK1 = new List<TargetGroup>() { new TargetGroup() { Code = "KR1" } };
//            targetGroupRepositoryMock.Expect(sr => sr.All()).Return(targetGroupK1.AsQueryable()).Repeat.Times(2);
//            translationManagerMock.Expect(tm => tm.Translate<TargetGroup, VmSelectableItem>(targetGroupK1.First())).IgnoreArguments().Return(CreateCheckBox()).Repeat.Once();
//            translationManagerMock.Expect(tm => tm.TranslateAll<TargetGroup, VmSelectableItem>(targetGroupK1.AsQueryable())).IgnoreArguments().Return(new List<VmSelectableItem>()).Repeat.Once();
//
//            var targetGroupK2 = new List<TargetGroup>() { new TargetGroup() { Code = "KR2" } };
//            targetGroupRepositoryMock.Expect(sr => sr.All()).Return(targetGroupK2.AsQueryable()).Repeat.Times(2);
//            translationManagerMock.Expect(tm => tm.Translate<TargetGroup, VmSelectableItem>(targetGroupK2.First())).IgnoreArguments().Return(CreateCheckBox()).Repeat.Once();
//            translationManagerMock.Expect(tm => tm.TranslateAll<TargetGroup, VmSelectableItem>(targetGroupK2.AsQueryable())).IgnoreArguments().Return(new List<VmSelectableItem>()).Repeat.Once();
//
//            var targetGroupK3 = new List<TargetGroup>() { new TargetGroup() { Code = "KR3" } };
//            targetGroupRepositoryMock.Expect(sr => sr.All()).Return(targetGroupK3.AsQueryable()).Repeat.Times(2);
//            translationManagerMock.Expect(tm => tm.Translate<TargetGroup, VmSelectableItem>(targetGroupK3.First())).IgnoreArguments().Return(CreateCheckBox()).Repeat.Once();
//            translationManagerMock.Expect(tm => tm.TranslateAll<TargetGroup, VmSelectableItem>(targetGroupK3.AsQueryable())).IgnoreArguments().Return(new List<VmSelectableItem>()).Repeat.Once();
//
//            var generalTargetGropupID = new Guid();
//            var statutatoryTargetGroups = new List<StatutoryServiceTargetGroup>() { new StatutoryServiceTargetGroup() { TargetGroupId = generalTargetGropupID } };
//            statutoryServiceTargetGroupRepositoryMock.Expect(sr => sr.All()).Return(statutatoryTargetGroups.AsQueryable()).Repeat.Once();
//
//            var statutatoryServiceClasses = new List<StatutoryServiceServiceClass>() { new StatutoryServiceServiceClass() { StatutoryServiceGeneralDescriptionId = new Guid() , ServiceClass = new ServiceClass()} };
//            statutoryServiceServiceClassRepositoryMock.Expect(sr => sr.All()).Return(statutatoryServiceClasses.AsQueryable()).Repeat.Any();
//            translationManagerMock.Expect(tm => tm.TranslateAll<StatutoryServiceServiceClass, VmTreeItem>(statutatoryServiceClasses)).IgnoreArguments().Return(CreateTreeList()).Repeat.Once();
//
//            var statutatoryLifeEvents = new List<StatutoryServiceLifeEvent>() { new StatutoryServiceLifeEvent() { StatutoryServiceGeneralDescriptionId = new Guid(), LifeEvent = new LifeEvent() } };
//            statutoryServiceLifeEventRepositoryMock.Expect(sr => sr.All()).Return(statutatoryLifeEvents.AsQueryable()).Repeat.Any();
//            translationManagerMock.Expect(tm => tm.TranslateAll<StatutoryServiceLifeEvent, VmTreeItem>(statutatoryLifeEvents)).IgnoreArguments().Return(CreateTreeList()).Repeat.Once();
//
//            var statutatoryOntologyTerms = new List<StatutoryServiceOntologyTerm>() { new StatutoryServiceOntologyTerm() { StatutoryServiceGeneralDescriptionId = new Guid(), OntologyTerm = new OntologyTerm() } };
//            statutoryServiceOntologyTermRepositoryMock.Expect(sr => sr.All()).Return(statutatoryOntologyTerms.AsQueryable()).Repeat.Any();
//            translationManagerMock.Expect(tm => tm.TranslateAll<StatutoryServiceOntologyTerm, VmTreeItem>(statutatoryOntologyTerms)).IgnoreArguments().Return(CreateTreeList()).Repeat.Once();
//
//
//            var serviceService = new ServiceService(contextManager, null, translationManagerMock, null, logger, new VmTreeLogic(), new ServiceLogic(), null, null);
//            var result = serviceService.GetServiceStep2(new VmGetServiceStep() { GeneralDescriptionId = new Guid(), ServiceId = null });
//
//            Assert.True(result.ServiceClassesSource.Any());
//            Assert.True(result.ServiceClassesInitial.First().IsSelected);
//            Assert.True(result.ServiceClassesInitial.First().IsDisabled);
//            Assert.True(result.ServiceClassesInitial.Any());
//            Assert.False(result.ServiceClassesTarget.Any());
//
//            Assert.True(result.OntologyTermsSource.Any());
//            Assert.True(result.OntologyTermsInitial.First().IsSelected);
//            Assert.True(result.OntologyTermsInitial.First().IsDisabled);
//            Assert.True(result.OntologyTermsInitial.Any());
//            Assert.False(result.OntologyTermsTarget.Any());
//
//            Assert.True(result.LifeEventsSource.Any());
//            Assert.True(result.LifeEventsInitial.First().IsSelected);
//            Assert.True(result.LifeEventsInitial.First().IsDisabled);
//            Assert.True(result.LifeEventsInitial.Any());
//            Assert.False(result.LifeEventsTarget.Any());
//
//            Assert.True(result.TargetGroupCitizens.IsSelected);
//            Assert.Equal(result.TargetGroupCitizens.Name, test);
//            Assert.True(result.TargetGroupCompanies.IsSelected);
//            Assert.Equal(result.TargetGroupCompanies.Name, test);
//            Assert.True(result.TargetGroupOfficials.IsSelected);
//            Assert.Equal(result.TargetGroupOfficials.Name, test);
        }

        private IQueryable<Model.Models.Service> CreateServiceList(VmServiceSearch vmServiceSearch)
        {
            var organization = new Model.Models.OrganizationService { OrganizationId = vmServiceSearch.OrganizationId.HasValue ? vmServiceSearch.OrganizationId.Value : Guid.Empty };
            return new List<Model.Models.Service>()
            {
//                new Service() { ServiceOntologyTerms = new List<ServiceOntologyTerm>() { new ServiceOntologyTerm { OntologyTerm = new OntologyTerm { Label = vmServiceSearch.OntologyWord} } } },
//                new Service() { ServiceNames = new List<ServiceName>() { new ServiceName() { Name = vmServiceSearch.ServiceName} } },
//                new Service() { OrganizationServices = new List<Model.Models.OrganizationService> { organization } },
//                new Service() { ServiceServiceClasses = new List<ServiceServiceClass>() { new ServiceServiceClass() { ServiceClassId = vmServiceSearch.ServiceClassId ?? Guid.Empty } } }

            }.AsQueryable();
        }
        private IReadOnlyList<VmListItem> CreateList()
        {
            return new List<VmListItem>() { new VmListItem() { Name = test } };
        }
        private IReadOnlyList<VmTreeItem> CreateTreeList()
        {
            return new List<VmTreeItem>() { new VmTreeItem() { Name = test } };
        }
        private IReadOnlyList<VmKeywordItem> CreateKeywordList()
        {
            return new List<VmKeywordItem>() { new VmKeywordItem() { Name = test } };
        }
        private VmSelectableItem CreateCheckBox()
        {
            return new VmSelectableItem() { Name = test };
        }
    }
}
