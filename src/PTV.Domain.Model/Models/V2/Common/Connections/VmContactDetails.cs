
using System.Collections.Generic;
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
namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    /// Contact information
    /// </summary>
    public class VmContactDetails
    {
        /// <summary>
        /// ctor
        /// </summary>
        public VmContactDetails()
        {
            Emails = new Dictionary<string, List<VmEmailData>>();
            FaxNumbers = new Dictionary<string, List<VmPhone>>();
            PhoneNumbers = new Dictionary<string, List<VmPhone>>();
            WebPages = new Dictionary<string, List<VmWebPage>>();
            PostalAddresses = new List<VmAddressSimple>();
        }
        /// <summary>
        /// Gets or sets emails
        /// </summary>
        public Dictionary<string, List<VmEmailData>> Emails { get; set; }
        /// <summary>
        /// Gets or sets fax numbers
        /// </summary>
        public Dictionary<string, List<VmPhone>> FaxNumbers { get; set; }
        /// <summary>
        /// Gets or sets phone numbers
        /// </summary>
        public Dictionary<string, List<VmPhone>> PhoneNumbers { get; set; }
        /// <summary>
        /// Gets or sets webpages
        /// </summary>
        public Dictionary<string, List<VmWebPage>> WebPages { get; set; }
        /// <summary>
        /// Gets or set postal addresses
        /// </summary>
        public List<VmAddressSimple> PostalAddresses { get; set; }
    }
}
