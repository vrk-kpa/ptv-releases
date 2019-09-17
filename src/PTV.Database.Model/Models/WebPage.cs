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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Framework.Formatters;
using PTV.Framework.Formatters.Attributes;

namespace PTV.Database.Model.Models
{
    internal partial class WebPage : EntityIdentifierBase, IName, IDescription, IOrderable
    {
        public WebPage()
        {
            OrganizationWebAddress = new HashSet<OrganizationWebPage>();
            ServiceChannelWebPages = new HashSet<ServiceChannelWebPage>();
            ServiceWebPages = new HashSet<ServiceWebPage>();
            ServiceServiceChannelWebPages = new HashSet<ServiceServiceChannelWebPage>();
        }

        [MaxLength(110)]
        [TrimSpacesFormatter]
        public string Name { get; set; }
        [TrimSpacesFormatter]
        public string Description { get; set; }
        public Guid LocalizationId { get; set; }
        [TrimSpacesFormatter]
        [MaxLength(500)]
        public string Url { get; set; }
        public int? OrderNumber { get; set; }
        public virtual ICollection<OrganizationWebPage> OrganizationWebAddress { get; set; }
        public virtual ICollection<ServiceChannelWebPage> ServiceChannelWebPages { get; set; }
        public virtual ICollection<ServiceWebPage> ServiceWebPages { get; set; }
        public virtual ICollection<ServiceServiceChannelWebPage> ServiceServiceChannelWebPages { get; set; }
        public Language Localization { get; set; }

        Guid IName.TypeId => throw new NotSupportedException();
        Guid IDescription.TypeId => throw new NotSupportedException();
    }
}
