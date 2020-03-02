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
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PTV.Framework.Interfaces;

namespace PTV.Framework.ServiceManager
{
    public class PtvAppException : Exception
    {
        public string CodeIdentifier { get; protected set; }

        public IEnumerable<string> AdditionalParams { get; protected set; }

        public IVmBase AdditionalData { get; protected set; }

        public PtvAppException()
        {
        }

        public PtvAppException(IVmBase additionalData, string message, string codeIdentifier = null, IEnumerable<string> additionalParams = null) : this(message, null, codeIdentifier, additionalParams, additionalData)
        { }

        public PtvAppException(string message, string codeIdentifier = null, IEnumerable<string> additionalParams = null) : this(message, null, codeIdentifier, additionalParams)
        { }

        public PtvAppException(string message, Exception innerException, string codeIdentifier = null, IEnumerable<string> additionalParams = null, IVmBase additionalData = null) : base(message, innerException)
        {
            CodeIdentifier = codeIdentifier;
            AdditionalParams = additionalParams;
            AdditionalData = additionalData;
        }
    }

    public class PtvDbTooManyConnectionsException : PtvAppException
    {
    }

    public class PtvActionCancelledException : PtvAppException
    {
    }

    public class PtvArgumentException : PtvAppException
    {
        public string ParamName { get; set; }
        public PtvArgumentException(string message, string parameterName = "") : base(message)
        {
            ParamName = parameterName;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ErrorException : Exception, IError
    {
        public ErrorException()
        {
        }

        public ErrorException(string message) : base(message)
        {
        }

        public ErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ErrorException(string code, string subCode = null, IEnumerable<string> additionalParams = null)
        {
            SubCode = subCode;
            Code = code;
            Params = additionalParams.InclusiveToList();
        }

        public ErrorException(string message, string code, string subCode = null, IEnumerable<string> additionalParams = null) : base(message)
        {
            SubCode = subCode;
            Code = code;
            Params = additionalParams.InclusiveToList();
        }

        [JsonProperty]
        public string Code { get; set; }
        [JsonProperty]
        public string SubCode { get; set; }
        [JsonProperty]
        public IReadOnlyList<string> Params { get; set; }
    }
}
