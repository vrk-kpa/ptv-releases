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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Models;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Common
{
    public class UpdateModifiedDatesTest
    {
        private readonly DateTime dateModified = new DateTime(2019, 1, 1);
        
        [Fact]
        public void UpdateModifiedDate_Services()
        {
            var versionedList = new List<ServiceVersioned>{ new ServiceVersioned() };
            var languageList = new List<ServiceLanguageAvailability>{ new ServiceLanguageAvailability() };
            var commonService = GetCommonService;
            
            commonService.UpdateModifiedDates(versionedList, languageList, dateModified);
            
            Assert.Equal(dateModified, versionedList[0].Modified);
            Assert.Equal(dateModified, languageList[0].Modified);
        }
        
        [Fact]
        public void UpdateModifiedDate_Channels()
        {
            var versionedList = new List<ServiceChannelVersioned>{ new ServiceChannelVersioned() };
            var languageList = new List<ServiceChannelLanguageAvailability>{ new ServiceChannelLanguageAvailability() };
            var commonService = GetCommonService;
            
            commonService.UpdateModifiedDates(versionedList, languageList, dateModified);
            
            Assert.Equal(dateModified, versionedList[0].Modified);
            Assert.Equal(dateModified, languageList[0].Modified);
        }
        
        [Fact]
        public void UpdateModifiedDate_Organizations()
        {
            var versionedList = new List<OrganizationVersioned>{ new OrganizationVersioned() };
            var languageList = new List<OrganizationLanguageAvailability>{ new OrganizationLanguageAvailability() };
            var commonService = GetCommonService;
            
            commonService.UpdateModifiedDates(versionedList, languageList, dateModified);
            
            Assert.Equal(dateModified, versionedList[0].Modified);
            Assert.Equal(dateModified, languageList[0].Modified);
        }
        
        [Fact]
        public void UpdateModifiedDate_GeneralDescriptions()
        {
            var versionedList = new List<StatutoryServiceGeneralDescriptionVersioned>{ new StatutoryServiceGeneralDescriptionVersioned() };
            var languageList = new List<GeneralDescriptionLanguageAvailability>{ new GeneralDescriptionLanguageAvailability() };
            var commonService = GetCommonService;
            
            commonService.UpdateModifiedDates(versionedList, languageList, dateModified);
            
            Assert.Equal(dateModified, versionedList[0].Modified);
            Assert.Equal(dateModified, languageList[0].Modified);
        }
        
        [Fact]
        public void UpdateModifiedDate_ServiceCollections()
        {
            var versionedList = new List<ServiceCollectionVersioned>{ new ServiceCollectionVersioned() };
            var languageList = new List<ServiceCollectionLanguageAvailability>{ new ServiceCollectionLanguageAvailability() };
            var commonService = GetCommonService;
            
            commonService.UpdateModifiedDates(versionedList, languageList, dateModified);
            
            Assert.Equal(dateModified, versionedList[0].Modified);
            Assert.Equal(dateModified, languageList[0].Modified);
        }
        
        private static ICommonServiceInternal GetCommonService => new CommonService(
            null, 
            null, 
            null, 
            null, 
            null, 
            null, 
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
    }
}
