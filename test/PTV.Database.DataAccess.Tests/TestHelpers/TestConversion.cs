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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.TestHelpers
{
    public class TestConversion
    {
        private readonly DateTimeConverter dateTimeConverter = new DateTimeConverter();
        
        public long GetTime(TimeSpan time)
        {
            return (time.Ticks % new TimeSpan(1, 0, 0, 0).Ticks + new TimeSpan(1, 0, 0, 0).Ticks) / 10000;
        }

        public long GetUnixTime(int minutes)
        {
            return minutes * 60 * 1000;
        }

        public long GetTicks(long unixTime)
        {
            return unixTime * 10000;
        }

        public long GetTicksFromMinutes(int minutes)
        {
            return GetTicks(GetUnixTime(minutes));
        }
        
        public long? GetDate(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                var dateTime = DateTime.Parse(date, CultureInfo.InvariantCulture);
                return dateTime.ToEpochTime();    
            }

            return null;
        }        
    }
}
