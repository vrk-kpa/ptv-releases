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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models.Feedback
{
    /// <summary>
    /// Structure for holding feedback data from Suomi.fi.
    /// </summary>
    public class VmFeedback : VmBase
    {
        /// <summary>
        /// Unique ID of the message. Currently not necessary, but can be used later for message storing and retrieval.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Type of Suomi.fi content which has received feedback.
        /// Note: PTV does not contain the types: informativeContent, register, instruction and news. Currently, it
        /// does not affect the functionality of the feedback service. However, if further processing will be required
        /// in the future, this might be an issue.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public FeedbackContentType ContentType { get; set; }

        /// <summary>
        /// Unique ID of the content which has received feedback.
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// Unique ID of the organization whose content received feedback.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// UTC date and time when the feedback was provided.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The actual feedback message provided by Suomi.fi users.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Direct link to the Suomi.fi content.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Email of the user who sent the feedback.
        /// </summary>
        public string SenderEmail { get; set; }
    }
}
