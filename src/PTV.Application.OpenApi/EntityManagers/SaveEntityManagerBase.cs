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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;

namespace PTV.Application.OpenApi.EntityManagers
{
    /// <summary>
    /// Save entity base manager
    /// </summary>
    public abstract class SaveEntityManagerBase<TModelRequest, TModelResult> : EntityManagerBase<TModelRequest, TModelResult>
        where TModelRequest : IVmOpenApiEntityBase, IOpenApiInVersionBase<TModelRequest>
        where TModelResult : IVmEntityBase, IOpenApiPublishing
    {
        /// <summary>
        /// Entity identifier
        /// </summary>
        protected string Id;
        /// <summary>
        /// Entity external source id
        /// </summary>
        protected string SourceId;
        /// <summary>
        /// Current version of the entity
        /// </summary>
        protected TModelResult CurrentVersion;

        /// <summary>
        /// Ctrl
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="sourceId">External source id</param>
        /// <param name="model">the request model</param>
        /// <param name="openApiVersion">Open api version</param>
        /// <param name="modelState">Model state</param>
        /// <param name="logger">Logger</param>
        public SaveEntityManagerBase(
            string id,
            string sourceId,
            TModelRequest model,
            int openApiVersion,
            ModelStateDictionary modelState,
            ILogger logger)
            : base(model, openApiVersion, modelState, logger)
        {
            Id = id;
            SourceId = sourceId;
        }

        /// <summary>
        /// Check the request and parameters.
        /// </summary>
        /// <returns></returns>
        protected override IActionResult CheckRequestAndParameters()
        {
            var result = base.CheckRequestAndParameters();
            if (result != null) return result;

            if (!string.IsNullOrEmpty(Id))
            {
                ViewModel.Id = Id.ParseToGuid();

                if (!ViewModel.Id.HasValue)
                {
                    return new NotFoundObjectResult(new VmError() { ErrorMessage = GetErrorMessage() });
                }
            }
            else if (!string.IsNullOrEmpty(SourceId))
            {
                ViewModel.SourceId = SourceId;
            }
            else
            {
                return new NotFoundObjectResult(new VmError() { ErrorMessage = GetErrorMessage() });
            }

            // Check that entity exists
            CurrentVersion = CallCurrentVersionServiceMethod();
            if (CurrentVersion == null || string.IsNullOrEmpty(CurrentVersion.PublishingStatus))
            {
                return new NotFoundObjectResult(new VmError() { ErrorMessage = GetErrorMessage() });
            }

            return null;
        }

        /// <summary>
        /// The steps performed before validating the data within request view model.
        /// </summary>
        protected override void PreDataValidationSteps()
        {
            base.PreDataValidationSteps();
            ViewModel.CurrentPublishingStatus = CurrentVersion.PublishingStatus;
        }

        /// <summary>
        /// Get the entity related error message.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetErrorMessage();

        /// <summary>
        /// The method to call to get the current version of entity.
        /// </summary>
        /// <returns></returns>
        protected abstract TModelResult CallCurrentVersionServiceMethod();
    }
}
