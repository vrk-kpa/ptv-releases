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

using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Models;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Database.DataAccess.Interfaces.Services.V2
{
    public interface IConnectionsService
    {
        /// <summary>
        /// Save all connections
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmConnectionsPageOutput SaveRelations(VmConnectionsPageInput model);

        /// <summary>
        /// Saves services small connections
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmConnectionsOutput SaveServiceRelations(VmConnectionsInput model);

        /// <summary>
        /// Saves service channels small connections
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmConnectionsOutput SaveServiceChannelRelations(VmConnectionsInput model);

        /// <summary>
        /// Get channel relations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        VmConnectionsOutput  GetChannelRelations(VmConnectionsInput model);
    }

    internal interface IConnectionsServiceInternal : IConnectionsService
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="unificRootIds"></param>
        /// <returns></returns>
        Dictionary<Guid, List<ServiceServiceChannel>> GetAllServiceRelations(IUnitOfWork unitOfWork, List<Guid> unificRootIds);
        /// <summary>
        ///
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="unificRootIds"></param>
        /// <returns></returns>
        Dictionary<Guid, List<ServiceServiceChannel>> GetAllServiceChannelRelations(IUnitOfWork unitOfWork, List<Guid> unificRootIds);
    }
}
