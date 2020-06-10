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
using PTV.Framework;
using PTV.TaskScheduler.Interfaces;

namespace PTV.TaskScheduler.Configuration
{
    public class YPlatformConfiguration : UrlBaseConfiguration, IJobDataConfiguration<YPlatformConfiguration>
    {
        public override string ConfigurationName => nameof(YPlatformConfiguration);
        
        public string MunicipalityVersion { get; set; }
        public string ProvinceVersion { get; set; }
        public string ProvinceRelationshipVersion { get; set; }
        public string HospitalRegionVersion { get; set; }
        public string HospitalRegionRelationshipVersion { get; set; }
        public string BusinessRegionVersion { get; set; }
        public string BusinessRegionRelationshipVersion { get; set; }
        public string ApiVersion { get; set; }
        public string Token { get; set; }
        
        public bool Equals(YPlatformConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) 
                   || string.Equals(UrlBase, other.UrlBase)
                   || string.Equals(MunicipalityVersion, other.MunicipalityVersion)
                   || string.Equals(ProvinceVersion, other.ProvinceVersion)
                   || string.Equals(ProvinceRelationshipVersion, other.ProvinceRelationshipVersion)
                   || string.Equals(HospitalRegionVersion, other.HospitalRegionVersion)
                   || string.Equals(HospitalRegionRelationshipVersion, other.HospitalRegionRelationshipVersion)
                   || string.Equals(BusinessRegionVersion, other.BusinessRegionVersion)
                   || string.Equals(BusinessRegionRelationshipVersion, other.BusinessRegionRelationshipVersion)
                   || string.Equals(ApiVersion, other.ApiVersion);
        }

        public bool Validate()
        {
            return !UrlBase.IsNullOrWhitespace() 
                   && !MunicipalityVersion.IsNullOrWhitespace() 
                   && !ProvinceVersion.IsNullOrWhitespace() 
                   && !ProvinceRelationshipVersion.IsNullOrWhitespace() 
                   && !HospitalRegionVersion.IsNullOrWhitespace() 
                   && !HospitalRegionRelationshipVersion.IsNullOrWhitespace() 
                   // TODO: Not yet implemented
                   // && !BusinessRegionVersion.IsNullOrWhitespace() 
                   // && !BusinessRegionRelationshipVersion.IsNullOrWhitespace() 
                   && !ApiVersion.IsNullOrWhitespace();
        }
    }
}
