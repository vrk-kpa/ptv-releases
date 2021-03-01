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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models
{

    /// <summary>
    /// ids of admin tasks
    /// </summary>
    public enum AdminTasksIdsEnum
    {
        /// <summary>
        /// Failed transation orders
        /// </summary>
        FailedTranslationOrders
    }

    /// <summary>
    /// View model for tasks numner
    /// </summary>
    public class VmAdminTasksBase
    {
        /// <summary>
        /// Task Id
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter), true)]
        public AdminTasksIdsEnum Id { get; set; }
        /// <summary>
        /// Number of entities
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Send to client only positive number
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeCount()
        {
            return Count >= 0;
        }
    }

    /// <summary>
    /// View model for tasks number
    /// </summary>
    public class VmAdminTasks : VmAdminTasksBase, IVmSearchBase
    {
        /// <summary>
        /// C tor
        /// </summary>
        public VmAdminTasks()
        {
        }

        /// <summary>
        /// c tor
        /// </summary>
        /// <param name="baseModel"></param>
        public VmAdminTasks(IVmSearchBase baseModel)
        {
            if (baseModel == null) return;
            Count = baseModel.Count;
            MaxPageCount = baseModel.MaxPageCount;
            PageNumber = baseModel.PageNumber;
            MoreAvailable = baseModel.MoreAvailable;
        }

        /// <summary>
        /// Entitties of tasks
        /// </summary>
        public IEnumerable<IVmBase> Entities { get; set; }

        /// <inheritdoc />
        public int MaxPageCount { get; set; }

        /// <inheritdoc />
        public int PageNumber { get; set; }

        /// <inheritdoc />
        public bool MoreAvailable { get; set; }
    }

    /// <summary>
    /// View model of admin translation item
    /// </summary>
    public class VmAdminTranslationItem : VmAdminTaskItemBase
    {
        /// <summary>
        /// Sub language
        /// </summary>
        public Guid SourceLanguage { get; set; }
        /// <summary>
        /// Target language
        /// </summary>
        public Guid Targetlanguage { get; set; }
        /// <summary>
        /// Sender email
        /// </summary>
        public string SenderEmail { get; set; }
        /// <summary>
        /// Sant at
        /// </summary>
        public long SentAt { get; set; }
        /// <summary>
        /// Translation state type id
        /// </summary>
        public Guid TranslationStateTypeId { get; set; }
        /// <summary>
        /// Error description
        /// </summary>
        public string ErrorDescription { get; set; }
        /// <summary>
        /// Previous translation order states
        /// </summary>
        public IReadOnlyList<IVmAdminDetailItem> NestedItems { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="VmAdminTranslationItem"/> class.
        /// </summary>
        public VmAdminTranslationItem()
        {
            NestedItems = new List<IVmAdminDetailItem>();
        }
    }

    /// <summary>
    /// view model of admin task item base
    /// </summary>
    public class VmAdminTaskItemBase : IVmEntityBase, IVmEntityType
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// Gets or sets the languages availabilities.
        /// </summary>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public Dictionary<string, string> Name { get; set; }
        /// <summary>
        /// Entity type
        /// </summary>
        public EntityTypeEnum EntityType { get; set; }
        /// <summary>
        /// Sub entity type
        /// </summary>
        public string SubEntityType { get; set; }
        /// <summary>
        /// Unific root id
        /// </summary>
        public Guid EntityVersionedId { get; set; }
        /// <summary>
        /// Unific root id
        /// </summary>
        public Guid EntityUnificRootId { get; set; }
    }

    /// <summary>
    /// Vew model of admin translation order detail items
    /// </summary>
    public class VmAdminTranslationDetailItem : IVmAdminDetailItem
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// Sender email
        /// </summary>
        public string SenderEmail { get; set; }
        /// <summary>
        /// Sant at
        /// </summary>
        public long SentAt { get; set; }
        /// <summary>
        /// Translation state type id
        /// </summary>
        public Guid TranslationStateTypeId { get; set; }
        /// <summary>
        /// Parent id
        /// </summary>
        public Guid ParentId { get; set; }
    }
}
