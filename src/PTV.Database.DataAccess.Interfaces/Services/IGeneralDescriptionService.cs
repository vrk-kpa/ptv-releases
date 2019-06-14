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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.Interfaces;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IGeneralDescriptionService
    {
        Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions SearchGeneralDescriptions(VmGeneralDescriptionSearchForm searchData);       
        Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionOutput GetGeneralDescription(VmGeneralDescriptionGet model);
        VmGeneralDescriptionOutput GetGeneralDescription(IUnitOfWork unitOfWork, VmGeneralDescriptionGet model);
        IVmOpenApiGuidPageVersionBase<VmOpenApiItem> GetGeneralDescriptions(DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null);
        IVmOpenApiGuidPageVersionBase<VmOpenApiItem> GetNewGeneralDescriptions(int pageNumber = 1, int pageSize = 1000);
        IList<VmOpenApiGeneralDescriptionVersionBase> GetGeneralDescriptionsSimple(List<Guid> idList);
        IList<IVmOpenApiGeneralDescriptionVersionBase> GetGeneralDescriptions(List<Guid> idList, int openApiVersion);
        IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionVersionBase(Guid id, int openApiVersion, bool getOnlyPublished = true, bool checkRestrictions = false);
        IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionSimple(IUnitOfWork unitOfWork, Guid id);
        IVmOpenApiGeneralDescriptionVersionBase GetPublishedGeneralDescriptionWithDetails(IUnitOfWork unitOfWork, Guid id);
        IList<VmOpenApiGeneralDescriptionVersionBase> GetPublishedGeneralDescriptionsWithDetails(List<Guid> idList);
        bool GeneralDescriptionExists(Guid id);

        IVmOpenApiGeneralDescriptionVersionBase AddGeneralDescription(IVmOpenApiGeneralDescriptionInVersionBase vm, int openApiVersion);
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
        VmEntityHeaderBase PublishGeneralDescription(IVmLocalizedEntityModel model);
        /// <summary>
        /// Schedule publishing od archiving general description
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmEntityHeaderBase ScheduleGeneralDescription(IVmLocalizedEntityModel model);
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
        VmGeneralDescriptionHeader DeleteGeneralDescription(Guid entityId);
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
        /// Get validated Entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmGeneralDescriptionHeader GetValidatedEntity(VmEntityBasic model);
        /// <summary>
        /// Save connections
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmServiceConnectionsOutput SaveRelations(VmConnectionsInput model);
        /// <summary>
        /// Check gd by Id if is connectable
        /// </summary>
        /// <param name="id">gd Id</param>
        /// <returns></returns>
        IVmEntityBase IsConnectable(Guid id);
        /// <summary>
        /// Send general description entity to translation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateSaveOutputs SendGeneralDescriptionEntityToTranslation(VmTranslationOrderInput model);
        /// <summary>
        /// Get geenral description translation data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateOutputs GetGeneralDescriptionTranslationData(VmTranslationDataInput model);

        VmGeneralDescriptionHeader RemoveGeneralDescription(Guid entityId);
    }

    internal interface IGeneralDescriptionServiceInternal
    {
        /// <summary>
        /// Remove all usages of general description in services
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="entityId"></param>
        void OnDeletingGeneralDescription(IUnitOfWorkWritable unitOfWork, Guid entityId);

    }
}
