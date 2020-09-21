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

using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V10;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Service
{
    public class GetServiceByIdTests : ServiceServiceTestBase
    {
        private const string description = "Description";

        private List<ServiceVersioned> _list;
        private ServiceVersioned _publishedItem;
        private Guid _publishedRootItemId;

        public GetServiceByIdTests()
        {
            SetupTypesCacheMock<ServiceType>(typeof(ServiceTypeEnum));
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));
            SetupTypesCacheMock<ServiceChargeType>(typeof(ServiceChargeTypeEnum));
            SetupTypesCacheMock<AreaInformationType>(typeof(AreaInformationTypeEnum));
            SetupTypesCacheMock<AreaType>(typeof(AreaTypeEnum));
            SetupTypesCacheMock<ProvisionType>(typeof(ProvisionTypeEnum));
            SetupTypesCacheMock<PhoneNumberType>(typeof(PhoneNumberTypeEnum));

            var fiId = LanguageCache.Get("fi");
            _list = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            _publishedItem = _list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            _publishedRootItemId = _publishedItem.UnificRootId;

            // Set the data
            _publishedItem.ServiceNames.Add(new ServiceName
            {
                Name = "Name",
                TypeId = TypeCache.Get<NameType>(NameTypeEnum.AlternateName.ToString())
            });
            _publishedItem.ServiceDescriptions.Add(new ServiceDescription
            {
                LocalizationId = fiId,
                Description = description,
                TypeId = TypeCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString())
            });
            _publishedItem.ServiceDescriptions.Add(new ServiceDescription
            {
                LocalizationId = fiId,
                Description = description,
                TypeId = TypeCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString())
            });
            _publishedItem.ServiceDescriptions.Add(new ServiceDescription
            {
                LocalizationId = fiId,
                Description = description,
                TypeId = TypeCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceUserInstruction.ToString())
            });
            _publishedItem.ServiceDescriptions.Add(new ServiceDescription
            {
                LocalizationId = fiId,
                Description = description,
                TypeId = TypeCache.Get<DescriptionType>(DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString())
            });
            _publishedItem.ServiceDescriptions.Add(new ServiceDescription
            {
                LocalizationId = fiId,
                Description = description,
                TypeId = TypeCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString())
            });
            _publishedItem.ServiceDescriptions.Add(new ServiceDescription
            {
                LocalizationId = fiId,
                Description = description,
                TypeId = TypeCache.Get<DescriptionType>(DescriptionTypeEnum.DeadLineAdditionalInfo.ToString())
            });
            _publishedItem.ServiceDescriptions.Add(new ServiceDescription
            {
                LocalizationId = fiId,
                Description = description,
                TypeId = TypeCache.Get<DescriptionType>(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString())
            });
            _publishedItem.ServiceDescriptions.Add(new ServiceDescription
            {
                LocalizationId = fiId,
                Description = description,
                TypeId = TypeCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceTypeAdditionalInfo.ToString())
            });
            
            _publishedItem.ServiceDescriptions.ForEach(x => x.ServiceVersionedId = _publishedItem.Id);
            _publishedItem.ServiceNames.ForEach(x => x.ServiceVersionedId = _publishedItem.Id);
        }

        [Theory]
        [InlineData(VersionStatusEnum.Published)]
        [InlineData(VersionStatusEnum.Latest)]
        [InlineData(VersionStatusEnum.LatestActive)]
        public void RightVersionNotFound(VersionStatusEnum status)
        {
            // Arrange
            var id = Guid.NewGuid();
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWork, id, PublishingStatus.Published, true)).Returns((Guid?)null);
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWork, id, null, It.IsAny<bool>())).Returns((Guid?)null);

            var service = new ServiceService(contextManager, translationManagerMock, TranslationManagerVModel, Logger, serviceUtilities,
                CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker, LanguageOrderCache, targetGroupDataCache, null, null);

            // Act
            var result = service.GetServiceById(id, DefaultVersion, status);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("PermitOrObligation", "Chargeable", "Nationwide")]
        [InlineData("ProfessionalQualification", "FreeOfCharge", "NationwideExceptAlandIslands")]
        [InlineData("Service", "Other", "LimitedType")]
        public void InterfaceVersion11CanBeFetched(string serviceType, string serviceChargeType, string areaInformationType)
        {
            // Arrange
            var service = Arrange();
            _publishedItem.TypeId = TypeCache.Get<ServiceType>(serviceType.GetEnumValueByOpenApiEnumValue<ServiceTypeEnum>());
            _publishedItem.ChargeTypeId = TypeCache.Get<ServiceChargeType>(serviceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>());
            _publishedItem.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(areaInformationType.GetEnumValueByOpenApiEnumValue<AreaInformationTypeEnum>());
            _publishedItem.Areas = areaInformationType == "LimitedType" ? new List<ServiceArea>
            {
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString())}, ServiceVersionedId = _publishedItem.Id },
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString())}, ServiceVersionedId = _publishedItem.Id },
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Municipality.ToString())}, ServiceVersionedId = _publishedItem.Id },
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Province.ToString())}, ServiceVersionedId = _publishedItem.Id }
            } : null;
            if (_publishedItem.Areas != null)
            {
                ServiceAreaRepoMock.Setup(g => g.All()).Returns(_publishedItem.Areas.AsQueryable());
            }
            if (_publishedItem.ServiceNames != null)
            {
                ServiceNameRepoMock.Setup(g => g.All()).Returns(_publishedItem.ServiceNames.AsQueryable());
            }
            if (_publishedItem.ServiceDescriptions != null)
            {
                ServiceDescriptionRepoMock.Setup(g => g.All()).Returns(_publishedItem.ServiceDescriptions.AsQueryable());
            }

            // Act
            var result = service.GetServiceById(_publishedRootItemId, 11, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vm = Assert.IsType<V11VmOpenApiService>(result);
            vm.Type.Should().Be(serviceType);
            vm.ServiceChargeType.Should().Be(serviceChargeType);
            vm.ServiceNames.Should().NotBeNullOrEmpty();
            vm.ServiceNames.Count.Should().Be(1);
            vm.ServiceNames.First().Type.Should().Be("AlternativeName");
            vm.ServiceDescriptions.Should().NotBeNullOrEmpty();
            vm.ServiceDescriptions.Count.Should().Be(8);
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "Description").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "Summary").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "UserInstruction").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ValidityTime").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ProcessingTime").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "DeadLine").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ChargeTypeAdditionalInfo").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ServiceType").Should().NotBeNull();
            vm.AreaType.Should().Be(areaInformationType);
            if (areaInformationType == "LimitedType")
            {
                vm.Areas.Should().NotBeNull();
                vm.Areas.Count.Should().BeGreaterThan(0);
                vm.Areas.FirstOrDefault(a => a.Type == "Municipality").Should().NotBeNull();
                vm.Areas.FirstOrDefault(a => a.Type == "Region").Should().NotBeNull();
                vm.Areas.FirstOrDefault(a => a.Type == "BusinessSubRegion").Should().NotBeNull();
                vm.Areas.FirstOrDefault(a => a.Type == "HospitalDistrict").Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData("PermitOrObligation", "Chargeable", "Nationwide")]
        [InlineData("ProfessionalQualification", "FreeOfCharge", "NationwideExceptAlandIslands")]
        [InlineData("Service", "Other", "LimitedType")]
        public void InterfaceVersion10CanBeFetched(string serviceType, string serviceChargeType, string areaInformationType)
        {
            // Arrange
            var service = Arrange();
            _publishedItem.TypeId = TypeCache.Get<ServiceType>(serviceType.GetEnumValueByOpenApiEnumValue<ServiceTypeEnum>());
            _publishedItem.ChargeTypeId = TypeCache.Get<ServiceChargeType>(serviceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>());
            _publishedItem.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(areaInformationType.GetEnumValueByOpenApiEnumValue<AreaInformationTypeEnum>());
            _publishedItem.Areas = areaInformationType == "LimitedType" ? new List<ServiceArea>
            {
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString())}, ServiceVersionedId = _publishedItem.Id },
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString())}, ServiceVersionedId = _publishedItem.Id },
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Municipality.ToString())}, ServiceVersionedId = _publishedItem.Id },
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Province.ToString())}, ServiceVersionedId = _publishedItem.Id }
            } : null;
            if (_publishedItem.Areas != null)
            {
                ServiceAreaRepoMock.Setup(g => g.All()).Returns(_publishedItem.Areas.AsQueryable());
            }
            if (_publishedItem.ServiceNames != null)
            {
                ServiceNameRepoMock.Setup(g => g.All()).Returns(_publishedItem.ServiceNames.AsQueryable());
            }
            if (_publishedItem.ServiceDescriptions != null)
            {
                ServiceDescriptionRepoMock.Setup(g => g.All()).Returns(_publishedItem.ServiceDescriptions.AsQueryable());
            }
            
            // Act
            var result = service.GetServiceById(_publishedRootItemId, 10, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vm = Assert.IsType<V10VmOpenApiService>(result);
            vm.Type.Should().Be(serviceType);
            vm.ServiceChargeType.Should().Be(serviceChargeType);
            vm.ServiceNames.Should().NotBeNullOrEmpty();
            vm.ServiceNames.Count.Should().Be(1);
            vm.ServiceNames.First().Type.Should().Be("AlternativeName");
            vm.ServiceDescriptions.Should().NotBeNullOrEmpty();
            vm.ServiceDescriptions.Count.Should().Be(8);
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "Description").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "Summary").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "UserInstruction").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ValidityTime").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ProcessingTime").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "DeadLine").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ChargeTypeAdditionalInfo").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ServiceType").Should().NotBeNull();
            vm.AreaType.Should().Be(areaInformationType);
            if (areaInformationType == "LimitedType")
            {
                vm.Areas.Should().NotBeNull();
                vm.Areas.Count.Should().BeGreaterThan(0);
                vm.Areas.FirstOrDefault(a => a.Type == "Municipality").Should().NotBeNull();
                vm.Areas.FirstOrDefault(a => a.Type == "Region").Should().NotBeNull();
                vm.Areas.FirstOrDefault(a => a.Type == "BusinessSubRegion").Should().NotBeNull();
                vm.Areas.FirstOrDefault(a => a.Type == "HospitalDistrict").Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData("PermitOrObligation", "Chargeable", "Nationwide")]
        [InlineData("ProfessionalQualification", "FreeOfCharge", "NationwideExceptAlandIslands")]
        [InlineData("Service", "Other", "LimitedType")]
        public void InterfaceVersion9CanBeFetched(string serviceType, string serviceChargeType, string areaInformationType)
        {
            // Arrange
            var service = Arrange();
            _publishedItem.TypeId = TypeCache.Get<ServiceType>(serviceType.GetEnumValueByOpenApiEnumValue<ServiceTypeEnum>());
            _publishedItem.ChargeTypeId = TypeCache.Get<ServiceChargeType>(serviceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>());
            _publishedItem.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(areaInformationType.GetEnumValueByOpenApiEnumValue<AreaInformationTypeEnum>());
            _publishedItem.Areas = areaInformationType == "LimitedType" ? new List<ServiceArea>
            {
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString())}, ServiceVersionedId = _publishedItem.Id },
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString())}, ServiceVersionedId = _publishedItem.Id },
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Municipality.ToString())}, ServiceVersionedId = _publishedItem.Id },
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Province.ToString())}, ServiceVersionedId = _publishedItem.Id }
            } : null;
            if (_publishedItem.Areas != null)
            {
                ServiceAreaRepoMock.Setup(g => g.All()).Returns(_publishedItem.Areas.AsQueryable());
            }
            if (_publishedItem.ServiceNames != null)
            {
                ServiceNameRepoMock.Setup(g => g.All()).Returns(_publishedItem.ServiceNames.AsQueryable());
            }
            if (_publishedItem.ServiceDescriptions != null)
            {
                ServiceDescriptionRepoMock.Setup(g => g.All()).Returns(_publishedItem.ServiceDescriptions.AsQueryable());
            }

            // Act
            var result = service.GetServiceById(_publishedRootItemId, 9, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vm = Assert.IsType<V9VmOpenApiService>(result);
            vm.Type.Should().Be(serviceType);
            vm.ServiceChargeType.Should().Be(serviceChargeType);
            vm.ServiceNames.Should().NotBeNullOrEmpty();
            vm.ServiceNames.Count.Should().Be(1);
            vm.ServiceNames.First().Type.Should().Be("AlternativeName");
            vm.ServiceDescriptions.Should().NotBeNullOrEmpty();
            vm.ServiceDescriptions.Count.Should().Be(8);
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "Description").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "Summary").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "UserInstruction").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ValidityTime").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ProcessingTime").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "DeadLine").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ChargeTypeAdditionalInfo").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ServiceType").Should().NotBeNull();
            vm.AreaType.Should().Be(areaInformationType);
            if (areaInformationType == "LimitedType")
            {
                vm.Areas.Should().NotBeNull();
                vm.Areas.Count.Should().BeGreaterThan(0);
                vm.Areas.FirstOrDefault(a => a.Type == "Municipality").Should().NotBeNull();
                vm.Areas.FirstOrDefault(a => a.Type == "Region").Should().NotBeNull();
                vm.Areas.FirstOrDefault(a => a.Type == "BusinessSubRegion").Should().NotBeNull();
                vm.Areas.FirstOrDefault(a => a.Type == "HospitalDistrict").Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData("PermitOrObligation", "Chargeable", "Nationwide")]
        [InlineData("ProfessionalQualification", "FreeOfCharge", "NationwideExceptAlandIslands")]
        [InlineData("Service", "Other", "LimitedType")]
        public void InterfaceVersion8CanBeFetched(string serviceType, string serviceChargeType, string areaInformationType)
        {
            // Arrange
            var service = Arrange();
            _publishedItem.TypeId = TypeCache.Get<ServiceType>(serviceType.GetEnumValueByOpenApiEnumValue<ServiceTypeEnum>());
            _publishedItem.ChargeTypeId = TypeCache.Get<ServiceChargeType>(serviceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>());
            _publishedItem.AreaInformationTypeId = TypeCache.Get<AreaInformationType>(areaInformationType.GetEnumValueByOpenApiEnumValue<AreaInformationTypeEnum>());
            _publishedItem.Areas = areaInformationType == "LimitedType" ? new List<ServiceArea>
            {
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString())}, ServiceVersionedId = _publishedItem.Id },
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString())}, ServiceVersionedId = _publishedItem.Id },
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Municipality.ToString())}, ServiceVersionedId = _publishedItem.Id },
                new ServiceArea{ Area = new Area{ AreaTypeId = TypeCache.Get<AreaType>(AreaTypeEnum.Province.ToString())}, ServiceVersionedId = _publishedItem.Id }
            } : null;
            if (_publishedItem.Areas != null)
            {
                ServiceAreaRepoMock.Setup(g => g.All()).Returns(_publishedItem.Areas.AsQueryable());
            }
            if (_publishedItem.ServiceNames != null)
            {
                ServiceNameRepoMock.Setup(g => g.All()).Returns(_publishedItem.ServiceNames.AsQueryable());
            }
            if (_publishedItem.ServiceDescriptions != null)
            {
                ServiceDescriptionRepoMock.Setup(g => g.All()).Returns(_publishedItem.ServiceDescriptions.AsQueryable());
            }

            // Act
            var result = service.GetServiceById(_publishedRootItemId, 8, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vm = Assert.IsType<V8VmOpenApiService>(result);
            vm.Type.Should().Be(serviceType);
            vm.ServiceChargeType.Should().Be(serviceChargeType);
            vm.ServiceNames.Should().NotBeNullOrEmpty();
            vm.ServiceNames.Count.Should().Be(1);
            vm.ServiceNames.First().Type.Should().Be("AlternativeName");
            vm.ServiceDescriptions.Should().NotBeNullOrEmpty();
            vm.ServiceDescriptions.Count.Should().Be(8);
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "Description").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "Summary").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "UserInstruction").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ValidityTime").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ProcessingTime").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "DeadLine").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ChargeTypeAdditionalInfo").Should().NotBeNull();
            vm.ServiceDescriptions.FirstOrDefault(d => d.Type == "ServiceType").Should().NotBeNull();
            vm.AreaType.Should().Be(areaInformationType);
            if (areaInformationType == "LimitedType")
            {
                vm.Areas.Should().NotBeNull();
                vm.Areas.Count.Should().BeGreaterThan(0);
                vm.Areas.FirstOrDefault(a => a.Type == "Municipality").Should().NotBeNull();
                vm.Areas.FirstOrDefault(a => a.Type == "Region").Should().NotBeNull();
                vm.Areas.FirstOrDefault(a => a.Type == "BusinessSubRegion").Should().NotBeNull();
                vm.Areas.FirstOrDefault(a => a.Type == "HospitalDistrict").Should().NotBeNull();
            }
        }

        [Fact]
        public void InterfaceVersion7CannotBeFetched()
        {
            // Arrange
            var service = Arrange();

            // Act
            Action act = () => service.GetServiceById(_publishedRootItemId, 7, VersionStatusEnum.Published);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData(ServiceTypeEnum.PermissionAndObligation)]
        [InlineData(ServiceTypeEnum.ProfessionalQualifications)]
        [InlineData(ServiceTypeEnum.Service)]
        public void InterfaceVersion6CannotBeFetched(ServiceTypeEnum type)
        {
            // Arrange
            var service = Arrange();
            _publishedItem.TypeId = TypeCache.Get<ServiceType>(type.ToString());

            // Act
            Action act = () => service.GetServiceById(_publishedRootItemId, 6, VersionStatusEnum.Published);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData(ServiceTypeEnum.PermissionAndObligation)]
        [InlineData(ServiceTypeEnum.ProfessionalQualifications)]
        [InlineData(ServiceTypeEnum.Service)]
        public void InterfaceVersion5CannotBeFetched(ServiceTypeEnum type)
        {
            // Arrange
            var service = Arrange();
            _publishedItem.TypeId = TypeCache.Get<ServiceType>(type.ToString());

            // Act
            Action act = () => service.GetServiceById(_publishedRootItemId, 5, VersionStatusEnum.Published);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void InterfaceVersion4CannotBeFetched()
        {
            // Arrange
            var service = Arrange();

            // Act
            Action act = () => service.GetServiceById(_publishedRootItemId, 4, VersionStatusEnum.Published);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void InterfaceVersion3CannotBeFetched()
        {
            // Arrange
            var service = Arrange();

            // Act
            Action act = () => service.GetServiceById(_publishedRootItemId, 3, VersionStatusEnum.Published);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void NoLanguageVersionsPublished()
        {
            // Arrange
            var list = EntityGenerator.GetServiceEntityList(1, PublishingStatusCache);
            // set available languages to empty
            list.ForEach(o => o.LanguageAvailabilities = new List<ServiceLanguageAvailability>());
            var publishedOrganization = list.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            var service = Arrange(list);

            // Act
            var result = service.GetServiceById(publishedOrganization.UnificRootId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().BeNull();
            ServiceRepoMock.Verify(x => x.All(), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceVersioned, VmOpenApiServiceVersionBase>(It.IsAny<ServiceVersioned>()), Times.Never);
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void GetLatestService(PublishingStatus publishingStatus)
        {
            // Arrange
            var item = _list.Where(i => i.PublishingStatusId == PublishingStatusCache.Get(publishingStatus)).FirstOrDefault();
            var rootId = item.UnificRootId;
            var id = item.Id;
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWorkMockSetup.Object, rootId, null, false)).Returns(id);
            var service = Arrange();

            // Act
            var result = service.GetServiceById(rootId, DefaultVersion, VersionStatusEnum.Latest);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(publishingStatus.ToString());
            VersioningManagerMock.Verify(x => x.GetVersionId<ServiceVersioned>(unitOfWorkMockSetup.Object, rootId, null, false), Times.Once);
        }

        [Theory]
        [InlineData(PublishingStatus.Published)]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void GetLatestActiveService(PublishingStatus publishingStatus)
        {
            // Arrange
            var item = _list.Where(i => i.PublishingStatusId == PublishingStatusCache.Get(publishingStatus)).FirstOrDefault();
            var rootId = item.UnificRootId;
            var id = item.Id;
            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWorkMockSetup.Object, rootId, null, true))
                .Returns(() =>
                {
                    if (publishingStatus == PublishingStatus.Deleted || publishingStatus == PublishingStatus.OldPublished) return null;

                    return id;
                });
            var service = Arrange();

            // Act
            var result = service.GetServiceById(rootId, DefaultVersion, VersionStatusEnum.LatestActive);

            // Assert
            // Method should only return draft, modified or published versions.
            VersioningManagerMock.Verify(x => x.GetVersionId<ServiceVersioned>(unitOfWorkMockSetup.Object, rootId, null, true), Times.Once);
            if (publishingStatus == PublishingStatus.Draft || publishingStatus == PublishingStatus.Modified || publishingStatus == PublishingStatus.Published)
            {
                result.Should().NotBeNull();
                var vmResult = Assert.IsType<V11VmOpenApiService>(result);
                vmResult.PublishingStatus.Should().Be(publishingStatus.ToString());
            }
            else
            {
                result.Should().BeNull();
            }
        }

        [Fact]
        public void CanGetMainOrganization()
        {
            // Set organization data
            var orgName = "Name";
            var publishedOrganization = GetAndSetOrganizationForService(_publishedItem, PublishedId, orgName, null, true, false, false);

            OrganizationVersionedRepoMock.Setup(o => o.All()).Returns((new List<OrganizationVersioned> { publishedOrganization }).AsQueryable());

            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Organizations.Should().NotBeNullOrEmpty();
            var resultMainOrg = vmResult.Organizations.First();
            resultMainOrg.Should().NotBeNull();
            resultMainOrg.RoleType.Should().Be("Responsible");
            resultMainOrg.Organization.Id.Should().Be(publishedOrganization.UnificRootId);
            resultMainOrg.Organization.Name.Should().Be(orgName);
        }

        [Fact]
        public void CanGetOtherResponsibleOrganizations()
        {
            // Set organization data
            var orgName = "Name";
            var publishedOrganization = GetAndSetOrganizationForService(_publishedItem, PublishedId, orgName, null, false, true, false);

            OrganizationVersionedRepoMock.Setup(o => o.All()).Returns((new List<OrganizationVersioned> { publishedOrganization }).AsQueryable());

            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Organizations.Should().NotBeNullOrEmpty();
            var resultOrg = vmResult.Organizations.First();
            resultOrg.Should().NotBeNull();
            resultOrg.RoleType.Should().Be("OtherResponsible");
            resultOrg.Organization.Id.Should().Be(publishedOrganization.UnificRootId);
            resultOrg.Organization.Name.Should().Be(orgName);
        }

        [Fact]
        public void CannotGetOtherResponsibleOrganizationsV6()
        {
            // Set organization data
            var orgName = "Name";
            var publishedOrganization = GetAndSetOrganizationForService(_publishedItem, PublishedId, orgName, null, false, true, false);

            OrganizationVersionedRepoMock.Setup(o => o.All()).Returns((new List<OrganizationVersioned> { publishedOrganization }).AsQueryable());

            var service = Arrange();

            // Act
            Action act = () => service.GetServiceById(_publishedRootItemId, 6, VersionStatusEnum.Published);

            // Assert
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void CanGetProducerOrganizations()
        {
            // Set organization data
            var orgName = "Name";
            var publishedOrganization = GetAndSetOrganizationForService(_publishedItem, PublishedId, orgName, null, false, false, true);

            OrganizationVersionedRepoMock.Setup(o => o.All()).Returns((new List<OrganizationVersioned> { publishedOrganization }).AsQueryable());

            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Organizations.Should().NotBeNullOrEmpty();
            var resultOrg = vmResult.Organizations.First();
            resultOrg.Should().NotBeNull();
            resultOrg.RoleType.Should().Be("Producer");
            resultOrg.Organization.Id.Should().Be(publishedOrganization.UnificRootId);
            resultOrg.Organization.Name.Should().Be(orgName);
            vmResult.Organizations.FirstOrDefault(o => o.ProvisionType == "SelfProducedServices").Should().NotBeNull();
            vmResult.Organizations.FirstOrDefault(o => o.ProvisionType == "ProcuredServices").Should().NotBeNull();
            vmResult.Organizations.FirstOrDefault(o => o.ProvisionType == "Other").Should().NotBeNull();
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void NotPublishedOrganizationsNotReturned(PublishingStatus publishingStatus)
        {
            // Set organization data
            var organization = GetAndSetOrganizationForService(_publishedItem, PublishingStatusCache.Get(publishingStatus), "Name");

            OrganizationVersionedRepoMock.Setup(o => o.All()).Returns((new List<OrganizationVersioned> { organization }).AsQueryable());

            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Organizations.Should().BeNullOrEmpty();
        }

        [Fact]
        public void CanGetPublishedServiceChannels()
        {
            // Arrange
            // Set channel data
            var name = "Name";

            var connection = GetAndSetConnectionForService(_publishedItem, PublishedId, name);

            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceChannelRepoMock.Setup(x => x.All()).Returns(connection.ServiceChannel.Versions.ToList().AsQueryable());
            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.ServiceChannels.Should().NotBeNullOrEmpty();
            var resultConnection = vmResult.ServiceChannels.First();
            resultConnection.Should().NotBeNull();
            resultConnection.ServiceChannel.Should().NotBeNull();
            resultConnection.ServiceChannel.Name.Should().Be(name);
            resultConnection.ContactDetails.Should().NotBeNull();
            resultConnection.ContactDetails.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "FreeOfCharge").Should().NotBeNull();
            resultConnection.ContactDetails.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "Chargeable").Should().NotBeNull();
            resultConnection.ContactDetails.PhoneNumbers.FirstOrDefault(p => p.ServiceChargeType == "Other").Should().NotBeNull();
        }

        [Theory]
        [InlineData(PublishingStatus.Deleted)]
        [InlineData(PublishingStatus.Draft)]
        [InlineData(PublishingStatus.Modified)]
        [InlineData(PublishingStatus.OldPublished)]
        public void NotPublishedServiceChannelsNotReturned(PublishingStatus publishingStatus)
        {
            // Arrange
            // Set channel data
            var statusId = PublishingStatusCache.Get(publishingStatus);

            var connection = GetAndSetConnectionForService(_publishedItem, statusId, "Name");

            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceChannelRepoMock.Setup(x => x.All()).Returns(connection.ServiceChannel.Versions.ToList().AsQueryable());
            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.ServiceChannels.Should().BeNullOrEmpty();
        }

        [Fact]
        public void OnlyPublishedNamesReturned()
        {
            // Arrange
            // Set channel data
            var fiName = "Finnish name";
            var svName = "Swedish name";
            var fiId = LanguageCache.Get("fi");
            var svId = LanguageCache.Get("sv");
            // Add only swedish language as published version
            var connection = GetAndSetConnectionForService(_publishedItem, PublishedId, svName, svId);
            var channel = connection.ServiceChannel.Versions.First();
            channel.ServiceChannelNames.Add(new ServiceChannelName { Name = fiName, LocalizationId = fiId });

            ConnectionRepoMock.Setup(x => x.All()).Returns((new List<ServiceServiceChannel> { connection }).AsQueryable());
            ServiceChannelRepoMock.Setup(x => x.All()).Returns(connection.ServiceChannel.Versions.ToList().AsQueryable());
            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.ServiceChannels.Should().NotBeNullOrEmpty();
            var resultConnection = vmResult.ServiceChannels.First();
            resultConnection.Should().NotBeNull();
            resultConnection.ServiceChannel.Should().NotBeNull();
            resultConnection.ServiceChannel.Name.Should().Be(svName);

        }
/* SOTE has been disabled (SFIPTV-1177)
        [Theory]
        [InlineData(GeneralDescriptionTypeEnum.PrescribedByFreedomOfChoiceAct)]
        [InlineData(GeneralDescriptionTypeEnum.OtherPermissionGrantedSote)]
        public void GdDataIsAttached(GeneralDescriptionTypeEnum subType)
        {
            // Arrange
            var gdId = Guid.NewGuid();
            var gd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                Type = ServiceTypeEnum.PermissionAndObligation.ToString(),
                GeneralDescriptionType = subType.ToString(),
                TargetGroups = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem { Id = Guid.NewGuid()} },
                ServiceClasses = new List<V7VmOpenApiFintoItemWithDescription> {  new V7VmOpenApiFintoItemWithDescription { Id = Guid.NewGuid()} },
                OntologyTerms = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem { Id = Guid.NewGuid() } },
                PublishingStatus = PublishingStatus.Published.ToString()
            };

            _publishedItem.StatutoryServiceGeneralDescriptionId = gdId;
            gdServiceMock.Setup(x => x.GetGeneralDescriptionSimple(unitOfWorkMockSetup.Object, gdId)).Returns(gd);
            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V10VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Type.Should().Be(ServiceTypeEnum.PermissionAndObligation.ToString());
            vmResult.SubType.Should().Be(subType.ToString());
            vmResult.TargetGroups.Count.Should().Be(1);
            vmResult.ServiceClasses.Count.Should().Be(1);
            vmResult.OntologyTerms.Count.Should().Be(1);
        }
*/
        [Theory]
        [InlineData(GeneralDescriptionTypeEnum.AlandIsland)]
        [InlineData(GeneralDescriptionTypeEnum.BusinessSubregion)]
        [InlineData(GeneralDescriptionTypeEnum.Municipality)]
        public void GdSubTypeOtherThanSoteNotAttached(GeneralDescriptionTypeEnum subType)
        {
            // Arrange
            var gdId = Guid.NewGuid();
            var gd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                GeneralDescriptionType = subType.ToString(),
            };

            _publishedItem.StatutoryServiceGeneralDescriptionId = gdId;
            gdServiceMock.Setup(x => x.GetGeneralDescriptionSimple(unitOfWorkMockSetup.Object, gdId)).Returns(gd);
            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, DefaultVersion, VersionStatusEnum.Published);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.SubType.Should().BeNull();
        }

        [Theory]
        [InlineData(GeneralDescriptionTypeEnum.AlandIsland)]
        [InlineData(GeneralDescriptionTypeEnum.BusinessSubregion)]
        [InlineData(GeneralDescriptionTypeEnum.Municipality)]

/* SOTE has been disabled (SFIPTV-1177)
        [InlineData(GeneralDescriptionTypeEnum.PrescribedByFreedomOfChoiceAct)]
        [InlineData(GeneralDescriptionTypeEnum.OtherPermissionGrantedSote)]
*/
        public void FullGdDataIsAttached(GeneralDescriptionTypeEnum subType)
        {
            // Arrange
            var fi = "fi";
            var descriptionType = DescriptionTypeEnum.Description.ToString();
            var gdDescriptionValue = "Text";
            var gdId = Guid.NewGuid();
            var gd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                Type = ServiceTypeEnum.PermissionAndObligation.ToString(),
                GeneralDescriptionType = subType.ToString(),
                ServiceChargeType = ServiceChargeTypeEnum.Charged.ToString(),
                Descriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem {
                    Language = fi, Type = descriptionType, Value = gdDescriptionValue } },
                TargetGroups = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem { Id = Guid.NewGuid() } },
                ServiceClasses = new List<V7VmOpenApiFintoItemWithDescription> { new V7VmOpenApiFintoItemWithDescription { Id = Guid.NewGuid() } },
                OntologyTerms = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem { Id = Guid.NewGuid() } },
                LifeEvents = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem { Id = Guid.NewGuid() } },
                IndustrialClasses = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem { Id = Guid.NewGuid() } },
                Requirements = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Id = Guid.NewGuid()} },
                Legislation = new List<V4VmOpenApiLaw> { new V4VmOpenApiLaw { Id = Guid.NewGuid()} },
                PublishingStatus = PublishingStatus.Published.ToString()
            };

            _publishedItem.StatutoryServiceGeneralDescriptionId = gdId;
            gdServiceMock.Setup(x => x.GetPublishedGeneralDescriptionWithDetails(unitOfWorkMockSetup.Object, gdId, false)).Returns(gd);
            var service = Arrange();

            // Act
            var result = service.GetServiceById(_publishedRootItemId, DefaultVersion, VersionStatusEnum.Published, true);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V11VmOpenApiService>(result);
            vmResult.PublishingStatus.Should().Be(PublishingStatus.Published.ToString());
            vmResult.Type.Should().Be(ServiceTypeEnum.PermissionAndObligation.ToString());
            vmResult.SubType.Should().Be(subType.ToString());
            vmResult.ServiceChargeType.Should().Be(ServiceChargeTypeEnum.Charged.ToString());
            var vmDescription = vmResult.ServiceDescriptions.FirstOrDefault(d => d.Type == "GD_" + descriptionType);
            vmDescription.Should().NotBeNull();
            vmDescription.Value.Should().Be(gdDescriptionValue);
            vmResult.TargetGroups.Count.Should().Be(1);
            vmResult.ServiceClasses.Count.Should().Be(1);
            vmResult.OntologyTerms.Count.Should().Be(1);
            vmResult.LifeEvents.Count.Should().Be(1);
            vmResult.IndustrialClasses.Count.Should().Be(1);
            vmResult.Requirements.Count.Should().Be(1);
            vmResult.Legislation.Count.Should().Be(1);
        }


        private ServiceService Arrange(List<ServiceVersioned> list = null)
        {
            var serviceList = list ?? _list;
            var publishedItem = serviceList.Where(o => o.PublishingStatusId == PublishedId).FirstOrDefault();
            var id = publishedItem.Id;
            var rootId = publishedItem.UnificRootId;

            // Service
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceVersioned> services, Func<IQueryable<ServiceVersioned>, IQueryable<ServiceVersioned>> func, bool applyFilters) =>
               {
                   return services;
               });

            // Related organizations
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<OrganizationVersioned>>(),
               It.IsAny<Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<OrganizationVersioned> orgs, Func<IQueryable<OrganizationVersioned>, IQueryable<OrganizationVersioned>> func, bool applyFilters) =>
               {
                   return orgs;
               });

            // Channel connections
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceServiceChannel>>(),
               It.IsAny<Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceServiceChannel> items, Func<IQueryable<ServiceServiceChannel>, IQueryable<ServiceServiceChannel>> func, bool applyFilters) =>
               {
                   return items;
               });

            // Related channels
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<ServiceChannelVersioned>>(),
               It.IsAny<Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<ServiceChannelVersioned> items, Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> func, bool applyFilters) =>
               {
                   return items;
               });

            // Related service collections
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
               It.IsAny<IQueryable<Model.Models.ServiceCollectionService>>(),
               It.IsAny<Func<IQueryable<Model.Models.ServiceCollectionService>, IQueryable<Model.Models.ServiceCollectionService>>>(),
               It.IsAny<bool>()
               )).Returns((IQueryable<Model.Models.ServiceCollectionService> items, Func<IQueryable<Model.Models.ServiceCollectionService>, IQueryable<Model.Models.ServiceCollectionService>> func, bool applyFilters) =>
               {
                   return items;
               });

            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);

            var serviceUtilities = new ServiceUtilities(UserIdentification, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserOrganizationChecker, CacheManagerMock.Object);

            ArrangeTranslateService();

            var translationManagerMock = translationManagerMockSetup.Object;

            VersioningManagerMock.Setup(s => s.GetVersionId<ServiceVersioned>(unitOfWork, rootId, PublishingStatus.Published, true)).Returns(id);

            // repositories
            ServiceRepoMock.Setup(g => g.All()).Returns(serviceList.AsQueryable());

            return new ServiceService(contextManager, translationManagerMock, TranslationManagerVModel, Logger, serviceUtilities,
                CommonService, CacheManager.TypesCache, LanguageCache, PublishingStatusCache,
                VersioningManager, gdService, UserOrganizationChecker, LanguageOrderCache, targetGroupDataCache, null, null);
        }
    }
}
