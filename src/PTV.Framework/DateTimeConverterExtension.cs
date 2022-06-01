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
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Framework
{
    public static class DateTimeConverterExtension
    {
        /// <summary>
        /// Converts DateTime into Epoch (Unix) time representation
        /// </summary>
        /// <param name="dateTime">Input DateTime type</param>
        /// <returns>Epoch time representation as long type</returns>
        public static long? ToEpochTime(this DateTime? dateTime)
        {
            if (dateTime == null) return null;
            return ToEpochTime(dateTime.Value);
        }

        /// <summary>
        /// Converts Epoch (Unix) time into DateTime representation
        /// </summary>
        /// <param name="unixTime">Input long with Epoch time</param>
        /// <returns>DateTime format of date and time</returns>
        public static DateTime? FromEpochTime(this long? unixTime)
        {
            if (unixTime == null) return null;
            return FromEpochTime(unixTime.Value);
        }

        /// <summary>
        /// Converts DateTime into Epoch (Unix) time representation
        /// </summary>
        /// <param name="dateTime">Input DateTime type</param>
        /// <returns>Epoch time representation as long type</returns>
        public static long ToEpochTime(this DateTime dateTime)
        {
            var utcDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return new DateTimeOffset(utcDateTime).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Converts Epoch (Unix) time into DateTime representation
        /// </summary>
        /// <param name="unixTime">Input long with Epoch time</param>
        /// <returns>DateTime format of date and time</returns>
        public static DateTime FromEpochTime(this long unixTime)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTime).DateTime;
        }

        /// <summary>
        /// Converts Time of day in <see cref="TimeSpan"/> format into Epoch (Unix) time representation
        /// </summary>
        /// <param name="dayTime">Input string with time of day data</param>
        /// <returns>Epoch time representation as long type</returns>
        public static long? ToEpochTimeOfDay(this TimeSpan? dayTime)
        {
            if (!dayTime.HasValue)
            {
                return null;
            }
            return ToEpochTimeOfDay(dayTime.Value);
        }

        /// <summary>
        /// Converts Time of day in <see cref="TimeSpan"/> format into Epoch (Unix) time representation
        /// </summary>
        /// <param name="dayTime">Input string with time of day data</param>
        /// <returns>Epoch time representation as long type</returns>
        public static long ToEpochTimeOfDay(this TimeSpan dayTime)
        {
            return (dayTime.Ticks % new TimeSpan(1, 0, 0, 0).Ticks)/10000;
        }

        /// <summary>
        /// Converts Epoch (Unix) time representation to Time of day into <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="dayTime">Input string with time of day data</param>
        /// <returns>Epoch time representation as long type</returns>
        public static TimeSpan? FromEpochTimeOfDay(this long? dayTime)
        {
            if (!dayTime.HasValue)
            {
                return null;
            }

            return FromEpochTimeOfDay(dayTime.Value);
        }


        /// <summary>
        /// Converts Epoch (Unix) time representation to Time of day into <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="dayTime">Input string with time of day data</param>
        /// <returns>Epoch time representation as long type</returns>
        public static TimeSpan FromEpochTimeOfDay(this long dayTime)
        {
            return new TimeSpan((dayTime * 10000) % new TimeSpan(1, 0, 0, 0).Ticks);
        }

        /// <summary>
        /// Converts Epoch (Unix) time representation to Time of day into <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="dayTime">Input string with time of day data</param>
        /// <returns>Epoch time representation as long type</returns>
        public static TimeSpan FromEpochTimeDuration(this long dayTime)
        {
            return new TimeSpan(dayTime * 10000);
        }

        /// <summary>
        /// Converts Time of day in <see cref="TimeSpan"/> format into miliseconds time representation
        /// </summary>
        /// <returns>Milliseconds time representation as long type</returns>
        public static long ToMillisecondsTime(this TimeSpan timeSpan)
        {
            return timeSpan.Ticks / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Converts date to <see cref="DateTimeOffset"/> and assumes the date
        /// is already in UTC no matter what <see cref="DateTime.Kind"/> says.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTimeOffset ToUtcDateWithoutConversion(this DateTime date)
        {
            // Note we don't look at the Kind property because DateTime coming from
            // db or via some date <-> unix timestamp conversion probably has wrong Kind
            // or "Unspecified". We assume the date is in UTC and use the date as is
            // with correct utc offset.
            return new DateTimeOffset(date, TimeSpan.Zero);
        }

        public static long? ToEpochTime(this DateTimeOffset? dateTime)
        {
            if (dateTime == null) return null;
            return ToEpochTime(dateTime.Value.DateTime);
        }
    }
}
