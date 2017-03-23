using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Interfaces
{
    public class PublishingAffectedResult
    {
        public Guid Id { get; set; }
        public Guid PublishingStatusOld { get; set; }
        public Guid PublishingStatusNew { get; set; }
    }

    public class PublishingResult : PublishingAffectedResult
    {
        public IList<PublishingAffectedResult> AffectedEntities { get; set; }

        public VmVersion Version { get; set; }
    }
}