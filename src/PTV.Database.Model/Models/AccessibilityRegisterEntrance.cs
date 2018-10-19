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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models
{
    internal sealed class AccessibilityRegisterEntrance : EntityIdentifierBase, IOrderable
    {
        public AccessibilityRegisterEntrance()
        {
            SentenceGroups = new HashSet<AccessibilityRegisterGroup>();
            Names = new HashSet<AccessibilityRegisterEntranceName>();
        }

        public int EntranceId { get; set; }
        public bool IsMain { get; set; }
        public int? OrderNumber { get; set; }

        public Guid AccessibilityRegisterId { get; set; }
        public AccessibilityRegister AccessibilityRegister { get; set; }

        public Guid? AddressId { get; set; }
        public Address Address { get; set; }

        public ICollection<AccessibilityRegisterGroup> SentenceGroups { get; set; }
        public ICollection<AccessibilityRegisterEntranceName> Names { get; set; }
    }
}
