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

using System.Collections.Generic;
using FluentAssertions;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using ServiceChannelTranslationDefinitionHelper = PTV.Database.DataAccess.Translators.Channels.V2.ServiceChannelTranslationDefinitionHelper;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class ElectronicChannelMainTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public ElectronicChannelMainTranslatorTest()
        {
            SetupTypesCacheMock<ServiceChannelType>(typeof(ServiceChannelTypeEnum));
            SetupTypesCacheMock<AccessibilityClassificationLevelType>();
            SetupTypesCacheMock<WcagLevelType>();

            ServiceChannelVersioned channel = null;
            translators = new List<object>
            {
                new ElectronicChannelBasicInfoMainTranslator(ResolveManager, TranslationPrimitives, new ServiceChannelTranslationDefinitionHelper(CacheManager), CacheManager),
                RegisterTranslatorMock<ElectronicChannel, VmElectronicChannel>(),
                RegisterTranslatorMock<ServiceChannelAttachment, VmChannelAttachment>(),
                RegisterTranslatorMock<ServiceChannelLanguage, VmListItem>(),
                RegisterTranslatorMock<ServiceChannelVersioned, VmOpeningHours>(model =>
                    {
                        var result = channel ?? new ServiceChannelVersioned();
                        result.ServiceChannelServiceHours = new List<ServiceChannelServiceHours> { new ServiceChannelServiceHours() };
                        return result;
                }, setTargetEntityAction: (e, vm) => channel = e),
                RegisterTranslatorMock<ServiceChannelVersioned, VmServiceChannel>(model =>
                    {
                        var result = channel ?? new ServiceChannelVersioned();
                        result.OrganizationId = model.OrganizationId;
                        return result;
                    },
                    setTargetEntityAction: (entity, vm) => channel = entity
                ),
                RegisterTranslatorMock<ServiceChannelAccessibilityClassification, VmAccessibilityClassification>(),
            };
            RegisterDbSet(new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
            RegisterEntityForVersionManager<ServiceChannelVersioned, ServiceChannel>();
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TranslateElectronicChannelToEntity(bool isAccessibiltyClassificationSet)
        {
            var model = TestHelper.CreateVmElectronicChannelModel(isAccessibiltyClassificationSet);
            TestHelper.SetupLanguagesAvailabilities(model, "fi");

            var translation = RunTranslationModelToEntityTest<VmElectronicChannel, ServiceChannelVersioned>(translators, model, unitOfWorkMockSetup.Object);

            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmElectronicChannel source, ServiceChannelVersioned translation)
        {
            translation.Languages.Count.Should().Be(source.Languages?.Count ?? 0, "Languages");
            translation.TypeId.Should().Be(ServiceChannelTypeEnum.EChannel.ToString().GetGuid());
            translation.OrganizationId.Should().Be(source.OrganizationId);
            translation.ServiceChannelServiceHours.Count.Should().Be(1);
            translation.LanguageAvailabilities.Count.Should().Be(source.LanguagesAvailabilities.Count);
            translation.AccessibilityClassifications.Count.Should().Be(source.LanguagesAvailabilities.Count);
        }
    }
}
