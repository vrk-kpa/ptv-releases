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

using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.V2
{
    internal abstract class V2OpenApiServiceChannelBaseTranslator<TFromModel, TToModel> : Translator<TFromModel, TToModel>
        where TFromModel : class, IVmOpenApiServiceChannelBase, new() where TToModel : class, IVmOpenApiServiceChannelBase//, new()
    {
        public V2OpenApiServiceChannelBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override TToModel TranslateEntityToVm(TFromModel entity)
        {
            var model = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus)
                .GetFinal();

            model.ServiceChannelDescriptions = entity.ServiceChannelDescriptions;
            model.ServiceHours = entity.ServiceHours;
            model.Languages = entity.Languages;
            model.WebPages = entity.WebPages;

            return model;
        }

        public override TFromModel TranslateVmToEntity(TToModel vModel)
        {
            var model = CreateViewModelEntityDefinition(vModel)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus)
                .GetFinal();

            model.ServiceChannelDescriptions = vModel.ServiceChannelDescriptions;
            model.ServiceHours = vModel.ServiceHours;
            model.Languages = vModel.Languages;
            model.WebPages = vModel.WebPages;
            return model;
        }

        protected IList<VmOpenApiSupport> TranslateToV1SupportContacts(IList<VmOpenApiLanguageItem> emails, IList<VmOpenApiPhone> phoneNumbers)
        {
            var list = new List<VmOpenApiSupport>();
            emails.ForEach(e => list.Add(new VmOpenApiSupport() {
                Email = e.Value,
                Language = e.Language
            }));
            phoneNumbers.ForEach(p => list.Add(new VmOpenApiSupport() {
                Phone = p.Number,
                PhoneChargeDescription = p.ChargeDescription,
                Language = p.Language,
                ServiceChargeTypes = new List<string>() { p.ServiceChargeType }
            }));
            return list;
        }

        protected IList<VmOpenApiLanguageItem> TranslateToV2SupportEmails(IList<VmOpenApiSupport> supportEmails)
        {
            var list = new List<VmOpenApiLanguageItem>();
            supportEmails.Where(e => !string.IsNullOrEmpty(e.Email)).ForEach(e => list.Add(new VmOpenApiLanguageItem() {
                OwnerReferenceId = e.OwnerReferenceId,
                Value = e.Email,
                Language = e.Language
            }));

            return list;
        }

        protected IList<VmOpenApiPhone> TranslateToV2SupportPhones(IList<VmOpenApiSupport> supportPhones)
        {
            var list = new List<VmOpenApiPhone>();

            supportPhones.Where(p => !string.IsNullOrEmpty(p.Phone)).ForEach(p => list.Add(new VmOpenApiPhone()
            {
                OwnerReferenceId = p.OwnerReferenceId,
                Language = p.Language,
                Number = p.Phone,
                ChargeDescription = p.PhoneChargeDescription,
                ServiceChargeType = p.ServiceChargeTypes.First()
            }));

            return list;
        }

        protected IReadOnlyList<VmOpenApiServiceHour> TranslateToV1ServiceHours(IReadOnlyList<V2VmOpenApiServiceHour> serviceHours)
        {
            var list = new List<VmOpenApiServiceHour>();
            serviceHours.ForEach(h => list.AddRange(TranslateServiceHours(h)));
            return list;
        }

        protected IReadOnlyList<V2VmOpenApiServiceHour> TranslateToV2ServiceHours(IReadOnlyList<VmOpenApiServiceHour> serviceHours)
        {
            var list = new List<V2VmOpenApiServiceHour>();
            serviceHours.ForEach(h => list.Add(TranslateServiceHours(h)));
            return list;
        }

        private List<VmOpenApiServiceHour> TranslateServiceHours(V2VmOpenApiServiceHour serviceHour)
        {
            if (serviceHour.OpeningHour.Count == 0)
            {
                return new List<VmOpenApiServiceHour>() { GetBaseServiceHour(serviceHour) };
            }

            if (serviceHour.ServiceHourType == ServiceHoursTypeEnum.Standard.ToString())
            {
                return GetStandardHours(serviceHour);
            }

            return GetHours(serviceHour);
        }

        private List<VmOpenApiServiceHour> GetStandardHours(V2VmOpenApiServiceHour v2ServiceHour)
        {
            var list = new List<VmOpenApiServiceHour>();
            VmOpenApiServiceHour v1ServiceHour = null;
            var sortedOpeningHours = v2ServiceHour.OpeningHour.OrderBy(o => o.From).OrderBy(o => o.To).ToList();
            sortedOpeningHours.ForEach(o =>
            {
                if (v1ServiceHour == null)
                {
                    v1ServiceHour = GetStandardHour(v2ServiceHour, o);
                }
                else if (o.From.ToString() == v1ServiceHour.Opens && o.To.ToString() == v1ServiceHour.Closes)
                {
                    UpdateServiceHours(v1ServiceHour, o);
                }
                else
                {
                    list.Add(v1ServiceHour);
                    v1ServiceHour = GetStandardHour(v2ServiceHour, o);
                }
            });
            if (v1ServiceHour != null)
            {
                list.Add(v1ServiceHour);
            }
            return list;
        }

        private VmOpenApiServiceHour GetStandardHour(V2VmOpenApiServiceHour v2ServiceHour, V2VmOpenApiDailyOpeningTime v2OpeningTime)
        {
            var serviceHour = GetBaseServiceHour(v2ServiceHour);
            serviceHour.Monday = v2OpeningTime.DayFrom == WeekDayEnum.Monday.ToString();
            serviceHour.Tuesday = v2OpeningTime.DayFrom == WeekDayEnum.Tuesday.ToString();
            serviceHour.Wednesday = v2OpeningTime.DayFrom == WeekDayEnum.Wednesday.ToString();
            serviceHour.Thursday = v2OpeningTime.DayFrom == WeekDayEnum.Thursday.ToString();
            serviceHour.Friday = v2OpeningTime.DayFrom == WeekDayEnum.Friday.ToString();
            serviceHour.Saturday = v2OpeningTime.DayFrom == WeekDayEnum.Saturday.ToString();
            serviceHour.Sunday = v2OpeningTime.DayFrom == WeekDayEnum.Sunday.ToString();
            serviceHour.Opens = v2OpeningTime.From.ToString();
            serviceHour.Closes = v2OpeningTime.To.ToString();

            return serviceHour;
        }

        private List<VmOpenApiServiceHour> GetHours(V2VmOpenApiServiceHour v2ServiceHour)
        {
            var list = new List<VmOpenApiServiceHour>();
            v2ServiceHour.OpeningHour.ForEach(o =>
            {
                list.Add(new VmOpenApiServiceHour()
                {
                    ServiceHourType = v2ServiceHour.ServiceHourType,
                    ValidFrom = v2ServiceHour.ValidFrom,
                    ValidTo = v2ServiceHour.ValidTo,
                    ExceptionHourType = v2ServiceHour.IsClosed ? ExceptionHoursStatus.Closed.ToString() : ExceptionHoursStatus.Open.ToString(),
                    AdditionalInformation = v2ServiceHour.AdditionalInformation,
                    Monday = IsValidForDay((int)WeekDayEnum.Monday, o),
                    Tuesday = IsValidForDay((int)WeekDayEnum.Tuesday, o),
                    Wednesday = IsValidForDay((int)WeekDayEnum.Wednesday, o),
                    Thursday = IsValidForDay((int)WeekDayEnum.Thursday, o),
                    Friday = IsValidForDay((int)WeekDayEnum.Friday, o),
                    Saturday = IsValidForDay((int)WeekDayEnum.Saturday, o),
                    Sunday = IsValidForDay((int)WeekDayEnum.Sunday, o),
                    Opens = o.From.ToString(),
                    Closes = o.To.ToString(),
                });
            });
            return list;
        }

        private VmOpenApiServiceHour GetBaseServiceHour(V2VmOpenApiServiceHour v2ServiceHour)
        {
            return new VmOpenApiServiceHour()
            {
                ServiceHourType = v2ServiceHour.ServiceHourType,
                ValidFrom = v2ServiceHour.ValidFrom,
                ValidTo = v2ServiceHour.ValidTo,
                ExceptionHourType = v2ServiceHour.IsClosed ? ExceptionHoursStatus.Closed.ToString() : ExceptionHoursStatus.Open.ToString(),
                AdditionalInformation = v2ServiceHour.AdditionalInformation,
            };
        }
        private void UpdateServiceHours(VmOpenApiServiceHour v1ServiceHour, V2VmOpenApiDailyOpeningTime v2OpeningTime)
        {
            if (v2OpeningTime.DayFrom == WeekDayEnum.Monday.ToString())
            {
                v1ServiceHour.Monday = true;
                return;
            }
            if (v2OpeningTime.DayFrom == WeekDayEnum.Tuesday.ToString())
            {
                v1ServiceHour.Tuesday = true;
                return;
            }
            if (v2OpeningTime.DayFrom == WeekDayEnum.Wednesday.ToString())
            {
                v1ServiceHour.Wednesday = true;
                return;
            }
            if (v2OpeningTime.DayFrom == WeekDayEnum.Thursday.ToString())
            {
                v1ServiceHour.Thursday = true;
                return;
            }
            if (v2OpeningTime.DayFrom == WeekDayEnum.Friday.ToString())
            {
                v1ServiceHour.Friday = true;
                return;
            }
            if (v2OpeningTime.DayFrom == WeekDayEnum.Saturday.ToString())
            {
                v1ServiceHour.Saturday = true;
                return;
            }
            if (v2OpeningTime.DayFrom == WeekDayEnum.Sunday.ToString())
            {
                v1ServiceHour.Sunday = true;
                return;
            }
        }

        private V2VmOpenApiServiceHour TranslateServiceHours(VmOpenApiServiceHour serviceHour)
        {
            var periods = GetPeriods(serviceHour);
            var timeList = new List<V2VmOpenApiDailyOpeningTime>();
            periods.ForEach(p => timeList.Add(new V2VmOpenApiDailyOpeningTime()
            {
                DayFrom = p.ToString(),
                From = serviceHour.Opens,
                To = serviceHour.Closes,
            }));

            return new V2VmOpenApiServiceHour()
            {
                ServiceHourType = serviceHour.ServiceHourType,
                ValidFrom = serviceHour.ValidFrom,
                ValidTo = serviceHour.ValidTo,
                IsClosed = serviceHour.ExceptionHourType == ExceptionHoursStatus.Closed.ToString(),
                AdditionalInformation = serviceHour.AdditionalInformation,
                OpeningHour = timeList
            };
        }

        private bool IsValidForDay(int day, V2VmOpenApiDailyOpeningTime dailyOpeningTime)
        {
            int dayFrom = GetWeekDayNumber(dailyOpeningTime.DayFrom);
            int dayTo = GetWeekDayNumber(dailyOpeningTime.DayTo);

            if (dayFrom < 0)
            {
                throw new Exception($"DayFrom field is invalid: { dailyOpeningTime.DayFrom }.");
            }

            if (dayTo < 0)
            {   // There was no value in DayTo field so we only need to match the DayFrom field
                return day == dayFrom;
            }
            else
            {   // The opening time is related to certain time frame ( DayForm - DayTo ).
                if (day >= dayFrom && day <= dayTo)
                    return true;

                return false;
            }
        }

        private int GetWeekDayNumber(string strDay)
        {
            if (string.IsNullOrEmpty(strDay))
                return -1;

            return (int)((WeekDayEnum)Enum.Parse(typeof(WeekDayEnum), strDay));
        }

        private List<int> GetPeriods(VmOpenApiServiceHour serviceHour)
        {
            var list = new List<int>();
            if (serviceHour.Monday)
            {
                list.Add((int)WeekDayEnum.Monday);
            }
            if (serviceHour.Tuesday)
            {
                list.Add((int)WeekDayEnum.Tuesday);
            }
            if (serviceHour.Wednesday)
            {
                list.Add((int)WeekDayEnum.Wednesday);
            }
            if (serviceHour.Thursday)
            {
                list.Add((int)WeekDayEnum.Thursday);
            }
            if (serviceHour.Friday)
            {
                list.Add((int)WeekDayEnum.Friday);
            }
            if (serviceHour.Saturday)
            {
                list.Add((int)WeekDayEnum.Saturday);
            }
            if (serviceHour.Sunday)
            {
                list.Add((int)WeekDayEnum.Sunday);
            }
            return list;
        }
    }
}
