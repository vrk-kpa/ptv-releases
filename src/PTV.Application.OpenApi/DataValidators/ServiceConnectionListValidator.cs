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
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using System;
using System.Collections.Generic;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service connection list.
    /// </summary>
    public class ServiceConnectionListValidator : BaseValidator<IList<V7VmOpenApiServiceServiceChannelAstiInBase>>
    {
        private IChannelService channelService;
        private UserRoleEnum userRole;

        /// <summary>
        ///  Ctor - service channel connection list validator.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="channelService"></param>
        /// <param name="userRole"></param>
        /// <param name="propertyName"></param>
        public ServiceConnectionListValidator(IList<V7VmOpenApiServiceServiceChannelAstiInBase> model, IChannelService channelService, UserRoleEnum userRole, string propertyName = "ChannelRelations")
            : base(model, propertyName)
        {
            this.channelService = channelService;
            this.userRole = userRole;
        }

        /// <summary>
        /// Validates service channels
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            var i = 0;
            Model.ForEach(relation =>
            {
                var channelId = new ServiceChannelConnectionValidator(relation, channelService, userRole, $"{PropertyName}.[{ i++ }]");
                channelId.Validate(modelState);
            });
        }
    }
}
