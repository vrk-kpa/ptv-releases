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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.SignalR;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Enums;
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
        private readonly IServiceManager serviceManager;
        private const string MessagePublishValidationFailed = "MassTool.Exception.PublishValidationFailed";
        private const string MessagePublishEntitiesValidationFailed = "MassTool.Exception.PublishValidationEntitiesFailed";
        private const string MessageArchiveEntitiesValidationFailed = "MassTool.Exception.ArchiveValidationEntitiesFailed";
        private const string MessageCopyEntitiesValidationFailed = "MassTool.Exception.CopyValidationEntitiesFailed";
        private const string MessageMaxCountEntitiesValidationFailed = "MassTool.Exception.MaxCountEntitiesValidationFailed";
        private const string MessageMaxCountLanguageVersionsValidationFailed = "MassTool.Exception.MaxCountLanguageVersionsValidationFailed";
        private const string MessageMandatoryOrganizationValidationFailed = "MassTool.Exception.MandatoryOrganizationValidationFailed";
        private const string MessageRestoreEntitiesValidationFailed = "MassTool.Exception.RestoreValidationEntitiesFailed";
        private const string MassToolStarted = "MassTool.{0}.Started";
        private const string MassToolStartedDescription = "MassTool.{0}.Started.Description";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceManager"></param>
        public MassToolHub(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        private void CallProcessStarted(IClientProxy client, Guid id, MassToolType type)
        {

            client.SendAsync
            (
                "ProcessStarted",
                new VmMassToolProcessMessage
                (
                    id,
                    string.Format(MassToolStarted, type),
                    null,
                    string.Format(MassToolStartedDescription, type)
                )
            );
        }

        private void CallProcessFinished(IClientProxy client, Guid id)
        {
            client.SendAsync
            (
                "ProcessFinished",
                new VmMassToolProcessMessage(id, null, null)
            );
        }

        private IMessage CallMassService(IResolveManager rm, Func<IMassService, IMessage> callService)
        {
            var massService = rm.Resolve<IMassService>();
            return callService(massService);
        }

        public void MassPublish(VmMassDataModel<VmPublishingModel> model)
        {
            Action<IClientProxy> callProcessStartedAction = null;
            Action<IClientProxy> callProcessFinishedAction = null;

            if (!model.PublishAt.HasValue)
            {
                Guid id = Guid.NewGuid();
                callProcessStartedAction = client => CallProcessStarted(client, id, MassToolType.Publish);
                callProcessFinishedAction = client => CallProcessFinished(client, id);
            }

            serviceManager.CallRService
            (
                (rm, client, result) =>
                {
                    callProcessStartedAction?.Invoke(client);
                    return CallMassService(rm, massService => massService.PublishEntities(model, result));
                },
                new Dictionary<Type, string>
                {
                    {typeof(PtvValidationException), MessagePublishValidationFailed},
                    {typeof(PtvEntitiesValidationException), MessagePublishEntitiesValidationFailed},
                    {typeof(PtvMaxCountEntitiesValidationException), MessageMaxCountEntitiesValidationFailed},
                    {
                        typeof(PtvMaxCountLanguageVersionsValidationException),
                        MessageMaxCountLanguageVersionsValidationFailed
                    }
                },
                Clients,
                serviceCallFinishAction: callProcessFinishedAction
            );
        }

        public void MassArchive(VmMassDataModel<VmArchivingModel> model)
        {
            Action<IClientProxy> callProcessStartedAction = null;
            Action<IClientProxy> callProcessFinishedAction = null;

            if (!model.ArchiveAt.HasValue)
            {
                Guid id = Guid.NewGuid();
                callProcessStartedAction = client => CallProcessStarted(client, id, MassToolType.Archive);
                callProcessFinishedAction = client => CallProcessFinished(client, id);
            }
            serviceManager.CallRService
            (
                (rm, client, result) =>
                {
                    callProcessStartedAction?.Invoke(client);
                    return CallMassService(rm, massService => massService.ArchiveEntities(model));
                },
                new Dictionary<Type, string>
                {
                    { typeof(PtvEntitiesValidationException), MessageArchiveEntitiesValidationFailed },
                    { typeof(PtvMaxCountEntitiesValidationException), MessageMaxCountEntitiesValidationFailed  }
                },
                Clients,
                serviceCallFinishAction: callProcessFinishedAction
            );
        }

        public void MassCopy(VmMassDataModel<VmCopyingModel> model)
        {
            Guid id = Guid.NewGuid();
            serviceManager.CallRService(
                (rm, client, result) =>
                {
                    CallProcessStarted(client, id, MassToolType.Copy);
                    return CallMassService(rm, massService => massService.CopyEntities(model));
                },
                new Dictionary<Type, string>
                {
                    { typeof(PtvEntitiesValidationException), MessageCopyEntitiesValidationFailed },
                    { typeof(PtvMaxCountEntitiesValidationException), MessageMaxCountEntitiesValidationFailed },
                    { typeof(PtvMandatoryOrganizationValidationException), MessageMandatoryOrganizationValidationFailed }

                },
                Clients,
                serviceCallFinishAction: client => CallProcessFinished(client, id));
        }

        public void MassRestore(VmMassDataModel<VmRestoringModel> model)
        {
            Guid id = Guid.NewGuid();
            serviceManager.CallRService(
                (rm, client, result) =>
                {
                    CallProcessStarted(client, id, MassToolType.Restore);
                    return CallMassService(rm, massService => massService.RestoreEntities(model));
                },
                new Dictionary<Type, string>
                {
                    { typeof(PtvEntitiesValidationException), MessageRestoreEntitiesValidationFailed },
                    { typeof(PtvMaxCountEntitiesValidationException), MessageMaxCountEntitiesValidationFailed }

                },
                Clients,
                serviceCallFinishAction: client => CallProcessFinished(client, id));
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
