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


using PTV.TaskScheduler.Interfaces;
using PTV.Framework;

namespace PTV.TaskScheduler.Configuration
{
    public class AccessibilityRegisterConfiguration : UrlBaseConfiguration, IJobDataConfiguration<AccessibilityRegisterConfiguration>
    {

        public override string ConfigurationName => "AccessibilityRegisterConfiguration";

        /// <summary>
        /// Service points url
        /// </summary>
        public string UrlServicePoints { get; set; }

        /// <summary>
        /// Sentences url
        /// </summary>
        public string UrlSentences { get; set; }

        /// <summary>
        /// Entrances url
        /// </summary>
        public string UrlEntrances { get; set; }

        /// <summary>
        /// SystemId
        /// </summary>
        public string SystemId { get; set; }


        public bool Equals(AccessibilityRegisterConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(UrlServicePoints, other.UrlServicePoints)
                   && Equals(UrlSentences, other.UrlSentences)
                   && Equals(UrlBase, other.UrlBase)
                   && Equals(SystemId, other.SystemId)
                   && Equals(UrlEntrances, other.UrlEntrances);
        }

        public bool Validate()
        {
            return !UrlServicePoints.IsNullOrEmpty()
                   && !UrlSentences.IsNullOrEmpty()
                   && !UrlBase.IsNullOrEmpty()
                   && !UrlEntrances.IsNullOrEmpty()
                   && !SystemId.IsNullOrEmpty();
        }
    }
}
