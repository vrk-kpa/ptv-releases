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
using PTV.Domain.Model.Enums;

namespace PTV.Database.Model.Views
{
    public class VBrokenLink
    {
        /// <summary>
        /// Database ID of the WebPage.
        /// </summary>
        public Guid WebPageId { get; set; }
        /// <summary>
        /// Url of the WebPage.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Database ID of the Organization to which the content belongs (Unific root).
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// Whether the WebPage is marked as Exception.
        /// </summary>
        public bool IsException { get; set; }
        /// <summary>
        /// Comment to the Exception mark.
        /// </summary>
        public string ExceptionComment { get; set; }
        /// <summary>
        /// Date when the WebPage was marked as Unstable.
        /// </summary>
        public DateTime? ValidationDate { get; set; }
        /// <summary>
        /// Type of the Entity.
        /// </summary>
        public string EntityType { get; set; }
        /// <summary>
        /// Subtype of the Entity.
        /// </summary>
        public string SubEntityType { get; set; }
        /// <summary>
        /// Version ID of the Entity (or Service in case of connections).
        /// </summary>
        public Guid EntityId { get; set; }
        /// <summary>
        /// Only for connections: ID of the versioned channel.
        /// </summary>
        public Guid? ConnectedChannelId { get; set; }
    }
}
