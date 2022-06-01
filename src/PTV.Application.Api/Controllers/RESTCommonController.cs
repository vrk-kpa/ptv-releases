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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Logic;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Security;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Mass;
using PTV.Framework.Enums;
using PTV.Framework.Exceptions.DataAccess;
using PTV.Framework.Extensions;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// REST controller for actions related to all "Common" stuff
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/common")]
    [Controller]
    public class RESTCommonController : RESTBaseController
    {
        private readonly ILogger logger;
        private readonly IAddressService addressService;
        private readonly IServiceManager serviceManager;
        private readonly ILanguageService languageService;
        private readonly ICommonService commonService;
        private readonly IFintoService fintoService;
        private readonly IDialCodeService dialCodeService;
        private readonly ICountryService countryService;
        private readonly IPostalCodeService postalCodeService;
        private readonly IUserOrganizationService userOrganizationService;
        private readonly IUserInfoService userInfoService;
        private readonly ISearchService searchService;
        private readonly IStreetService streetService;
        private readonly IEnumService enumService;

        /// <summary>
        /// Coordinates cannot get
        /// </summary>
        public const string CoordinatesConnectionFailed = "Address.Service.Connection.Failed";

        private const string MessageAnnotationSuccess = "Service.Annotations.MessageSucceeded";
        private const string MessageAnnotationNotFound = "Service.Annotations.TermsNotFound";

        /// <summary>
        /// Message ui used for informing user about existing modified version of entity
        /// </summary>
        public const string MessageUnableEditLocked = "Common.EntityException.UnableEdit";

        /// <summary>
        /// Message ui used for informing user about requirement that at least one language version must be visible
        /// </summary>
        public const string MessageNotVisibleLanguage = "Common.EntityException.NotVisibleLanguage";

        /// <summary>
        /// Message ui used for informing user about not possibility to publish entity to Modified because another Modified already exists
        /// </summary>
        public const string MessagePublishModifiedExists = "Common.EntityException.PublishModifiedExists";

        /// <summary>
        /// Message ui used for informing user about not possibility to publish entity to Modified because another Modified already exists
        /// </summary>
        public const string MessageWithdrawModifiedExists = "Common.EntityException.WithdrawModifiedExists";

        /// <summary>
        /// Message ui used for informing user about not possibility to publish entity to Modified because another Modified already exists
        /// </summary>
        public const string MessageRestoreModifiedExists = "Common.EntityException.RestoreModifiedExists";

        /// <summary>
        /// Message ui used for informing user about not possibility to remove last published language
        /// </summary>
        public const string MessageDeleteLanguage = "Common.Exception.ArchiveLanguage";
        /// <summary>
        /// Message ui used for informing user about not possibility to remove last published language
        /// </summary>
        public const string MessageRestoreLanguage = "Common.Exception.RestoreLanguage";
        /// <summary>
        /// Message ui used for informing user about not possibility to schedule publish entity because not all mandatory field are filled
        /// </summary>
        public const string MessageSchedulePublishError = "Common.EntityException.SchedulePublishError";


        /// <summary>
        /// Constructor of Common controller
        /// </summary>
        /// <param name="logger">logger component to support logging - injected by framework</param>
        /// <param name="addressService">address service responsible for operation related to addresses - injected by framework</param>
        /// <param name="serviceManager">manager responsible for wrapping of individual service call to UI output format - injected by framework</param>
        /// <param name="languageService">language service responsible for operation related to languages - injected by framework</param>
        /// <param name="commonService">common service responsible for operation related to common stuff - injected by framework</param>
        /// <param name="dialCodeService">dial code service responsible for operation related to dial codes - injected by framework</param>
        /// <param name="fintoService">finto service responsible for operation related to finto stuff - injected by framework</param>
        /// <param name="countryService">country service responsible for operation related to countries - injected by framework</param>
        /// <param name="userOrganizationService">user organization service - injected by framework</param>
        /// <param name="postalCodeService">postal code service responsible for operation related to postal codes - injected by framework</param>
        /// <param name="userInfoService">user info service - gets information about logged user</param>
        /// <param name="searchService">search service - gets front page search result</param>
        /// <param name="streetService">street service working with cls addresses</param>
        /// <param name="enumService">enum service to work with cached data</param>
        public RESTCommonController(
            ILogger<RESTCommonController> logger,
            IAddressService addressService,
            IServiceManager serviceManager,
            ILanguageService languageService,
            ICommonService commonService,
            IDialCodeService dialCodeService,
            IFintoService fintoService,
            ICountryService countryService,
            IUserOrganizationService userOrganizationService,
            IPostalCodeService postalCodeService,
            IUserInfoService userInfoService,
            ISearchService searchService,
            IStreetService streetService,
            IEnumService enumService) : base(logger)
        {
            this.logger = logger;
            this.addressService = addressService;
            this.serviceManager = serviceManager;
            this.languageService = languageService;
            this.commonService = commonService;
            this.fintoService = fintoService;
            this.dialCodeService = dialCodeService;
            this.countryService = countryService;
            this.userOrganizationService = userOrganizationService;
            this.postalCodeService = postalCodeService;
            this.userInfoService = userInfoService;
            this.searchService = searchService;
            this.streetService = streetService;
            this.enumService = enumService;
        }

        /// <summary>
        /// Get data for front page search form
        /// </summary>
        /// <returns>data needed for search form like publishing statuses, types etc.</returns>
        [Route("GetFrontPageSearch")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetFrontPageSearch()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap { Data = commonService.GetFrontPageSearch() },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get all enum types
        /// </summary>
        /// <returns>enum types needed by application.</returns>
        [Route("GetEnumTypes")]
        [HttpPost]
        // [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetEnumTypes([FromBody]VmUserInfoBase userInfo)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = enumService.GetEnumTypes(userInfo) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get user roles for organizations
        /// </summary>
        /// <returns>enum types needed by application.</returns>
        [Route("GetUserOrganizationsAndRoles")]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        [HttpPost]
        public IServiceResultWrap GetUserOrganizationsAndRoles()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = userOrganizationService.GetAllUserOrganizationsAndRoles() },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get user information
        /// </summary>
        /// <returns>enum types needed by application.</returns>
        [Route("GetUserInfo")]
//        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        [HttpPost]
        public IServiceResultWrap GetUserInfo()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = userInfoService.GetUserInfo() },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get organization enum
        /// </summary>
        /// <returns>enum types needed by application.</returns>
        [Route("GetOrganizationEnum")]
        [HttpGet]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetOrganizationEnum()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = commonService.GetOrganizationEnum() },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the address for given model
        /// </summary>
        /// <param name="model">input parameters to indicate correct address</param>
        /// <returns>address for given model</returns>
        [Route("GetAddress")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAddress([FromBody] VmGetCoordinatesForAddressIn model)
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap { Data = addressService.GetAddress(model) },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets the coordinates for address in given model
        /// </summary>
        /// <param name="model">input parameters to indicate correct address</param>
        /// <returns>coordinates for given model</returns>
        [Route("GetCoordinatesForAddress")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetCoordinatesForAddress([FromBody] VmGetCoordinatesForAddressIn model)
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap { Data = addressService.GetAddressWithCoordinates(model) },
               new Dictionary<Type, string>
               {
                   { typeof(CoordinateException), CoordinatesConnectionFailed }
               });
        }

        /// <summary>
        /// Gets the all languages, that can be used for storage of content
        /// </summary>
        /// <returns>languages available for content</returns>
        [Route("GetTranslationLanguages")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetTranslationLanguages()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap { Data = languageService.GetTranslationLanguages() },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets all available publishing statues within the application
        /// </summary>
        /// <returns>publishing statuses</returns>
        [Route("GetPublishingStatuses")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetPublishingStatuses()
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap { Data = enumService.GetPublishingStatuses() },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets all supported dial codes used through the application ()
        /// </summary>
        /// <param name="searchedCode"></param>
        /// <returns>dial codes </returns>
        [Route("GetDialCodes")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetDialCodes([FromBody] string searchedCode)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = dialCodeService.GetDialCodes(searchedCode) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get postal code by its id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetDialCodeById")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetDialCodeById([FromBody] VmEntityBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = dialCodeService.GetDialCode(model.Id ?? Guid.Empty) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets postal codes for given search code/municipality name
        /// </summary>
        /// <param name="searchedCode">postal code or municipality name</param>
        /// <returns>postal codes</returns>
        [Route("GetPostalCodes")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetPostalCodes([FromBody] string searchedCode)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = postalCodeService.GetPostalCodes(searchedCode) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get postal code by its ids
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetPostalCodeById")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetPostalCodeById([FromBody] VmEntityBase model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = postalCodeService.GetPostalCode(model.Id ?? Guid.Empty) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Searches street names corresponding to given language and searched expression.
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("GetStreets")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetStreets([FromBody] VmStreetSearch searchModel)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = streetService.GetStreets(
                        searchModel.SearchedStreetName,
                        searchModel.PostalCodeId,
                        searchModel.LanguageId,
                        searchModel.Offset)
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets all supported countries used through the application ()
        /// </summary>
        /// <param name="searchedCode"></param>
        /// <returns>countries</returns>
        [Route("GetCountries")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetCountries([FromBody] string searchedCode)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = countryService.GetCountries(searchedCode) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets Ontology term from annotation service for given model
        /// </summary>
        /// <param name="model">ServiceInfo model</param>
        /// <returns>ontology terms returned from annotation tool</returns>
        [Route("GetAnnotations")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAnnotations([FromBody] ServiceInfo model)
        {
            var texts = new Dictionary<Type, string>
            {
                {typeof(string), MessageAnnotationSuccess}
            };
            return serviceManager.CallService(
               () =>
               {
                   VmAnnotations annotations = new VmAnnotations();
                   try
                   {
                       annotations = fintoService.GetAnnotations(model);
                       if (annotations.AnnotationOntologyTerms.Count == 0)
                       {
                           texts[typeof(string)] = MessageAnnotationNotFound;
                       }
                   }
                   catch(Exception ex)
                   {
                       logger.LogError(ex, "An exception was encountered while fetching annotations.");
                   }
               
                   return new ServiceResultWrap {Data = annotations};
               },
               texts
               );
        }
        /// <summary>
        /// Gets Ontology term from annotation service for given model
        /// </summary>
        /// <param name="model">ServiceInfo model</param>
        /// <returns>ontology terms returned from annotation tool</returns>
        [Route("LoadAnnotations")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap LoadAnnotations([FromBody] VmGuidList model)
        {
          return serviceManager.CallService(
              () => new ServiceResultWrap { Data = fintoService.GetAnnotations(model.Data) },
              new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets parent hierarchy for ontology term
        /// </summary>
        /// <param name="model">VmGetFilteredTree model</param>
        /// <returns>ontology term hierarchy from annotation tool</returns>
        [Route("GetAnnotationHierarchy")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAnnotationHierarchy([FromBody] VmGetFilteredTree model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap {Data = fintoService.GetAnnotationHierarchy(model)},
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets parent hierarchy for ontology term
        /// </summary>
        /// <param name="model">VmGetFilteredTree model</param>
        /// <returns>ontology term hierarchy from annotation tool</returns>
        [Route("GetTypedData")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetTypedData([FromBody]  VmStringList model)
        {
            return serviceManager.CallService(
               () => new ServiceResultWrap { Data = commonService.GetTypedData(model.Strings) },
               new Dictionary<Type, string>());
        }

        /// <summary>
        /// Save Environment Instructions
        /// </summary>
        /// <param name="model">VmEnvironmentInstructionsIn model</param>
        [Route("SaveEnvironmentInstructions")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppWrite)]
        public IServiceResultWrap SaveEnvironmentInstructions([FromBody]  VmEnvironmentInstructionsIn model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = commonService.SaveEnvironmentInstructions(model) },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get Environment Instructions
        /// </summary>
        /// <return> environment instructions</return>
        [Route("GetEnvironmentInstructions")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetEnvironmentInstructions()
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap { Data = commonService.GetEnvironmentInstructions() },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Gets organization area information for service
        /// </summary>
        /// <param name="model">model containing id of the service and id of organization</param>
        /// <returns>area information</returns>
        [Route("GetDefaultAreaInformation")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetAreaInformation([FromBody] VmGetAreaInformation model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = commonService.GetAreaInformationForOrganization(model),
                },
                new Dictionary<Type, string>());
        }

        /// <summary>
        /// Get information of entities about last modified version
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetLastModifiedInfo")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public IServiceResultWrap GetLastModifiedInfo([FromBody] VmMassDataModel<VmRestoringModel> model)
        {
            return serviceManager.CallService(
                () => new ServiceResultWrap
                {
                    Data = commonService.GetEntityLastModifiedInfos(model),
                },
                new Dictionary<Type, string>());
        }


        /// <summary>
        /// Search entities for given criteria
        /// </summary>
        /// <param name="model">model containing search criteria</param>
        /// <returns>found entities</returns>
        [Route("SearchEntities")]
        [HttpPost]
        [AccessRightRequirement(AccessRightEnum.UiAppRead)]
        public async Task<IServiceResultWrap> SearchEntities([FromBody] VmEntitySearch model)
        {
            return await serviceManager.CallServiceInSync(
                () => new ServiceResultWrap
                {
                    Data = searchService.SearchEntities(model)
                },
                new Dictionary<Type, string>());
        }
    }
}
