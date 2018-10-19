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
using PTV.Framework;
using System;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for publishing status.
    /// </summary>
    public class PublishingStatusValidator : BaseValidator<string>
    {
        private string currentStatus;

        /// <summary>
        /// Ctor - municipality code validator.
        /// </summary>
        /// <param name="model">Publishing status code</param>
        /// <param name="currentStatus">Current publishing status</param>
        /// <param name="propertyName">Property name</param>
        public PublishingStatusValidator(string model, string currentStatus, string propertyName = "PublishingStatus") : base(model, propertyName)
        {
            this.currentStatus = currentStatus;
        }

        /// <summary>
        /// Checks if publishing status is valid or not.
        /// Allowed updates for publishing status:
        /// Draft/null -> Draft or Published
        /// Published  -> Modified, Published, Archived
        /// Archived   -> Modified, Published, Archived
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            PublishingStatus? newPublishingStatus = null;
            if (!string.IsNullOrEmpty(Model)) { newPublishingStatus = Model.Parse<PublishingStatus>(); }

            if (string.IsNullOrEmpty(currentStatus))
            {
                if (newPublishingStatus.HasValue && newPublishingStatus != PublishingStatus.Draft && newPublishingStatus != PublishingStatus.Published)
                {
                    modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.PublishingStatusNotValid, newPublishingStatus, currentStatus));
                }
                return;
            }

            PublishingStatus currentPublishingStatus = currentStatus.Parse<PublishingStatus>();

            // Validation rules can be found from PTV-1173.
            switch (currentPublishingStatus)
            {
                case PublishingStatus.Draft:
                    if (newPublishingStatus.HasValue && newPublishingStatus != PublishingStatus.Draft && newPublishingStatus != PublishingStatus.Published)
                    {
                        modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.PublishingStatusNotValid, newPublishingStatus, currentStatus));
                    }
                    break;
                case PublishingStatus.Published:
                    if (newPublishingStatus.HasValue && newPublishingStatus != PublishingStatus.Modified && newPublishingStatus != PublishingStatus.Published && newPublishingStatus != PublishingStatus.Deleted)
                    {
                        modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.PublishingStatusNotValid, newPublishingStatus, currentStatus));
                    }
                    break;
                case PublishingStatus.Deleted:
                    if (!newPublishingStatus.HasValue)
                    {
                        modelState.AddModelError(PropertyName, $"You need to define Publishing status when current one is {currentStatus}.");
                        break;
                    }
                    if (newPublishingStatus.HasValue && newPublishingStatus != PublishingStatus.Modified && newPublishingStatus != PublishingStatus.Published && newPublishingStatus != PublishingStatus.Deleted)
                    {
                        modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.PublishingStatusNotValid, newPublishingStatus, currentStatus));
                    }
                    break;
                case PublishingStatus.Modified:
                    modelState.AddModelError(PropertyName, "You cannot update entity with status Modified.");
                    break;
                case PublishingStatus.OldPublished:
                    modelState.AddModelError(PropertyName, "You cannot update entity with status OldPublished.");
                    break;
                default:
                    break;
            }
        }
    }
}
