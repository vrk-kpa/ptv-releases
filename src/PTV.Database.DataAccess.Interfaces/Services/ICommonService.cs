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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;
using System.Linq.Expressions;
using PTV.Database.Model.Models;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.V2.Mass;
using PTV.Domain.Model.Models.V2.TranslationOrder;
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
        /// Get a list of area codes
        /// </summary>
        /// <param name="type">Type of areas</param>
        /// <returns>List of area codes.</returns>
        IList<VmOpenApiCodeListItem> GetAreaCodeList(AreaTypeEnum? type);

        /// <summary>
        /// Get Organizations names.
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="takeAll"></param>
        /// <param name="allowedPublishingStatuses"></param>
        /// <returns>List of Organizations names by search text.</returns>
        IReadOnlyList<VmListItem> GetOrganizationNames(string searchText = null, bool takeAll = true, List<PublishingStatus> allowedPublishingStatuses = null);

        /// <summary>
        /// Translation language list of strings
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<string> GetTranslationLanguageList();

        /// <summary>
        ///  Get language Id
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        Guid GetLocalizationId(string langCode);

        /// Returns organization names filtered
        /// </summary>
        /// <param name="unitOfWork">unit of work</param>
        /// <param name="organizationSet">organizations which should not be listed</param>
        /// <returns>organization names filtered</returns>
        IReadOnlyList<VmListItem> GetOrganizationNamesWithoutSetOfOrganizations(IUnitOfWork unitOfWork, IList<Guid?> organizationSet);

        void ExtendPublishingStatusesByEquivalents(IList<Guid> statuses);

        void ExtendPublishingStatusesByEquivalents(IList<PublishingStatus> statuses);

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
        /// Checks if organization with defined id exists
        /// </summary>
        /// <param name="id">The organization id</param>
        /// <param name="requiredStatus">The requested publishing status</param>
        /// <returns>true/false</returns>
        bool OrganizationExists(Guid id, PublishingStatus? requiredStatus = null);

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
        IVmOpenApiModelWithPagingBase<VmOpenApiEntityItem> GetServicesAndChannelsByOrganization(Guid organizationId, bool getSpecialTypes, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null);

        VmEntityHeaderBase GetValidatedHeader(VmEntityHeaderBase header, Dictionary<Guid, List<ValidationMessage>> validationMessages);

        /// <summary>
        /// Get area information for service from organization
        /// </summary>
        /// <param name="model">IVmGetAreaInformation</param>
        /// <returns>IVmServiceAreaInformation</returns>
        IVmAreaInformation GetAreaInformationForOrganization(IVmGetAreaInformation model);
        
        /// <summary>
        /// Get area information type of organization
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        AreaInformationTypeEnum GetAreaInformationTypeForOrganization(IVmGetAreaInformation model);

        IReadOnlyList<VmListItem> GetOrganizations(IEnumerable<Guid> ids);

        bool IsOidUniqueForOrganization(string oid, Guid? organizationId = null, IUnitOfWork unitOfWork = null);

/* SOTE has been disabled (SFIPTV-1177)
        /// <summary>
        /// Check, is organization is sote type
        /// </summary>
        /// <param name="organizationType"></param>
        /// <returns></returns>
        bool OrganizationIsSote(Guid? organizationType);

        /// <summary>
        /// Check, is organization is sote type
        /// </summary>
        /// <param name="organizationType"></param>
        /// <returns></returns>
        bool OrganizationIsSote(string organizationType);

        /// <summary>
        /// Creates GeneralDescriptionFocTypeOrganizationSoteRelation filters for organization list
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="unificRootIds"></param>
        void HandleOrganizationSoteFocFilter(IUnitOfWorkWritable unitOfWork, List<Guid> unificRootIds);
*/
        /// <summary>
        /// Get restricted general descriptions
        /// </summary>
        /// <returns></returns>
        List<Guid> GetRestrictedDescriptionTypes();

        Guid? GetDefaultDialCodeId(IUnitOfWork unitOfWork);
        /// <summary>
        /// Get additional information of entities about last modified version
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IVmListItemsData<IVmLastModifiedInfo> GetEntityLastModifiedInfos(VmMassDataModel<VmRestoringModel> model);

        /// <summary>
        /// Check service name exist within organization.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="organizationId"></param>
        /// <param name="entityUnificRootId"></param>
        /// <returns></returns>
        bool CheckExistsServiceNameWithinOrganization(string name, Guid organizationId, Guid? entityUnificRootId = null);

        /// <summary>
        /// Check channel name exist within organization.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="organizationId"></param>
        /// <param name="channelTypeId"></param>
        /// <param name="entityUnificRootId"></param>
        /// <returns></returns>
        bool CheckExistsChannelNameWithinOrganization(string name, Guid organizationId, Guid channelTypeId, Guid? entityUnificRootId = null);

        /// <summary>
        /// Get information on entity lock
        /// </summary>
        /// <param name="id">Unific root id</param>
        /// <returns></returns>
        IVmEntityLockBase EntityLockedBy(Guid id);

        /// <summary>
        /// Get Service channel type id
        /// </summary>
        /// <param name="serviceChannelType"></param>
        /// <returns></returns>
        Guid GetServiceChannelTypeId(string serviceChannelType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceChannleUnificRootId"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        bool CheckExistsAstiConnections(Guid serviceChannleUnificRootId, Guid organizationId);
    }

    internal interface ICommonServiceInternal : ICommonService
    {
        void IncludeConnectionAddresses(IUnitOfWork unitOfWork, List<ServiceServiceChannel> connections);
        void IncludeConnectionCommonDetails(IUnitOfWork unitOfWork, List<ServiceServiceChannel> connections);
        void IncludeConnectionContactDetails(IUnitOfWork unitOfWork, List<ServiceServiceChannel> connections);
        /// <summary>
        /// Fill versioning meta data
        /// </summary>
        /// <param name="entity">Version entity</param>
        /// <param name="copyTemplate">Information about the source entity in case the history action is Copy.</param>
        /// <param name="translationOrderDetails">Information about translation order.</param>
        /// <param name="action">Action (Save, Delete, Restore, Publish, Withdraw)</param>
        /// <param name="setByEntity">Set All language avail. status same as entity publ. status</param>
        /// <param name="forceUpdate">Set whether existing history metadata should be rewritten if they exist.</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        void CreateHistoryMetaData<TEntity, TLanguageAvail>(
            TEntity entity,
            ICopyTemplate copyTemplate = null,
            VmTranslationOrderEntityTargetLanguages translationOrderDetails = null,
            HistoryAction? action = null,
            bool setByEntity = false,
            bool forceUpdate = false)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        PublishingResult PublishEntity<TEntity, TLanguageAvail>(IVmLocalizedEntityModel model) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IAuditing, new() where TLanguageAvail : class, ILanguageAvailability;
        PublishingResult PublishEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model,  HistoryAction publishAction = HistoryAction.Publish) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IAuditing, new() where TLanguageAvail : class, ILanguageAvailability;
        PublishingResult PublishAndScheduleEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IAuditing, new() where TLanguageAvail : class, ILanguageAvailability;
        PublishingResult PublishAllAvailableLanguageVersions<TEntity, TLanguageAvail>(Guid Id, Expression<Func<TLanguageAvail, bool>> getSelectedIdFunc) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IAuditing, new() where TLanguageAvail : class, ILanguageAvailability;
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

        /// <summary>
        /// Sets an unversioned entity to archived state.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityId"></param>
        /// <param name="action"></param>
        /// <param name="allowAnonymous"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        /// <returns></returns>
        TEntity ChangeEntityToDeleted<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid entityId, HistoryAction action, bool allowAnonymous = false, Action<IUnitOfWorkWritable, Guid, TEntity> onAdditionalAction = null)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IAuditing, new()
            where TLanguageAvail : class, ILanguageAvailability;

        TEntity ChangeEntityToRemoved<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, Guid entityId)  //with DraftModifiedPublished version
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        /// <summary>
        /// Sets a versioned entity to archived state.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityId"></param>
        /// <param name="action"></param>
        /// <param name="allowAnonymous"></param>
        /// <param name="onAdditionalAction"></param>
        /// <param name="expiredAfterMonths">Number of months after which the content is bound to expire.</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        /// <returns></returns>
        TEntity ChangeEntityVersionedToDeleted<TEntity, TLanguageAvail>(
            IUnitOfWorkWritable unitOfWork,
            Guid entityId,
            HistoryAction action,
            bool allowAnonymous = false,
            Action<IUnitOfWorkWritable, Guid, TEntity> onAdditionalAction = null,
            string expiredAfterMonths = null)
            where TEntity : class, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IAuditing, new()
            where TLanguageAvail : class, ILanguageAvailability;

        VmPublishingResultModel RestoreEntity<TEntity, TLanguageAvail>(Guid entityVersionedId, Func<IUnitOfWork, TEntity, bool> additionalCheckAction = null) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability;

        List<OrganizationTreeItem> GetOrganizationNamesTree(string searchText);
        List<OrganizationTreeItem> GetOrganizationNamesTree(ICollection<Guid> ids = null);

//        /// <summary>
        /// <summary>
        /// Execute publish entities with validation of mandatory of fields
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="modelList"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        /// <returns></returns>
        IList<PublishingResult> ExecutePublishEntities<TEntity, TLanguageAvail>(
            IUnitOfWorkWritable unitOfWork, IReadOnlyList<IVmLocalizedEntityModel> modelList, bool allowAnonymous = false)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IAuditing, new()
            where TLanguageAvail : class, ILanguageAvailability;

        TEntity ArchiveLanguageVersionWithVersioning<TEntity, TLanguageAvail>(
            IUnitOfWorkWritable unitOfWork,
            IVmLocalizedEntityModel model,
            HistoryAction action,
            bool allowAnonymous = false,
            Action<IUnitOfWorkWritable, Guid, TEntity> onAdditionalAction = null)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IAuditing, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability;

        /// <summary>
        /// Execute archiving language versions of entity - for Mass tool
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="modelList"></param>
        /// <param name="action">The type of archiving used</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        void ExecuteArchiveEntityLanguageVersions<TEntity, TLanguageAvail>(
            IUnitOfWorkWritable unitOfWork,
            IReadOnlyList<IVmLocalizedEntityModel> modelList,
            HistoryAction action,
            Action<IUnitOfWorkWritable, Guid, TEntity> onAdditionalAction = null)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IAuditing, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability;

        /// <summary>
        /// Execute archive of entities - for Mass tool
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityIds"></param>
        /// <param name="action">The type of archiving used</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        void ExecuteArchiveEntities<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork,
            IReadOnlyList<Guid> entityIds,
            HistoryAction action,
            Action<IUnitOfWorkWritable, Guid, TEntity> onAdditionalAction = null)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IAuditing, new()
            where TLanguageAvail : class, ILanguageAvailability;

        /// <summary>
        /// Schedule publiching or archiving entity
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="model"></param>
        /// <param name="updateHistory">Update version history</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        /// <returns></returns>
        PublishingResult SchedulePublishArchiveEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork,
            IVmLocalizedEntityModel model, bool updateHistory = true)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
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
        IEnumerable<Guid> ExecuteCopyEntities<TEntityVersioned, TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, IReadOnlyList<Guid> entityIds, Guid? organizationId = null)
            where TEntityVersioned : class, IEntityIdentifier, IVersionedVolume<TEntity>, IMultilanguagedEntity<TLanguageAvail>, IOriginalEntity, IOrganizationInfo, INameReferences, new()
            where TEntity : IVersionedRoot, new()
            where TLanguageAvail : class, ILanguageAvailability;


        /// <summary>
        /// Execute restoring entities to Draft version
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityIds"></param>
        /// <param name="onAdditionalAction"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        List<Guid> ExecuteRestoreEntities<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork,
            IReadOnlyList<Guid> entityIds, Action<IUnitOfWorkWritable, Guid, TEntity> onAdditionalAction = null)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, IValidity, new()
            where TLanguageAvail : class, ILanguageAvailability;

        /// <summary>
        /// Set finalized properties to copied entity
        /// </summary>
        /// <param name="newEntity"></param>
        /// <param name="copiedModel"></param>
        /// <param name="organizationId"></param>
        /// <typeparam name="TEntityVersioned"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        void FinalizeCopyEntity<TEntityVersioned, TLanguageAvail>(TEntityVersioned newEntity, ICopyTemplate copiedModel, Guid? organizationId)
            where TEntityVersioned : class, IEntityIdentifier, IVersionedVolume, IOriginalEntity, IOrganizationInfo, INameReferences, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        /// <summary>
        /// Removes the connections between a service channel and services from other organizations.
        /// </summary>
        /// <param name="versionedIds">Collection of service channel IDs which are being published.</param>
        /// <param name="unitOfWork"></param>
        /// <param name="checkChannelStatus">Determines whether the method should check if the channel is
        /// really not common.</param>
        void RemoveNotCommonConnections(IEnumerable<Guid> versionedIds, IUnitOfWorkWritable unitOfWork, bool checkChannelStatus = true);

        /// <summary>
        ///
        /// </summary>
        /// <param name="unificRootIds"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="includeChain"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguage"></typeparam>
        /// <returns></returns>
        List<TEntity> GetNotificationEntity<TEntity, TLanguage>(IEnumerable<Guid> unificRootIds,
            IUnitOfWork unitOfWork, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain)
            where TEntity : class, IVersionedVolume, IMultilanguagedEntity<TLanguage>
            where TLanguage : LanguageAvailability;

        /// <summary>
        ///
        /// </summary>
        /// <param name="entityIds"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="includeChain"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguage"></typeparam>
        /// <returns></returns>
        List<TEntity> GetNotificationEntityCustom<TEntity, TLanguage>(IEnumerable<Guid> entityIds,
            IUnitOfWork unitOfWork, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain)
            where TEntity : class, IVersionedVolume, IMultilanguagedEntity<TLanguage>
            where TLanguage : LanguageAvailability;

        /// <summary>
        ///
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="allEntities"></param>
        /// <returns></returns>
        string GetChannelSubType(Guid entityId,
            List<ServiceChannelVersioned> allEntities);

        /// <summary>
        ///
        /// </summary>
        /// <param name="versionedGd"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        Dictionary<Guid, Dictionary<string, string>> GetEntityNames<TEntity>(List<TEntity> versionedGd)
            where TEntity : class, IVersionedVolume, INameReferences;

        /// <summary>
        /// Get names of channels by DisplayNameTypeId
        /// </summary>
        /// <param name="versionedChannels">List of ServiceChannelVersioned</param>
        /// <returns></returns>
        Dictionary<Guid, Dictionary<string, string>> GetChannelNames(List<ServiceChannelVersioned> versionedChannels);
        /// <summary>
        ///
        /// </summary>
        /// <param name="entities"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguage"></typeparam>
        /// <returns></returns>
        Dictionary<Guid, IReadOnlyList<VmLanguageAvailabilityInfo>>
            GetLanguageAvailabilites<TEntity, TLanguage>(List<TEntity> entities)
            where TEntity : class, IVersionedVolume, IMultilanguagedEntity<TLanguage>
            where TLanguage : LanguageAvailability;

        /// <summary>
        /// Get all related service unific root ids for connected service channel
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="unificRootId">service channel unific root id</param>
        /// <returns>list of service ids</returns>
        List<Guid> GetServiceChannelRelationIds(IUnitOfWork unitOfWork, Guid unificRootId);

        /// <summary>
        /// Delete all related connection for channel
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceChannelVersionedId">Service channel version ID</param>
        void DeleteServiceChannelConnections(IUnitOfWork unitOfWork, Guid serviceChannelVersionedId);

        /// <summary>
        /// Update History metadata
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="languages"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        void UpdateHistoryMetaData<TEntity, TLanguageAvail>(IEnumerable<TEntity> entities, List<TLanguageAvail> languages)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailabilityBase;

        /// <summary>
        /// Throw exception if user try archive entity with ASTI connection
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityVersionedId">Versioned ID of entity</param>
        /// <typeparam name="TEntity"></typeparam>
        void CheckArchiveAstiContract<TEntity>(IUnitOfWork unitOfWork, Guid entityVersionedId) where TEntity : class, IVersionedVolume;

        /// <summary>
        /// Explicitly sets the modified date for versioned entity and given language availabilities.
        /// Does not update the ModifiedBy field or any other fields.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="languageAvailabilities"></param>
        /// <param name="modified"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        void UpdateModifiedDates<TEntity, TLanguageAvail>(
            IReadOnlyList<TEntity> entities,
            IReadOnlyList<TLanguageAvail> languageAvailabilities,
            DateTime? modified = null)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IAuditing, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailabilityBase;

        /// <summary>
        /// Handles SocialHealthCenter for the channel
        /// </summary>
        /// <param name="vm">StringItem model</param>
        /// <param name="unitOfWork">UnitOfWork</param>
        void HandleSocialHealthCenter(IVmStringItem vm, IUnitOfWork unitOfWork);
        
        /// <summary>
        /// Remove item from TrackingEntityVersioned table by entity unific root id
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="rootId">entity unific root id</param>
        void RemoveArchiveTracking(IUnitOfWorkWritable unitOfWork, Guid rootId);

        /// <summary>
        /// Gets the default collation for ordering SQL queries.
        /// </summary>
        /// <returns></returns>
        string GetQueryCollation();

        /// <summary>
        /// Returns all unificRootIds of channels, which have ASTI connections
        /// </summary>
        /// <param name="rootIds"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        IEnumerable<Guid> GetAstiChannelIds(IList<Guid> rootIds, IUnitOfWork unitOfWork);
    }
}
