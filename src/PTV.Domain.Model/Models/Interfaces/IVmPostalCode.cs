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
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// View model interface of postal code
    /// </summary>
    public interface IVmPostalCode : IVmBase
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        string Code { get; set; }
        /// <summary>
        /// Gets or sets the post office.
        /// </summary>
        /// <value>
        /// The post office.
        /// </value>
        string PostOffice { get; set; }
        /// <summary>
        /// A set of multipolygon coordinates describing the border of the postal code area.
        /// </summary>
        List<VmCoordinateCollection> Border { get; set; }
        /// <summary>
        /// Coordinate describing the center of the postal code area.
        /// </summary>
        VmCoordinate CenterCoordinate { get; set; }
    }
}
