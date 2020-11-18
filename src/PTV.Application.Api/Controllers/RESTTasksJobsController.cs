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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// RESTTasksJobsController
    /// </summary>

    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/tasksJobs")]
    [AllowAnonymous]
    [AccessRightRequirement(AccessRightEnum.UiAppRead)]
    [ClaimRoleRequirement(UserRoleEnum.Eeva)]
    [Controller]
    public class RESTTasksJobsController : Controller
    {
        private string getMethodPath = "{0}/{1}";
        //private string postMethodPath = "{0}";
        private readonly ApplicationConfiguration configuration;
        
        /// <summary>
        /// Constructor of TasksJobs controller
        /// </summary>
        /// <param name="configuration">configuration component - injected by framework</param>
        public RESTTasksJobsController(ApplicationConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        /// <summary>
        /// Job list
        /// </summary>
        /// <returns></returns>
        [HttpGet("JobList")]
        public async Task<IGeneralJsonResultWrap>  JobList()
        {
            return await ForwardRequest();
        }

        /// <summary>
        /// Force job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("ForceJob/{jobType}")]
        public async Task<IGeneralJsonResultWrap>  ForceJob(string jobType)
        {
            return await ForwardRequest(jobType);
        }
        
        /// <summary>
        /// Force job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("GetJob/{jobType}")]
        public async Task<IGeneralJsonResultWrap>  GetJob(string jobType)
        {
            return await ForwardRequest(jobType);
        }
        
        /// <summary>
        /// Get logs
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLog")]
        public async Task<FileResult> GetLog()
        {
            return await ForwardFileGetRequest();
        }

        /// <summary>
        /// Resume job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("ResumeJob/{jobType}")]
        public async Task<IGeneralJsonResultWrap>  ResumeJob(string jobType)
        {
            return await ForwardRequest(jobType);
        }
        
        /// <summary>
        /// Pause job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("PauseJob/{jobType}")]
        public async Task<IGeneralJsonResultWrap>  PauseJob(string jobType)
        {
            return await ForwardRequest(jobType);
        }

        /// <summary>
        /// Restart job
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        [HttpGet("RestartJob/{jobType}")]
        public async Task<IGeneralJsonResultWrap> RestartJob(string jobType)
        {
            return await ForwardRequest(jobType);
        }

        /// <summary>
        /// Restart all jobs
        /// </summary>
        /// <returns></returns>
        [HttpGet("RestartAll")]
        public async Task<IGeneralJsonResultWrap> RestartAll()
        {
            return await ForwardRequest();
        }
        
        /// <summary>
        /// Shutdown scheduler
        /// </summary>
        [HttpGet("DisableScheduler")]
        public async void DisableScheduler()
        {
            await ForwardRequest();
        }
        
        /// <summary>
        /// Shutdown scheduler
        /// </summary>
        [HttpGet("EnableScheduler")]
        public async void EnableScheduler()
        {
            await ForwardRequest();
        }

        private async Task<IGeneralJsonResultWrap> ForwardRequest()
        {
            return await ForwardRequest(string.Empty);
        }

        private async Task<IGeneralJsonResultWrap> ForwardRequest<TIn>(TIn data)
        {
            var httpMethod = HttpContext.Request.Method;
            
            var action = ControllerContext.RouteData.Values["action"].ToString();
            var targetApi = configuration.GetQuartzApiUrl();
            
            return await PtvHttpClient.UseAsync(async http =>
            {
                Console.WriteLine($"XXX ForwardRequest - httpMethod: {httpMethod}, action: {action}, data: {data}, GET format: {string.Format(targetApi,string.Format(getMethodPath, action, data))},  POST format: {string.Format(targetApi, action)}");
                
                HttpResponseMessage response;
                switch (httpMethod)
                {
                    case "GET" : response= await http.GetAsync(string.Format(targetApi,string.Format(getMethodPath, action, data)));
                        break;
                    case "POST" :
                        var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(serialized);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        response = await http.PostAsync(string.Format(targetApi, action), byteContent);
                        break;
                    default: throw new Exception("Unsupported method type.");
                }
                return new GeneralJsonResultWrap {Data =  JObject.Parse(await response.Content.ReadAsStringAsync())};
            });
        }
        private async Task<FileResult> ForwardFileGetRequest()
        {
            var httpMethod = HttpContext.Request.Method;
            
            var action = ControllerContext.RouteData.Values["action"].ToString();
            var targetApi = configuration.GetQuartzApiUrl();

            Console.WriteLine($"XXX ForwardRequest - GetQuartzApiUrl - targetApi:{targetApi}");
                           
            return await PtvHttpClient.UseAsync(async http =>
            {
                Console.WriteLine($"XXX ForwardRequest - httpMethod: {httpMethod}, action: {action}");
                 
                byte[] response;
                switch (httpMethod)
                {
                    case "GET" : response= await http.GetByteArrayAsync(string.Format(targetApi, action));
                        break;
                   default: throw new Exception("Unsupported method type.");
                }

                return
                    File(response, "application/text");
            });
        }
    }
}
