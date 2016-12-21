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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Converters;

namespace PTV.Domain.Model.Models
{
    public class VmServiceStep2 : VmEnumEntityBase, IVmServiceStep2
    {
        [JsonConverter(typeof(ListItemListConverter))]
        public List<VmListItem> ServiceClasses { get; set; }
        [JsonConverter(typeof(ListItemListConverter))]
        public List<VmListItem> IndustrialClasses { get; set; }
        [JsonConverter(typeof(ListItemListConverter))]
        public List<VmListItem> OntologyTerms { get; set; }
        [JsonConverter(typeof(ListItemListConverter))]
        public List<VmListItem> LifeEvents { get; set; }
        public List<Guid> TargetGroups { get; set; }
        public List<Guid> KeyWords { get; set; }
        public List<VmKeywordItem> NewKeyWords { get; set; }
        public LanguageCode Language { get; set; }
        public VmServiceStep2()
        {
            TargetGroups = new List<Guid>();
            ServiceClasses = new List<VmListItem>();
            IndustrialClasses = new List<VmListItem>();
            OntologyTerms = new List<VmListItem>();
            LifeEvents = new List<VmListItem>();
            KeyWords = new List<Guid>();
            NewKeyWords = new List<VmKeywordItem>();
        }
    }
}
