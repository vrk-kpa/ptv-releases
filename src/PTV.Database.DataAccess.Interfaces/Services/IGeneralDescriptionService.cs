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
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using VmGeneralDescription = PTV.Domain.Model.Models.VmGeneralDescription;
using VmGeneralDescriptions = PTV.Domain.Model.Models.VmGeneralDescriptions;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IGeneralDescriptionService
    {
        VmGeneralDescriptionSearchForm GetGeneralDescriptionSearchForm();
        VmTargetGroups GetSubTargetGroups(Guid targetGroupId);
        VmGeneralDescriptions SearchGeneralDescriptions(VmGeneralDescriptionSearchForm searchData);

        /// <summary>
        /// Searches the general descriptions v2.
        /// </summary>
        /// <param name="searchData">The search data.</param>
        /// <returns>Searched general descriptions for given model</returns>
        Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions SearchGeneralDescriptions_V2(VmGeneralDescriptionSearchForm searchData);
        VmGeneralDescription GetGeneralDescriptionById(Guid id);
        Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionDialog GetGeneralDescription_V2(VmGeneralDescriptionIn model);
        IVmOpenApiGuidPageVersionBase V3GetGeneralDescriptions(DateTime? date, int pageNumber = 1, int pageSize = 1000);
        IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionVersionBase(Guid id, int openApiVersion, bool getOnlyPublished = true);
        bool GeneralDescriptionExists(Guid id);

        IVmOpenApiGeneralDescriptionVersionBase AddGeneralDescription(IVmOpenApiGeneralDescriptionInVersionBase vm, bool allowAnonymous, int openApiVersion);
        IVmOpenApiGeneralDescriptionVersionBase SaveGeneralDescription(IVmOpenApiGeneralDescriptionInVersionBase vm, int openApiVersion);

        /// <summary>
        /// Add general description to DB 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IVmEntityBase AddGeneralDescription(VmGeneralDescription model);
        /// <summary>
        /// Get general description by model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmGeneralDescription GetGeneralDescription(VmGeneralDescriptionIn model);
        /// <summary>
        /// Update general description values.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmGeneralDescription UpdateGeneralDescription(VmGeneralDescription model);        
        /// <summary>
        /// Publish general descrition 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmPublishingResultModel PublishGeneralDescription(VmPublishingModel model);
        /// <summary>
        /// Withdraw general description
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmPublishingResultModel WithdrawGeneralDescription(VmPublishingModel model);
        /// <summary>
        /// Restore general description
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmPublishingResultModel RestoreGeneralDescription(VmPublishingModel model);
        /// <summary>
        /// Delete general description
        /// </summary>
        /// <param name="entityId">id of general description to delete</param>
        /// <returns>base entity containing entityId and publishing status</returns>
        IVmEntityBase DeleteGeneralDescription(Guid? entityId);
        /// <summary>
        /// Get general description status
        /// </summary>
        /// <param name="entityId">id of general description</param>
        /// <returns>base entity containing publishing status</returns>
        IVmEntityBase GetGeneralDescriptionStatus(Guid? entityId);
        /// <summary>
        /// Lock general description by Id
        /// </summary>
        /// <param name="id">General description Id</param>
        /// <returns></returns>
        IVmEntityBase LockGeneralDescription(Guid id);
        /// <summary>
        /// UnLock general description by Id
        /// </summary>
        /// <param name="id">General description Id</param>
        /// <returns></returns>
        IVmEntityBase UnLockGeneralDescription(Guid id);
        /// <summary>
        /// Throw exception if general description is locked
        /// </summary>
        /// <param name="id">General description Id</param>
        /// <returns></returns>
        IVmEntityBase IsGeneralDescriptionLocked(Guid id);
        /// <summary>
        /// Gets the general descrition names.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        VmEntityNames GetGeneralDescriptionNames(VmEntityBase model);

        /// <summary>
        /// Get information if general descrition is editable
        /// </summary>
        /// <param name="id">General descrition Id</param>
        /// <returns></returns>
        IVmEntityBase IsGeneralDescriptionEditable(Guid id);

    }
}
