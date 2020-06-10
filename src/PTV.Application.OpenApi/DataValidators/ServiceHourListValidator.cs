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

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service hour list.
    /// </summary>
    public class ServiceHourListValidator<TModel, TModelOpeningTime> : BaseValidator<IList<TModel>>
        where TModel : IVmOpenApiServiceHourBase<TModelOpeningTime>
        where TModelOpeningTime : IVmOpenApiDailyOpeningTime
    {
        /// <summary>
        /// Ctor - service hour list validator.
        /// </summary>
        /// <param name="model">Service hour list</param>
        /// <param name="propertyName"></param>
        public ServiceHourListValidator(IList<TModel> model, string propertyName = "ServiceHours") : base(model, propertyName)
        {
        }

        /// <summary>
        /// Checks if service hour list is valid or not.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            var i = 0;
            Model.ForEach(h =>
            {
                var itemValidator = new ServiceHourValidator<TModel, TModelOpeningTime>(h, $"{PropertyName}[{i}]");
                itemValidator.Validate(modelState);
                i++;
            });
        }
    }

    /// <summary>
    /// Validator for a service hour.
    /// </summary>
    public class ServiceHourValidator<TModel, TModelOpeningTime> : BaseValidator<TModel>
        where TModel : IVmOpenApiServiceHourBase<TModelOpeningTime>
        where TModelOpeningTime : IVmOpenApiDailyOpeningTime
    {
        /// <summary>
        /// Ctor - service hour validator.
        /// </summary>
        /// <param name="model">Service hour list</param>
        /// <param name="propertyName"></param>
        public ServiceHourValidator(TModel model, string propertyName = "ServiceHours") : base(model, propertyName)
        {
        }

        /// <summary>
        /// Checks if service hour is valid or not.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            // Check start and end times
            if (Model.ValidFrom.HasValue && Model.ValidTo.HasValue)
            {
                if (Model.ValidFrom.Value.Date >= Model.ValidTo.Value.Date)
                {
                    modelState.AddModelError($"{PropertyName}", "ValidTo cannot be earlier or the same than ValidFrom.");
                }
            }

            // Check opening hours
            var j = 0;

            var serviceHourType = (ServiceHoursTypeEnum)Model.ServiceHourType.GetEnumByOpenApiEnumValue(typeof(ServiceHoursTypeEnum));

            // With only standard hours the IsAlwaysOpen property can be set to true. - SFIPTV-1912
            if (Model.IsAlwaysOpen && (serviceHourType == ServiceHoursTypeEnum.Exception || serviceHourType == ServiceHoursTypeEnum.Special))
            {
                modelState.AddModelError($"{PropertyName}", "ServiceHour with ServiceHourType 'Exceptional' or 'OverMidnight' cannot have IsAlwaysOpen set to true.");
            }
            // IsAlwaysOpen and IsClosed cannot be true at the same time - SFIPTV-1912
            if (Model.IsAlwaysOpen && Model.IsClosed)
            {
                modelState.AddModelError($"{PropertyName}", "IsAlwaysOpen and IsClosed properties cannot both be set to true.");
            }

            // With only standard hours the IsReservation property can be set to true. - SFIPTV-1822
            if (Model.IsReservation && (serviceHourType == ServiceHoursTypeEnum.Exception || serviceHourType == ServiceHoursTypeEnum.Special))
            {
                modelState.AddModelError($"{PropertyName}", "ServiceHour with ServiceHourType 'Exceptional' or 'OverMidnight' cannot have IsReservation set to true.");
            }
            // IsReservation and IsClosed cannot be true at the same time - SFIPTV-1822
            if (Model.IsReservation && Model.IsClosed)
            {
                modelState.AddModelError($"{PropertyName}", "IsReservation and IsClosed properties cannot both be set to true.");
            }
            // IsReservation and IsAlwaysOpen cannot be true at the same time - SFIPTV-1822
            if (Model.IsReservation && Model.IsAlwaysOpen)
            {
                modelState.AddModelError($"{PropertyName}", "IsReservation and IsAlwaysOpen properties cannot both be set to true.");
            }

            if (Model.OpeningHour?.Count > 0)
            {
                // Only standard hours can have multiple service hours attached.
                if ((serviceHourType == ServiceHoursTypeEnum.Exception || serviceHourType == ServiceHoursTypeEnum.Special) && Model.OpeningHour.Count > 1)
                {
                    modelState.AddModelError($"{PropertyName}", "ServiceHour with ServiceHourType 'Exceptional' or 'OverMidnight' can only contain one OpeningHour item.");
                }

                // The hours should not be set if IsAlwaysOpen property is set to true. - SFIPTV-1912
                if (Model.IsAlwaysOpen)
                {
                    modelState.AddModelError($"{PropertyName}", "When IsAlwaysOpen property is set to true no OpeningHours should be set.");
                }

                // The hours should not be set if IsReservation property is set to true. - SFIPTV-1822
                if (Model.IsReservation)
                {
                    modelState.AddModelError($"{PropertyName}", "When IsReservation property is set to true no OpeningHours should be set.");
                }

                Model.OpeningHour.ForEach(o =>
                {
                    // DayFrom must be specified for any validation
                    if (!string.IsNullOrEmpty(o.DayFrom))
                    {
                        // Standard service hours cannot have opening hours where To is earlier than From (Special service hours can)
                        if (serviceHourType == ServiceHoursTypeEnum.Standard)
                        {
                            // On single day opening hours To cannot be earlier than From
                            if (string.IsNullOrEmpty(o.DayTo) || o.DayFrom.Equals(o.DayTo))
                            {
                                if (TimeSpan.TryParse(o.From, out var from) && TimeSpan.TryParse(o.To, out var to))
                                {
                                    if (from > to)
                                    {
                                        modelState.AddModelError($"{PropertyName}.OpeningHour[{j}].To", "To cannot be earlier than From.");
                                    }
                                }
                            }
                        }
                    }
                    else if (serviceHourType != ServiceHoursTypeEnum.Exception)
                    {
                        modelState.AddModelError($"{PropertyName}.OpeningHour[{j}].DayFrom", "The DayFrom field is required.");
                    }

                    // The time within To field cannot be earlier than within From field (SFIPTV-2029).
                    // The checking does not need to be done for Special hours (over midnight).
                    // For Exceptional hours the checking is only done if ValidTo is not set.
                    if (serviceHourType == ServiceHoursTypeEnum.Standard || (serviceHourType == ServiceHoursTypeEnum.Exception && !Model.ValidTo.HasValue))
                    {
                        if (!TimeIsValid(o.From, o.To))
                        {
                            modelState.AddModelError($"{PropertyName}.OpeningHour[{j}].To", "To cannot be earlier or the same than From.");
                        }
                    }
                    j++;
                });
            }
            else
            {
                // No opening hours set.
                // Opening hours list is not required if type is Exception and set as closed.
                // Opening hours list is not required if type is Standard.
                // For all other cases opening hours list need to be set! PTV-3586
                if (!(serviceHourType == ServiceHoursTypeEnum.Standard || (serviceHourType == ServiceHoursTypeEnum.Exception && Model.IsClosed)))
                {
                    modelState.AddModelError($"{PropertyName}.OpeningHour", "The OpeningHour field is required.");
                }
            }

            // Check IsExtra field for V4VmOpenApiServiceHour. PTV-3875
            var hour = Model as V4VmOpenApiServiceHour;
            if (hour != null)
            {
                var itemValidator = new ServiceHourIsExtraValidator(hour, $"{PropertyName}");
                itemValidator.Validate(modelState);
            }
        }

        private bool TimeIsValid(string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return true;
            }

            // User can add the time in either HH:mm or HH:mm:ss format so we need check the format.
            DateTime dateFrom = DateTime.ParseExact(from, GetTimeFormat(from), null, System.Globalization.DateTimeStyles.None);
            DateTime dateTo = DateTime.ParseExact(to, GetTimeFormat(to), null, System.Globalization.DateTimeStyles.None);
            if (dateFrom >= dateTo)
            {
                return false;
            }
            return true;
        }

        private string GetTimeFormat(string time)
        {
            if (time.Length == 5)
            {
                return "HH:mm";
            }

            return "HH:mm:ss";
        }
    }
}
