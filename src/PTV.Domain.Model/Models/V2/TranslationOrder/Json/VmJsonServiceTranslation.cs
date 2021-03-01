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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models.V2.TranslationOrder.Json
{
    /// <summary>
    /// View model for result of publishing process of entity
    /// </summary>
    public class VmJsonServiceTranslation : ITranslationData
    {
        /// <summary>
        /// Id of entity
        /// </summary>
        [JsonIgnore]
        public Guid Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [MaxLength(100)]
        public VmJsonTranslationText Name { get; set; }

        /// <summary>
        /// Alternate name
        ///  </summary>
        [MaxLength(100)]
        public VmJsonTranslationText AlternateName { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        [MaxLength(150)]
        public VmJsonTranslationText Summary { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [MaxLength(2500)]
        public VmJsonTranslationText Description { get; set; }

        /// <summary>
        /// Conditions And Criteria
        /// </summary>
        [MaxLength(2500)]
        public VmJsonTranslationText ConditionsAndCriteria { get; set; }

        /// <summary>
        /// User instructions
        /// </summary>
        [MaxLength(2500)]
        public VmJsonTranslationText UserInstructions { get; set; }

        /// <summary>
        /// Charge additional information
        /// </summary>
        [MaxLength(500)]
        public VmJsonTranslationText ChargeAdditionalInformation { get; set; }

        /// <summary>
        /// Deadline
        /// </summary>
        [MaxLength(500)]
        public VmJsonTranslationText Deadline { get; set; }

        /// <summary>
        /// Processing time
        /// </summary>
        [MaxLength(500)]
        public VmJsonTranslationText ProcessingTime { get; set; }

        /// <summary>
        /// Deadline
        /// </summary>
        [MaxLength(500)]
        public VmJsonTranslationText PeriodOfValidity { get; set; }
    }
}
