using System;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Interfaces.Next
{
    public interface IGeneralDescriptionQueries
    {
        GdSearchResultModel Search(GdSearchModel searchParameters);
        GeneralDescriptionModel Get(Guid versionedId, List<PublishingStatus> acceptedStatuses);
        GeneralDescriptionModel GetPublishedByUnificRootId(Guid unificRootId);
    }
}
