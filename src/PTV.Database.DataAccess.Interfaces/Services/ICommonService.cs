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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

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
        /// Get list of data Types specified by string name
        /// </summary>
        /// <param name="dataTypes"></param>
        /// <returns></returns>
        IVmBase GetTypedData(IEnumerable<string> dataTypes);
        /// <summary>
        ///  Get list of web phone charge types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmListItem> GetPhoneChargeTypes();
        /// <summary>
        ///  Get list of web page types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmListItem> GetWebPageTypes();
        /// <summary>
        ///  Get list of service types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmListItem> GetServiceTypes();
        /// <summary>
        /// Get list of provision types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmListItem> GetProvisionTypes();
        /// <summary>
        ///  Get list of printable form url types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmListItem> GetPrintableFormUrlTypes();
        /// <summary>
        ///  Get list of phone types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmListItem> GetPhoneTypes();
        /// <summary>
        ///  Get list of service hour types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmListItem> GetServiceHourTypes();

        /// <summary>
        ///  Get list of area information types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmListItem> GetAreaInformationTypes();

        /// <summary>
        ///  Get list of area types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmListItem> GetAreaTypes();

        /// <summary>
        /// Get all areas of specific type
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="type">Type of areas</param>
        /// <returns>List of areas</returns>
        IReadOnlyList<VmListItemReferences> GetAreas(IUnitOfWork unitOfWork, AreaTypeEnum type);

        /// <summary>
        /// Get list of publishing statuses
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmListItem> GetPublishingStatuses();

        /// <summary>
        /// Get Organizations names.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="searchText"></param>
        /// <param name="takeAll"></param>
        /// <param name="onlyUserOrganization"></param>
        /// <returns>List of Organizations names by search text.</returns>
        IReadOnlyList<VmListItem> GetOrganizationNames(IUnitOfWork unitOfWork, string searchText = null, bool takeAll = true);

        /// <summary>
        /// Get all available organizations for user.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="searchText"></param>
        /// <param name="takeAll"></param>
        /// <returns>List of Organizations names by search text.</returns>
        IReadOnlyList<VmListItem> GetUserAvailableOrganizationNames(IUnitOfWork unitOfWork, string searchText = null, bool takeAll = true);
        /// <summary>
        /// Get all available organizations for user organization.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="organizationId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        IReadOnlyList<VmListItem> GetUserAvailableOrganizationNamesForOrganization(IUnitOfWork unitOfWork, Guid? organizationId = null, Guid? parentId = null);
        /// <summary>
        ///  Get list of languages
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmListItem> GetLanguages();

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
        VmListItemsData<VmListItem> GetServiceChannelTypes();

        /// <summary>
        /// Gets all municipalities
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        IReadOnlyList<VmListItem> GetMunicipalities(IUnitOfWork unitOfWork);
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
        /// Returns all organizations in tree
        /// </summary>
        /// <param name="unitOfWork">unit of work</param>
        /// <returns>organization's tree</returns>
        List<VmTreeItem> GetOrganizations(IUnitOfWork unitOfWork);
        /// <summary>
        ///  Get list of coordinate types
        /// </summary>
        /// <returns></returns>
        VmListItemsData<VmListItem> GetCoordinateTypes();
        

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
        VmListItemsData<VmListItem> GetServiceChannelConnectionTypes();

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
        VmListItemsData<VmListItem> GetServiceFundingTypes();

        /// <summary>
        /// Saves the environment instructions.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>saved informations</returns>
        VmEnvironmentInstructionsOut SaveEnvironmentInstructions(VmEnvironmentInstructionsIn model);

        /// <summary>
        /// Gets the environment instructions.
        /// </summary>
        /// <returns></returns>
        VmEnvironmentInstructionsOut GetEnvironmentInstructions();

        /// <summary>
        /// Gets all the user organizations including the sub organizations.
        /// </summary>
        /// <param name="userOrganization"></param>
        /// <returns></returns>
        List<Guid> GetUserOrganizations(Guid userOrganization);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="date"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IVmOpenApiGuidPageVersionBase GetServcesAndChannelsByOrganization(Guid organizationId, DateTime? date, int pageNumber, int pageSize);
    }

    internal interface ICommonServiceInternal : ICommonService
    {
        PublishingResult PublishEntity<TEntity, TLanguageAvail>(VmPublishingModel model) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailability;
        PublishingResult PublishEntity<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork, VmPublishingModel model) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailability;
        PublishingResult PublishAllAvailableLanguageVersions<TEntity, TLanguageAvail>(Guid Id, Expression<Func<TLanguageAvail, bool>> getSelectedIdFunc) where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new() where TLanguageAvail : class, ILanguageAvailability;
        IList<PublishingAffectedResult> RestoreArchivedEntity<TEntity>(IUnitOfWorkWritable unitOfWork, Guid versionId) where TEntity : class, IEntityIdentifier, IVersionedVolume, new();

        VmPublishingResultModel WithdrawEntity<TEntity>(Guid entityVersionedId) where TEntity : class, IEntityIdentifier, IVersionedVolume;
        VmPublishingResultModel WithdrawEntityByRootId<TEntity>(Guid rootId) where TEntity : class, IEntityIdentifier, IVersionedVolume, IPublishingStatus;

        VmPublishingResultModel RestoreEntity<TEntity>(Guid entityVersionedId, Func<IUnitOfWork, TEntity, bool> additionalCheckAction = null) where TEntity : class, IEntityIdentifier, IVersionedVolume;
    }
}
