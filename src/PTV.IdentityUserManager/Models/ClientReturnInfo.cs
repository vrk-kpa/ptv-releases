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
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace PTV.IdentityUserManager.Models
{
    /// <summary>
    /// Models the information and mechanisms for allowing a user to return to a client application.
    /// </summary>
    public class ClientReturnInfo
    {
        /// <summary>
        /// The identifier for the client application the user will be sent to.
        /// </summary>
        /// <value>
        /// The Client Id.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// The Uri of the client where the user can be returned.
        /// </summary>
        /// <value>
        /// The return Uri.
        /// </value>
        public string Uri { get; set; }

        /// <summary>
        /// The HTML-encoded values for the POST body to be used if IsPost is true.
        /// </summary>
        /// <value>
        /// The POST body.
        /// </value>
        public string PostBody { get; set; }

        /// <summary>
        /// Value that indicates if the return must be performed via a POST, rather than a redirect with GET.
        /// </summary>
        /// <value>
        /// The IsPost flag.
        /// </value>
        public bool IsPost
        {
            get
            {
                return PostBody != null;
            }
        }
    }
}
