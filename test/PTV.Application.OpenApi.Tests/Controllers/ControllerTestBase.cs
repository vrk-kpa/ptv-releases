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

using Microsoft.Extensions.Options;
using Moq;
using PTV.Application.OpenApi.Models;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Security;
using System;

namespace PTV.Application.OpenApi.Tests.Controllers
{
    public abstract class ControllerTestBase
    {
        protected Mock<IChannelService> channelServiceMockSetup;
        protected Mock<IServiceAndChannelService> serviceAndChannelServiceMockSetup;
        protected Mock<IServiceService> serviceServiceMockSetup;
        protected Mock<ICommonService> commonServiceMockSetup;
        protected Mock<ICodeService> codeServiceMockSetup;

        protected IChannelService channelService;
        protected IServiceAndChannelService serviceAndChannelService;
        protected IServiceService serviceService;
        protected ICommonService commonService;
        protected ICodeService codeService;

        protected IUserInfoService userService;

        protected IOptions<AppSettings> settings;

        public ControllerTestBase()
        {
            channelServiceMockSetup = new Mock<IChannelService>();
            channelService = channelServiceMockSetup.Object;
            serviceAndChannelServiceMockSetup = new Mock<IServiceAndChannelService>();
            //serviceAndChannelService = serviceAndChannelServiceMockSetup.Object;
            serviceServiceMockSetup = new Mock<IServiceService>();
            serviceService = serviceServiceMockSetup.Object;
            commonServiceMockSetup = new Mock<ICommonService>();
            commonService = commonServiceMockSetup.Object;
            codeServiceMockSetup = new Mock<ICodeService>();
            codeService = codeServiceMockSetup.Object;

            var userServiceMock = new Mock<IUserInfoService>();
            userServiceMock.Setup(u => u.GetUserInfo()).Returns(new VmUserInfo() { UserOrganization = Guid.NewGuid() });
            userService = userServiceMock.Object;

            var settingsMock = new Mock<IOptions<AppSettings>>();
            settingsMock.Setup(s => s.Value).Returns(new AppSettings());
            settings = settingsMock.Object;
        }
    }
}
