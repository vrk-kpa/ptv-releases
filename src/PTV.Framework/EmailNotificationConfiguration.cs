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

using System.Collections.Generic;
using System.IO;

namespace PTV.Framework
{
    /// <summary>
    /// Configuration for sending emails via the PaHa notification gateway.
    /// </summary>
    public class EmailNotificationConfiguration
    {
        /// <summary>
        /// PaHa URL for sending emails.
        /// </summary>
        public string UrlBase { get; set; }
        
        /// <summary>
        /// URL for PaHa authentication.
        /// </summary>
        public string AuthenticationUrl { get; set; }
        
        /// <summary>
        /// Username for PaHa authentication.
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Password for PaHa authentication.
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// Email subject.
        /// </summary>
        public string Subject { get; set; }
        
        /// <summary>
        /// Filepath to email template.
        /// </summary>
        public string TemplatePath { get; set; }

        /// <summary>
        /// Content of the file in TemplatePath.
        /// </summary>
        public virtual string TemplateText => File.ReadAllText(TemplatePath);
        
        public EmailTestConfiguration TestConfiguration { get; set; }
    }

    public class EmailTestConfiguration
    {
        public string DefaultSahaId { get; set; }
        
        public List<string> AllowedSahaIds { get; set; }
    }
}
