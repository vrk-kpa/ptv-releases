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
using PTV.Domain.Model.Models.V2.ServiceCollection;
using PTV.Domain.Model.Models.V2.Service;
using System.Collections.Generic;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Connections;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Database.DataAccess.Interfaces.Services.V2
{
    public interface IServiceCollectionService
    {
        /// <summary>
        /// Get model for serviceCollection
        /// </summary>
        /// <param name="model">input model</param>
        /// <returns></returns>
        VmServiceCollectionOutput GetServiceCollection(VmServiceCollectionBasic model);
        /// <summary>
        /// Saves serviceCollection 
        /// </summary>
        /// <param name="model">input model</param>
        /// <returns>serviceCollection</returns>
        VmServiceCollectionOutput SaveServiceCollection(VmServiceCollectionBase model);
        /// <summary>
        /// Gets the serviceCollection header.
        /// </summary>
        /// <param name="serviceId">The serviceCollection identifier.</param>
        /// <returns></returns>
        VmServiceCollectionHeader GetServiceCollectionHeader(Guid? serviceId);

        /// <summary>
        ///Publish serviceCollection 
        /// </summary>
        /// <param name="model">get model</param>
        /// <returns></returns>
        VmEntityHeaderBase PublishServiceCollection(IVmLocalizedEntityModel model);
        /// <summary>
        /// Schedule publishing od archiving serviceCollection
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmEntityHeaderBase ScheduleServiceCollection(IVmLocalizedEntityModel model);
        /// <summary>
        /// Lock serviceCollection by Id
        /// </summary>
        /// <param name="id">serviceCollection Id</param>
        /// <param name="isLockDisAllowedForArchived">indicates Whether lock is allowed for archived serviceCollection</param>
        /// <returns></returns>
        IVmEntityBase LockServiceCollection(Guid id, bool isLockDisAllowedForArchived = false);
        /// <summary>
        /// UnLock serviceCollection by Id
        /// </summary>
        /// <param name="id">serviceCollection Id</param>
        /// <returns></returns>
        IVmEntityBase UnLockServiceCollection(Guid id);
        /// <summary>
        /// Archive serviceCollection
        /// </summary>
        /// <param name="serviceCollectionId"></param>
        /// <returns></returns>
        VmServiceCollectionHeader DeleteServiceCollection(Guid serviceCollectionId);
        /// <summary>
        /// Archive Language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmServiceCollectionHeader ArchiveLanguage(VmEntityBasic model);
        /// <summary>
        /// Restore Language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmServiceCollectionHeader RestoreLanguage(VmEntityBasic model);
        /// <summary>
        /// Withdraw Language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmServiceCollectionHeader WithdrawLanguage(VmEntityBasic model);
        /// <summary>
        /// Withdraw serviceCollection.
        /// </summary>
        /// <param name="serviceCollectionId"></param>
        /// <returns></returns>
        VmServiceCollectionHeader WithdrawServiceCollection(Guid serviceCollectionId);
        /// <summary>
        /// Restore of serviceCollection.
        /// </summary>
        /// <param name="serviceCollectionId"></param>
        /// <returns></returns>
        VmServiceCollectionHeader RestoreServiceCollection(Guid serviceCollectionId);
        /// <summary>
        /// Get validated Entity
        /// </summary>
        /// <param name="model"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        VmServiceCollectionHeader GetValidatedEntity(VmEntityBasic model);

        /// <summary>
        /// Save connections
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmServiceCollectionConnectionsOutput SaveRelations(VmConnectionsInput model);

        /// <summary>
        /// Check serviceCollection by Id if is connectable
        /// </summary>
        /// <param name="id">serviceCollection Id</param>
        /// <returns></returns>
        IVmEntityBase IsConnectable(Guid id);        

    }
    internal interface IServiceCollectionServiceInternal : IServiceCollectionService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceUnificRootId"></param>
        /// <returns></returns>
        List<VmServiceCollectionConnectionOutput> GetAllServiceRelations(IUnitOfWork unitOfWork,
            Guid serviceUnificRootId);
    }
}   