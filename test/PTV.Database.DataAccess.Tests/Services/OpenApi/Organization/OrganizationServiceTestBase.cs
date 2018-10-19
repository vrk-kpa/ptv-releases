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
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Models;
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.Organization
{
    public abstract class OrganizationServiceTestBase : ServiceTestBase
    {
        internal ILogger<DataAccess.Services.OrganizationService> Logger { get; private set; }

        internal OrganizationLogic OrganizationLogic { get; private set; }

        internal Mock<IOrganizationVersionedRepository> OrganizationRepoMock { get; private set; }

        internal Mock<IOrganizationNameRepository> OrganizationNameRepoMock { get; private set; }

        internal Mock<IOrganizationServiceRepository> OrganizationServiceRepoMock { get; private set; }

        internal Mock<IServiceVersionedRepository> ServiceRepoMock { get; private set; }

        internal Mock<IServiceLanguageAvailabilityRepository> ServiceLanguageAvailabilityRepoMock { get; private set; }

        internal Mock<IServiceNameRepository> ServiceNameRepoMock { get; private set; }

        internal Mock<IServiceProducerOrganizationRepository> ServiceProducerRepoMock { get; private set; }

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
            OrganizationServiceRepoMock = new Mock<IOrganizationServiceRepository>();
            ServiceRepoMock = new Mock<IServiceVersionedRepository>();
            ServiceLanguageAvailabilityRepoMock = new Mock<IServiceLanguageAvailabilityRepository>();
            ServiceNameRepoMock = new Mock<IServiceNameRepository>();
            ServiceProducerRepoMock = new Mock<IServiceProducerOrganizationRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationVersionedRepository>()).Returns(OrganizationRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IRepository<OrganizationVersioned>>()).Returns(OrganizationRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationNameRepository>()).Returns(OrganizationNameRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IOrganizationServiceRepository>()).Returns(OrganizationServiceRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceVersionedRepository>()).Returns(ServiceRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceLanguageAvailabilityRepository>()).Returns(ServiceLanguageAvailabilityRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceNameRepository>()).Returns(ServiceNameRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceProducerOrganizationRepository>()).Returns(ServiceProducerRepoMock.Object);
        }

        internal ServiceVersioned GetAndSetServiceForOrganization(OrganizationVersioned item, Guid statusId, string channelName,
            Guid? languageId = null, bool setMain = true, bool setOtherResponsible = true, bool setProducer = true)
        {
            var langId = languageId.HasValue ? languageId.Value : LanguageCache.Get("fi");
            var service = EntityGenerator.CreateEntity<ServiceVersioned, Model.Models.Service, ServiceLanguageAvailability>(statusId);
            service.ServiceNames.Add(new ServiceName
            {
                Name = channelName,
                LocalizationId = langId,
                TypeId = TypeCache.Get<NameType>(NameTypeEnum.Name.ToString()),
                ServiceVersionedId = service.Id,
            });
            service.LanguageAvailabilities.Add(new ServiceLanguageAvailability { StatusId = statusId, LanguageId = langId, ServiceVersionedId = service.Id });
            
            // main responsible
            if (setMain)
            {
                service.Organization = item.UnificRoot;
                service.Organization.Versions.Add(item);
                service.OrganizationId = item.UnificRootId;
            }
            // other responsible
            if (setOtherResponsible)
            {
                item.UnificRoot.OrganizationServices.Add(new OrganizationService
                {
                    ServiceVersioned = service,
                    ServiceVersionedId = service.Id,
                    OrganizationId = item.UnificRootId
                });
            }
            // producer
            if (setProducer)
            {
                item.UnificRoot.ServiceProducerOrganizations.Add(new ServiceProducerOrganization
                {
                    ServiceProducer = new ServiceProducer
                    {
                        ServiceVersioned = service,
                        ServiceVersionedId = service.UnificRootId,
                    },
                    OrganizationId = item.UnificRootId
                });
            }
            return service;
        }
    }
}
