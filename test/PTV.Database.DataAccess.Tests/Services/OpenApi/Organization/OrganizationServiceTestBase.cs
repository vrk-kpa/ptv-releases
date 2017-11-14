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

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Models;
using PTV.Domain.Logic.Channels;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public abstract class OrganizationServiceTestBase : ServiceTestBase
    {
        internal ILogger<DataAccess.Services.OrganizationService> Logger { get; private set; }

        internal OrganizationLogic OrganizationLogic { get; private set; }

        internal Mock<IOrganizationVersionedRepository> OrganizationRepoMock { get; private set; }

        internal Mock<IOrganizationNameRepository> OrganizationNameRepoMock { get; private set; }

        public OrganizationServiceTestBase()
        {
            Logger = new Mock<ILogger<DataAccess.Services.OrganizationService>>().Object;
            var mapServiceProvider = new MapServiceProvider(
                (new Mock<IHostingEnvironment>()).Object,
                new ApplicationConfiguration((new Mock<IConfigurationRoot>()).Object),
                (new Mock<IOptions<ProxyServerSettings>>()).Object,
                new Mock<ILogger<MapServiceProvider>>().Object);
            OrganizationLogic = new OrganizationLogic(new ChannelAttachmentLogic(), new WebPageLogic(), AddressLogic);
            OrganizationRepoMock = new Mock<IOrganizationVersionedRepository>();
            OrganizationNameRepoMock = new Mock<IOrganizationNameRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationVersionedRepository>()).Returns(OrganizationRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<OrganizationVersioned>>()).Returns(OrganizationRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationNameRepository>()).Returns(OrganizationNameRepoMock.Object);
        }
    }
}
