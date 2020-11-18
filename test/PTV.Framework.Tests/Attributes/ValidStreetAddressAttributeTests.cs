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
using System.Text;
using FluentAssertions;
using Xunit;
using PTV.Framework.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PTV.Framework.Tests.Attributes
{
    public class ValidStreetAddressAttributeTests
    {
        [Fact]
        public void ListCanBeNull()
        {
            AddressTestObject obj = new AddressTestObject();
            obj.StreetAddresses = null;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StreetAddresses";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.StreetAddresses, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void ListCanBeEmpty()
        {
            AddressTestObject obj = new AddressTestObject();

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StreetAddresses";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.StreetAddresses, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void StreetNumberIsGiven()
        {
            AddressTestObject obj = new AddressTestObject();
            obj.StreetAddresses.Add("Aleksanterinkatu");
            obj.StreetNumber = "5";

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StreetAddresses";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.StreetAddresses, ctx, validationResults).Should().BeTrue();
        }

        [Fact]
        public void StreetNumberIsNotGiven()
        {
            AddressTestObject obj = new AddressTestObject();
            obj.StreetAddresses.Add("Aleksanterinkatu");

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StreetAddresses";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.StreetAddresses, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void LongitudeAndLatitudeValuesAreGiven()
        {
            AddressTestObject obj = new AddressTestObject();
            obj.StreetAddresses.Add("Aleksanterinkatu");
            obj.Longitude = "627262"; // doesn't validate values, on purpose setting bogus values, enough that the value is not null
            obj.Latitude = "782872"; // doesn't validate values, on purpose setting bogus values, enough that the value is not null

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StreetAddresses";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.StreetAddresses, ctx, validationResults).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, "36636363")]
        [InlineData("8484266", null)]
        public void LongitudeOrLatitudeValueIsNotGiven(string lon, string lat)
        {
            AddressTestObject obj = new AddressTestObject();
            obj.StreetAddresses.Add("Aleksanterinkatu");
            obj.Longitude = lon;
            obj.Latitude = lat;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StreetAddresses";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(obj.StreetAddresses, ctx, validationResults).Should().BeFalse();
        }

        [Fact]
        public void ShouldThrowWhenUsedOnWrongTypeProperty()
        {
            // the implementation currently casts the value to IList and if the casted object is null it returns success
            // should differiante null value and the cast failing
            // for the using code it would be clear that an exception is thrown if used on a type that doesn't implement IList
            // so the using code knows that whatever they are trying to validate can't be validated with this attribute

            AddressTestObject obj = new AddressTestObject();
            obj.InvalidPropertyType = string.Empty;

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "InvalidPropertyType";

            Action act = () => Validator.ValidateProperty(obj.InvalidPropertyType, ctx);

            act.Should().ThrowExactly<InvalidOperationException>(because: "The ValidStreetAddressAttribute is used on a type that doesn't implement IList.");
        }

        [Fact]
        public void ShouldThrowWhenLatitudePropertyMissing()
        {
            // this test might be incorrect if it is ok not to have this property but required to now have the StreetNumber property

            MissingLatitude obj = new MissingLatitude();
            obj.StreetAddresses.Add("Aleksanterinkatu");
            obj.Longitude = "627262";

            // Note: currently throws validation exception because StreetNumber doesn't have value and latitude property is missing

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StreetAddresses";

            Action act = () => Validator.ValidateProperty(obj.StreetAddresses, ctx);

            act.Should().ThrowExactly<InvalidOperationException>(because: "Required property 'Latitude' is not found.");
        }

        [Fact]
        public void ShouldThrowWhenLongitudePropertyMissing()
        {
            // this test might be incorrect if it is ok not to have this property but required to now have the StreetNumber property

            MissingLongitude obj = new MissingLongitude();
            obj.StreetAddresses.Add("Aleksanterinkatu");
            obj.Latitude = "627262";

            // Note: currently throws validation exception because StreetNumber doesn't have value and longitude property is missing

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "StreetAddresses";

            Action act = () => Validator.ValidateProperty(obj.StreetAddresses, ctx);

            act.Should().ThrowExactly<InvalidOperationException>(because: "Required property 'Longitude' is not found.");
        }

        internal class AddressTestObject
        {
            public AddressTestObject()
            {
                StreetAddresses = new List<string>();
            }

            // in actual code this is a list of localized street addresses
            [ValidStreetAddress]
            public IList<string> StreetAddresses { get; set; }

            public string StreetNumber { get; set; }

            public string Latitude { get; set; }

            public string Longitude { get; set; }

            [ValidStreetAddress]
            public string InvalidPropertyType { get; set; }
        }

        internal abstract class MissingPropertyTestObjectBase
        {
            public MissingPropertyTestObjectBase()
            {
                StreetAddresses = new List<string>();
            }

            [ValidStreetAddress]
            public List<string> StreetAddresses { get; set; }
        }

        internal sealed class MissingStreetNumber : MissingPropertyTestObjectBase
        {
            public string Latitude { get; set; }

            public string Longitude { get; set; }
        }

        internal sealed class MissingLatitude : MissingPropertyTestObjectBase
        {
            public string StreetNumber { get; set; }

            public string Longitude { get; set; }
        }

        internal sealed class MissingLongitude : MissingPropertyTestObjectBase
        {
            public string Latitude { get; set; }

            public string StreetNumber { get; set; }
        }
    }
}
