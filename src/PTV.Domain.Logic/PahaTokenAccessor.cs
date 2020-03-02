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
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Logic
{
    public interface IPahaTokenAccessor
    {
        AccessRightEnum AccessRights { get; }
        Guid ActiveOrganizationId { get; }
        string ErrorMessageFlat { get; }
        bool IsUserPresent { get; }
        string UserName { get; }
        UserRoleEnum UserRole { get; }

        IPahaTokenAccessor ProcessToken(string token = null);
    }

    [RegisterService(typeof(IPahaTokenAccessor), RegisterType.Transient)]
    public class PahaTokenAccessor : IPahaTokenAccessor
    {
        private readonly IResolveManager resolveManager;

        public PahaTokenAccessor(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }

        private PahaToken GetToken()
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessorInternal>();
                return pahaTokenProcessor.GetToken();
            }
        }

        private VmUserAccessRights GetUserAccessRights()
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessorInternal>();
                return pahaTokenProcessor.GetUserAccessRights();
            }
        }

        private Guid GetActiveOrganizationId()
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessor>();
                return pahaTokenProcessor.ActiveOrganization;
            }
        }

        private List<string> GetErrorMessages()
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessor>();
                return pahaTokenProcessor.ErrorMessages;
            }
        }

        public AccessRightEnum AccessRights => GetUserAccessRights().AccessRights;
        public Guid ActiveOrganizationId => GetActiveOrganizationId();
        public string ErrorMessageFlat => string.Join(" // ", GetErrorMessages());
        public bool IsUserPresent => !string.IsNullOrEmpty(UserName);
        public string UserName => GetToken()?.UserName;
        public UserRoleEnum UserRole => GetUserAccessRights().UserRole;
        public IPahaTokenAccessor ProcessToken(string token = null)
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessor>();
                pahaTokenProcessor.ProcessToken(token);
                return this;
            }
        }

        public static PahaToken ExtractPahaToken(IEnumerable<Claim> claims, Dictionary<Guid,Guid> guidMappings = null)
        {
            return PahaToken.ExtractPahaToken(claims, guidMappings);
        }
    }
}
