﻿/**
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
using WebProxy = System.Net.WebProxy;

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
        WrongFormatReceived,
        EnteredByUser,
        EnteredByAR
    }

    public class AddressInfo
    {
        public Guid Id { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string MunicipalityCode { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public CoordinateStates State { get; set; }
    }


    [RegisterService(typeof(MapServiceProvider), RegisterType.Transient)]
    public class MapServiceProvider
    {
        private readonly XDocument requestBody;
        private readonly ApplicationConfiguration configuration;

        private static readonly string requestBodyContent = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4NCjx3ZnM6R2V0RmVhdHVyZSB4bWxuczpqeD0iaHR0cDovL2FwYWNoZS5vcmcvY29jb29uL3RlbXBsYXRlcy9qeC8xLjAiIHhtbG5zOm9zbz0iaHR0cDovL3htbC5ubHMuZmkvT3NvaXR0ZWV0L09zb2l0ZXBpc3RlLzIwMTEvMDIiIHhtbG5zOndmcz0iaHR0cDovL3d3dy5vcGVuZ2lzLm5ldC93ZnMiIHhtbG5zOmdtbD0iaHR0cDovL3d3dy5vcGVuZ2lzLm5ldC9nbWwiIHhtbG5zOm9nYz0iaHR0cDovL3d3dy5vcGVuZ2lzLm5ldC9vZ2MiIHhtbG5zOnhzaT0iaHR0cDovL3d3dy53My5vcmcvMjAwMS9YTUxTY2hlbWEtaW5zdGFuY2UiIHZlcnNpb249IjEuMS4wIiB4c2k6c2NoZW1hTG9jYXRpb249Imh0dHA6Ly93d3cub3Blbmdpcy5uZXQvd2ZzIGh0dHA6Ly9zY2hlbWFzLm9wZW5naXMubmV0L3dmcy8xLjEuMC93ZnMueHNkIj4NCiAgPHdmczpRdWVyeSB0eXBlTmFtZT0ib3NvOk9zb2l0ZW5pbWkiPg0KICAgIDxvZ2M6U29ydEJ5Pg0KICAgICAgPG9nYzpTb3J0UHJvcGVydHk+DQogICAgICAgIDxvZ2M6UHJvcGVydHlOYW1lPm9zbzprdW50YW5pbWlGaW48L29nYzpQcm9wZXJ0eU5hbWU+DQogICAgICA8L29nYzpTb3J0UHJvcGVydHk+DQogICAgICA8b2djOlNvcnRPcmRlcj5ERVNDPC9vZ2M6U29ydE9yZGVyPg0KICAgIDwvb2djOlNvcnRCeT4NCiAgICA8b2djOkZpbHRlcj4NCiAgICAgIDxvZ2M6QW5kPg0KICAgICAgICA8b2djOlByb3BlcnR5SXNMaWtlIHdpbGRDYXJkPSIqIiBzaW5nbGVDaGFyPSI/IiBlc2NhcGU9IiEiIG1hdGNoQ2FzZT0iZmFsc2UiPg0KICAgICAgICAgIDxvZ2M6UHJvcGVydHlOYW1lPm9zbzprYXR1bmltaTwvb2djOlByb3BlcnR5TmFtZT4NCiAgICAgICAgICA8b2djOkxpdGVyYWw+PC9vZ2M6TGl0ZXJhbD4NCiAgICAgICAgPC9vZ2M6UHJvcGVydHlJc0xpa2U+DQogICAgICAgIDxvZ2M6UHJvcGVydHlJc0VxdWFsVG8+DQogICAgICAgICAgPG9nYzpQcm9wZXJ0eU5hbWU+b3NvOmthdHVudW1lcm88L29nYzpQcm9wZXJ0eU5hbWU+DQogICAgICAgICAgPG9nYzpMaXRlcmFsPjwvb2djOkxpdGVyYWw+DQogICAgICAgIDwvb2djOlByb3BlcnR5SXNFcXVhbFRvPg0KICAgICAgICA8b2djOlByb3BlcnR5SXNFcXVhbFRvPg0KICAgICAgICAgIDxvZ2M6UHJvcGVydHlOYW1lPm9zbzprdW50YXR1bm51czwvb2djOlByb3BlcnR5TmFtZT4NCiAgICAgICAgICA8b2djOkxpdGVyYWw+PC9vZ2M6TGl0ZXJhbD4NCiAgICAgICAgPC9vZ2M6UHJvcGVydHlJc0VxdWFsVG8+DQogICAgICA8L29nYzpBbmQ+DQogICAgPC9vZ2M6RmlsdGVyPg0KICA8L3dmczpRdWVyeT4NCjwvd2ZzOkdldEZlYXR1cmU+";

        private static readonly string xmlMediaContentType = "application/xml";
        private static readonly string propertyName = "PropertyName";
        private static readonly string streetNameValue = "oso:katunimi";
        private static readonly string streetNumberValue = "oso:katunumero";
        private static readonly string municipalityCodeValue = "oso:kuntatunnus";
        private static readonly string positionElementName = "pos";

        private static readonly XNamespace gml = "http://www.opengis.net/gml";
        private static readonly XNamespace ogc = "http://www.opengis.net/ogc";
        private static readonly XNamespace wfs = "http://www.opengis.net/wfs";
        private readonly XElement query;
        private readonly ProxyServerSettings proxySettings;
        private readonly bool initialized = false;
        private ILogger<MapServiceProvider> logger;

        public MapServiceProvider(ApplicationConfiguration configuration, IOptions<ProxyServerSettings> proxySettings, ILogger<MapServiceProvider> logger)
        {
            this.configuration = configuration;
            this.proxySettings = proxySettings.Value;
            try
            {
                using (var streamBodyContent = new MemoryStream(Convert.FromBase64String(requestBodyContent)))
                {
                    requestBody = XDocument.Load(streamBodyContent);
                }
            }
            catch (Exception e)
            {
                logger.LogCritical(CoreExtensions.ExtractAllInnerExceptions(e));
                return;
            }

            query = requestBody.Descendants(wfs + "Query").First();
            query.Remove();
            initialized = true;
            this.logger = logger;
        }

        private XElement GetQueryElement()
        {
            return new XElement(query);
        }

        private XElement AddElementToBody(AddressInfo address)
        {
            var element = GetQueryElement();
            var queryNodes = element.Descendants(ogc + propertyName);
            var queryStreetName = queryNodes?.First(x => x.Value == streetNameValue).NextNode as XElement;
            var queryStreetNumber = queryNodes?.First(x => x.Value == streetNumberValue).NextNode as XElement;
            var queryMunicipalityCode = queryNodes?.First(x => x.Value == municipalityCodeValue).NextNode as XElement;

            if (queryStreetName == null || queryStreetNumber == null || queryMunicipalityCode == null) return null;

            queryStreetName.Value = address.Street.Replace("'", "?").Trim();
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
            address.Longitude = double.TryParse(splitPos[0], NumberStyles.Number,
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


        public bool CallTestAddress()
        {
            var result = GetCoordinates(new List<AddressInfo>()
            {
                new AddressInfo()
                {
                    State = CoordinateStates.Loading,
                    MunicipalityCode = "091",
                    // ReSharper disable once StringLiteralTypo
                    Street = "Lintulahdenkuja",
                    StreetNumber = "4"
                }
            }).Result;
            return result.Any();
        }

        public async Task<List<AddressInfo>> GetCoordinates(IReadOnlyList<AddressInfo> addresses)
        {
            if (!initialized) return null;
            
            var baseUri = configuration.GetMapServiceUri();
            if (string.IsNullOrEmpty(baseUri)) return null;

            return await HttpClientWithProxy.UseAsync(proxySettings, async client =>
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(xmlMediaContentType));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(
                    string.Format("{0}:{1}", configuration.GetMapUserName(), configuration.GetMapPassword()))));

                var result = new List<AddressInfo>();

                //var counter = 0;
                foreach (var address in addresses)
                {
                    //counter++;
                    if (string.IsNullOrWhiteSpace(address.Street) || string.IsNullOrWhiteSpace(address.StreetNumber) ||
                        string.IsNullOrWhiteSpace(address.MunicipalityCode))
                    {
                        result.Add(ProccessAddress(null, address));
                        continue;
                    }

                    var element = AddElementToBody(address);

                    if (element != null)
                    {
                        try
                        {
                            await GetCoordinatesForAddress(client, address, result);
                        }
                        catch (AggregateException)
                        {
                            logger.LogWarning("AggregateException thrown. loop closed, result returned for further process");
                            break;
                        }
                        catch (TaskCanceledException e)
                        {
                            logger.LogWarning(e.Message);
                            break;
                        }
                        finally
                        {
                            element.Remove();
                        }
                    }
                }
                return result;
            }, 30);
        }

        private async Task GetCoordinatesForAddress(HttpClient client, AddressInfo address, List<AddressInfo> result)
        {
            using (HttpContent content = new StringContent(requestBody.ToString(), Encoding.UTF8, xmlMediaContentType))
            {
                using (var response = await client.PostAsync("", content))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        address.State = CoordinateStates.Failed;
                        result.Add(address);
                    }
                    else
                    {
                        var data = await response.Content.ReadAsStreamAsync();

                        var xElement =
                            XDocument.Load(data).Descendants(gml + "featureMember").FirstOrDefault();

                        result.Add(ProccessAddress(xElement, address));
                    }
                }
            }
        }
    }
}
