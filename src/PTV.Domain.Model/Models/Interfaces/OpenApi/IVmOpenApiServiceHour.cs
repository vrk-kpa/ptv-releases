

using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
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
namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service hour
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceHourBase" />
    public interface IVmOpenApiServiceHour : IVmOpenApiServiceHourBase
    {
        /// <summary>
        /// Gets or sets the type of the exception hour.
        /// </summary>
        /// <value>
        /// The type of the exception hour.
        /// </value>
        string ExceptionHourType { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IVmOpenApiServiceHour"/> is monday.
        /// </summary>
        /// <value>
        ///   <c>true</c> if monday; otherwise, <c>false</c>.
        /// </value>
        bool Monday { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IVmOpenApiServiceHour"/> is tuesday.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tuesday; otherwise, <c>false</c>.
        /// </value>
        bool Tuesday { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IVmOpenApiServiceHour"/> is wednesday.
        /// </summary>
        /// <value>
        ///   <c>true</c> if wednesday; otherwise, <c>false</c>.
        /// </value>
        bool Wednesday { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IVmOpenApiServiceHour"/> is thursday.
        /// </summary>
        /// <value>
        ///   <c>true</c> if thursday; otherwise, <c>false</c>.
        /// </value>
        bool Thursday { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IVmOpenApiServiceHour"/> is friday.
        /// </summary>
        /// <value>
        ///   <c>true</c> if friday; otherwise, <c>false</c>.
        /// </value>
        bool Friday { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IVmOpenApiServiceHour"/> is saturday.
        /// </summary>
        /// <value>
        ///   <c>true</c> if saturday; otherwise, <c>false</c>.
        /// </value>
        bool Saturday { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IVmOpenApiServiceHour"/> is sunday.
        /// </summary>
        /// <value>
        ///   <c>true</c> if sunday; otherwise, <c>false</c>.
        /// </value>
        bool Sunday { get; set; }
        /// <summary>
        /// Gets or sets the opens.
        /// </summary>
        /// <value>
        /// The opens.
        /// </value>
        string Opens { get; set; }
        /// <summary>
        /// Gets or sets the closes.
        /// </summary>
        /// <value>
        /// The closes.
        /// </value>
        string Closes { get; set; }
        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        new IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; }
    }
}
