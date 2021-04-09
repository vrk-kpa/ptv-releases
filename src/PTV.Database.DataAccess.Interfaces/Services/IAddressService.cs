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

using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IAddressService
    {
        /// <summary>
        /// Return address info with coordinates
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmAddressSimpleBase GetAddress(VmGetCoordinatesForAddressIn model);
        /// <summary>
        /// Return address with coordinates
        /// </summary>
        /// <param name="model"></param>
        /// <returns>coordinates</returns>
        VmAddressSimpleBase GetAddressWithCoordinates(VmGetCoordinatesForAddressIn model);
        /// <summary>
        /// Update address with map coordinates
        /// </summary>
        /// <param name="addresses">id's of addresses to updated</param>
        void UpdateAddress(IList<Guid> addresses);

        /// <summary>
        ///
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="addresses"></param>
        /// <param name="newStreets"></param>
        void AddNewStreetAddresses(IUnitOfWorkWritable unitOfWork, IEnumerable<VmAddressSimple> addresses,
            Dictionary<(Guid, string), Guid> newStreets);
        /// <summary>
        ///
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="addresses"></param>
        /// <param name="newStreets"></param>
        void AddNewStreetAddressNumbers(IUnitOfWorkWritable unitOfWork, IEnumerable<VmAddressSimple> addresses,
            Dictionary<(Guid, string), Guid> newStreets);

        /// <summary>
        /// Add service channel postal addresses
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="addresses"></param>
        /// <param name="channelId"></param>
        void AddServiceChannelPostalAddresses(IUnitOfWorkWritable unitOfWork, IEnumerable<VmOpenApiAddressPostOfficeBoxIn> addresses, Guid channelId);
        
        /// <summary>
        /// Add organization postal addresses
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="addresses"></param>
        /// <param name="organizationId"></param>
        void AddOrganizationPostalAddresses(IUnitOfWorkWritable unitOfWork, IEnumerable<VmOpenApiAddressPostOfficeBoxIn> addresses, Guid organizationId);

    }
}
