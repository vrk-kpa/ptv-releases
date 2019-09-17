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
using PTV.Database.DataAccess.Translators.Mass;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Mass;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Mass
{
    public class MassLanguageAvailabilityTranslatorTest : TranslatorTestBase
    {
        [Theory]
        [MemberData(nameof(TranslateScheduledArchiveData))]
        public void TranslateScheduledArchive(DateTime? validTo, PublishActionTypeEnum publishAction)
        {
            var model = CreateModel(null, validTo, publishAction);
            var translators = RegisterTranslators();

            var translation = RunTranslationModelToEntityTest<VmMassLanguageAvailabilityModel, ServiceLanguageAvailability>(
                translators,
                model);
            
            Assert.Equal(validTo, translation.ArchiveAt);
            Assert.Null(translation.LastFailedPublishAt);

            if (validTo == null)
            {
                Assert.Null(translation.SetForArchived);
                Assert.Null(translation.SetForArchivedBy);
            }
            else
            {
                Assert.Equal(ArchivedDate, translation.SetForArchived);
                Assert.Equal(ArchivedByName, translation.SetForArchivedBy);
            }
        }

        [Theory]
        [MemberData(nameof(TranslateScheduledPublishData))]
        public void TranslateScheduledPublish(DateTime? validFrom)
        {
            var model = CreateModel(validFrom, null, PublishActionTypeEnum.SchedulePublish);
            var translators = RegisterTranslators();

            var translation =
                RunTranslationModelToEntityTest<VmMassLanguageAvailabilityModel, ServiceLanguageAvailability>(
                    translators,
                    model);
            
            Assert.Equal(validFrom, translation.PublishAt);
            Assert.Null(translation.LastFailedPublishAt);

            if (validFrom == null)
            {
                Assert.Null(translation.Reviewed);
                Assert.Null(translation.ReviewedBy);
            }
            else
            {
                Assert.Equal(ReviewedDate, translation.Reviewed);
                Assert.Equal(ReviewedByName, translation.ReviewedBy);
            }
        }

        [Theory]
        [MemberData(nameof(TranslateScheduledPublishArchiveData))]
        public void TranslateScheduledPublishArchive(DateTime? validFrom, DateTime? validTo)
        {
            var model = CreateModel(validFrom, validTo, PublishActionTypeEnum.SchedulePublishArchive);
            var translators = RegisterTranslators();

            var translation =
                RunTranslationModelToEntityTest<VmMassLanguageAvailabilityModel, ServiceLanguageAvailability>(
                    translators,
                    model);
            
            Assert.Equal(validFrom, translation.PublishAt);
            Assert.Equal(validTo, translation.ArchiveAt);
            Assert.Null(translation.LastFailedPublishAt);
            Assert.Equal(ReviewedDate, translation.Reviewed);
            Assert.Equal(ReviewedByName, translation.ReviewedBy);

            if (validTo == null)
            {
                Assert.Null(translation.SetForArchived);
                Assert.Null(translation.SetForArchivedBy);
            }
            else
            {
                Assert.Equal(ArchivedDate, translation.SetForArchived);
                Assert.Equal(ArchivedByName, translation.SetForArchivedBy);
            }
        }

        private List<object> RegisterTranslators()
        {
            var translators = new List<object>
            {
                new ServiceLanguageAvailabilityTranslator(ResolveManager, TranslationPrimitives),
                new MassLanguageAvailabilityTranslator(ResolveManager, TranslationPrimitives)
            };

            return translators;
        }

        private VmMassLanguageAvailabilityModel CreateModel(DateTime? validFrom, DateTime? validTo, PublishActionTypeEnum publishAction)
        {
            return new VmMassLanguageAvailabilityModel
            {
                ValidTo = validTo,
                Archived = ArchivedDate,
                ArchivedBy = ArchivedByName,
                ValidFrom = validFrom,
                Reviewed = ReviewedDate,
                ReviewedBy = ReviewedByName,
                AllowSaveNull = true,
                PublishAction = publishAction
            };
        }
        
        private static readonly DateTime ValidFromDate = new DateTime(2015, 7, 7);
        private static readonly DateTime ValidToDate = new DateTime(2020, 8, 8);
        private static readonly DateTime ArchivedDate = new DateTime(2016, 9, 9);
        private const string ArchivedByName = "archivist";
        private static readonly DateTime ReviewedDate = new DateTime(2017, 10, 10);
        private const string ReviewedByName = "reviewer";

        public static IEnumerable<object[]> TranslateScheduledArchiveData()
        {
            var allData = new List<object[]>
            {
                new object[] {(DateTime?) null, PublishActionTypeEnum.ScheduleArchive},
                new object[] {ValidToDate, PublishActionTypeEnum.ScheduleArchive},
                new object[] {(DateTime?) null, PublishActionTypeEnum.SchedulePublishArchive},
                new object[] {ValidToDate, PublishActionTypeEnum.SchedulePublishArchive}
            };

            return allData;
        }

        public static IEnumerable<object[]> TranslateScheduledPublishData()
        {
            var allData = new List<object[]>
            {
                new object[] {null},
                new object[] {ValidFromDate}
            };

            return allData;
        }

        public static IEnumerable<object[]> TranslateScheduledPublishArchiveData()
        {
            var allData = new List<object[]>
            {
                new object[] {ValidFromDate, null},
                new object[] {ValidFromDate, ValidToDate}
            };

            return allData;
        }
    }
}