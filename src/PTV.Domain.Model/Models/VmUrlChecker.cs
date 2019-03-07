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

using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Enums;
using System;
using PTV.Framework;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of url checker
    /// </summary>
    /// <seealso cref="PTV.Framework.ServiceManager.VmBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmUrlChecker" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmLocalized" />
    public class VmUrlChecker : VmBase, IVmUrlChecker, IVmLocalized
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id{ get; set; }
        /// <summary>
        /// Indicates whether url exist or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if url exist; <c>false</c> if not, <c>null</c> checking not available.
        /// </value>
        public bool? UrlExists { get; set; }
        /// <summary>
        /// Gets or sets the URL address.
        /// </summary>
        /// <value>
        /// The URL address.
        /// </value>
        public string UrlAddress { set; get; }
    }

    /// <summary>
    /// View model of url checker
    /// </summary>
    /// <seealso cref="PTV.Framework.ServiceManager.VmBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmUrlChecker" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmLocalized" />
    public class VmUrlCheckerV2 : VmBase, IVmUrlChecker, IVmLocalized
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id{ get; set; }
        /// <summary>
        /// Indicates whether url exist or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if url exist; <c>false</c> if not, <c>null</c> checking not available.
        /// </value>
        public Dictionary<string, bool> UrlExists { get; set; } = new Dictionary<string, bool>();
        /// <summary>
        /// Gets or sets the URL address.
        /// </summary>
        /// <value>
        /// The URL address.
        /// </value>
        public Dictionary<string, string> UrlAddress { set; get; } = new Dictionary<string, string>();
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public string Language { set; get; }

        bool? IVmUrlChecker.UrlExists
        {
            get
            {
                return UrlExists.TryGet(Language);
            }
            set
            {
                if (value.HasValue)
                {
                    UrlExists[Language] = value.Value;
                }
            }
        }

        string IVmUrl.UrlAddress {
            get
            {
                return UrlAddress.TryGet(Language.ToString());
            }
            set
            {
                UrlAddress[Language.ToString()] = value;
            }
        }

    }
}
