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
using PTV.Application.OpenApi.DataValidators;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.EntityManagers
{
    /// <summary>
    /// Save service collection manager
    /// </summary>
    public class SaveServiceCollectionManager : SaveEntityManagerBase<IVmOpenApiServiceCollectionInVersionBase, IVmOpenApiServiceCollectionBase>
    {
        private IServiceCollectionService _service;
        private ICommonService _commonService;
        private IServiceService _serviceService;
        private IList<Guid> _userOrganizations;

        /// <summary>
        /// Ctrl
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="sourceId">External source id</param>
        /// <param name="model">Service collection request model</param>
        /// <param name="openApiVersion">Open api version</param>
        /// <param name="modelState">Model state</param>
        /// <param name="logger">Logger</param>
        /// <param name="serviceCollectionService">Service collection service</param>
        /// <param name="commonService">Common service</param>
        /// <param name="serviceService">Service service</param>
        /// <param name="userOrganizations">User organizations</param>
        public SaveServiceCollectionManager(
            string id,
            string sourceId,
            IVmOpenApiServiceCollectionInVersionBase model,
            int openApiVersion,
            ModelStateDictionary modelState,
            ILogger logger,
            IServiceCollectionService serviceCollectionService,
            ICommonService commonService,
            IServiceService serviceService,
            IList<Guid> userOrganizations
            ) : base(id, sourceId, model, openApiVersion, modelState, logger)
        {
            _service = serviceCollectionService;
            _commonService = commonService;
            _serviceService = serviceService;
            _userOrganizations = userOrganizations;
        }

        /// <summary>
        /// Get the entity related error message.
        /// </summary>
        /// <returns></returns>
        protected override string GetErrorMessage()
        {
            if (ViewModel.Id.IsAssigned())
            {
                return $"ServiceCollection with id '{Id}' not found.";
            }

            return $"ServiceCollection with source id '{SourceId}' not found.";
        }

        /// <summary>
        /// The method to call to get the current version of entity.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiServiceCollectionBase CallCurrentVersionServiceMethod()
        {
            return ViewModel.Id.IsAssigned() ? _service.GetServiceCollectionById(ViewModel.Id.Value, 0, false) : _service.GetServiceCollectionBySource(ViewModel.SourceId, 0, false);
        }

        /// <summary>
        /// Get the entity related data validator.
        /// </summary>
        /// <returns></returns>
        protected override IBaseValidator GetValidator()
        {
            // Get the available languages from current version
            // Check if user has added new language versions. New available languages and data need to be validated (required fields need to exist in request).
            var newLanguages = ViewModel.AvailableLanguages.Where(i => !CurrentVersion.AvailableLanguages.Contains(i)).ToList();

            return new ServiceCollectionValidator(ViewModel, _commonService, _serviceService, newLanguages, _userOrganizations);
        }

        /// <summary>
        /// The method for updating the service.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiServiceCollectionBase CallServiceMethod()
        {
            return _service.SaveServiceCollection(ViewModel, OpenApiVersion, SourceId);
        }
    }
}
