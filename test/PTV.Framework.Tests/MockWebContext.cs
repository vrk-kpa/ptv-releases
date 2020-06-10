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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using Microsoft.Extensions.Primitives;

namespace PTV.Framework.Tests
{
    public class MockWebContext
    {
        public MockWebContext() : this(new Dictionary<string, StringValues>(), new Dictionary<string, object>()) { }

        public MockWebContext(Dictionary<string, StringValues> querystring) : this(querystring, new Dictionary<string, object>()) { }

        public MockWebContext(Dictionary<string, object> actionArguments) : this(new Dictionary<string, StringValues>(), actionArguments) { }

        public MockWebContext(Dictionary<string, StringValues> querystring, Dictionary<string, object> actionArguments)
        {
            Query = querystring ?? new Dictionary<string, StringValues>();
            ActionArguments = actionArguments ?? new Dictionary<string, object>();

            HttpContext = new Mock<HttpContext>();
            HttpRequest = new Mock<HttpRequest>();

            // setup property tracking for response statuscode
            HttpResponse = new Mock<HttpResponse>();
            HttpResponse.SetupProperty(x => x.StatusCode);

            Controller = new Mock<Controller>();
            ActionDescriptor = new Mock<ActionDescriptor>();
            ModelState = new ModelStateDictionary();
            RouteData = new Mock<RouteData>();
        }

        public Mock<HttpContext> HttpContext { get; private set; }

        public Mock<HttpRequest> HttpRequest { get; private set; }

        public Mock<HttpResponse> HttpResponse { get; private set; }

        public Mock<RouteData> RouteData { get; private set; }

        public Mock<Controller> Controller { get; private set; }

        public Mock<ActionDescriptor> ActionDescriptor { get; private set; }

        public ModelStateDictionary ModelState { get; private set; }

        public Dictionary<string, StringValues> Query { get; private set; }

        public Dictionary<string, object> ActionArguments { get; private set; }

        /// <summary>
        /// Creates ActionExecutingContext based on the instance mocks and values.
        /// </summary>
        /// <returns>ActionExecutingContext</returns>
        public ActionExecutingContext CreateActionExecutingContext()
        {
            // create the request query collection based on the dictionary
            HttpContext.SetupGet(x => x.Request.Query).Returns(new QueryCollection(Query));

            // use the mock for the response
            HttpContext.SetupGet(x => x.Response).Returns(HttpResponse.Object);

            // create the action context
            var actionContext = new ActionContext(HttpContext.Object, RouteData.Object, ActionDescriptor.Object, ModelState);

            // create and return the ActionExecutingContext
            return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), ActionArguments, Controller);
        }
    }
}
