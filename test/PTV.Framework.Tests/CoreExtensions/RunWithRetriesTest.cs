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
using FluentAssertions;
using Xunit;

namespace PTV.Framework.Tests.CoreExtensions
{
    public class RunWithRetriesTest
    {
        [Fact]
        public void RunWithRetriesShouldHandleNullAction()
        {
            Func<bool> actIn = null;

            Action act = () => Framework.CoreExtensions.RunWithRetries(5, actIn);

            act.Should().NotThrow("Because there is currently exception block that hides all exceptions.");
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

            Action act = () => Framework.CoreExtensions.RunWithRetries(retries, jt.IncreaseCount);
            act.Should().ThrowExactly<ArgumentException>("Invalid value given for retries count.");
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

            Framework.CoreExtensions.RunWithRetries(retries, jt.IncreaseCount);

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

            Framework.CoreExtensions.RunWithRetries(5, jt.IncreaseCount);

            jt.Count.Should().Be(treshold);
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
    }
}
