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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service channel id list.
    /// </summary>
    public class ServiceChannelIdListValidator : BaseValidator<IList<Guid>>
    {
        private readonly IChannelService channelService;
        private readonly UserRoleEnum userRole;

        /// <summary>
        /// Ctor - service channel id list validator.
        /// </summary>
        /// <param name="model">List of service channel ids</param>
        /// <param name="channelService">Service channel service</param>
        /// <param name="userRole">Current user own organizations (including sub organizations).</param>
        /// <param name="propertyName">Property name</param>
        public ServiceChannelIdListValidator(IList<Guid> model, IChannelService channelService, UserRoleEnum userRole, string propertyName = "ServiceChannelId") : base(model, propertyName)
        {
            this.channelService = channelService;
            this.userRole = userRole;
        }

        /// <summary>
        /// Validates service channel id list.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            var i = 0;
            Model.ForEach(id =>
            {
                var channelId = new ServiceChannelIdValidator(id, channelService, userRole, $"[{ i++ }].{PropertyName}");
                channelId.Validate(modelState);
            });
        }
    }
}
