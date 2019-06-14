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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Coordinate = GeoAPI.Geometries.Coordinate;

namespace PTV.Database.DataAccess.Services
{
    /// <inheritdoc />
    [RegisterService(typeof(IPostalCodeCoordinatesService), RegisterType.Transient)]
    public class PostalCodeCoordinatesService : IPostalCodeCoordinatesService
    {
        private readonly XNamespace postialue = XNamespace.Get("http://www.postialueet.fi");
        private readonly XNamespace gml = XNamespace.Get("http://www.opengis.net/gml");

        /// <inheritdoc />
        public async Task<List<VmPostalCodeCoordinate>> DownloadBatch(HttpClient client, int offset, int pageSize,
            PostalCodeCoordinatesSettings postalCodeCoordinatesSettings, StringBuilder jobStatus)
        {
            var result = new List<VmPostalCodeCoordinate>();
            var batchUrl = string.Format(postalCodeCoordinatesSettings.BatchUrl,postalCodeCoordinatesSettings.UrlBase, pageSize, offset);
            var response = await client.GetStreamAsync(batchUrl);
            var xml = XDocument.Load(response);
            var elements = xml.Root?.Element(gml + "featureMembers")?.Elements(postialue + "pno_meri");

            if (elements == null)
            {
                return result;
            }

            foreach (var element in elements)
            {
                var code = element.Element(postialue + "posti_alue")?.Value;

                try
                {
                    var center = CalculateCenterPoint(element);
                    var border = ParseMultiPolygon(element);

                    result.Add(new VmPostalCodeCoordinate
                    {
                        Code = code,
                        Border = border,
                        Center = center
                    });
                }
                catch (Exception e)
                {
                    jobStatus.Append($"Error while processing postal code {code}: {e.Message}");
                }
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<List<VmPostalCodeCoordinate>> DownloadCoordinates(HttpClient client, 
            IEnumerable<VmPostalCode> postalCodes, PostalCodeCoordinatesSettings postalCodeCoordinatesSettings, StringBuilder jobStatus)
        {
            var result = new List<VmPostalCodeCoordinate>();

            foreach (var postalCode in postalCodes)
            {
                var postalCodeUrl = string.Format(postalCodeCoordinatesSettings.SingleUrl, postalCodeCoordinatesSettings.UrlBase, postalCode.Code);
                var response = await client.GetStreamAsync(postalCodeUrl);
                var xml = XDocument.Load(response);
                var postalCodeElement = xml.Root
                    ?.Element(gml + "featureMembers")
                    ?.Element(postialue + "pno_meri");

                if (postalCodeElement == null)
                {
                    jobStatus.Append($"No coordinates found for postal code {postalCode.Code}.");
                }

                try
                {
                    var center = CalculateCenterPoint(postalCodeElement);
                    var border = ParseMultiPolygon(postalCodeElement);

                    result.Add(new VmPostalCodeCoordinate
                    {
                        Code = postalCode.Code,
                        Border = border,
                        Center = center
                    });
                }
                catch (Exception e)
                {
                    jobStatus.Append($"Error while processing postal code {postalCode.Code}: {e.Message}");
                }
            }

            return result;
        }

        /// <inheritdoc />
        public void UpdateCoordinates(IUnitOfWorkWritable unitOfWork, List<VmPostalCodeCoordinate> postalCodeCoordinates)
        {
            var postalCodesRepo = unitOfWork.CreateRepository<IPostalCodeRepository>();
            var postalCodes = postalCodesRepo.All().ToDictionary(pc => pc.Code);

            foreach (var postalCodeCoordinate in postalCodeCoordinates)
            {
                if (postalCodes.TryGetValue(postalCodeCoordinate.Code, out var postalCode))
                {
                    postalCode.CenterCoordinate = postalCodeCoordinate.Center;
                    postalCode.Border = postalCodeCoordinate.Border;
                }
            }

            unitOfWork.Save(SaveMode.AllowAnonymous);
        }

        private MultiPolygon ParseMultiPolygon(XElement postalCodeElement)
        {
            var polygons = new List<IPolygon>();
            var multiPolygonElement = postalCodeElement
                ?.Element(postialue + "geom")
                ?.Element(gml + "MultiSurface");

            if (multiPolygonElement == null)
            {
                return new MultiPolygon(polygons.ToArray());
            }

            foreach (var surfaceMember in multiPolygonElement.Elements(gml + "surfaceMember"))
            {
                var coordinatesElement = surfaceMember
                    ?.Element(gml + "Polygon")
                    ?.Element(gml + "exterior")
                    ?.Element(gml + "LinearRing")
                    ?.Element(gml + "posList");

                if (coordinatesElement == null)
                    continue;

                var values = coordinatesElement.Value.Split(" ");
                var linearRing = CreateLinearRing(values);
                var polygon = new Polygon(linearRing);
                polygons.Add(polygon);
            }

            return new MultiPolygon(polygons.ToArray());
        }

        private LinearRing CreateLinearRing(string[] values)
        {
            var coordinates = new List<Coordinate>();

            for (var i = 0; i < values.Length; i += 2)
            {
                var x = double.Parse(values[i]);
                var y = double.Parse(values[i + 1]);
                coordinates.Add(new Coordinate(x, y));
            }

            return new LinearRing(coordinates.ToArray());
        }

        private Point CalculateCenterPoint(XElement postalCodeElement)
        {
            var envelope = postalCodeElement
                ?.Element(gml + "boundedBy")
                ?.Element(gml + "Envelope");

            var lowerCorner = envelope?.Element(gml + "lowerCorner");
            var upperCorner = envelope?.Element(gml + "upperCorner");

            if (lowerCorner == null || upperCorner == null)
                return null;

            var lowerPoint = ParsePoint(lowerCorner);
            var upperPoint = ParsePoint(upperCorner);

            var midX = (lowerPoint.X + upperPoint.X) / 2;
            var midY = (lowerPoint.Y + upperPoint.Y) / 2;
            return new Point(midX, midY);
        }

        private Point ParsePoint(XElement xElement)
        {
            var value = xElement.Value;
            var parts = value.Split(" ");
            var first = double.Parse(parts[0]);
            var second = double.Parse(parts[1]);

            return new Point(first, second);
        }
    }
}