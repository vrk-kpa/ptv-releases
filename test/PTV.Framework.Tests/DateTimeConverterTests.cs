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
using System.Text;
using FluentAssertions;
using Xunit;
using PTV.Framework;

namespace PTV.Framework.Tests
{
    public class DateTimeConverterTests
    {
        // ref: https://www.epochconverter.com/
        // PTV DateTimeConverter uses milliseconds, not seconds as the smallest unit
        // note: PTV DateTimeConverter is not Unix time converter as it uses milliseconds instead of seconds
        // "Unix time (also known as POSIX time or epoch time) is a system for describing a point in time, defined as the number of seconds that have elapsed since 00:00:00 Coordinated Universal Time (UTC)"

        [Fact]
        public void FromEpochTimeValueIsNull()
        {
            long? time = null;

            DateTime? dt = time.FromEpochTime();

            dt.Should().BeNull();
        }

        [Fact]
        public void EpochTimeFromAndToValueIsNegativeNullable()
        {
            // this is milliseconds
            long? time = -1;
            DateTime dt = new DateTime(1969, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);

            DateTime? dtResult = time.FromEpochTime();

            // value will be: 1969-12-31 23:59:59.999

            dtResult.Should().Be(dt);

            dtResult.ToEpochTime().Should().Be(time);
        }

        [Fact]
        public void EpochTimeFromAndToValueIsZeroNullable()
        {
            // this is milliseconds
            long? time = 0;
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            DateTime? dtResult = time.FromEpochTime();

            // value will be: 1970-01-01

            dtResult.Should().Be(dt);

            dtResult.ToEpochTime().Should().Be(time);
        }

        [Fact]
        public void EpochTimeFromAndToValueIsPositiveNullable()
        {
            // this is milliseconds
            long? time = 1;
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 1, DateTimeKind.Utc);

            DateTime? dtResult = time.FromEpochTime();

            // value will be: 1970-01-01

            dtResult.Should().Be(dt);

            dtResult.ToEpochTime().Should().Be(time);
        }

        [Fact]
        public void EpochTimeFromAndToValueY2KNullable()
        {
            // this is milliseconds
            long? time = 946684800000;
            DateTime dt = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            DateTime? dtResult = time.FromEpochTime();

            // value will be: 2000-01-01

            dtResult.Should().Be(dt);

            dtResult.ToEpochTime().Should().Be(time);
        }

        [Fact]
        public void EpochTimeFromAndToValueIsNegative()
        {
            // this is milliseconds
            long time = -1;
            DateTime dt = new DateTime(1969, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);

            DateTime dtResult = time.FromEpochTime();

            // value will be: 1969-12-31 23:59:59.999

            dtResult.Should().Be(dt);

            dtResult.ToEpochTime().Should().Be(time);
        }

        [Fact]
        public void EpochTimeFromAndToValueIsZero()
        {
            // this is milliseconds
            long time = 0;
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            DateTime dtResult = time.FromEpochTime();

            // value will be: 1970-01-01

            dtResult.Should().Be(dt);

            dtResult.ToEpochTime().Should().Be(time);
        }

        [Fact]
        public void EpochTimeFromAndToValueIsPositive()
        {
            // this is milliseconds
            long time = 1;
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 1, DateTimeKind.Utc);

            DateTime dtResult = time.FromEpochTime();

            // value will be: 1970-01-01

            dtResult.Should().Be(dt);

            dtResult.ToEpochTime().Should().Be(time);
        }

        [Fact]
        public void EpochTimeFromAndToValueY2K()
        {
            // this is milliseconds
            long time = 946684800000;
            DateTime dt = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            DateTime dtResult = time.FromEpochTime();

            // value will be: 2000-01-01

            dtResult.Should().Be(dt);

            dtResult.ToEpochTime().Should().Be(time);
        }

        [Fact]
        public void ToEpochTimeUsingLocalTime()
        {
            // test that when local time is used the implementation still uses UTC for the timestamp

            // this is milliseconds
            long time = 946684800000;
            DateTime dt = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);

            dt.ToEpochTime().Should().Be(time);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void TimeOfDayToEpochTime(string dayTime)
        {
            dayTime.TimeOfDayToEpochTime().Should().BeNull();
        }

        [Fact]
        public void TimeOfDayToEpochTimeY2KString()
        {
            // this is milliseconds
            long? time = 946684800000;
            DateTime dt = new DateTime(2000, 1, 1);

            // Comment: Martin & Jaroslav, check this method TimeOfDayToEpochTime
            // Is it really needed as no code uses it
            // It is unclear what it is actually supposed to do
            // Code looks to expect the string to be datetime but argument name indicates something else
            // argument used to be called dateTime vs now it is dayTime

            // on purpose not specifiying the IFormatter as we can't specify it to the extension implementation so just hoping the same is used when parsing the string to date
            string dayTime = dt.ToString();

            long? result = dayTime.TimeOfDayToEpochTime();

            result.Should().NotBeNull();
            result.Should().Be(time);
        }

        [Fact]
        public void ToEpochTimeOfDayNullableNullValue()
        {
            TimeSpan? ts = null;

            ts.ToEpochTimeOfDay().Should().BeNull();
        }

        [Fact]
        public void ToEpochTimeOfDayNullableTimeSpanValue()
        {
            TimeSpan? ts = new TimeSpan(10, 10, 5);
            // 10h 10m 5s => converted to milliseconds
            long? expectedValue = 36605000;

            // returns milliseconds of the timespan
            ts.ToEpochTimeOfDay().Should().Be(expectedValue);
        }

        [Fact]
        public void ToEpochTimeOfDayNullableTimeSpanValueOverlapsDay()
        {
            TimeSpan? ts = new TimeSpan(3, 10, 10, 5);
            // implementation ignores day value
            // 10h 10m 5s => converted to milliseconds
            long? expectedValue = 36605000;

            // returns milliseconds of the timespan
            ts.ToEpochTimeOfDay().Should().Be(expectedValue);
        }

        [Fact]
        public void ToEpochTimeOfDayTimeSpanValue()
        {
            TimeSpan ts = new TimeSpan(10, 10, 3);
            // 10h 10m 3s => converted to milliseconds
            long expectedValue = 36603000;

            // returns milliseconds of the timespan
            ts.ToEpochTimeOfDay().Should().Be(expectedValue);
        }

        [Fact]
        public void ToEpochTimeOfDayTimeSpanValueOverlapsDay()
        {
            TimeSpan ts = new TimeSpan(3, 10, 10, 3);
            // implementation ignores day value
            // 10h 10m 3s => converted to milliseconds
            long expectedValue = 36603000;

            // returns milliseconds of the timespan
            ts.ToEpochTimeOfDay().Should().Be(expectedValue);
        }

        [Fact]
        public void FromEpochTimeOfDayNullableValueIsNull()
        {
            long? time = null;

            TimeSpan? result = time.FromEpochTimeOfDay();

            result.Should().BeNull();
        }

        [Fact]
        public void FromEpochTimeOfDayNullable()
        {
            // this is milliseconds
            long? time = 36603000;
            TimeSpan? expected = new TimeSpan(10, 10, 3);

            TimeSpan? result = time.FromEpochTimeOfDay();

            result.Should().Be(expected);
        }

        [Fact]
        public void FromEpochTimeOfDay()
        {
            // this is milliseconds
            long time = 36603000;
            TimeSpan expected = new TimeSpan(10, 10, 3);

            TimeSpan result = time.FromEpochTimeOfDay();

            result.Should().Be(expected);
        }

        [Fact]
        public void FromEpochTimeOfDayValueIsNegative()
        {
            // this is milliseconds
            long time = -36603000;
            TimeSpan expected = new TimeSpan(-10, -10, -3);

            TimeSpan result = time.FromEpochTimeOfDay();

            result.Should().Be(expected);
        }

        [Fact]
        public void FromEpochTimeOfDayValueIsZero()
        {
            // this is milliseconds
            long time = 0;
            TimeSpan expected = new TimeSpan(0, 0, 0);

            TimeSpan result = time.FromEpochTimeOfDay();

            result.Should().Be(expected);
        }

        [Fact]
        public void ToAndFromEpochTimeOfDay()
        {
            TimeSpan? ts = new TimeSpan(10, 10, 5);

            long? tmpresult = ts.ToEpochTimeOfDay();

            TimeSpan? result = tmpresult.FromEpochTimeOfDay();

            result.Should().Be(ts);
        }
    }
}
