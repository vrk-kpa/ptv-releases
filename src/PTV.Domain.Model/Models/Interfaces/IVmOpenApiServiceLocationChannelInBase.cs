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

using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces
{
    public interface IVmOpenApiServiceLocationChannelInBase : IV2VmOpenApiServiceLocationChannelInBase
    {
        string Email { get; set; }
        string Phone { get; set; }
        string Fax { get; set; }
        IReadOnlyList<string> ServiceChargeTypes { get; set; }
        IReadOnlyList<VmOpenApiLanguageItem> PhoneChargeDescriptions { get; set; }
        string Latitude { get; set; }
        string Longitude { get; set; }
        string CoordinateSystem { get; set; }
        bool CoordinatesSetManually { get; set; }
        new IList<VmOpenApiAddressWithType> Addresses { get; set; }

        new IReadOnlyList<VmOpenApiServiceHour> ServiceHours { get; set; }

        bool DeleteEmail { get; set; }

        bool DeletePhone { get; set; }

        bool DeleteFax { get; set; }

        bool DeleteAllServiceChargeTypes { get; set; }
    }
}
