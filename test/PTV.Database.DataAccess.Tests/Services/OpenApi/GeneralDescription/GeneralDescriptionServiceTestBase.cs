﻿/**
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

using Microsoft.Extensions.Logging;
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.GeneralDescription
{
    public abstract class GeneralDescriptionServiceTestBase : ServiceTestBase
    {
        internal ILogger<GeneralDescriptionService> Logger;

        internal Mock<IStatutoryServiceGeneralDescriptionVersionedRepository> GdRepoMock { get; private set; }
        internal Mock<IStatutoryServiceServiceClassRepository> ServiceClassRepoMock { get; private set; }
        internal Mock<IStatutoryServiceOntologyTermRepository> OntologyTermRepoMock { get; private set; }
        internal Mock<IStatutoryServiceLifeEventRepository> LifeEventRepoMock { get; private set; }
        internal Mock<IStatutoryServiceIndustrialClassRepository> IndustrialClassRepoMock { get; private set; }
        internal Mock<IStatutoryServiceTargetGroupRepository> TargetGroupRepoMock { get; private set; }
        internal Mock<IStatutoryServiceLawRepository> LawRepoMock { get; private set; }
        internal Mock<IGeneralDescriptionServiceChannelRepository> GdConnectionRepoMock { get; private set; }
        internal Mock<IServiceChannelVersionedRepository> ServiceChannelRepoMock { get; private set; }

        public GeneralDescriptionServiceTestBase()
        {
            Logger = new Mock<ILogger<GeneralDescriptionService>>().Object;

            GdRepoMock = new Mock<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            ServiceClassRepoMock = new Mock<IStatutoryServiceServiceClassRepository>();
            OntologyTermRepoMock = new Mock<IStatutoryServiceOntologyTermRepository>();
            LifeEventRepoMock = new Mock<IStatutoryServiceLifeEventRepository>();
            IndustrialClassRepoMock = new Mock<IStatutoryServiceIndustrialClassRepository>();
            TargetGroupRepoMock = new Mock<IStatutoryServiceTargetGroupRepository>();
            LawRepoMock = new Mock<IStatutoryServiceLawRepository>();
            GdConnectionRepoMock = new Mock<IGeneralDescriptionServiceChannelRepository>();
            ServiceChannelRepoMock = new Mock<IServiceChannelVersionedRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>()).Returns(GdRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IStatutoryServiceServiceClassRepository>()).Returns(ServiceClassRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IStatutoryServiceOntologyTermRepository>()).Returns(OntologyTermRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IStatutoryServiceLifeEventRepository>()).Returns(LifeEventRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IStatutoryServiceIndustrialClassRepository>()).Returns(IndustrialClassRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IStatutoryServiceTargetGroupRepository>()).Returns(TargetGroupRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IStatutoryServiceLawRepository>()).Returns(LawRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IGeneralDescriptionServiceChannelRepository>()).Returns(GdConnectionRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IServiceChannelVersionedRepository>()).Returns(ServiceChannelRepoMock.Object);
        }

        internal GeneralDescriptionServiceChannel GetAndSetConnectionForGD(StatutoryServiceGeneralDescriptionVersioned gd, Guid statusId, string channelName, Guid? languageId = null)
        {
            var langId = languageId.HasValue ? languageId.Value : LanguageCache.Get("fi");
            var serviceChannel = EntityGenerator.CreateEntity<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(statusId);
            serviceChannel.ServiceChannelNames.Add(new ServiceChannelName { Name = channelName, LocalizationId = langId });
            serviceChannel.LanguageAvailabilities.Add(new ServiceChannelLanguageAvailability { StatusId = statusId, LanguageId = langId });

            var connection = new GeneralDescriptionServiceChannel
            {
                ServiceChannel = serviceChannel.UnificRoot,
                ServiceChannelId = serviceChannel.UnificRootId,
                StatutoryServiceGeneralDescriptionId = gd.UnificRootId,
                StatutoryServiceGeneralDescription = gd.UnificRoot
            };
            gd.UnificRoot.StatutoryServiceGeneralDescriptionServiceChannels.Add(connection);

            return connection;
        }
    }
}
