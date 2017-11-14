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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PTV.MapServer.Common;
using PTV.MapServer.ExceptionHandler;
using PTV.MapServer.Interfaces;

namespace PTV.MapServer.Models
{
    public class HttpGetRequestQuery : IValidatableObject, IRequestParameters
    {
        public string Service { get; set; }

        public string Request { get; set; }

        public string Version { get; set; }

        public string AcceptVersions { get; set; }

        public string Sections { get; set; }

        public string SrsName { get; set; }

        public string TypeName { get; set; }

        public SupportedOcgServiceEnum SupportedService { get; private set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
//            if (Service == null)
//            {
//                throw new OwsException(
//                    OwsExceptionCodeEnum.MissingParameterValue,
//                    "service",
//                    CoreMessages.NoServiceWasSpecified);
//            }

            RequestValidation.ValidateKnownService(Service);

            if (Request == null)
            {
                throw new OwsException(
                    OwsExceptionCodeEnum.MissingParameterValue,
                    "request",
                    CoreMessages.RequestParameterMustBeSet);
            }

//            KnownOcgServiceEnum knownOcgService;
//            if (!Enum.TryParse(Service, true, out knownOcgService))
//            {
//                throw new OwsException(
//                    OwsExceptionCodeEnum.InvalidParameterValue,
//                    "service",
//                    string.Format(CoreMessages.SpecifiedServiceIsNotKnownOcgService, Service));
//            }

//            SupportedOcgServiceEnum supportedService;
//            if (!Enum.TryParse(Service, true, out supportedService))
//            {
//                throw new OwsException(
//                    OwsExceptionCodeEnum.NoApplicableCode,
//                    "service",
//                    new[]
//                    {
//                        $"Specified service '{Service}' is not a supported.",
//                        $"Supported services: '{string.Join(", ", Enum.GetNames(typeof (SupportedOcgServiceEnum))).ToUpper()}'"
//                    });
//            }

            SupportedService = RequestValidation.ValidateSupportedService(Service);
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
