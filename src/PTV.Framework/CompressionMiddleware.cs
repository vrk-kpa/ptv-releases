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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace PTV.Framework
{
    public static class CompressionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCompression(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CompressionMiddleware>();
        }
    }

    public class CompressionMiddleware
    {
        private readonly RequestDelegate nextDelegate;
        private readonly ILogger<CompressionMiddleware> logger;

        public CompressionMiddleware(RequestDelegate next, ILogger<CompressionMiddleware> logger)
        {
            nextDelegate = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var acceptEncoding = httpContext.Request.Headers["Accept-Encoding"];
            //Checking that we have been given the correct header before moving forward
            if ((!string.IsNullOrEmpty(acceptEncoding)) && (acceptEncoding.Any(i => i.ToLower().Contains("gzip"))))
            {
                using (var memoryStream = new MemoryStream())
                {
                    var stream = httpContext.Response.Body;
                    httpContext.Response.Body = memoryStream;

                    await nextDelegate(httpContext);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    long uncompressedSize = memoryStream.Length > 0 ? memoryStream.Length : 1;
                    long compressedSize = 0;
                    using (var compressedStream = memoryStream.Compress(CompressionLevel.Fastest))
                    {
                        httpContext.Response.Headers.Add("Content-Encoding", new[] {"gzip"});
                        compressedStream.Seek(0, SeekOrigin.Begin);
                        if (httpContext.Response.ContentLength != null)
                        {
                            httpContext.Response.ContentLength = compressedStream.Length;
                        }

                        compressedSize = compressedStream.Length;
                        await compressedStream.CopyToAsync(stream);

                    }

                    var logMessage = $"Request {httpContext.Request?.Path.Value ?? string.Empty}, Size Original {uncompressedSize / 1024} KB, Compressed {compressedSize / 1024} KB, Ratio {compressedSize * 100 / uncompressedSize}%";
                    logger.LogInformation(logMessage);
                    httpContext.Response.Body = stream;
                }
            }
            else
            {
                await nextDelegate(httpContext);
            }
        }
    }
}
