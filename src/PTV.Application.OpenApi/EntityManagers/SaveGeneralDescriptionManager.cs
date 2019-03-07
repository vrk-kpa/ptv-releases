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
using Microsoft.Extensions.Logging;
using PTV.Application.OpenApi.DataValidators;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.EntityManagers
{
    /// <summary>
    /// Save general description handler
    /// </summary>
    public class SaveGeneralDescriptionManager : SaveEntityManagerBase<IVmOpenApiGeneralDescriptionInVersionBase, IVmOpenApiGeneralDescriptionVersionBase>
    {
        //private IList<string> _newLanguages;

        private IGeneralDescriptionService _service;
        private ICodeService _codeService;
        private IFintoService _fintoService;
        private IOntologyTermDataCache _ontologyTermDataCache;

        /// <summary>
        /// Ctrl
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="sourceId">External source id</param>
        /// <param name="model">the request model</param>
        /// <param name="openApiVersion">Open api version</param>
        /// <param name="modelState">Model state</param>
        /// <param name="logger">Logger</param>
        /// <param name="generalDescriptionService">General description service</param>
        /// <param name="codeService">Code service</param>
        /// <param name="fintoService">Finto service</param>
        /// <param name="ontologyTermDataCache">Ontology term data cache</param>
        public SaveGeneralDescriptionManager(
            string id,
            IVmOpenApiGeneralDescriptionInVersionBase model,
            int openApiVersion,
            ModelStateDictionary modelState,
            ILogger logger,
            IGeneralDescriptionService generalDescriptionService,
            ICodeService codeService,
            IFintoService fintoService,
            IOntologyTermDataCache ontologyTermDataCache)
            : base(id, null, model, openApiVersion, modelState, logger)
        {
            //_newLanguages = new List<string>();

            _service = generalDescriptionService;
            _codeService = codeService;
            _fintoService = fintoService;
            _ontologyTermDataCache = ontologyTermDataCache;
        }

        /// <summary>
        /// Get the entity related error message.
        /// </summary>
        /// <returns></returns>
        protected override string GetErrorMessage()
        {
            return $"General description with id '{Id}' not found.";
        }

        /// <summary>
        /// The method to call to get the current version of entity.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiGeneralDescriptionVersionBase CallCurrentVersionServiceMethod()
        {
            return _service.GetGeneralDescriptionVersionBase(ViewModel.Id.Value, 0, false);
        }

        /// <summary>
        /// Get the entity related validator.
        /// </summary>
        /// <returns></returns>
        protected override IBaseValidator GetValidator()
        {
            // Get the available languages from current version.
            // Check if user has added new language versions. New available languages and data need to be validated (required fields need to exist in request).
            var newLanguages = ViewModel.AvailableLanguages.Where(i => !CurrentVersion.AvailableLanguages.Contains(i)).ToList();
            ViewModel.CurrentVersionId = CurrentVersion.CurrentVersionId;
            List<IVmOpenApiFintoItemVersionBase> list = new List<IVmOpenApiFintoItemVersionBase>();
            CurrentVersion.TargetGroups.ForEach(i => list.Add(i));
            return new GeneralDescriptionValidator(ViewModel, _codeService, _fintoService, _ontologyTermDataCache, newLanguages, list);
        }

        /// <summary>
        /// The method for adding the general description.
        /// </summary>
        /// <returns></returns>
        protected override IVmOpenApiGeneralDescriptionVersionBase CallServiceMethod()
        {
            return _service.SaveGeneralDescription(ViewModel, OpenApiVersion);
        }
    }
}
