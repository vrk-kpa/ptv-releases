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
using FluentAssertions;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using Moq;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class ElectronicChannelMainTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public ElectronicChannelMainTranslatorTest()
        {
            var scTranslationHelperMock = new Mock<ServiceChannelTranslationDefinitionHelper>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<object>()
            {
                new ElectronicChannelBasicInfoMainTranslator(ResolveManager, TranslationPrimitives, scTranslationHelperMock.Object, CacheManager),
//                new ElectronicChannelMainStep1Translator(ResolveManager, TranslationPrimitives, new ServiceChannelTranslationDefinitionHelper(CacheManager), CacheManager),
//                new AttachmentTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<ElectronicChannel, VmElectronicChannel>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, VmServiceChannel>>(), unitOfWorkMock
//                ,
//                    channel =>
//                    {
//                        return channel.
//                    }
                ),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, VmOpeningHours>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, IEmail>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, IPhoneNumber>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, IVmChannelDescription>>(), unitOfWorkMock),
//                new ElectronicChannelUrlTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceHourAdditionalInformationTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceChannelDescriptionTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceChannelNameTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceChannelServiceHourTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelAttachment, VmChannelAttachment>>(), unitOfWorkMock),
//
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelType, string>>(), unitOfWorkMock, code => new ServiceChannelType { Code = code }),
//                new AttachmentTypeCodeTranslator(ResolveManager, TranslationPrimitives),
//                new PublishingStatusTranslator(ResolveManager, TranslationPrimitives),
//                new NameTypeCodeTranslator(ResolveManager, TranslationPrimitives),
//                new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
//                new ServiceHourTypeCodeTranslator(ResolveManager, TranslationPrimitives),
//                new ExceptionHoursStatusTypeCodeTranslator(ResolveManager, TranslationPrimitives),


                RegisterTranslatorMock(new Mock<ITranslator<Language, string>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, VmOpeningHoursStep>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelEmail, VmEmailData>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelPhone, VmPhone>>(), unitOfWorkMock)
            };
            RegisterDbSet(new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ElectronicChannel>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ElectronicChannelUrl>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceChannelName>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceChannelDescription>(), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<ServiceChannelType>(typeof(ServiceChannelTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<AttachmentType>(typeof(AttachmentTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<NameType>(typeof(NameTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<DescriptionType>(typeof(DescriptionTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<ServiceHourType>(typeof(ServiceHoursTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<ExceptionHoursStatusType>(typeof(ExceptionHoursStatus)), unitOfWorkMockSetup);

            RegisterDbSet(new List<Attachment>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceChannelServiceHours>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateElectronicChannelToEntity()
        {
            var model = TestHelper.CreateVmElectronicChannelModel();
            var toTranslate = new List<VmElectronicChannel>() { model };

            var translations = RunTranslationModelToEntityTest<VmElectronicChannel, ServiceChannelVersioned>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            //            Assert.Equal(model.PublishingStatus, translation.PublishingStatusId);
            //            Assert.Equal(model.Step1Form.OrganizationId, translation.OrganizationId);
            //                        Assert.Equal(ServiceChannelTypeEnum.EChannel.ToString(),
            translation.Id.Should().NotBeEmpty();
            translation.TypeId.Should().Be(ServiceChannelTypeEnum.EChannel.ToString().GetGuid());
        }
    }
}
