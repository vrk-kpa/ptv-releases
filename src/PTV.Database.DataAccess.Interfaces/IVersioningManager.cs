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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Interfaces
{
    /// <summary>
    /// Manager handling versions of entity which uses IVersioned interface
    /// </summary>
    internal interface IVersioningManager
    {
        /// <summary>
        /// Assign proper version to entity, i.e. like draft for new one and modified for already existing, new publishing for publishing. Clonging performed automatically if updating published version
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be updated</typeparam>
        /// <param name="unitOfWork">Unit Of Work</param>
        /// <param name="entity">Entity which will be updated</param>
        /// <param name="targetStatus">target status of cloned entity</param>
        /// <returns></returns>
        TEntityType CreateEntityVersion<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity, VersioningMode versioningMode = VersioningMode.Standard, PublishingStatus? targetStatus = null) where TEntityType : class, IVersionedVolume, new();

        /// <summary>
        /// Get and return all available versions of entity
        /// </summary>
        /// <typeparam name="TEntityType"></typeparam>
        /// <param name="unitOfWork">Unit Of Work must be provided for correct functionality</param>
        /// <param name="entity">Entity which versions will be retrieved</param>
        /// <returns>List of available versions of specified entity</returns>
        List<VersionInfo> GetAllVersions<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity) where TEntityType : class, IVersionedVolume;

        /// <summary>
        /// Get and return all available versions of entity
        /// </summary>
        /// <typeparam name="TEntityType"></typeparam>
        /// <param name="unitOfWork">Unit Of Work must be provided for correct functionality</param>
        /// <param name="unificRootId">Entity RootId which versions will be retrieved</param>
        /// <returns>List of available versions of specified entity</returns>
        List<VersionInfo> GetAllVersions<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid unificRootId) where TEntityType : class, IVersionedVolume;

        /// <summary>
        /// Publish specified entity, check latest version and create new version with published state
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be promoted to published state</typeparam>
        /// <param name="unitOfWork">Unit Of Work</param>
        /// <param name="entity">Entity which will be promoted to published state</param>
        /// <param name="targetPublishingStatus">Entity which will be promoted to published state</param>
        IList<PublishingAffectedResult> PublishVersion<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity, PublishingStatus targetPublishingStatus = PublishingStatus.Published) where TEntityType : class, IVersionedVolume, new();

        /// <summary>
        /// Withdraw entity from published state, entity's status is changed from Published to Modifed
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be withdrawed</typeparam>
        /// <param name="unitOfWork">Unit Of Work</param>
        /// <param name="entity">Entity which will be withdrawed</param>
        ///  <param name="onlyAllowedSourceState">List of allowed source statuses</param>
        /// <returns></returns>
        IList<PublishingAffectedResult> ChangeToModified<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity, List<PublishingStatus> onlyAllowedSourceState = null) where TEntityType : class, IVersionedVolume;

        /// <summary>
        /// Change publishing status of language version of specific entity which is of IMultilanguagedEntity type
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <typeparam name="TLang">Type of langauge version relation</typeparam>
        /// <param name="unitOfWork">Unit of work instance</param>
        /// <param name="entity">Entity to change</param>
        /// <param name="publishingStatusTo">Target publishing status of language version</param>
        /// <param name="publishingStatusFrom">Input criteria for selecting the language versions which will be switched</param>
        /// <param name="languageGuids">Input criteria for selecting the language versions which will be switched</param>
        void ChangeStatusOfLanguageVersion<T, TLang>(ITranslationUnitOfWork unitOfWork, T entity, PublishingStatus publishingStatusTo,
            IEnumerable<PublishingStatus> publishingStatusFrom = null, IEnumerable<Guid> languageGuids = null) where T : class, IMultilanguagedEntity<TLang>, new()
            where TLang : class, ILanguageAvailability;

        /// <summary>
        /// Apply filter for publishinf statuses to filter out entity by its status. Get Draft and Published only.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="query">Query on which the filter will be applied</param>
        /// <returns></returns>
        IQueryable<TEntityType> ApplyPublishingStatusFilter<TEntityType>(IQueryable<TEntityType> query) where TEntityType : IVersionedVolume;

        /// <summary>
        /// Apply filter for publishinf statuses to filter out entity by its status. Get Draft and Published only.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="query">Query on which the filter will be applied</param>
        /// <returns></returns>
        IEnumerable<TEntityType> ApplyPublishingStatusFilter<TEntityType>(IEnumerable<TEntityType> query) where TEntityType : IVersionedVolume;

        /// <summary>
        /// Apply filter for publishing statuses to filter out entity by its status order by priority fallback. Get Published, Draft or modified version.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="query">Query on which the filter will be applied</param>
        /// <returns></returns>
        IQueryable<TEntityType> ApplyPublishingStatusOrderByPriorityFallback<TEntityType>(IQueryable<TEntityType> query) where TEntityType : IVersionedVolume;
        
        /// <summary>
        /// Get versioned entity with specific publishing status.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="unitOfWork">Instance of unit of work</param>
        /// <param name="entity">Instance of entity which is used as input for searching specific version</param>
        /// <param name="publishingStatus">Searching criteria for publishing status</param>
        /// <returns></returns>
        TEntityType GetSpecificVersion<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity, PublishingStatus publishingStatus)
            where TEntityType : class, IVersionedVolume, new();

        /// <summary>
        /// Get versioned entity with specific publishing status.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="unitOfWork">Instance of unit of work</param>
        /// <param name="rootId">ID of unific root which is used as input for searching its specific version</param>
        /// <param name="publishingStatus">Searching criteria for publishing status</param>
        /// <returns></returns>
        TEntityType GetSpecificVersionByRoot<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid rootId, PublishingStatus publishingStatus)
            where TEntityType : class, IVersionedVolume, new();

        /// <summary>
        ///Get entity from collection of entities which is Published or Draft
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="entities">Collection of entities which will filtered</param>
        /// <returns></returns>
        TEntityType GetNotModifiedVersion<TEntityType>(IEnumerable<TEntityType> entities) where TEntityType : class, IVersionedVolume, new();


       bool FilterByPublishingStatus<TEntityType>(TEntityType entity) where TEntityType : IVersionedVolume;


        /// <summary>
        /// Apply publishing status filter with fallback, i.e. try to get instance in Published state, if does not exist, then take Draft one, then Modified one.
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
        /// <param name="collection">Collection of entities on which the fallback filter will be applied</param>
        /// <returns></returns>
        TEntityType ApplyPublishingStatusFilterFallback<TEntityType>(IEnumerable<TEntityType> collection) where TEntityType : class, IVersionedVolume;

//        /// <summary>
//        /// Apply publishing status filter with fallback, i.e. try to get instance in Published state, if does not exist, then take Draft one, then Modified one.
//        /// </summary>
//        /// <typeparam name="TEntityType">Type of entity which will be filtered</typeparam>
//        /// <param name="collection">Collection of entities on which the fallback filter will be applied</param>
//        /// <returns></returns>
//        TEntityType ApplyPublishingStatusFilterFallback<TEntityType>(IQueryable<TEntityType> collection) where TEntityType : class, IVersionedVolume;

        /// <summary>
        /// Ensure that root entity is properly created for versioned entity. If not, then new root is created.
        /// </summary>
        /// <typeparam name="TRootType">Type of unific root related to versioned entity</typeparam>
        /// <param name="entity">Instance of entity which should be checked</param>
        void EnsureUnificRoot<TRootType>(IVersionedVolume<TRootType> entity) where TRootType : IVersionedRoot, new();

        /// <summary>
        /// Acquire UnificRootId from versioned entity by its ID
        /// </summary>
        /// <typeparam name="T">Type of versioned entity</typeparam>
        /// <param name="unitOfWork">Instance of unit of work</param>
        /// <param name="versionedEntityId">ID of versioned entity</param>
        /// <returns></returns>
        Guid? GetUnificRootId<T>(ITranslationUnitOfWork unitOfWork, Guid? versionedEntityId) where T : class, IVersionedVolume;

        Guid? GetVersionId<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid rootId, PublishingStatus? publishingStatus = null, bool ignoreDeleted = true) where TEntityType : class, IVersionedVolume;

        PublishingStatus? GetLatestVersionPublishingStatus<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid rootId) where TEntityType : class, IVersionedVolume;

        /// <summary>
        /// Get last publish version of entity
        /// </summary>
        /// <typeparam name="TEntityType"></typeparam>
        /// <param name="unitOfWork">Unit Of Work must be provided for correct functionality</param>
        /// <param name="unificRootId">Entity unificrootId which version will be retrieved</param>
        /// <returns>Available publish version of specified entity</returns>
        VersionInfo GetLastPublishedVersion<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid unificRootId) where TEntityType : class, IVersionedVolume;
        /// <summary>
        /// Get last modified version of entity
        /// </summary>
        /// <typeparam name="TEntityType"></typeparam>
        /// <param name="unitOfWork">Unit Of Work must be provided for correct functionality</param>
        /// <param name="unificRootId">Entity unificrootId which version will be retrieved</param>
        /// <returns>Available modified version of specified entity</returns>
        VersionInfo GetLastModifiedVersion<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid unificRootId) where TEntityType : class, IVersionedVolume;
        bool IsAllowedForEditing<TEntityType>(ITranslationUnitOfWork unitOfWork, TEntityType entity) where TEntityType : class, IVersionedVolume;
        bool IsAllowedForEditing<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid entityId) where TEntityType : class, IVersionedVolume;
        bool IsEntityArchived<TEntityType>(ITranslationUnitOfWork unitOfWork, Guid entityId) where TEntityType : class, IVersionedVolume;

        TEntityType ApplyLanguageFilterFallback<TEntityType>(IEnumerable<TEntityType> collection, Guid? requestedLanguage) where TEntityType : class, IName;

        /// <summary>
        /// Change publishing status of language version of specific entity which is of IMultilanguagedEntity type
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <typeparam name="TLang">Type of langauge version relation</typeparam>
        /// <param name="unitOfWork">Unit of work instance</param>
        /// <param name="entity">Entity to change</param>
        /// <param name="languageAvailabilities">languages to change</param>
        void ChangeStatusOfLanguageVersion<T, TLang>(ITranslationUnitOfWork unitOfWork, T entity, IEnumerable<VmLanguageAvailabilityInfo> languageAvailabilities) where T : class, IMultilanguagedEntity<TLang>, new() where TLang : class, ILanguageAvailability;
        /// <summary>
        /// Change publishing status of language version of specific entity which is of IMultilanguagedEntity type
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <typeparam name="TLang">Type of langauge version relation</typeparam>
        /// <param name="unitOfWork">Unit of work instance</param>
        /// <param name="entity">Entity to change</param>
        /// <param name="statusId">status id for all languages</param>
        void ChangeStatusOfLanguageVersion<T, TLang>(ITranslationUnitOfWork unitOfWork, T entity, Guid statusId) where T : class, IMultilanguagedEntity<TLang>, new() where TLang : class, ILanguageAvailability;
        /// <summary>
        /// Change publishing status of language version of specific entity which is of IMultilanguagedEntity type
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <typeparam name="TLang">Type of langauge version relation</typeparam>
        /// <param name="unitOfWork">Unit of work instance</param>
        /// <param name="entityId">Entity to change</param>
        /// <param name="languageAvailabilities">languages to change</param>
        void ChangeStatusOfLanguageVersion<T, TLang>(ITranslationUnitOfWork unitOfWork, Guid entityId, IEnumerable<VmLanguageAvailabilityInfo> languageAvailabilities) where T : class, IEntityIdentifier, IMultilanguagedEntity<TLang>, new() where TLang : class, ILanguageAvailability;

        bool IsInAllowedPublishingStatus(Guid publishingStatus);
    }
}