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
using System;

namespace PTV.Application.OpenApi.Controllers
{
    /// <summary>
    /// PTV Open Api removed versions.
    /// </summary>
    [Route("api/v{version:range(2,5)}/CodeList")]
    [Route("api/v{version:range(2,5)}/Common")]
    [Route("api/v{version:range(2,5)}/Connection")]
    [Route("api/v{version:range(2,5)}/GeneralDescription")]
    [Route("api/v{version:range(2,5)}/Organization")]
    [Route("api/v{version:range(2,5)}/Service")]
    [Route("api/v{version:range(2,5)}/ServiceChannel")]
    [Route("api/v{version:range(2,5)}/ServiceCollection")]
    public class OldVersionsController : Controller
    {
        /// <summary>
        /// Old versions are not supported. Let's return error message.
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [HttpGet("{param1}")]
        [HttpGet("{param1}/{param2}")]
        [HttpGet("{param1}/{param2}/{param3}")]
        [HttpGet("{param1}/{param2}/{param3}/{param4}")]
        [HttpPost("")]
        [HttpPost("{param1}")]
        [HttpPut("{param1}")]
        [HttpPut("{param1}/{param2}")]
        [HttpPut("{param1}/{param2}/{param3}")]
        public IActionResult NotSupported()
        {
            var request = HttpContext.Request;
            var swaggerUrl = request.IsHttps ? "https://" : "http://" + request.Host + "/swagger/ui";
            return NotFound($"This version is not supported! Please check supported versions from swagger: {swaggerUrl}.");
        }
    }
}
