﻿/**
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

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for areas and area type.
    /// </summary>
    public class AreaAndTypeValidator : BaseValidator<IList<VmOpenApiAreaIn>>
    {
        private readonly string type;
        private readonly Guid? organizationId;
        private readonly ICodeService codeService;
        private readonly ICommonService commonService;

        private MunicipalityCodeListValidator municipalities;
        private AreaListValidator areas;

        /// <summary>
        /// Ctor - area code list validator.
        /// </summary>
        /// <param name="model">Area code list</param>
        /// <param name="type">Type of the area</param>
        /// <param name="codeService">Code service</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="commonService">Common Service</param>
        /// <param name="organizationId">Organization id</param>
        public AreaAndTypeValidator(IList<VmOpenApiAreaIn> model, string type, ICodeService codeService, string propertyName = "Areas", ICommonService commonService = null, Guid? organizationId = null) : base(model, propertyName)
        {
            this.type = type;
            this.codeService = codeService;
            this.commonService = commonService;
            this.organizationId = organizationId;
        }

        /// <summary>
        /// Validates area code list.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            AreaInformationTypeEnum? serviceAreaType = null;
            if (!type.IsNullOrEmpty())
            {
                serviceAreaType = (AreaInformationTypeEnum)type.GetEnumByOpenApiEnumValue(typeof(AreaInformationTypeEnum));//.Parse<AreaInformationTypeEnum>();
            }

            if (serviceAreaType.HasValue && organizationId.HasValue && commonService != null)
            {
                var organizationAreaType = commonService.GetAreaInformationTypeForOrganization(new VmGetAreaInformation
                    {OrganizationId = organizationId.Value});
                if (serviceAreaType < organizationAreaType)
                {
                    modelState.AddModelError("Areas", $"Area information type {type} is too wide.");
                }
            }

            switch (serviceAreaType)
            {
                case AreaInformationTypeEnum.WholeCountry:
                case AreaInformationTypeEnum.WholeCountryExceptAlandIslands:
                    // No area info accepted if service are type is WholeCountry or WholeCountryExceptAlandIslands
                    if (Model.Count > 0)
                    {
                        modelState.AddModelError("Areas", $"No Areas accepted when AreaType has value {type}.");
                    }
                    break;
                default:
                    var i = 0;
                    Model.ForEach(area =>
                    {
                        if (area.Type == AreaTypeEnum.Municipality.ToString())
                        {
                            municipalities = new MunicipalityCodeListValidator(area.AreaCodes, codeService, $"{ PropertyName }[{ i++ }].AreaCodes");
                            municipalities.Validate(modelState);
                        }
                        else
                        {
                            areas = new AreaListValidator(area.AreaCodes, area.Type, codeService, $"{ PropertyName }[{ i++ }].AreaCodes");
                            areas.Validate(modelState);
                        }
                    });
                    break;
            }
        }
    }
}
