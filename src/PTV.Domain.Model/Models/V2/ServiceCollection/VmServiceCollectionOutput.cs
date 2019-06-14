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
using PTV.Domain.Model.Models.V2.Common;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models.V2.ServiceCollection
{
    /// <summary>
    /// ViewModel for basic service collection information
    /// </summary>
    public class VmServiceCollectionOutput : VmServiceCollectionBase, IEnumCollection
    {       
        /// <summary>
        ///  Gets or sets the connected services
        /// </summary>
        /// <value>
        /// The connected services.
        /// </value>
        public List<VmServiceCollectionConnectionOutput> Connections { get; set; }

        /// <summary>
        /// Gets or sets the enum collection.
        /// </summary>
        /// <value>
        /// The enum collection.
        /// </value>
        public IVmDictionaryItemsData<IEnumerable<IVmBase>> EnumCollection { get; set; }
    }
}
