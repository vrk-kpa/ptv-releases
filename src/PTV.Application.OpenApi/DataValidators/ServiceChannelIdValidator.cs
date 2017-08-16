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
using PTV.Framework;
using System;
using System.Collections.Generic;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service channel id.
    /// </summary>
    public class ServiceChannelIdValidator : BaseValidator<Guid>
    {
        private IChannelService channelService;
        private List<Guid> userOrganizations;

        /// <summary>
        /// Ctor - service channel id validator.
        /// </summary>
        /// <param name="model">Service channel id</param>
        /// <param name="channelService">Service channel service</param>
        /// <param name="userOrganizations">Current user own organizations (including sub organizations).</param>
        /// <param name="propertyName">Property name</param>
        public ServiceChannelIdValidator(Guid model, IChannelService channelService, List<Guid> userOrganizations, string propertyName = "ServiceChannelId") : base(model, propertyName)
        {
            this.channelService = channelService;
            this.userOrganizations = userOrganizations;
        }

        /// <summary>
        /// Validates channel id.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            var channel = channelService.GetServiceChannelById(Model, 0, true);
            if (channel?.Id != Model)
            {
                modelState.AddModelError(PropertyName, CoreMessages.OpenApi.RecordNotFound);
                return;
            }

            if (!channel.IsVisibleForAll && (userOrganizations == null || !userOrganizations.Contains(channel.OrganizationId)))
            {
                modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.ChannelNotVisibleForAll, Model));
            }
        }
    }
}
