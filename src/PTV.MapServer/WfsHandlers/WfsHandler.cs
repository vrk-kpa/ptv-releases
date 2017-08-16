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
using System.Xml.Linq;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.MapServer.Common;
using PTV.MapServer.Interfaces;
using PTV.MapServer.Models;

namespace PTV.MapServer.WfsHandlers
{
    [RegisterService(typeof(WfsHandler), RegisterType.Transient)]
    public class WfsHandler : BaseServiceHandler
    {

        public WfsHandler(IResolveManager resolveManager) : base(resolveManager)
        {}

        public override XDocument Handle(HttpGetRequestQuery requestParameters)
        {
            var request = ParseRequest<WfsSupportedOperationEnum>(requestParameters.Request);
            switch (request)
            {
                case WfsSupportedOperationEnum.GetCapabilities:
//                    return resolveManager.Resolve<WfsGetCapabilitiesHandler>().Handle(requestParameters);
                    var parameters = new WfsGetCapabilitiesParameters();
                    return resolveManager.Resolve<WfsGetCapabilitiesHandler>().Handle(parameters);

                case WfsSupportedOperationEnum.DescribeFeatureType:
                    return resolveManager.Resolve<WfsDescribeFeatureType>().Handle(requestParameters);

                case WfsSupportedOperationEnum.GetFeature:
                    return resolveManager.Resolve<WfsGetFeatureHandler>().Handle(requestParameters);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override XDocument Handle(XElement requestParameters)
        {
            var request = ParseRequest<WfsSupportedOperationEnum>(requestParameters);
            return new XDocument("pica pice pijci klice ... ");
        }


        private WfsGetCapabilitiesParameters Parse(XElement element)
        {
            return null;
        }

        //private T Parse<T>(XElement )
    }
}
