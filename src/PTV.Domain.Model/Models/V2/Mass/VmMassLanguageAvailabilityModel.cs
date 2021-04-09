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

namespace PTV.Domain.Model.Models.V2.Mass
{
    /// <summary>
    ///  View model for Mass language availability
    /// </summary>
    public class VmMassLanguageAvailabilityModel
    {
        /// <summary>
        /// Identifier of entity that should be published
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Language Id
        /// </summary>
        public Guid LanguageId { get; set; }

        /// <summary>
        /// Valid from
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Valid to
        /// </summary>
        public DateTime? ValidTo { get; set; }

        /// <summary>
        /// Reviewed
        /// </summary>
        public DateTime? Reviewed { get; set; }

        /// <summary>
        /// Reviewed by
        /// </summary>
        public string ReviewedBy { get; set; }

        /// <summary>
        /// Allow Save Null
        /// </summary>
        public bool AllowSaveNull { get; set; }

        /// <summary>
        /// Archived
        /// </summary>
        public DateTime? Archived { get; set; }

        /// <summary>
        /// Reviewed by
        /// </summary>
        public string ArchivedBy { get; set; }

        /// <summary>
        /// Publish action
        /// </summary>
        public PublishActionTypeEnum? PublishAction { get; set; }

    }
}
