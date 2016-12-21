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
using Microsoft.AspNetCore.Authorization;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces;
using System;
using PTV.Framework.ServiceManager;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NLog.LayoutRenderers;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Localization;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "Channels"
    /// </summary>
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/channel")]
    public class RESTChannelController : RESTBaseController
    {
        private readonly IChannelService channelService;
        private readonly IServiceManager serviceManager;
        private readonly ILanguageService languageService;
        private readonly IPostalCodeService postalCodeService;

        private const string messageAddElectronicChannel = "Service.AddElectronicChannel.MessageSaved";
        private const string messageChannelStepSave = "Service.ChannelStep.MessageSaved";
        private const string messagePublishChannel = "Channel.Published.Successfully";
        private const string messageDeleteChannel = "Channel.Deleted.Successfully";
        private const string messageAddWebPageChannel = "Service.AddWebPageChannel.MessageSaved";
        private const string messageAddPhoneChannel = "Service.AddPhoneChannel.MessageSaved";
        private const string messageAddLocationChannel = "Service.AddLocationChannel.MessageSaved";
        private const string messageArgumentException = "Service.Exception.InvalidArgument";

        public RESTChannelController(IChannelService channelService, IPostalCodeService postalCodeService, IServiceManager serviceManager, ILanguageService languageService, ILogger<RESTChannelController> logger) : base(logger)
        {
            this.channelService = channelService;
            this.serviceManager = serviceManager;
            this.languageService = languageService;
            this.postalCodeService = postalCodeService;
        }


        /// <summary>
        /// Get data for search form for channels
        /// </summary>
        /// <returns></returns>
        [Route("GetChannelSearch")]
        [HttpGet]
        public IServiceResultWrap GetChannelSearch()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = channelService.GetChannelSearch() },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Search for channel by criterias provided as model
        /// </summary>
        /// <param name="model">Model containing search criterias</param>
        /// <returns>List of channels</returns>
        [Route("ChannelSearchResult")]
        [HttpPost]
        public IServiceResultWrap ChannelSearchResult([FromBody] VmChannelSearchParams model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)  { Data = channelService.SearchChannelResult(model) },
                new Dictionary<Type, string>());
        }

        [Route("GetChannelServiceStep")]
        [HttpPost]
        public IServiceResultWrap GetChannelServiceStep([FromBody] VmChannelSearchParams model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetChannelServiceStep(model) },
                new Dictionary<Type, string>());
        }

        [Route("GetPostalCodes")]
        [HttpPost]
        public IServiceResultWrap GetPostalCodes([FromBody] string searchedCode)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = postalCodeService.GetPostalCodes(searchedCode)},
                new Dictionary<Type, string>());
        }

        [Route("ConnectingChannelSearchResultData")]
        [HttpPost]
        public IServiceResultWrap ConnectingChannelSearchResultData([FromBody] VmServiceStep4 model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = channelService.ConnectingChannelSearchResult(model) },
                new Dictionary<Type, string>());
        }

        [Route("CheckUrl")]
        [HttpPost]
        public IServiceResultWrap CheckUrl([FromBody] VmUrlChecker model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = channelService.CheckUrl(model) },
                new Dictionary<Type, string>());
        }

        [Route("GetAddElectronicChannelStep1")]
        [HttpPost]
        public IServiceResultWrap GetAddElectronicChannelStep1([FromBody] VmGetChannelStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetElectronicChannelStep1(model)},
                new Dictionary<Type, string>());
        }

        [Route("GetAddWebPageChannelStep1")]
        [HttpPost]
        public IServiceResultWrap GetAddWebPageChannelStep1([FromBody] VmGetChannelStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetWebPageChannelStep1(model) },
                new Dictionary<Type, string>());
        }

        [Route("GetAddPhoneChannelStep1")]
        [HttpPost]
        public IServiceResultWrap GetAddPhoneChannelStep1([FromBody] VmGetChannelStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetPhoneChannelStep1(model) },
                new Dictionary<Type, string>());
        }

        [Route("GetAddLocationChannelStep1")]
        [HttpPost]
        public IServiceResultWrap GetAddLocationChannelStep1([FromBody] VmGetChannelStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetLocationChannelStep1(model) },
                new Dictionary<Type, string>());
        }

        [Route("GetAddLocationChannelStep2")]
        [HttpPost]
        public IServiceResultWrap GetAddLocationChannelStep2([FromBody] VmGetChannelStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetLocationChannelStep2(model) },
                new Dictionary<Type, string>());
        }

        [Route("GetAddLocationChannelStep3")]
        [HttpPost]
        public IServiceResultWrap GetAddLocationChannelStep3([FromBody] VmGetChannelStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetLocationChannelStep3(model) },
                new Dictionary<Type, string>());
        }

        [Route("GetAddPrintableFormChannelStep1")]
        [HttpPost]
        public IServiceResultWrap GetAddPrintableFormChannelStep1([FromBody] VmGetChannelStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetPrintableFormChannelStep1(model) },
                new Dictionary<Type, string>());
        }

        [Route("GetAddLocationChannelStep4")]
        [Route("GetAddElectronicChannelStep2")]
        [Route("GetAddPhoneChannelStep2")]
        [Route("GetOpeningHoursStep")]
        [HttpGet]
        [HttpPost]
        public IServiceResultWrap GetOpeningHoursStep([FromBody] VmGetChannelStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetOpeningHoursStep(model) },
                new Dictionary<Type, string>());
        }

        [Route("SaveAllElectronicChannelChanges")]
        [HttpPost]
        public IServiceResultWrap SaveAllElectronicChannelChanges([FromBody] VmElectronicChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.AddElectronicChannel(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageAddElectronicChannel },
                    { typeof(ArgumentException), messageArgumentException }
                }
            );
        }

        [Route("SaveAllLocationChannelChanges")]
        [HttpPost]
        public IServiceResultWrap SaveAllLocationChannelChanges([FromBody] VmLocationChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.AddLocationChannel(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageAddLocationChannel },
                    { typeof(ArgumentException), messageArgumentException }
                });
        }

        [Route("GetWorldLanguages")]
        [HttpGet]
        public IServiceResultWrap GetWorldLanguages()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = languageService.GetWorldLanguages() },
                new Dictionary<Type, string>());
        }

        [Route("PhoneSaveAllChanges")]
        [HttpPost]
        public IServiceResultWrap PhoneSaveAllChanges([FromBody] VmPhoneChannel model)
        {

            var data = model;
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.AddPhoneChannel(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageAddPhoneChannel },
                    { typeof(ArgumentException), messageArgumentException }
                });
        }

        [Route("SaveAllWebPageChannelChanges")]
        [HttpPost]
        public IServiceResultWrap SaveAllWebPageChannelChanges([FromBody] VmWebPageChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.AddWebPageChannel(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageAddWebPageChannel },
                    { typeof(ArgumentException), messageArgumentException }
                }
            );
        }

        [Route("SaveWebPageStep1Changes")]
        [HttpPost]
        public IServiceResultWrap SaveWebPageStep1Changes([FromBody] VmWebPageChannelStep1 model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SaveWebPageChannelStep1(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageChannelStepSave },
                    { typeof(ArgumentException), messageArgumentException }
                }
            );
        }


        [Route("SavePhoneChannelStep1Changes")]
        [HttpPost]
        public IServiceResultWrap SavePhoneChannelStep1Changes([FromBody] VmPhoneChannelStep1 model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SavePhoneChannelStep1(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageChannelStepSave },
                    { typeof(ArgumentException), messageArgumentException }
                }
            );
        }

        [Route("SaveAllPrintableFormChannelChanges")]
        [HttpPost]
        public IServiceResultWrap SaveAllPrintableFormChannelChanges([FromBody] VmPrintableFormChannel model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.AddPrintableFormChannel(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageAddWebPageChannel },
                    { typeof(ArgumentException), messageArgumentException }
                }
            );
        }

        [Route("SavePrintableFormChannelStep1Changes")]
        [HttpPost]
        public IServiceResultWrap SavePrintableFormChannelStep1Changes([FromBody] VmPrintableFormChannelStep1 model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SavePrintableFormChannelStep1(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageChannelStepSave },
                    { typeof(ArgumentException), messageArgumentException }
                }
            );
        }


        [Route("SaveElectronicChannelStep1Changes")]
        [HttpPost]
        public IServiceResultWrap SaveElectronicChannelStep1Changes([FromBody] VmElectronicChannelStep1 model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SaveElectronicChannelStep1(model ) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageChannelStepSave },
                    { typeof(ArgumentException), messageArgumentException }
                }
            );
        }

        [Route("SaveElectronicChannelStep2Changes")]
        [Route("SavePhoneChannelStep2Changes")]
        [Route("SaveLocationChannelStep4Changes")]
        [Route("SaveOpeningHoursStep")]
        [HttpPost]
        public IServiceResultWrap SaveOpeningHoursStep([FromBody] VmOpeningHoursStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SaveOpeningHoursStep(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageChannelStepSave },
                    { typeof(ArgumentException), messageArgumentException }
                }
            );
        }

        [Route("SaveLocationChannelStep1Changes")]
        [HttpPost]
        public IServiceResultWrap SaveLocationChannelStep1Changes([FromBody] VmLocationChannelStep1 model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SaveLocationChannelStep1(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageChannelStepSave },
                    { typeof(ArgumentException), messageArgumentException }
                }
            );
        }
        [Route("SaveLocationChannelStep2Changes")]
        [HttpPost]
        public IServiceResultWrap SaveLocationChannelStep2Changes([FromBody] VmLocationChannelStep2 model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SaveLocationChannelStep2(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageChannelStepSave },
                    { typeof(ArgumentException), messageArgumentException }
                }
            );
        }
        [Route("SaveLocationChannelStep3Changes")]
        [HttpPost]
        public IServiceResultWrap SaveLocationChannelStep3Changes([FromBody] VmLocationChannelStep3 model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.SaveLocationChannelStep3(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageChannelStepSave },
                    { typeof(ArgumentException), messageArgumentException }
                }
            );
        }

        [Route("PublishChannel")]
        [HttpPost]
        public IServiceResultWrap PublishChannel([FromBody] VmChannelBase model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.PublishChannel(model.Id) },
                new Dictionary<Type, string>() { { typeof(string), messagePublishChannel } });
        }

        [Route("DeleteChannel")]
        [HttpPost]
        public IServiceResultWrap DeleteChannel([FromBody] VmChannelBase model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.DeleteChannel(model.Id) },
                new Dictionary<Type, string>() { { typeof(string), messageDeleteChannel } });
        }

        [Route("GetChannelStatus")]
        [HttpPost]
        public IServiceResultWrap GetChannelStatus([FromBody] VmChannelBase model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = channelService.GetChannelStatus(model.Id.Value) },
                new Dictionary<Type, string>());
        }
    }
}
