﻿/**
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
using PTV.Domain.Model.Models.Interfaces;
using System;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of dial code
    /// </summary>
    public class VmDialCode : IVmDialCode, IVmTranslatedItem, IVmBase
    {
        /// <summary>
        /// Code of dial code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Name of the country
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Id of dial Code
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// translation model
        /// </summary>
        public IVmTranslationItem Translation { get; set; }

        /// <summary>
        /// Default translation
        /// </summary>
        string IVmTranslatedItem.DefaultTranslation
        {
            get { return CountryName; }
            set { CountryName = value; }
        }
    }
}
