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
using System.Text;
using NLog;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;
using System.Linq;

namespace PTV.Framework.Logging
{
    [LayoutRenderer("custom-aspnet-request-ip")]
    public class LayoutRendererCustomRequestIp : AspNetLayoutRendererBase
    {
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var allIps = $"{GetHeaderValueAs<string>("X-Forwarded-For").SplitCsv().FirstOrDefault() ?? string.Empty};{(HttpContextAccessor.HttpContext?.Connection?.RemoteIpAddress != null ? HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString() : string.Empty)};{GetHeaderValueAs<string>("REMOTE_ADDR") ?? string.Empty}";
            builder.Append(allIps);
        }

        private T GetHeaderValueAs<T>(string headerName)
        {
            if (HttpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue(headerName, out var values) ?? false)
            {
                string rawValues = values.ToString();

                if (!rawValues.IsNullOrEmpty())
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }
    }
}
