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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PTV.Framework.Extensions
{
    public enum CoordinateStates
    {
        Loading,
        Ok,
        Failed,
        NotReceived,
        EmptyInputReceived,
        MultipleResultsReceived,
        WrongFormatReceived
    }

    public class AddressInfo
    {
        public Guid Id { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string MunicipalityCode { get; set; }
        public double? Longtitude { get; set; }
        public double? Latitude { get; set; }
        public CoordinateStates State { get; set; }
    }

    public class WebProxy : IWebProxy
    {
        public WebProxy(string proxyUri)
            : this(new Uri(proxyUri))
        {
        }

        public WebProxy(Uri proxyUri)
        {
            this.ProxyUri = proxyUri;
        }

        public Uri ProxyUri { get; set; }

        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination)
        {
            return this.ProxyUri;
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }
    }

    [RegisterService(typeof(MapServiceProvider), RegisterType.Singleton)]
    public class MapServiceProvider
    {
        private readonly XDocument requestBody;
        private readonly ApplicationConfiguration configuration;

        private static readonly string frameworkProjectPath = "..\\PTV.Framework\\";
        private static readonly string requestBodyFileName = "MapServiceRequestBody.xml";

        private static readonly string xmlMediaContentType = "application/xml";
        private static readonly string propertyName = "PropertyName";
        private static readonly string streetNameValue = "oso:katunimi";
        private static readonly string streetNumberValue = "oso:katunumero";
        private static readonly string municipalityCodeValue = "oso:kuntatunnus";
        private static readonly string positionElementName = "pos";

        private static readonly XNamespace gml = "http://www.opengis.net/gml";
        private static readonly XNamespace ogc = "http://www.opengis.net/ogc";
        private static readonly XNamespace wfs = "http://www.opengis.net/wfs";
        private static XElement query;
        //private static XElement streetName;
        //private static XElement streetNumber;
        //private static XElement municipalityCode;
        //private static XElement queryStreetName;
        //private static XElement queryStreetNumber;
        //private static XElement queryMunicipalityCode;
        private readonly ProxyServerSettings proxySettings;
        private readonly bool initialized = false;

        public MapServiceProvider(IHostingEnvironment env, ApplicationConfiguration configuration, IOptions<ProxyServerSettings> proxySettings, ILogger<MapServiceProvider> logger)
        {
            this.configuration = configuration;
            this.proxySettings = proxySettings.Value;
            try
            {
                requestBody = XDocument.Load(env.IsDevelopment() ? string.Format("{0}{1}", frameworkProjectPath, requestBodyFileName) : Path.Combine(env.ContentRootPath, requestBodyFileName));
            }
            catch (Exception e)
            {
                logger.LogCritical(CoreExtensions.ExtractAllInnerExceptions(e));
                return;
            }

            query = requestBody.Descendants(wfs + "Query").First();
            query.Remove();
            initialized = true;
        }

        private XElement GetQueryElement()
        {
            return new XElement(query);
        }

        private XElement AddElementToBody(AddressInfo address, bool isBunch = false)
        {
            var element = GetQueryElement();
            var queryNodes = element.Descendants(ogc + propertyName);
            var queryStreetName = queryNodes?.First(x => x.Value == streetNameValue).NextNode as XElement;
            var queryStreetNumber = queryNodes?.First(x => x.Value == streetNumberValue).NextNode as XElement;
            var queryMunicipalityCode = queryNodes?.First(x => x.Value == municipalityCodeValue).NextNode as XElement;

            if (queryStreetName == null || queryStreetNumber == null || queryMunicipalityCode == null) return null;

            queryStreetName.Value = address.Street.Trim();
            queryStreetNumber.Value = new string(address.StreetNumber.Trim().TakeWhile(Char.IsDigit).ToArray());
            queryMunicipalityCode.Value = address.MunicipalityCode;

            requestBody.Element(wfs + "GetFeature").Add(element);
            return element;
        }

        private AddressInfo ProccessAddress(XElement xElement, AddressInfo address)
        {
            if (xElement == null)
            {
                address.State = CoordinateStates.NotReceived;
                return address;
            }

            var positons = xElement.Descendants(gml + positionElementName).ToList();

            if (positons.Count > 1)
            {
                address.State = CoordinateStates.MultipleResultsReceived;
                return address;
            }

            var position = positons.FirstOrDefault();

            if (string.IsNullOrEmpty(position?.Value))
            {
                address.State = CoordinateStates.NotReceived;
                return address;
            }

            var splitPos = position.Value.Split(' ');

            if (splitPos.Length != 2)
            {
                address.State = CoordinateStates.WrongFormatReceived;
                return address;
            }

            double temp;
            address.Longtitude = double.TryParse(splitPos[0], NumberStyles.Number,
                CultureInfo.InvariantCulture, out temp)
                ? temp
                : (double?)null;
            address.Latitude = double.TryParse(splitPos[1], NumberStyles.Number,
                CultureInfo.InvariantCulture, out temp)
                ? temp
                : (double?)null;
            address.State = CoordinateStates.Ok;

            return address;
        }

        public async Task<List<AddressInfo>> GetCoordiantes(IReadOnlyList<AddressInfo> addresses, bool isBunch = false, bool isBatch = false)
        {
            if (!initialized) return null;
            var baseUri = configuration.GetMapServiceUri();
            if (string.IsNullOrEmpty(baseUri)) return null;

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
                using (var client = new HttpClient(httpClientHandler) {BaseAddress = new Uri(baseUri)})
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(xmlMediaContentType));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", configuration.GetMapUserName(), configuration.GetMapPassword()))));

                    var result = new List<AddressInfo>();

                    var counter = 0;
                    foreach (var address in addresses)
                    {
                        counter++;
                        if (string.IsNullOrEmpty(address.Street) || string.IsNullOrEmpty(address.StreetNumber) ||
                            string.IsNullOrEmpty(address.MunicipalityCode))
                        {
                            result.Add(new AddressInfo() {State = CoordinateStates.EmptyInputReceived, Id = address.Id});
                            continue;
                        }
                        var element = AddElementToBody(address);

                        if (!isBunch && element != null)
                        {
                            try
                            {
                                HttpContent content = new StringContent(requestBody.ToString(),
                                    Encoding.UTF8,
                                    xmlMediaContentType);
                                var response = isBatch
                                    ? client.PostAsync("", content).Result
                                    : await client.PostAsync("", content);

                                if (isBatch)
                                {
                                    Console.Write("#");
                                    Thread.Sleep(150);
                                }

                                if (response.StatusCode != HttpStatusCode.OK)
                                {
                                    address.State = CoordinateStates.Failed;
                                    result.Add(address);
                                }
                                else
                                {
                                    var data = isBatch
                                        ? response.Content.ReadAsStreamAsync().Result
                                        : await response.Content.ReadAsStreamAsync();

                                    var xElement =
                                        XDocument.Load(data).Descendants(gml + "featureMember").FirstOrDefault();

                                    result.Add(ProccessAddress(xElement, address));
                                }

                                element.Remove();
                            }
                            catch (AggregateException)
                            {
                                Console.WriteLine("AggregateException thrown. loop closed, result returned for further process");
                                break;
                            }
                        }
                    }

                    if (!isBunch) return result;
                    {
                        HttpContent content = new StringContent(requestBody.ToString(), System.Text.Encoding.UTF8,
                            xmlMediaContentType);
                        var response = await client.PostAsync("", content);



                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            // not implemented, because not same result returned
                            Console.WriteLine("yes");
                        }
                        else
                        {
                            var data = await response.Content.ReadAsByteArrayAsync();
                            var encoded = Encoding.UTF8.GetString(data);
                            XDocument doc = XDocument.Parse(encoded);
                            var positions = doc.Descendants(gml + "featureMember").ToList();

                            foreach (var position in positions)
                            {
                                // not implemented, because not same result returned
                            }
                        }
                    }

                    return result;
                }
            }
        }
    }
}
