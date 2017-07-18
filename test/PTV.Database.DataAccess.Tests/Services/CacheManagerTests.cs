using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Caches;
using System;
using System.Collections.Generic;
using System.Text;
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

        private Mock<ILanguageCache> _languageCache;
        private Mock<IPublishingStatusCache> _publishingStatusCache;
        private Mock<ITypesCache> _typesCache;
        private Mock<ILanguageOrderCache> _languageOrderCache;

        private CacheManager _cacheManager;

        public CacheManagerFixture()
        {
            _languageCache = new Mock<ILanguageCache>();
            _publishingStatusCache = new Mock<IPublishingStatusCache>();
            _typesCache = new Mock<ITypesCache>();
            _languageOrderCache = new Mock<ILanguageOrderCache>();

            _cacheManager = new CacheManager(_languageCache.Object, _publishingStatusCache.Object, _typesCache.Object, _languageOrderCache.Object);
        }

        internal Mock<IPublishingStatusCache> PublishingStatusCache { get => _publishingStatusCache; }
        internal Mock<ILanguageCache> LanguageCache { get => _languageCache; }
        internal Mock<ITypesCache> TypesCache { get => _typesCache; }
        internal Mock<ILanguageOrderCache> LanguageOrderCache { get => _languageOrderCache; }
        internal CacheManager CacheManager { get => _cacheManager; }
    }
}
