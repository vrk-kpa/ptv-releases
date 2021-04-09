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
using FluentAssertions;
using PTV.Framework.Attributes;
using PTV.Framework.Tests.DummyClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;

namespace PTV.Framework.Tests.Attributes
{
    public class ListPropertyMaxLengthAttributeTests
    {
        // TODO: create a test that passes valid values then null and then invalid values, code checks item == null return success

        [Fact]
        public void ConstructorDoesNotThrow()
        {
            Action act = () => new ListPropertyMaxLengthAttribute(-100, "Value", null);

            act.Should().NotThrow();
        }

        [Fact]
        public void InvalidLengthShouldThrow()
        {
            ListPropertyMaxLengthAttribute attr = new ListPropertyMaxLengthAttribute(-100, "Value", null);

            Action act = () => attr.IsValid("some text");
            act.Should().ThrowExactly<InvalidOperationException>("Invalid length value defined (zero or less than -1 (negative one is magic value in .net framework implementation)).");
        }

        [Fact]
        public void InvalidLengthShouldThrowTestTwo()
        {
            List<SomeDemoObject> inValues = new List<SomeDemoObject>
            {
                new SomeDemoObject
                {
                    Integer = 1,
                    String = "a"
                },
                new SomeDemoObject
                {
                    Integer = 2,
                    String = "ab"
                },
                new SomeDemoObject
                {
                    Integer = 3,
                    String = "cb"
                }
            };

            MaxLengthTestObject obj = new MaxLengthTestObject();
            obj.InvalidLengthDefinedForList.AddRange(inValues);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "InvalidLengthDefinedForList";

            Action act = () => Validator.ValidateProperty(obj.InvalidLengthDefinedForList, ctx);

            act.Should().ThrowExactly<InvalidOperationException>("MaxLength is a negative value.");
        }

        [Fact]
        public void UsedOnInvalidTypeShouldThrow()
        {
            ListPropertyMaxLengthAttribute attr = new ListPropertyMaxLengthAttribute(5, "Value", null);

            Action act = () => attr.IsValid("NotAnIListType");

            act.Should().Throw<InvalidOperationException>().WithMessage("Attribute used on a type that doesn't implement IList.");
        }

        [Fact]
        public void InvalidItemPropertyName()
        {
            List<SomeDemoObject> inValues = new List<SomeDemoObject>
            {
                new SomeDemoObject
                {
                    Integer = 1,
                    String = "a"
                },
                new SomeDemoObject
                {
                    Integer = 2,
                    String = "ab"
                },
                new SomeDemoObject
                {
                    Integer = 3,
                    String = "cb"
                }
            };

            MaxLengthTestObject obj = new MaxLengthTestObject();
            obj.InvalidPropertyNameList.AddRange(inValues);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "InvalidPropertyNameList";

            Action act = () => Validator.ValidateProperty(obj.InvalidPropertyNameList, ctx);

            act.Should().ThrowExactly<ValidationException>("Named property not found from item.");
        }

        [Fact]
        public void ItemTypeValueNullShouldntThrow()
        {
            // validator tries to validate items which Type property value is: TypeValueTest
            // list items have null value, so these should be skipped by the loop
            // currently causes nullreferenceexception because code tries to do item.Type.ToString() and the Type value is null

            List<SomeDemoObject> inValues = new List<SomeDemoObject>
            {
                new SomeDemoObject
                {
                    Integer = 1,
                    String = "a"
                },
                new SomeDemoObject
                {
                    Integer = 2,
                    String = "ab"
                },
                new SomeDemoObject
                {
                    Integer = 3,
                    String = "cb"
                }
            };

            MaxLengthTestObject obj = new MaxLengthTestObject();
            obj.ItemTypeValueNullList.AddRange(inValues);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ItemTypeValueNullList";

            Action act = () => Validator.ValidateProperty(obj.ItemTypeValueNullList, ctx);

            act.Should().NotThrow();
        }

        [Fact]
        public void NullItemInListShouldntMakeTheWholeListValid()
        {
            // COMMENT: if there is a null value in the list, currently makes the whole list values valid

            List<SomeDemoObject> inValues = new List<SomeDemoObject>
            {
                new SomeDemoObject
                {
                    Integer = 1,
                    String = "a"
                },
                new SomeDemoObject
                {
                    Integer = 2,
                    String = "ab"
                },
                new SomeDemoObject
                {
                    Integer = 3,
                    String = "cb"
                },
                null, // this makes whole list valid
                new SomeDemoObject
                {
                    Integer = 4,
                    String = "too long string"
                }
            };

            MaxLengthTestObject obj = new MaxLengthTestObject(inValues);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ValuesList";

            Action act = () => Validator.ValidateProperty(obj.ValuesList, ctx);

            act.Should().ThrowExactly<ValidationException>().WithMessage("Maximum length of property 'String' must be '2'.", "Null item in list shouldn't make all values valid.");
        }

        [Fact]
        public void NullItemPropertyValueShouldntMakeTheWholeListValid()
        {
            // COMMENT: if there is a null value in the list item property, currently makes the whole list values valid

            List<SomeDemoObject> inValues = new List<SomeDemoObject>
            {
                new SomeDemoObject
                {
                    Integer = 1,
                    String = "a"
                },
                new SomeDemoObject
                {
                    Integer = 2,
                    String = "ab"
                },
                new SomeDemoObject
                {
                    Integer = 3,
                    String = "cb"
                },
                new SomeDemoObject
                {
                    Integer = 3,
                    String = null // this makes whole list valid
                },
                new SomeDemoObject
                {
                    Integer = 4,
                    String = "too long string"
                }
            };

            MaxLengthTestObject obj = new MaxLengthTestObject(inValues);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ValuesList";

            Action act = () => Validator.ValidateProperty(obj.ValuesList, ctx);

            act.Should().ThrowExactly<ValidationException>().WithMessage("Maximum length of property 'String' must be '2'.", "Null property value in a single item shouldn't make all list items valid.");
        }

        [Fact]
        public void InvalidValueInValuesList()
        {
            List<SomeDemoObject> inValues = new List<SomeDemoObject>
            {
                new SomeDemoObject
                {
                    Integer = 1,
                    String = "a"
                },
                new SomeDemoObject
                {
                    Integer = 2,
                    String = "ab"
                },
                new SomeDemoObject
                {
                    Integer = 3,
                    String = "cb"
                },
                new SomeDemoObject
                {
                    Integer = 4,
                    String = "too long string"
                }
            };

            MaxLengthTestObject obj = new MaxLengthTestObject(inValues);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ValuesList";

            Action act = () => Validator.ValidateProperty(obj.ValuesList, ctx);

            act.Should().ThrowExactly<ValidationException>().WithMessage("Maximum length of property 'String' must be '2'.");
        }

        [Fact]
        public void PublicIsValidAndProtectedIsValidShouldWorkTheSame()
        {
            // COMMENT: background, for example .Net framework MaxLengthAttribute is implemneted in public IsValid method
            // out inherited attribute was implemented in protected IsValid so that means we have two different implementations that work differently
            // public IsValid(object) and protected IsValid(object, validationContext) should result in same

            // public isvalid validates string length or collection count, custom implementation validate list objects string length

            ListPropertyMaxLengthAttribute attr = new ListPropertyMaxLengthAttribute(2, "String");

            List<SomeDemoObject> inValues = new List<SomeDemoObject>
            {
                new SomeDemoObject
                {
                    Integer = 1,
                    String = "a"
                },
                new SomeDemoObject
                {
                    Integer = 2,
                    String = "ab"
                },
                new SomeDemoObject
                {
                    Integer = 3,
                    String = "cb"
                }
            };

            // this call actually currently checks that there are at maximum 2 items in the list and returns fale because there are 3 items in the list
            bool publicIsValid = attr.IsValid(inValues);

            MaxLengthTestObject obj = new MaxLengthTestObject(inValues);

            ValidationContext ctx = new ValidationContext(obj);
            ctx.MemberName = "ValuesList";

            List<ValidationResult> validationResults = new List<ValidationResult>();

            Validator.TryValidateProperty(inValues, ctx, validationResults).Should().Be(publicIsValid, "Public and protected IsValid methods should result to the same outcome.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void PropertyNameNullOrEmptyShouldThrow(string propertyName)
        {
            Action act = () => new ListPropertyMaxLengthAttribute(5, propertyName);

            act.Should().ThrowExactly<ArgumentException>("Because propertyName attribute is null or empty.");
        }

        [Fact]
        public void ListIsNullIsValid()
        {
            // passing null (IList) to isvalid should be ok
            ListPropertyMaxLengthAttribute attr = new ListPropertyMaxLengthAttribute(100, "Value", null);
            attr.IsValid(null).Should().BeTrue();
        }

        [Fact]
        public void ListIsNullInvalidLengthShouldThrow()
        {
            ListPropertyMaxLengthAttribute attr = new ListPropertyMaxLengthAttribute(-100, "Value", null);

            Action act = () => attr.IsValid(null);

            act.Should().ThrowExactly<InvalidOperationException>().WithMessage("ListPropertyMaxLengthAttribute must have a length value that is greater than zero.", "Because length attribute is negative.");
        }

        [Fact]
        public void ListIsEmptyIsValid()
        {
            ListPropertyMaxLengthAttribute attr = new ListPropertyMaxLengthAttribute(100, "Value", null);
            attr.IsValid(new List<string>()).Should().BeTrue();
        }

        [Fact]
        public void ListIsEmptyInvalidLengthShouldThrow()
        {
            ListPropertyMaxLengthAttribute attr = new ListPropertyMaxLengthAttribute(-100, "Value", null);

            Action act = () => attr.IsValid(new List<string>());

            act.Should().ThrowExactly<InvalidOperationException>().WithMessage("ListPropertyMaxLengthAttribute must have a length value that is greater than zero.", "Because length attribute is negative.");
        }


        #region Helper objects

        internal class MaxLengthTestObject
        {
            internal MaxLengthTestObject() : this(new List<SomeDemoObject>()) { }

            /// <summary>
            /// Creates a new instance
            /// </summary>
            /// <param name="valuesList">initial list for ValuesList property</param>
            internal MaxLengthTestObject(List<SomeDemoObject> valuesList)
            {
                if (valuesList == null)
                {
                    throw new ArgumentNullException(nameof(valuesList));
                }

                ValuesList = valuesList;

                InvalidLengthDefinedForList = new List<SomeDemoObject>();
                InvalidPropertyNameList = new List<SomeDemoObject>();
                ItemTypeValueNullList = new List<SomeDemoObject>();
            }

            /// <summary>
            /// ValuesList is decorated with ListPropertyMaxLength(2, "String") so max length is 2 and value is validated from property named "String"
            /// </summary>
            [ListPropertyMaxLength(2, "String")]
            public List<SomeDemoObject> ValuesList { get; private set; }

            [ListPropertyMaxLength(-100, "String")]
            public List<SomeDemoObject> InvalidLengthDefinedForList { get; private set; }

            [ListPropertyMaxLength(50, "ItemInvalidPropertyName")]
            public List<SomeDemoObject> InvalidPropertyNameList { get; private set; }

            [ListPropertyMaxLength(50, "String", "TypeValueTest")]
            public List<SomeDemoObject> ItemTypeValueNullList { get; private set; }
        }

        #endregion
    }
}
