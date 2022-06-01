using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Next;

namespace PTV.Application.Api.Controllers.Next
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/next/region")]
    public class RegionController : BaseController
    {
        private readonly IRegionQueries regionQueries;

        public RegionController(ILogger<RegionController> logger, IRegionQueries regionQueries) 
            : base(logger)
        {
            this.regionQueries = regionQueries;
        }

        [HttpGet("municipalities")]
        public IActionResult GetMunicipalities()
        {
            var result = regionQueries.GetMunicipalities();
            return Ok(result);
        }

        [HttpGet("businessregions")]
        public IActionResult GetBusinessRegions()
        {
            var result = regionQueries.GetBusinessRegions();
            return Ok(result);
        }

        [HttpGet("hospitalregions")]
        public IActionResult GetHospitalRegions()
        {
            var result = regionQueries.GetHospitalRegions();
            return Ok(result);
        }

        [HttpGet("provinces")]
        public IActionResult GetProvinces()
        {
            var result = regionQueries.GetProvinces();
            return Ok(result);
        }

        [HttpGet("check")]
        public IActionResult Check()
        {
            var result = regionQueries.GetLastUpdate();
            return Ok(result);
        }
    }
}