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
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models.Import
{
    /// <summary>
    /// View model of expirations time json
    /// </summary>
    public class VmJsonTasksConfiguration
    {
        /// <summary>
        /// Gets or sets the expiration time code.
        /// </summary>
        /// <value>
        /// The expiration time code.
        /// </value>
        public string Code { get; set; }
        /// <value>
        /// The expiration time of entity.
        /// </value>
        public string Entity { get; set; }
        /// <value>
        /// The expiration time of entity.
        /// </value>
        public string PublishingStatus { get; set; }
        /// <summary>
        /// expiration in Months
        /// </summary>
        public string Interval { get; set; }
    }
}
