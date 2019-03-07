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

using PTV.Domain.Model.Models.OpenApi.V4;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Framework;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System;
using PTV.Domain.Model.Enums;

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
    /// Validator for service hour list.
    /// </summary>
    public class ServiceHourValidator<TModel, TModelOpeningTime> : BaseValidator<TModel>
        where TModel : IVmOpenApiServiceHourBase<TModelOpeningTime>
        where TModelOpeningTime : IVmOpenApiDailyOpeningTime
    {
        /// <summary>
        /// Ctor - service hour list validator.
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
                if (Model.ValidFrom.Value > Model.ValidTo)
                {
                    modelState.AddModelError($"{PropertyName}", "ValidTo cannot be earlier than ValidFrom.");
                }
            }

            // Check opening hours
            var dayFromList = new HashSet<int>();
            var dayFromListExtra = new HashSet<int>();
            var j = 0;

            var serviceHourType = Model.ServiceHourType.Parse<ServiceHoursTypeEnum>();

            if (Model.OpeningHour?.Count > 0)
            {
            // Only standard hours can have multiple service hours attached.
            if ((serviceHourType == ServiceHoursTypeEnum.Exception || serviceHourType == ServiceHoursTypeEnum.Special) && Model.OpeningHour.Count > 1)
            {
                modelState.AddModelError($"{PropertyName}", "ServiceHour with ServiceHourType 'Exception' or 'Special' can only contain one OpeningHour item.");
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
                            TimeSpan from, to;
                            if (TimeSpan.TryParse(o.From, out from) && TimeSpan.TryParse(o.To, out to))
                            {
                                if (from > to)
                                {
                                    modelState.AddModelError($"{PropertyName}.OpeningHour[{j}].To", $"To cannot be earlier than From.");
                                }
                            }
                        }
                    }
                }
                else if (serviceHourType != ServiceHoursTypeEnum.Exception)
                {
                    modelState.AddModelError($"{PropertyName}.OpeningHour[{j}].DayFrom", "The DayFrom field is required.");
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
                if (!(serviceHourType == ServiceHoursTypeEnum.Standard || (serviceHourType == ServiceHoursTypeEnum.Exception && Model.IsClosed == true)))
                {
                    modelState.AddModelError($"{PropertyName}.OpeningHour", "The OpeningHour field is required.");
                }
            }

            // Check IsExtra field for V4VmOpenApiServiceHour. PTV-3875
            if (Model is V4VmOpenApiServiceHour)
            {
                var itemValidator = new ServiceHourIsExtraValidator(Model as V4VmOpenApiServiceHour, $"{PropertyName}");
                itemValidator.Validate(modelState);
            }
        }
    }
}