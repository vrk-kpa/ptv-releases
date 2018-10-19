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
using System.Security.Cryptography.X509Certificates;

namespace PTV.Domain.Model.Models.Import
{
    /// <summary>
    /// saha json country
    /// </summary>
    public class VmSahaJsonCountry
    {
        private string _code;

        /// <summary>
        /// language code
        /// </summary>
        public string Code
        {
            get => _code.ToLower();
            set => _code = value;
        }
    }
    
    /// <summary>
    /// saha json account data
    /// </summary>
    public class VmJsonAccountData
    {
        /// <summary>
        /// saha country
        /// </summary>
        public VmSahaJsonCountry Country { get; set; }
    }
    /// <summary>
    /// View model for saha guids
    /// </summary>
    public class VmJsonSahaGuids
    {
        /// <summary>
        /// Account Data
        /// </summary>
        public VmJsonAccountData AccountData { get; set; }
        /// <summary>
        /// bussines code
        /// </summary>
        public string BusinessId { get; set; }
        /// <summary>
        /// Saha guid
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Name of organization
        /// </summary>
        public string OrganizationName { get; set; }
    }
}