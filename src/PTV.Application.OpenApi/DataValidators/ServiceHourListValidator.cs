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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System;
using PTV.Domain.Model.Enums;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service hour list.
    /// </summary>
    public class ServiceHourListValidator<TModel> : BaseValidator<IList<TModel>> where TModel : IVmOpenApiServiceHourBase
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
                // Check start and end times
                if (h.ValidFrom.HasValue && h.ValidTo.HasValue)
                {
                    if (h.ValidFrom.Value > h.ValidTo)
                    {
                        modelState.AddModelError($"{PropertyName}[{i}]", "ValidTo cannot be earlier than ValidFrom.");
                    }
                }

                // Check opening hours
                var dayFromList = new HashSet<int>();
                var dayFromListExtra = new HashSet<int>();
                var j = 0;

                var serviceHourType = h.ServiceHourType.Parse<ServiceHoursTypeEnum>();

                if (h.OpeningHour?.Count > 0)
                {
                    // Only standard hours can have multiple service hours attached.
                    if ((serviceHourType == ServiceHoursTypeEnum.Exception || serviceHourType == ServiceHoursTypeEnum.Special) && h.OpeningHour.Count > 1)
                    {
                        modelState.AddModelError($"{PropertyName}[{i}]", "ServiceHour with ServiceHourType 'Exception' or 'Special' can only contain one OpeningHour item.");
                    }

                    // If there are any opening hours where isExtra = true there also needs to be an opening hour with isExtra = false for that day.
                    // There can be several opening hours with isExtra = true. PTV-3575
                    var notExtraHours = h.OpeningHour.Where(o => o.IsExtra == false).ToList();
                    var extraHours = h.OpeningHour.Where(o => o.IsExtra == true).ToList();
                    if (extraHours.Count > 0)
                    {
                        if (notExtraHours.Count == 0)
                        {
                            modelState.AddModelError($"{PropertyName}[{i}].OpeningHour", $"Invalid list. One item with 'isExtra=false' is required!");
                        }
                        else
                        {
                            // Check that all itmes (where IsExtra = true) has an item for that day where isExtra = false
                            extraHours.ForEach(e =>
                            {
                                if (!notExtraHours.Any(n => n.DayFrom == e.DayFrom))
                                {
                                    modelState.AddModelError($"{PropertyName}[{i}].OpeningHour", $"Invalid list. One item with 'isExtra=false' where 'DayFrom={e.DayFrom}' is required!");
                                }
                            });
                        }
                    }
                    // Only one item per day with isExtra=false is accepted. PTV-3575
                    if (notExtraHours.Count > 1)
                    {
                        if (notExtraHours.GroupBy(n => n.DayFrom).Any(c => c.Count() > 1))
                        {
                            modelState.AddModelError($"{PropertyName}[{i}].OpeningHour", $"Invalid list. Only one item with 'isExtra=false' for a day is accepted!");
                        }                        
                    }

                    h.OpeningHour.ForEach(o =>
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
                                            modelState.AddModelError($"{PropertyName}[{i}].OpeningHour[{j}].To", $"To cannot be earlier than From.");
                                        }
                                    }
                                }
                            }
                        }
                        else if (serviceHourType != ServiceHoursTypeEnum.Exception)
                        {
                            modelState.AddModelError($"{PropertyName}[{i}].OpeningHour[{j}].DayFrom", "The DayFrom field is required.");
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
                    if (!(serviceHourType == ServiceHoursTypeEnum.Standard || (serviceHourType == ServiceHoursTypeEnum.Exception && h.IsClosed == true)))
                    {
                        modelState.AddModelError($"{PropertyName}[{i}].OpeningHour", "The OpeningHour field is required.");
                    }
                }

                i++;
            });
        }
    }
}