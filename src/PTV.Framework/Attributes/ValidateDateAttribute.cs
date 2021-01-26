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
using System.Globalization;
using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// Checks the date query string parameter format.
    /// </summary>
    public class ValidateDateAttribute : ActionFilterAttribute
    {
        private string propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateDateAttribute"/> class.
        /// </summary>
        public ValidateDateAttribute(string propertyName = null)
        {
            this.propertyName = propertyName ?? "date";
        }
        /// <summary>
        /// Validation of request when action is executing
        /// </summary>
        /// <param name="context"></param>
        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var queryString = context.HttpContext.Request.Query;
            var datePair = queryString.FirstOrDefault(q => q.Key == propertyName);
            if (datePair.Key != null)
            {
                var s = datePair.Value.First();
                DateTime date;
                if (!DateTime.TryParseExact(s, "yyyy-MM-ddTHH:mm:ss",
                                      CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, out date))
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.ModelState.AddModelError(propertyName, CoreMessages.OpenApi.DateMalFormatted);
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
