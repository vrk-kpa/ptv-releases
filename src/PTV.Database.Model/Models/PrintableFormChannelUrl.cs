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
using PTV.Database.Model.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PTV.Database.Model.Interfaces;
using PTV.Framework.Formatters;
using PTV.Framework.Formatters.Attributes;

namespace PTV.Database.Model.Models
{
    internal class PrintableFormChannelUrl : EntityIdentifierBase, ILocalizable, IOrderable, IEntityWithWebPage, IUrl
    {
        /// <summary>
        /// Language of the attachment.
        /// </summary>
        public Language Localization { get; set; }
        /// <summary>
        /// Language guid.
        /// </summary>
        public Guid LocalizationId { get; set; }

        /// <summary>
        /// Service channel where this attachment is related to.
        /// </summary>
        public PrintableFormChannel PrintableFormChannel { get; set; }
        /// <summary>
        /// Service channel guid.
        /// </summary>
        public Guid PrintableFormChannelId { get; set; }

        public Guid TypeId { get; set; }

        /// <summary>
        /// Filetype (extension) of the Url.
        /// </summary>
        public PrintableFormChannelUrlType Type { get; set; }

        /// <summary>
        /// Url to the attachment.
        /// </summary>
        [MaxLength(500)]
        [TrimSpacesFormatter]
        [Obsolete]
        public string Url { get; set; }
        /// <summary>
        /// Order number
        /// </summary>
        public int? OrderNumber { get; set; }

        public Guid WebPageId { get; set; }
        public WebPage WebPage { get; set; }
    }
}
