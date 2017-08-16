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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTV.MapServer.Common;
using PTV.MapServer.ExceptionHandler;
using PTV.Framework.Interfaces;
using PTV.MapServer.Interfaces;
using PTV.MapServer.Models;
using PTV.MapServer.WfsHandlers;
using PTV.MapServer.WmsHandlers;

namespace PTV.MapServer.Controllers
{
    [Authorize]
    public class OwsController : Controller
    {
        private readonly IResolveManager resolveManager;

        public OwsController(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }

        [HttpGet("ows"), Produces("text/xml")]
        public IActionResult OwsGet(HttpGetRequestQuery requestParameters)
        {
            if (HttpContext.Request.Query.Count == 0)
            {
                throw new OwsException(
                    OwsExceptionCodeEnum.NoApplicableCode,
                    HttpContext.Request.Path,
                    "No key-value pairs were given and the request does not contain parsable xml.");
            }

            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .SelectMany(e => e.Value.Errors)
                    .Select(e => e.ErrorMessage);
                throw new OwsException(OwsExceptionCodeEnum.NoApplicableCode, string.Empty, errorMessages.ToArray());
            }

            XDocument resultXml;
            switch (requestParameters.SupportedService)
            {
                case SupportedOcgServiceEnum.Wfs:
                    resultXml = resolveManager.Resolve<WfsHandler>().Handle(requestParameters);
                    break;
                case SupportedOcgServiceEnum.Wms:
                    resultXml = resolveManager.Resolve<WmsHandler>().Handle(requestParameters);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Content(resultXml.ToString(), "text/xml", Encoding.UTF8);
        }

        [HttpPost("ows"), Produces("text/xml")]
        public IActionResult OwsPost([FromBody] XElement requestXml)
        {
            if (requestXml == null) return BadRequest("XML request is not valid.");

            var requestedService = requestXml.Attributes().FirstOrDefault(a => string.Compare(a.Name.LocalName, "service", StringComparison.OrdinalIgnoreCase) == 0)?.Value;
            RequestValidation.ValidateKnownService(requestedService);

            var service = RequestValidation.ValidateSupportedService(requestedService);
            XDocument resultXml;
            switch (service)
            {
                case SupportedOcgServiceEnum.Wfs:
                    resultXml = resolveManager.Resolve<WfsHandler>().Handle(requestXml);
                    break;
                case SupportedOcgServiceEnum.Wms:
                    resultXml = resolveManager.Resolve<WmsHandler>().Handle(requestXml);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Content(resultXml.ToString(), "text/xml", Encoding.UTF8);
        }
    }
}
