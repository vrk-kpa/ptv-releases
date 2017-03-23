using System;
using System.Collections.Generic;
using PTV.Database.Model.Models;

namespace PTV.Database.Model.Interfaces
{
    internal interface ILanguageAvailability
    {
        Guid LanguageId { get; set; }

        Guid StatusId { get; set; }

        PublishingStatusType Status { get; set; }
    }

    internal interface IMultilanguagedEntity { }

    internal interface IMultilanguagedEntity<T> : IMultilanguagedEntity where T : ILanguageAvailability
    {
        ICollection<T> LanguageAvailabilities { get; set; }
    }
}