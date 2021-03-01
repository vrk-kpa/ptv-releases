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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Notifications;
using PTV.Framework.Logging;

namespace PTV.Database.DataAccess.Interfaces.Services.V2
{
    public interface INotificationService
    {
        /// <summary>
        /// Gets all notification counts
        /// </summary>
        /// <param name="forOrganizations">Get data for specified organizations</param>
        /// <returns></returns>
        IVmListItemsData<VmNotificationsBase> GetNotificationsNumbers(List<Guid> forOrganizations);
        /// <summary>
        /// Gets all notifications contains services connect with not common channel
        /// </summary>
        /// <param name="search">Search notification view model</param>
        /// <param name="forOrganizations">Get data for specified organizations</param>
        /// <returns>List of notification view models</returns>
        IVmSearchBase GetChannelInCommonUse(IVmNotificationSearch search, List<Guid> forOrganizations);
        /// <summary>
        /// Gets all notifications contains changed service/channel
        /// </summary>
        /// <param name="search">Search notification view model</param>
        /// <param name="forOrganizations">Get data for specified organizations</param>
        /// <returns>Notification view model</returns>
        IVmSearchBase GetContentChanged(IVmNotificationSearch search, List<Guid> forOrganizations);
        /// <summary>
        /// Gets all notifications contains changed general description
        /// </summary>
        /// <param name="search">Search notification view model</param>
        /// <returns>Notification view model</returns>
        IVmSearchBase GetGeneralDescriptionChanged(IVmNotificationSearch search);
        /// <summary>
        /// Gets all notifications contains added general description
        /// </summary>
        /// <param name="search">Search notification view model</param>
        /// <returns>Notification view model</returns>
        IVmSearchBase GetGeneralDescriptionAdded(IVmNotificationSearch search);
        /// <summary>
        /// Clear 1 month old notifications from all tracing tables(used in scheduler)
        /// </summary>
        void ClearOldNotifications(VmJobLogEntry logInfo);
        /// <summary>
        /// Gets all notifications contains connections changes
        /// </summary>
        /// <param name="search">Search notification view model</param>
        /// <param name="forOrganizations">Get data for specified organizations</param>
        /// <returns></returns>
        IVmSearchBase GetConnectionChanges(IVmNotificationSearch search, List<Guid> forOrganizations);
    }
}
