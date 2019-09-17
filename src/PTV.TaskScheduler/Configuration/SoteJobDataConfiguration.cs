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

using PTV.TaskScheduler.Interfaces;
using PTV.Framework;

namespace PTV.TaskScheduler.Configuration
{
    public class SoteJobDataConfiguration : UrlBaseConfiguration, ICertificateConfiguration, IJobDataConfiguration<SoteJobDataConfiguration>
    {
        public override string ConfigurationName => "SoteConfiguration";
        
        /// <summary>
        /// ServiceProvider Url
        /// </summary>
        public string ServiceProviderUrl { get; set; }
        
        /// <summary>
        /// ServiceOrganizer Url
        /// </summary>
        public string ServiceOrganizerUrl { get; set; }
        
        /// <summary>
        /// Certificate file name
        /// </summary>
        public string CertificateFileName { get; set; }
        
        /// <summary>
        /// Authorization header value
        /// </summary>
        public string AuthorizationHeaderValue { get; set; } 
        
        /// <summary>
        /// Ignore server certificate
        /// </summary>
        public bool IgnoreServerCertificate { get; set; }
        
        public bool Equals(SoteJobDataConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(ServiceProviderUrl, other.ServiceProviderUrl)
                   && string.Equals(ServiceOrganizerUrl, other.ServiceOrganizerUrl);
        }

        public bool Validate()
        {
            return !ServiceProviderUrl.IsNullOrEmpty();
        }

        
    }
}