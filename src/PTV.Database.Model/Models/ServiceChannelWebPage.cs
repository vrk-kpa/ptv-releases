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
using System.ComponentModel.DataAnnotations;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.Interfaces;
using PTV.Framework.Formatters;

namespace PTV.Database.Model.Models
{
    internal class ServiceChannelWebPage : EntityIdentifierBase, IEntityWithWebPage, IName, IDescription, IOrderable
    {
        public Guid ServiceChannelVersionedId { get; set; }
        [MaxLength(110)]
        [TrimSpacesFormatter]
        public string Name { get; set; }
        [TrimSpacesFormatter]
        public string Description { get; set; }
        public Guid TypeId { get; set; }
        public Guid WebPageId { get; set; }


        public ServiceChannelVersioned ServiceChannelVersioned { get; set; }
        public WebPageType Type { get; set; }
        public virtual WebPage WebPage { get; set; }
        public int? OrderNumber { get; set; }
        public Guid LocalizationId { get; set; }
        public Language Localization { get; set; }

        Guid IName.TypeId => throw new NotSupportedException();
        Guid IDescription.TypeId => throw new NotSupportedException();
    }
}
