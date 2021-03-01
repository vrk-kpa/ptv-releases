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

namespace PTV.Framework
{
    /// <summary>
    /// Settings for downloading coordinates for postal codes.
    /// </summary>
    public class PostalCodeCoordinatesSettings
    {
        /// <summary>
        /// Url template for downloading by batches. The template contains a {0} placeholder for url base ,a {1} placeholder for specifying the size of
        /// the batch and {2} placeholder for the offset index.
        /// </summary>
        public string BatchUrl { get; set; }

        /// <summary>
        /// Url template for downloading postal code coordinates one by one. The template contains a {0} placeholder for url base and a {1} placeholder for
        /// specifying the respective postal code.
        /// </summary>
        public string SingleUrl { get; set; }
        /// <summary>
        /// Base Url template for downloading postal code coordinates one by one.
        /// </summary>
        public string UrlBase { get; set; }
        /// <summary>
        /// Size of a batch (page).
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Determines which method and url will be used for downloading - one by one or by batches.
        /// </summary>
        public bool DownloadOneByOne { get; set; }
    }
}
