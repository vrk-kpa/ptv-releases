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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.ServiceManager;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models
{
    public class VmPhoneChannelStep1 : VmEnumEntityStatusBase, IVmPhoneChannelStep1
    {
        public VmPhoneChannelStep1()
        {
            Languages = new List<Guid>();            
        }

        [JsonIgnore]
        public Guid?  PhoneChannelId { get; set; }

        public VmWebPage WebPage { get; set; }

        public VmEmailData Email { get; set; }

        public VmPhone PhoneNumber { get; set; }

        public string Name { get; set; }
        public string ShortDescription { get; set; }

        public Guid? OrganizationId { get; set; }
        public string Description { get; set; }

        public List<Guid> Languages { get; set; }
        public LanguageCode Language { get; set; }
    }
}
