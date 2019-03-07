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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using Xunit;
using Xunit.Sdk;
using DateTimeConverter = System.ComponentModel.DateTimeConverter;

namespace PTV.Database.DataAccess.Tests.Translators.Channels.OpeningHours
{
    public class ServiceChannelServiceHourTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock = null; // TODO: fix, add mock
        //private ItemListModelGenerator listModelGenerator;
        private TestConversion conversion;
        
        public ServiceChannelServiceHourTranslatorTest()
        {
            //listModelGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();

            translators = new List<object>
            {
                new OpeningHourTranslator(ResolveManager, TranslationPrimitives, CacheManager),
//                new TextTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceHoursAdditionalInformation, VmLanguageText>>(), unitOfWorkMock, model => new ServiceHoursAdditionalInformation { Text = model.Text, LocalizationId = model.LocalizationId }),
                RegisterTranslatorMock<IText, string>(model => new ServiceHoursAdditionalInformation { Text = model }, entity => entity.Text),
            };
            RegisterDbSet(new List<ServiceHours>(), unitOfWorkMockSetup);
            SetupTypesCacheMock<ServiceHourType>(typeof(ServiceHoursTypeEnum));
        }

        /// <summary>
        /// test for ServiceChannelServiceHourTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(ServiceHoursTypeEnum.Standard, "fi;sv", true, "1.21.2015", "1.23.2015")]
        [InlineData(ServiceHoursTypeEnum.Special, null, true, "1.21.2015", null)]
        [InlineData(ServiceHoursTypeEnum.Special, "fi;sv;en", true, null, null)]
        [InlineData(ServiceHoursTypeEnum.Standard, null, true, null, "1.21.2015")]
        [InlineData(ServiceHoursTypeEnum.Special, "en", false, null, null)]
        [InlineData(ServiceHoursTypeEnum.Standard, "", false, "1.21.2015", null)]
        [InlineData(ServiceHoursTypeEnum.Special, "smn", false, null, "1.21.2015")]
        [InlineData(ServiceHoursTypeEnum.Special, "", false, "1.25.2015", "1.21.2015")]
        [InlineData(ServiceHoursTypeEnum.Exception, "", true, "1.25.2015", "1.21.2015")]
        [InlineData(ServiceHoursTypeEnum.Exception, "", false, "1.25.2015", "1.21.2015")]
        [InlineData(ServiceHoursTypeEnum.Exception, "", false, null, "1.21.2015")]
        public void TranslateServiceChannelServiceHourToEntity(ServiceHoursTypeEnum serviceHoursType, string names, bool isRange, string dateFrom, string dateTo)
        {
            
            var model = TestHelper.CreateVmHoursModel<VmOpeningHour>(serviceHoursType);
            model.ServiceHoursType = serviceHoursType;
            model.IsPeriod = isRange;
            model.DateFrom = conversion.GetDate(dateFrom);
            model.DateTo = conversion.GetDate(dateTo);
            model.Name = names?.Split(";").Where(x => !string.IsNullOrEmpty(x)).ToDictionary(x => x);
            model.OrderNumber = isRange ? 6 : (int?) null;

            var translation = RunTranslationModelToEntityTest<VmOpeningHour, ServiceHours>(translators, model, unitOfWorkMock);

            CheckTranslation(model, translation);
        }

        private static void CheckTranslation(VmOpeningHour model, ServiceHours translation)
        {
            if (model.Name != null)
            {
                translation.AdditionalInformations.Count.Should().Be(model.Name.Count);
                if (model.Name.Count > 0)
                {
                    translation.AdditionalInformations.Select(x => x.Text).Should().Contain(model.Name.Values);
                }
            }
            else
            {
                translation.AdditionalInformations.Should().BeEmpty();
            }

            translation.ServiceHourTypeId.Should().Be(model.ServiceHoursType.ToString().GetGuid());
            translation.OrderNumber.Should().Be(model.OrderNumber);
            
            if (model.IsPeriod)
            {
                translation.OpeningHoursFrom.Should().Be(model.DateFrom?.FromEpochTime());
                translation.OpeningHoursTo.Should().Be(model.DateTo?.FromEpochTime());
            }
            else
            {
                translation.OpeningHoursFrom.Should().Be(model.ServiceHoursType == ServiceHoursTypeEnum.Exception ? model.DateFrom?.FromEpochTime() : null);
                translation.OpeningHoursTo.HasValue.Should().BeFalse();
            }
            
        }

        /// <summary>
        /// test for ServiceChannelServiceHourTranslator entity - vm
        /// </summary>
        [Theory]
        [InlineData(ServiceHoursTypeEnum.Standard, true, true, "fi;sv")]
        [InlineData(ServiceHoursTypeEnum.Special, false, true, "fi")]
        [InlineData(ServiceHoursTypeEnum.Exception, true, false, "")]
        [InlineData(ServiceHoursTypeEnum.Exception, false, false, null)]
        public void TranslateServiceHoursToVm(ServiceHoursTypeEnum hoursType, bool setDateFrom, bool setDateTo, string additionalInfo)
        {
            var entity = CreateModel(hoursType);
            entity.AdditionalInformations = additionalInfo?.Split(";").Where(x => !string.IsNullOrEmpty(x)).Select(x => new ServiceHoursAdditionalInformation { Text = x, LocalizationId = x.GetGuid() } ).ToList();
            entity.OpeningHoursTo = setDateFrom ? DateTime.Now : (DateTime?)null;
            entity.OpeningHoursFrom = setDateTo ? DateTime.Now.AddDays(2) : (DateTime?) null;
            entity.OrderNumber = setDateFrom ? 6 : (int?) null;
            
            var translation = RunTranslationEntityToModelTest<ServiceHours, VmOpeningHour>(translators, entity);
            CheckTranslation(entity, translation);
        }

        private static void CheckTranslation(ServiceHours source, VmOpeningHour translation)
        {
            if (source.AdditionalInformations != null)
            {
                translation.Name.Count.Should().Be(source.AdditionalInformations.Count);
                if (source.AdditionalInformations.Count > 0)
                {
                    translation.Name.Should().ContainValues(source.AdditionalInformations.Select(x => x.Text));
                }
            }
            else
            {
                translation.Name.Should().BeNull();
            }
            translation.Id.Should().Be(source.Id);
            translation.DateFrom.Should().Be(source.OpeningHoursFrom.ToEpochTime());
            translation.DateTo.Should().Be(source.OpeningHoursTo.ToEpochTime());
            translation.OrderNumber.Should().Be(source.OrderNumber);
        }

        private ServiceHours CreateModel(ServiceHoursTypeEnum shType)
        {
            return new ServiceHours
            {
                Id = Guid.NewGuid(),
                ServiceHourTypeId = shType.ToString().GetGuid(),
                AdditionalInformations = new HashSet<ServiceHoursAdditionalInformation>(),
                DailyOpeningTimes = new HashSet<DailyOpeningTime>()
            };
        }
    }
}
