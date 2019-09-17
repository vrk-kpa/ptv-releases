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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    /// VmHistoryMetaData
    /// </summary>
    public class VmHistoryMetaData : ICopyTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        public VmHistoryMetaData()
        {
            LanguagesMetaData = new List<VmHistoryMetaDataLanguage>();
            TargetLanguageIds = new List<Guid>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid EntityStatusId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HistoryAction HistoryAction { get; set; }

        /// <summary>
        /// 
        /// </summary>        
        public List<VmHistoryMetaDataLanguage> LanguagesMetaData { get; set; }

        /// <summary>
        /// ID of the source entity used as a template to create this entity.
        /// </summary>
        public Guid? TemplateId { get; set; }

        /// <summary>
        /// ID of the source organization whose entity was used as a template to create this entity.
        /// </summary>
        public Guid? TemplateOrganizationId { get; set; }

        /// <summary>
        /// ID of the source language if the history action is connected to translations.
        /// </summary>
        public Guid? SourceLanguageId { get; set; }

        /// <summary>
        /// ID of the target languages if the history action is connected to translations.
        /// </summary>
        public List<Guid> TargetLanguageIds { get; set; }

        /// <summary>
        /// Number of months after which the content is bound to expire.
        /// </summary>
        public string ExpirationPeriod { get; set; }
    }

    /// <summary>
    /// VmHistoryMetaDataLanguage
    /// </summary>
    public class VmHistoryMetaDataLanguage
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid EntityStatusId { get; set; }
        /// <summary>
        /// 
        /// </summary>        
        public Guid LanguageId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? PublishedAt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? Reviewed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String ReviewedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ArchivedAt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? Archived { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String ArchivedBy { get; set; }
    }
    /// <summary>
    /// Entity history action
    /// </summary>
    public enum HistoryAction
    {
        /// <summary>
        /// save action
        /// </summary>
        Save,
        /// <summary>
        /// delete action
        /// </summary>
        Delete,
        /// <summary>
        /// publish action
        /// </summary>
        Publish,
        /// <summary>
        /// restore deleted action
        /// </summary>
        Restore,
        /// <summary>
        /// witdraw published action
        /// </summary>
        Withdraw,
        /// <summary>
        /// Translation order send
        /// </summary>
        TranslationOrdered,
        /// <summary>
        /// Translation order received
        /// </summary>
        TranslationReceived,
        /// <summary>
        /// copy action
        /// </summary>
        Copy,
        /// <summary>
        /// Mass publish action
        /// </summary>
        MassPublish,
        /// <summary>
        /// Scheduled publish action
        /// </summary>
        ScheduledPublish,
        /// <summary>
        /// Scheduled publish action
        /// </summary>
        ScheduledArchive,
        /// <summary>
        /// Mass restore action
        /// </summary>
        MassRestore,
        /// <summary>
        /// Translation order sent again
        /// </summary>
        TranslationReordered,
        /// <summary>
        /// Entity is archived when a new version is published 
        /// </summary>
        OldPublished,
        /// <summary>
        /// Entity is archived because it has not been modified for a prolonged amount of time
        /// </summary>
        Expired,
        /// <summary>
        /// Mass archive action
        /// </summary>
        MassArchive,
        /// <summary>
        /// Content was archived because its parent organization was archived
        /// </summary>
        ArchivedViaOrganization
    }
}
