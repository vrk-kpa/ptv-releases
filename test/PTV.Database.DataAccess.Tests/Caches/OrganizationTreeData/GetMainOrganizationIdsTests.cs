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
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.Organizations;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.ServiceDataHolders;
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Framework;
using PTV.Framework.Interfaces;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Caches.OrganizationTreeData
{
    public class GetMainOrganizationIdsTests : TestBase
    {
        private Guid TopOrgA = "TopOrgA".GetGuid();
        private Guid MidOrgA = "MidOrgA".GetGuid();
        private Guid BotOrgA = "BotOrgA".GetGuid();
        private Guid TopOrgB = "TopOrgB".GetGuid();
        private Guid MidOrgB = "MidOrgB".GetGuid();
        private Guid BotOrgB = "BotOrgB".GetGuid();
        private Guid TopOrgC = "TopOrgC".GetGuid();
        private Guid MidOrgC = "MidOrgC".GetGuid();
        private Guid BotOrgC = "BotOrgC".GetGuid();

        private Guid SahaA = "SahaA".GetGuid();
        private Guid SahaB = "SahaB".GetGuid();

        [Fact]
        public void FindForChildA()
        {
            var typeCacheMock = SetupTypesCache();
            RegisterRepositories();
            var translationEntityMock = SetupTranslators();
            var cache = new OrganizationTreeDataCache(ResolveManager, translationEntityMock.Object, typeCacheMock.Object);

            var (organizationId, sahaId) = cache.GetMainOrganizationIds(BotOrgA);

            Assert.Equal(TopOrgA, organizationId);
            Assert.Equal(SahaA, sahaId);
        }

        [Fact]
        public void FindForTopB()
        {
            var typeCacheMock = SetupTypesCache();
            RegisterRepositories();
            var translationEntityMock = SetupTranslators();
            var cache = new OrganizationTreeDataCache(ResolveManager, translationEntityMock.Object, typeCacheMock.Object);

            var (organizationId, sahaId) = cache.GetMainOrganizationIds(TopOrgB);

            Assert.Equal(TopOrgB, organizationId);
            Assert.Equal(SahaB, sahaId);
        }
        
        [Fact]
        public void FindForMidC()
        {
            var typeCacheMock = SetupTypesCache();
            RegisterRepositories();
            var translationEntityMock = SetupTranslators();
            var cache = new OrganizationTreeDataCache(ResolveManager, translationEntityMock.Object, typeCacheMock.Object);

            var (organizationId, sahaId) = cache.GetMainOrganizationIds(MidOrgC);

            Assert.Equal(TopOrgC, organizationId);
            Assert.Null(sahaId);
        }

        private Mock<ITranslationEntity> SetupTranslators()
        {
            CacheManagerMock.Setup(x => x.LanguageCache).Returns(new Mock<IInternalLanguageCache>().Object);
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo
                {RegisterAs = typeof(IContextManager), Instance = contextManagerMock.Object});
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo
                {RegisterAs = typeof(IVersioningManager), Instance = new Mock<IVersioningManager>().Object});
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo
                {RegisterAs = typeof(ModelUtility), Instance = new Mock<ModelUtility>().Object});
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo
                {RegisterAs = typeof(ICacheManager), Instance = CacheManager});
            
            var translationPrimitives = new TranslationPrimitives(ResolveManager);

            ResolveManager.RegisterInstance(new TestRegisterServiceInfo
            {
                RegisterAs = typeof(ITranslator<OrganizationVersioned, VmListItem>),
                Instance = new OrganizationSimpleListTranslator(ResolveManager, translationPrimitives, CacheManager)
            });
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo
            {
                RegisterAs = typeof(ITranslator<INameReferences, IVmTranslationItem>),
                Instance = new NameReferenceTranslationTranslator(ResolveManager, translationPrimitives, CacheManager)
            });
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo
            {
                RegisterAs = typeof(ITranslator<IName, String>),
                Instance = new NameTranslator(ResolveManager, translationPrimitives)
            });


            var translationEntityMock = new Mock<ITranslationEntity>();
            translationEntityMock.Setup(x =>
                x.TranslateAll<OrganizationVersioned, VmListItemWithStatus>(
                    It.IsAny<ICollection<OrganizationVersioned>>())).Returns(
                (IEnumerable<OrganizationVersioned> orgs) =>
                {
                    var translator = new OrganizationSimpleListStatusTranslator(ResolveManager, translationPrimitives);
                    var result = new List<VmListItemWithStatus>();
                    foreach (var org in orgs)
                    {
                        var item = translator.TranslateEntityToVm(org);
                        result.Add(item);
                    }

                    return result;
                });
            return translationEntityMock;
        }

        private static Mock<ITypesCache> SetupTypesCache()
        {
            var publishingStatuses = GetPublishingStatuses();
            var typeCacheMock = new Mock<ITypesCache>();
            typeCacheMock.Setup(x => x.GetCacheData<PublishingStatusType>()).Returns(publishingStatuses);
            return typeCacheMock;
        }

        private void RegisterRepositories()
        {
            var toa = new OrganizationVersioned
            {
                Id = TopOrgA,
                UnificRootId = TopOrgA,
                ParentId = null,
                Parent = null,
                PublishingStatusId = PublishingStatus.Published.ToString().GetGuid(),
                UnificRoot = new Organization
                {
                    Id = TopOrgA
                }
            };
            var moa = new OrganizationVersioned
            {
                Id = MidOrgA,
                UnificRootId = MidOrgA,
                ParentId = TopOrgA,
                Parent = toa.UnificRoot,
                PublishingStatusId = PublishingStatus.Published.ToString().GetGuid(),
                UnificRoot = new Organization
                {
                    Id = MidOrgA
                }
            };
            var boa = new OrganizationVersioned
            {
                Id = BotOrgA,
                UnificRootId = BotOrgA,
                ParentId = MidOrgA,
                Parent = moa.UnificRoot,
                PublishingStatusId = PublishingStatus.Published.ToString().GetGuid(),
                UnificRoot = new Organization
                {
                    Id = BotOrgA
                }
            };
            var tob = new OrganizationVersioned
            {
                Id = TopOrgB,
                UnificRootId = TopOrgB,
                ParentId = null,
                Parent = null,
                PublishingStatusId = PublishingStatus.Published.ToString().GetGuid(),
                UnificRoot = new Organization
                {
                    Id = TopOrgB
                }
            };
            var mob = new OrganizationVersioned
            {
                Id = MidOrgB,
                UnificRootId = MidOrgB,
                ParentId = TopOrgB,
                Parent = tob.UnificRoot,
                PublishingStatusId = PublishingStatus.Published.ToString().GetGuid(),
                UnificRoot = new Organization
                {
                    Id = MidOrgB
                }
            };
            var bob = new OrganizationVersioned
            {
                Id = BotOrgB,
                UnificRootId = BotOrgB,
                ParentId = MidOrgB,
                Parent = mob.UnificRoot,
                PublishingStatusId = PublishingStatus.Published.ToString().GetGuid(),
                UnificRoot = new Organization
                {
                    Id = BotOrgB
                }
            };
            var toc = new OrganizationVersioned
            {
                Id = TopOrgC,
                UnificRootId = TopOrgC,
                ParentId = null,
                Parent = null,
                PublishingStatusId = PublishingStatus.Published.ToString().GetGuid(),
                UnificRoot = new Organization
                {
                    Id = TopOrgC
                }
            };
            var moc = new OrganizationVersioned
            {
                Id = MidOrgC,
                UnificRootId = MidOrgC,
                ParentId = TopOrgC,
                Parent = toc.UnificRoot,
                PublishingStatusId = PublishingStatus.Published.ToString().GetGuid(),
                UnificRoot = new Organization
                {
                    Id = MidOrgC
                }
            };
            var boc = new OrganizationVersioned
            {
                Id = BotOrgC,
                UnificRootId = BotOrgC,
                ParentId = MidOrgC,
                Parent = moc.UnificRoot,
                LastOperationTimeStamp = new DateTime(2019, 9, 19),
                PublishingStatusId = PublishingStatus.Published.ToString().GetGuid(),
                UnificRoot = new Organization
                {
                    Id = BotOrgC
                }
            };

            var sa = new SahaOrganizationInformation
            {
                OrganizationId = TopOrgA,
                Organization = toa.UnificRoot,
                SahaId = SahaA,
                SahaParentId = SahaA
            };
            var sb = new SahaOrganizationInformation
            {
                OrganizationId = TopOrgB,
                Organization = tob.UnificRoot,
                SahaId = SahaB,
                SahaParentId = SahaB
            };

            toa.UnificRoot.Children = new List<OrganizationVersioned> {moa};
            toa.UnificRoot.Versions = new List<OrganizationVersioned> {toa};
            toa.UnificRoot.SahaOrganizationInformations = new List<SahaOrganizationInformation> {sa};
            moa.UnificRoot.Children = new List<OrganizationVersioned> {boa};
            moa.UnificRoot.Versions = new List<OrganizationVersioned> {moa};
            boa.UnificRoot.Versions = new List<OrganizationVersioned> {boa};
            tob.UnificRoot.Children = new List<OrganizationVersioned> {mob};
            tob.UnificRoot.Versions = new List<OrganizationVersioned> {tob};
            tob.UnificRoot.SahaOrganizationInformations = new List<SahaOrganizationInformation> {sb};
            mob.UnificRoot.Children = new List<OrganizationVersioned> {bob};
            mob.UnificRoot.Versions = new List<OrganizationVersioned> {mob};
            bob.UnificRoot.Versions = new List<OrganizationVersioned> {bob};
            toc.UnificRoot.Children = new List<OrganizationVersioned> {moc};
            toc.UnificRoot.Versions = new List<OrganizationVersioned> {toc};
            moc.UnificRoot.Children = new List<OrganizationVersioned> {boc};
            moc.UnificRoot.Versions = new List<OrganizationVersioned> {moc};
            boc.UnificRoot.Versions = new List<OrganizationVersioned> {boc};

            var organizations = new List<OrganizationVersioned>
            {
                toa, moa, boa, tob, mob, bob, toc, moc, boc
            };
            var sahaInfos = new List<SahaOrganizationInformation>
            {
                sa, sb
            };

            RegisterRepository<IOrganizationVersionedRepository, OrganizationVersioned>(organizations.AsQueryable());
            RegisterRepository<ISahaOrganizationInformationRepository, SahaOrganizationInformation>(sahaInfos.AsQueryable());
            SetupContextManager<OrganizationTreeItem, List<OrganizationTreeItem>>();
        }

        private static List<VmType> GetPublishingStatuses()
        {
            return new List<VmType>
            {
                new VmType{ Code = PublishingStatus.Deleted.ToString(), Id = PublishingStatus.Deleted.ToString().GetGuid() },
                new VmType{ Code = PublishingStatus.Draft.ToString(), Id = PublishingStatus.Draft.ToString().GetGuid() },
                new VmType{ Code = PublishingStatus.Modified.ToString(), Id = PublishingStatus.Modified.ToString().GetGuid() },
                new VmType{ Code = PublishingStatus.Published.ToString(), Id = PublishingStatus.Published.ToString().GetGuid() },
                new VmType{ Code = PublishingStatus.Removed.ToString(), Id = PublishingStatus.Removed.ToString().GetGuid() },
                new VmType{ Code = PublishingStatus.OldPublished.ToString(), Id = PublishingStatus.OldPublished.ToString().GetGuid() },
            };
        }
    }
}
