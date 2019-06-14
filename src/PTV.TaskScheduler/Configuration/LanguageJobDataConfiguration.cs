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

using System.Collections.Generic;
using System.Linq;
using PTV.Framework;
using PTV.TaskScheduler.Interfaces;

namespace PTV.TaskScheduler.Configuration
{
    /// <summary>
    /// PostalCode job data configuration
    /// </summary>
    public class LanguageJobDataConfiguration : UrlJobDataConfiguration, IJobDataConfiguration<LanguageJobDataConfiguration>
    {
        public List<string> AllowedForData { get; set; }
        public List<string> AllowedForTranslation { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// IEquatable implemantation
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(LanguageJobDataConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(AllowedForData, other.AllowedForData) && 
                   string.Equals(AllowedForTranslation, other.AllowedForTranslation);
        }

        public override bool Validate()
        {
            
            return base.Validate() && AllowedForData.Any() && AllowedForTranslation.Any();
        }
    }


    public class EmailNotifyJobDataConfiguration : UrlBaseConfiguration, IJobDataConfiguration<EmailNotifyJobDataConfiguration>
    {
        public override string ConfigurationName => "EmailNotifyConfiguration";
        
        public bool Equals(EmailNotifyJobDataConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return true;
        }
        
        public string AuthenticationUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Validate()
        {
            return !string.IsNullOrEmpty(AuthenticationUrl) && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password)&& !string.IsNullOrEmpty(UrlBase);
        }
    }
}
