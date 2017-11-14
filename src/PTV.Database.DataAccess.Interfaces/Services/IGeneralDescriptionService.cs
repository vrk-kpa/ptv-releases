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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using VmGeneralDescription = PTV.Domain.Model.Models.VmGeneralDescription;
using VmGeneralDescriptions = PTV.Domain.Model.Models.VmGeneralDescriptions;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IGeneralDescriptionService
    {
        VmGeneralDescriptionSearchForm GetGeneralDescriptionSearchForm();
        VmTargetGroups GetSubTargetGroups(Guid targetGroupId);
        Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions SearchGeneralDescriptions(VmGeneralDescriptionSearchForm searchData);

        /// <summary>
        /// Searches the general descriptions v2.
        /// </summary>
        /// <param name="searchData">The search data.</param>
        /// <returns>Searched general descriptions for given model</returns>
        //Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions SearchGeneralDescriptions_V2(VmGeneralDescriptionSearchForm searchData);
        VmGeneralDescription GetGeneralDescriptionById(Guid id);
        Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionOutput GetGeneralDescription(VmGeneralDescriptionGet model);
        Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionOutput GetGeneralDescription(VmGeneralDescriptionGet model, IUnitOfWork unitOfWork);
        IVmOpenApiGuidPageVersionBase GetGeneralDescriptions(DateTime? date, int pageNumber = 1, int pageSize = 1000);
        IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionVersionBase(Guid id, int openApiVersion, bool getOnlyPublished = true);
        IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionSimple(IUnitOfWork unitOfWork, Guid id);
        bool GeneralDescriptionExists(Guid id);

        IVmOpenApiGeneralDescriptionVersionBase AddGeneralDescription(IVmOpenApiGeneralDescriptionInVersionBase vm, bool allowAnonymous, int openApiVersion);
        IVmOpenApiGeneralDescriptionVersionBase SaveGeneralDescription(IVmOpenApiGeneralDescriptionInVersionBase vm, int openApiVersion);
        /// <summary>
        /// Saves service
        /// </summary>
        /// <param name="model">input model</param>
        /// <returns>service</returns>
        VmGeneralDescriptionOutput SaveGeneralDescription(VmGeneralDescriptionInput model);
        /// <summary>
        /// Gets the general description header.
        /// </summary>
        /// <param name="generalDescriptionId">The general description identifier.</param>
        /// <returns></returns>
        VmGeneralDescriptionHeader GetGeneralDescriptionHeader(Guid? generalDescriptionId);
        /// <summary>
        /// Gets the general description header.
        /// </summary>
        /// <param name="generalDescriptionId">The general description identifier.</param>
        /// <param name="unitOfWork">Unit of work.</param>
        /// <returns></returns>
        VmGeneralDescriptionHeader GetGeneralDescriptionHeader(Guid? generalDescriptionId, IUnitOfWork unitOfWork);
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
        //VmGeneralDescription GetGeneralDescription(VmGeneralDescriptionGet model);
        /// <summary>
        /// Update general description values.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //VmGeneralDescription UpdateGeneralDescription(VmGeneralDescription model);
        /// <summary>
        /// Publish general descrition
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmEntityHeaderBase PublishGeneralDescription(IVmPublishingModel model);
        /// <summary>
        /// Withdraw general description
        /// </summary>
        /// <param name="generalDescriptionId">id</param>
        /// <returns></returns>
        VmGeneralDescriptionHeader WithdrawGeneralDescription(Guid generalDescriptionId);
        /// <summary>
        /// Restore general description
        /// </summary>
        /// <param name="generalDescriptionId">id</param>
        /// <returns></returns>
        VmGeneralDescriptionHeader RestoreGeneralDescription(Guid generalDescriptionId);
        /// <summary>
        /// Archive language of GD
        /// </summary>
        /// <param name="model">model with id and language id</param>
        /// <returns></returns>
        VmGeneralDescriptionHeader ArchiveLanguage(VmEntityBasic model);
        /// <summary>
        /// Restore language of GD
        /// </summary>
        /// <param name="model">model with id and language id</param>
        /// <returns></returns>
        VmGeneralDescriptionHeader RestoreLanguage(VmEntityBasic model);
        /// <summary>
        /// Withdraw language of GD
        /// </summary>
        /// <param name="model">model with id and language id</param>
        /// <returns></returns>
        VmGeneralDescriptionHeader WithdrawLanguage(VmEntityBasic model);
        /// <summary>
        /// Delete general description
        /// </summary>
        /// <param name="entityId">id of general description to delete</param>
        /// <returns>base entity containing entityId and publishing status</returns>
        VmGeneralDescriptionHeader DeleteGeneralDescription(Guid? entityId);
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
        /// <param name="isLockDisAllowedForArchived">indicates whether archived GD can be locked or not</param>
        /// <returns></returns>
        IVmEntityBase LockGeneralDescription(Guid id, bool isLockDisAllowedForArchived = false);
        /// <summary>
        /// UnLock general description by Id
        /// </summary>
        /// <param name="id">General description Id</param>
        /// <returns></returns>
        IVmEntityBase UnLockGeneralDescription(Guid id);
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
        /// <summary>
        /// Get validated Entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmGeneralDescriptionHeader GetValidatedEntity(VmEntityBasic model);
        /// <summary>
        /// Save and validate service.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmGeneralDescriptionOutput SaveAndValidateGeneralDescription(VmGeneralDescriptionInput model);
        /// <summary>
        /// Save connections
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmConnectionsOutput SaveRelations(VmConnectionsInput model);
    }
}
