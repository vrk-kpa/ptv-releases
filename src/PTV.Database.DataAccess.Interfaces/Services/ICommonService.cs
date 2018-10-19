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
using Microsoft.AspNetCore.Mvc.Rendering;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using System.Linq.Expressions;
using PTV.Database.Model.Models;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface ICommonService
    {
        /// <summary>
        /// Get data for UI front page search
        /// </summary>
        /// <returns></returns>
        IVmGetFrontPageSearch GetFrontPageSearch();

        /// <summary>
        /// Get data for UI
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        IVmDictionaryItemsData<IEnumerable<IVmBase>> GetEnumTypes(VmUserInfoBase organizationId);

        /// <summary>
        /// Get data for UI
        /// </summary>
        /// <returns></returns>
        IVmDictionaryItemsData<IEnumerable<IVmBase>> GetEnumTypesForLogin();

        /// <summary>
        /// Get data for UI
        /// </summary>
        /// <returns></returns>
        IVmDictionaryItemsData<IEnumerable<IVmBase>> GetOrganizationEnum();
        /// <summary>
        /// Get list of data Types specified by string name
        /// </summary>
        /// <param name="dataTypes"></param>
        /// <returns></returns>
        IVmBase GetTypedData(IEnumerable<string> dataTypes);
        /// <summary>
        ///  Get list of web phone charge types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetPhoneChargeTypes();
        /// <summary>
        ///  Get list of web page types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetWebPageTypes();
        /// <summary>
        ///  Get list of service types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetServiceTypes();
        /// <summary>
        /// Get list of provision types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetProvisionTypes();
        /// <summary>
        ///  Get list of printable form url types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetPrintableFormUrlTypes();
        /// <summary>
        ///  Get list of phone types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetPhoneTypes();
        /// <summary>
        ///  Get list of service hour types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetServiceHourTypes();

        /// <summary>
        ///  Get list of area information types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetAreaInformationTypes();

        /// <summary>
        ///  Get list of area types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetAreaTypes();

        /// <summary>
        /// Get all areas of specific type
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="type">Type of areas</param>
        /// <returns>List of areas</returns>
        IReadOnlyList<VmListItemReferences> GetAreas(IUnitOfWork unitOfWork, AreaTypeEnum type);

        /// <summary>
        /// Get a list of area codes
        /// </summary>
        /// <param name="type">Type of areas</param>
        /// <returns>List of area codes.</returns>
        IList<VmOpenApiCodeListItem> GetAreaCodeList(AreaTypeEnum? type);

        /// <summary>
        /// Get list of publishing statuses
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetPublishingStatuses();

        /// <summary>
        /// Get Organizations names.
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="takeAll"></param>
        /// <returns>List of Organizations names by search text.</returns>
        IReadOnlyList<VmListItem> GetOrganizationNames(string searchText = null, bool takeAll = true);

        /// <summary>
        ///  Get list of languages
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetLanguages();

        /// <summary>
        /// Translation language list of strings
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<string> GetTranslationLanguageList();

        /// <summary>
        ///  Get language code
        /// </summary>
        /// <param name="languageId"></param>
        /// <returns></returns>
        string GetLocalization(Guid? languageId);
        /// <summary>
        ///  Get language Id
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        Guid GetLocalizationId(string langCode);

        /// <summary>
        ///  Get list of channelTypes
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetServiceChannelTypes();

        /// <summary>
        /// Gets all municipalities
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="onlyValid"></param>
        /// <returns></returns>
        IReadOnlyList<VmListItem> GetMunicipalities(IUnitOfWork unitOfWork, bool onlyValid = true);
        /// Returns organization names filtered
        /// </summary>
        /// <param name="unitOfWork">unit of work</param>
        /// <param name="organizationSet">organizations which should not be listed</param>
        /// <returns>organization names filtered</returns>
        IReadOnlyList<VmListItem> GetOrganizationNamesWithoutSetOfOrganizations(IUnitOfWork unitOfWork, IList<Guid?> organizationSet);
        /// <summary>
        /// Returns all supported translation languages
        /// </summary>
        /// <returns>supported translation languages</returns>
        IReadOnlyList<VmListItem> GetTranslationLanguages();
        /// <summary>
        ///  Get list of coordinate types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetCoordinateTypes();


        /// <summary>
        /// Retruns Default prefix number
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        IReadOnlyList<VmDialCode> GetDefaultDialCode(IUnitOfWork unitOfWork);

        Guid GetDraftStatusId();
        /// <summary>
        /// Returns all laws according takenIds
        /// </summary>
        /// <param name="unitOfWork">unit of work</param>
        /// <param name="takeIds">Ids of laws</param>
        /// <returns></returns>
        IReadOnlyList<VmLaw> GetLaws(IUnitOfWork unitOfWork, List<Guid> takeIds);

        void ExtendPublishingStatusesByEquivalents(IList<Guid> statuses);

        /// <summary>
        ///  Return true/false if value is required enum type
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsDescriptionEnumType(Guid typeId, string type);

        /// <summary>
        /// Get description by code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Guid GetDescriptionTypeId(string code);

        /// <summary>
        ///  Get list of service channel connection types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetServiceChannelConnectionTypes();

        /// <summary>
        /// Checks if organization with defined id exists
        /// </summary>
        /// <param name="id">The organization id</param>
        /// <param name="requiredStatus">The requested publishing status</param>
        /// <returns>true/false</returns>
        bool OrganizationExists(Guid id, PublishingStatus? requiredStatus = null);

       /// <summary>
        /// Get all funding types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmEnumType> GetServiceFundingTypes();

        /// <summary>
        /// Saves the environment instructions.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>saved informations</returns>
        IVmListItemsData<VmEnvironmentInstruction> SaveEnvironmentInstructions(VmEnvironmentInstructionsIn model);

        /// <summary>
        /// Gets the environment instructions.
        /// </summary>
        /// <returns></returns>
        IVmListItemsData<VmEnvironmentInstruction> GetEnvironmentInstructions();

        /// <summary>
        /// Gets services and service channels by organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="date"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IVmOpenApiGuidPageVersionBase GetServicesAndChannelsByOrganization(Guid organizationId, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null);
        VmEntityHeaderBase GetValidatedHeader(VmEntityHeaderBase header, Dictionary<Guid, List<ValidationMessage>> validationMessages);
        /// <summary>
        /// Get area information for service from organization
        /// </summary>
        /// <param name="model">IVmGetAreaInformation</param>
        /// <returns>IVmServiceAreaInformation</returns>
        IVmAreaInformation GetAreaInformationForOrganization(IVmGetAreaInformation model);

        IReadOnlyList<VmListItem> GetOrganizations(IEnumerable<Guid> ids);
    }

    internal interface ICommonServiceInternal : ICommonService
    {
        /// <summary>
        /// Fill versioning meta data
        /// </summary>
        /// <param name="entity">Version entity</param>
        /// <param name="action">Action (Save, Delete, Restore, Publish, Withdraw)</param>
        /// <param name="setByEntity">Set All language avail. status same as entity publ. status</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        void AddHistoryMetaData<TEntity, TLanguageAvail>(TEntity entity, HistoryAction action = HistoryAction.Save, bool setByEntity = false)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;
        PublishingResult PublishEntity<TEntity, TLanguageAvail>(IVmLocalizedEntityModel model) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailability;
        PublishingResult PublishEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model, bool saveAutomatically = true) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailability;
        PublishingResult PublishAllAvailableLanguageVersions<TEntity, TLanguageAvail>(Guid Id, Expression<Func<TLanguageAvail, bool>> getSelectedIdFunc) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailability;
        IList<PublishingAffectedResult> RestoreArchivedEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid versionId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailability;

        VmPublishingResultModel WithdrawEntity<TEntity, TLanguageAvail>(Guid entityVersionedId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability;

        TEntity ArchiveLanguage<TEntity, TLanguageAvail>(VmEntityBasic model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        TEntity RestoreLanguage<TEntity, TLanguageAvail>(VmEntityBasic model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        TEntity WithdrawLanguage<TEntity, TLanguageAvail>(VmEntityBasic model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        TEntity ChangeEntityToDeleted<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid entityId)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        TEntity ChangeEntityVersionedToDeleted<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork,
            Guid entityId)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;
        
        VmPublishingResultModel WithdrawEntityByRootId<TEntity, TLanguageAvail>(Guid rootId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability;

        VmPublishingResultModel RestoreEntity<TEntity, TLanguageAvail>(Guid entityVersionedId, Func<IUnitOfWork, TEntity, bool> additionalCheckAction = null) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability;

        bool CheckModifiedExists<TEntity>(Guid versionedId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IValidity;

		List<OrganizationTreeItem> GetOrganizationNamesTree(string searchText);
        List<OrganizationTreeItem> GetOrganizationNamesTree(ICollection<Guid> ids = null);

        /// <summary>
        /// Execute publish entity with validation mandatory fields.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="model"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        /// <returns></returns>
        PublishingResult ExecutePublishEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        /// <summary>
        /// Execute publish entities with validation of mandatory of fields
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="modelList"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        /// <returns></returns>
        IList<PublishingResult> ExecutePublishEntities<TEntity, TLanguageAvail>(
            IUnitOfWorkWritable unitOfWork, IReadOnlyList<IVmLocalizedEntityModel> modelList)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        TEntity ArchiveLanguageVersionWithVersioning<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        /// <summary>
        /// Execute archiving language versions of entity
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="modelList"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        void ExecuteArchiveEntityLanguageVersions<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork,
            IReadOnlyList<IVmLocalizedEntityModel> modelList)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        /// <summary>
        /// Execute archive of entities
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityIds"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        void ExecuteArchiveEntities<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork,
            IReadOnlyList<Guid> entityIds)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;
        
        /// <summary>
        /// Schedule publiching or archiving entity
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="model"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        /// <returns></returns>
        PublishingResult SchedulePublishArchiveEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork,
            IVmLocalizedEntityModel model)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        /// <summary>
        /// Copy(clone) entity, set organization, without connections, based as Draft version 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityVersionedId"></param>
        /// <param name="organizationId"></param>
        /// <typeparam name="TEntityVersioned"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        void CopyEntity<TEntityVersioned, TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid entityVersionedId, Guid? organizationId)
            where TEntityVersioned : class, IEntityIdentifier, IVersionedVolume<TEntity>, IMultilanguagedEntity<TLanguageAvail>, IOriginalEntity, IOrganizationInfo, INameReferences, new()
            where TEntity : IVersionedRoot, new()
            where TLanguageAvail : class, ILanguageAvailability;
        
        /// <summary>
        /// Execute Copy(clone) entities, set organization, without connections, based as Draft version 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityIds"></param>
        /// <param name="organizationId"></param>
        /// <typeparam name="TEntityVersioned"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        void ExecuteCopyEntities<TEntityVersioned, TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IReadOnlyList<Guid> entityIds, Guid? organizationId = null)
            where TEntityVersioned : class, IEntityIdentifier, IVersionedVolume<TEntity>, IMultilanguagedEntity<TLanguageAvail>, IOriginalEntity, IOrganizationInfo, INameReferences, new()
            where TEntity : IVersionedRoot, new()
            where TLanguageAvail : class, ILanguageAvailability;

        /// <summary>
        /// Set finalized properties to copied entity
        /// </summary>
        /// <param name="newEntity"></param>
        /// <param name="originEntityId"></param>
        /// <param name="organizationId"></param>
        /// <typeparam name="TEntityVersioned"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        void FinalizeCopyEntity<TEntityVersioned, TLanguageAvail>(TEntityVersioned newEntity, Guid originEntityId, Guid? organizationId)
            where TEntityVersioned : class, IEntityIdentifier, IVersionedVolume, IOriginalEntity, IOrganizationInfo, INameReferences, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;
    }
}
