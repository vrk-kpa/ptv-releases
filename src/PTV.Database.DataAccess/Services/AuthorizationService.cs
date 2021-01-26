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
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.Security;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IAuthorizationService), RegisterType.Transient)]
    internal class AuthorizationService : ServiceBase, IAuthorizationService
    {
        private readonly IContextManager contextManager;
        private readonly ApplicationConfiguration configuration;
        private readonly ICacheManager cacheManager;
        private readonly ITranslationEntity translationManagerToVm;
        private const string MessageUserLoginFailed = "Authorization.Exception.LoginFailed";

        public AuthorizationService(ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            ApplicationConfiguration configuration,
            IContextManager contextManager,
            ICacheManager cacheManager,
            IVersioningManager versioningManager)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker,
                versioningManager)
        {
            this.contextManager = contextManager;
            this.configuration = configuration;
            this.cacheManager = cacheManager;
            this.translationManagerToVm = translationManagerToVm;
        }


        public IReadOnlyList<VmUserAccessRightsGroupSimple> GetUserAccessGroupsList()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IUserAccessRightsGroupRepository>();
                return translationManagerToVm.TranslateAll<UserAccessRightsGroup, VmUserAccessRightsGroupSimple>(rep.All().Include(i => i.UserAccessRightsGroupNames));
            });
        }

        public IReadOnlyList<VmUserAccessRightsGroup> GetUserAccessGroupsFull()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IUserAccessRightsGroupRepository>();
                return translationManagerToVm.TranslateAll<UserAccessRightsGroup, VmUserAccessRightsGroup>(rep.All());
            });
        }


        public Guid SaveAuthorizationInfo(VmAuthEntryPoint model)
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var entity = TranslationManagerToEntity.Translate<VmAuthEntryPoint, AuthorizationEntryPoint>(model, unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous, PreSaveAction.DoNotSetAudits);
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

        public string GetAuthorizationToken(VmLoginForm loginForm)
        {
            var userAccessRightsGroupCode = cacheManager.UserAccessRightsCache.Get(loginForm.UserAccessRightsGroup).GroupCode;

            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, configuration.GetTokenServiceUrl() + "/connect/token");
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", loginForm.Name),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("password", loginForm.Password),
                    new KeyValuePair<string, string>("organizationId", loginForm.Organization.ToString()),
                    new KeyValuePair<string, string>("userAccessRightsGroup", userAccessRightsGroupCode)
                });
                request.Content = content;

                var response = httpClient.SendAsync(request).Result;
                var responseBody = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.Forbidden || string.IsNullOrEmpty(responseBody))
                {
                    string subCode = null;
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        var error = JsonConvert.DeserializeObject<VmTokenError>(responseBody, new JsonSerializerSettings { Error = (_, arg) => Console.WriteLine("kokos")});
                        subCode = $"Authorization.Exception.Type.{error.Error}";
                    }
                    throw new AuthorizationException("An error occurred while retrieving an access token. Possible reason: Invalid credentials.", MessageUserLoginFailed, subCode);
                }
                Console.WriteLine(responseBody);
                JObject payload;
                try
                {
                    payload = JObject.Parse(responseBody);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("An error occurred while retrieving an access token. Malformed response.", ex);
                }
                if (payload["error"] != null)
                {
                    throw new InvalidOperationException($"An error occurred while retrieving an access token. Error:{payload["error"]}");
                }

                return (string) payload["serviceToken"];
            }
        }
    }
}
