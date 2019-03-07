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
using PTV.Domain.Model.Models.StreetData.Responses;

namespace PTV.Domain.Model.Models.Interfaces.StreetData
{
    /// <summary>
    /// A single item from the CLS API.
    /// </summary>
    public interface IScalarModel
    {
        /// <summary>
        /// Self URL to the object in the CLS API.
        /// </summary>
        string Url { get; set; }
        /// <summary>
        /// A unique identifier in the CLS API.
        /// </summary>
        Guid Id { get; set; }
        /// <summary>
        /// The source of the object.
        /// </summary>
        string Source { get; set; }
        /// <summary>
        /// Validity information.
        /// </summary>
        VmStatusType Status { get; set; }
        /// <summary>
        /// Time when the object was created.
        /// </summary>
        DateTime Created { get; set; }
        /// <summary>
        /// The most recent time of object's data being modified.
        /// </summary>
        DateTime? Modified { get; set; }
    }
}