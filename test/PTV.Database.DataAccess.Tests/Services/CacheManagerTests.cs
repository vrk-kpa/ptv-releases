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
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using System;
using System.Collections.Generic;
using System.Text;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Logic;
using PTV.Framework.Interfaces;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services
{
    /// <summary>
    /// Contains PTV CacheManager implementation tests.
    /// </summary>
    /// <remarks>
    /// <para>IClassFixture is used to share the mock and created CacheManager between tests.</para>
    /// <para>Tests test that when we create CacheManager instance the properties are set in CacheManager and the instances are the same as passed in constructor.</para>
    /// </remarks>
    public class CacheManagerTests : IClassFixture<CacheManagerFixture>
    {
        private CacheManagerFixture _cacheManagerFixture;

        public CacheManagerTests(CacheManagerFixture fixture)
        {
            _cacheManagerFixture = fixture;
        }

        public CacheManagerFixture TestFixture { get => _cacheManagerFixture; }

        [Fact]
        public void CacheManagerIsDecoratedWithRegisterService()
        {
            typeof(CacheManager).Should().BeDecoratedWith<Framework.RegisterServiceAttribute>();
        }

        [Fact]
        public void CacheManagerImplementsICacheManager()
        {
            TestFixture.CacheManager.Should().BeAssignableTo<ICacheManager>();
        }

        [Fact]
        public void CacheManagerLanguageCacheIsNotNull()
        {
            TestFixture.CacheManager.LanguageCache.Should().NotBeNull();
        }

        [Fact]
        public void CacheManagerLanguageCacheIsSame()
        {
            TestFixture.CacheManager.LanguageCache.Should().BeSameAs(TestFixture.LanguageCache.Object);
        }

        [Fact]
        public void CacheManagerPublishingStatusCacheIsNotNull()
        {
            TestFixture.CacheManager.PublishingStatusCache.Should().NotBeNull();
        }

        [Fact]
        public void CacheManagerPublishingStatusCacheIsSame()
        {
            TestFixture.CacheManager.PublishingStatusCache.Should().BeSameAs(TestFixture.PublishingStatusCache.Object);
        }

        [Fact]
        public void CacheManagerTypesCacheIsNotNull()
        {
            TestFixture.CacheManager.TypesCache.Should().NotBeNull();
        }

        [Fact]
        public void CacheManagerTypesCacheIsSame()
        {
            TestFixture.CacheManager.TypesCache.Should().BeSameAs(TestFixture.TypesCache.Object);
        }

        [Fact]
        public void CacheManagerLanguageOrderCacheIsNotNull()
        {
            TestFixture.CacheManager.LanguageOrderCache.Should().NotBeNull();
        }

        [Fact]
        public void CacheManagerLanguageOrderCacheIsSame()
        {
            TestFixture.CacheManager.LanguageOrderCache.Should().BeSameAs(TestFixture.LanguageOrderCache.Object);
        }
    }

    public class CacheManagerFixture
    {
        // create the mock and cachemanager only once for all tests in CacheManagerTests
        // xunit creates this fixture class once and then uses it for each test

        private readonly Mock<ILanguageCache> languageCache;
        private readonly Mock<IPublishingStatusCache> publishingStatusCache;
        private readonly Mock<ITypesCache> typesCache;
        private readonly Mock<IEntityTreesCache> treesCache;
        private readonly Mock<IPostalCodeCache> postalCodeCache;
        private readonly Mock<ILanguageOrderCache> languageOrderCache;
        private readonly Mock<IJobStatusCache> jobStatusCache;
        private readonly Mock<IJobExecutionCache> jobExecutionCache;

        private readonly CacheManager cacheManager;
        private readonly Mock<IUserAccessRightsCache> userAccessRightCache;
        private readonly Mock<ILanguageStateCultureCache> langugeStateCultureCache;
        private readonly ResolveManagerTest resolveManager;
        private readonly Mock<IContextManager> contextManager;


        public CacheManagerFixture()
        {
            languageCache = new Mock<ILanguageCache>();
            publishingStatusCache = new Mock<IPublishingStatusCache>();
            typesCache = new Mock<ITypesCache>();
            languageOrderCache = new Mock<ILanguageOrderCache>();
            userAccessRightCache = new Mock<IUserAccessRightsCache>();
            contextManager = new Mock<IContextManager>();
            resolveManager = new ResolveManagerTest();
            jobExecutionCache = new Mock<IJobExecutionCache>();
            jobStatusCache = new Mock<IJobStatusCache>();
            

            resolveManager.RegisterInstance(new TestRegisterServiceInfo { Instance = contextManager.Object, RegisterAs = typeof(IContextManager)});
            langugeStateCultureCache = new Mock<ILanguageStateCultureCache>();
            treesCache = new Mock<IEntityTreesCache>();
            postalCodeCache = new Mock<IPostalCodeCache>();
            cacheManager = new CacheManager(
                languageCache.Object,
                publishingStatusCache.Object,
                typesCache.Object,
                languageOrderCache.Object,
                userAccessRightCache.Object,
                langugeStateCultureCache.Object,
                resolveManager,
                treesCache.Object,
                postalCodeCache.Object,
                jobExecutionCache.Object,
                jobStatusCache.Object
            );
        }

        internal Mock<IPublishingStatusCache> PublishingStatusCache { get => publishingStatusCache; }
        internal Mock<ILanguageCache> LanguageCache { get => languageCache; }
        internal Mock<ITypesCache> TypesCache { get => typesCache; }
        internal Mock<ILanguageOrderCache> LanguageOrderCache { get => languageOrderCache; }
        internal Mock<ILanguageStateCultureCache> LangugeStateCultureCache { get => langugeStateCultureCache; }
        internal CacheManager CacheManager { get => cacheManager; }
    }
}
