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

using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for timed publishing.
    /// </summary>
    public class TimedPublishingBaseValidator<TModel> : BaseValidator<TModel> where TModel : IOpenApiTimedPublishing
    {
        /// <summary>
        /// Ctor - base for timed publishing item validator
        /// </summary>
        public TimedPublishingBaseValidator(TModel model, string propertyName) :base(model, propertyName)
        {
        }

        /// <summary>
        /// Checks if service model is valid or not.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {
            // Check that ValidTo is greater than ValidFrom (SFIPTV-190)
            if (Model.ValidFrom.HasValue && Model.ValidTo.HasValue && Model.ValidFrom.Value.Date >= Model.ValidTo.Value.Date)
            {
                modelState.AddModelError("ValidTo", $"Archiving date must be greater than publishing date.");
            }

            // SFIPTV-608
            // If ValidFrom is defined and is set into future, publishing status needs to be set as Draft or Modified.
            if (Model.ValidFrom.HasValue)
            {
                if (Model.PublishingStatus != PublishingStatus.Draft.ToString() && Model.PublishingStatus != PublishingStatus.Modified.ToString())
                {
                    modelState.AddModelError("PublishingStatus", $"You cannot set Publishing status as {Model.PublishingStatus} when ValidFrom is set to {Model.ValidFrom}.");
                }
            }
            else
            {
                // If ValidTo is defined (and ValidFrom not), publishing status needs to be set as Published.
                if (Model.ValidTo.HasValue && Model.PublishingStatus != PublishingStatus.Published.ToString())
                {
                    modelState.AddModelError("PublishingStatus", $"You cannot set Publishing status as {Model.PublishingStatus} when ValidTo is set to {Model.ValidTo} and ValidFrom is not set.");
                }
            }
        }
    }
}
