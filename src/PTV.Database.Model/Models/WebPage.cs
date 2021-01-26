/**
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
using System.ComponentModel.DataAnnotations;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Framework.Formatters;
using PTV.Framework.Formatters.Attributes;

namespace PTV.Database.Model.Models
{
    internal partial class WebPage : EntityIdentifierBase, IUrl
    {
        public WebPage()
        {
            OrganizationWebAddress = new HashSet<OrganizationWebPage>();
            ServiceChannelWebPages = new HashSet<ServiceChannelWebPage>();
            ServiceWebPages = new HashSet<ServiceWebPage>();
            ServiceServiceChannelWebPages = new HashSet<ServiceServiceChannelWebPage>();
            LawWebPages = new HashSet<LawWebPage>();
            ElectronicChannelUrls = new HashSet<ElectronicChannelUrl>();
            WebpageChannelUrls = new HashSet<WebpageChannelUrl>();
            PrintableFormChannelUrls = new HashSet<PrintableFormChannelUrl>();
        }

        [TrimSpacesFormatter]
        [MaxLength(500)]
        public string Url { get; set; }
        public virtual ICollection<OrganizationWebPage> OrganizationWebAddress { get; set; }
        public virtual ICollection<ServiceChannelWebPage> ServiceChannelWebPages { get; set; }
        public virtual ICollection<ServiceWebPage> ServiceWebPages { get; set; }
        public virtual ICollection<ServiceServiceChannelWebPage> ServiceServiceChannelWebPages { get; set; }
        public virtual ICollection<LawWebPage> LawWebPages { get; set; }
        public virtual ICollection<ElectronicChannelUrl> ElectronicChannelUrls { get; set; }
        public virtual ICollection<WebpageChannelUrl> WebpageChannelUrls { get; set; }
        public virtual ICollection<PrintableFormChannelUrl> PrintableFormChannelUrls { get; set; }
        public bool IsBroken { get; set; }
        public bool IsException { get; set; }
        public string ExceptionComment { get; set; }

        public DateTime? ValidationDate { get; set; }
    }
}
