using System;
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
                PostalCodeId,
                streetNumber,
                EvenStreet.GetGuid());

            Assert.Null(result);
        }

        [Theory]
        [InlineData("2", EvenStreet)]
        [InlineData("10", EvenStreet)]
        [InlineData("5", EvenStreet)]
        [InlineData("3", OddStreet)]
        [InlineData("13", OddStreet)]
        public void OutOfRangeNumberReturnsNull(string streetNumber, string streetName)
        {
            var streetService = Init();

            var result = streetService.GetStreetNumberRangeId(
                unitOfWorkMockSetup.Object,
                PostalCodeId,
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
                PostalCodeId,
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
        private const int OddEndNumber = 11;
        private static Guid PostalCodeId = Guid.Parse("E3842F00-7B1D-418C-ABEE-58DA99EA3406");

        private List<ClsAddressStreetNumber> StreetNumbers => new List<ClsAddressStreetNumber>
        {
            new ClsAddressStreetNumber
            {
                Id = EvenStreetNumberId.GetGuid(),
                StartNumber = EvenStartNumber,
                EndNumber = EvenEndNumber,
                IsEven = true,
                IsValid = true,
                ClsAddressStreetId = EvenStreet.GetGuid(),
                PostalCodeId = PostalCodeId
            },

            new ClsAddressStreetNumber
            {
                Id = OddStreetNumberId.GetGuid(),
                StartNumber = OddStartNumber,
                EndNumber = OddEndNumber,
                IsEven = false,
                IsValid = true,
                ClsAddressStreetId = OddStreet.GetGuid(),
                PostalCodeId = PostalCodeId
            }
        };
    }
}
