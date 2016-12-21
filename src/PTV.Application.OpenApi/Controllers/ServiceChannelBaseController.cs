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

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using System;
using System.Linq;

namespace PTV.Application.OpenApi.Controllers
{
    public class ServiceChannelBaseController : ValidationBaseController
    {
        private IChannelService channelService;

        /// <summary>
        /// Gets the OrganizationService instance.
        /// </summary>
        protected IChannelService ChannelService { get { return channelService; } private set { } }

        /// <summary>
        /// ServiceChannelController constructor.
        /// </summary>
        public ServiceChannelBaseController(IChannelService channelService, IOrganizationService organizationService,
            ICodeService codeService, IOptions<AppSettings> settings, IFintoService fintoService) : base(organizationService, codeService, settings, fintoService)
        {
            this.channelService = channelService;
        }

        public virtual IActionResult Get([FromQuery]DateTime? date, [FromQuery]int page = 1)
        {
            var pageSize = Settings.PageSize > 0 ? Settings.PageSize : 1000;
            return Ok(ChannelService.GetServiceChannels(date, page, pageSize));
        }


        protected IActionResult ValidateRequest<TModel>(TModel request) where TModel : IVmOpenApiServiceChannelIn
        {
            if (request == null)
            {
                ModelState.AddModelError("RequestIsNull", CoreMessages.OpenApi.RequestIsNull);
                return new BadRequestObjectResult(ModelState);
            }

            ValidateOrganization(request.OrganizationId);
            ValidateLanguageList(request.Languages);

            // Validate printable form channel specific items.
            if(request is V2VmOpenApiPrintableFormChannelInBase)
            {
                var deliveryAddress = (request as V2VmOpenApiPrintableFormChannelInBase).DeliveryAddress;
                if (request is VmOpenApiPrintableFormChannelIn)
                {
                    deliveryAddress = (request as VmOpenApiPrintableFormChannelIn).DeliveryAddress;
                }
                else if (request is VmOpenApiPrintableFormChannelInBase)
                {
                    deliveryAddress = (request as VmOpenApiPrintableFormChannelInBase).DeliveryAddress;
                }

                ValidateAddress(deliveryAddress, "DeliveryAddress");
            }

            // Validate service location channel specific items
            if (request is V2VmOpenApiServiceLocationChannelInBase)
            {
                // Validate municipalities (service areas)
                var i = 0;
                (request as V2VmOpenApiServiceLocationChannelInBase).ServiceAreas.ForEach(m => ValidateMunicipalityCode(m, string.Format("ServiceAreas[{0}]", i++)));

                //Validate addresses
                if (request is VmOpenApiServiceLocationChannelIn)
                {
                    ValidateAddressList((request as VmOpenApiServiceLocationChannelIn).Addresses);
                }
                else if (request is VmOpenApiServiceLocationChannelInBase)
                {
                    ValidateAddressList((request as VmOpenApiServiceLocationChannelInBase).Addresses);
                    //addresses = (request as VmOpenApiServiceLocationChannelInBase).Addresses;
                }
                else
                {
                    ValidateAddressList((request as V2VmOpenApiServiceLocationChannelInBase).Addresses);
                }
            }

            // Validate the items
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            return null;
        }

        /// <summary>
        /// Checks if a service channel with given id exists in the database.
        /// </summary>
        /// <param name="channelId">channel guid as string</param>
        /// <returns>true if the channel exists otherwise false</returns>
        protected bool CheckChannelExists(string channelId)
        {
            if (string.IsNullOrWhiteSpace(channelId))
            {
                return false;
            }

            Guid? chId = channelId.ParseToGuid();

            if (!chId.HasValue)
            {
                return false;
            }

            return ChannelService.ChannelExists(chId.Value);
        }

    }
}
