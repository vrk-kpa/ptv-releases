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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using Moq;
using System.Linq;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class GeneralDescriptionValidatorTests : ValidatorTestBase
    {
        private const string _language = "language";
        private const string _value = "text";

        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new GeneralDescriptionValidator(null, fintoService, ontologyTermDataCache, new List<string> { _language });

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DescriptionOrBackGroundDescriptionNotSet()
        {
            // Arrange
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Names = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Language = _language, Value = _value} },
                Descriptions = new List<VmOpenApiLocalizedListItem>(),
                AvailableLanguages = new List<string> { _language}
            };
            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Descriptions").Should().BeTrue();
        }

        [Fact]
        public void ShortDescriptionNotSet()
        {
            // Arrange
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Descriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = _language, Type = DescriptionTypeEnum.Description.GetOpenApiValue(), Value = _value
                    },
                }
            };
            var validator = new GeneralDescriptionValidator(vm, fintoService,ontologyTermDataCache, new List<string> { _language });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Descriptions").Should().BeTrue();
        }

        [Fact]
        public void ModelIsValid_DescriptionSet()
        {
            // Arrange
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Names = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = _language, Type = NameTypeEnum.Name.GetOpenApiValue(), Value = _value
                    }
                },
                Descriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = _language, Type = DescriptionTypeEnum.Description.GetOpenApiValue(), Value = _value
                    },
                    new VmOpenApiLocalizedListItem
                    {
                        Language = _language, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue(), Value = _value
                    }
                }
            };
            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelIsValid_BackgroundDescriptionSet()
        {
            // Arrange
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Names = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = _language, Type = NameTypeEnum.Name.GetOpenApiValue(), Value = _value
                    }
                },
                Descriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = _language, Type = DescriptionTypeEnum.BackgroundDescription.GetOpenApiValue(), Value = _value
                    },
                    new VmOpenApiLocalizedListItem
                    {
                        Language = _language, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue(), Value = _value
                    }
                }
            };
            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, null);

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
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = targetGroupsSet ? new List<string> { targetGroupUri } : null,
                LifeEvents = new List<string> { "SomeLifeEvent" }
            };
            var vmTargetGroup = new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "NotKR1" };
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupUri)).Returns(vmTargetGroup);

            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, new List<string>(), currentTargetGroupsSet ? new List<IVmOpenApiFintoItemVersionBase> { vmTargetGroup } : null);

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
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = targetGroupsSet ? new List<string> { targetGroupUri } : null,
                IndustrialClasses = new List<string> { "SomeIndustrialClass" }
            };
            var vmTargetGroup = new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "NotKR2" };
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupUri)).Returns(vmTargetGroup);

            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, new List<string>(), currentTargetGroupsSet ? new List<IVmOpenApiFintoItemVersionBase> { vmTargetGroup } : null);

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
            var lifeEventList = new List<string> { "SomeLifeEvent" };
            var industrialClassList = new List<string> { "SomeIndustrialClass" };

            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = targetGroupsSet ? new List<string> { targetGroupK1Uri, targetGroupK2Uri } : null,
                LifeEvents = lifeEventList,
                IndustrialClasses = industrialClassList
            };
            var vmTargetGroupKR1 = new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "KR1" };
            var vmTargetGroupKR2 = new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "KR2" };
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupK1Uri)).Returns(vmTargetGroupKR1);
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupK2Uri)).Returns(vmTargetGroupKR2);
            fintoServiceMockSetup.Setup(x => x.CheckLifeEvents(lifeEventList)).Returns((List<string>)null);
            fintoServiceMockSetup.Setup(x => x.CheckIndustrialClasses(industrialClassList)).Returns((List<string>)null);

            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, new List<string>(), currentTargetGroupsSet ? new List<IVmOpenApiFintoItemVersionBase> { vmTargetGroupKR1, vmTargetGroupKR2 } : null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            if (targetGroupsSet)
            {
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(targetGroupK1Uri), Times.Once);
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(targetGroupK2Uri), Times.Once);
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
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = new List<string> { targetGroupUri },
                LifeEvents = lifeEventSet ? new List<string> { "SomeLifeEvent" } : null,
                IndustrialClasses = industrialClassSet ? new List<string> { "SomeIndustrialClass" } : null
            };
            var vmTargetGroupKR1 = new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "KR1" };
            var vmTargetGroupKR2 = new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "KR2" };

            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupUri)).Returns(new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "Code" });

            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, new List<string>(), new List<IVmOpenApiFintoItemVersionBase> { vmTargetGroupKR1, vmTargetGroupKR2 });

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
        public void LanguageVersionsAdded_NoCurrentVersion_RequiredPropertiesMissing()
        {
            // Arrange
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Requirements = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = _language, Value = _value } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, null);

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
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Requirements = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = _language, Value = _value },
                    new VmOpenApiLanguageItem { Language = _language + "2", Value = _value + "2" }},
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, new List<string> { _language });

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
            var language2 = _language + "2";
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Descriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Language = _language, Value = _value, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    new VmOpenApiLocalizedListItem { Language = _language, Value = _value, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue() },
                    new VmOpenApiLocalizedListItem { Language = language2, Value = _value, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    new VmOpenApiLocalizedListItem { Language = language2, Value = _value, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue() },
                },
                Requirements = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = _language, Value = _value },
                    new VmOpenApiLanguageItem { Language = language2, Value = _value }},
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, new List<string> { _language });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Names").Should().BeTrue();
        }

        [Fact]
        public void NoLanguageVersionsAdded_CurrentVersionExists_RequiredPropertiesMissing()
        {
            // Arrange
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Descriptions = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Language = _language, Value = _value, Type = DescriptionTypeEnum.Description.GetOpenApiValue()} },
                Requirements = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = _language, Value = _value } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, new List<string> { _language });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Descriptions").Should().BeTrue();
            controller.ModelState.ContainsKey("Names").Should().BeFalse();
        }

        [Fact]
        public void NoChangesWithinRequiredProperties_CurrentVersionExists_OptionalPropertiesUpdated()
        {
            // Arrange
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Requirements = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Language = _language, Value = _value } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, new List<string> { _language });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RemovingLanguageVersions_CurrentVersionsExist_Valid()
        {
            // Arrange
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Names = new List<VmOpenApiLocalizedListItem> { new VmOpenApiLocalizedListItem { Language = _language, Value = _value, Type = NameTypeEnum.Name.GetOpenApiValue() } },
                Descriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Language = _language, Value = _value, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    new VmOpenApiLocalizedListItem { Language = _language, Value = _value, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue() } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, new List<string> { _language, _language + "2" });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void RemovingLanguageVersions_CurrentVersionsExist_NotValid()
        {
            // Arrange
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Descriptions = new List<VmOpenApiLocalizedListItem> {
                    new VmOpenApiLocalizedListItem { Language = _language, Value = _value, Type = DescriptionTypeEnum.Description.GetOpenApiValue() },
                    new VmOpenApiLocalizedListItem { Language = _language, Value = _value, Type = DescriptionTypeEnum.ShortDescription.GetOpenApiValue() } },
                PublishingStatus = PublishingStatus.Published.ToString(),
            };
            var validator = new GeneralDescriptionValidator(vm, fintoService, ontologyTermDataCache, new List<string> { _language, _language + "2" });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ContainsKey("Names").Should().BeTrue();
        }
    }
}
