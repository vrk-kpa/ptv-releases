using System;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next
{
    public static class ConnectionHistoryMapper
    {
        private static ConnectionHistoryModel ToNewModel(this VmConnectionOperation input, ITypesCache typesCache)
        {
            return new ConnectionHistoryModel
            {
                OperationType = input.OperationType.ToEnum<ConnectionHistoryOperation>(),
                EditedAt = input.Created.FromEpochTime(),
                Editor = input.CreatedBy,
                LanguageVersions = input.GetLanguageVersions(typesCache),
                OperationId = input.Id,
                EntityType = input.EntityType,
                SubEntityType = GetSubEntityType(input.EntityType, input.SubEntityType, typesCache),
                Id = input.EntityId,
            };
        }

        private static SubEntityType GetSubEntityType(EntityTypeEnum entityType, Guid? subEntityType, ITypesCache typesCache)
        {
            if (subEntityType == null)
            {
                return SubEntityType.Unknown;
            }
            
            switch (entityType)
            {
                case EntityTypeEnum.Channel:
                    return typesCache.GetByValue<ServiceChannelType>(subEntityType.Value).ToEnum<SubEntityType>();
                case EntityTypeEnum.Service:
                    return typesCache.GetByValue<ServiceType>(subEntityType.Value).ToEnum<SubEntityType>();
                case EntityTypeEnum.GeneralDescription:
                    return typesCache.GetByValue<GeneralDescriptionType>(subEntityType.Value).ToEnum<SubEntityType>();
                case EntityTypeEnum.Organization:
                    return SubEntityType.Organization;
                case EntityTypeEnum.ServiceCollection:
                    return SubEntityType.ServiceCollection;
                default:
                    return SubEntityType.Unknown;
            }
        }

        internal static InfiniteModel<ConnectionHistoryModel> ToNewModel(this VmSearchResult<VmConnectionOperation> input,
            ITypesCache typesCache)
        {
            var result = new InfiniteModel<ConnectionHistoryModel>
                {IsMoreAvailable = input.MoreAvailable, Page = input.PageNumber};
            
            foreach (var operation in input.SearchResult)
            {
                var historyItem = operation.ToNewModel(typesCache);
                result.Data.Add(historyItem);
            }

            return result;
        }
    }
}