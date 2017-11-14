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

using Microsoft.Extensions.Logging;
using Moq;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Services;

namespace PTV.Database.DataAccess.Tests.Services.OpenApi.GeneralDescription
{
    public abstract class GeneralDescriptionServiceTestBase : ServiceTestBase
    {
        internal ILogger<GeneralDescriptionService> Logger;

        internal Mock<IStatutoryServiceGeneralDescriptionVersionedRepository> GdRepoMock { get; private set; }
        internal Mock<IGeneralDescriptionServiceChannelRepository> GdConnectionRepoMock { get; private set; }

        public GeneralDescriptionServiceTestBase()
        {
            Logger = new Mock<ILogger<GeneralDescriptionService>>().Object;

            GdRepoMock = new Mock<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            GdConnectionRepoMock = new Mock<IGeneralDescriptionServiceChannelRepository>();
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>()).Returns(GdRepoMock.Object);
            unitOfWorkMockSetup.Setup(uw => uw.CreateRepository<IGeneralDescriptionServiceChannelRepository>()).Returns(GdConnectionRepoMock.Object);
        }
    }
}
