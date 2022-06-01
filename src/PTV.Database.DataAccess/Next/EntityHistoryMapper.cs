using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next
{
    public static class EntityHistoryMapper
    {
        private static EntityHistoryModel.CopyDetails GetCopyDetails(this VmEntityOperation input, ITypesCache typesCache)
        {
            if (input.TemplateId == null || input.TemplateOrganizationId == null)
            {
                return null;
            }
            
            return new EntityHistoryModel.CopyDetails
            {
                TemplateId = input.TemplateId.Value,
                TemplateOrganizationId = input.TemplateOrganizationId.Value,
                TemplateOrganizationNames = input.TemplateOrganization.Name.ToDictionary(x => x.Key.ToEnum<LanguageEnum>(), x => x.Value)
            };
        }

        

        private static EntityHistoryModel ToNewModel(this VmEntityOperation input, ITypesCache typesCache)
        {
            return new EntityHistoryModel
            {
                CopyInfo = input.GetCopyDetails(typesCache),
                EditedAt = input.Created.FromEpochTime(),
                Editor = input.CreatedBy,
                EntityType = input.EntityType,
                HistoryAction = input.HistoryAction,
                Id = input.EntityId,
                LanguageVersions = input.GetLanguageVersions(typesCache),
                OperationId = input.Id,
                SubEntityType = input.SubEntityType,
                Version = $"{input.VersionMajor}.{input.VersionMinor}",
                NextVersion = $"{input.VersionMajor + 1}.0",
                SourceLanguage = input.SourceLanguageId.HasValue
                    ? typesCache.GetByValue<Language>(input.SourceLanguageId.Value).ToEnum<LanguageEnum>()
                    : (LanguageEnum?)null,
                TargetLanguages = input.TargetLanguageIds
                    .Select(x => typesCache.GetByValue<Language>(Guid.Parse(x)).ToEnum<LanguageEnum>()).ToList(),
                ShowLink = input.ShowLink
            };
        }
        
        internal static InfiniteModel<EntityHistoryModel> ToNewModel(this VmSearchResult<VmEntityOperation> input, ITypesCache typesCache)
        {
            var result = new InfiniteModel<EntityHistoryModel>
                {IsMoreAvailable = input.MoreAvailable, Page = input.PageNumber};

            foreach (var operation in input.SearchResult)
            {
                var historyItem = operation.ToNewModel(typesCache);
                result.Data.Add(historyItem);

                foreach (var subOperation in operation.SubOperations)
                {
                    var historySubItem = subOperation.ToNewModel(typesCache);
                    result.Data.Add(historySubItem);
                }
            }

            return result;
        }
    }
}