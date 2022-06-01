using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Framework.Extensions;

namespace PTV.Application.Api.Controllers.Next
{
    /// <summary>
    /// Controller for getting all enum types individually.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/next")]
    public class EnumController : BaseController
    {
        private readonly IEnumQueries enumQueries;
        private readonly IConstantQueries constantQueries;
        private const string OntologyRoute = "ontology";
        private const string IndustrialClassRoute = "industrialclass";
        private const string DigitalAuthorizationRoute = "digitalauthorization";
        private const string LifeEventRoute = "lifeevent";
        private const string ServiceClassRoute = "serviceclass";
        private const string TargetGroupRoute = "targetgroup";
        private const string HolidayRoute = "holiday";
        private const string ServiceNumberRoute = "servicenumber";
        private const string LanguageRoute = "language";
        private const string DialCodeRoute = "dialcode";
        private const string ConstantRoute = "constant";
        private const string PostalCodeRoute = "postalcode";
        private const string CountryRoute = "country";

        public EnumController(ILogger<EnumController> logger, IEnumQueries enumQueries, IConstantQueries constantQueries) 
            : base(logger)
        {
            this.enumQueries = enumQueries;
            this.constantQueries = constantQueries;
        }

        /// <summary>
        /// Gets all valid ontology terms with their parents, children and exact match URIs.
        /// </summary>
        /// <returns></returns>
        [HttpGet(OntologyRoute + "/all")]
        public IActionResult GetOntologyAll()
        {
            var result = enumQueries.GetOntologyAll();
            return Ok(result);
        }

        /// <summary>
        /// Gets a single ontology term with its parents, children and exact match URIs.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost(OntologyRoute + "/getByIds")]
        public IActionResult GetOntologyById([FromBody]List<Guid> ids)
        {
            var result = enumQueries.GetOntologyById(ids).ToList();
            return Ok(result);
        }

        /// <summary>
        /// Tries to find suitable ontology terms based on provided expression and language.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        [HttpGet(OntologyRoute + "/search")]
        public IActionResult SearchOntology([FromQuery] string expression, [FromQuery] string language)
        {
            var result = enumQueries.SearchOntology(expression, language);
            return Ok(result);
        }

        [HttpGet(OntologyRoute + "/search-by-name")]
        public IActionResult SearchByName([FromQuery] string name, [FromQuery] string language)
        {
            var result = enumQueries.SearchByName(name, language);
            return Ok(result);
        }
        
        /// <summary>
        /// Gets the last update time of the ontology terms.
        /// </summary>
        /// <returns></returns>
        [HttpGet(OntologyRoute + "/check")]
        public IActionResult CheckOntology()
        {
            var result = enumQueries.CheckOntology();
            return Ok(result);
        }

        [HttpPost(OntologyRoute + "/annotations")]
        public IActionResult GetAnnotations([FromBody] ServiceInfo model)
        {
            var result = enumQueries.GetAnnotations(model);
            switch (result.State)
            {
                case AnnotationStates.Ok:
                    return Ok(result.Ontologies);
                case AnnotationStates.EmptyInputReceived:
                    return BadRequest();
                default:
                    return NotFound();
            }    
        }

        /// <summary>
        /// Gets all valid industrial classes.
        /// </summary>
        /// <returns></returns>
        [HttpGet(IndustrialClassRoute + "/all")]
        public IActionResult GetIndustrialClassAll()
        {
            var result = enumQueries.GetIndustrialClassAll();
            return Ok(result);
        }
        
        /// <summary>
        /// Gets the last update time of the industrial classes.
        /// </summary>
        /// <returns></returns>
        [HttpGet(IndustrialClassRoute + "/check")]
        public IActionResult CheckIndustrialClass()
        {
            var result = enumQueries.CheckIndustrialClass();
            return Ok(result);
        }

        /// <summary>
        /// Gets all valid digital authorizations.
        /// </summary>
        /// <returns></returns>
        [HttpGet(DigitalAuthorizationRoute + "/all")]
        public IActionResult GetDigitalAuthorizationAll()
        {
            var result = enumQueries.GetDigitalAuthorizationAll();
            return Ok(result);
        }
        
        /// <summary>
        /// Gets the last update time of the digital authorizations.
        /// </summary>
        /// <returns></returns>
        [HttpGet(DigitalAuthorizationRoute + "/check")]
        public IActionResult CheckDigitalAuthorization()
        {
            var result = enumQueries.CheckDigitalAuthorization();
            return Ok(result);
        }

        /// <summary>
        /// Gets all valid life events.
        /// </summary>
        /// <returns></returns>
        [HttpGet(LifeEventRoute + "/all")]
        public IActionResult GetLifeEventAll()
        {
            var result = enumQueries.GetLifeEventAll();
            return Ok(result);
        }
        
        /// <summary>
        /// Gets the last update time of the life events.
        /// </summary>
        /// <returns></returns>
        [HttpGet(LifeEventRoute + "/check")]
        public IActionResult CheckLifeEvent()
        {
            var result = enumQueries.CheckLifeEvent();
            return Ok(result);
        }

        /// <summary>
        /// Gets all valid service classes.
        /// </summary>
        /// <returns></returns>
        [HttpGet(ServiceClassRoute + "/all")]
        public IActionResult GetServiceClassAll()
        {
            var result = enumQueries.GetServiceClassAll();
            return Ok(result);
        }
        
        /// <summary>
        /// Gets the last update time of the service classes.
        /// </summary>
        /// <returns></returns>
        [HttpGet(ServiceClassRoute + "/check")]
        public IActionResult CheckServiceClass()
        {
            var result = enumQueries.CheckServiceClass();
            return Ok(result);
        }

        /// <summary>
        /// Gets all valid target groups.
        /// </summary>
        /// <returns></returns>
        [HttpGet(TargetGroupRoute + "/all")]
        public IActionResult GetTargetGroupAll()
        {
            var result = enumQueries.GetTargetGroupAll();
            return Ok(result);
        }
        
        /// <summary>
        /// Gets the last update time of the target groups.
        /// </summary>
        /// <returns></returns>
        [HttpGet(TargetGroupRoute + "/check")]
        public IActionResult CheckTargetGroup()
        {
            var result = enumQueries.CheckTargetGroup();
            return Ok(result);
        }

        /// <summary>
        /// Gets all valid holidays.
        /// </summary>
        /// <returns></returns>
        [HttpGet(HolidayRoute + "/all")]
        public IActionResult GetHolidayAll()
        {
            var result = enumQueries.GetHolidayAll();
            return Ok(result);
        }
        
        /// <summary>
        /// Gets the last update time of the holidays.
        /// </summary>
        /// <returns></returns>
        [HttpGet(HolidayRoute + "/check")]
        public IActionResult CheckHoliday()
        {
            var result = enumQueries.CheckHoliday();
            return Ok(result);
        }

        /// <summary>
        /// Gets all valid service numbers.
        /// </summary>
        /// <returns></returns>
        [HttpGet(ServiceNumberRoute + "/all")]
        public IActionResult GetServiceNumberAll()
        {
            var result = enumQueries.GetServiceNumberAll();
            return Ok(result);
        }
        
        /// <summary>
        /// Gets the last update time of the service numbers.
        /// </summary>
        /// <returns></returns>
        [HttpGet(ServiceNumberRoute + "/check")]
        public IActionResult CheckServiceNumber()
        {
            var result = enumQueries.CheckServiceNumber();
            return Ok(result);
        }

        /// <summary>
        /// Gets all valid languages.
        /// </summary>
        /// <returns></returns>
        [HttpGet(LanguageRoute + "/all")]
        public IActionResult GetLanguageAll()
        {
            var result = enumQueries.GetLanguageAll();
            return Ok(result);
        }
        
        /// <summary>
        /// Gets the last update time of the languages.
        /// </summary>
        /// <returns></returns>
        [HttpGet(LanguageRoute + "/check")]
        public IActionResult CheckLanguage()
        {
            var result = enumQueries.CheckLanguage();
            return Ok(result);
        }

        /// <summary>
        /// Gets all valid dial codes.
        /// </summary>
        /// <returns></returns>
        [HttpGet(DialCodeRoute + "/all")]
        public IActionResult GetDialCodeAll()
        {
            var result = enumQueries.GetDialCodeAll();
            return Ok(result);
        }
        
        /// <summary>
        /// Gets the last update time of the dial codes.
        /// </summary>
        /// <returns></returns>
        [HttpGet(DialCodeRoute + "/check")]
        public IActionResult CheckDialCode()
        {
            var result = enumQueries.CheckDialCode();
            return Ok(result);
        }

        [HttpGet(ConstantRoute + "/texts")]
        public IActionResult GetConstantTexts()
        {
            var result = constantQueries.GetTexts();
            return Ok(result);
        }

        [HttpGet(PostalCodeRoute + "/all")]
        public IActionResult GetPostalCode()
        {
            var result = enumQueries.GetAllPostalCodes();
            return Ok(result);
        }

        [HttpGet(PostalCodeRoute + "/check")]
        public IActionResult CheckPostalCode()
        {
            var result = enumQueries.CheckPostalCode();
            return Ok(result);
        }

        [HttpGet(CountryRoute + "/all")]
        public IActionResult GetCountries()
        {
            var result = enumQueries.GetAllCountries();
            return Ok(result);
        }

        [HttpGet(CountryRoute + "/check")]
        public IActionResult CheckCountries()
        {
            var result = enumQueries.CheckCountries();
            return Ok(result);
        }
    }
}