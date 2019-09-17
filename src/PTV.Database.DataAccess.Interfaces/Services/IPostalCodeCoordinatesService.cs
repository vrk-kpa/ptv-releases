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

using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Models;
using PTV.Framework;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    /// <summary>
    /// Service for downloading and importing coordinates for postal codes.
    /// </summary>
    public interface IPostalCodeCoordinatesService
    {
        /// <summary>
        /// Downloads coordinates one by one.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="postalCodes">Collection of postal codes for which coordinates should be downloaded.</param>
        /// <param name="postalCodeCoordinatesSettings">settings for download
        /// where the respective postal code will be inserted.</param>
        /// <param name="jobStatus">A StringBuilder logging possible issues while downloading.</param>
        /// <returns>List of postal codes with their respective center coordinates and a multipolygon border.</returns>
        Task<List<VmPostalCodeCoordinate>> DownloadCoordinates(HttpClient client, IEnumerable<VmPostalCode> postalCodes,
            PostalCodeCoordinatesSettings postalCodeCoordinatesSettings, StringBuilder jobStatus);
        
        /// <summary>
        /// Imports coordinates into the database.
        /// </summary>
        /// <param name="unitOfWork">A unit of work for writing new values.</param>
        /// <param name="postalCodeCoordinates">List of postal codes with coordinates.</param>
        void UpdateCoordinates(IUnitOfWorkWritable unitOfWork, List<VmPostalCodeCoordinate> postalCodeCoordinates);

        /// <summary>
        /// Downloads coordinates in batches by querying whole pages.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="offset">Offset of for the first coordinate to be downloaded.</param>
        /// <param name="pageSize">Size of the page to be downloaded.</param>
        /// <param name="postalCodeCoordinatesSettings">settings for download
        /// to specify the page size and a {1} placeholder for the start </param>
        /// <param name="jobStatus">A StringBuilder logging possible issues while downloading.</param>
        /// <returns>List of postal codes with their respective center coordinates and a multipolygon border.</returns>
        Task<List<VmPostalCodeCoordinate>> DownloadBatch(HttpClient client, int offset, int pageSize, PostalCodeCoordinatesSettings postalCodeCoordinatesSettings,
            StringBuilder jobStatus);
    }
}