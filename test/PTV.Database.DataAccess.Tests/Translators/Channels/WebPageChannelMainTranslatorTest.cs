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
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.DataAccess.Translators.Channels.V2.WebPage;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class WebPageChannelMainTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly IUnitOfWork unitOfWorkMock;

        public WebPageChannelMainTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            SetupTypesCacheMock<ServiceChannelType>(typeof(ServiceChannelTypeEnum));

            ServiceChannelVersioned entity = null;
            VmWebPageChannel vmmodel = null;
            translators = new List<object>()
            {
                new WebPageChannelMainTranslator(ResolveManager, TranslationPrimitives, new ServiceChannelTranslationDefinitionHelper(CacheManager)),
                RegisterTranslatorMock<WebpageChannel, VmWebPageChannel>(model => new WebpageChannel(), ent =>
                {
                    var result = vmmodel ?? new VmWebPageChannel();
                    result.WebPage = ent?.LocalizedUrls.ToDictionary(x => x.LocalizationId.ToString(), x => x.Url);
                    return result;
                }, setTargetViewAction: (vm, v) => vmmodel = vm),
                RegisterTranslatorMock<ServiceChannelVersioned, VmServiceChannel>(model =>
                {
                    var result = entity ?? new ServiceChannelVersioned();
                    result.OrganizationId = model.OrganizationId;
                    return result;
                }, setTargetEntityAction: (sv, v) => entity = sv),
                RegisterTranslatorMock<ServiceChannelLanguage, VmListItem>(model => new ServiceChannelLanguage { LanguageId = model.Id }),
                RegisterTranslatorMock<ServiceChannelAccessibilityClassification, VmAccessibilityClassification>(),
                RegisterTranslatorMock<AccessibilityClassification, VmAccessibilityClassification>(),
            };

            RegisterDbSet(new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
        }

          /// <summary>
        /// test for WebPageChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("fi")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("fi;sv;en")]
        public void TranslateToEntity(string languages)
        {
            var model = new VmWebPageChannel
            {
                OrganizationId = "test partial".GetGuid(),
                Languages = languages?.Split(";").Select(x => x.GetGuid()).ToList(),
                AccessibilityClassifications = new Dictionary<string, VmAccessibilityClassification>() {{ "fi", TestHelper.CreateAccessibilityClassificationModel()}}
            };
            TestHelper.SetupLanguagesAvailabilities(model, "fi");
            var translation = RunTranslationModelToEntityTest<VmWebPageChannel, ServiceChannelVersioned>(translators, model, unitOfWorkMock);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmWebPageChannel source, ServiceChannelVersioned translation)
        {
            translation.TypeId.Should().Be(ServiceChannelTypeEnum.WebPage.ToString().GetGuid());
            translation.OrganizationId.Should().Be(source.OrganizationId);
            translation.WebpageChannels.Count.Should().Be(1);
            translation.LanguageAvailabilities.Should().NotBeEmpty();
            translation.AccessibilityClassifications.Count.Should().Be(source.LanguagesAvailabilities.Count);

            if (source.Languages != null)
            {
                translation.Languages.Count.Should().Be(source.Languages.Count);
                translation.Languages.Select(x => x.LanguageId).Should().Contain(source.Languages);
            }
            else
            {
                translation.Languages.Should().BeEmpty();
            }
//            translation.LocalizedUrls.Count.Should().Be(source.WebPage?.Count ?? 0);
//            if (source.WebPage?.Count > 0)
//            {
//                translation.LocalizedUrls.Select(x => x.Url).Should().Contain(source.WebPage.Values);
//            }
        }
        
        /// <summary>
        /// test for WebPageChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("fi", true)]
        [InlineData("", false)]
        [InlineData("fi;sv;en", true)]
        public void TranslateToViewModel(string languages, bool subEntityAdded)
        {
            var entity = new ServiceChannelVersioned
            {
                OrganizationId = "test partial".GetGuid(),
                Languages = languages?.Split(";").Select(x => new ServiceChannelLanguage { LanguageId = x.GetGuid() }).ToList(),
                WebpageChannels = subEntityAdded ?
                    new List<WebpageChannel>
                    {
                        new WebpageChannel { LocalizedUrls = new List<WebpageChannelUrl>() }
                    } : null,
                AccessibilityClassifications = {new ServiceChannelAccessibilityClassification() {AccessibilityClassification = new AccessibilityClassification() {LocalizationId = "fi".GetGuid()}}}
            };
            var translation = RunTranslationEntityToModelTest<ServiceChannelVersioned, VmWebPageChannel>(translators, entity);
            CheckTranslation(entity, translation);
        }

        private void CheckTranslation(ServiceChannelVersioned source, VmWebPageChannel translation)
        {
            translation.OrganizationId.Should().Be(source.OrganizationId);
            translation.AccessibilityClassifications.Count.Should().Be(source.AccessibilityClassifications.Count);

            if (source.WebpageChannels == null)
            {
                translation.WebPage.Should().BeNull();
            }
            else
            {
                translation.WebPage.Count.Should().Be(source.WebpageChannels.Sum(x => x.LocalizedUrls.Count));    
            }
            if (source.Languages != null)
            {
                translation.Languages.Count.Should().Be(source.Languages.Count);
                translation.Languages.Should().Contain(source.Languages.Select(x => x.LanguageId));
            }
            else
            {
                translation.Languages.Should().BeEmpty();
            }
        }
    }
}