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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace PTV.Framework.Extensions
{
    public class ServiceInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public List<String> TargetGroups  { get; set; }
        public List<String> ServiceClasses  { get; set; }
        public List<String> LifeEvents { get; set; }
        public List<String> IndustrialClasses { get; set; }
        public string LanguageCode { get; set; }
    }
    public class Annotation
    {
        public string Value { get; set; }
        public string Uri { get; set; }
    }

    public class RequestBody
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("lang")]
        public string Lang { get; set; }
        [JsonProperty("timeout")]
        public long Timeout { get; set; }
        [JsonProperty("limit")]
        public long Limit { get; set; }
        [JsonProperty("content")]
        public List<ContentField> Content { get; set; }
        [JsonProperty("properties")]
        public List<string> Properties { get; set; }
    }

    public class ContentField
    {
        [JsonProperty("field")]
        public string Field { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class ResponseBody
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("matches")]
        public List<string> Matches { get; set; }
        [JsonProperty("error")]
        public ResponseError Error { get; set; }
    }
    public class ResponseError
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("cause")]
        public string Cause { get; set; }
    }



    public enum AnnotationStates
    {
        Ok,
        Failed,
        EmptyInputReceived
    }

    public enum AnnotationFieldName
    {
        kuvaus, // Body text of the document
        otsikko, // Title of the document
        palvelupiste // Description of the location
    }

    public class AnnotationResult
    {
        public List<Annotation> Annotations { get; set; }
        public string ServiceId { get; set; }
        public AnnotationStates State { get; set; }
    }


    [RegisterService(typeof(AnnotationServiceProvider), RegisterType.Singleton)]
    public class AnnotationServiceProvider
    {
        private readonly ProxyServerSettings proxySettings;
        private readonly bool initialized = false;
        private static readonly string mediaContentType = "application/json";
        private readonly AnnotationServiceConfiguration serviceConnectionConfiguration;
        private readonly ILogger<AnnotationServiceProvider> logger;

        public AnnotationServiceProvider(IOptions<ProxyServerSettings> proxySettings, ILogger<AnnotationServiceProvider> logger, IOptions<AnnotationServiceConfiguration> serviceConnectionConfiguration )
        {
            this.proxySettings = proxySettings.Value;
            this.serviceConnectionConfiguration = serviceConnectionConfiguration.Value;
            this.logger = logger;
            initialized = true;
        }

        private void AddContentStringField(List<ContentField> content, string value)
        {
            if(!string.IsNullOrEmpty(value))
            {
                content.Add(new ContentField() { Field = AnnotationFieldName.kuvaus.ToString(), Value = value });
            }
        }

        private void AddContentListField(List<ContentField> content, List<string> value)
        {
            if (value.Any())
            {
                content.Add(new ContentField() { Field = AnnotationFieldName.kuvaus.ToString(), Value = string.Join(", ", value) });
            }
        }

        private List<ContentField> ProcessRequest(ServiceInfo serviceInfo)
        {
            var content = new List<ContentField>();
            AddContentStringField(content, serviceInfo.Name);
            AddContentStringField(content, serviceInfo.Description);
            AddContentStringField(content, serviceInfo.ShortDescription);
            AddContentListField(content, serviceInfo.LifeEvents);
            AddContentListField(content, serviceInfo.IndustrialClasses);
            AddContentListField(content, serviceInfo.TargetGroups);
            AddContentListField(content, serviceInfo.ServiceClasses);
            return content;
        }

        private AnnotationResult ProccessResponse(ResponseBody response)
        {
            var result = new AnnotationResult();
            if (response.Error != null)
            {
                result.State = AnnotationStates.Failed;
            }
            else
            {
                result.ServiceId = response.SessionId;
                result.State = AnnotationStates.Ok;
                result.Annotations = response.Matches!=null ? response.Matches.Select(x => new Annotation() { Uri = x }).ToList() : new List<Annotation>();
            }

            return result;
        }

        public async Task<AnnotationResult> GetAnnotations(ServiceInfo serviceInfo)
        {
            if (!initialized) return null;
            if (string.IsNullOrEmpty(serviceConnectionConfiguration.Uri)) return null;
            if (serviceInfo == null) return null;

            WebProxy proxy = null;

            if ((proxySettings != null) && (!string.IsNullOrEmpty(proxySettings.Address)))
            {
                string proxyUri = string.Format("{0}:{1}", proxySettings.Address, proxySettings.Port);
                NetworkCredential proxyCreds = new NetworkCredential(proxySettings.UserName, proxySettings.Password);
                proxy = new WebProxy(proxyUri)
                {
                    Credentials = proxyCreds,
                };
            }

            using (HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy
            })
            {
                if (serviceConnectionConfiguration.IgnoreCertificate)
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;
                }
                using (var client = new HttpClient(httpClientHandler) {BaseAddress = new Uri(serviceConnectionConfiguration.Uri) })
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaContentType));
                    var result = new AnnotationResult() { ServiceId = serviceInfo.Id };

                    var requestBody = new RequestBody()
                    {
                        SessionId = serviceInfo.Id.ToString(),
                        Lang = serviceInfo.LanguageCode,
                        Limit = 100,
                        Timeout = 3000,
                        Content = ProcessRequest(serviceInfo),
                        Properties = new List<string>() { "rdfs:label", "skos:narrower", "skos:broader", "skos:related", "skos:inScheme","rdf:type" }
                    };

                    if (!requestBody.Content.Any())
                    {
                        result.State = AnnotationStates.EmptyInputReceived;
                        return result;
                    }

                    try
                    {
                        HttpContent content = new StringContent(
                            JsonConvert.SerializeObject(requestBody),
                            Encoding.UTF8,
                            mediaContentType);
                        var response = await client.PostAsync("", content);

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            logger.LogError($"Annotation failed: {response.Content?.ReadAsStringAsync()?.Result ?? string.Empty}");
                            Console.WriteLine("Annotation failed.");
                            result.State = AnnotationStates.Failed;
                        }
                        else
                        {
                            string text = await response.Content.ReadAsStringAsync();
                            var responseBody = JsonConvert.DeserializeObject<ResponseBody>(text);
                            result = ProccessResponse(responseBody);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Annotation failed.");
                        logger.LogError($"Annotation failed: {e.Message}", e);
                        result.State = AnnotationStates.Failed;
                    }
                    return result;
                }
            }
        }
    }
}
