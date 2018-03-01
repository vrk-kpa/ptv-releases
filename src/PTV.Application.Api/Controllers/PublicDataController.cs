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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services.V2;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// Relaying controller for map calls
    /// </summary>
    [Microsoft.AspNetCore.Mvc.Route("publicData")]
    [AllowAnonymous]
    [Controller]
    public class PublicDataController
    {
        private ILogger<PublicDataController> logger;
        private readonly ITranslationService translationService;

        /// <summary>
        /// Public data controller
        /// </summary>
        /// <param name="translationService"></param>
        /// <param name="logger"></param>
        public PublicDataController(ITranslationService translationService, ILogger<PublicDataController> logger)
        {
            this.logger = logger;
            this.translationService = translationService;
        }

        /// <summary>
        /// Get Translation data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Route("GetTranslationData")]
        //[HttpGet]
        [HttpGet("GetTranslationData/{id}")]
        [AllowAnonymous]
        public string GetTranslationData(Guid id)
        {
            return translationService.GetTranslationDataJson(id);
        }
    }
}
