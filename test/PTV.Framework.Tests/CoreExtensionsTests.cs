using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;
using PTV.Framework;
using PTV.Framework.Tests.DummyClasses;
using System.Collections;

namespace PTV.Framework.Tests
{
    public class CoreExtensionsTests
    {
        [Fact]
        public void TestSafeCallWithNullInstance()
        {
            SomeDemoObject obj = null;

            Action act = () => obj.SafeCall(sdo => {
                sdo.Integer = 100;
                sdo.String = "Hello world";
                sdo.Type = "Demo";
            });

            act.ShouldNotThrow();
        }

        [Fact]
        public void TestSafeCall()
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 9,
                String = "vrk",
                Type = "demo"
            };

            obj.SafeCall(sdo =>
            {
                sdo.Integer = 100;
            });

            obj.Integer.Should().Be(100);
        }

        [Fact]
        public void TestSafeCallReturnWithNullInstance()
        {
            SomeDemoObject obj = null;
            string retval = "vrk"; // just initializing to something, we are expecting this to be null after the safecall

            // implementation returns default(T) when called instance is null, so retval should be null
            retval = obj.SafeCall(sdo =>
            {
                return sdo.ToString();
            });

            retval.Should().BeNull();
        }

        [Fact]
        public void TestSafeCallReturn()
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 9,
                String = "vrk",
                Type = "demo"
            };

            string str = obj.ToString();

            string strB = obj.SafeCall(sdo =>
            {
                return sdo.ToString();
            });

            strB.Should().Be(str);
        }

        [Fact]
        public void TestSafeCallReturnCalculation()
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 9,
                String = "vrk",
                Type = "demo"
            };

            int calcResult = obj.SafeCall(sdo =>
            {
                return sdo.Integer + 1;
            });

            calcResult.Should().Be(10);
        }

        [Fact]
        public void TestSafePropertyFromFirstWithNullInstance()
        {
            List<SomeDemoObject> theList = null;

            string mj = theList.SafePropertyFromFirst(x =>
            {
                return x.ToString();
            });

            mj.Should().BeNull();
        }

        [Fact]
        public void TestSafePropertyFromFirstWithEmptyList()
        {
            List<SomeDemoObject> theList = new List<SomeDemoObject>();

            string mj = theList.SafePropertyFromFirst(x =>
            {
                return x.ToString();
            });

            mj.Should().BeNull();
        }

        [Fact]
        public void TestSafePropertyFromFirstWithList()
        {
            List<SomeDemoObject> theList = new List<SomeDemoObject>();
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 3,
                String = "ptv",
                Type = "demo"
            };
            theList.Add(obj);
            theList.Add(new SomeDemoObject()
            {
                Integer = 4,
                String = "vrk",
                Type = "test"
            });

            int getValue = theList.SafePropertyFromFirst(x =>
            {
                return x.Integer;
            });

            getValue.Should().Be(obj.Integer);
        }

        [Fact]
        public void SetPropertyValueStringUsingExpression()
        {
            string valueToSet = "ptv";

            SomeDemoObject obj = new SomeDemoObject();

            obj.SetPropertyValue(x => x.String, valueToSet);

            obj.String.Should().Be(valueToSet);
        }

        [Fact]
        public void SetPropertyValueIntUsingExpression()
        {
            int valueToSet = 6;

            SomeDemoObject obj = new SomeDemoObject();

            obj.SetPropertyValue(x => x.Integer, valueToSet);

            obj.Integer.Should().Be(valueToSet);
        }

        [Fact]
        public void SetPropertyValueUsingInvalidExpressionShouldThrow()
        {
            string valueToSet = "ptv";

            SomeDemoObject obj = new SomeDemoObject();

            // invalid "memberacces" expression
            System.Linq.Expressions.Expression<Func<SomeDemoObject, string>> f = (sdo) => sdo.ToString();

            Action act = () => obj.SetPropertyValue(f, valueToSet);

            // currently if invalid expression is given the value is not set which can lead to cases that intrdocude hard to find bugs to our app
            // if implementation is changed to throw, change the expected exception type
            act.ShouldThrow<Exception>("Maybe this should throw because the value is not set. Currently just silently does nothing because of invalid memberexpression.");
        }

        [Fact]
        public void CreateLambdaEqualPropertyNameIsNullShouldThrow()
        {
            Action act = () => CoreExtensions.CreateLambdaEqual<SomeDemoObject>(null, "test");
            act.ShouldThrowExactly<ArgumentNullException>("Property name argument is null.");
        }

        [Fact]
        public void CreateLambdaEqualInvalidPropertyNameShouldThrow()
        {
            Action act = () => CoreExtensions.CreateLambdaEqual<SomeDemoObject>("NotFoundPropertyName", "test");
            act.ShouldThrowExactly<ArgumentException>("Named property not found from the object.");
        }

        [Fact]
        public void CreateLambdaEqual()
        {
            int equalityValue = 101;
            var lambda = CoreExtensions.CreateLambdaEqual<SomeDemoObject>("Integer", equalityValue);

            lambda.Should().NotBeNull();

            List<SomeDemoObject> theList = new List<SomeDemoObject>()
            {
                new SomeDemoObject()
                {
                    Integer = 5
                },
                new SomeDemoObject()
                {
                    Type = "Demo"
                },
                new SomeDemoObject()
                {
                    Integer = equalityValue
                }
            };

            var sdo = theList.AsQueryable().FirstOrDefault(lambda);

            sdo.Should().NotBeNull();
            sdo.Integer.Should().Be(equalityValue);
        }

        [Fact]
        public void SetPropertyValueNullInstanceShouldThrow()
        {
            // if instance is null and calling the extension method causes currently TargetInvocationException
            // our implementation should throw argumentnullexception or invalidoperationexception because called on null instead of instance
            SomeDemoObject obj = null;
            Action act = () => obj.SetPropertyValue("Type", "Test");
            act.ShouldThrowExactly<ArgumentException>("Calling SetPropertyValue on null instance.");
        }

        [Fact]
        public void SetPropertyValueNullPropertyNameShouldThrow()
        {
            // if property name is null the code should handle it and throw and not silently fail/do nothing
            SomeDemoObject obj = new SomeDemoObject();
            Action act = () => obj.SetPropertyValue(null, "something");
            act.ShouldThrowExactly<ArgumentNullException>("Property name is null.");
        }

        [Fact]
        public void SetPropertyValueNotFoundPropertyNameShouldThrow()
        {
            // if invalid property name is given it should throw and not just silently skip the setting of the value
            // if silently just skipping the caller doesn't know that the value is not set
            SomeDemoObject obj = new SomeDemoObject();
            Action act = () => obj.SetPropertyValue("NotFound", "something");
            act.ShouldThrowExactly<ArgumentException>("Invalid property name.");
        }

        [Fact]
        public void SetPropertyValue()
        {
            // if invalid property name is given it should throw and not just silently skip the setting of the value
            // if silently just skipping the caller doesn't know that the value is not set
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 100,
                String = "some text",
                Type = "Demo"
            };

            obj.SetPropertyValue("Integer", 65);
            obj.SetPropertyValue("Type", "Test");

            obj.Integer.Should().Be(65);
            obj.Type.Should().Be("Test");
        }

        [Fact]
        public void GetPropertyValueTyped()
        {
            int number = 7;
            string txt = "vrk ptv";
            string typeText = "Test";

            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = number,
                String = txt,
                Type = typeText
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            obj.GetPropertyValue<SomeDemoObject, int>("Integer").Should().Be(number);
            obj.GetPropertyValue<SomeDemoObject, string>("String").Should().Be(txt);
            obj.GetPropertyValue<SomeDemoObject, string>("Type").Should().Be(typeText);

            IList<string> theList = obj.GetPropertyValue<SomeDemoObject, IList<string>>("SomeList");

            theList.Should().NotBeNullOrEmpty();
            theList.Count.Should().Be(2);
        }

        [Fact]
        public void GetPropertyValueTypedPropertyNameIsNullShouldThrow()
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "vrk ptv",
                Type = "Demo"
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            // extension should throw an ArgumentNullException
            Action act = () => obj.GetPropertyValue<SomeDemoObject, int>(null);

            act.ShouldThrowExactly<ArgumentNullException>("Property name is null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void GetPropertyValueTypedInvalidPropertyNameShouldThrow(string propertyName)
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "vrk ptv",
                Type = "Demo"
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            // extension should throw an argumenexception when a known invalid propertyname is passed or a property is not found
            // why? because otherwise the caller doesn't know that the property is missing from the object
            Action act = () => obj.GetPropertyValue<SomeDemoObject, int>(propertyName);

            act.ShouldThrowExactly<ArgumentException>("Known invalid property name.");
        }

        [Fact]
        public void GetPropertyValueTypedPropertyMissingShouldThrow()
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "vrk ptv",
                Type = "Demo"
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            // extension should throw an argumenexception when a property is not found
            // why? because otherwise the caller doesn't know that the property is missing from the object
            Action act = () => obj.GetPropertyValue<SomeDemoObject, int>("NotFoundPropertyName");

            act.ShouldThrowExactly<ArgumentException>("Property not found from object.");
        }

        [Fact]
        public void GetPropertyValueObject()
        {
            int number = 7;
            string txt = "vrk ptv";
            string typeText = "Test";

            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = number,
                String = txt,
                Type = typeText
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            obj.GetPropertyObjectValue("Integer").Should().Be(number);
            obj.GetPropertyObjectValue("String").Should().Be(txt);
            obj.GetPropertyObjectValue("Type").Should().Be(typeText);

            IList<string> theList = obj.GetPropertyObjectValue("SomeList") as IList<string>;

            theList.Should().NotBeNullOrEmpty();
            theList.Count.Should().Be(2);
        }

        [Fact]
        public void GetPropertyValueObjectPropertyNameIsNullShouldThrow()
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "vrk ptv",
                Type = "Demo"
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            // extension should throw an ArgumentNullException
            Action act = () => obj.GetPropertyObjectValue(null);

            act.ShouldThrowExactly<ArgumentNullException>("Property name is null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void GetPropertyValueObjectInvalidPropertyNameShouldThrow(string propertyName)
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "vrk ptv",
                Type = "Demo"
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            // extension should throw an argumenexception when a known invalid propertyname is passed
            // why? because otherwise the caller doesn't know that the property is missing from the object
            Action act = () => obj.GetPropertyObjectValue(propertyName);

            act.ShouldThrowExactly<ArgumentException>("Known invalid property name.");
        }

        [Fact]
        public void GetPropertyValueObjectPropertyMissingShouldThrow()
        {
            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "vrk ptv",
                Type = "Demo"
            };
            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            // extension should throw an argumenexception when a property is not found
            // why? because otherwise the caller doesn't know that the property is missing from the object
            Action act = () => obj.GetPropertyValue<SomeDemoObject, int>("NotFoundPropertyName");

            act.ShouldThrowExactly<ArgumentException>("Property not found from object.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("1")]
        public void EnumerableIsNullOrEmpty(string mode)
        {
            IEnumerable<string> ie = null;

            if (mode == null)
            {
                ie.IsNullOrEmpty().Should().BeTrue();
            }
            else if (mode.Length == 0)
            {
                ie = new List<string>().AsEnumerable();

                ie.IsNullOrEmpty().Should().BeTrue();
            }
            else if(mode.Length == 1)
            {
                ie = new List<string>() { "ptv vrk" }.AsEnumerable();

                ie.IsNullOrEmpty().Should().BeFalse();
            }
            else
            {
                throw new ArgumentException("Invalid mode parameter value.", nameof(mode));
            }
        }

        [Fact]
        public void ReadOnlyListInclusiveToList()
        {
            List<string> theList = new List<string>()
            {
                "ptv", "vrk", "kapa"
            };

            int originalListCount = theList.Count;

            // create a readonly wrapper
            var roList = theList.AsReadOnly();

            var list = roList.InclusiveToList();

            list.Should().NotBeNull();
            list.Count.Should().Be(originalListCount);

            int count = list.Count;

            // and we should be able to add new items to the returned list
            list.Add("kehä I");
            // the count should be increased by one
            list.Count.Should().Be(count + 1);
            // the old list shouldn't change
            theList.Count.Should().Be(originalListCount);
        }

        [Fact]
        public void WritableListInclusiveToList()
        {
            List<string> theList = new List<string>()
            {
                "ptv", "vrk", "kapa"
            };
            
            // because the passed in list is not readonlycollection the returned list should be the same instance
            var list = theList.InclusiveToList();

            list.Should().NotBeNull();
            list.Count.Should().Be(theList.Count);
            // object references should be the same
            list.Should().BeSameAs(theList);

            int count = list.Count;

            // and we should be able to add new items to the returned list
            list.Add("kehä I");
            // the count should be increased by one
            list.Count.Should().Be(count + 1);
            // the old list count should also increase
            theList.Count.Should().Be(list.Count);
        }

        [Fact]
        public void NullListInclusiveToList()
        {
            IReadOnlyList<string> nullList = null;

            List<string> list = nullList.InclusiveToList();

            list.Should().BeNull();
        }

        [Fact]
        public void EmptyReadOnlyListInclusiveToList()
        {
            List<string> theList = new List<string>(){};

            var roList = theList.AsReadOnly();

            var list = ((IReadOnlyList<string>)roList).InclusiveToList();
            int count = list.Count;
            list.Add("kehä I");

            list.Count.Should().Be(count + 1);
        }

        [Fact]
        public void AddAndReturn()
        {
            List<SomeDemoObject> theList = new List<SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 8,
                String = "text",
                Type = "demo"
            };

            var returnedObj = theList.AddAndReturn(obj);

            returnedObj.Should().BeSameAs(obj);
        }

        [Fact]
        public void AddAndReturnHandlesAddNullItem()
        {
            List<SomeDemoObject> theList = new List<SomeDemoObject>();
            var addedObj = theList.AddAndReturn(null);

            addedObj.Should().BeNull();
        }

        [Fact]
        public void AddAndReturnShouldHandleNullCollection()
        {
            ICollection<SomeDemoObject> col = null;

            Action act = () => col.AddAndReturn(new SomeDemoObject());

            act.ShouldThrowExactly<ArgumentNullException>("Collection instance is null.").WithMessage("Value cannot be null.*Parameter name: collection");
        }

        [Fact]
        public void TestGenericListIsEnumerable()
        {
            typeof(List<string>).IsEnumerable().Should().BeTrue();
        }

        [Fact]
        public void TestArrayListIsNotEnumerable()
        {
            // extension assumes generic ienumerable<T>
            typeof(ArrayList).IsEnumerable().Should().BeFalse();
        }

        [Fact]
        public void TestStringIsEnumerable()
        {
            // extension assumes generic ienumerable<T>
            typeof(string).IsEnumerable().Should().BeTrue();
        }

        [Fact]
        public void TestStringExcludedIsNotEnumerable()
        {
            // extension assumes generic ienumerable<T>
            typeof(string).IsEnumerable(new List<Type>() { typeof(string) }).Should().BeFalse();
        }

        [Fact]
        public void IsEnumerableShouldHandleNullInstance()
        {
            Type t = null;
            Action act = () => t.IsEnumerable();

            // parameter name: target would actually be the correct one as the current exception is thrown by framework
            act.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter name: type");
        }

        [Fact]
        public void TestGenericListIsCollection()
        {
            typeof(List<string>).IsCollection().Should().BeTrue();
        }

        [Fact]
        public void TestArrayListIsNotCollection()
        {
            // extension assumes generic icollection<T>
            typeof(ArrayList).IsCollection().Should().BeFalse();
        }

        [Fact]
        public void IsCollectionShouldHandleNullInstance()
        {
            Type t = null;
            Action act = () => t.IsCollection();

            // parameter name: target would actually be the correct one as the current exception is thrown by framework
            act.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.*Parameter name: type");
        }

        [Fact]
        public void TestRunInThreadAndWait()
        {
            Action act = () => Console.WriteLine("Output from test method: TestRunInThreadAndWait");

            CoreExtensions.RunInThreadAndWait(act);
        }

        [Fact]
        public void TestRunInThreadAndWaitWithNullAction()
        {
            // this will crash the whole app domain currently because there is NullReferenceException in the call
            CoreExtensions.RunInThreadAndWait(null);

            // NOTE! Decision should the RunInThreadAndWait handle the possible exception in the thread and throw own exception
            // or let it go to appdomain unhandled exception and tear down the appdomain
        }

        [Theory]
        [InlineData(false)] // use null value
        [InlineData(true)] // use guid empty
        public void TestNullableGuidIsNotAssigned(bool useEmpty)
        {
            Guid? g = null;

            if (useEmpty)
            {
                g = Guid.Empty;
            }

            g.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void TestNullableGuidIsAssigned()
        {
            Guid? g = Guid.NewGuid();
            g.IsAssigned().Should().BeTrue();
        }

        [Fact]
        public void TestGuidIsAssigned()
        {
            Guid g = Guid.NewGuid();
            g.IsAssigned().Should().BeTrue();
        }

        [Fact]
        public void TestGuidIsNotAssigned()
        {
            Guid g = Guid.Empty;
            g.IsAssigned().Should().BeFalse();
        }

        [Fact]
        public void GetGuidFromStringValues()
        {
            string valueToUse = "SomeDemoObject";
            string forType = null;

            Guid g = valueToUse.GetGuid();
            Guid gNull = valueToUse.GetGuid(forType);

            forType = string.Empty;

            Guid gEmpty = valueToUse.GetGuid(forType);

            // returned guid shouldn't be empty
            g.Should().NotBeEmpty();

            // using default value for type, null or empty string should all return the same "new" guid
            g.Should().Be(gNull);
            gNull.Should().Be(gEmpty);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetGuidUsingNullAndEmptyValues(string value)
        {
            Guid g = value.GetGuid();
            g.Should().NotBeEmpty();
        }

        [Fact]
        public void GetGuidFromStringValuesTyped()
        {
            string valueToUse = "some text";

            Guid g = valueToUse.GetGuid<SomeDemoObject>();
            Guid gnd = valueToUse.GetGuid<SomeDemoObject>();

            // returned guid shouldn't be empty
            g.Should().NotBeEmpty();

            // should be the same guid always for the same string using the same type
            g.Should().Be(gnd);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetGuidUsingNullAndEmptyValuesTyped(string value)
        {
            Guid g = value.GetGuid<SomeDemoObject>();
            g.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GetGuidUsingNullAndEmptyValuesWithType(string value)
        {
            Guid g = value.GetGuid(typeof(SomeDemoObject));
            g.Should().NotBeEmpty();
        }

        [Fact]
        public void GetGuidUsingTypeShouldHandleNull()
        {
            // passing null as type should work because other overloads also work with null type (where passed as string)
            Type t = null;
            Guid g = "some string".GetGuid(t);

            g.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")] // Will be Guid.Empty
        [InlineData(" ")] // Will be Guid.Empty
        [InlineData("  ")] // Will be Guid.Empty
        [InlineData("5-5-5-5")]
        [InlineData("vrk")]
        [InlineData("ca761232ed4211cebacd00aa0057b22")] // missing one character
        [InlineData("CA761232-ED4211CE-BACD-00AA0057B223")] // missing one '-' character
        [InlineData("{CA761232-ED42-11CE-BACD-00AA0057B223")] // missing ending '}' character
        [InlineData("(CA761232-ED42-11CE-BACD-00AA0057B223")] // missing ending ')' character
        [InlineData("{0xCA761232, 0xED42, 0x11CE, {0xBA, 0xCD, 0x00, 0xAA, 0x00, 0x57, 0xB2, 0x23}")] // missing ending '}' character
        public void ParseInvalidStringToGuid(string valueToParse)
        {
            Guid? g = valueToParse.ParseToGuid();
            g.HasValue.Should().BeFalse();
        }

        [Theory]
        [InlineData("ca761232ed4211cebacd00aa0057b222")]
        [InlineData("CA761232-ED42-11CE-BACD-00AA0057B223")]
        [InlineData("{CA761232-ED42-11CE-BACD-00AA0057B223}")]
        [InlineData("(CA761232-ED42-11CE-BACD-00AA0057B223)")]
        [InlineData("{0xCA761232, 0xED42, 0x11CE, {0xBA, 0xCD, 0x00, 0xAA, 0x00, 0x57, 0xB2, 0x23}}")]
        public void ParseValidStringToGuid(string valueToParse)
        {
            Guid? g = valueToParse.ParseToGuid();
            g.HasValue.Should().BeTrue();
        }

        [Fact]
        public void TryGetDictionaryValueByKey()
        {
            Dictionary<string, SomeDemoObject> dict = new Dictionary<string, SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 65
            };

            dict.Add("demo", obj);

            var foundObj = dict.TryGet("demo");

            foundObj.Should().NotBeNull();
            foundObj.Should().BeSameAs(obj);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("")]
        public void TryGetDictionaryValueByKeyNotFound(string keyValue)
        {
            Dictionary<string, SomeDemoObject> dict = new Dictionary<string, SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 65
            };

            dict.Add("demo", obj);

            var foundObj = dict.TryGet(keyValue);

            foundObj.Should().BeNull();
        }

        [Fact]
        public void TryGetDictionaryValueWithNullThrows()
        {
            Dictionary<string, SomeDemoObject> dict = new Dictionary<string, SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 65
            };

            dict.Add("demo", obj);

            Action act = () => dict.TryGet(null);

            act.ShouldThrowExactly<ArgumentNullException>("Key cannot be null.");
        }

        [Fact]
        public void TryGetDictionaryValueWithNullInstanceShouldThrow()
        {
            Dictionary<string, SomeDemoObject> dict = null;

            Action act = () => dict.TryGet("somekey");

            act.ShouldThrowExactly<ArgumentNullException>("Implementation should handle the extension used on null instance.");
        }

        [Fact]
        public void TryGetIDictionaryValueByKey()
        {
            IDictionary<string, SomeDemoObject> dict = new Dictionary<string, SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 65
            };

            dict.Add("demo", obj);

            var foundObj = dict.TryGet("demo");

            foundObj.Should().NotBeNull();
            foundObj.Should().BeSameAs(obj);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("")]
        public void TryGetIDictionaryValueByKeyNotFound(string keyValue)
        {
            IDictionary<string, SomeDemoObject> dict = new Dictionary<string, SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 65
            };

            dict.Add("demo", obj);

            var foundObj = dict.TryGet(keyValue);

            foundObj.Should().BeNull();
        }

        [Fact]
        public void TryGetIDictionaryValueWithNullThrows()
        {
            IDictionary<string, SomeDemoObject> dict = new Dictionary<string, SomeDemoObject>();

            SomeDemoObject obj = new SomeDemoObject()
            {
                Integer = 65
            };

            dict.Add("demo", obj);

            Action act = () => dict.TryGet(null);

            act.ShouldThrowExactly<ArgumentNullException>("Key cannot be null.");
        }

        [Fact]
        public void TryGetIDictionaryValueWithNullInstanceShouldThrow()
        {
            IDictionary<string, SomeDemoObject> dict = null;

            Action act = () => dict.TryGet("somekey");

            act.ShouldThrowExactly<ArgumentNullException>("Implementation should handle the extension used on null instance.");
        }

        [Fact]
        public void TestExtractAllInnerExceptionsWithNull()
        {
            string message = CoreExtensions.ExtractAllInnerExceptions(null);
            message.Should().BeEmpty();
        }

        [Fact]
        public void TestExtractAllInnerExceptions()
        {
            // the implementation only concatenates exception type and message (no stacktrace)
            // format is: excp.GetType().Name + " : " + excp.Message + "\n" and this same is repeated as long there are innerexceptions

            ArgumentNullException nex = new ArgumentNullException("nex", "Parameter cannot be null.");
            ArgumentException argEx = new ArgumentException("Invalid argument supplied.", nex);
            Exception ex = new Exception("An error occured when calling typography service.", argEx);

            string extractedMessage = CoreExtensions.ExtractAllInnerExceptions(ex);

            extractedMessage.Should().NotBeNullOrWhiteSpace();

            // create the expected message parts separated with the \n
            // remove \r\n so that we can split with \n easily
            string partOne = $"{ex.GetType().Name} : {ex.Message}".Replace("\r\n", string.Empty);
            string partTwo = $"{argEx.GetType().Name} : {argEx.Message}".Replace("\r\n", string.Empty);
            string partThree = $"{nex.GetType().Name} : {nex.Message}".Replace("\r\n", string.Empty);

            // remove \r\n also from the result (when parameter name is given it will be part of the ex.Message)
            extractedMessage = extractedMessage.Replace("\r\n", string.Empty);

            // don't remove empty entries
            var splittedMsgs = extractedMessage.Split(new string[] { "\n" }, StringSplitOptions.None);

            splittedMsgs.Length.Should().Be(4, "Count should be 4 because there are 3 exceptions and after last exception there will be '\n' which will produce the extra string in the array.");

            splittedMsgs[0].Should().Be(partOne);
            splittedMsgs[1].Should().Be(partTwo);
            splittedMsgs[2].Should().Be(partThree);
        }

        [Fact]
        public void RunWithRetriesShouldHandleNullAction()
        {
            Func<bool> actIn = null;

            Action act = () => CoreExtensions.RunWithRetries(5, actIn);

            act.ShouldNotThrow("Because there is currently exception block that hides all exceptions.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void RunWithRetriesShouldThrowOnInvalidRetriesValue(int retries)
        {
            // action is never called if the value is zero or negative
            RetriesTestObject jt = new RetriesTestObject()
            {
                Treshold = 3
            };

            Action act = () => CoreExtensions.RunWithRetries(retries, jt.IncreaseCount);
            act.ShouldThrowExactly<ArgumentException>("Invalid value given for retries count.");
        }

        [Fact]
        public void TestRunWithRetriesAllRetries()
        {
            // treshold will not be reached so the method should be called as many times retries is defined

            RetriesTestObject jt = new RetriesTestObject()
            {
                Treshold = 100
            };

            int retries = 3;

            CoreExtensions.RunWithRetries(retries, jt.IncreaseCount);

            jt.Count.Should().Be(retries);
        }

        [Fact]
        public void TestRunWithRetriesExitEarly()
        {
            // treshold will be reached so the method should not be called as many times as defined by retries
            // method will return true as soon as count reaches treshold and the implementation stops retrying when true is returned

            int treshold = 3;

            RetriesTestObject jt = new RetriesTestObject()
            {
                Treshold = treshold
            };

            CoreExtensions.RunWithRetries(5, jt.IncreaseCount);

            jt.Count.Should().Be(treshold);
        }

        [Fact]
        public void TestSerializeAndDeserialize()
        {
            SerTestObject obj = new SerTestObject()
            {
                Number = 100,
                Text = "Demo"
            };

            obj.SomeList.Add("ptv");
            obj.SomeList.Add("vrk");

            var serialized = obj.SerializeObject();

            serialized.Should().NotBeNullOrWhiteSpace();

            SerTestObject desObj = serialized.DeserializeObject<SerTestObject>();

            desObj.Should().NotBeNull();

            desObj.Number.Should().Be(obj.Number);
            desObj.Text.Should().Be(obj.Text);

            desObj.SomeList.Count.Should().Be(obj.SomeList.Count); // this is not really needed as the below line covers this too
            Enumerable.SequenceEqual(desObj.SomeList.OrderBy(k => k), obj.SomeList.OrderBy(t => t)).Should().BeTrue();
        }

        [Fact]
        public void SerializeObjectShouldHandleNullInstance()
        {
            SerTestObject obj = null;
            Action act = () => obj.SerializeObject();
            // currently throws NullReferenceException
            act.ShouldThrowExactly<ArgumentNullException>("Object to serialize is null.");
        }

        [Fact]
        public void DeserializeObjectShouldHandleNullInstance()
        {
            string des = null;

            Action act = () => des.DeserializeObject<SerTestObject>();

            act.ShouldThrowExactly<ArgumentNullException>("String to deserialize is null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void DeserializeObjectShouldHandleEmptyStrings(string toDeserialize)
        {
            Action act = () => toDeserialize.DeserializeObject<SerTestObject>();

            act.ShouldThrowExactly<ArgumentException>("String to deserialize is empty or whitespaces. Would be better to throw ArgumetnException by the implementation that it is more clear to caller vs the currently thrown InvalidOperationException about bad xml.");
        }


        [Theory]
        [InlineData(-100000)]
        [InlineData(-15000)]
        [InlineData(-2795)]
        [InlineData(-101)]
        [InlineData(-10)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(100000)]
        [InlineData(15000)]
        [InlineData(2795)]
        [InlineData(101)]
        [InlineData(10)]
        [InlineData(1)]
        public void TestParseToInt(int number)
        {
            // functionality relies on current thread localization/number formatter
            string n = number.ToString();

            int? parsed = n.ParseToInt();

            parsed.Should().HaveValue();
            parsed.Value.Should().Be(number);
        }

        [Fact]
        public void TestParseToIntHandlesNullInstance()
        {
            string toParse = null;
            int? parsed = toParse.ParseToInt();

            parsed.Should().NotHaveValue();
        }

        [Theory]
        [InlineData(-100000.01D)]
        [InlineData(-15000.02D)]
        [InlineData(-2795.00D)]
        [InlineData(-101.00D)]
        [InlineData(-10.99D)]
        [InlineData(-1.0D)]
        [InlineData(0.00D)]
        [InlineData(100000.06D)]
        [InlineData(15000.75D)]
        [InlineData(2795.60D)]
        [InlineData(101.52D)]
        [InlineData(10.1D)]
        [InlineData(1.00000D)]
        public void TestParseToDouble(double number)
        {
            // functionality relies on current thread localization/number formatter
            string n = number.ToString();

            double? parsed = n.ParseToDouble();

            parsed.Should().HaveValue();
            parsed.Value.Should().Be(number);
        }

        [Fact]
        public void TestParseToDoubleHandlesNullInstance()
        {
            string toParse = null;
            double? parsed = toParse.ParseToDouble();

            parsed.Should().NotHaveValue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ptv")]
        [InlineData("87927892")]
        [InlineData("ca761232ed4211cebacd00aa0057b22")] // missing one character
        [InlineData("CA761232-ED4211CE-BACD-00AA0057B223")] // missing one '-' character
        [InlineData("{CA761232-ED42-11CE-BACD-00AA0057B223")] // missing ending '}' character
        [InlineData("(CA761232-ED42-11CE-BACD-00AA0057B223")] // missing ending ')' character
        [InlineData("{0xCA761232, 0xED42, 0x11CE, {0xBA, 0xCD, 0x00, 0xAA, 0x00, 0x57, 0xB2, 0x23}")] // missing ending '}' character
        public void ParseToGuidWithExeptionInvalidValues(string guidstring)
        {
            Action act = () => guidstring.ParseToGuidWithExeption();
            act.ShouldThrowExactly<Exception>().WithMessage($"Cannot parse '{guidstring}' to type of Guid.");
        }

        [Theory]
        [InlineData("ca761232ed4211cebacd00aa0057b222")]
        [InlineData("CA761232-ED42-11CE-BACD-00AA0057B223")]
        [InlineData("{CA761232-ED42-11CE-BACD-00AA0057B223}")]
        [InlineData("(CA761232-ED42-11CE-BACD-00AA0057B223)")]
        [InlineData("{0xCA761232, 0xED42, 0x11CE, {0xBA, 0xCD, 0x00, 0xAA, 0x00, 0x57, 0xB2, 0x23}}")]
        public void ParseToGuidWithExeptionValidValues(string guidstring)
        {
            Guid g = guidstring.ParseToGuidWithExeption();
            g.Should().NotBeEmpty();
        }

        [Fact]
        public void TestParseToEnum()
        {
            var parsed = "Color".Parse<DummyEnum>();
            parsed.Should().HaveFlag(DummyEnum.Color);
        }

        [Fact]
        public void TestParseToEnumNotEnumTypeThrows()
        {
            Action act = () => "Name".Parse<SomeDemoObject>();
            act.ShouldThrowExactly<ServiceManager.PtvArgumentException>("Because generic type is not enum type.");
        }

        [Fact]
        public void TestParseToEnumInvalidValueShouldThrow()
        {
            Action act = () => "PTV-VRK".Parse<DummyEnum>();
            act.ShouldThrowExactly<Exception>().WithMessage("The field is invalid. Please use one of these: *");
        }

        [Fact]
        public void TestParseToEnumNullValueShouldThrow()
        {
            string val = null;
            Action act = () => val.Parse<DummyEnum>();
            act.ShouldThrowExactly<Exception>().WithMessage("The field is invalid. Please use one of these: *");
        }

        [Fact]
        public void TestTimespanToString()
        {
            TimeSpan ts = new TimeSpan(5, 7, 1);
            string strts = ts.ConvertToString();

            strts.Should().Be("5:07:01");
        }

        [Fact]
        public void TestTimespanToStringWithDays()
        {
            TimeSpan ts = new TimeSpan(3, 5, 17, 1);
            string strts = ts.ConvertToString();

            strts.Should().Be("77:17:01");
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void TestPositiveOrZero(int value)
        {
            int result = value.PositiveOrZero();

            if (value < 0)
            {
                result.Should().Be(0);
            }
            else
            {
                result.Should().Be(value);
            }
        }

        [Fact]
        public void TestEnumNameToCamelCaseString()
        {
            var strResult = DummyEnum.AlternateName.ToCamelCase();

            strResult.Should().Be("alternateName");
        }

        [Fact]
        public void TestEnumNameToCamelCaseStringWeirdOption()
        {
            var strResult = DummyEnum.Weird_Option.ToCamelCase();

            strResult.Should().Be("weirdOption");
        }

        [Fact]
        public void TestEnumNameToCamelCaseStringSmallLetters()
        {
            var strResult = DummyEnum.small_letters.ToCamelCase();

            strResult.Should().Be("smallLetters");
        }

        /// <summary>
        /// Test class for RunWithRetries
        /// </summary>
        internal class RetriesTestObject
        {
            public int Treshold { get; set; }

            public int Count { get; set; }

            /// <summary>
            /// Returns true if Count is equal or greater than the treshold, otherwise false.
            /// </summary>
            /// <returns></returns>
            public bool IncreaseCount()
            {
                Count++;

                return Count >= Treshold;
            }
        }


        public class SerTestObject
        {
            public SerTestObject()
            {
                SomeList = new List<string>();
            }

            public int Number { get; set; }
            public string Text { get; set; }
            public List<string> SomeList { get; set; }
        }
    }
}
