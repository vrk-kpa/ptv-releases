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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service - base
    /// </summary>
    public interface IVmOpenApiServiceBase : IVmOpenApiEntityBase
    {
        /// <summary>
        /// Service type. Possible values are: Service, Notice, Registration or Permission.
        /// </summary>
        string Type { get; set; }
        /// <summary>
        /// Service funding type. Possible values are: PubliclyFunded or MarketFunded.
        /// </summary>
        string FundingType { get; set; }
        /// <summary>
        /// List of localized service names.
        /// </summary>
        IList<VmOpenApiLocalizedListItem> ServiceNames { get; set; }
        /// <summary>
        /// Service charge type. Possible values are: Charged or Free
        /// </summary>
        string ServiceChargeType { get; set; }
        /// <summary>
        /// Area type. 
        /// </summary>
        string AreaType { get; set; }
        /// <summary>
        /// List of localized service descriptions.
        /// </summary>
        IList<VmOpenApiLocalizedListItem> ServiceDescriptions { get; set; }
        /// <summary>
        /// List of service languages.
        /// </summary>
        IList<string> Languages { get; set; }
        /// <summary>
        /// List of laws related to the service.
        /// </summary>
        IList<V4VmOpenApiLaw> Legislation { get; set; }
        /// <summary>
        /// List of service keywords.
        /// </summary>
        IList<VmOpenApiLanguageItem> Keywords { get; set; }
        /// <summary>
        /// Localized service usage requirements (description of requirement).
        /// </summary>
        IList<VmOpenApiLanguageItem> Requirements { get; set; }
        /// <summary>
        /// Indicates if service vouchers are used in the service.
        /// </summary>
        bool ServiceVouchersInUse { get; set; }
        /// <summary>
        /// List of service vouchers
        /// </summary>
        IList<V9VmOpenApiServiceVoucher> ServiceVouchers { get; set; }
        /// <summary>
        /// Gets or sets availble languages
        /// </summary>
        IList<string> AvailableLanguages { get; set; }
    }
}