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
using System.Text;
using FluentAssertions;
using Xunit;
using PTV.Framework.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Net;
using PTV.Framework.ServiceManager;

namespace PTV.Framework.Tests.Attributes
{
    public class ValidationBaseAttributeTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CtorShouldThrowOnBadKeyName(string keyName)
        {
            Action act = () => new ValidationBaseAttribute(keyName, new ValidGuidAttribute());

            act.Should().Throw<PtvArgumentException>().WithMessage("keyName cannot be null, empty string or whitespaces.", "Because keyname cannot be null, empty string or whitespaces.");
        }

        [Fact]
        public void CtorShouldThrowWhenValidatorIsNull()
        {
            Action act = () => new ValidationBaseAttribute("Id", null);

            act.Should().Throw<PtvArgumentException>().WithMessage("validator cannot be null.", "Because validator cannot be null.");
        }

        [Fact]
        public void ValidatorShouldntThrowWhenKeyNameIsMissingFromRequest()
        {
            // it shouldn't throw if it is ok that the value is missing from request
            // for example with guid attribute null and empty values are ok by the validator
            // or the implementation of the validator should be changed so that it doesn't
            // crash with KeyNotFoundException, the code should check if the required key exists and if not return
            // the similiar response (json) as it currently returns if the supplied value is not valid

            MockWebContext ctx = new MockWebContext();
            ctx.ActionArguments.Add("id", "{CA761232-ED42-11CE-BACD-00AA0057B223}");

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidationBaseAttribute attr = new ValidationBaseAttribute("notfoundkeyname", new ValidGuidAttribute());
            attr.OnActionExecuting(actExeContext);

            // if the argument needs to be present when using this validator then we should expect here validation error with http status code 400 or 422
            actExeContext.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public void ValidateValidValue()
        {
            MockWebContext ctx = new MockWebContext();
            ctx.ActionArguments.Add("id", "{CA761232-ED42-11CE-BACD-00AA0057B223}");

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidationBaseAttribute attr = new ValidationBaseAttribute("id", new ValidGuidAttribute());
            attr.OnActionExecuting(actExeContext);

            // if the argument needs to be present when using this validator then we should expect here validation error with http status code 400 or 422
            // if everything is ok the status is not changed so default 0 value is the expected value and not http 200
            actExeContext.HttpContext.Response.StatusCode.Should().Be(0);
        }

        [Fact]
        public void ValidateInvalidValue()
        {
            MockWebContext ctx = new MockWebContext();
            ctx.ActionArguments.Add("id", "{CA761232-ED42-11CE-BACD-00AA0057B223"); // missing ending curly brace

            var actExeContext = ctx.CreateActionExecutingContext();

            ValidationBaseAttribute attr = new ValidationBaseAttribute("id", new ValidGuidAttribute());
            attr.OnActionExecuting(actExeContext);

            // bad request response, http status code 400
            actExeContext.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}
