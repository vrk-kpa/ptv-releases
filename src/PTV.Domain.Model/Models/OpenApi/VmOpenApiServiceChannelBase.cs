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
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service channel - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceChannelBase" />
    public class VmOpenApiServiceChannelBase : IVmOpenApiServiceChannelBase
    {
        /// <summary>
        /// PTV identifier for the service channel.
        /// </summary>
        [JsonProperty(Order = 1)]
        public virtual Guid? Id { get; set; }

        /// <summary>
        /// List of localized service channel descriptions.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListValueNotEmpty("Value")]
        [ListWithEnum(typeof(DescriptionTypeEnum), "Type")]
        [ListPropertyMaxLength(150, "Value", "ShortDescription")]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        public virtual IList<VmOpenApiLocalizedListItem> ServiceChannelDescriptions { get; set; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [JsonProperty(Order = 15)]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<V4VmOpenApiPhone> SupportPhones { get; set; } = new List<V4VmOpenApiPhone>();

        /// <summary>
        /// List of support email addresses for the service channel.
        /// </summary>
        [JsonProperty(Order = 16)]
        [EmailAddressList("Value")]
        [LocalizedListLanguageDuplicityForbidden]
        [ListPropertyMaxLength(100, "Value")]
        public virtual IList<VmOpenApiLanguageItem> SupportEmails { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of languages the service channel is available in (two letter language code).
        /// </summary>
        [JsonProperty(Order = 17)]
        [ListRegularExpression(@"^[a-z]{2}$")]
        public virtual IList<string> Languages { get; set; }

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonProperty(Order = 19)]
        [LocalizedListPropertyDuplicityForbidden("OrderNumber")]
        public virtual IList<VmOpenApiWebPageWithOrderNumber> WebPages { get; set; } = new List<VmOpenApiWebPageWithOrderNumber>();

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public virtual IList<V4VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V4VmOpenApiServiceHour>();

        /// <summary>
        /// Service channel publishing status. Values: Draft, Published, Deleted, Modified or OldPublished.
        /// </summary>
        [JsonProperty(Order = 30)]
        [ValidEnum(typeof(PublishingStatus))]
        public virtual string PublishingStatus { get; set; }

        /// <summary>
        /// Gets or sets availble languages
        /// </summary>
        [JsonIgnore]
        public IList<string> AvailableLanguages { get; set; }

        /// <summary>
        /// Gets or sets the special channel identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonIgnore]
        public Guid? ChannelId { get; set; }

        #region Methods
        /// <summary>
        /// Gets the service channel base model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>service channel base model</returns>
        protected TModel GetServiceChannelBaseModel<TModel>() where TModel : IVmOpenApiServiceChannelBase, new()
        {
            return new TModel
            {
                Id = this.Id,
                ServiceChannelDescriptions = this.ServiceChannelDescriptions,
                SupportPhones = this.SupportPhones,
                SupportEmails = this.SupportEmails,
                Languages = this.Languages,
                WebPages = this.WebPages,
                ServiceHours = this.ServiceHours,
                PublishingStatus = this.PublishingStatus,
                AvailableLanguages = this.AvailableLanguages,
                ChannelId = this.ChannelId,
            };
        }

        /// <summary>
        /// Translates to v1 support contacts.
        /// </summary>
        /// <param name="emails">The emails.</param>
        /// <param name="phoneNumbers">The phone numbers.</param>
        /// <returns>translated support contacts</returns>
        protected IList<VmOpenApiSupport> TranslateToV1SupportContacts(IList<VmOpenApiLanguageItem> emails, IList<VmOpenApiPhone> phoneNumbers)
        {
            var list = new List<VmOpenApiSupport>();
            emails.ForEach(e => list.Add(new VmOpenApiSupport()
            {
                Email = e.Value,
                Language = e.Language
            }));
            phoneNumbers.ForEach(p => list.Add(new VmOpenApiSupport()
            {
                Phone = string.IsNullOrEmpty(p.PrefixNumber) ? p.Number : $"{p.PrefixNumber} {p.Number}",
                PhoneChargeDescription = p.ChargeDescription,
                Language = p.Language,
                ServiceChargeTypes = new List<string>() { p.ServiceChargeType }
            }));
            return list;
        }

        /// <summary>
        /// Translates to v2 support emails.
        /// </summary>
        /// <param name="supportEmails">The support emails.</param>
        /// <returns>translated support emails</returns>
        protected IList<VmOpenApiLanguageItem> TranslateToV2SupportEmails(IList<VmOpenApiSupport> supportEmails)
        {
            var list = new List<VmOpenApiLanguageItem>();
            supportEmails.Where(e => !string.IsNullOrEmpty(e.Email)).ForEach(e => list.Add(new VmOpenApiLanguageItem()
            {
                OwnerReferenceId = e.OwnerReferenceId,
                Value = e.Email,
                Language = e.Language
            }));

            return list;
        }

        /// <summary>
        /// Translates to a support phone model that the current latest version uses.
        /// </summary>
        /// <param name="supportPhones">The support phones.</param>
        /// <returns>translated support phones</returns>
        protected IList<V4VmOpenApiPhone> TranslateToVersionBaseSupportPhones(IList<VmOpenApiSupport> supportPhones)
        {
            var list = new List<V4VmOpenApiPhone>();

            supportPhones.Where(p => !string.IsNullOrEmpty(p.Phone)).ForEach(p => list.Add(new V4VmOpenApiPhone()
            {
                OwnerReferenceId = p.OwnerReferenceId,
                Language = p.Language,
                Number = p.Phone,
                ChargeDescription = p.PhoneChargeDescription,
                ServiceChargeType = p.ServiceChargeTypes.First()
            }));

            return list;
        }

        /// <summary>
        /// Translates to v1 service hours.
        /// </summary>
        /// <param name="serviceHours">The service hours.</param>
        /// <returns>translated service hours</returns>
        protected IReadOnlyList<VmOpenApiServiceHour> TranslateToV1ServiceHours(IList<V2VmOpenApiServiceHour> serviceHours)
        {
            var list = new List<VmOpenApiServiceHour>();
            serviceHours.ForEach(h => list.AddRange(TranslateServiceHours(h)));
            return list;
        }

        /// <summary>
        /// Translates to v4 service hours.
        /// </summary>
        /// <param name="serviceHours">The service hours.</param>
        /// <returns>translated service hours</returns>
        protected IList<V4VmOpenApiServiceHour> TranslateToV4ServiceHours(IList<VmOpenApiServiceHour> serviceHours)
        {
            var list = new List<V4VmOpenApiServiceHour>();
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

        private V4VmOpenApiServiceHour TranslateServiceHours(VmOpenApiServiceHour serviceHour)
        {
            var periods = GetPeriods(serviceHour);
            var timeList = new List<V2VmOpenApiDailyOpeningTime>();
            periods.ForEach(p => timeList.Add(new V2VmOpenApiDailyOpeningTime()
            {
                DayFrom = p.ToString(),
                From = serviceHour.Opens,
                To = serviceHour.Closes,
            }));

            return new V4VmOpenApiServiceHour()
            {
                ServiceHourType = serviceHour.ServiceHourType,
                ValidFrom = serviceHour.ValidFrom,
                ValidTo = serviceHour.ValidTo,
                IsClosed = serviceHour.ExceptionHourType == ExceptionHoursStatus.Closed.ToString(),
                AdditionalInformation = serviceHour.AdditionalInformation.SetListValueLength(100),
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
        #endregion
    }
}
