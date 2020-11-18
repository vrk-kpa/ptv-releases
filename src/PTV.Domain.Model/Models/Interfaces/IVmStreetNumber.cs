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
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// Represents a street number and range
    /// </summary>
    public interface IVmStreetNumber : IVmBase
    {
        /// <summary>
        /// Street number Id
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Start number of the valid sequence
        /// </summary>
        int StartNumber { get; set; }

        /// <summary>
        /// End number of the valid sequence
        /// </summary>
        int EndNumber { get; set; }

        /// <summary>
        /// Determines whether the current sequence has even or odd numbers
        /// </summary>
        bool IsEven { get; set; }

        /// <summary>
        /// Related postal code
        /// </summary>
        VmPostalCode PostalCode { get; set; }

        /// <summary>
        /// Determines whether given street range is valid.
        /// </summary>
        bool IsValid { get; set; }

        /// <summary>
        /// Id of the street to which the number belongs
        /// </summary>
        Guid StreetId { get; set; }

        /// <summary>
        /// Coordinate for entered street number (optional)
        /// </summary>
        VmCoordinate Coordinate { get; set; }
    }
}
