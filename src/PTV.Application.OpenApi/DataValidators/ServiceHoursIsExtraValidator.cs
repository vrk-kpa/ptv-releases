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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for IsExtra property for service hour list.
    /// </summary>
    public class ServiceHoursIsExtraValidator : BaseValidator<IList<V4VmOpenApiServiceHour>>
    {
        /// <summary>
        /// Ctor - service hour list validator.
        /// </summary>
        /// <param name="model">Service hour list</param>
        /// <param name="propertyName"></param>
        public ServiceHoursIsExtraValidator(IList<V4VmOpenApiServiceHour> model, string propertyName = "ServiceHours") : base(model, propertyName)
        {
        }

        /// <summary>
        /// Checks if service hour list related to IsExtra property is valid or not.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            var i = 0;
            Model.ForEach(h =>
            {
                var itemValidator = new ServiceHourIsExtraValidator(h, $"{PropertyName}[{i}]");
                itemValidator.Validate(modelState);
                i++;
            });
        }
    }
    /// <summary>
    /// Validator for IsExtra property for service hour list.
    /// </summary>
    public class ServiceHourIsExtraValidator : BaseValidator<V4VmOpenApiServiceHour>
    {
        /// <summary>
        /// Ctor - service hour list validator.
        /// </summary>
        /// <param name="model">Service hour list</param>
        /// <param name="propertyName"></param>
        public ServiceHourIsExtraValidator(V4VmOpenApiServiceHour model, string propertyName = "ServiceHours") : base(model, propertyName)
        {
        }

        /// <summary>
        /// Checks if service hour list related to IsExtra property is valid or not.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            if (Model.OpeningHour?.Count > 0)
            {
                // If there are any opening hours where isExtra = true there also needs to be an opening hour with isExtra = false for that day.
                // There can be several opening hours with isExtra = true. PTV-3575
                var notExtraHours = Model.OpeningHour.Where(o => o.IsExtra == false).ToList();
                var extraHours = Model.OpeningHour.Where(o => o.IsExtra).ToList();
                if (extraHours.Count > 0)
                {
                    if (notExtraHours.Count == 0)
                    {
                        modelState.AddModelError($"{PropertyName}.OpeningHour", "Invalid list. One item with 'isExtra=false' is required!");
                    }
                    else
                    {
                        // Check that all itmes (where IsExtra = true) has an item for that day where isExtra = false
                        extraHours.ForEach(e =>
                        {
                            if (!notExtraHours.Any(n => n.DayFrom == e.DayFrom))
                            {
                                modelState.AddModelError($"{PropertyName}.OpeningHour", $"Invalid list. One item with 'isExtra=false' where 'DayFrom={e.DayFrom}' is required!");
                            }
                        });
                    }
                }
                //Only one item per day with isExtra = false is accepted.PTV - 3575
                if (notExtraHours.Count > 1)
                {
                    if (notExtraHours.GroupBy(n => n.DayFrom).Any(c => c.Count() > 1))
                    {
                        modelState.AddModelError($"{PropertyName}.OpeningHour", "Invalid list. Only one item with 'isExtra=false' for a day is accepted!");
                    }
                }
            }
        }
    }
}
