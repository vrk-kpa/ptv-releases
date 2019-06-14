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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Common.Connections;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Database.DataAccess.Interfaces.Services.V2
{
    public interface IEntityHistoryService
    {
        //Entity history
        
        /// <summary>
        /// Get history of service
        /// </summary>
        /// <param name="search">Search and paging model</param>
        /// <returns>service history</returns>
        IVmSearchBase GetServiceEntityHistory(IVmHistorySearch search);
        /// <summary>
        /// Get history of service channel
        /// </summary>
        /// <param name="search">Search and paging model</param>
        /// <returns>service channel history</returns>
        IVmSearchBase GetChannelEntityHistory(IVmHistorySearch search);
        /// <summary>
        /// Get history of organization
        /// </summary>
        /// <param name="search">Search and paging model</param>
        /// <returns>organization history</returns>
        IVmSearchBase GetOrganizationEntityHistory(IVmHistorySearch search);
        /// <summary>
        /// Get history of service collection
        /// </summary>
        /// <param name="search">Search and paging model</param>
        /// <returns>service collection history</returns>
        IVmSearchBase GetServiceCollectionEntityHistory(IVmHistorySearch search);
        /// <summary>
        /// Get history of general description
        /// </summary>
        /// <param name="search">Search and paging model</param>
        /// <returns>general description history</returns>
        IVmSearchBase GetGeneralDescriptionEntityHistory(IVmHistorySearch search);
        
        // Entity connection history
       
        /// <summary>
        /// Get history of service connected entities
        /// </summary>
        /// <param name="search">Search and paging model</param>
        /// <returns>history of connected entities</returns>
        IVmSearchBase GetServiceConnectionHistory(IVmHistorySearch search);
        /// <summary>
        /// Get history of channel connected entities
        /// </summary>
        /// <param name="search">Search and paging model</param>
        /// <returns>history of connected entities</returns>
        IVmSearchBase GetChannelConnectionHistory(IVmHistorySearch search);
        /// <summary>
        /// Get history of service collection connected entities
        /// </summary>
        /// <param name="search">Search and paging model</param>
        /// <returns>history of connected entities</returns>
        IVmSearchBase GetServiceCollectionConnectionHistory(IVmHistorySearch search);  
        /// <summary>
        /// Get history of general description connected entities
        /// </summary>
        /// <param name="search">Search and paging model</param>
        /// <returns>history of connected entities</returns>
        IVmSearchBase GetGeneralDescriptionConnectionHistory(IVmHistorySearch search);  
        
    }
}
