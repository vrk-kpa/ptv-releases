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

using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IPostalCodeService
    {
        /// <summary>
        /// Get Postal Codes
        /// </summary>
        /// <param name="searchedCode"></param>
        /// <param name="onlyValid"></param>
        /// <returns></returns>
        IVmListItemsData<IVmPostalCode> GetPostalCodes(string searchedCode, bool onlyValid = true);

        /// <summary>
        /// Get postal code by postal code id
        /// </summary>
        /// <param name="postalCodeId"></param>
        /// <returns></returns>
        IVmPostalCode GetPostalCode(Guid postalCodeId);

        /// <summary>
        /// Get a list of Postal Codes
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of postal codes.</returns>
        IVmOpenApiGuidPageVersionBase GetPostalCodeList(int pageNumber, int pageSize);

    }
}
