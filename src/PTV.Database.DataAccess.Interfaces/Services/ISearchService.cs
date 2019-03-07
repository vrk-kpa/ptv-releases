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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface ISearchService
    {        
        /// <summary>
        /// Search entities based on search criteria
        /// </summary>
        /// <param name="vmEntitySearch"></param>
        /// <returns></returns>
        IVmSearchBase SearchEntities(IVmEntitySearch vmEntitySearch);        
    }

    internal interface ISearchServiceInternal : ISearchService
    {
        /// <summary>
        /// Search entities based on search criteria
        /// </summary>
        /// <param name="vmEntitySearch"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        IVmSearchBase SearchOrganizations(IVmEntitySearch vmEntitySearch, IUnitOfWork unitOfWork);

        /// <summary>
        /// Search notification connections based on search criteria
        /// </summary>
        /// <param name="vmEntitySearch"></param>
        /// <returns></returns>
        IVmSearchBase SearchConnections(IVmEntitySearch vmEntitySearch);

        /// <summary>
        /// Search notification connections count based on search criteria
        /// </summary>
        /// <param name="vmEntitySearch"></param>
        /// <returns></returns>
        int SearchConnectionsCount(IVmEntitySearch vmEntitySearch);
    }
}
