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
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.Extensions.Logging;
using PTV.Domain.Model;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework.ServiceManager;
using PTV.Framework.Interfaces;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PTV.Application.Api.Controllers
{
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/service")]
    [Controller]
    public class RESTServiceController : RESTBaseController
    {
        private IServiceService service;
        private IOrganizationService organizationService;
        private IServiceManager serviceManager;
        private IFintoService fintoService;
        private ICommonService commonService;

        private const string messageAddService = "Service.AddService.MessageSaved";
        private const string messagePublishService = "Service.EditService.MessagePublished";
        private const string messageSaveServiceStep = "Service.EditService.MessageStepSaved";
        private const string messageDeleteService = "Service.EditService.MessageDeleted";

        public RESTServiceController(
            IServiceService service,
            IOrganizationService organizationService,
            IServiceManager serviceManager,
            IFintoService fintoService,
            ICommonService commonService,
            ILogger<RESTServiceController> logger) : base(logger)
        {
            this.service = service;
            this.organizationService = organizationService;
            this.serviceManager = serviceManager;
            this.fintoService = fintoService;
            this.commonService = commonService;
        }

        [Route("GetServiceSearch")]
        [HttpGet]
        public IServiceResultWrap GetServiceSearch()
        {
            //service.CloneService();
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = service.GetServiceSearch() },
               new Dictionary<Type, string>());
        }
        [Route("SearchServices")]
        [HttpPost]
        public IServiceResultWrap SearchServices([FromBody] VmServiceSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = service.SearchServices(model) },
                new Dictionary<Type, string>());
        }

        [Route("SearchService")]
        [HttpPost]
        public IServiceResultWrap SearchService([FromBody] VmServiceSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model) { Data = service.SearchRelationService(model) },
                new Dictionary<Type, string>());
        }

        [Route("SearchChannelServices")]
        [HttpPost]
        public IServiceResultWrap SearchChannelServices([FromBody] VmServiceSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap { Data = service.SearchRelationServices(model) },
                new Dictionary<Type, string>());
        }

        [Route("GetAddServiceStep1")]
        [HttpPost]
        public IServiceResultWrap GetAddServiceStep1([FromBody] VmGetServiceStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = service.GetServiceStep1(model)
                },
                new Dictionary<Type, string>());
        }

        [Route("GetAddServiceStep2")]
        [HttpPost]
        public IServiceResultWrap GetAddServiceStep2([FromBody] VmGetServiceStep model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = service.GetServiceStep2(model),
                },
                new Dictionary<Type, string>());
        }

        [Route("GetAddServiceStep3")]
        [HttpPost]
        public IServiceResultWrap GetAddServiceStep3([FromBody] VmGetServiceStep model)
        {
            return serviceManager.CallService(
                 () => new ServiceLocalizedResultWrap (model)
                 {
                     Data = service.GetServiceStep3(model)
                 },
                 new Dictionary<Type, string>());
        }

        [Route("GetFintoTree")]
        [HttpPost]
        public IServiceResultWrap GetFintoTree([FromBody] VmNode model)
        {
            return serviceManager.CallService(
                 () => new ServiceResultWrap() { Data = fintoService.GetFintoTree(model) },
                 new Dictionary<Type, string>());
        }

        [Route("GetFilteredTree")]
        [HttpPost]
        public IServiceResultWrap GetFilteredTree([FromBody] VmGetFilteredTree model)
        {
            return serviceManager.CallService(
                 () => new ServiceResultWrap() { Data = fintoService.GetFilteredTree(model) },
                 new Dictionary<Type, string>());
        }

        [Route("GetFilteredList")]
        [HttpPost]
        public IServiceResultWrap GetFilteredList([FromBody] VmGetFilteredTree model)
        {
            return serviceManager.CallService(
                 () => new ServiceResultWrap() { Data = fintoService.Search(model) },
                 new Dictionary<Type, string>());
        }

        [Route("GetServiceStep4Channeldata")]
        [HttpPost]
        public IServiceResultWrap GetServiceStep4Channeldata([FromBody] VmGetServiceStep model)
        {
            return serviceManager.CallService(
                 () => new ServiceLocalizedResultWrap(model)
                 {
                     Data = service.GetServiceStep4Channeldata(model),
                 },
                 new Dictionary<Type, string>());
        }

        [Route("SaveAllChanges")]
        [HttpPost]
        public IServiceResultWrap SaveAllChanges([FromBody] VmService model)
        {
             return serviceManager.CallService(
                 ()=> new ServiceLocalizedResultWrap(model)
                 {
                     Data = service.AddService(model),
                 },
                 new Dictionary<Type, string>() { { typeof(string), messageAddService } });
        }

        [Route("PublishService")]
        [HttpPost]
        public IServiceResultWrap PublishService([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = service.PublishService(model.Id),
                },
                new Dictionary<Type, string>() { { typeof(string), messagePublishService } });
        }

        [Route("DeleteService")]
        [HttpPost]
        public IServiceResultWrap DeleteService([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = service.DeleteService(model.Id),
                },
                new Dictionary<Type, string>() { { typeof(string), messageDeleteService } });
        }

        [Route("SaveStep1Changes")]
        [HttpPost]
        public IServiceResultWrap SaveStep1Changes([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model.Step1Form)
                {
                    Data = service.SaveStep1Changes(model.Id.Value, model.Step1Form),
                },
                new Dictionary<Type, string>() { { typeof(string), messageSaveServiceStep } });
        }

        [Route("SaveStep2Changes")]
        [HttpPost]
        public IServiceResultWrap SaveStep2Changes([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model.Step2Form)
                {
                    Data = service.SaveStep2Changes(model.Id.Value, model.Step2Form),
                },
                new Dictionary<Type, string>() { { typeof(string), messageSaveServiceStep } });
        }

        [Route("SaveStep3Changes")]
        [HttpPost]
        public IServiceResultWrap SaveStep3Changes([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model.Step3Form)
                {
                    Data = service.SaveStep3Changes(model.Id.Value, model.Step3Form),
                },
                new Dictionary<Type, string>() { { typeof(string), messageSaveServiceStep } });
        }

        [Route("SaveStep4Changes")]
        [HttpPost]
        public IServiceResultWrap SaveStep4Changes([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = service.SaveStep4Changes(model.Id.Value, model.Step4Form),
                },
                new Dictionary<Type, string>() { { typeof(string), messageSaveServiceStep } });
        }

        [Route("GetServiceStatus")]
        [HttpPost]
        public IServiceResultWrap GetServiceStatus([FromBody] VmService model)
        {
            return serviceManager.CallService(
                () => new ServiceLocalizedResultWrap(model)
                {
                    Data = service.GetServiceStatus(model.Id.Value),
                },
                new Dictionary<Type, string>());
        }
    }
}
