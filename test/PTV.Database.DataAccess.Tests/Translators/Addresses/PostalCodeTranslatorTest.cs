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

using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using Moq;
using NetTopologySuite.Geometries;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Addresses;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.GeoData;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Framework;
using Xunit;
using Coordinate = GeoAPI.Geometries.Coordinate;

namespace PTV.Database.DataAccess.Tests.Translators.Addresses
{
    public class PostalCodeTranslatorTest : TranslatorTestBase
    {
        [Fact]
        public void TranslateToViewModel()
        {
            var translators = RegisterTranslators();
            var postalCode = CreatePostalCodeEntity();
            var toTranslate = new List<PostalCode> {postalCode};
            var expectedBorder = CreateBorderModel();
            var expectedCenter = CreateCenterModel();

            var translations = RunTranslationEntityToModelTest<PostalCode, VmPostalCode>(translators, toTranslate);
            var translation = translations.FirstOrDefault();
            
            Assert.Equal(postalCode.Id, translation.Id);
            Assert.Equal(postalCode.Code, translation.Code);
            // AssertBorder(expectedBorder, translation.Border);
            Assert.Equal(postalCode.MunicipalityId, translation.MunicipalityId);
            AssertCoordinate(expectedCenter, translation.CenterCoordinate);
        }

        private static void AssertCoordinate(VmCoordinate expected, VmCoordinate translated)
        {
            Assert.Equal(expected.Latitude, translated.Latitude);
            Assert.Equal(expected.Longitude, translated.Longitude);
            Assert.Equal(expected.CoordinateState, translated.CoordinateState);
        }

        private void AssertBorder(List<VmCoordinateCollection> expectedBorder, List<VmCoordinateCollection> translationBorder)
        {
            Assert.Equal(expectedBorder.Count, translationBorder.Count);

            foreach (var linearRing in expectedBorder)
            {
                var firstCoordinate = linearRing.Coordinates.First();
                var translatedRing = translationBorder.FirstOrDefault(r =>
                {
                    var coordinate = r.Coordinates.First();
                    return coordinate.Latitude == firstCoordinate.Latitude &&
                           coordinate.Longitude == firstCoordinate.Longitude;
                });

                for (var i = 0; i < linearRing.Coordinates.Count; i++)
                {
                    AssertCoordinate(linearRing.Coordinates[i], translatedRing.Coordinates[i]);
                }
            }
        }

        private VmPostalCode CreatePostalCodeModel()
        {
            return new VmPostalCode()
            {
                Id = PostalCode.GetGuid(),
                Code = PostalCode,
                Border = CreateBorderModel(),
                MunicipalityId = MunicipalityCode.GetGuid(),
                CenterCoordinate = CreateCenterModel(),
                MunicipalityCode = MunicipalityCode
            };
        }

        private PostalCode CreatePostalCodeEntity()
        {
            return new PostalCode
            {
                Id = PostalCode.GetGuid(),
                Code = PostalCode,
                Border = CreateBorderEntity(),
                IsValid = true,
                MunicipalityId = MunicipalityCode.GetGuid(),
                CenterCoordinate = CreateCenterEntity()
            };
        }

        private VmCoordinate CreateCenterModel()
        {
            return new VmCoordinate {Latitude = 4, Longitude = 5, CoordinateState = "ok"};
        }

        private Point CreateCenterEntity()
        {
            return new Point(5, 4);
        }

        private List<VmCoordinateCollection> CreateBorderModel()
        {
            return new List<VmCoordinateCollection>
            {
                new VmCoordinateCollection
                {
                    Coordinates = new List<VmCoordinate>
                        {
                            new VmCoordinate { Longitude = 0, Latitude = 1, CoordinateState = "ok"},
                            new VmCoordinate { Longitude = 2, Latitude = 3, CoordinateState = "ok" },
                            new VmCoordinate { Longitude = 4, Latitude = 5, CoordinateState = "ok" },
                            new VmCoordinate { Longitude = 0, Latitude = 1, CoordinateState = "ok" },
                        }
                },
                new VmCoordinateCollection
                {
                    Coordinates =  new List<VmCoordinate>
                        {
                            new VmCoordinate { Longitude = 6, Latitude = 7, CoordinateState = "ok" },
                            new VmCoordinate { Longitude = 8, Latitude = 9, CoordinateState = "ok" },
                            new VmCoordinate { Longitude = 1, Latitude = 0, CoordinateState = "ok" },
                            new VmCoordinate { Longitude = 3, Latitude = 2, CoordinateState = "ok" },
                            new VmCoordinate { Longitude = 6, Latitude = 7, CoordinateState = "ok" },
                        }
                }
            };
        }

        private MultiPolygon CreateBorderEntity()
        {
            return new MultiPolygon(new IPolygon[]
            {
                new Polygon(new LinearRing(new Coordinate[]
                {
                    new Coordinate(0, 1),
                    new Coordinate(2, 3),
                    new Coordinate(4, 5),
                    new Coordinate(0, 1), 
                })),
                new Polygon(new LinearRing(new Coordinate[]
                {
                    new Coordinate(6, 7),
                    new Coordinate(8, 9),
                    new Coordinate(1, 0), 
                    new Coordinate(3, 2),
                    new Coordinate(6, 7),
                }))
            });
        }

        private List<object> RegisterTranslators()
        {
            var unitOfWork = unitOfWorkMockSetup.Object;
            
            var translators = new List<object>
            {
                new PostalCodeTranslator(ResolveManager, TranslationPrimitives, new TranslatedItemDefinitionHelper()),
                new PostalCodeNameTranslator(ResolveManager, TranslationPrimitives),
                new PointTranslator(ResolveManager, TranslationPrimitives),
                new VmCoordinateCollectionTranslator(ResolveManager, TranslationPrimitives),
                new VmCoordinateTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<INameReferences, IVmTranslatedItem>>(), unitOfWork,
                    vm => default(INameReferences), entity => new Mock<IVmTranslatedItem>().Object),
                RegisterTranslatorMock(new Mock<ITranslator<IName, string>>(), unitOfWork, vm => default(IName),
                    entity => new Mock<string>().Object),
                RegisterTranslatorMock(new Mock<ITranslator<INameReferences, IVmTranslationItem>>(), unitOfWork,
                    vm => default(INameReferences), entity => new Mock<IVmTranslationItem>().Object)
            };

            return translators;
        }

        private const string PostalCode = "12345";
        private const string MunicipalityCode = "678";
    }
}