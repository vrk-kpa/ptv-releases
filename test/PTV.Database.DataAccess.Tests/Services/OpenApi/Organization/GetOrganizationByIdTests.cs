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

using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public class GetOrganizationByIdTests : OrganizationServiceTestBase
    {
        private Guid fiId;
        private List<OrganizationVersioned> _organizationList;
        private OrganizationVersioned _publishedOrganization;
        //private Guid _publishedOrganizationId;
        private Guid _publishedOrganizationRootId;

        public GetOrganizationByIdTests()
        {
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));
            SetupTypesCacheMock<AreaInformationType>(typeof(AreaInformationTypeEnum));
            SetupTypesCacheMock<AreaType>(typeof(AreaTypeEnum));
            SetupTypesCacheMock<ServiceChargeType>(typeof(ServiceChargeTypeEnum));

            fiId = LanguageCache.Get("fi");
            _organizationList = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            _publishedOrganization = _organizationList.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            //_publishedOrganizationId = _publishedOrganization.Id;
            _publishedOrganizationRootId = _publishedOrganization.UnificRootId;
            // Set the changed values for organization (PTV-2184)
            _publishedOrganization.OrganizationNames.Add(new OrganizationName
            {
                //OrganizationVersionedId = _publishedOrganizationId,
                Name = "Name",
                TypeId = TypeCache.Get<NameType>(NameTypeEnum.AlternateName.ToString())
            });
            _publishedOrganization.OrganizationDisplayNameTypes.Add(new OrganizationDisplayNameType
            {
                LocalizationId = fiId,
                DisplayNameTypeId = TypeCache.Get<NameType>(NameTypeEnum.AlternateName.ToString())
            });
            _publishedOrganization.OrganizationDescriptions.Add(new OrganizationDescription
            {
                LocalizationId = fiId,
                Description = "Description",
                TypeId = TypeCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString())
            });
            _publishedOrganization.OrganizationPhones.Add(new OrganizationPhone
            {
                Phone = new Phone { ChargeTypeId = TypeCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Free.ToString()) }
            });
            _publishedOrganization.OrganizationPhones.Add(new OrganizationPhone
            {
                Phone = new Phone { ChargeTypeId = TypeCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()) }
            });
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RightVersionNotFound(bool getOnlyPublished)
        {
            // Arrange
            var id = Guid.NewGuid();
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Published, true)).Returns((Guid?)null);
            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, id, null, false)).Returns((Guid?)null);

            var service = new DataAccess.Services.OrganizationService(contextManager, translationManagerMock, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache, LanguageOrderCache, UserOrganizationService, PahaTokenProcessor);

            // Act
            var result = service.GetOrganizationById(id, DefaultVersion, getOnlyPublished);

            // Assert
            result.Should().BeNull();
            if (getOnlyPublished)
            {
                VersioningManagerMock.Verify(x => x.GetVersionId<OrganizationVersioned>(unitOfWork, id, PublishingStatus.Published, true), Times.Once());
            }
            else
            {
                VersioningManagerMock.Verify(x => x.GetVersionId<OrganizationVersioned>(unitOfWork, id, null, false), Times.Once());
            }
        }

        [Theory]
        [InlineData(AreaInformationTypeEnum.WholeCountry, null)]
        [InlineData(AreaInformationTypeEnum.WholeCountryExceptAlandIslands, null)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.Municipality)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.BusinessRegions)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.HospitalRegions)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.Province)]
        public void InterfaceVersion9CanBeFetched(AreaInformationTypeEnum areaInformationType, AreaTypeEnum? areaType)
        {
            // Arrange
            _publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(areaInformationType.ToString());
            if (areaInformationType == AreaInformationTypeEnum.AreaType)
            {
                _publishedOrganization.OrganizationAreas.Add(new OrganizationArea
                {
                    Area = new Area
                    {
                        AreaTypeId = TypeCache.Get<AreaType>(areaType.Value.ToString()),
                        AreaNames = new List<AreaName> { new AreaName { Name = "AreaName", LocalizationId = fiId } }
                    }
                });
            }
            var service = Arrange();

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, 9);

            // Assert
            result.Should().NotBeNull();
            var vm = Assert.IsType<V9VmOpenApiOrganization>(result);
            vm.OrganizationNames.Should().NotBeNullOrEmpty();
            vm.OrganizationNames.Count.Should().Be(1);
            vm.OrganizationNames.First().Type.Should().Be("AlternativeName");
            vm.DisplayNameType.Should().NotBeNullOrEmpty();
            vm.DisplayNameType.Count.Should().Be(1);
            vm.DisplayNameType.First().Type.Should().Be("AlternativeName");
            vm.OrganizationDescriptions.Should().NotBeNullOrEmpty();
            vm.OrganizationDescriptions.Count.Should().Be(1);
            vm.OrganizationDescriptions.First().Type.Should().Be("Summary");
            vm.AreaType.Should().Be(areaInformationType.ToString().GetOpenApiEnumValue<AreaInformationTypeEnum>());
            if (areaInformationType == AreaInformationTypeEnum.AreaType)
            {
                vm.Areas.Should().NotBeNullOrEmpty();
                vm.Areas.Count.Should().Be(1);
                vm.Areas.First().Type.Should().Be(areaType.ToString().GetOpenApiEnumValue<AreaTypeEnum>());
            }
            vm.PhoneNumbers.Should().NotBeNullOrEmpty();
            vm.PhoneNumbers.Count.Should().Be(2);
            vm.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "FreeOfCharge").Should().NotBeNull();
            vm.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "Chargeable").Should().NotBeNull();
        }


        [Theory]
        [InlineData(AreaInformationTypeEnum.WholeCountry, null)]
        [InlineData(AreaInformationTypeEnum.WholeCountryExceptAlandIslands, null)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.Municipality)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.BusinessRegions)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.HospitalRegions)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.Province)]
        public void InterfaceVersion8CanBeFetched(AreaInformationTypeEnum areaInformationType, AreaTypeEnum? areaType)
        {
            // Arrange
            _publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(areaInformationType.ToString());
            if (areaInformationType == AreaInformationTypeEnum.AreaType)
            {
                _publishedOrganization.OrganizationAreas.Add(new OrganizationArea
                {
                    Area = new Area
                    {
                        AreaTypeId = TypeCache.Get<AreaType>(areaType.Value.ToString()),
                        AreaNames = new List<AreaName> { new AreaName { Name = "AreaName", LocalizationId = fiId } }
                    }
                });
            }
            var service = Arrange();

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, 8);

            // Assert
            result.Should().NotBeNull();
            var vm = Assert.IsType<V8VmOpenApiOrganization>(result);
            vm.OrganizationNames.Should().NotBeNullOrEmpty();
            vm.OrganizationNames.Count.Should().Be(1);
            vm.OrganizationNames.First().Type.Should().Be("AlternativeName");
            vm.DisplayNameType.Should().NotBeNullOrEmpty();
            vm.DisplayNameType.Count.Should().Be(1);
            vm.DisplayNameType.First().Type.Should().Be("AlternativeName");
            vm.OrganizationDescriptions.Should().NotBeNullOrEmpty();
            vm.OrganizationDescriptions.Count.Should().Be(1);
            vm.OrganizationDescriptions.First().Type.Should().Be("Summary");
            vm.AreaType.Should().Be(areaInformationType.ToString().GetOpenApiEnumValue<AreaInformationTypeEnum>());
            if (areaInformationType == AreaInformationTypeEnum.AreaType)
            {
                vm.Areas.Should().NotBeNullOrEmpty();
                vm.Areas.Count.Should().Be(1);
                vm.Areas.First().Type.Should().Be(areaType.ToString().GetOpenApiEnumValue<AreaTypeEnum>());
            }
            vm.PhoneNumbers.Should().NotBeNullOrEmpty();
            vm.PhoneNumbers.Count.Should().Be(2);
            vm.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "FreeOfCharge").Should().NotBeNull();
            vm.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "Chargeable").Should().NotBeNull();
        }

        [Theory]
        [InlineData(AreaInformationTypeEnum.WholeCountry, null)]
        [InlineData(AreaInformationTypeEnum.WholeCountryExceptAlandIslands, null)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.Municipality)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.BusinessRegions)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.HospitalRegions)]
        [InlineData(AreaInformationTypeEnum.AreaType, AreaTypeEnum.Province)]
        public void InterfaceVersion7CanBeFetched(AreaInformationTypeEnum areaInformationType, AreaTypeEnum? areaType)
        {
            // Arrange
            _publishedOrganization.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(areaInformationType.ToString());
            if (areaInformationType == AreaInformationTypeEnum.AreaType)
            {
                _publishedOrganization.OrganizationAreas.Add(new OrganizationArea
                {
                    Area = new Area
                    {
                        AreaTypeId = TypeCache.Get<AreaType>(areaType.Value.ToString()),
                        AreaNames = new List<AreaName> { new AreaName { Name = "AreaName", LocalizationId = fiId } }
                    }
                });
            }
            var service = Arrange();

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, 7);

            // Assert
            result.Should().NotBeNull();
            var vm = Assert.IsType<V7VmOpenApiOrganization>(result);
            vm.OrganizationNames.Should().NotBeNullOrEmpty();
            vm.OrganizationNames.Count.Should().Be(1);
            vm.OrganizationNames.First().Type.Should().Be(NameTypeEnum.AlternateName.ToString());
            vm.DisplayNameType.Should().NotBeNullOrEmpty();
            vm.DisplayNameType.Count.Should().Be(1);
            vm.DisplayNameType.First().Type.Should().Be(NameTypeEnum.AlternateName.ToString());
            vm.OrganizationDescriptions.Should().NotBeNullOrEmpty();
            vm.OrganizationDescriptions.Count.Should().Be(1);
            vm.OrganizationDescriptions.First().Type.Should().Be(DescriptionTypeEnum.ShortDescription.ToString());
            vm.AreaType.Should().Be(areaInformationType.ToString());
            if (areaInformationType == AreaInformationTypeEnum.AreaType)
            {
                vm.Areas.Should().NotBeNullOrEmpty();
                vm.Areas.Count.Should().Be(1);
                vm.Areas.First().Type.Should().Be(areaType.ToString());
            }
            vm.PhoneNumbers.Should().NotBeNullOrEmpty();
            vm.PhoneNumbers.Count.Should().Be(2);
            vm.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == ServiceChargeTypeEnum.Free.ToString()).Should().NotBeNull();
            vm.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == ServiceChargeTypeEnum.Charged.ToString()).Should().NotBeNull();
        }

        [Fact]
        public void InterfaceVersion6CanBeFetched()
        {
            // Arrange
            var service = Arrange();

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, 6);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V6VmOpenApiOrganization>();
        }

        [Fact]
        public void InterfaceVersion5CannotBeFetched()
        {
            // Arrange            
            var service = Arrange();

            // Act
            Action act = () => service.GetOrganizationById(_publishedOrganizationRootId, 5);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void InterfaceVersion4CannotBeFetched()
        {
            // Arrange            
            var service = Arrange();

            // Act
            Action act = () => service.GetOrganizationById(_publishedOrganizationRootId, 4);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void InterfaceVersion3CannotBeFetched()
        {
            // Arrange            
            var service = Arrange();

            // Act
            Action act = () => service.GetOrganizationById(_publishedOrganizationRootId, 3);

            // Assert
            act.ShouldThrow<Exception>();
        }

        [Fact]
        public void NoLanguageVersionsPublished()
        {
            // Arrange
            var list = EntityGenerator.GetOrganizationEntityList(1, PublishingStatusCache);
            // set available languages to empty
            list.ForEach(o => o.LanguageAvailabilities = new List<OrganizationLanguageAvailability>());
            var publishedOrganization = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            var service = Arrange(list);

            // Act
            var result = service.GetOrganizationById(publishedOrganization.UnificRootId, DefaultVersion);

            // Assert
            result.Should().BeNull();
            VersioningManagerMock.Verify(x => x.GetVersionId<OrganizationVersioned>(unitOfWorkMockSetup.Object, publishedOrganization.UnificRootId, PublishingStatus.Published, true), Times.Once);
            unitOfWorkMockSetup.Verify(x => x.ApplyIncludes(It.IsAny<IQueryable<OrganizationVersioned>>(),
                It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(), It.IsAny<bool>()), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<OrganizationVersioned>()), Times.Never);
        }

        [Fact]
        public void CanGetRelatedSubOrganizations()
        {
            // Arrange
            var subOrganization = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId);
            subOrganization.ParentId = _publishedOrganizationRootId;
            _organizationList.Add(subOrganization);
            var service = Arrange();
            
            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V9VmOpenApiOrganization>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.SubOrganizations.Should().NotBeNullOrEmpty();
            var resultSubOrg = vmResult.SubOrganizations.First();
            resultSubOrg.Should().NotBeNull();
            resultSubOrg.Id.Should().Be(subOrganization.UnificRootId);
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void NotPublishedSubOrganizationsNotReturned(PublishingStatus publishingStatus)
        {
            // Arrange
            var statusId = PublishingStatusCache.Get(publishingStatus);
            var subOrganization = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(statusId);
            subOrganization.ParentId = _publishedOrganizationRootId;
            _organizationList.Add(subOrganization);
            var service = Arrange();

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V9VmOpenApiOrganization>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.SubOrganizations.Should().BeNullOrEmpty();
        }

        [Fact]
        public void CanGetRelatedServicesWhereOrganizationAsResponsible()
        {
            // Arrange
            var name = "Name";
            var sv = GetAndSetServiceForOrganization(_publishedOrganization, PublishedId, name, null, true, false, false);
            var service = Arrange();

            ServiceRepoMock.Setup(o => o.All()).Returns((new List<ServiceVersioned> { sv }).AsQueryable());
            ServiceLanguageAvailabilityRepoMock.Setup(g => g.All()).Returns(sv.LanguageAvailabilities.AsQueryable());
            ServiceNameRepoMock.Setup(g => g.All()).Returns(sv.ServiceNames.AsQueryable());

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V9VmOpenApiOrganization>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Services.Should().NotBeNullOrEmpty();
            var resultService = vmResult.Services.First();
            resultService.Should().NotBeNull();
            resultService.RoleType.Should().Be("Responsible");
            resultService.Service.Id.Should().Be(sv.UnificRootId);
            resultService.Service.Name.Should().Be(name);
        }

        [Fact]
        public void CanGetRelatedServicesWhereOrganizationAsOtherResponsible()
        {
            // Arrange
            var name = "Name";
            var sv = GetAndSetServiceForOrganization(_publishedOrganization, PublishedId, name, null, false, true, false);
            var service = Arrange();

            OrganizationServiceRepoMock.Setup(o => o.All()).Returns(_publishedOrganization.UnificRoot.OrganizationServices.AsQueryable());
            ServiceLanguageAvailabilityRepoMock.Setup(g => g.All()).Returns(sv.LanguageAvailabilities.AsQueryable());
            ServiceNameRepoMock.Setup(g => g.All()).Returns(sv.ServiceNames.AsQueryable());

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V9VmOpenApiOrganization>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Services.Should().NotBeNullOrEmpty();
            var resultService = vmResult.Services.First();
            resultService.Should().NotBeNull();
            resultService.RoleType.Should().Be("OtherResponsible");
            resultService.Service.Id.Should().Be(sv.UnificRootId);
            resultService.Service.Name.Should().Be(name);
        }

        [Fact]
        public void CanGetRelatedServicesWhereOrganizationAsProducer()
        {
            // Arrange
            var name = "Name";
            var sv = GetAndSetServiceForOrganization(_publishedOrganization, PublishedId, name, null, false, false, true);
            var service = Arrange();

            ServiceProducerRepoMock.Setup(o => o.All()).Returns(_publishedOrganization.UnificRoot.ServiceProducerOrganizations.AsQueryable());
            ServiceLanguageAvailabilityRepoMock.Setup(g => g.All()).Returns(sv.LanguageAvailabilities.AsQueryable());
            ServiceNameRepoMock.Setup(g => g.All()).Returns(sv.ServiceNames.AsQueryable());

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V9VmOpenApiOrganization>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Services.Should().NotBeNullOrEmpty();
            var resultService = vmResult.Services.First();
            resultService.Should().NotBeNull();
            resultService.RoleType.Should().Be("Producer");
            resultService.Service.Id.Should().Be(sv.UnificRootId);
            resultService.Service.Name.Should().Be(name);
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void NotPublishedServicesNotReturned(PublishingStatus publishingStatus)
        {
            // Arrange
            var sv = GetAndSetServiceForOrganization(_publishedOrganization, PublishingStatusCache.Get(publishingStatus), "Name");
            var service = Arrange();

            ServiceProducerRepoMock.Setup(o => o.All()).Returns(_publishedOrganization.UnificRoot.ServiceProducerOrganizations.AsQueryable());
            OrganizationServiceRepoMock.Setup(o => o.All()).Returns(_publishedOrganization.UnificRoot.OrganizationServices.AsQueryable());
            ServiceRepoMock.Setup(o => o.All()).Returns((new List<ServiceVersioned> { sv }).AsQueryable());
            ServiceLanguageAvailabilityRepoMock.Setup(g => g.All()).Returns(sv.LanguageAvailabilities.AsQueryable());
            ServiceNameRepoMock.Setup(g => g.All()).Returns(sv.ServiceNames.AsQueryable());

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V9VmOpenApiOrganization>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Services.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OnlyPublishedNamesReturned()
        {
            // Arrange
            var fiName = "Finnish name";
            var svName = "Swedish name";
            var fiId = LanguageCache.Get("fi");
            var svId = LanguageCache.Get("sv");
            // Set sub organizations
            var subOrganization = EntityGenerator.CreateEntity<OrganizationVersioned, Model.Models.Organization, OrganizationLanguageAvailability>(PublishedId);
            subOrganization.ParentId = _publishedOrganizationRootId;
            subOrganization.OrganizationNames.Add(new OrganizationName { Name = svName, LocalizationId = svId, OrganizationVersionedId = subOrganization.Id });
            subOrganization.OrganizationNames.Add(new OrganizationName { Name = fiName, LocalizationId = fiId, OrganizationVersionedId = subOrganization.Id });
            // Set only swedish as published version
            subOrganization.LanguageAvailabilities.Add(new OrganizationLanguageAvailability { LanguageId = svId, StatusId = PublishedId });
            _organizationList.Add(subOrganization);
            // Set services
            // Add only swedish language as published version
            var sv = GetAndSetServiceForOrganization(_publishedOrganization, PublishedId, svName, svId);
            sv.ServiceNames.Add(new ServiceName { Name = fiName, LocalizationId = fiId, TypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString()) });
            var service = Arrange();

            ServiceProducerRepoMock.Setup(o => o.All()).Returns(_publishedOrganization.UnificRoot.ServiceProducerOrganizations.AsQueryable());
            OrganizationNameRepoMock.Setup(g => g.All()).Returns(subOrganization.OrganizationNames.ToList().AsQueryable());
            OrganizationServiceRepoMock.Setup(o => o.All()).Returns(_publishedOrganization.UnificRoot.OrganizationServices.AsQueryable());
            ServiceRepoMock.Setup(o => o.All()).Returns((new List<ServiceVersioned> { sv }).AsQueryable());
            ServiceLanguageAvailabilityRepoMock.Setup(g => g.All()).Returns(sv.LanguageAvailabilities.AsQueryable());
            ServiceNameRepoMock.Setup(g => g.All()).Returns(sv.ServiceNames.AsQueryable());

            // Act
            var result = service.GetOrganizationById(_publishedOrganizationRootId, DefaultVersion);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V9VmOpenApiOrganization>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.SubOrganizations.Should().NotBeNullOrEmpty();
            var resultSubOrg = vmResult.SubOrganizations.First();
            resultSubOrg.Should().NotBeNull();
            resultSubOrg.Id.Should().Be(subOrganization.UnificRootId);
            resultSubOrg.Name.Should().Be(svName);
            vmResult.Services.Should().NotBeNullOrEmpty();
            var resultService = vmResult.Services.First();
            resultService.Service.Name.Should().Be(svName);
        }

        private DataAccess.Services.OrganizationService Arrange(List<OrganizationVersioned> list = null)
        {
            var organizationList = list ?? _organizationList;
            var publishedOrganization = organizationList.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            var id = publishedOrganization.Id;
            var rootId = publishedOrganization.UnificRootId;

            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<OrganizationVersioned>>(),
               It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<OrganizationVersioned> orgList, Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>> func, bool applyfilters) =>
               {
                   return orgList;
               });
            // service connections
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<Model.Models.OrganizationService>>(),
               It.IsAny<Func<IQueryable<Model.Models.OrganizationService>, IQueryable<Model.Models.OrganizationService>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<Model.Models.OrganizationService> items, Func<IQueryable<Model.Models.OrganizationService>, IQueryable<Model.Models.OrganizationService>> func, bool applyFilters) =>
               {
                   return items;
               });
            // Related services
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceVersioned> items, Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>> func, bool applyFilters) =>
               {
                   return items;
               });
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);
                        
            translationManagerMockSetup.Setup(t => t.Translate<OrganizationVersioned, VmOpenApiOrganizationVersionBase>(It.IsAny<OrganizationVersioned>()))
                .Returns((OrganizationVersioned org) =>
                {
                    if (org == null)
                    {
                        return null;
                    }
                    var vm = new VmOpenApiOrganizationVersionBase
                    {
                        Id = org.UnificRootId,
                        Modified = org.Modified,
                        PublishingStatus = PublishingStatusCache.GetByValue(org.PublishingStatusId),
                        AreaType = org.AreaInformationTypeId == null || org.AreaInformationTypeId == Guid.Empty ? null
                            : TypeCache.GetByValue<AreaInformationType>(org.AreaInformationTypeId).GetOpenApiEnumValue<AreaInformationTypeEnum>()
                    };
                    if (org.OrganizationNames?.Count > 0)
                    {
                        vm.OrganizationNames = new List<VmOpenApiLocalizedListItem>();
                        org.OrganizationNames.ForEach(n => vm.OrganizationNames.Add(new VmOpenApiLocalizedListItem
                        {
                            Value = n.Name,
                            Type = TypeCache.GetByValue<NameType>(n.TypeId).GetOpenApiEnumValue<NameTypeEnum>(),
                            Language = LanguageCache.GetByValue(n.LocalizationId)
                        }));
                    }
                    if (org.OrganizationDisplayNameTypes?.Count > 0)
                    {
                        vm.DisplayNameType = new List<VmOpenApiNameTypeByLanguage>();
                        org.OrganizationDisplayNameTypes.ForEach(d => vm.DisplayNameType.Add(new VmOpenApiNameTypeByLanguage
                        {
                            Language = LanguageCache.GetByValue(d.LocalizationId),
                            Type = TypeCache.GetByValue<NameType>(d.DisplayNameTypeId).GetOpenApiEnumValue<NameTypeEnum>(),
                        }));
                    }
                    if (org.OrganizationDescriptions?.Count > 0)
                    {
                        vm.OrganizationDescriptions = new List<VmOpenApiLocalizedListItem>();
                        org.OrganizationDescriptions.ForEach(d => vm.OrganizationDescriptions.Add(new VmOpenApiLocalizedListItem
                        {
                            Value = d.Description,
                            Type = TypeCache.GetByValue<DescriptionType>(d.TypeId).GetOpenApiEnumValue<DescriptionTypeEnum>(),
                            Language = LanguageCache.GetByValue(d.LocalizationId)
                        }));
                    }
                    if (org.OrganizationAreas?.Count > 0)
                    {
                        vm.Areas = new List<VmOpenApiArea>();
                        org.OrganizationAreas.ForEach(a => vm.Areas.Add(new VmOpenApiArea
                        {
                            Type = a.Area ==  null ? null : TypeCache.GetByValue<AreaType>(a.Area.AreaTypeId).GetOpenApiEnumValue<AreaTypeEnum>(),
                        }));
                    }
                    if (org.OrganizationPhones?.Count > 0)
                    {
                        vm.PhoneNumbers = new List<V4VmOpenApiPhone>();
                        org.OrganizationPhones.ForEach(p => vm.PhoneNumbers.Add(new V4VmOpenApiPhone
                        {
                            ServiceChargeType = p.Phone == null ? null : TypeCache.GetByValue<ServiceChargeType>(p.Phone.ChargeTypeId).GetOpenApiEnumValue<ServiceChargeTypeEnum>()
                        }));
                    }
                    if (org.UnificRoot.Children?.Count > 0)
                    {
                        vm.SubOrganizations = new List<VmOpenApiItem>();
                        org.UnificRoot.Children.ForEach(subOrg =>
                        {
                            var publishedLanguageIds = subOrg.LanguageAvailabilities.Where(l => l.StatusId == PublishedId).Select(l => l.LanguageId).ToList();
                            string name = null;
                            if (subOrg.OrganizationNames?.Count > 0)
                            {
                                var nameItem = subOrg.OrganizationNames.Where(o => publishedLanguageIds.Contains(o.LocalizationId)).FirstOrDefault();
                                if (nameItem != null) { name = nameItem.Name; }
                            }
                            vm.SubOrganizations.Add(new VmOpenApiItem { Id = subOrg.UnificRootId, Name = name });
                        });
                    }
                    return vm;
                });
                
            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<OrganizationVersioned>(unitOfWork, rootId, PublishingStatus.Published, true)).Returns(id);

            // repositories
            OrganizationRepoMock.Setup(g => g.All()).Returns(organizationList.AsQueryable());
            OrganizationNameRepoMock.Setup(m => m.All()).Returns(Enumerable.Empty<OrganizationName>().AsQueryable());

            return new DataAccess.Services.OrganizationService(contextManager, translationManagerMock, TranslationManagerVModel, Logger, OrganizationLogic,
                serviceUtilities, DataUtils, CommonService, AddressService, PublishingStatusCache, LanguageCache,
                VersioningManager, UserOrganizationChecker, CacheManager.TypesCache, LanguageOrderCache, UserOrganizationService, PahaTokenProcessor);
        }
    }
}
