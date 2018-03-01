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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IAuthorizationService), RegisterType.Transient)]
    internal class AuthorizationService : ServiceBase, IAuthorizationService
    {
        private readonly IContextManager contextManager;
        public AuthorizationService(ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IContextManager contextManager) : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
        }

        public Guid SaveAuthorizationInfo(VmAuthEntryPoint model)
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var entity = TranslationManagerToEntity.Translate<VmAuthEntryPoint, AuthorizationEntryPoint>(model, unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                return entity.Id;
            });
        }

        public string GetAuthorizationToken(Guid tokenID)
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var authRep = unitOfWork.CreateRepository<IAuthorizationEntryPointRepository>();
                var authEntryPoint = authRep.All().FirstOrDefault(i => i.Id == tokenID);
                if (string.IsNullOrEmpty(authEntryPoint?.Token))
                {
                    throw new ArgumentNullException("Token does not exist for given tokenID");
                }
                authRep.Remove(authEntryPoint);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                return authEntryPoint.Token;
            });
        }
    }
}
