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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for Oid.
    /// </summary>
    public class OidValidator : BaseValidator<string>
    {
        private IOrganizationService organizationService;
        private bool isCreateOperation;
        private Guid? organizationId;
        private string sourceId;

        /// <summary>
        /// Ctor - municipality code validator.
        /// </summary>
        /// <param name="model">Municipality code</param>
        /// <param name="organizationService">Organization service</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="isCreateOperation">Indicates if organization related to oid is beeing created or updated</param>
        /// <param name="organizationId">Organization id</param>
        /// <param name="sourceId">Source id</param>
        public OidValidator(string model, IOrganizationService organizationService, string propertyName = "Oid",
            bool isCreateOperation = false, Guid? organizationId = null, string sourceId = "") : base(model, propertyName)
        {
            this.organizationService = organizationService;
            this.isCreateOperation = isCreateOperation;
            this.organizationId = organizationId;
            this.sourceId = sourceId;
            if (!model.IsNullOrEmpty() && !isCreateOperation && !organizationId.HasValue && string.IsNullOrEmpty(sourceId))
            {
                throw new ArgumentException($"Either { nameof(organizationId)} or { nameof(sourceId)} should be given!");
            }
        }

        /// <summary>
        /// Checks if municipality code is valid or not.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (string.IsNullOrEmpty(Model))
            {
                return;
            }

            if (isCreateOperation)
            {
                if (organizationService.GetOrganizationIdByOid(Model) != Guid.Empty)
                {
                    modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.OidExists, Model));
                }
                return;
            }

            if (organizationId.IsAssigned())
            {
                Guid orgId = organizationService.GetOrganizationIdByOid(Model);

                // OID exists in the database and it is used by some other organization
                if (orgId != Guid.Empty && orgId != organizationId)
                {
                    modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.OidExists, Model));
                }
                return;
            }

            if (!string.IsNullOrEmpty(sourceId))
            {
                var idByOid = organizationService.GetOrganizationIdByOid(Model);
                var idBySource = organizationService.GetOrganizationIdBySource(sourceId);
                // If the organization id was found from external sources and it was not the same than the id related to Oid, validation was not successfull.
                if (idBySource != Guid.Empty && idByOid != Guid.Empty && idByOid != idBySource)
                {
                    modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.OidExists, Model));
                }
            }
        }
    }
}
