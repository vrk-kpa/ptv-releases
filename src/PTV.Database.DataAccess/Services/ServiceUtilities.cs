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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ServiceUtilities), RegisterType.Transient)]
    public class ServiceUtilities
    {
        private readonly IUserIdentification userIdentification;
        private ICommonService commonService;
        private VmListItemLogic listItemLogic;

        public ServiceUtilities(IUserIdentification userIdentification, ICommonService commonService, VmListItemLogic listItemLogic)
        {
            this.userIdentification = userIdentification;
            this.commonService = commonService;
            this.listItemLogic = listItemLogic;
        }

        internal Organization GetUserOrganization(IUnitOfWork unitOfWork)
        {
            var userOrganizationsRep = unitOfWork.CreateRepository<IUserOrganizationRepository>();
            var organizationRep = unitOfWork.CreateRepository<IOrganizationRepository>();
            var userName = userIdentification.UserName.ToLower();
            var preselectedOrgId = userOrganizationsRep.All().FirstOrDefault(x => x.UserName.ToLower() == userName)?.RelationId;
            return preselectedOrgId != null ? organizationRep.All().FirstOrDefault(x => x.Oid == preselectedOrgId) : null;
        }

        /// <summary>
        /// Get the identification of current user for ExternalSource table.
        /// </summary>
        /// <returns></returns>
        internal string GetRelationIdForExternalSource()
        {
            var relationId = userIdentification.UserName;
            if (string.IsNullOrEmpty(relationId))
            {
                throw new Exception(CoreMessages.OpenApi.RelationIdNotFound);
            }
            return relationId;
        }

        internal WebPage GetWebPageByUrl(string url, string language, IUnitOfWork unitOfWork)
        {
            var webpageRep = unitOfWork.CreateRepository<IWebPageRepository>();
            return webpageRep.All().FirstOrDefault(webPage => webPage.Url == url && webPage.Localization.Code == language);
        }
    }
}
