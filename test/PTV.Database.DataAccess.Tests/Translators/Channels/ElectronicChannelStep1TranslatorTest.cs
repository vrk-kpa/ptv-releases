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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Database.DataAccess.Translators.Channels;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.DataAccess.Translators.Channels.V2.Electronic;
using PTV.Domain.Model.Models.V2.Channel;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class ElectronicChannelStep1TranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public ElectronicChannelStep1TranslatorTest()
        {
            translators = new List<object>
            {
                new ElectronicChannelBasicInfoTranslator(ResolveManager, TranslationPrimitives),

//                RegisterTranslatorMock(new Mock<ITranslator<ElectronicChannelUrl, VmWebPage>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelAttachment, VmChannelAttachment>>(), unitOfWorkMock),
            };

            RegisterDbSet(new List<ElectronicChannel>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ElectronicChannelTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateElectronicChannelToEntity()
        {
            var model = TestHelper.CreateVmElectronicChannelModel();

            var translation = RunTranslationModelToEntityTest<VmElectronicChannel, ElectronicChannel>(translators, model, unitOfWorkMockSetup.Object);

            translation.RequiresAuthentication.Should().Be(model.IsOnLineAuthentication ?? false);
            translation.RequiresSignature.Should().Be(model.IsOnlineSign);
            translation.SignatureQuantity.Should().Be(model.SignatureCount);
        }
    }
}
