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

using Xunit;
using PTV.Framework.Attributes;
using Microsoft.Extensions.Primitives;
using System.Net;
using FluentAssertions;

namespace PTV.Framework.Tests.Attributes
{
    public class ValidateDateAttributeTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("20170519")]
        [InlineData("2017-05-19")]
        [InlineData("2017-05-19 11:10:02")]
        [InlineData("2017-05-19T11:10")]
        [InlineData("2037-13-01T11:10:02")]
        public void TestInvalidDateStringFormats(string dateString)
        {
            MockWebContext ctx = new MockWebContext();
            ctx.Query.Add("date", new StringValues(dateString));

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidateDateAttribute attr = new ValidateDateAttribute();
            attr.OnActionExecuting(actExeContext);

            actExeContext.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(null)] // no date in querystring
        [InlineData("2017-05-19T11:10:02")]
        [InlineData("2017-02-19T23:59:59")]
        [InlineData("2037-01-01T11:10:02")]
        public void TestValidDateStringFormats(string dateString)
        {
            MockWebContext ctx = new MockWebContext();

            // null value would cause sequence contains no elements exception
            // so we use null here to indicate not to include the date parameter
            if (dateString != null)
            {
                ctx.Query.Add("date", new StringValues(dateString));
            }

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidateDateAttribute attr = new ValidateDateAttribute();
            attr.OnActionExecuting(actExeContext);

            // if everything is ok the status is not changed so default 0 value is the expected value and not http 200
            actExeContext.HttpContext.Response.StatusCode.Should().Be(0);
        }
    }
}
