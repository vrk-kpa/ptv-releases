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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using PTV.Application.OpenApi.DataValidators;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using System;

namespace PTV.Application.OpenApi.EntityManagers
{
    /// <summary>
    /// Base class for entity managers.
    /// </summary>
    public abstract class EntityManagerBase<TModelRequest, TModelResult>
        where TModelRequest : IVmEntityBase, IOpenApiInVersionBase<TModelRequest>
        where TModelResult : IVmEntityBase
    {
        /// <summary>
        /// The view model
        /// </summary>
        protected TModelRequest ViewModel;
        /// <summary>
        /// The open api version
        /// </summary>
        protected int OpenApiVersion;
        /// <summary>
        /// The model state
        /// </summary>
        protected ModelStateDictionary ModelState;
        /// <summary>
        /// Logger
        /// </summary>
        protected ILogger Logger;

        /// <summary>
        /// Ctrl
        /// </summary>
        protected EntityManagerBase(TModelRequest model, int openApiVersion, ModelStateDictionary modelState, ILogger logger)
        {
            ViewModel = model;
            OpenApiVersion = openApiVersion;
            ModelState = modelState;
            Logger = logger;
        }

        /// <summary>
        /// The actions to be performed to add/save the entity.
        /// </summary>
        /// <returns></returns>
        public IActionResult PerformAction()
        {
            try
            {
                var result = CheckRequestAndParameters();
                if (result != null)
                {
                    return result;
                }

                // Validate the items
                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                PreDataValidationSteps();

                // Get the data validator and validate
                var validator = GetValidator();
                validator.Validate(ModelState);
                // Check validation status
                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                return new OkObjectResult(CallServiceMethod());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occured in EntityManagerBase. {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Check the request and parameters.
        /// </summary>
        protected virtual IActionResult CheckRequestAndParameters()
        {
            if (ViewModel == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            return null;
        }

        /// <summary>
        /// The steps performed before validating the data within request view model.
        /// </summary>
        protected virtual void PreDataValidationSteps()
        {
            ViewModel = ViewModel.VersionBase();
        }

        /// <summary>
        /// Get the entity related validator.
        /// </summary>
        /// <returns></returns>
        protected abstract IBaseValidator GetValidator();

        /// <summary>
        /// The method to call to add/save entity.
        /// </summary>
        /// <returns></returns>
        protected abstract TModelResult CallServiceMethod();
    }
}
