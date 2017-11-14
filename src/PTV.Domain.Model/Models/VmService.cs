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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model for result of publishing process of entity
    /// </summary>
    public class VmPublishingResultModel : VmPublishingModel, IEnumCollection, IVmEntityStatusBase
    {
        /// <summary>
        /// Gets or sets the enum collection.
        /// </summary>
        /// <value>
        /// The enum collection.
        /// </value>
        public IVmDictionaryItemsData<IEnumerable<IVmBase>> EnumCollection { get; set; }
        /// <summary>
        /// Identifier of entity which was published
        /// </summary>
        public new Guid? Id { get; set; }
        /// <summary>
        /// New publishing status of entity
        /// </summary>
        public Guid? PublishingStatusId { get; set; }

        /// <summary>
        /// Version of related entity
        /// </summary>
        public VmVersion Version { get; set; }
    }

    /// <summary>
    /// View model of service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmServiceBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmService" />
    public class VmService : VmServiceBase, IVmService
    {
        /// <summary>
        /// Gets or sets the step1 form.
        /// </summary>
        /// <value>
        /// The step1 form.
        /// </value>
        public VmServiceStep1 Step1Form { get; set; }

        /// <summary>
        /// Gets or sets the step2 form.
        /// </summary>
        /// <value>
        /// The step2 form.
        /// </value>
        public VmServiceStep2 Step2Form { get; set; }

        /// <summary>
        /// Gets or sets the step3 form.
        /// </summary>
        /// <value>
        /// The step3 form.
        /// </value>
        public VmServiceStep3 Step3Form { get; set; }

        /// <summary>
        /// Gets or sets the step4 form.
        /// </summary>
        /// <value>
        /// The step4 form.
        /// </value>
        public List<Guid> Step4Form { get; set; }

        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public LanguageCode Language { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmService"/> class.
        /// </summary>
        public VmService()
        {
            Step4Form = new List<Guid>();
        }

    }

}
