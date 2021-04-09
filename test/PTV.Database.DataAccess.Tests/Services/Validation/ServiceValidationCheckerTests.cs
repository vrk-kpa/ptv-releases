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
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using NLog;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Services.Validation;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Validation
{
    public class ServiceValidationCheckerTests:TestBase
    {
        private readonly ServiceValidationChecker serviceValidationChecker;

        private Guid serviceId = "serviceId".GetGuid();
        private Guid languageId = "fi".GetGuid();
        private Guid generalId = "gd".GetGuid();
        public ServiceValidationCheckerTests()
        {
            TestSetup();
            serviceValidationChecker = new ServiceValidationChecker(CacheManager,ResolveManager);
        }

        private void TestSetup()
        {
            var validationManagerMock = new Mock<IValidationManager>();
            validationManagerMock.As<IInternalValidation>()
                .Setup(x => x.CheckEntity<Model.Models.Organization>(
                    It.IsAny<Guid>(),
                    It.IsAny<IUnitOfWork>(),
                    It.IsAny<List<ValidationPath>>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<List<Guid>>()))
                .Returns(new Dictionary<Guid, List<ValidationMessage>>());
            var versioningManagerMock = new Mock<IVersioningManager>();
            versioningManagerMock
                .Setup(x => x.GetLastPublishedVersion<OrganizationVersioned>(
                    It.IsAny<IUnitOfWork>(),
                    It.IsAny<Guid>()))
                .Returns((VersionInfo)null);
            versioningManagerMock
                .Setup(x => x.ApplyPublishingStatusFilterFallback(
                    It.IsAny<IEnumerable<StatutoryServiceGeneralDescriptionVersioned>>()))
                .Returns((IEnumerable<StatutoryServiceGeneralDescriptionVersioned> input) => input.First());
            var commonServiceMock = new Mock<ICommonService>();

            ResolveManager.RegisterInstance(new TestRegisterServiceInfo { RegisterAs = typeof(IValidationManager), Instance = validationManagerMock.Object});
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo { RegisterAs = typeof(IVersioningManager), Instance = versioningManagerMock.Object});
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo { RegisterAs = typeof(ICommonService), Instance = commonServiceMock.Object});

            RegisterRepository<IServiceServiceClassRepository, ServiceServiceClass>(new List<ServiceServiceClass>().AsQueryable());
            RegisterRepository<IServiceOntologyTermRepository, ServiceOntologyTerm>(new List<ServiceOntologyTerm>().AsQueryable());
            RegisterRepository<IServiceProducerRepository, ServiceProducer>(new List<ServiceProducer>().AsQueryable());
        }

        /// <summary>
        /// Test for validate service target groups
        /// </summary>
        /// <param name="targetGroups">Selected main target group codes</param>
        /// <param name="subTargetGroups">Selected sub target group codes</param>
        /// <param name="valid">Test result - entity can be published from target groups point of view</param>
        [Theory]
        [InlineData("KR1,KR2,KR3","", false)]
        [InlineData("KR1,KR2,KR3","KR1.1", false)]
        [InlineData("KR1,KR2,KR3","KR2.1", true)]
        [InlineData("KR1,KR2,KR3","KR1.1,KR1.2", false)]
        [InlineData("KR1,KR2,KR3","KR2.1,KR2.2", true)]
        [InlineData("KR1,KR2,KR3","KR1.2,KR2.2", true)]
        public void ValidateEntityServiceTargetGroupTest(string targetGroups, string subTargetGroups, bool valid)
        {
            RegisterRepository<IRepository<ServiceVersioned>, ServiceVersioned>(GetTestServiceData());
            RegisterRepository<IServiceTargetGroupRepository, ServiceTargetGroup>(GetTestTargetGroupData(targetGroups, subTargetGroups));
            RegisterRepository<IOrganizationVersionedRepository, OrganizationVersioned>(
                new List<OrganizationVersioned>().AsQueryable());
            serviceValidationChecker.Init(serviceId, unitOfWorkMockSetup.Object, new List<ValidationPath>());
            serviceValidationChecker.SetLanguages(new List<Guid>{languageId});
            var result = serviceValidationChecker.ValidateEntity();

            var tgResult = result[languageId].FirstOrDefault(x => x.Key == "targetGroups");

            Assert.True(valid == (tgResult == null));

        }
        /// <summary>
        /// Test for validate target groups comes to service from connected general description
        /// </summary>
        /// <param name="targetGroups">Selected main general description target group codes</param>
        /// <param name="subTargetGroups">Selected sub general description target group codes</param>
        /// <param name="valid">Test result - entity can be published from target groups point of view</param>
        [Theory]
        [InlineData("KR1,KR2,KR3","", false)]
        [InlineData("KR1,KR2,KR3","KR1.1", false)]
        [InlineData("KR1,KR2,KR3","KR2.1", true)]
        [InlineData("KR1,KR2,KR3","KR1.1,KR1.2", false)]
        [InlineData("KR1,KR2,KR3","KR2.1,KR2.2", true)]
        [InlineData("KR1,KR2,KR3","KR1.2,KR2.2", true)]
        public void ValidateEntityGeneralDescriptionTargetGroupTest(string targetGroups, string subTargetGroups, bool valid)
        {
            RegisterRepository<IRepository<ServiceVersioned>, ServiceVersioned>(GetTestServiceData());
            RegisterRepository<IServiceTargetGroupRepository, ServiceTargetGroup>(GetTestTargetGroupData(targetGroups, subTargetGroups));
            RegisterRepository<IStatutoryServiceGeneralDescriptionVersionedRepository, StatutoryServiceGeneralDescriptionVersioned>(GetTestGeneralData(targetGroups, subTargetGroups));
            RegisterRepository<IOrganizationVersionedRepository, OrganizationVersioned>(
                new List<OrganizationVersioned>().AsQueryable());
            serviceValidationChecker.Init(serviceId, unitOfWorkMockSetup.Object, new List<ValidationPath>());
            serviceValidationChecker.SetLanguages(new List<Guid>{languageId});
            var result = serviceValidationChecker.ValidateEntity();

            var tgResult = result[languageId].FirstOrDefault(x => x.Key == "targetGroups");

            Assert.True(valid == (tgResult == null));

        }
        /// <summary>
        /// Test for validate service target groups and target groups comes to service from connected general description
        /// </summary>
        /// <param name="serviceTargetGroups">Selected main target group codes</param>
        /// <param name="serviceSubTargetGroups">Selected sub target group codes</param>
        /// <param name="genTargetGroups">Selected main general description target group codes</param>
        /// <param name="genSubTargetGroups">Selected sub general description target group codes</param>
        /// <param name="valid">Test result - entity can be published from target groups point of view</param>
        /// <example>
        /// KR1 - user select main tg KR1
        /// KR1:O - user deselect main tg KR1 comes from general description
        /// KR1.1 - user select sub tg KR1.1 of KR1 main tg
        /// KR1.1:O - user deselect sub tg KR1.1 of KR1 main tg comes from general description
        /// </example>
        [Theory]
        [InlineData("","KR2.1:O","KR2","KR2.1", false)]
        [InlineData("KR1","KR1.1","KR2","", false)]
        [InlineData("KR1","KR1.1,KR2.2","KR2","", true)]
        [InlineData("","KR1.1,KR2.2","KR1,KR2","", true)]
        [InlineData("KR1:O","KR2.2","KR1,KR2","", true)]
        [InlineData("KR1:O,KR2:O,KR3","","KR1,KR2","", true)]
        [InlineData("","KR1.1:O,KR2.1:O","KR1,KR2","KR1.1,KR1.2,KR2.1,KR2.2", true)]
        [InlineData("","KR1.1:O,KR2.1:O,KR2.2:O","KR1,KR2","KR1.1,KR1.2,KR2.1,KR2.2", false)]
        [InlineData("","KR1.1:O,KR2.1:O,KR1.2:O","KR1,KR2","KR1.1,KR1.2,KR2.1,KR2.2", true)]
        [InlineData("","KR2.1","KR2","", true)]
        public void ValidateEntityTargetGroupTest(
            string serviceTargetGroups,
            string serviceSubTargetGroups,
            string genTargetGroups,
            string genSubTargetGroups,
            bool valid)
        {
            RegisterRepository<IRepository<ServiceVersioned>, ServiceVersioned>(GetTestServiceData(generalId));
            RegisterRepository<IServiceTargetGroupRepository, ServiceTargetGroup>(GetTestTargetGroupData(serviceTargetGroups, serviceSubTargetGroups));
            RegisterRepository<IStatutoryServiceGeneralDescriptionVersionedRepository, StatutoryServiceGeneralDescriptionVersioned>(GetTestGeneralData(genTargetGroups, genSubTargetGroups));
            RegisterRepository<IOrganizationVersionedRepository, OrganizationVersioned>(
                new List<OrganizationVersioned>().AsQueryable());
            serviceValidationChecker.Init(serviceId, unitOfWorkMockSetup.Object, new List<ValidationPath>());
            serviceValidationChecker.SetLanguages(new List<Guid>{languageId});
            var result = serviceValidationChecker.ValidateEntity();

            var tgResult = result[languageId].FirstOrDefault(x => x.Key == "targetGroups");

            Assert.True(valid == (tgResult == null));

        }

        private List<ServiceLanguageAvailability> GetLanguages()
        {
            return new List<ServiceLanguageAvailability>
            {
                new ServiceLanguageAvailability
                {
                    LanguageId = languageId,
                    StatusId = PublishingStatusCache.Get(PublishingStatus.Published)
                }
            };
        }

        private IQueryable<ServiceVersioned> GetTestServiceData(Guid? genDescId = null)
        {
            return new List<ServiceVersioned>
            {
                new ServiceVersioned
                {
                    Id = serviceId,
                    LanguageAvailabilities = GetLanguages(),
                    StatutoryServiceGeneralDescriptionId = genDescId
                }
            }.AsQueryable();
        }

        private IQueryable<ServiceTargetGroup> GetTestTargetGroupData(string targetGroups, string subTargetGroups)
        {
            var tgs = CreateServiceTargetGroups(targetGroups);
            tgs.AddRange(CreateServiceTargetGroups(subTargetGroups));

            foreach (var tg in tgs)
            {
                tg.ServiceVersionedId = serviceId;
            }

            return tgs.AsQueryable();
        }

        private IQueryable<StatutoryServiceGeneralDescriptionVersioned> GetTestGeneralData(string targetGroups, string subTargetGroups)
        {
            var tgs = CreateGeneralTargetGroups(targetGroups);
            tgs.AddRange(CreateGeneralTargetGroups(subTargetGroups));
            return new List<StatutoryServiceGeneralDescriptionVersioned>
            {
                new StatutoryServiceGeneralDescriptionVersioned
                {
                    UnificRootId = generalId,
                    TargetGroups = tgs
                }
            }.AsQueryable();

        }


        private List<ServiceTargetGroup> CreateServiceTargetGroups(string targetGroups)
        {
            var result = new List<ServiceTargetGroup>();
            targetGroups.Split(',').Where(x=>x.Length>0).ForEach(tgCode =>
            {
                var over = tgCode.Contains("O");
                var code = tgCode.Split(':').First();
                result.Add(new ServiceTargetGroup
                {
                    TargetGroup = CreateTargetGroup(code),
                    TargetGroupId = code.GetGuid(),
                    Override = over
                });
            });
            return result;
        }

        private List<StatutoryServiceTargetGroup> CreateGeneralTargetGroups(string targetGroups)
        {
            var result = new List<StatutoryServiceTargetGroup>();
            targetGroups.Split(',').Where(x=>x.Length>0).ForEach(tgCode =>
            {
                result.Add(new StatutoryServiceTargetGroup
                {
                    TargetGroup = CreateTargetGroup(tgCode),
                    TargetGroupId = tgCode.GetGuid()
                });
            });
            return result;
        }

        private TargetGroup CreateTargetGroup(string tgCode)
        {
            return new TargetGroup
            {
                Code = tgCode,
                Id = tgCode.GetGuid(),
                ParentId = tgCode.Contains('.') ? tgCode.Split('.').First().GetGuid() : (Guid?) null
            };
        }
    }
}
