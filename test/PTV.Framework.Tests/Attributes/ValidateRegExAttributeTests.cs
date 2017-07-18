using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;
using PTV.Framework.Attributes;
using System.Net;
using System.Text.RegularExpressions;
using PTV.Framework.ServiceManager;

namespace PTV.Framework.Tests.Attributes
{
    public class ValidateRegExAttributeTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("1234567-8")]
        [InlineData("0000000-0")]
        [InlineData("0101010-1")]
        public void ValidateValidBusinessIds(string businessId)
        {
            MockWebContext ctx = new MockWebContext();
            ctx.ActionArguments.Add("code", businessId);

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidateRegExAttribute attr = new ValidateRegExAttribute("code", @"^[0-9]{7}-[0-9]{1}$");
            attr.OnActionExecuting(actExeContext);

            // if everything is ok the status is not changed so default 0 value is the expected value and not http 200
            actExeContext.HttpContext.Response.StatusCode.Should().Be(0);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("1234567")]
        [InlineData("12345678")]
        [InlineData("00000028-0")]
        [InlineData("01-2")]
        public void ValidateInvalidBusinessIds(string businessId)
        {
            MockWebContext ctx = new MockWebContext();
            ctx.ActionArguments.Add("code", businessId);

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidateRegExAttribute attr = new ValidateRegExAttribute("code", @"^[0-9]{7}-[0-9]{1}$");
            attr.OnActionExecuting(actExeContext);

            actExeContext.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public void ShouldHandleKeyNameNotFound()
        {
            // the attribute should handle cases that the key is not found instead of code causing KeyNotFoundException
            // caller has no idea what key is missing
            // attribute implementation skips null values so it should also skip validation if the key is not found?
            // either way the Exception should be handled and return correct message and status code

            MockWebContext ctx = new MockWebContext();
            ctx.ActionArguments.Add("code", "1234567-8");

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidateRegExAttribute attr = new ValidateRegExAttribute("notfoundkeyname", @"^[0-9]{7}-[0-9]{1}$");
            attr.OnActionExecuting(actExeContext);

            // if everything is ok the status is not changed so default 0 value is the expected value and not http 200
            actExeContext.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CtorShouldThrowOnBadKeyNames(string badKeyName)
        {
            Action act = () => new ValidateRegExAttribute(badKeyName, @"^[0-9]{7}-[0-9]{1}$");
            // either way the Exception should be handled and return correct message and status code
            act.ShouldThrow<PtvArgumentException>().WithMessage("keyName cannot be null, empty string or whitespaces.", "Because keyname cannot be null, empty string or whitespaces.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("+?'-)&")]
        public void InvalidPatternThrows(string pattern)
        {
            Action act = () => new ValidateRegExAttribute("code", pattern);

            act.ShouldThrow<PtvArgumentException>().WithMessage("pattern cannot be null, empty string or not valid.", "Because pattern cannot be null, empty string or not valid regular expression.");
        }
    }
}
