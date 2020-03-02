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
using PTV.ToolUtilities;

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
        [JsonProperty("@graph")]
        public List<Graph> Graph { get; set; }
    }
    public class Graph
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
        [JsonProperty("http://www.w3.org/2004/02/skos/core#broader")]
        public List<Graph> Broader { get; set; }
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
                content.Add(new ContentField { Field = AnnotationFieldName.kuvaus.ToString(), Value = value });
            }
        }

        private void AddContentListField(List<ContentField> content, List<string> value)
        {
            if (value.Any())
            {
                content.Add(new ContentField { Field = AnnotationFieldName.kuvaus.ToString(), Value = string.Join(", ", value) });
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
                FilterMatches(response);
                result.ServiceId = response.SessionId;
                result.State = AnnotationStates.Ok;
                result.Annotations = response.Matches!=null ? response.Matches.Select(x => new Annotation { Uri = x }).ToList() : new List<Annotation>();
            }

            return result;
        }
        private void FilterMatches(ResponseBody response)
        {
            if (response.Graph != null)
            {
                var broaders = response.Graph
                    .Where(x => x.Broader != null && x.Broader.Count > 0)
                    .SelectMany(x => x.Broader)
                    .Select(x => x.Id).ToList();
                response.Matches = response.Matches.Where(x => !broaders.Contains(x)).ToList();
            }
        }

        public HealthCheckResult CallTestAnnotation()
        {
            var result = GetAnnotations(new ServiceInfo
            {
                    Id = Guid.NewGuid().ToString(),
                   Name = "Test",
                   Description = "Test",
                   ShortDescription = "Test",
                   LanguageCode = "fi",
                   ServiceClasses = new List<string> { "Test" },
                   IndustrialClasses = new List<string> { "Test" },
                   LifeEvents = new List<string> { "Test" },
                   TargetGroups = new List<string> { "Test" }
            }).Result;
            if (result == null) return HealthCheckResult.NotConfigured;
            if (result.Annotations.IsNullOrEmpty()) return HealthCheckResult.Failed;
            return HealthCheckResult.Ok;
        }

        public async Task<AnnotationResult> GetAnnotations(ServiceInfo serviceInfo)
        {
            if (!initialized) return null;
            if (string.IsNullOrEmpty(serviceConnectionConfiguration.Uri)) return null;
            if (serviceInfo == null) return null;

            return await HttpClientWithProxy.UseAsync(proxySettings, async client =>
            {
                client.BaseAddress = new Uri(serviceConnectionConfiguration.Uri);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaContentType));
                client.Timeout = TimeSpan.FromSeconds(59);
                var result = new AnnotationResult {ServiceId = serviceInfo.Id};

                var requestBody = new RequestBody
                {
                    SessionId = serviceInfo.Id,
                    Lang = serviceInfo.LanguageCode,
                    Limit = 100,
                    Timeout = 30000,
                    Content = ProcessRequest(serviceInfo),
                    Properties = new List<string> {"rdfs:label", "skos:narrower", "skos:broader", "skos:related", "skos:inScheme", "rdf:type"}
                };

                if (!requestBody.Content.Any())
                {
                    result.State = AnnotationStates.EmptyInputReceived;
                    return result;
                }

                try
                {
                    using (HttpContent content = new StringContent(
                        JsonConvert.SerializeObject(requestBody),
                        Encoding.UTF8,
                        mediaContentType))
                    {
                        using (var response = await client.PostAsync("", content))
                        {
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                logger.LogError(
                                    $"Annotation failed: {response.Content?.ReadAsStringAsync()?.Result ?? string.Empty}");
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
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Annotation failed.");
                    logger.LogError($"Annotation failed: {e.Message}", e);
                    result.State = AnnotationStates.Failed;
                }

                return result;
            }, ignoreServerCertificate:true);
        }
    }
}
