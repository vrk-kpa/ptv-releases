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
using PTV.Application.OpenApi.DataValidators;
using Xunit;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;
using System;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using Moq;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V7;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class ServiceValidatorTests : ValidatorTestBase
    {
        private const string FI = "fi";
        private const string TEXT = "text";

        private List<string> _availableLanguages;

        private int _defaultVersion = 11;

        public ServiceValidatorTests()
        {
            _availableLanguages = new List<string> { FI };
        }

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new ServiceValidator(null, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, UserRoleEnum.Eeva, _defaultVersion);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void OnlyPublishingStatusDefined()
        {
            // Arrange & act
            var validator = new ServiceValidator(new VmOpenApiServiceInVersionBase { PublishingStatus = PublishingStatus.Published.ToString() },
                generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("id")] // not valid Guid
        [InlineData("00000000-0000-0000-0000-000000000000")] // empty Guid
        public void GeneralDescriptionSet_NotValid(string gdId)
        {
            // Arrange & act
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId
            };
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("GeneralDescriptionId").Should().BeTrue();
        }

        [Fact]
        public void GeneralDescriptionSet_NotExists()
        {
            // Arrange & act
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString()
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>())).Returns((VmOpenApiGeneralDescriptionVersionBase)null);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("GeneralDescriptionId").Should().BeTrue();
        }

        [Theory]
        [InlineData(DescriptionTypeEnum.Description)]
        [InlineData(DescriptionTypeEnum.ShortDescription)]
        [InlineData(DescriptionTypeEnum.ServiceUserInstruction)]
        [InlineData(DescriptionTypeEnum.ValidityTimeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ProcessingTimeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.DeadLineAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ChargeTypeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ServiceTypeAdditionalInfo)]
        public void GeneralDescriptionSet_DescriptionNotSetWithinGD_NotValid(DescriptionTypeEnum type)
        {
            // Arrange & act
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString(),
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = type.GetOpenApiValue()}
                }
            };
            var vmGd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>())).Returns(vmGd);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceDescriptions").Should().BeTrue();
        }

        [Fact]
        public void GeneralDescriptionSet_DescriptionNotSetWithinGD_Valid()
        {
            // Arrange & act
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString(),
                ServiceNames = new List<VmOpenApiLocalizedListItem>{
                    new VmOpenApiLocalizedListItem { Value = "Name", Language = FI, Type = NameTypeEnum.Name.GetOpenApiValue()} },
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.Description.GetOpenApiValue()}
                }
            };
            var vmGd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>())).Returns(vmGd);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(DescriptionTypeEnum.BackgroundDescription)]
        [InlineData(DescriptionTypeEnum.ServiceUserInstruction)]
        [InlineData(DescriptionTypeEnum.ValidityTimeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ProcessingTimeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.DeadLineAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ChargeTypeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ServiceTypeAdditionalInfo)]
        public void GeneralDescriptionSet_DescriptionSetWithinGD_NotValid(DescriptionTypeEnum type)
        {
            // Arrange & act
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString(),
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = type.GetOpenApiValue()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue()}
                }
            };
            var vmGd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                PublishingStatus = PublishingStatus.Published.ToString(),
                Descriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.BackgroundDescription.GetOpenApiValue()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ServiceUserInstruction.GetOpenApiValue()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ValidityTimeAdditionalInfo.GetOpenApiValue()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ProcessingTimeAdditionalInfo.GetOpenApiValue()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.DeadLineAdditionalInfo.GetOpenApiValue()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ServiceTypeAdditionalInfo.GetOpenApiValue()},
                }
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>())).Returns(vmGd);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceDescriptions").Should().BeTrue();
        }


        [Theory]
        [InlineData(DescriptionTypeEnum.Description)]
        [InlineData(DescriptionTypeEnum.ServiceUserInstruction)]
        [InlineData(DescriptionTypeEnum.ValidityTimeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ProcessingTimeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.DeadLineAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ChargeTypeAdditionalInfo)]
        [InlineData(DescriptionTypeEnum.ServiceTypeAdditionalInfo)]
        public void GeneralDescriptionSet_DescriptionSetWithinGD_Valid(DescriptionTypeEnum type)
        {
            // Arrange & act
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString(),
                ServiceNames = new List<VmOpenApiLocalizedListItem>{
                    new VmOpenApiLocalizedListItem { Value = "Name", Language = FI, Type = NameTypeEnum.Name.GetOpenApiValue()} },
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = type.GetOpenApiValue()},
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue()}
                }
            };
            var vmGd = new VmOpenApiGeneralDescriptionVersionBase
            {
                Id = gdId,
                PublishingStatus = PublishingStatus.Published.ToString(),
                Descriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Value = TEXT, Language = FI, Type = DescriptionTypeEnum.Description.GetOpenApiValue()},
                }
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>())).Returns(vmGd);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelIsValid()
        {
            // Arrange
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                GeneralDescriptionId = gdId.ToString()
            };
            generalDescriptionServiceMockSetup.Setup(s => s.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase { Id = Guid.NewGuid() });
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService,
                channelService, organizationService, UserRoleEnum.Eeva, _defaultVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void TargetGroups_LifeEventsAttached_NotValid(bool targetGroupsSet, bool currentTargetGroupsSet)
        {
            // Arrange
            var targetGroupUri = "uri";
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = targetGroupsSet ? new List<string> { targetGroupUri } : null,
                LifeEvents = new List<string> { "SomeLifeEvent" }
            };
            var vmTargetGroup = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "NotKR1" };
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupUri)).Returns(vmTargetGroup);

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Eeva, _defaultVersion,
                currentTargetGroupsSet ? new VmOpenApiServiceVersionBase { TargetGroups = new List<V4VmOpenApiFintoItem> { vmTargetGroup } } : null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            if (targetGroupsSet)
            {
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(targetGroupUri), Times.Once);
            }
            controller.ModelState.ContainsKey("LifeEvents").Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue("LifeEvents", out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().StartWith("Target group 'Citizens (KR1)' or one of the sub target groups");
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void TargetGroupNotKR2_IndustrialClassesAttached_NotValid(bool targetGroupsSet, bool currentTargetGroupsSet)
        {
            // Arrange
            var targetGroupUri = "uri";
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = targetGroupsSet ? new List<string> { targetGroupUri } : null,
                IndustrialClasses = new List<string> { "SomeIndustrialClass" }
            };
            var vmTargetGroup = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "NotKR2" };
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupUri)).Returns(vmTargetGroup);

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService,  ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Eeva, _defaultVersion,
                currentTargetGroupsSet ? new VmOpenApiServiceVersionBase { TargetGroups = new List<V4VmOpenApiFintoItem> { vmTargetGroup } } : null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            if (targetGroupsSet)
            {
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(targetGroupUri), Times.Once);
            }
            controller.ModelState.ContainsKey("IndustrialClasses").Should().BeTrue();
            ModelStateEntry value;
            controller.ModelState.TryGetValue("IndustrialClasses", out value);
            value.Should().NotBeNull();
            var error = value.Errors.First();
            error.ErrorMessage.Should().StartWith("Target group 'Businesses and non-government organizations (KR2)' or one of the sub target groups");
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void TargetGroups_LifeEvents_IndustrialClasses_Valid(bool targetGroupsSet, bool currentTargetGroupsSet)
        {
            // Arrange
            var targetGroupK1Uri = "KR1";
            var targetGroupK2Uri = "KR2";
            var subTargetGroupK2Uri = "KR2.1";
            var lifeEventList = new List<string> { "SomeLifeEvent" };
            var industrialClassList = new List<string> { "SomeIndustrialClass" };

            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = targetGroupsSet ? new List<string> { targetGroupK1Uri, targetGroupK2Uri, subTargetGroupK2Uri } : null,
                LifeEvents = lifeEventList,
                IndustrialClasses = industrialClassList,
                MainResponsibleOrganization = Guid.NewGuid().ToString()
            };
            var vmTargetGroupKR1 = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "KR1", Uri = targetGroupK1Uri};
            var vmTargetGroupKR2 = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "KR2", Uri = targetGroupK2Uri};
            var vmSubTargetGroupKR2 = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "KR2.1", ParentId = vmTargetGroupKR2.Id, ParentUri = targetGroupK2Uri};
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupK1Uri)).Returns(vmTargetGroupKR1);
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupK2Uri)).Returns(vmTargetGroupKR2);
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(subTargetGroupK2Uri)).Returns(vmSubTargetGroupKR2);
            fintoServiceMockSetup.Setup(x => x.CheckLifeEvents(lifeEventList)).Returns((List<string>)null);
            fintoServiceMockSetup.Setup(x => x.CheckIndustrialClasses(industrialClassList)).Returns((List<string>)null);
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(It.IsAny<Guid>())).Returns(new List<string>());

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Eeva, _defaultVersion,
                currentTargetGroupsSet ? new VmOpenApiServiceVersionBase { TargetGroups = new List<V4VmOpenApiFintoItem> { vmTargetGroupKR1, vmTargetGroupKR2, vmSubTargetGroupKR2 } } : null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            if (targetGroupsSet)
            {
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(targetGroupK1Uri), Times.Once);
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(targetGroupK2Uri), Times.Once);
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(subTargetGroupK2Uri), Times.Once);
            }

            fintoServiceMockSetup.Verify(x => x.CheckLifeEvents(lifeEventList), Times.Once);
            fintoServiceMockSetup.Verify(x => x.CheckIndustrialClasses(industrialClassList), Times.Once);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void TargetGroupsSet_AttachedTargetGroupsNotKR1OrKR2_NotValid(bool lifeEventSet, bool industrialClassSet)
        {
            // Arrange
            var targetGroupUri = "uri";
            var vm = new VmOpenApiServiceInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = new List<string> { targetGroupUri },
                LifeEvents = lifeEventSet ? new List<string> { "SomeLifeEvent" } : null,
                IndustrialClasses = industrialClassSet ? new List<string> { "SomeIndustrialClass" } : null
            };
            var vmTargetGroupKR1 = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "KR1" };
            var vmTargetGroupKR2 = new V4VmOpenApiFintoItem { Id = Guid.NewGuid(), Code = "KR2" };

            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupUri)).Returns(new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "Code" });

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Eeva, _defaultVersion,
                new VmOpenApiServiceVersionBase { TargetGroups = new List<V4VmOpenApiFintoItem> { vmTargetGroupKR1, vmTargetGroupKR2 } });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            if (lifeEventSet)
            {
                controller.ModelState.ContainsKey("LifeEvents").Should().BeTrue();
                ModelStateEntry value;
                controller.ModelState.TryGetValue("LifeEvents", out value);
                value.Should().NotBeNull();
                var error = value.Errors.First();
                error.ErrorMessage.Should().StartWith("Target group 'Citizens (KR1)' or one of the sub target groups");
            }
            if (industrialClassSet)
            {
                controller.ModelState.ContainsKey("IndustrialClasses").Should().BeTrue();
                ModelStateEntry value;
                controller.ModelState.TryGetValue("IndustrialClasses", out value);
                value.Should().NotBeNull();
                var error = value.Errors.First();
                error.ErrorMessage.Should().StartWith("Target group 'Businesses and non-government organizations (KR2)' or one of the sub target groups");
            }
        }

        [Fact]
        public void MainOrganizationExistsButNotOwnOrganization_Eeva()
        {
            // Arrange
            var id = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                MainResponsibleOrganization = id.ToString()
            };

            organizationServiceMockSetup.Setup(o => o.GetAvailableLanguagesForOwnOrganization(id)).Returns(_availableLanguages);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Eeva, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void MainOrganizationExistsButNotOwnOrganization_Pete()
        {
            // Arrange
            var id = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                MainResponsibleOrganization = id.ToString()
            };

            organizationServiceMockSetup.Setup(o => o.GetAvailableLanguagesForOwnOrganization(id)).Throws(new Exception());
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("MainResponsibleOrganization").Should().BeTrue();
        }

        [Fact]
        public void MainOrganizationNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                MainResponsibleOrganization = id.ToString()
            };

            organizationServiceMockSetup.Setup(o => o.GetAvailableLanguagesForOwnOrganization(id)).Returns((List<string>)null);
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Eeva, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("MainResponsibleOrganization").Should().BeTrue();
        }

        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        public void MoreThan10OntologyTerms_NotValid(int version)
        {
            // Arrange
            ontologyTermDataCacheMock.Setup(o => o.HasUris(It.IsAny<List<string>>()))
                .Returns((List<string> list) =>
                {
                    return list;
                });

            var vm = new VmOpenApiServiceInVersionBase
            {
                OntologyTerms = new List<string> { "term1", "term2", "term3", "term4", "term5", "term6", "term7", "term8", "term9", "term10", "term11" }
            };
            ontologyTermDataCacheMock.Setup(x => x.HasUris(It.IsAny<IEnumerable<string>>()))
                .Returns((IList<string> ots) => ots);

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("OntologyTerms").Should().BeTrue();
        }

        [Theory]
        [InlineData(8)]
        public void MoreThan10OntologyTerms_OlderVersions_Valid(int version)
        {
            // Arrange
            ontologyTermDataCacheMock.Setup(o => o.HasUris(It.IsAny<List<string>>()))
                .Returns((List<string> list) =>
                {
                    return list;
                });

            var vm = new VmOpenApiServiceInVersionBase
            {
                OntologyTerms = new List<string> { "term1", "term2", "term3", "term4", "term5", "term6", "term7", "term8", "term9", "term10", "term11" }
            };
            ontologyTermDataCacheMock.Setup(x => x.HasUris(It.IsAny<IEnumerable<string>>()))
                .Returns((IList<string> ots) => ots);

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }
/* SFIPTV-2048 - postpone functionality of SFIPTV-1990
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        public void MoreThan10OntologyTerms_GDAttached_NotValid(int version)
        {
            // Arrange
            ontologyTermDataCacheMock.Setup(o => o.HasUris(It.IsAny<List<string>>()))
                .Returns((List<string> list) =>
                {
                    return list;
                });

            var gdId = Guid.NewGuid();
            generalDescriptionServiceMockSetup.Setup(g => g.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase
                {
                    Id = gdId,
                    OntologyTerms = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem(), new V4VmOpenApiFintoItem()}
                });
            var vm = new VmOpenApiServiceInVersionBase
            {
                GeneralDescriptionId = gdId.ToString(),
                OntologyTerms = new List<string> { "term1", "term2", "term3", "term4", "term5", "term6", "term7", "term8", "term9" }
            };
            ontologyTermDataCacheMock.Setup(x => x.HasUris(It.IsAny<IEnumerable<string>>()))
                .Returns((IList<string> ots) => ots);

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("OntologyTerms").Should().BeTrue();
        }
*/
/* SFIPTV-2048 - postpone functionality of SFIPTV-1990
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        public void MoreThan10OntologyTerms_GDNotAttached_CurrentVersionGDExists_NotValid(int version)
        {
            // Arrange
            ontologyTermDataCacheMock.Setup(o => o.HasUris(It.IsAny<List<string>>()))
                .Returns((List<string> list) =>
                {
                    return list;
                });

            var gdId = Guid.NewGuid();
            generalDescriptionServiceMockSetup.Setup(g => g.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase
                {
                    Id = gdId,
                    OntologyTerms = new List<V4VmOpenApiFintoItem> { new V4VmOpenApiFintoItem(), new V4VmOpenApiFintoItem() }
                });
            var vm = new VmOpenApiServiceInVersionBase
            {
                OntologyTerms = new List<string> { "term1", "term2", "term3", "term4", "term5", "term6", "term7", "term8", "term9" }
            };
            ontologyTermDataCacheMock.Setup(x => x.HasUris(It.IsAny<IEnumerable<string>>()))
                .Returns((IList<string> ots) => ots);
            var currentVersion = new VmOpenApiServiceVersionBase
            {
                Id = Guid.NewGuid(),
                GeneralDescriptionId = gdId
            };
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, currentVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("OntologyTerms").Should().BeTrue();
        }
*/

        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        public void MoreThan4ServiceClasses_NotValid(int version)
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceClasses = new List<string> { "term1", "term2", "term3", "term4", "term5" }
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceClasses").Should().BeTrue();
        }

        [Theory]
        [InlineData(8)]
        public void MoreThan4ServiceClasses_OlderVersions_Valid(int version)
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceClasses = new List<string> { "term1", "term2", "term3", "term4", "term5" }
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

/* SFIPTV-2048 - postpone functionality of SFIPTV-1990
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        public void MoreThan4ServiceClasses_GDAttached_NotValid(int version)
        {
            // Arrange
            var gdId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                GeneralDescriptionId = gdId.ToString(),
                ServiceClasses = new List<string> { "term1", "term2", "term3" }
            };

            generalDescriptionServiceMockSetup.Setup(g => g.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase
                {
                    Id = gdId,
                    ServiceClasses = new List<V7VmOpenApiFintoItemWithDescription> { new V7VmOpenApiFintoItemWithDescription(), new V7VmOpenApiFintoItemWithDescription() }
                });

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceClasses").Should().BeTrue();
        }
*/

/* SFIPTV-2048 - postpone functionality of SFIPTV-1990
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        public void MoreThan4ServiceClasses_GDNotAttached_CurrentVersionGDExists_NotValid(int version)
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceClasses = new List<string> { "term1", "term2", "term3" }
            };

            var gdId = Guid.NewGuid();
            generalDescriptionServiceMockSetup.Setup(g => g.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase
                {
                    Id = gdId,
                    ServiceClasses = new List<V7VmOpenApiFintoItemWithDescription> { new V7VmOpenApiFintoItemWithDescription(), new V7VmOpenApiFintoItemWithDescription() }
                });

            var currentVersion = new VmOpenApiServiceVersionBase
            {
                Id = Guid.NewGuid(),
                GeneralDescriptionId = gdId
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, currentVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceClasses").Should().BeTrue();
        }
*/
        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        public void GDDeleted_CurrentVersionGDExists_Valid(int version)
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceClasses = new List<string> { "term1", "term2", "term3", "term4" },
                OntologyTerms = new List<string> { "term1", "term2", "term3", "term4", "term5", "term6", "term7", "term8", "term9", "term10" },
                DeleteGeneralDescriptionId = true
            };

            var gdId = Guid.NewGuid();
            generalDescriptionServiceMockSetup.Setup(g => g.GetGeneralDescriptionVersionBase(gdId, 0, true, It.IsAny<bool>()))
                .Returns(new VmOpenApiGeneralDescriptionVersionBase
                {
                    Id = gdId,
                    ServiceClasses = new List<V7VmOpenApiFintoItemWithDescription> { new V7VmOpenApiFintoItemWithDescription(), new V7VmOpenApiFintoItemWithDescription() },
                    OntologyTerms = new List<V4VmOpenApiFintoItem> {  new V4VmOpenApiFintoItem(), new V4VmOpenApiFintoItem() }
                });

            ontologyTermDataCacheMock.Setup(x => x.HasUris(It.IsAny<IEnumerable<string>>()))
                .Returns((IList<string> ots) => ots);

            var currentVersion = new VmOpenApiServiceVersionBase
            {
                Id = Guid.NewGuid(),
                GeneralDescriptionId = gdId
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, currentVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        public void AllMainServiceClasses_V9_NotValid(int version)
        {
            // Arrange
            var list = new List<string> { "term1", "term2", "term3", "term4", "term5" };
            fintoServiceMockSetup.Setup(x => x.CheckServiceClasses(It.IsAny<List<string>>()))
                .Returns((null, list));
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceClasses = list
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceClasses").Should().BeTrue();
        }

        [Theory]
        [InlineData(8)]
        public void AllMainServiceClasses_OlderVersions_Valid(int version)
        {
            // Arrange
            var list = new List<string> { "term1", "term2", "term3", "term4", "term5" };
            fintoServiceMockSetup.Setup(x => x.CheckServiceClasses(It.IsAny<List<string>>()))
                .Returns((null, list));
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceClasses = list
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        public void OrganizationLanguageAvailability_Publish_V9_Valid(int version)
        {
            // Arrange
            var id = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                MainResponsibleOrganization = id.ToString(),
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            organizationServiceMockSetup.Setup(x => x.GetAvailableLanguagesForOwnOrganization(id)).Returns(_availableLanguages);

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, new VmOpenApiServiceVersionBase { AvailableLanguages = _availableLanguages });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetValidDates))]
        public void TimedPublishing_Valid(DateTime? validFrom, DateTime? validTo, PublishingStatus status)
        {
            // Arrange
            var validator = new ServiceValidator(
                new VmOpenApiServiceInVersionBase
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInValidDates))]
        public void TimedPublishing_NotValid(DateTime? validFrom, DateTime? validTo, PublishingStatus status)
        {
            // Arrange
            var validator = new ServiceValidator(
                new VmOpenApiServiceInVersionBase
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ValidTo").Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetInValidPublishingStatus))]
        public void TimedPublishing_NotValid_PublishingStatus(DateTime? validFrom, DateTime? validTo, PublishingStatus status)
        {
            // Arrange
            var validator = new ServiceValidator(
                new VmOpenApiServiceInVersionBase
                {
                    PublishingStatus = status.ToString(),
                    ValidFrom = validFrom,
                    ValidTo = validTo
                },
                generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("PublishingStatus").Should().BeTrue();
        }

        [Fact]
        public void NameAndSummary_DescriptionsNotSet_CurrentVersionExists_Valid()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceNames = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Value = "Name", Type = NameTypeEnum.Name.GetOpenApiValue(), Language = FI } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var vmCurrentVersion = new VmOpenApiServiceVersionBase {
                AvailableLanguages = _availableLanguages,
                Organizations = new List<V6VmOpenApiServiceOrganization> { new V6VmOpenApiServiceOrganization { Organization = new VmOpenApiItem { Id = organizationId }, RoleType = CommonConsts.RESPONSIBLE } }
            };
            organizationServiceMockSetup.Setup(o => o.GetAvailableLanguagesForOwnOrganization(organizationId)).Returns(new List<string>{ FI });
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, vmCurrentVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void NameAndSummary_DescriptionsNotSet_CurrentVersionNotExists_NotValid()
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceNames = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Value = "Name", Type = NameTypeEnum.Name.GetOpenApiValue(), Language = FI } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceDescriptions").Should().BeTrue();
        }

        [Fact]
        public void NameAndSummary_Same_NotValid()
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceNames = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Value = TEXT, Type = NameTypeEnum.Name.GetOpenApiValue(), Language = FI } },
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Value = TEXT, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue(), Language = FI } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceNames").Should().BeTrue();
        }

        [Theory]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void NameAndSummary_Same_OlderVersions_Valid(int version)
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceNames = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Value = TEXT, Type = NameTypeEnum.Name.GetOpenApiValue(), Language = FI } },
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Value = TEXT, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue(), Language = FI },
                new VmOpenApiLocalizedListItem { Value = TEXT, Type = DescriptionTypeEnum.Description.GetOpenApiValue(), Language = FI }},
                PublishingStatus = PublishingStatus.Published.ToString(),
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, version, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_NoCurrentVersion_RequiredPropertiesMissing()
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                Requirements = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = FI, Value = TEXT } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                Requirements = new List<VmOpenApiLanguageItem> {
                    new VmOpenApiLanguageItem { Language = FI, Value = TEXT },
                    new VmOpenApiLanguageItem { Language = FI + "2", Value = TEXT }
                },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var vmCurrentVersion = new VmOpenApiServiceVersionBase { AvailableLanguages = _availableLanguages };
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, vmCurrentVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Model").Should().BeTrue();
        }

        [Fact]
        public void LanguageVersionsAdded_CurrentVersionExists_NamesMissing()
        {
            // Arrange
            var language2 = FI + "2";
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{Language = FI, Value = TEXT, Type = DescriptionTypeEnum.Description.GetOpenApiValue()},
                    new VmOpenApiLocalizedListItem{Language = FI, Value = TEXT, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue()},
                    new VmOpenApiLocalizedListItem{Language = language2, Value = TEXT, Type = DescriptionTypeEnum.Description.GetOpenApiValue()},
                    new VmOpenApiLocalizedListItem{Language = language2, Value = TEXT, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue()},
                },
                Requirements = new List<VmOpenApiLanguageItem> {
                    new VmOpenApiLanguageItem { Language = FI, Value = TEXT },
                    new VmOpenApiLanguageItem { Language = language2, Value = TEXT }
                },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var vmCurrentVersion = new VmOpenApiServiceVersionBase { AvailableLanguages = _availableLanguages };
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, vmCurrentVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceNames").Should().BeTrue();
        }

        [Fact]
        public void NoLanguageVersionsAdded_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem{Language = FI, Value = TEXT, Type = DescriptionTypeEnum.Description.GetOpenApiValue()},
                },
                Requirements = new List<VmOpenApiLanguageItem> {
                    new VmOpenApiLanguageItem { Language = FI, Value = TEXT },
                },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var vmCurrentVersion = new VmOpenApiServiceVersionBase { AvailableLanguages = _availableLanguages };
            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, vmCurrentVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceDescriptions").Should().BeTrue();
            controller.ModelState.ContainsKey("ServiceNames").Should().BeFalse();
        }

        [Fact]
        public void NoChangesWithinRequiredProperties_CurrentVersionExists_OptionalPropertiesUpdated()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                Requirements = new List<VmOpenApiLanguageItem> {
                    new VmOpenApiLanguageItem { Language = FI, Value = TEXT },
                },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var vmCurrentVersion = new VmOpenApiServiceVersionBase
            {
                AvailableLanguages = _availableLanguages,
                Organizations = new List<V6VmOpenApiServiceOrganization> { new V6VmOpenApiServiceOrganization { Organization = new VmOpenApiItem { Id = organizationId }, RoleType = CommonConsts.RESPONSIBLE } }
            };
            organizationServiceMockSetup.Setup(o => o.GetAvailableLanguagesForOwnOrganization(organizationId)).Returns(new List<string> { FI });

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, vmCurrentVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RemovingLanguageVersions_CurrentVersionExists_Valid()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceNames = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Language = FI, Value = "Name", Type = NameTypeEnum.Name.GetOpenApiValue() } },
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Language = FI, Value = TEXT, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    new VmOpenApiLocalizedListItem { Language = FI, Value = TEXT, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue() }
            },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var vmCurrentVersion = new VmOpenApiServiceVersionBase
            {
                AvailableLanguages = new List<string> { FI, FI + "2" },
                Organizations = new List<V6VmOpenApiServiceOrganization> { new V6VmOpenApiServiceOrganization { Organization = new VmOpenApiItem { Id = organizationId }, RoleType = CommonConsts.RESPONSIBLE } }
            };
            organizationServiceMockSetup.Setup(o => o.GetAvailableLanguagesForOwnOrganization(organizationId)).Returns(new List<string> { FI });

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, vmCurrentVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RemovingLanguageVersions_CurrentVersionExists_NotValid()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var vm = new VmOpenApiServiceInVersionBase
            {
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Language = FI, Value = TEXT, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    new VmOpenApiLocalizedListItem { Language = FI, Value = TEXT, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue() }
            },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var vmCurrentVersion = new VmOpenApiServiceVersionBase
            {
                AvailableLanguages = new List<string> { FI, FI + "2" },
                Organizations = new List<V6VmOpenApiServiceOrganization> { new V6VmOpenApiServiceOrganization { Organization = new VmOpenApiItem { Id = organizationId }, RoleType = CommonConsts.RESPONSIBLE } }
            };
            organizationServiceMockSetup.Setup(o => o.GetAvailableLanguagesForOwnOrganization(organizationId)).Returns(new List<string> { FI });

            var validator = new ServiceValidator(vm, generalDescriptionService, codeService, fintoService, ontologyTermDataCache, commonService, channelService,
                organizationService, UserRoleEnum.Pete, _defaultVersion, vmCurrentVersion);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("ServiceNames").Should().BeTrue();
        }

        public static IEnumerable<object[]> GetValidDates()
        {
            yield return new object[] { DateTime.Now, DateTime.Now.AddDays(1), PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, DateTime.Now.AddMonths(1), PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, DateTime.Now.AddYears(1), PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, null, PublishingStatus.Draft };
            yield return new object[] { null, DateTime.Now, PublishingStatus.Published };
        }

        public static IEnumerable<object[]> GetInValidDates()
        {
            yield return new object[] { DateTime.Now.AddDays(1), DateTime.Now, PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, DateTime.Now.AddSeconds(10), PublishingStatus.Draft };
            yield return new object[] { DateTime.Now, DateTime.Now.AddMinutes(10), PublishingStatus.Draft };
        }

        public static IEnumerable<object[]> GetInValidPublishingStatus()
        {
            yield return new object[] { DateTime.Now, DateTime.Now.AddDays(1), PublishingStatus.Published };
            yield return new object[] { DateTime.Now, DateTime.Now.AddMonths(1), PublishingStatus.Published };
            yield return new object[] { DateTime.Now, DateTime.Now.AddYears(1), PublishingStatus.Published };
            yield return new object[] { DateTime.Now, null, PublishingStatus.Published };
            yield return new object[] { null, DateTime.Now, PublishingStatus.Draft };
        }
    }
}
