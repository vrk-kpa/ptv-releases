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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Mass;
using PTV.Framework.Interfaces;

namespace PTV.SignalR.Hubs
{
    /// <summary>
    /// Hub for actions related to mass toll used by signalR
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MassToolHub : Hub
    {
        private readonly IMassService _massService;
        private readonly IServiceManager _serviceManager;
        private const string MessagePublishValidationFailed = "MassTool.Exception.PublishValidationFailed";
        private const string MessagePublishEntitiesValidationFailed = "MassTool.Exception.PublishValidationEntitiesFailed";
        private const string MessageArchiveEntitiesValidationFailed = "MassTool.Exception.ArchiveValidationEntitiesFailed";
        private const string MessageCopyEntitiesValidationFailed = "MassTool.Exception.CopyValidationEntitiesFailed";
        private const string MessageMaxCountEntitiesValidationFailed = "MassTool.Exception.MaxCountEntitiesValidationFailed";
        private const string MessageMaxCountLanguageVersionsValidationFailed = "MassTool.Exception.MaxCountLanguageVersionsValidationFailed";
        private const string MessageMandatoryOrganizationValidationFailed = "MassTool.Exception.MandatoryOrganizationValidationFailed";
        private const string MessageEntitiesCopiedSuccessfully = "MassTool.Copy.Success";
        
        public MassToolHub(IMassService massService, IServiceManager serviceManager)
        {
            this._massService = massService;
            this._serviceManager = serviceManager;
        }
        
        public void MassPublish(VmMassDataModel<VmPublishingModel> model)
        {
            _serviceManager.CallRService(
            (rm) => _massService.PublishEntities(rm, model),
            new Dictionary<Type, string>()
            {
                { typeof(PtvValidationException), MessagePublishValidationFailed },
                { typeof(PtvEntitiesValidationException), MessagePublishEntitiesValidationFailed },
                { typeof(PtvMaxCountEntitiesValidationException), MessageMaxCountEntitiesValidationFailed },
                { typeof(PtvMaxCountLanguageVersionsValidationException), MessageMaxCountLanguageVersionsValidationFailed  }
            },
            Clients);
        }
        
        public void MassArchive(VmMassDataModel<VmArchivingModel> model)
        {
            _serviceManager.CallRService(
                (rm) => _massService.ArchiveEntities(rm, model),
                new Dictionary<Type, string>()
                {
                    { typeof(PtvEntitiesValidationException), MessageArchiveEntitiesValidationFailed },
                    { typeof(PtvMaxCountEntitiesValidationException), MessageMaxCountEntitiesValidationFailed  }
                },
                Clients);
        }
        
        public void MassCopy(VmMassDataModel<VmCopyingModel> model)
        {
            _serviceManager.CallRService(
                (rm) => _massService.CopyEntities(rm, model),
                new Dictionary<Type, string>()
                {
                    { typeof(string), MessageEntitiesCopiedSuccessfully },
                    { typeof(PtvEntitiesValidationException), MessageCopyEntitiesValidationFailed },
                    { typeof(PtvMaxCountEntitiesValidationException), MessageMaxCountEntitiesValidationFailed },
                    { typeof(PtvMandatoryOrganizationValidationException), MessageMandatoryOrganizationValidationFailed }
                    
                },
                Clients);
        }
        
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }
    }
    
    
}