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
using System.Text.RegularExpressions;
using System.Xml.Linq;
using PTV.Framework.Interfaces;
using PTV.MapServer.ExceptionHandler;
using PTV.MapServer.Interfaces;
using PTV.MapServer.Models;

namespace PTV.MapServer.WfsHandlers
{
    internal abstract class WfsOperationBase : IWfsOperationHandler
    {
        protected readonly MapServerConfiguration configuration;
        protected readonly IResolveManager resolveManager;

        

        protected WfsOperationBase(MapServerConfiguration configuration, IResolveManager resolveManager)
        {
            this.configuration = configuration;
            this.resolveManager = resolveManager;
        }

        public abstract XDocument Handle(HttpGetRequestQuery requestParameters);
//        public abstract XDocument Handle(IWfsRequestParameters requestParameters);

        protected WfsSupportedVersionEnum ParseVersion(string version)
        {
            var versionEnum = ParseStringVersionToEnumFormat(version);
            WfsSupportedVersionEnum result;
            if (!Enum.TryParse(versionEnum, true, out result))
            {
                throw new OwsException(
                OwsExceptionCodeEnum.InvalidParameterValue,
                "version", new[] {
                $"Version couldnt be parsed: '{version}'.", "Use: 'X.Y.Z' format."});
            }

            return result;
        }

        protected Version ParseVersion(WfsSupportedVersionEnum version)
        {
            var versionStr = ParseEnumVersionToVersionFormat(version);
            return new Version(versionStr);
        }

        protected string ParseStringVersionToEnumFormat(string version)
        {
            return "_" + version.Replace(".", "_");
        }

        protected string ParseEnumVersionToVersionFormat(WfsSupportedVersionEnum version)
        {
            var versionStr = version.ToString();
            versionStr = versionStr.Substring(1);
            return versionStr.Replace("_", ".");
        }

        

    }
}

