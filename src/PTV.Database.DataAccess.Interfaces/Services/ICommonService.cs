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
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface ICommonService
    {
        List<VmSelectableItem> GetPhoneChargeTypes(IUnitOfWork unitOfWork);
        /// <summary>
        ///  Get list of web page types
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        List<VmListItem> GetWebPageTypes(IUnitOfWork unitOfWork);
        /// <summary>
        ///  Get list of service types
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        List<VmSelectableItem> GetServiceTypes(IUnitOfWork unitOfWork);
        /// <summary>
        /// Get list of provision types
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        List<VmListItem> GetProvisionTypes(IUnitOfWork unitOfWork);
        /// <summary>
        /// Get list of service coverage types
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        List<VmListItem> GetServiceCoverageTypes(IUnitOfWork unitOfWork);
        /// <summary>
        ///  Get list of printable form url types
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        List<VmListItem> GetPrintableFormUrlTypes(IUnitOfWork unitOfWork);
        /// <summary>
        ///  Get list of phone types
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        List<VmSelectableItem> GetPhoneTypes(IUnitOfWork unitOfWork);
        /// <summary>
        ///  Get list of service hour types
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        List<VmSelectableItem> GetServiceHourTypes(IUnitOfWork unitOfWork);
        /// <summary>
        ///  Get list of organization types
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        List<VmListItem> GetOrganizationTypes(IUnitOfWork unitOfWork);
        /// <summary>
        ///  Get list of publishing statuses
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        List<VmPublishingStatus> GetPublishingStatuses(IUnitOfWork unitOfWork);
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
        /// <returns>List of Organizations names by search text.</returns>
        IReadOnlyList<VmListItem> GetOrganizationNames(IUnitOfWork unitOfWork, string searchText = null, bool takeAll = true);
        /// <summary>
        ///  Get list of languages
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        IReadOnlyList<VmListItem> GetLanguages(IUnitOfWork unitOfWork);

        /// <summary>
        ///  Get language code
        /// </summary>
        /// <param name="languageId"></param>
        /// <returns></returns>
        string GetLozalizadion(Guid? languageId);

        /// <summary>
        ///  Get list of channelTypes
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        List<VmSelectableItem> GetServiceChannelTypes(IUnitOfWork unitOfWork);

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
        /// <param name="unitOfWork">unit of work</param>
        /// <returns>supported translation languages</returns>
        IReadOnlyList<VmListItem> GetTranslationLanguages(IUnitOfWork unitOfWork);
        /// <summary>
        /// Returns all organizations in tree
        /// </summary>
        /// <param name="unitOfWork">unit of work</param>
        /// <returns>organization's tree</returns>
        List<VmTreeItem> GetOrganizations(IUnitOfWork unitOfWork);

        void MapUserToOrganization(Guid userId, string userName, Guid? organizationGuid);
        List<SelectListItem> GetOrganizationByUser(string userName);

        bool IsUserAssignedToOrganization(string userName);

        List<Guid> GetCoUsersOfUser(string userName);

        Guid GetDraftStatusId();
    }
}
