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

namespace PTV.Domain.Model.Models.OpenApi.V11
{
    /// <summary>
    /// Model for fetching archived service channels
    /// </summary>
    public class V11VmOpenApiGetArchivedServiceChannels
    {
        /// <summary>
        /// How channel was archived.
        /// </summary>
        [Required]
        public ArchivingType ArchivingType { get; set; }

        /// <summary>
        /// Return only channels belonging to this organization.
        /// </summary>
        [Required]
        public Guid OrganizationId { get;set; }

        /// <summary>
        /// Return only channels archived after this time.
        /// ISO 8601 format (e.g. 2020-10-26T05:24:11+00:00).
        /// </summary>
        public DateTimeOffset? MinArchivingDate { get; set; }

        /// <summary>
        /// Return only channels archived before this time.
        /// ISO 8601 format (e.g. 2020-10-26T05:24:11+00:00).
        /// </summary>
        public DateTimeOffset? MaxArchivingDate { get; set; }

        /// <summary>
        /// Skip the first n channels.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Skip { get; set; }

        /// <summary>
        /// How many channels to return.
        /// </summary>
        [Required]
        [Range(0, 100)]
        public int Take { get; set; }
    }
}
