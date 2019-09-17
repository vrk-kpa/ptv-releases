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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;

namespace PTV.Database.DataAccess.Tests.Services.EntityServiceBase
{
    internal class TestEntityVersioned : IEntityIdentifier, IVersionedVolume<TestEntityRoot>,
        IMultilanguagedEntity<TestLanguageAvail>, IValidity, IAuditing
    {
        public Guid Id { get; set; }
        public bool VersioningApplied { get; set; }
        public PublishingStatusType PublishingStatus { get; set; }
        public Guid PublishingStatusId { get; set; }
        public Guid? VersioningId { get; set; }
        public Versioning Versioning { get; set; }
        public Guid UnificRootId { get; set; }
        public TestEntityRoot UnificRoot { get; set; }
        public IEnumerable<ILanguageAvailability> LanguageAvailabilitiesReference { get; }
        public ICollection<TestLanguageAvail> LanguageAvailabilities { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }
        public string LastOperationType { get; set; }
        public Guid LastOperationIdentifier { get; set; }
        public DateTime LastOperationTimeStamp { get; set; }
    }
}