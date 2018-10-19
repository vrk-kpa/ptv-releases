﻿/**
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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Channel
{
    public class AddServiceChannelTests : ChannelServiceTestBase
    {
        const string fi = "fi";
        const string sourceId = "sourceId";

        private ServiceChannelVersioned publishedEntity;
        private ChannelService _service;

        public AddServiceChannelTests()
        {
            SetupTypesCacheMock<ServiceChannelType>();
            SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            SetupTypesCacheMock<DescriptionType>(typeof(DescriptionTypeEnum));

            publishedEntity = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(PublishingStatusCache.Get(PublishingStatus.Published));
            
            var unitOfWork = unitOfWorkMockSetup.Object;
            var contextManager = new TestContextManager(unitOfWork, unitOfWork);
            var serviceUtilities = new ServiceUtilities(UserIdentificationMock.Object, LockingManager, contextManager, UserOrganizationService,
                VersioningManager, UserInfoService, UserOrganizationChecker, CacheManagerMock.Object);

            _service = new ChannelService(contextManager, translationManagerMockSetup.Object, TranslationManagerVModel,
                Logger, serviceUtilities, CommonService, AddressService, CacheManager, PublishingStatusCache, VersioningManager, UserOrganizationChecker, LanguageOrderCache, CloningManager);
        }

        [Fact]
        public void CannotGetUser()
        {
            // Arrange
            UserIdentificationMock.Setup(s => s.UserName).Returns((string)null);
            
            // Act
            Action act = () => _service.AddServiceChannel(new VmOpenApiServiceChannelIn(), false, DefaultVersion, null);

            // Assert
            act.ShouldThrowExactly<Exception>(CoreMessages.OpenApi.RelationIdNotFound);
        }

        [Fact]
        public void ExternalSourceExists()
        {
            // Arrange
            var sourceId = "sourceId";
            UserIdentificationMock.Setup(s => s.UserName).Returns(USERNAME);
            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { SourceId = sourceId, RelationId = USERNAME, ObjectType = typeof(Model.Models.ServiceChannel).Name }
                }.AsQueryable());
            
            // Act
            Action act = () => _service.AddServiceChannel(new VmOpenApiServiceChannelIn() { SourceId = sourceId }, false, DefaultVersion, null);

            // Assert
            act.ShouldThrowExactly<ExternalSourceExistsException>(string.Format(CoreMessages.OpenApi.ExternalSourceExists, sourceId));
        }

        [Fact]
        public void CanAddElectronicChannel()
        {
            // Arrange
            var vm = new VmOpenApiElectronicChannelInVersionBase
            {
                SourceId = sourceId,
                PublishingStatus = PublishingStatus.Published.ToString()
            };

            var service = Arrange(vm, ServiceChannelTypeEnum.EChannel);

            // Act
            var result = service.AddServiceChannel(vm, false, DefaultVersion, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V9VmOpenApiElectronicChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<VmOpenApiElectronicChannelInVersionBase, ServiceChannelVersioned>(It.IsAny<VmOpenApiElectronicChannelInVersionBase>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void CanAddPhoneChannel()
        {
            // Arrange
            var vm = new VmOpenApiPhoneChannelInVersionBase
            {
                SourceId = sourceId,
                PublishingStatus = PublishingStatus.Published.ToString()
            };

            var service = Arrange(vm, ServiceChannelTypeEnum.Phone);

            // Act
            var result = service.AddServiceChannel(vm, false, DefaultVersion, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V9VmOpenApiPhoneChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<VmOpenApiPhoneChannelInVersionBase, ServiceChannelVersioned>(It.IsAny<VmOpenApiPhoneChannelInVersionBase>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void CanAddPrintableFormChannel()
        {
            // Arrange
            var vm = new VmOpenApiPrintableFormChannelInVersionBase
            {
                SourceId = sourceId,
                PublishingStatus = PublishingStatus.Published.ToString()
            };

            var service = Arrange(vm, ServiceChannelTypeEnum.PrintableForm);

            // Act
            var result = service.AddServiceChannel(vm, false, DefaultVersion, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V9VmOpenApiPrintableFormChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<VmOpenApiPrintableFormChannelInVersionBase, ServiceChannelVersioned>(It.IsAny<VmOpenApiPrintableFormChannelInVersionBase>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void CanAddServiceLocationChannel()
        {
            // Arrange
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                SourceId = sourceId,
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var service = Arrange(vm, ServiceChannelTypeEnum.ServiceLocation);
            
            // Act
            var result = service.AddServiceChannel(vm, false, DefaultVersion, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V9VmOpenApiServiceLocationChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<VmOpenApiServiceLocationChannelInVersionBase, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceLocationChannelInVersionBase>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void CanAddWebPageChannel()
        {
            // Arrange
            var vm = new VmOpenApiWebPageChannelInVersionBase
            {
                SourceId = sourceId,
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var service = Arrange(vm, ServiceChannelTypeEnum.WebPage);

            // Act
            var result = service.AddServiceChannel(vm, false, DefaultVersion, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<V9VmOpenApiWebPageChannel>();
            translationManagerVModelMockSetup.Verify(x => x.Translate<VmOpenApiWebPageChannelInVersionBase, ServiceChannelVersioned>(It.IsAny<VmOpenApiWebPageChannelInVersionBase>(), unitOfWorkMockSetup.Object), Times.Once());
            //translationManagerVModelMockSetup.Verify(x => x.Translate<VmOpenApiWebPageChannelInVersionBase, WebpageChannel>(It.IsAny<VmOpenApiWebPageChannelInVersionBase>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
        }

        [Fact]
        public void PrintableFormChannel_MunicipalityAutomaticallySet()
        {
            // Arrange
            var postalCode = "00100";
            var municipalityCode = "049";
            var item = new PostalCode { Code = postalCode, IsValid = true, Municipality = new Municipality { Code = municipalityCode, IsValid = true } };
            PostalCodeRepositoryMock.Setup(x => x.All()).Returns((new List<PostalCode> { item }).AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
            It.IsAny<IQueryable<PostalCode>>(),
            It.IsAny<Func<IQueryable<PostalCode>, IQueryable<PostalCode>>>(),
            It.IsAny<bool>()
            )).Returns((IQueryable<PostalCode> postalCodes, Func<IQueryable<PostalCode>, IQueryable<PostalCode>> func, bool applyFilters) =>
            {
                return postalCodes;
            });
            var vm = new VmOpenApiPrintableFormChannelInVersionBase
            {
                SourceId = sourceId,
                DeliveryAddresses = new List<V8VmOpenApiAddressDeliveryIn> {
                    new V8VmOpenApiAddressDeliveryIn
                    {
                        SubType = AddressTypeEnum.Street.ToString(),
                        StreetAddress = new VmOpenApiAddressStreetIn
                        {
                            Street = new List<VmOpenApiLanguageItem>{ new VmOpenApiLanguageItem { Value = "Street", Language = fi } },
                            PostalCode = postalCode
                        }
                    },
                    new V8VmOpenApiAddressDeliveryIn
                    {
                        SubType = AddressTypeEnum.PostOfficeBox.ToString(),
                        PostOfficeBoxAddress = new VmOpenApiAddressPostOfficeBoxIn
                        {
                            PostOfficeBox = new List<VmOpenApiLanguageItem>{ new VmOpenApiLanguageItem { Value = "PL100", Language = fi } },
                            PostalCode = postalCode
                        }
                    }
                },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var service = Arrange(vm, ServiceChannelTypeEnum.PrintableForm);

            // Act
            var result = service.AddServiceChannel(vm, false, DefaultVersion, null);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V9VmOpenApiPrintableFormChannel>(result);
            PostalCodeRepositoryMock.Verify(x => x.All(), Times.Exactly(2));
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
            vmResult.DeliveryAddresses.Should().NotBeNullOrEmpty();
            vmResult.DeliveryAddresses.ForEach(a =>
            {
                if (a.StreetAddress != null)
                {
                    a.StreetAddress.Municipality.Code.Should().Be(municipalityCode);
                }
                if (a.PostOfficeBoxAddress != null)
                {
                    a.PostOfficeBoxAddress.Municipality.Code.Should().Be(municipalityCode);
                }
            });
        }

        [Fact]
        public void ServiceLocationChannel_MunicipalityAutomaticallySet()
        {
            // Arrange
            var postalCode = "00100";
            var municipalityCode = "049";
            var item = new PostalCode { Code = postalCode, IsValid = true, Municipality = new Municipality { Code = municipalityCode, IsValid = true } };
            PostalCodeRepositoryMock.Setup(x => x.All()).Returns((new List<PostalCode> { item }).AsQueryable());
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
            It.IsAny<IQueryable<PostalCode>>(),
            It.IsAny<Func<IQueryable<PostalCode>, IQueryable<PostalCode>>>(),
            It.IsAny<bool>()
            )).Returns((IQueryable<PostalCode> postalCodes, Func<IQueryable<PostalCode>, IQueryable<PostalCode>> func, bool applyFilters) =>
            {
                return postalCodes;
            });
            var vm = new VmOpenApiServiceLocationChannelInVersionBase
            {
                SourceId = sourceId,
                Addresses = new List<V9VmOpenApiAddressLocationIn> {
                    new V9VmOpenApiAddressLocationIn
                    {
                        SubType = AddressTypeEnum.Street.ToString(),
                        StreetAddress = new VmOpenApiAddressStreetWithCoordinatesIn
                        {
                            Street = new List<VmOpenApiLanguageItem>{ new VmOpenApiLanguageItem { Value = "Street", Language = fi } },
                            PostalCode = postalCode
                        }
                    },
                    new V9VmOpenApiAddressLocationIn
                    {
                        SubType = AddressTypeEnum.PostOfficeBox.ToString(),
                        PostOfficeBoxAddress = new VmOpenApiAddressPostOfficeBoxIn
                        {
                            PostOfficeBox = new List<VmOpenApiLanguageItem>{ new VmOpenApiLanguageItem { Value = "PL100", Language = fi } },
                            PostalCode = postalCode
                        }
                    }
                },
                PublishingStatus = PublishingStatus.Published.ToString()
            };
            var service = Arrange(vm, ServiceChannelTypeEnum.ServiceLocation);

            // Act
            var result = service.AddServiceChannel(vm, false, DefaultVersion, null);

            // Assert
            result.Should().NotBeNull();
            var vmResult = Assert.IsType<V9VmOpenApiServiceLocationChannel>(result);
            PostalCodeRepositoryMock.Verify(x => x.All(), Times.Exactly(2));
            translationManagerVModelMockSetup.Verify(x => x.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(It.IsAny<VmOpenApiServiceChannelIn>(), unitOfWorkMockSetup.Object), Times.Once());
            translationManagerMockSetup.Verify(x => x.Translate<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(It.IsAny<ServiceChannelVersioned>()), Times.Once());
            CommonServiceMock.Verify(x => x.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()), Times.Once());
            vmResult.Addresses.Should().NotBeNullOrEmpty();
            vmResult.Addresses.ForEach(a =>
            {
                if (a.StreetAddress != null)
                {
                    a.StreetAddress.Municipality.Code.Should().Be(municipalityCode);
                }
                if (a.PostOfficeBoxAddress != null)
                {
                    a.PostOfficeBoxAddress.Municipality.Code.Should().Be(municipalityCode);
                }
            });
        }

        private ChannelService Arrange(IVmOpenApiServiceChannelIn vm, ServiceChannelTypeEnum channelType)
        {
            var unitOfWork = unitOfWorkMockSetup.Object;

            ExternalSourceRepoMock.Setup(s => s.All())
                .Returns(new List<ExternalSource>()
                {
                    new ExternalSource { SourceId = sourceId + "2", RelationId = USERNAME, ObjectType = typeof(ServiceChannel).Name }
                }.AsQueryable()); // does not return same source id
            
            translationManagerVModelMockSetup.Setup(t => t.Translate<IVmOpenApiServiceChannelIn, ServiceChannelVersioned>(vm, unitOfWork))
                .Returns((IVmOpenApiServiceChannelIn vmChannel, IUnitOfWorkWritable uw) =>
                {
                    if (vm is VmOpenApiPrintableFormChannelInVersionBase)
                    {
                        (vm as VmOpenApiPrintableFormChannelInVersionBase).DeliveryAddresses.ForEach(a =>
                        {
                            var address = new Address();
                            if (a.StreetAddress != null)
                            {
                                address.AddressStreets.Add(new AddressStreet
                                {
                                    PostalCode = new PostalCode { Code = a.StreetAddress.PostalCode },
                                    Municipality = new Municipality { Code = a.StreetAddress.Municipality }
                                });
                            }
                            if (a.PostOfficeBoxAddress != null)
                            {
                                address.AddressPostOfficeBoxes.Add(new AddressPostOfficeBox
                                {
                                    PostalCode = new PostalCode { Code = a.PostOfficeBoxAddress.PostalCode },
                                    Municipality = new Municipality { Code = a.PostOfficeBoxAddress.Municipality }
                                });
                            }
                            publishedEntity.Addresses.Add(new ServiceChannelAddress { Address = address });
                        });
                    }
                    else if (vm is VmOpenApiServiceLocationChannelInVersionBase)
                    {
                        (vm as VmOpenApiServiceLocationChannelInVersionBase).Addresses.ForEach(a =>
                        {
                            var address = new Address();
                            if (a.StreetAddress != null)
                            {
                                address.AddressStreets.Add(new AddressStreet
                                {
                                    PostalCode = new PostalCode { Code = a.StreetAddress.PostalCode },
                                    Municipality = new Municipality { Code = a.StreetAddress.Municipality }
                                });
                            }
                            if (a.PostOfficeBoxAddress != null)
                            {
                                address.AddressPostOfficeBoxes.Add(new AddressPostOfficeBox
                                {
                                    PostalCode = new PostalCode { Code = a.PostOfficeBoxAddress.PostalCode },
                                    Municipality = new Municipality { Code = a.PostOfficeBoxAddress.Municipality }
                                });
                            }
                            publishedEntity.Addresses.Add(new ServiceChannelAddress { Address = address });
                        });
                    }
                    return publishedEntity;
                });
            translationManagerVModelMockSetup.Setup(t => t.TranslateAll<VmOpenApiConnection, ServiceServiceChannel>(It.IsAny<List<VmOpenApiConnection>>(), unitOfWork))
                .Returns(new List<ServiceServiceChannel>());

            CommonServiceMock.Setup(s => s.PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(publishedEntity.Id, It.IsAny<Expression<Func<ServiceChannelLanguageAvailability, bool>>>()))
                .Returns(new PublishingResult());

            return ArrangeForGet(new List<ServiceChannelVersioned> { publishedEntity }, channelType);
        }
    }
}
