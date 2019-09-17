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
using System;
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IStreetService
    {
        /// <summary>
        /// Searches street names in given name.
        /// </summary>
        /// <param name="searchedExpression">Beginning of the street name, case insensitive.</param>
        /// <param name="postalCodeId">Postal code of the area where the street should occur.</param>
        /// <param name="languageId">Language in which the street name will be displayed.</param>
        /// <param name="offset">Number of items to be skipped.</param>
        /// <param name="onlyValid">Search only valid streets.</param>
        /// <returns></returns>
        IVmSearchBase GetStreets(string searchedExpression, Guid? postalCodeId, Guid languageId,
            int offset, bool onlyValid = true);

        /// <summary>
        /// Selects corresponding street number range ID from table ClsAddressStreetNumber.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="streetNumber"></param>
        /// <param name="streetId"></param>
        /// <returns></returns>
        Guid? GetStreetNumberRangeId(IUnitOfWork unitOfWork, string streetNumber, Guid streetId);
    }
}
