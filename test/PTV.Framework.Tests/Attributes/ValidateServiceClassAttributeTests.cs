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
using Xunit;
using PTV.Framework.Attributes;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace PTV.Framework.Tests.Attributes
{
    public class ValidateServiceClassAttributeTests
    {
        [Theory]
        [InlineData("1")]
        [InlineData("http://test.fi/")]
        public void InvalidServiceClass(string serviceClassString)
        {
            MockWebContext ctx = new MockWebContext();
            ctx.Query.Add("uri", new StringValues(WebUtility.UrlEncode(serviceClassString)));

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidateServiceClassAttribute attr = new ValidateServiceClassAttribute();
            attr.OnActionExecuting(actExeContext);

            actExeContext.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("http://urn.fi/URN:NBN:fi:au:ptvl:v1065")]
        public void ValidServiceClass(string serviceClassString)
        {
            MockWebContext ctx = new MockWebContext();
            ctx.Query.Add("uri", new StringValues(WebUtility.UrlEncode(serviceClassString)));

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidateServiceClassAttribute attr = new ValidateServiceClassAttribute();
            attr.OnActionExecuting(actExeContext);

            actExeContext.HttpContext.Response.StatusCode.Should().Be(0);
        }
    }
}
