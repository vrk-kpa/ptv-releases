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
using Newtonsoft.Json.Serialization;

namespace PTV.NetworkServices.Emailing
{
    [Flags]
    public enum EPahaNotificationServiceGroup
    {
        NoDef= 0,
        Ptv = 1,
        Siha = 2,
        EIdentification = 4,
        Sema = 8,
        VrkSema = 16,
        ServiceBus = 32,
        EAuthoriyation = 64,
        Messages = 128
    }



    public class NotificationText
    {
        [JsonProperty(PropertyName = "sendToHtml")]

        public bool SendToHtml { get; set; }

        [JsonProperty(PropertyName = "insertHeaderFooter")]

        public bool InsertHeaderFooter { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "messageFormat")]
        public string MessageFormat { get; set; }
    }

    public class NotificationAccountId
    {
        [JsonProperty(PropertyName = "idValue")]
        public Guid IdValue { get; set; }
    }

    public class NotificationEmail
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "showStartTime")]
        public string ShowStartTime { get; set; }

        [JsonProperty(PropertyName = "showEndTime")]
        public string ShowEndTime { get; set; }

        [JsonProperty(PropertyName = "notificationType")]
        public int NotificationType { get; set; }

        [JsonProperty(PropertyName = "serviceGroup")]
        public int ServiceGroup { get; set; }

        [JsonProperty(PropertyName = "serviceToleGroup")]
        public int ServiceRoleGroup { get; set; }

        [JsonProperty(PropertyName = "sendToTarget")]
        public int SendToTarget { get; set; }

        [JsonProperty(PropertyName = "pahaRoleGroup")]
        public int PahaRoleGroup { get; set; }

        [JsonProperty(PropertyName = "externaleMailAddress")]

        public string ExternaleMailAddress { get; set; }

        [JsonProperty(PropertyName = "sendToHtml")]

        public bool SendToHtml { get; set; }

        [JsonProperty(PropertyName = "insertHeaderFooter")]

        public bool InsertHeaderFooter { get; set; }

        [JsonProperty(PropertyName = "notificationText")]
        public List<NotificationText> NotificationText { get; set; }
        [JsonProperty(PropertyName = "userIds")]
        public List<NotificationAccountId> UserIds { get; set; }
        [JsonProperty(PropertyName = "accountIds")]
        public List<NotificationAccountId> AccountIds { get; set; }
    }
}
