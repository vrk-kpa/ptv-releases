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
using System.Collections.Generic;
using System.Linq;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using FluentAssertions;
using System;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Framework;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Channels.V2.PrintableForm;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Channel.PrintableForm;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class PrintableFormChannelMainTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private readonly ItemListModelGenerator itemListGenerator;
        
        public PrintableFormChannelMainTranslatorTest()
        {
            ServiceChannelVersioned entity = null;
            itemListGenerator = new ItemListModelGenerator();
            SetupTypesCacheMock<ServiceChannelType>(typeof(ServiceChannelTypeEnum));
            RegisterEntityForVersionManager<ServiceChannelVersioned, ServiceChannel>();
            translators = new List<object>()
            {
                new PrintableFormMainTranslator(ResolveManager, TranslationPrimitives, new ServiceChannelTranslationDefinitionHelper(CacheManager)),

//                RegisterTranslatorMock(new Mock<ITranslator<PrintableFormChannel, VmPrintableFormChannel>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, VmPrintableFormChannelStep1>>(), unitOfWorkMock),
                RegisterTranslatorMock<PrintableFormChannel, VmPrintableForm>(),
                RegisterTranslatorMock<ServiceChannelAttachment, VmChannelAttachment>(),
                RegisterTranslatorMock<ServiceChannelLanguage, VmListItem>(),
                RegisterTranslatorMock<ServiceChannelAddress, VmAddressSimple>(),
                
//                RegisterTranslatorMock<pfch, VmWebPageChannel>(model => new WebpageChannel(), ent =>
//                {
//                    var result = vmmodel ?? new VmWebPageChannel();
//                    result.WebPage = ent?.LocalizedUrls.ToDictionary(x => x.LocalizationId.ToString(), x => x.Url);
//                    return result;
//                }, setTargetViewAction: (vm, v) => vmmodel = vm),
                RegisterTranslatorMock<ServiceChannelVersioned, VmServiceChannel>(model =>
                {
                    var result = entity ?? new ServiceChannelVersioned();
                    result.OrganizationId = model.OrganizationId;
                    return result;
                }, setTargetEntityAction: (sv, v) => entity = sv),
            };
        }

        /// <summary>
        /// test for PrintableFormChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("fi")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("fi;sv;en")]
        public void TranslateToEntity(string list)
        {
            RegisterDbSet(new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
            var model = new VmPrintableForm
            {
                OrganizationId = "Partial".GetGuid(),
                Languages = itemListGenerator.CreateList(list, x => x.GetGuid())?.ToList(),
                Attachments = list?.Split(";").ToDictionary(x => x, x => new List<VmChannelAttachment> { TestHelper.CreateVmChannelAttachmentModel(x.GetGuid()) } ),
                DeliveryAddresses = list?.Split(";").Select(x => TestHelper.CreateVmAdressSimpleModel()).ToList()
            };
            TestHelper.SetupLanguagesAvailabilities(model);
            var translation = RunTranslationModelToEntityTest<VmPrintableForm, ServiceChannelVersioned>(translators, model, unitOfWorkMockSetup.Object);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmPrintableForm source, ServiceChannelVersioned translation)
        {
            translation.TypeId.Should().Be(ServiceChannelTypeEnum.PrintableForm.ToString().GetGuid());
            translation.LanguageAvailabilities.Should().NotBeEmpty();
            translation.Languages.Count.Should().Be(source.Languages?.Count ?? 0, "Languages");
            translation.PrintableFormChannels.Should().NotBeEmpty();
            translation.OrganizationId.Should().Be(source.OrganizationId);
            if (source.Attachments != null)
            {
                translation.Attachments.Count.Should().Be(source.Attachments.Sum(x => x.Value.Count));
            }
            else
            {
                translation.Attachments.Should().BeEmpty();
            }            
            if (source.DeliveryAddresses != null)
            {
                translation.Addresses.Count.Should().Be(source.DeliveryAddresses.Count);
            }
            else
            {
                translation.Addresses.Should().BeEmpty();
            }
        }

        /// <summary>
        /// test for PrintableFormChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("", "exist")]
        [InlineData("not found id", "exist")]
        [InlineData("exist", "exist")]
        public void TranslateToEntityCreateUpdate(string modelId, string entityId)
        {
            var entity = new ServiceChannelVersioned() { Id = entityId.GetGuid()};
            RegisterDbSet(new List<ServiceChannelVersioned>() { entity }, unitOfWorkMockSetup);
            var model = new VmPrintableForm
            {
                Id = string.IsNullOrEmpty(modelId) ? (Guid?) null : modelId.GetGuid()
            };
            if (string.IsNullOrEmpty(modelId) || modelId == entityId)
            {
                var translation = RunTranslationModelToEntityTest<VmPrintableForm, ServiceChannelVersioned>(translators, model, unitOfWorkMockSetup.Object);
                if (modelId == entityId)
                {
                    translation.Id.Should().Be(entityId.GetGuid());
                    translation.Id.Should().Be(model.Id.Value);
                    translation.Should().Be(entity);
                }
                else
                {
                    translation.Id.Should().NotBeEmpty();
                    translation.Should().NotBe(entity);
                }
            }
            else
            {
                Assert.Throws<DbEntityNotFoundException>(() => RunTranslationModelToEntityTest<VmPrintableForm, ServiceChannelVersioned>(translators, model, unitOfWorkMockSetup.Object));
            }
        }
    }
}
