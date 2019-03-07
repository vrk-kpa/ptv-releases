﻿/**
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

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Linq;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for target group list.
    /// </summary>
    public class TargetGroupListValidator : BaseValidator<IList<string>>
    {
        private IFintoService fintoService;
        private IList<IVmOpenApiFintoItemVersionBase> targetGroups;

        /// <summary>
        /// Checked target groups.
        /// </summary>
        public IList<IVmOpenApiFintoItemVersionBase> TargetGroups { get { return targetGroups; } private set { } }

        /// <summary>
        /// Ctor - target group list validator.
        /// </summary>
        /// <param name="model">Target group list</param>
        /// <param name="fintoService">Finto item service</param>
        /// <param name="propertyName">Property name</param>
        public TargetGroupListValidator(IList<string> model, IFintoService fintoService, string propertyName = "TargetGroups") : base(model, propertyName)
        {
            this.fintoService = fintoService;
            this.targetGroups = new List<IVmOpenApiFintoItemVersionBase>();
        }

        /// <summary>
        /// Checks if target group list is valid or not.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            var i = 0;

            var targetGroupModels = Model.Select(uri => fintoService.GetTargetGroupByUri(uri)).ToList();
            var requiredParentUris = targetGroupModels
                .Where(tg => tg?.ParentId != null)
                .Select(tg => tg.ParentUri)
                .ToList();
            var notProvidedParentUris = requiredParentUris.Where(uri => !Model.Contains(uri)).ToList();

            if (notProvidedParentUris.Any())
            {
                modelState.AddModelError($"{PropertyName}",
                    string.Format(CoreMessages.OpenApi.ParentUrisNotFound, string.Join(", ", notProvidedParentUris)));
                return;
            }
            
            targetGroupModels.ForEach(item =>
            {
                if (item == null || !item.Id.IsAssigned())
                {
                    modelState.AddModelError($"{PropertyName}[{ i++ }]", string.Format(CoreMessages.OpenApi.CodeNotFound, item?.Uri));
                }
                else
                {
                    targetGroups.Add(item);
                }
            });
        }
    }
}
