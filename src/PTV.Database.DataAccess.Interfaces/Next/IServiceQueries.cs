using System;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Interfaces.Next
{
    public interface IServiceQueries
    {
        ServiceModel GetModel(Guid versionedId);
        ValidationModel<ServiceModel> Validate(Guid id);
        InfiniteModel<EntityHistoryModel> GetEditHistory(Guid id, int page);
        InfiniteModel<ConnectionHistoryModel> GetConnectionHistory(Guid id, int page);
        PublishingStatus? GetLanguageVersionPublishingStatus(Guid id, LanguageEnum lang);
        PublishingStatus? GetServicePublishingStatus(Guid id);
    }
}