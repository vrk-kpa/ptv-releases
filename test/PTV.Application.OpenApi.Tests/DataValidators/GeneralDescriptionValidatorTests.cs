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

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public class GeneralDescriptionValidatorTests : ValidatorTestBase
    {
        [Fact]
        public void ModelIsNull()
        {
            // Arrange & act
            Action act = () => new GeneralDescriptionValidator(null, codeService, fintoService, new List<string> { "language" });

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void DescriptionOrBackGroundDescriptionNotSet()
        {
            // Arrange
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Descriptions = new List<VmOpenApiLocalizedListItem>()
            };
            var validator = new GeneralDescriptionValidator(vm, codeService, fintoService, new List<string> { "language" });

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
            var language = "language";
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Descriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = DescriptionTypeEnum.Description.ToString(), Value = "TestValue"
                    },
                }
            };
            var validator = new GeneralDescriptionValidator(vm, codeService, fintoService, new List<string> { language });

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
            var language = "language";
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Names = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = NameTypeEnum.Name.ToString(), Value = "TestValue"
                    }
                },
                Descriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = DescriptionTypeEnum.Description.ToString(), Value = "TestValue"
                    },
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = DescriptionTypeEnum.ShortDescription.ToString(), Value = "TestValue"
                    }
                }
            };
            var validator = new GeneralDescriptionValidator(vm, codeService, fintoService, new List<string> { language });

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ModelIsValid_BackgroundDescriptionSet()
        {
            // Arrange
            var language = "language";
            var vm = new VmOpenApiGeneralDescriptionInVersionBase
            {
                Names = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = NameTypeEnum.Name.ToString(), Value = "TestValue"
                    }
                },
                Descriptions = new List<VmOpenApiLocalizedListItem>
                {
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = DescriptionTypeEnum.BackgroundDescription.ToString(), Value = "TestValue"
                    },
                    new VmOpenApiLocalizedListItem
                    {
                        Language = language, Type = DescriptionTypeEnum.ShortDescription.ToString(), Value = "TestValue"
                    }
                }
            };
            var validator = new GeneralDescriptionValidator(vm, codeService, fintoService, new List<string> { language });

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
            var vm = new VmOpenApiGeneralDescriptionInVersionBase()
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = targetGroupsSet ? new List<string> { targetGroupUri } : null,
                LifeEvents = new List<string> { "SomeLifeEvent" }
            };
            var vmTargetGroup = new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "NotKR1" };
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupUri)).Returns(vmTargetGroup);

            var validator = new GeneralDescriptionValidator(vm, codeService, fintoService, new List<string>(), currentTargetGroupsSet ? new List<IVmOpenApiFintoItemVersionBase> { vmTargetGroup } : null);

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
            var vm = new VmOpenApiGeneralDescriptionInVersionBase()
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = targetGroupsSet ? new List<string> { targetGroupUri } : null,
                IndustrialClasses = new List<string> { "SomeIndustrialClass" }
            };
            var vmTargetGroup = new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "NotKR2" };
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupUri)).Returns(vmTargetGroup);

            var validator = new GeneralDescriptionValidator(vm, codeService, fintoService, new List<string>(), currentTargetGroupsSet ? new List<IVmOpenApiFintoItemVersionBase> { vmTargetGroup } : null);

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
            var lifeEventUri = "SomeLifeEvent";
            var industrialClassUri = "SomeIndustrialClass";

            var vm = new VmOpenApiGeneralDescriptionInVersionBase()
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = targetGroupsSet ? new List<string> { targetGroupK1Uri, targetGroupK2Uri } : null,
                LifeEvents = new List<string> { lifeEventUri },
                IndustrialClasses = new List<string> { industrialClassUri }
            };
            var vmTargetGroupKR1 = new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "KR1" };
            var vmTargetGroupKR2 = new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "KR2" };
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupK1Uri)).Returns(vmTargetGroupKR1);
            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupK2Uri)).Returns(vmTargetGroupKR2);
            fintoServiceMockSetup.Setup(x => x.GetLifeEventByUri(lifeEventUri)).Returns(new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "Code1" });
            fintoServiceMockSetup.Setup(x => x.GetIndustrialClassByUri(industrialClassUri)).Returns(new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "Code2" });

            var validator = new GeneralDescriptionValidator(vm, codeService, fintoService, new List<string>(), currentTargetGroupsSet ? new List<IVmOpenApiFintoItemVersionBase> { vmTargetGroupKR1, vmTargetGroupKR2 } : null);

            // Act
            validator.Validate(controller.ModelState);

            // Assert
            controller.ModelState.IsValid.Should().BeTrue();
            if (targetGroupsSet)
            {
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(targetGroupK1Uri), Times.Once);
                fintoServiceMockSetup.Verify(x => x.GetTargetGroupByUri(targetGroupK2Uri), Times.Once);
            }

            fintoServiceMockSetup.Verify(x => x.GetLifeEventByUri(lifeEventUri), Times.Once);
            fintoServiceMockSetup.Verify(x => x.GetIndustrialClassByUri(industrialClassUri), Times.Once);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void TargetGroupsSet_AttachedTargetGroupsNotKR1OrKR2_NotValid(bool lifeEventSet, bool industrialClassSet)
        {
            // Arrange
            var targetGroupUri = "uri";
            var vm = new VmOpenApiGeneralDescriptionInVersionBase()
            {
                PublishingStatus = PublishingStatus.Published.ToString(),
                TargetGroups = new List<string> { targetGroupUri },
                LifeEvents = lifeEventSet ? new List<string> { "SomeLifeEvent" } : null,
                IndustrialClasses = industrialClassSet ? new List<string> { "SomeIndustrialClass" } : null
            };
            var vmTargetGroupKR1 = new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "KR1" };
            var vmTargetGroupKR2 = new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "KR2" };

            fintoServiceMockSetup.Setup(x => x.GetTargetGroupByUri(targetGroupUri)).Returns(new VmOpenApiFintoItemVersionBase { Id = Guid.NewGuid(), Code = "Code" });

            var validator = new GeneralDescriptionValidator(vm, codeService, fintoService, new List<string>(), new List<IVmOpenApiFintoItemVersionBase> { vmTargetGroupKR1, vmTargetGroupKR2 });

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
    }
}
