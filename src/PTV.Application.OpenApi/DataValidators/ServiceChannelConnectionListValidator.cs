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

using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service channel connectionlist.
    /// </summary>
    public class ServiceChannelConnectionListValidator : ConnectionBaseValidator<V7VmOpenApiServiceChannelServiceInBase>
    {
        private IChannelService channelService;
        private IServiceService serviceService;

        /// <summary>
        ///  Ctor - service channel connection list validator.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="channelService"></param>
        /// <param name="serviceService"></param>
        /// <param name="codeService"></param>
        /// <param name="propertyName"></param>
        public ServiceChannelConnectionListValidator(IList<V7VmOpenApiServiceChannelServiceInBase> model, IChannelService channelService, IServiceService serviceService,
            ICodeService codeService, string propertyName = "ServiceRelations") : base(model, codeService, propertyName)
        {
            this.channelService = channelService;
            this.serviceService = serviceService;
        }

        /// <summary>
        /// Validates service channel connections. This is for ASTI connections.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            Guid? channelId = Model.Count > 0 ? Model.First().ChannelGuid : (Guid?)null;
            if (!channelId.HasValue)
            {
                return;
            }

            var channelValidator = new ServiceChannelIdValidator(channelId.Value, channelService, UserRoleEnum.Eeva, "ServiceChannelId"); // this is always asti connection so we won't check the channel for visibility by setting user role as Eeva!
            channelValidator.Validate(modelState);
            if (!modelState.IsValid) return;

            var channel = channelValidator.ServiceChannel;

            // ASTI connections are only allowed for service location channel (PTV-3539).
            if (channel.ServiceChannelType != ServiceChannelTypeEnum.ServiceLocation.ToString())
            {
                modelState.AddModelError("ServiceChannelId", string.Format(CoreMessages.OpenApi.MustBeServiceLocationChannel, channel.Id, channel.ServiceChannelType));
            }
            else
            {
                // Validate service ids
                var serviceIds = Model.Select(c => c.ServiceGuid).ToList();
                ServiceIdListValidator services = new ServiceIdListValidator(serviceIds, serviceService, "ServiceRelations");
                services.Validate(modelState);
            }

            if (!modelState.IsValid) return;

            // Validate additional connection info (service hours and contact details)
            base.Validate(modelState);
        }
    }
}
