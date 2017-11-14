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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using System;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service channel connection list.
    /// </summary>
    public class ServiceChannelConnectionValidator : BaseValidator<IVmOpenApiServiceServiceChannelInVersionBase>
    {
        private IChannelService channelService;
        private UserRoleEnum userRole;

        private ServiceChannelIdValidator channelValidator;

        /// <summary>
        /// Ctor - service channel id validator.
        /// </summary>
        /// <param name="model">Service channel id</param>
        /// <param name="channelService">Service channel service</param>
        /// <param name="userRole">Current user role.</param>
        /// <param name="propertyName">Property name</param>
        public ServiceChannelConnectionValidator(IVmOpenApiServiceServiceChannelInVersionBase model, IChannelService channelService, UserRoleEnum userRole,
            string propertyName = "ServiceChannelId") : base(model, propertyName)
        {
            if (model == null)
            {
                throw new ArgumentNullException(PropertyName, $"{PropertyName} must be defined.");
            }

            this.channelService = channelService;
            this.userRole = Model.IsASTIConnection ? UserRoleEnum.Eeva : userRole; // The Asti users have Eeva rights when adding connections - no visibility of channel is checked!

            channelValidator = new ServiceChannelIdValidator(model.ChannelGuid, channelService, userRole, propertyName, "ServiceChannelId");
        }

        /// <summary>
        /// Validates channel id.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;
            
            channelValidator.Validate(modelState);
            if (!modelState.IsValid)
            {
                return;
            }
            var channel = channelValidator.ServiceChannel;
            // additional connection information (service hours & contact information) are only allowed for service location channel. PTV-2475
            if (channel != null && channel.ServiceChannelType != ServiceChannelTypeEnum.ServiceLocation.ToString() &&
                (Model.ServiceHours?.Count > 0 || Model.ContactDetails != null))
            {
                modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.AdditionalInfoForConnection, channel.Id, channel.ServiceChannelType));
                return;
            }
        }
    }
}
