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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using PTV.Framework.Interfaces;
using PTV.MapServer.ExceptionHandler;
using PTV.MapServer.Interfaces;
using PTV.MapServer.Models;
using PTV.MapServer.WfsHandlers;
using PTV.MapServer.WmsHandlers;

namespace PTV.MapServer.Common
{
    public abstract class BaseServiceHandler : IServiceHandler
    {

        protected readonly IResolveManager resolveManager;

        protected BaseServiceHandler(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }

        public abstract XDocument Handle(HttpGetRequestQuery requestParameters);

        public abstract XDocument Handle(XElement requestParameters);

        protected static TEnum ParseRequest<TEnum>(string request) where TEnum : struct, IConvertible
        {
            var type = typeof (TEnum);
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsEnum)
            {
                throw new Exception($"Type '{type.FullName}' is not a Enum type.");
            }

            var possibleEnums = new[] { typeof(WfsSupportedOperationEnum), typeof(WmsSupportedOperationEnum) };
            if (!possibleEnums.Contains(type))
            {
                throw new Exception($"Enum '{type.FullName}' is not supported request enum.");
            }

            TEnum result;
            if (!Enum.TryParse(request, true, out result))
            {
                throw new OwsException(OwsExceptionCodeEnum.OperationNotSupported, "request",
                    new []
                    {
                        $"Request '{request}' is not supported.",
                        $"Supported requests: '{string.Join(", ", Enum.GetNames(type))}'"
                    });
            }


            return result;
        }

        protected static TEnum ParseRequest<TEnum>(XElement request) where TEnum : struct, IConvertible
        {

            return default(TEnum);
        }
    }
}
