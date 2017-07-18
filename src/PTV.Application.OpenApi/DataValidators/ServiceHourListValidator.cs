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
        public ServiceHourListValidator(IList<TModel> model) : base(model, "ServiceHours")
        {
        }

        /// <summary>
        /// Checks if address list is valid or not.
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
                h.OpeningHour.ForEach(o =>
                {
                    int dayFrom = (int)o.DayFrom.Parse<WeekDayEnum>();
                    if (o.IsExtra)
                    {
                        if (!dayFromListExtra.Add(dayFrom))
                        {
                            modelState.AddModelError($"{PropertyName}[{i}].OpeningHour[{j}].DayFrom", $"Invalid dayFrom value '{ o.DayFrom }'. Cannot use same value twice!");
                        }
                    }
                    else
                    {
                        if (!dayFromList.Add(dayFrom))
                        {
                            modelState.AddModelError($"{PropertyName}[{i}].OpeningHour[{j}].DayFrom", $"Invalid dayFrom value '{ o.DayFrom }'. Cannot use same value twice!");                        
                        }
                    }                    
                    j++;
                });

                i++;
            });
        }
    }
}
