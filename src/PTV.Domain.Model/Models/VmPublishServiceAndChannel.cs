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
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of publishing the relations service and channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmPublishServiceAndChannel" />
    public class VmPublishServiceAndChannel : IVmPublishServiceAndChannel
    {
        /// <summary>
        /// Gets or sets the services in relation.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        public List<Guid> Services { get; set; }
        /// <summary>
        /// Gets or sets the channels in relation.
        /// </summary>
        /// <value>
        /// The channels.
        /// </value>
        public List<Guid> Channels { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmPublishServiceAndChannel"/> class.
        /// </summary>
        public VmPublishServiceAndChannel()
        {
            Services = new List<Guid>();
            Channels = new List<Guid>();
        }
    }
}
