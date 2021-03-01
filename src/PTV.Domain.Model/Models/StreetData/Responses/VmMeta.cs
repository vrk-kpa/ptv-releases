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

namespace PTV.Domain.Model.Models.StreetData.Responses
{
    /// <summary>
    /// Metadata about a CLS collection.
    /// </summary>
    public class VmMeta
    {
        /// <summary>
        /// HTTP response code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Size of the current paginated response.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Position of the first item in the page in the whole collection.
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// The actual count of returned items.
        /// </summary>
        public int ResultCount { get; set; }

        /// <summary>
        /// Total number of items available.
        /// </summary>
        public int TotalResults { get; set; }

        /// <summary>
        /// The purpose of this field is unknown to me as of now.
        /// </summary>
        public string AfterResourceUrl { get; set; }

        /// <summary>
        /// URL to the next page.
        /// </summary>
        public string NextPage { get; set; }
    }
}
