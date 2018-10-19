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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Domain.Model.Models.V2.Service;
using System.Collections.Generic;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.V2.Connections;
using PTV.Domain.Model.Models.V2.TranslationOrder;

namespace PTV.Database.DataAccess.Interfaces.Services.V2
{
    public interface IServiceService
    {
        /// <summary>
        /// Get model for service 
        /// </summary>
        /// <param name="model">input model</param>
        /// <returns></returns>
        VmServiceOutput GetService(VmServiceBasic model);
        /// <summary>
        /// Saves service 
        /// </summary>
        /// <param name="model">input model</param>
        /// <returns>service</returns>
        VmServiceOutput SaveService(VmServiceInput model);
        /// <summary>
        /// Gets the service header.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <returns></returns>
        VmServiceHeader GetServiceHeader(Guid? serviceId);

        /// <summary>
        /// Save new or existing electronic channel to repository
        /// </summary>
        /// <param name="model">electronic channel model</param>
        /// <returns></returns>

        /// <summary>
        ///Publish service 
        /// </summary>
        /// <param name="model">get model</param>
        /// <returns></returns>
        VmEntityHeaderBase PublishService(IVmLocalizedEntityModel model);

        /// <summary>
        /// Schedule publishing od archiving service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmEntityHeaderBase ScheduleService(IVmLocalizedEntityModel model);
        
        /// <summary>
        /// Save and validate service.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmServiceOutput SaveAndValidateService(VmServiceInput model);
        /// <summary>
        /// Lock service by Id
        /// </summary>
        /// <param name="id">Service Id</param>
        /// <param name="isLockDisAllowedForArchived">indicates Whether lock is allowed for archived service</param>
        /// <returns></returns>
        IVmEntityBase LockService(Guid id, bool isLockDisAllowedForArchived = false);
        /// <summary>
        /// UnLock service by Id
        /// </summary>
        /// <param name="id">Service Id</param>
        /// <returns></returns>
        IVmEntityBase UnLockService(Guid id);
        /// <summary>
        /// ArchiveService
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        VmServiceHeader DeleteService(Guid serviceId);
        /// <summary>
        /// Archive Language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmServiceHeader ArchiveLanguage(VmEntityBasic model);
        /// <summary>
        /// Restore Language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmServiceHeader RestoreLanguage(VmEntityBasic model);
        /// <summary>
        /// Withdraw Language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmServiceHeader WithdrawLanguage(VmEntityBasic model);
        /// <summary>
        /// Withdraw service.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        VmServiceHeader WithdrawService(Guid serviceId);
        /// <summary>
        /// Restore of service.
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        VmServiceHeader RestoreService(Guid serviceId);
        /// <summary>
        /// Get validated Entity
        /// </summary>
        /// <param name="model"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        VmServiceHeader GetValidatedEntity(VmEntityBasic model);

        /// <summary>
        /// Get connectable services
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        VmConnectableServiceSearchResult GetConnectableService(VmConnectableServiceSearch search);

        /// <summary>
        /// Get connections page services search result
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        VmConnectionsServiceSearchResult GetConnectionsService(VmConnectionsServiceSearch search);

        /// <summary>
        /// Save connections
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmConnectionsOutput SaveRelations(VmConnectionsInput model);

        /// <summary>
        /// Searching for keywords based on localization
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmKeywordSearchOutput KeywordSearch(VmKeywordSearch model);

        /// <summary>
        /// Create XLIFF
        /// </summary>
        /// <param name="model"></param>
        VmServiceTranslationResult GenerateXliff(VmServiceTranslation model);
        /// <summary>
        /// Check service by Id if is connectable
        /// </summary>
        /// <param name="id">Service Id</param>
        /// <returns></returns>
        IVmEntityBase IsConnectable(Guid id);

        /// <summary>
        /// Send service entity to translation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateSaveOutputs SendServiceEntityToTranslation(VmTranslationOrderInput model);

        /// <summary>
        /// Get service translation data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmTranslationOrderStateOutputs GetServiceTranslationData(VmTranslationDataInput model);     
    }
}   