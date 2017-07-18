using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.Primitives;
using PTV.Framework.Attributes;
using System.Net;
using PTV.Framework.ServiceManager;

namespace PTV.Framework.Tests.Attributes
{
    public class ValidateIdAttributeTests
    {
        [Theory]
        [InlineData(null)] // Will be Guid.Empty
        [InlineData("")] // Will be Guid.Empty
        [InlineData(" ")] // Will be Guid.Empty
        [InlineData("  ")] // Will be Guid.Empty
        [InlineData("ca761232ed4211cebacd00aa0057b222")]
        [InlineData("CA761232-ED42-11CE-BACD-00AA0057B223")]
        [InlineData("{CA761232-ED42-11CE-BACD-00AA0057B223}")]
        [InlineData("(CA761232-ED42-11CE-BACD-00AA0057B223)")]
        [InlineData("{0xCA761232, 0xED42, 0x11CE, {0xBA, 0xCD, 0x00, 0xAA, 0x00, 0x57, 0xB2, 0x23}}")]
        public void ValidateValidGuids(string guid)
        {
            MockWebContext ctx = new MockWebContext();
            ctx.ActionArguments.Add("id", guid);

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidateIdAttribute attr = new ValidateIdAttribute("id");
            attr.OnActionExecuting(actExeContext);

            // if everything is ok the status is not changed so default 0 value is the expected value and not http 200
            actExeContext.HttpContext.Response.StatusCode.Should().Be(0);
        }

        [Theory]
        [InlineData("5-5-5-5")]
        [InlineData("vrk")]
        [InlineData("ca761232ed4211cebacd00aa0057b22")] // missing one character
        [InlineData("CA761232-ED4211CE-BACD-00AA0057B223")] // missing one '-' character
        [InlineData("{CA761232-ED42-11CE-BACD-00AA0057B223")] // missing ending '}' character
        [InlineData("(CA761232-ED42-11CE-BACD-00AA0057B223")] // missing ending ')' character
        [InlineData("{0xCA761232, 0xED42, 0x11CE, {0xBA, 0xCD, 0x00, 0xAA, 0x00, 0x57, 0xB2, 0x23}")] // missing ending '}' character
        public void ValidateInvalidGuids(string guid)
        {
            MockWebContext ctx = new MockWebContext();
            ctx.ActionArguments.Add("id", guid);

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidateIdAttribute attr = new ValidateIdAttribute("id");
            attr.OnActionExecuting(actExeContext);
            
            actExeContext.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public void ShouldHandleKeyNameNotFound()
        {
            // the attribute should handle cases that the key is not found instead of code causing KeyNotFoundException
            // caller has no idea what key is missing
            // attribute implementation skips null values so it should also skip validation if the key is not found?

            MockWebContext ctx = new MockWebContext();
            ctx.ActionArguments.Add("id", "{CA761232-ED42-11CE-BACD-00AA0057B223}");

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidateIdAttribute attr = new ValidateIdAttribute("notfoundkeyname");
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
            Action act = () => new ValidateIdAttribute(badKeyName);

            act.ShouldThrow<PtvArgumentException>().WithMessage("keyName cannot be null, empty string or whitespaces.", "Because keyname cannot be null, empty string or whitespaces.");
        }
    }
}
