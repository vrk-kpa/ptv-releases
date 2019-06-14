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
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PTV.Domain.Model.Models.StreetData.Responses;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IStreetDataService
    {
        /// <summary>
        /// Imports data from the CLS API. If an item already exists in the database, it will be updated.
        /// If not, a new item will be created.
        /// </summary>
        /// <param name="streetAddressCollection">Collection of downloaded data</param>
        StreetDataImportResult ImportAndUpdateAddresses(VmStreetAddressCollection streetAddressCollection);

        /// <summary>
        /// Invalidate all items in the database which have not been updated.
        /// </summary>
        /// <param name="updateId">Id of update job.</param>
        List<Guid> InvalidateNonAffectedStreetAddresses(Guid? updateId = null);

        /// <summary>
        /// Downloads all street addresses.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pageSize"></param>
        /// <param name="getBatchUrl"></param>
        /// <returns></returns>
        Task<VmStreetAddressCollection> DownloadAll(HttpClient client, int pageSize, Func<int, string> getBatchUrl);
    }
}