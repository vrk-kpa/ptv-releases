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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of serach general description form
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEnumBase" />
    public class VmGeneralDescriptionSearchForm : VmEnumBase, IVmMultiLocalized, IVmSearchParamsBase
    {
        /// <summary>
        /// Constructor of general description search form.
        /// </summary>
        public VmGeneralDescriptionSearchForm()
        {
            SortData = new List<VmSortParam>();
        }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public int PageNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxPageCount { get; set; }

        /// <summary>
        /// Gets or sets the name of the general description.
        /// </summary>
        /// <value>
        /// The name of the general description.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the target group identifier.
        /// </summary>
        /// <value>
        /// The target group identifier.
        /// </value>
        public Guid? TargetGroupId { get; set; }
        /// <summary>
        /// Gets or sets the sub target group identifier.
        /// </summary>
        /// <value>
        /// The sub target group identifier.
        /// </value>
        public Guid? SubTargetGroupId { get; set; }
        /// <summary>
        /// Gets or sets the service class identifier.
        /// </summary>
        /// <value>
        /// The service class identifier.
        /// </value>
        public Guid? ServiceClassId { get; set; }

        /// <summary>
        /// Gets or sets the service type identifier.
        /// </summary>
        /// <value>
        /// The service type identifier.
        /// </value>
        public Guid? ServiceTypeId { get; set; }

        /// <summary>
        /// Gets or sets the languages codes.
        /// </summary>
        /// <value>
        /// The language codes.
        /// </value>
        public List<LanguageCode> Languages { get; set; }
        /// <summary>
        /// Gets or sets how many items to skip.
        /// </summary>
        /// <value>
        /// The skip amount.
        /// </value>
        public int Skip { get; set; }
        /// <summary>
        /// Gets or sets the selected publishing statuses.
        /// </summary>
        /// <value>
        /// The selected publishing statuses.
        /// </value>
        public List<Guid> SelectedPublishingStatuses { get; set; }

        /// <summary>
        /// List if sorting params.
        /// </summary>
        public List<VmSortParam> SortData { get; set; }
        
        /// <summary>
        /// Search only published.
        /// </summary>
        public bool OnlyPublished { get; set; }
    }
}
