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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.V2.Notifications
{
    /// <summary>
    /// Notification type
    /// </summary>
    public enum Notification
    {
        /// <summary>
        /// If Service channel in common use value is change from 'Service channel can be connected to all organizations'
        /// to 'Service channel can be connected only to your organization's services' botification is added
        /// to all those organisations which has used this/these changed channel(s).
        /// </summary>
        ServiceChannelInCommonUse,
        /// <summary>
        /// Channel updated
        /// </summary>
        ContentUpdated,
        /// <summary>
        /// Channel archived
        /// </summary>
        ContentArchived,
        /// <summary>
        /// General description created
        /// </summary>
        GeneralDescriptionCreated,
        /// <summary>
        /// General description updated
        /// </summary>
        GeneralDescriptionUpdated,
        /// <summary>
        /// Connections updated
        /// </summary>
        ConnectionChanges
    }
    /// <summary>
    /// View model for notification
    /// </summary>
    public class VmNotificationsBase
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter), true)]
        public Notification Id { get; set; }
        /// <summary>
        /// Type
        /// </summary>
        public int Count { get; set; }
    }
    /// <summary>
    /// VmNotification 
    /// </summary>
    public class VmNotification : IVmEntityBase, IVmEntityType
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// Gets or sets the languages availabilities.
        /// </summary>
        /// <value>
        /// The languages availabilities.
        /// </value>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public Dictionary<string, string> Name { get; set; }
        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        /// <value>
        /// The operation type.
        /// </value>
        public string OperationType { get; set; }
        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        /// <value>
        /// The modified by.
        /// </value>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        /// <value>
        /// The modified.
        /// </value>
        public long Created { get; set; }
        /// <summary>
        /// Entity type
        /// </summary>
        public EntityTypeEnum MainEntityType { get; set; }
        /// <summary>
        /// Sub entity type
        /// </summary>
        public string SubEntityType { get; set; }
        /// <summary>
        /// Unific root id
        /// </summary>
        public Guid VersionedId { get; set; }
        /// <summary>
        /// Publishing status Id of entity
        /// </summary>
        public Guid? PublishingStatusId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public Dictionary<string, string> ConnectedName { get; set; }
        /// <summary>
        /// Connected Entity type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityTypeEnum ConnectedMainEntityType { get; set; }
        /// <summary>
        /// Connected Sub entity type
        /// </summary>
        public string ConnectedSubEntityType { get; set; }
        /// <summary>
        /// Connected Versiond id
        /// </summary>
        public Guid ConnectedVersionedId { get; set; }
    }
    /// <summary>
    /// View model for notification
    /// </summary>
    public class VmNotifications : VmNotificationsBase, IVmSearchBase
    {
        /// <summary>
        /// Notifications
        /// </summary>
        public IEnumerable<VmNotification> Notifications { get; set; }
        /// <summary>
        /// Max page count
        /// </summary>
        public int MaxPageCount { get; set; }
        /// <summary>
        /// Page number
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        ///  More available
        /// </summary>
        public bool MoreAvailable { get; set; }
    }
}
