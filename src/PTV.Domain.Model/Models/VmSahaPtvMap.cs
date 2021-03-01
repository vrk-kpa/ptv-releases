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

using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// saha ptv mapped
    /// </summary>
    public class VmSahaPtvMap
    {
    /// <summary>
    /// PTV unific root id
    /// </summary>
        public Guid PtvUnificRootId { get; set; }
        /// <summary>
        /// Saha id
        /// </summary>
        public Guid SahaId { get; set; }
        /// <summary>
        /// Ptv business code
        /// </summary>
        public string PtvBusinessCode { get; set; }

        /// <summary>
        /// Ptv name
        /// </summary>
        public string PtvOrgName { get; set; }
        /// <summary>
        /// Saha name
        /// </summary>
        public string SahaOrgName { get; set; }

/// <summary>
///
/// </summary>
        public IEnumerable<Tuple<Guid, string>> ONames { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string PtvOrgAltName { get; set; }
    }
}
