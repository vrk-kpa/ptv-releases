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


using Microsoft.AspNetCore.Mvc;
using Moq;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.Services;

namespace PTV.Application.OpenApi.Tests.DataValidators
{
    public abstract class ValidatorTestBase
    {
        protected Mock<ICodeService> codeServiceMockSetup;
        protected ICodeService codeService;
        
        protected Mock<ILanguageCache> languageCacheMockSetup;
        protected ILanguageCache languageCache;

        protected Mock<ICommonService> commonServiceMockSetup;
        protected ICommonService commonService;

        protected Mock<IGeneralDescriptionService> generalDescriptionServiceMockSetup;
        protected IGeneralDescriptionService generalDescriptionService;

        protected Mock<IFintoService> fintoServiceMockSetup;
        protected IFintoService fintoService;

        protected Mock<IOntologyTermDataCache> ontologyTermDataCacheMock;
        protected IOntologyTermDataCache ontologyTermDataCache;
        
        protected Mock<IChannelService> channelServiceMockSetup;
        protected IChannelService channelService;

        protected Mock<IServiceService> serviceServiceMockSetup;
        protected IServiceService serviceService;

        protected Mock<IOrganizationService> organizationServiceMockSetup;
        protected IOrganizationService organizationService;

        protected TestController controller;

        public ValidatorTestBase()
        {
            codeServiceMockSetup = new Mock<ICodeService>();
            codeService = codeServiceMockSetup.Object;
            
            languageCacheMockSetup = new Mock<ILanguageCache>();
            languageCache = languageCacheMockSetup.Object;

            commonServiceMockSetup = new Mock<ICommonService>();
            commonService = commonServiceMockSetup.Object;

            generalDescriptionServiceMockSetup = new Mock<IGeneralDescriptionService>();
            generalDescriptionService = generalDescriptionServiceMockSetup.Object;

            fintoServiceMockSetup = new Mock<IFintoService>();
            fintoService = fintoServiceMockSetup.Object;
            
            ontologyTermDataCacheMock = new Mock<IOntologyTermDataCache>();
            ontologyTermDataCache = ontologyTermDataCacheMock.Object;

            channelServiceMockSetup = new Mock<IChannelService>();
            channelService = channelServiceMockSetup.Object;

            serviceServiceMockSetup = new Mock<IServiceService>();
            serviceService = serviceServiceMockSetup.Object;

            organizationServiceMockSetup = new Mock<IOrganizationService>();
            organizationService = organizationServiceMockSetup.Object;

            controller = new TestController();
        }
    }

    public class TestController : Controller
    {

    }
}
