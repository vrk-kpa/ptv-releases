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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Services.Providers;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.QualityAgent;
using PTV.Domain.Model.Models.QualityAgent.Input;
using PTV.Domain.Model.Models.QualityAgent.Output;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.ServiceManager;
using PTV.ToolUtilities;
using IChannelService = PTV.Database.DataAccess.Interfaces.Services.IChannelService;
using IServiceService = PTV.Database.DataAccess.Interfaces.Services.IServiceService;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IQualityCheckService), RegisterType.Transient)]
    internal class QualityCheckService : IQualityCheckService
    {
        private readonly QualityAgentConfiguration configuration;
        private readonly IHostingEnvironment environment;
        private readonly IServiceService apiServiceService;
        private readonly IChannelService apiChannelService;
        private readonly ProxyServerSettings proxyServerSettings;

        public QualityCheckService(IOptions<QualityAgentConfiguration> serviceConnectionConfiguration, IHostingEnvironment environment, IServiceService serviceService, IChannelService apiChannelService, IOptions<ProxyServerSettings> proxySettings)
        {
            this.environment = environment;
            this.apiServiceService = serviceService;
            this.apiChannelService = apiChannelService;
            this.configuration = serviceConnectionConfiguration.Value;
            this.proxyServerSettings = proxySettings.Value;
        }

        public VmQualityAgentResult CheckProperty(string text)
        {
            var (data, statusCode) = CallService(text);

            if (statusCode != HttpStatusCode.OK)
            {
//                Console.WriteLine("Resp headers {0}. ", string.Join(Environment.NewLine, response.Headers.Select(x => $"{x.Key}: {string.Join(";", x.Value)}")));
                return new VmQualityAgentResult {Error = JsonConvert.DeserializeObject(data)};
            }

            if (environment.IsDevelopment())
            {
                Console.WriteLine("*************** INPUT *******************************");
                Console.WriteLine(text);
                Console.WriteLine("************** OUTPUT *******************************");
                Console.WriteLine(statusCode.ToString());
                Console.WriteLine(JsonConvert.DeserializeObject(data));
                Console.WriteLine("**********************************************");
            }
            var qaResult = JsonConvert.DeserializeObject<VmQualityAgentOutput>(data);
            return new VmQualityAgentResult
            {
                Result = qaResult.Results
                    .Where(x => x.Processed != null)
                    .Select(x => new VmQualityAgentValidation(x)).ToList()
            };
        }

        private (string, HttpStatusCode) CallService(string text)
        {
            return Asyncs.HandleAsyncInSync(() => HttpClientWithProxy.UseAsync(proxyServerSettings.OverrideBy(configuration.ProxyServerSettings), async client =>
            {
                client.BaseAddress = new Uri(configuration.Uri);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("username", configuration.UserName);
                client.DefaultRequestHeaders.Add("apikey", configuration.Password);
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var stringContent = new StringContent(text);
                stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var httpResponse = await client.PostAsync("", stringContent);
                var httpData = await httpResponse.Content.ReadAsStringAsync();
                return (httpData, httpResponse.StatusCode);
            }));
        }

        public VmQualityAgentResult CheckEntity(VmQualityAgentEntityHeader model)
        {
            if (!model.UnificRootId.IsAssigned())
            {
                return null;
            }
            switch (model.EntityType)
            {
                case EntityTypeEnum.Service:
                    var service =
                        apiServiceService.GetServiceById(model.UnificRootId, 8, VersionStatusEnum.LatestActive);
                    return CallEntityCheck
                    (
                        service,
                        service.ServiceNames.Select(x => x.Language),
                        model
                    );
                case EntityTypeEnum.Channel:
                    var channel =
                        apiChannelService.GetServiceChannelById(model.UnificRootId, 8, VersionStatusEnum.LatestActive);
                    return CallEntityCheck
                    (
                        channel,
                        channel.ServiceChannelNames.Select(x => x.Language),
                        model
                    );
                default:
                    return null;
            }
        }

        private VmQualityAgentResult CallEntityCheck(IVmEntityBase entity, IEnumerable<string> languages, VmQualityAgentEntityHeader model)
        {
            var inputModel = new VmQualityAgentModel { Profile = configuration.Profiles.TryGet(model.EntityType.ToString())};
            var camelCasePropertyNamesContractResolver = new CamelCasePropertyNamesContractResolver
            {
                NamingStrategy =
                {
                    OverrideSpecifiedNames = false,
                    ProcessDictionaryKeys = false,
                    ProcessExtensionDataNames = false
                }
            };

            JObject inputJson = JObject.FromObject(entity, new JsonSerializer{ ContractResolver = camelCasePropertyNamesContractResolver } );
            inputJson.AddFirst(new JProperty("alternativeId", model.AlternativeId));
            inputJson.AddFirst(new JProperty("status", "Saved"));
            inputModel.Input = inputJson;
            VmQualityAgentResult result = null;
            foreach (var language in languages.Where(l => configuration.Languages != null && configuration.Languages.Contains(l)))
            {
                inputModel.Language = language;
                var errors = CheckProperty(JsonConvert.SerializeObject(inputModel, Formatting.Indented));
                if (result == null)
                {
                    result = errors;
                }
                else
                {
                    result.Result.AddRange(errors.Result);
                }
            }
            return result;   
        }
        
        public HealthCheckResult CallTest()
        {
            if (configuration == null ||
                string.IsNullOrEmpty(configuration.UserName) ||
                string.IsNullOrEmpty(configuration.Password) ||
                string.IsNullOrEmpty(configuration.Uri)) return HealthCheckResult.NotConfigured;
            var inputModel = new VmQualityAgentModel
            {
                Input = new {},
                Language = "fi",
                Profile = configuration.Profiles.TryGet(EntityTypeEnum.Service.ToString())
            };
            var (_, statusCode) = CallService(JsonConvert.SerializeObject(inputModel));
            return statusCode == HttpStatusCode.OK ? HealthCheckResult.Ok : HealthCheckResult.Failed;
        }
    }
}