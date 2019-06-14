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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.ServiceManager;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Street
{
    public class IsAddressPointValidTest : TestBase
    {
        [Theory]
        [InlineData(null, MunicipalityValid, PostalCodeValid)]
        [InlineData(StreetValid, null, PostalCodeValid)]
        [InlineData(StreetValid, MunicipalityValid, null)]
        public void EmptyGuidsThrowException(string streetName, string municipalityName, string postalCode)
        {
            var streetService = Init(new List<ClsAddressStreet>());

            var streetId = streetName?.GetGuid() ?? Guid.Empty;
            var municipalityId = municipalityName?.GetGuid() ?? Guid.Empty;
            var postalCodeId = postalCode?.GetGuid() ?? Guid.Empty;

            Assert.Throws<PtvAppException>(() => streetService.IsAddressPointValid(streetId, municipalityId,
                StreetNumberValid.GetGuid(), postalCodeId, unitOfWorkMockSetup.Object));
        }

        [Theory]
        [InlineData(StreetInvalid, MunicipalityValid, StreetNumberValid, PostalCodeValid)]
        [InlineData(StreetValid, MunicipalityInvalid, StreetNumberValid, PostalCodeValid)]
        [InlineData(StreetValid, MunicipalityValid, StreetNumberInvalid, PostalCodeValid)]
        [InlineData(StreetValid, MunicipalityValid, StreetNumberValid, PostalCodeInvalid)]
        public void NonMatchingIdsReturnFalse(string streetName, string municipalityName, string streetNumber,
            string postalCode)
        {
            var streetService = Init(GetStreets());

            var streetId = streetName?.GetGuid() ?? Guid.Empty;
            var municipalityId = municipalityName?.GetGuid() ?? Guid.Empty;
            var streetNumberId = streetNumber?.GetGuid() ?? Guid.Empty;
            var postalCodeId = postalCode?.GetGuid() ?? Guid.Empty;

            var result = streetService.IsAddressPointValid(streetId, municipalityId,
                streetNumberId, postalCodeId, unitOfWorkMockSetup.Object);

            Assert.False(result);
        }

        [Theory]
        [InlineData(false, true, true, true)]
        [InlineData(true, false, true, true)]
        [InlineData(true, true, false, true)]
        [InlineData(true, true, true, false)]
        public void InvalidEntitiesReturnFalse(bool streetValid, bool municipalityValid, bool streetNumberValid,
            bool postalCodeValid)
        {
            var streets = GetStreets(streetValid, municipalityValid, streetNumberValid, postalCodeValid);
            var streetService = Init(streets);

            var result = streetService.IsAddressPointValid(StreetValid.GetGuid(), MunicipalityValid.GetGuid(),
                StreetNumberValid.GetGuid(), PostalCodeValid.GetGuid(), unitOfWorkMockSetup.Object);

            Assert.False(result);
        }

        [Fact]
        public void ValidIdsReturnTrue()
        {
            var streetService = Init(GetStreets());

            var result = streetService.IsAddressPointValid(StreetValid.GetGuid(), MunicipalityValid.GetGuid(),
                StreetNumberValid.GetGuid(), PostalCodeValid.GetGuid(), unitOfWorkMockSetup.Object);
            
            Assert.True(result);
        }

        [Fact]
        public void EmptyStreetNumberRangeReturnsTrue()
        {
            var streetService = Init(GetStreets());

            var result = streetService.IsAddressPointValid(StreetValid.GetGuid(), MunicipalityValid.GetGuid(),
                null, PostalCodeValid.GetGuid(), unitOfWorkMockSetup.Object);
            
            Assert.True(result);
        }

        private IStreetService Init(List<ClsAddressStreet> streets)
        {
            RegisterRepository<IClsAddressStreetRepository, ClsAddressStreet>(streets.AsQueryable());

            return new StreetService(
                null,
                null,
                PublishingStatusCache,
                LanguageCache,
                null,
                null,
                null);
        }

        private const string StreetValid = "Street";
        private const string MunicipalityValid = "Municipality";
        private const string StreetNumberValid = "132a";
        private const string PostalCodeValid = "00001";

        private const string StreetInvalid = "Teerts";
        private const string MunicipalityInvalid = "Ytilapicinum";
        private const string StreetNumberInvalid = "978b";
        private const string PostalCodeInvalid = "45654";

        private List<ClsAddressStreet> GetStreets(bool streetValid = true, bool municipalityValid = true,
            bool streetNumberValid = true, bool postalCodeValid = true)
        {
            return new List<ClsAddressStreet>
            {
                new ClsAddressStreet
                {
                    Id = StreetValid.GetGuid(),
                    IsValid = streetValid,
                    MunicipalityId = MunicipalityValid.GetGuid(),
                    Municipality = new Municipality
                    {
                        Id = MunicipalityValid.GetGuid(),
                        IsValid = municipalityValid
                    },
                    StreetNumbers = new List<ClsAddressStreetNumber>
                    {
                        new ClsAddressStreetNumber
                        {
                            Id = StreetNumberValid.GetGuid(),
                            IsValid = streetNumberValid,
                            PostalCodeId = PostalCodeValid.GetGuid(),
                            PostalCode = new PostalCode
                            {
                                Id = PostalCodeValid.GetGuid(),
                                IsValid = postalCodeValid,
                                MunicipalityId = MunicipalityValid.GetGuid()
                            }
                        }
                    }
                }
            };
        }
    }
}
