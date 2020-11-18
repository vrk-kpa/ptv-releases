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
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Services.V2;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Organization
{
    public class GetOrganizationMissingLanguagesTest : TestBase
    {

        private readonly OrganizationServiceInternal organizationService;
        private readonly Mock<IVersioningManager> versioningManagerMock;
        private Mock<IServiceUtilities> serviceUtilitiesMock;

        private const string MyOrganizationId = "MyOrgId";
        private const string MyOrganizationRootId = "MyOrgRootId";

        public GetOrganizationMissingLanguagesTest()
        {
            versioningManagerMock = new Mock<IVersioningManager>(MockBehavior.Strict);
            serviceUtilitiesMock = new Mock<IServiceUtilities>(MockBehavior.Strict);

            organizationService = new OrganizationServiceInternal
            (
                serviceUtilitiesMock.Object,
                versioningManagerMock.Object,
                CacheManager,
                null
            );
            SetupTypesCacheMock<PublishingStatusType>();
        }

        private void BaseTestSetup(
            string orgLangs,
            string channelLangs,
            string serviceLangs,
            PublishingStatus orgStatus,
            PublishingStatus channelStatus,
            PublishingStatus serviceStatus)
        {
            var oLangs = orgLangs.Split(',').ToList();
            var cLangs = channelLangs.Split(',').ToList();
            var sLangs = serviceLangs.Split(',').ToList();

            SetupContextManager<object, IEnumerable<Guid>>();
            RegisterRepository<IServiceChannelLanguageAvailabilityRepository, ServiceChannelLanguageAvailability>(CreateChannelLanguages(cLangs, channelStatus).AsQueryable());
            RegisterRepository<IServiceLanguageAvailabilityRepository, ServiceLanguageAvailability>(CreateServiceLanguages(sLangs, serviceStatus).AsQueryable());
            RegisterRepository<IOrganizationLanguageAvailabilityRepository, OrganizationLanguageAvailability>(CreateOrganizationLanguages(oLangs, orgStatus).AsQueryable());
            var id = MyOrganizationId.GetGuid();
            var rootId = MyOrganizationRootId.GetGuid();
            IList<Guid> myOrgs = new List<Guid> {rootId};
            versioningManagerMock
                .Setup(x => x.GetUnificRootId<OrganizationVersioned>(unitOfWorkMockSetup.Object, id)).Returns(rootId);
            serviceUtilitiesMock.Setup(x => x.GetAllUserOrganizations()).Returns(myOrgs.ToList());
            serviceUtilitiesMock.Setup(x => x.UserHighestRole()).Returns(UserRoleEnum.Eeva);
        }

        /// <summary>
        /// Check missing languages for organization
        /// </summary>
        /// <param name="orgLangs">List of language codes of published organization languages</param>
        /// <param name="channelLangs">List of language codes of published channel languages</param>
        /// <param name="serviceLangs">List of language codes of published service languages</param>
        /// <param name="missingLangs">List of missing language codes of organization</param>
        [Theory]
        [InlineData("fi","fi,sv","en", "sv,en")]
        [InlineData("fi,sv","fi,sv","en", "en")]
        [InlineData("fi,sv,en","fi,sv","en", "")]
        [InlineData("fi,sv","fi","sv", "")]
        [InlineData("fi,sv,en","en","en", "")]
        [InlineData("fi,sv,en","se,smn,sms","en", "se,smn,sms")]
        [InlineData("fi,sv,en,se,smn,sms","fi,sv,en","se,smn,sms", "")]
        [InlineData("fi","fi,sv,en,se,smn,sms","se,smn,sms", "sv,en,se,smn,sms")]
        [InlineData("fi","fi,sv,en","se,smn,sms", "sv,en,se,smn,sms")]
        [InlineData("fi,sms","fi,sv,en","se,smn,sms", "sv,en,se,smn")]
        public void CheckMissingLanguagesCase1(string orgLangs, string channelLangs, string serviceLangs, string missingLangs)
        {
            var mLangs = missingLangs.IsNullOrEmpty() ? new List<Guid>() : missingLangs.Split(',').Select(code => code.GetGuid());
            BaseTestSetup(orgLangs, channelLangs, serviceLangs, PublishingStatus.Published, PublishingStatus.Published, PublishingStatus.Published);
            var result =
                organizationService.GetOrganizationMissingLanguages(MyOrganizationId.GetGuid(),
                    unitOfWorkMockSetup.Object).ToList();
            result.Should().NotBeNull();
            result.Count().Should().Be(mLangs.Count());
            result.ForEach(missingLang =>
                mLangs.Contains(missingLang).Should().BeTrue());
        }

        /// <summary>
        /// Check missing languages for organization
        /// </summary>
        /// <param name="orgLangs">List of language codes of draft organization languages</param>
        /// <param name="channelLangs">List of language codes of draft channel languages</param>
        /// <param name="serviceLangs">List of language codes of draft service languages</param>
        /// <param name="missingLangs">List of missing language codes of organization</param>
        [Theory]
        [InlineData("fi","fi,sv","en", "sv,en")]
        [InlineData("fi,sv","fi,sv","en", "en")]
        [InlineData("fi,sv,en","fi,sv","en", "")]
        [InlineData("fi,sv","fi","sv", "")]
        [InlineData("fi,sv,en","en","en", "")]
        [InlineData("fi,sv,en","se,smn,sms","en", "se,smn,sms")]
        [InlineData("fi,sv,en,se,smn,sms","fi,sv,en","se,smn,sms", "")]
        [InlineData("fi","fi,sv,en,se,smn,sms","se,smn,sms", "sv,en,se,smn,sms")]
        [InlineData("fi","fi,sv,en","se,smn,sms", "sv,en,se,smn,sms")]
        [InlineData("fi,sms","fi,sv,en","se,smn,sms", "sv,en,se,smn")]
        public void CheckMissingLanguagesCase2(string orgLangs, string channelLangs, string serviceLangs, string missingLangs)
        {
            var mLangs = missingLangs.IsNullOrEmpty() ? new List<Guid>() : missingLangs.Split(',').Select(code => code.GetGuid());
            BaseTestSetup(orgLangs, channelLangs, serviceLangs, PublishingStatus.Draft, PublishingStatus.Draft, PublishingStatus.Draft);
            var result =
                organizationService.GetOrganizationMissingLanguages(MyOrganizationId.GetGuid(),
                    unitOfWorkMockSetup.Object).ToList();
            result.Should().NotBeNull();
            result.Count().Should().Be(mLangs.Count());
            result.ForEach(missingLang =>
                mLangs.Contains(missingLang).Should().BeTrue());
        }

        /// <summary>
        /// Check missing languages for organization
        /// </summary>
        /// <param name="orgLangs">List of language codes of published organization languages</param>
        /// <param name="channelLangs">List of language codes of draft channel languages</param>
        /// <param name="serviceLangs">List of language codes of draft service languages</param>
        /// <param name="missingLangs">List of missing language codes of organization</param>
        [Theory]
        [InlineData("fi","fi,sv","en", "sv,en")]
        [InlineData("fi,sv","fi,sv","en", "en")]
        [InlineData("fi,sv,en","fi,sv","en", "")]
        [InlineData("fi,sv","fi","sv", "")]
        [InlineData("fi,sv,en","en","en", "")]
        [InlineData("fi,sv,en","se,smn,sms","en", "se,smn,sms")]
        [InlineData("fi,sv,en,se,smn,sms","fi,sv,en","se,smn,sms", "")]
        [InlineData("fi","fi,sv,en,se,smn,sms","se,smn,sms", "sv,en,se,smn,sms")]
        [InlineData("fi","fi,sv,en","se,smn,sms", "sv,en,se,smn,sms")]
        [InlineData("fi,sms","fi,sv,en","se,smn,sms", "sv,en,se,smn")]
        public void CheckMissingLanguagesCase3(string orgLangs, string channelLangs, string serviceLangs, string missingLangs)
        {
            var mLangs = missingLangs.IsNullOrEmpty() ? new List<Guid>() : missingLangs.Split(',').Select(code => code.GetGuid());
            BaseTestSetup(orgLangs, channelLangs, serviceLangs, PublishingStatus.Published, PublishingStatus.Draft, PublishingStatus.Draft);
            var result =
                organizationService.GetOrganizationMissingLanguages(MyOrganizationId.GetGuid(),
                    unitOfWorkMockSetup.Object).ToList();
            result.Should().NotBeNull();
            result.Count().Should().Be(mLangs.Count());
            result.ForEach(missingLang =>
                mLangs.Contains(missingLang).Should().BeTrue());
        }

        /// <summary>
        /// Check missing languages for organization
        /// </summary>
        /// <param name="orgLangs">List of language codes of published organization languages</param>
        /// <param name="channelLangs">List of language codes of published channel languages</param>
        /// <param name="serviceLangs">List of language codes of deleted service languages</param>
        /// <param name="missingLangs">List of missing language codes of organization</param>
        [Theory]
        [InlineData("fi","fi,sv","en", "sv")]
        [InlineData("fi,sv","fi,sv","en", "")]
        [InlineData("fi,sv,en","fi,sv","en", "")]
        [InlineData("fi,sv","fi","sv", "")]
        [InlineData("fi,sv,en","en","en", "")]
        [InlineData("fi,sv,en","se,smn,sms","en", "se,smn,sms")]
        [InlineData("fi,sv,en,se,smn,sms","fi,sv,en","se,smn,sms", "")]
        [InlineData("fi","fi,sv,en,se,smn,sms","se,smn,sms", "sv,en,se,smn,sms")]
        [InlineData("fi","fi,sv,en","se,smn,sms", "sv,en")]
        [InlineData("fi,sms","fi,sv,en","se,smn,sms", "sv,en")]
        public void CheckMissingLanguagesCase4(string orgLangs, string channelLangs, string serviceLangs, string missingLangs)
        {
            var mLangs = missingLangs.IsNullOrEmpty() ? new List<Guid>() : missingLangs.Split(',').Select(code => code.GetGuid());
            BaseTestSetup(orgLangs, channelLangs, serviceLangs, PublishingStatus.Published, PublishingStatus.Published, PublishingStatus.Deleted);
            var result =
                organizationService.GetOrganizationMissingLanguages(MyOrganizationId.GetGuid(),
                    unitOfWorkMockSetup.Object).ToList();
            result.Should().NotBeNull();
            result.Count().Should().Be(mLangs.Count());
            result.ForEach(missingLang =>
                mLangs.Contains(missingLang).Should().BeTrue());
        }

        /// <summary>
        /// Check missing languages for organization
        /// </summary>
        /// <param name="orgLangs">List of language codes of published organization languages</param>
        /// <param name="channelLangs">List of language codes of draft channel languages</param>
        /// <param name="serviceLangs">List of language codes of published service languages</param>
        /// <param name="missingLangs">List of missing language codes of organization</param>
        [Theory]
        [InlineData("fi","fi,sv","en", "sv,en")]
        [InlineData("fi,sv","fi,sv","en", "en")]
        [InlineData("fi,sv,en","fi,sv","en", "")]
        [InlineData("fi,sv","fi","sv", "")]
        [InlineData("fi,sv,en","en","en", "")]
        [InlineData("fi,sv,en","se,smn,sms","en", "se,smn,sms")]
        [InlineData("fi,sv,en,se,smn,sms","fi,sv,en","se,smn,sms", "")]
        [InlineData("fi","fi,sv,en,se,smn,sms","se,smn,sms", "sv,en,se,smn,sms")]
        [InlineData("fi","fi,sv,en","se,smn,sms", "sv,en,se,smn,sms")]
        [InlineData("fi,sms","fi,sv,en","se,smn,sms", "sv,en,se,smn")]
        public void CheckMissingLanguagesCase5(string orgLangs, string channelLangs, string serviceLangs, string missingLangs)
        {
            var mLangs = missingLangs.IsNullOrEmpty() ? new List<Guid>() : missingLangs.Split(',').Select(code => code.GetGuid());
            BaseTestSetup(orgLangs, channelLangs, serviceLangs, PublishingStatus.Published, PublishingStatus.Draft, PublishingStatus.Published);
            var result =
                organizationService.GetOrganizationMissingLanguages(MyOrganizationId.GetGuid(),
                    unitOfWorkMockSetup.Object).ToList();
            result.Should().NotBeNull();
            result.Count().Should().Be(mLangs.Count());
            result.ForEach(missingLang =>
                mLangs.Contains(missingLang).Should().BeTrue());
        }
        private List<OrganizationLanguageAvailability> CreateOrganizationLanguages(List<string> publishedLanguages, PublishingStatus status)
        {
            var result = new List<OrganizationLanguageAvailability>();
            publishedLanguages.ForEach(lang =>
                result.Add(
                    new OrganizationLanguageAvailability
                    {
                        OrganizationVersionedId = MyOrganizationId.GetGuid(),
                        StatusId = status.ToString().GetGuid(),
                        LanguageId = lang.GetGuid()
                    }));
            return result;
        }
        private List<ServiceChannelLanguageAvailability> CreateChannelLanguages(List<string> publishedLanguages, PublishingStatus status)
        {
            var result = new List<ServiceChannelLanguageAvailability>();
            publishedLanguages.ForEach(lang =>
                result.Add(
                    new ServiceChannelLanguageAvailability
                    {
                        ServiceChannelVersioned = new ServiceChannelVersioned
                        {
                           OrganizationId = MyOrganizationRootId.GetGuid(),
                           PublishingStatusId = status.ToString().GetGuid(),
                           UnificRootId = "rootId".GetGuid()
                        },
                        StatusId = status.ToString().GetGuid(),
                        LanguageId = lang.GetGuid()
                    }));
            return result;
        }

        private List<ServiceLanguageAvailability> CreateServiceLanguages(List<string> publishedLanguages, PublishingStatus status)
        {
            var result = new List<ServiceLanguageAvailability>();
            publishedLanguages.ForEach(lang =>
                result.Add(
                    new ServiceLanguageAvailability
                    {
                        ServiceVersioned = new ServiceVersioned
                        {
                            OrganizationId = MyOrganizationRootId.GetGuid(),
                            PublishingStatusId = status.ToString().GetGuid(),
                            UnificRootId = "rootId".GetGuid()
                        },
                        StatusId = status.ToString().GetGuid(),
                        LanguageId = lang.GetGuid()
                    }));
            return result;
        }
    }
}
