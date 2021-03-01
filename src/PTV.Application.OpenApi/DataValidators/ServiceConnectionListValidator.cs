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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for service connection list.
    /// </summary>
    public class ServiceConnectionListValidator<TModel, TModelContact, TModelServiceHours, TModelOpeningTime>
        : ConnectionBaseValidator<TModel, TModelContact, TModelServiceHours, TModelOpeningTime>
        where TModel : IOpenApiConnectionForService<TModelContact, TModelServiceHours, TModelOpeningTime>
        where TModelContact : IVmOpenApiContactDetailsInVersionBase
        where TModelServiceHours : IVmOpenApiServiceHourBase<TModelOpeningTime>
         where TModelOpeningTime : IVmOpenApiDailyOpeningTime
    {
        private readonly IChannelService channelService;
        //private ICodeService codeService;
        private readonly List<Guid> userOrganizations;
        private readonly bool isASTI;

        /// <summary>
        ///  Ctor - service channel connection list validator.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="channelService"></param>
        /// <param name="codeService"></param>
        /// <param name="checkVisibility"></param>
        /// <param name="openApiVersion"></param>
        /// <param name="userOrganizations"></param>
        /// <param name="isASTI"></param>
        /// <param name="propertyName"></param>
        public ServiceConnectionListValidator(
            IList<TModel> model,
            IChannelService channelService,
            ICodeService codeService,
            bool checkVisibility,
            int openApiVersion,
            IList<Guid> userOrganizations = null,
            bool isASTI = false,
            string propertyName = "ChannelRelations")
            : base(model, codeService, openApiVersion, propertyName)
        {
            this.channelService = channelService;
            //this.codeService = codeService;
            this.userOrganizations = userOrganizations == null ? null : userOrganizations.ToList();
            this.isASTI = isASTI;

            if (checkVisibility && (userOrganizations == null || userOrganizations?.Count <= 0))
            {
                throw new ArgumentNullException(nameof(propertyName), "User organizations need to be defined if channel visibility needs to be checked.");
            }
        }

        /// <summary>
        /// Validates service channels
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            var channelIds = Model.Select(m => m.ChannelGuid).ToList();

            if (channelIds.Count > 0)
            {
                var connectionData = channelService.CheckChannels(channelIds, userOrganizations);
                if (connectionData == null)
                {
                    // Asti connections are only allowed for service location channel (PTV-3539)
                    if (isASTI)
                    {
                        modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.AllMustBeServiceLocationChannels, "Asti connections", string.Join(", ", channelIds)));
                    }
                    return;
                }

                if (connectionData.NotExistingChannels?.Count > 0)
                {
                    modelState.AddModelError(PropertyName, $"Some of the channels were not found: '{string.Join(", ", connectionData.NotExistingChannels)}'");
                }

                // Asti connections are only allowed for service location channel (PTV-3539)
                if (isASTI)
                {
                    if (connectionData.ServiceLocationChannels?.Count > 0)
                    {
                        var notServiceLocationChannels = channelIds.Where(c => !connectionData.ServiceLocationChannels.Contains(c)).ToList();
                        if (notServiceLocationChannels.Count > 0)
                        {
                            modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.AllMustBeServiceLocationChannels, "Asti connections", string.Join(", ", notServiceLocationChannels)));
                        }
                    }
                    else
                    {
                        modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.AllMustBeServiceLocationChannels, "Asti connections", string.Join(", ", channelIds)));
                    }
                }

                // Additional connection information (service hours & contact information) are only allowed for service location channels. PTV-2475
                var channelsWithAdditionalData = Model.Where(m =>m.ServiceHours?.Count() > 0 || m.ContactDetails != null)?.Select(m => m.ChannelGuid).ToList();
                if (channelsWithAdditionalData?.Count > 0)
                {
                    if (connectionData.ServiceLocationChannels?.Count > 0)
                    {
                        var notServiceLocationChannels = channelsWithAdditionalData.Where(c => !connectionData.ServiceLocationChannels.Contains(c)).ToList();
                        if (notServiceLocationChannels.Count > 0)
                        {
                            modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.AllMustBeServiceLocationChannels, "ServiceHours and ContactDetails", string.Join(", ", notServiceLocationChannels)));
                        }
                    }
                    else
                    {
                        modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.AllMustBeServiceLocationChannels, "ServiceHours and ContactDetails", string.Join(", ", channelsWithAdditionalData)));
                    }

                    if (!modelState.IsValid) return;

                    // Validate additional connection info
                    // Validate service hours
                    var channelsWithHours = Model.Where(m => m.ServiceHours?.Count > 0).ToList();
                    if (channelsWithHours.Count > 0)
                    {
                        var hours = channelsWithHours.SelectMany(h => h.ServiceHours).ToList();
                        ValidateServiceHours(hours, modelState);
                    }
                    // Validate contact information
                    var channelsWithContactInfo = Model.Where(m => m.ContactDetails != null)?.Select(m => m.ContactDetails).ToList();
                    ValidateContactInfo(channelsWithContactInfo, modelState);
                }
            }
        }
    }
}
