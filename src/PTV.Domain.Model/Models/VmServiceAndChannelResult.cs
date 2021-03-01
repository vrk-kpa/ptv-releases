﻿/**
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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of search result in relations between services and channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmServiceAndChannelResult" />
    public class VmServiceAndChannelResult : IVmServiceAndChannelResult
    {
        /// <summary>
        /// Gets or sets the connected services.
        /// </summary>
        /// <value>
        /// The connected services.
        /// </value>
        public List<IVmBase> ConnectedServices { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmServiceAndChannelResult"/> class.
        /// </summary>
        public VmServiceAndChannelResult()
        {
            ConnectedServices = new List<IVmBase>();
        }
    }
}
