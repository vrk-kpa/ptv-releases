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
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Exceptions;
using System;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service channel id.
    /// </summary>
    public class ServiceChannelIdValidator : BaseValidator<Guid>
    {
        private IChannelService channelService;
        private UserRoleEnum userRole;
        private bool publishedVersion;

        private IVmOpenApiServiceChannel serviceChannel;

        /// <summary>
        /// Ctor - service channel id validator.
        /// </summary>
        /// <param name="model">Service channel id</param>
        /// <param name="channelService">Service channel service</param>
        /// <param name="userRole">Current user role.</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="propertyItemName"></param>
        /// <param name="publishedVersion">Indicates if published or latest version should be fetched.</param>
        public ServiceChannelIdValidator(Guid model, IChannelService channelService, UserRoleEnum userRole, string propertyName, string propertyItemName = null, bool publishedVersion = true) : base(model, propertyName)
        {
            this.channelService = channelService;
            this.userRole = userRole;
            this.publishedVersion = publishedVersion;
            if (!string.IsNullOrEmpty(propertyItemName))
            {
                PropertyName = $"{propertyName}.{propertyItemName}";
            }
        }

        /// <summary>
        /// Validates channel id.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            serviceChannel = channelService.GetServiceChannelByIdSimple(Model, publishedVersion);
            if (serviceChannel?.Id != Model)
            {
                modelState.AddModelError(PropertyName, CoreMessages.OpenApi.RecordNotFound);
                return;
            }

            // Check channel visibility
            if (userRole != UserRoleEnum.Eeva)
            {
                if (!serviceChannel.IsVisibleForAll && !((VmOpenApiServiceChannel)serviceChannel).Security.IsOwnOrganization)
                {
                    modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.ChannelNotVisibleForAll, Model));
                }
            }
        }

        /// <summary>
        /// Get related service channel
        /// </summary>
        public IVmOpenApiServiceChannel ServiceChannel { get { return serviceChannel; } private set { } }
    }
}
