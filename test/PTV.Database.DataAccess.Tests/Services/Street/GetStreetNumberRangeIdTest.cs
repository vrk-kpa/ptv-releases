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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Street
{
    public class GetStreetNumberRangeIdTest : TestBase
    {
        [Theory]
        [InlineData((string) null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abc")]
        public void NonNumberReturnsNull(string streetNumber)
        {
            var streetService = Init();

            var result = streetService.GetStreetNumberRangeId(
                unitOfWorkMockSetup.Object,
                streetNumber,
                EvenStreet.GetGuid());

            Assert.Null(result);
        }

        [Theory]
        [InlineData("2", EvenStreet)]
        [InlineData("10", EvenStreet)]
        [InlineData("5", EvenStreet)]
        [InlineData("3", OddStreet)]
        [InlineData("7", OddStreet)]
        public void OutOfRangeNumberReturnsNull(string streetNumber, string streetName)
        {
            var streetService = Init();

            var result = streetService.GetStreetNumberRangeId(
                unitOfWorkMockSetup.Object,
                streetNumber,
                streetName.GetGuid());

            Assert.Null(result);
        }

        [Theory]
        [InlineData("4", EvenStreet, EvenStreetNumberId)]
        [InlineData("8abcdefghi", EvenStreet, EvenStreetNumberId)]
        [InlineData("5-7q", OddStreet, OddStreetNumberId)]
        public void CorrectInputReturnsId(string streetNumber, string streetName, string expectedResultString)
        {
            var streetService = Init();
            var expectedResultId = expectedResultString.GetGuid();

            var result = streetService.GetStreetNumberRangeId(
                unitOfWorkMockSetup.Object,
                streetNumber,
                streetName.GetGuid());

            Assert.Equal(expectedResultId, result);
        }

        private IStreetService Init()
        {
            RegisterRepository<IClsAddressStreetNumberRepository, ClsAddressStreetNumber>(StreetNumbers.AsQueryable());

            return new StreetService(
                null,
                null,
                PublishingStatusCache,
                LanguageCache,
                null,
                null,
                null);
        }

        private const string EvenStreet = "EvenStreet";
        private const string OddStreet = "OddStreet";

        private const string EvenStreetNumberId = "Even";
        private const int EvenStartNumber = 4;
        private const int EvenEndNumber = 8;

        private const string OddStreetNumberId = "Odd";
        private const int OddStartNumber = 5;

        private List<ClsAddressStreetNumber> StreetNumbers => new List<ClsAddressStreetNumber>
        {
            new ClsAddressStreetNumber
            {
                Id = EvenStreetNumberId.GetGuid(),
                StartNumber = EvenStartNumber,
                EndNumber = EvenEndNumber,
                IsEven = true,
                IsValid = true,
                ClsAddressStreetId = EvenStreet.GetGuid()
            },

            new ClsAddressStreetNumber
            {
                Id = OddStreetNumberId.GetGuid(),
                StartNumber = OddStartNumber,
                EndNumber = 0,
                IsEven = false,
                IsValid = true,
                ClsAddressStreetId = OddStreet.GetGuid()
            }
        };
    }
}
