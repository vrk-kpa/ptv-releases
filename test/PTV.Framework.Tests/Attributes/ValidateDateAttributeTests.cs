using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;
using PTV.Framework.Attributes;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Primitives;
using System.Net;

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
