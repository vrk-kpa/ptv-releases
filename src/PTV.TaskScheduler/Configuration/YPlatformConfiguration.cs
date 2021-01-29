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
        
        public string Municipality { get; set; }
        public string Province { get; set; }
        public string ProvinceRelationship { get; set; }
        public string HospitalRegion { get; set; }
        public string HospitalRegionRelationship { get; set; }
        public string BusinessRegion { get; set; }
        public string BusinessRegionRelationship { get; set; }
        
        public string ServiceClass { get; set; }
        public string TargetGroup { get; set; }
        public string LifeEvent { get; set; }
        public string IndustrialClass { get; set; }
        public string OrganizationType { get; set; }
        public string ProvisionType { get; set; }
        public string ApiVersion { get; set; }
        public string Token { get; set; }
        
        public bool Equals(YPlatformConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) 
                   || string.Equals(UrlBase, other.UrlBase)
                   || string.Equals(Municipality, other.Municipality)
                   || string.Equals(Province, other.Province)
                   || string.Equals(ProvinceRelationship, other.ProvinceRelationship)
                   || string.Equals(HospitalRegion, other.HospitalRegion)
                   || string.Equals(HospitalRegionRelationship, other.HospitalRegionRelationship)
                   || string.Equals(BusinessRegion, other.BusinessRegion)
                   || string.Equals(BusinessRegionRelationship, other.BusinessRegionRelationship)
                   || string.Equals(ServiceClass, other.ServiceClass)
                   || string.Equals(TargetGroup, other.TargetGroup)
                   || string.Equals(LifeEvent, other.LifeEvent)
                   || string.Equals(IndustrialClass, other.IndustrialClass)
                   || string.Equals(OrganizationType, other.OrganizationType)
                   || string.Equals(ProvisionType, other.ProvisionType)
                   || string.Equals(ApiVersion, other.ApiVersion);
        }

        public bool Validate()
        {
            return !UrlBase.IsNullOrWhitespace() 
                   && !Municipality.IsNullOrWhitespace() 
                   && !Province.IsNullOrWhitespace() 
                   && !ProvinceRelationship.IsNullOrWhitespace() 
                   && !HospitalRegion.IsNullOrWhitespace() 
                   && !HospitalRegionRelationship.IsNullOrWhitespace() 
                   && !ServiceClass.IsNullOrWhitespace() 
                   && !TargetGroup.IsNullOrWhitespace() 
                   && !LifeEvent.IsNullOrWhitespace() 
                   && !IndustrialClass.IsNullOrWhitespace() 
                   && !OrganizationType.IsNullOrWhitespace() 
                   && !ProvisionType.IsNullOrWhitespace() 
                   // TODO: Not yet implemented
                   // && !BusinessRegionVersion.IsNullOrWhitespace() 
                   // && !BusinessRegionRelationshipVersion.IsNullOrWhitespace() 
                   && !ApiVersion.IsNullOrWhitespace();
        }
    }
}
