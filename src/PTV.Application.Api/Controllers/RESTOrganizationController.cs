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

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Models;

namespace PTV.Application.Api.Controllers
{
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    [Route("api/organization")]
    [Controller]
    public class RESTOrganizationController : RESTBaseController
    {
        private IOrganizationService organizationService;
        private IMunicipalityService municipalityService;
        private IServiceManager serviceManager;

        private const string messageOrganizationStepSave = "Organization.OrganizationStep.MessageSaved";
        private const string messageArgumentDuplicityException = "Organization.Exception.DuplicityCheck";
        private const string messageAddOrganization = "Organization.AddOrganization.MessageSaved";
        private const string messageDeleteOrganization = "Organization.EditOrganization.MessageDeleted";
        private const string messagePublishOrganization = "Organization.EditOrganization.MessagePublished";


        public RESTOrganizationController(IOrganizationService organizationService, IMunicipalityService municipalityService, IServiceManager serviceManager, ILogger<RESTOrganizationController> logger) : base(logger)
        {
            this.organizationService = organizationService;
            this.municipalityService = municipalityService;
            this.serviceManager = serviceManager;
        }

        [Route("GetOrganizationSearch")]
        [HttpGet]
        public IServiceResultWrap GetOrganizationSearch()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap() { Data = organizationService.GetOrganizationSearch() },
               new Dictionary<Type, string>());
        }

        [Route("SearchOrganizations")]
        [HttpPost]
        public IServiceResultWrap SearchOrganizations([FromBody] VmOrganizationSearch model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = organizationService.SearchOrganizations(model) },
                new Dictionary<Type, string>());
        }

        [Route("GetOrganizationStep1")]
        [HttpPost]
        public IServiceResultWrap GetOrganizationStep1([FromBody] VmGetOrganizationStep model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = organizationService.GetOrganizationStep1(model.OrganizationId) },
                new Dictionary<Type, string>());
        }

        [Route("SaveAllChanges")]
        [HttpPost]
        public IServiceResultWrap SaveAllChanges([FromBody] VmOrganizationModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = organizationService.AddApiOrganization(model) },
                new Dictionary<Type, string>()
                {
                    { typeof(string), messageAddOrganization },
                    { typeof(ArgumentException), messageArgumentDuplicityException }
                }
            );
        }

        [Route("SaveStep1Changes")]
        [HttpPost]
        public IServiceResultWrap SaveStep1Changes([FromBody] VmOrganizationModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = organizationService.SaveOrganizationStep1(model) },
                new Dictionary<Type, string>()
                {
                                { typeof(string), messageOrganizationStepSave },
                                { typeof(ArgumentException), messageArgumentDuplicityException }
                }
            );
        }


        [Route("GetOrganizationStatus")]
        [HttpPost]
        public IServiceResultWrap GetOrganizationStatus([FromBody] VmOrganizationModel model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = organizationService.GetOrganizationStatus(model.Id.Value) },
                new Dictionary<Type, string>());
        }


        [Route("GetMunicipalities")]
        [HttpPost]
        public IServiceResultWrap GetMunicipalities([FromBody] string searchedCode)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = municipalityService.GetMunicipalities(searchedCode) },
                new Dictionary<Type, string>());
        }

        [Route("PublishOrganization")]
        [HttpPost]
        public IServiceResultWrap PublishOrganization([FromBody] VmOrganizationModelBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = organizationService.PublishOrganization(model.Id) },
                new Dictionary<Type, string>() { { typeof(string), messagePublishOrganization } });
        }

        [Route("DeleteOrganization")]
        [HttpPost]
        public IServiceResultWrap DeleteOrganization([FromBody] VmOrganizationModelBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = organizationService.DeleteOrganization(model.Id) },
                new Dictionary<Type, string>() { { typeof(string), messageDeleteOrganization } });
        }

        [Route("GetOrganizations")]
        [HttpPost]
        public IServiceResultWrap GetOrganizations([FromBody] string searchText)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap() { Data = organizationService.GetOrganizations(searchText)},
                new Dictionary<Type, string>());
        }
    }
}
