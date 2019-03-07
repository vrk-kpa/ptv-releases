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
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi.V7
{
    /// <summary>
    /// OPEN API - View Model interface of channel and service relationship IN (PUT).
    /// </summary>
    public interface IV7VmOpenApiChannelServicesIn
    {
        /// <summary>
        /// Identifier for service.
        /// </summary>
        Guid? ChannelId { get; set; }

        /// <summary>
        /// Set to true to delete all existing relations between defined service and service channels (the ChannelRelations collection for this object should be empty collection when this option is used).
        /// </summary>
        bool DeleteAllServiceRelations { get; set; }

        /// <summary>
        /// Gets or sets the service relations.
        /// </summary>
        /// <value>
        /// The channel relations.
        /// </value>
        List<V7VmOpenApiServiceChannelServiceInBase> ServiceRelations { get; set; }
    }
}
