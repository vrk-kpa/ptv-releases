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
using System.ComponentModel.DataAnnotations;
using PTV.Database.Model.Models.Base;
using PTV.Framework.Formatters;

namespace PTV.Database.Model.Models
{
    internal partial class TranslationOrder : EntityIdentifierBase
    {
        public TranslationOrder()
        {
            TranslationOrderStates = new HashSet<TranslationOrderState>();
            ServiceTranslationOrders = new HashSet<ServiceTranslationOrder>();
            ServiceChannelTranslationOrders = new List<ServiceChannelTranslationOrder>();
            GeneralDescriptionTranslationOrders = new List<GeneralDescriptionTranslationOrder>();
        }

        public string TranslationCompanyOrderIdentifier { get; set; }

        public long OrderIdentifier { get; set; }

        public DateTime? DeliverAt { get; set; }

        [TrimSpacesFormatter]
        [MaxLength(2500)]
        public string AdditionalInformation { get; set; }

        [TrimSpacesFormatter]
        [MaxLength(100)]
        public string SenderName { get; set; }

        [TrimSpacesFormatter]
        [MaxLength(100)]
        public string SenderEmail { get; set; }

        public Guid SourceLanguageId { get; set; }
        public Guid TargetLanguageId { get; set; }
        public Guid TranslationCompanyId { get; set; }
        public Guid? PreviousTranslationOrderId { get; set; }

        public string SourceLanguageData { get; set; }
        public string TargetLanguageData { get; set; }

        public string SourceLanguageDataHash { get; set; }
        public string TargetLanguageDataHash { get; set; }
        
        public long SourceLanguageCharAmount { get; set; }
        public string SourceEntityName { get; set; }
        public string OrganizationName { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public string OrganizationBusinessCode { get; set; }
        
        public virtual Language SourceLanguage { get; set; }
        public virtual Language TargetLanguage { get; set; }
        
        public virtual TranslationOrder PreviousTranslationOrder { get; set; }
        public virtual TranslationCompany TranslationCompany { get; set; }
        public virtual ICollection<TranslationOrderState> TranslationOrderStates { get; set; }
        public virtual ICollection<ServiceTranslationOrder> ServiceTranslationOrders { get; set; }
        public virtual ICollection<ServiceChannelTranslationOrder> ServiceChannelTranslationOrders { get; set; }
        public virtual ICollection<GeneralDescriptionTranslationOrder> GeneralDescriptionTranslationOrders { get; set; }
    }
}
