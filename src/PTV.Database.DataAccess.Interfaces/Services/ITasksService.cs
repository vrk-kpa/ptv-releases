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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework.Logging;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface ITasksService
    {
        /// <summary>
        /// Get numbers for all tasks
        /// </summary>
        /// <returns></returns>
        IVmListItemsData<VmTasksBase> GetTasksNumbers(IList<Guid> forOrganizations);

        /// <summary>
        /// Get numbers for all tasks
        /// </summary>
        /// <returns></returns>
        IVmListItemsData<VmTasksBase> GetTasksNumbers();

        /// <summary>
        /// Get entities for the tasks
        /// </summary>
        /// <returns></returns>
        IVmSearchBase GetTasksEntities(VmTasksSearch model);

        /// <summary>
        /// Get broken link content
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IVmSearchBase GetBrokenLinks(VmBrokenLinkContentSearch model);

        /// <summary>
        /// Archive entity by expiration date
        /// </summary>
        void ArchiveEntityByExpirationDate(VmJobLogEntry logInfo);

        /// <summary>
        /// Get tasks for open api.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="entityStatus"></param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<VmOpenApiExpiringTask> GetExpiringTasks(TasksIdsEnum taskId, int pageNumber, int pageSize);

        /// <summary>
        /// Get services that do not have connection to any service channel.
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<VmOpenApiTask> GetOrphanItemsTasks(TasksIdsEnum taskId, int pageNumber, int pageSize);
        
        /// <summary>
        /// Get not updated tasks of service and channels
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IVmOpenApiModelWithPagingBase<VmOpenApiNotUpdatedTask> GetNotUpdatedTasks(TasksIdsEnum taskId, int pageNumber, int pageSize);
    }
}
