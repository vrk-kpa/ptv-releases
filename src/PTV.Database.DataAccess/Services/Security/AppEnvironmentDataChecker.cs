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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.Security
{
    /// <summary>
    /// AppEnvironmentData role checker
    /// </summary>
    [RegisterService(typeof(AppEnvironmentDataChecker), RegisterType.Transient)]
    internal class AppEnvironmentDataChecker : RoleCheckerBase
    {
        private IUserInfoService userInfoService;
        private IUserIdentification userIdentification;
        public AppEnvironmentDataChecker(IUserIdentification userIdentification, IHttpContextAccessor ctxAccessor, IUserInfoService userInfoService) : base(ctxAccessor)
        {
            this.userInfoService = userInfoService;
            this.userIdentification = userIdentification;
        }

        public override void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork)
        {
            var checkStatus = false;
            var vmUserInfo = userInfoService.GetUserInfo();
            var permissons = vmUserInfo?.Permisions[vmUserInfo.Role][DomainEnum.CurrentIssues.ToCamelCase()];
            if ((permissons != null) && (permissons.RulesAll & PermisionEnum.Update) > 0)
            {
                checkStatus = true;
            }
            
            if (!checkStatus)
            {
                // TODO is list of Roles really needed???
                ThrowError(userIdentification, new List<UserRoleEnum>());
            }
        }
    }
}
