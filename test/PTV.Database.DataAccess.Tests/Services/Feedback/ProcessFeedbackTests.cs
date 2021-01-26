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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Services;
using PTV.Domain.Model.Models.Feedback;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Logging;
using PTV.NetworkServices.Emailing;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Feedback
{
    public class ProcessFeedbackTests
    {
        [Fact]
        public void CheckCalls()
        {
            var organizationCacheMock = new Mock<IOrganizationTreeDataCache>();
            organizationCacheMock.Setup(m => m.GetMainOrganizationIds(It.IsAny<Guid>()))
                .Returns((Guid.Empty, Guid.Empty));

            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(m => m.GetPahaTokenAuthentication(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<VmJobLogEntry>())).Returns("Token");
            emailServiceMock.Setup(m => m.SendEmailToOrganizationUsers(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<VmJobLogEntry>()))
                .Returns((true, String.Empty));

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x.GetSection(It.IsAny<string>()))
                .Returns(new Mock<IConfigurationSection>().Object);
//            var applicationConfiguration = new FakeApplicationConfiguration(configurationMock.Object);

            var emailConfigurationMock = new Mock<EmailNotificationConfiguration>();
            emailConfigurationMock.Setup(x => x.TemplateText).Returns("Template.");
            var applicationConfiguration = new Mock<ApplicationConfiguration>(configurationMock.Object);
            applicationConfiguration.Setup(x => x.GetEmailConfiguration()).Returns(emailConfigurationMock.Object);
            var service = new FeedbackService(emailServiceMock.Object, organizationCacheMock.Object, applicationConfiguration.Object, null);

            var result = service.ProcessFeedback(new VmFeedback());

            organizationCacheMock.Verify(m => m.GetMainOrganizationIds(It.IsAny<Guid>()), Times.Once);
            emailServiceMock.Verify(m => m.SendEmailToOrganizationUsers(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<VmJobLogEntry>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
