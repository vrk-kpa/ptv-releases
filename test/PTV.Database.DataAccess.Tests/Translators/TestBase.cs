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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Cloning;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.Views;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.TextManager;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public static class MoqExtensions
    {
        public static ISetup<T, TResult> SetupIgnoreArgs<T, TResult>(this Mock<T> mock,
            Expression<Func<T, TResult>> expression)
            where T : class
        {
            expression = new MakeAnyVisitor().VisitAndConvert(
                expression, "SetupIgnoreArgs");

            return mock.Setup(expression);
        }

        public static ISetup<T> SetupIgnoreArgs<T>(this Mock<T> mock,
            Expression<Action<T>> expression)
            where T : class
        {
            expression = new MakeAnyVisitor().VisitAndConvert(
                expression, "SetupIgnoreArgs");

            return mock.Setup(expression);
        }

        public static void VerifyIgnoreArgs<T>(this Mock<T> mock,
            Expression<Action<T>> expression, Func<Times> times = null)
            where T : class
        {
            expression = new MakeAnyVisitor().VisitAndConvert(
                expression, "VerifyIgnoreArgs");

            mock.Verify(expression, times ?? Times.AtLeastOnce);
        }

        public static void VerifyIgnoreArgs<T, TResult>(this Mock<T> mock,
            Expression<Func<T, TResult>> expression, Func<Times> times = null)
            where T : class
        {
            expression = new MakeAnyVisitor().VisitAndConvert(
                expression, "VerifyIgnoreArgs");

            mock.Verify(expression, times ?? Times.AtLeastOnce);
        }

        private class MakeAnyVisitor : ExpressionVisitor
        {
            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (node.Value != null)
                    return base.VisitConstant(node);

                var method = typeof(It)
                    .GetMethod("IsAny")
                    .MakeGenericMethod(node.Type);

                return Expression.Call(method);
            }
        }
    }
    
    public abstract class TestBase
    {
        internal Mock<ICacheManager> CacheManagerMock { get; private set; }
        internal Mock<IPerformanceMonitorManager> PerformanceMonitorManagerMock { get; private set; }
        internal ICacheManager CacheManager => CacheManagerMock.Object;
        internal TestLanguageCache LanguageCache { get; private set; }
        
        internal IInternalLanguageCache InternalLanguageCache { get; private set; }
        internal ITypesCache TypeCache { get; private set; }
        internal IPublishingStatusCache PublishingStatusCache { get; private set; }

        private Mock<ITypesCache> typesCacheMock;

        protected IResolveManagerTest ResolveManager { get; private set; }
        protected ITextManager TextManager { get; private set; }
        protected IPerformanceMonitorManager PerformanceMonitorManager { get; private set; }
        
        protected Mock<IUnitOfWorkWritable> unitOfWorkMockSetup;
        private Mock<IEntityTreesCache> entityTreeCacheMock;
        protected Mock<IContextManager> contextManagerMock;
        private Mock<ILanguageOrderCache> languageCacheOrder;
        
        protected TestBase()
        {
            ResolveManager = new ResolveManagerTest();
           
            unitOfWorkMockSetup = new Mock<IUnitOfWorkWritable>(MockBehavior.Loose);
            var cloneCache = new TranslationCloneCache(null);
            unitOfWorkMockSetup.SetupAllProperties();
            unitOfWorkMockSetup.Setup(uw => uw.TranslationCloneCache).Returns(cloneCache);
            unitOfWorkMockSetup.Setup(uw => uw.DetachEntity(It.IsAny<object>()));
            unitOfWorkMockSetup.Setup(uw => uw.Save(SaveMode.Normal, PreSaveAction.Standard, null, null));
            SetupCacheManager();
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo() { RegisterAs = typeof(ILanguageCache), Instance = LanguageCache });
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo() { RegisterAs = typeof(IInternalLanguageCache), Instance = InternalLanguageCache });
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo() { RegisterAs = typeof(ITextManager), Instance = new TextManager(new LoggerFactory()) });
            TypeCache = CacheManager.TypesCache;
            PerformanceMonitorManagerMock = new Mock<IPerformanceMonitorManager>();
            PerformanceMonitorManagerMock.Setup(i => i.StartMeasuring()).Returns(Guid.NewGuid);
            PerformanceMonitorManagerMock.Setup(i => i.StopMeasuring(It.IsAny<Guid>()));
            PerformanceMonitorManager = PerformanceMonitorManagerMock.Object;
            contextManagerMock = new Mock<IContextManager>(MockBehavior.Strict);
            contextManagerMock.Setup(x => x.ExecuteReader(It.IsAny<Action<IUnitOfWork>>()))
                .Callback((Action<IUnitOfWork> action) => action(unitOfWorkMockSetup.Object));
        }
        
        internal void SetupContextManager<TWriterOut, TReadOut>()
        {
            contextManagerMock.Setup(x => x.ExecuteReader(It.IsAny<Func<IUnitOfWork, TReadOut>>()))
                .Returns((Func<IUnitOfWork, TReadOut> func) => func(unitOfWorkMockSetup.Object));
            contextManagerMock.Setup(x => x.ExecuteWriter(It.IsAny<Func<IUnitOfWorkWritable, TWriterOut>>()))
                .Returns((Func<IUnitOfWorkWritable, TWriterOut> func) => func(unitOfWorkMockSetup.Object));
        }
        
        internal void VerifyContextManagerCalls<TWriterOut, TReadOut>(Times reader, Times writer)
        {
            contextManagerMock.Verify(
                x => x.ExecuteReader(It.IsAny<Func<IUnitOfWork, TReadOut>>()),
                reader
            );
                
            contextManagerMock.Verify(
                x => x.ExecuteWriter(It.IsAny<Func<IUnitOfWorkWritable, TWriterOut>>()),
                writer
            );
        }

        internal Mock<T> RegisterViewRepository<T, TEntity>(IQueryable<TEntity> list)
            where T : class, IVRepository<TEntity> where TEntity : class
        {
            var repMock = new Mock<T>();
            unitOfWorkMockSetup.Setup(x => x.CreateRepository<T>())
                .Returns(repMock.Object);
            repMock.Setup(x => x.All()).Returns(list);
            
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<TEntity>>(),
                It.IsAny<Func<IQueryable<TEntity>, IQueryable<TEntity>>>(),
                It.IsAny<bool>()
            )).Returns((IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TEntity>> func, bool applyFilters) => query);
            return repMock;
        }
        
        internal Mock<T> RegisterRepository<T, TEntity>(IQueryable<TEntity> list) 
            where T : class, IRepository<TEntity> where TEntity : class 
        {
            var repMock = new Mock<T>();
            unitOfWorkMockSetup.Setup(x => x.CreateRepository<T>())
                .Returns(repMock.Object);
            repMock.Setup(x => x.All()).Returns(list);
            
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<TEntity>>(),
                It.IsAny<Func<IQueryable<TEntity>, IQueryable<TEntity>>>(),
                It.IsAny<bool>()
            )).Returns((IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TEntity>> func, bool applyFilters) => query);
            return repMock;
        }
        
        internal Mock<T> RegisterVRepository<T, TEntity>(IQueryable<TEntity> list) where T : class, IVRepository<TEntity> where TEntity : class 
        {
            var repMock = new Mock<T>();
            unitOfWorkMockSetup.Setup(x => x.CreateRepository<T>())
                .Returns(repMock.Object);
            repMock.Setup(x => x.All()).Returns(list);
            
            unitOfWorkMockSetup.Setup(uw => uw.ApplyIncludes(
                It.IsAny<IQueryable<TEntity>>(),
                It.IsAny<Func<IQueryable<TEntity>, IQueryable<TEntity>>>(),
                It.IsAny<bool>()
            )).Returns((IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TEntity>> func, bool applyFilters) => query);
            return repMock;
        }
        
        internal TestRegisterServiceInfo CreateRegisterServiceInfo<T>(T instance)
        {
            return new TestRegisterServiceInfo { Instance = instance, RegisterAs = typeof(T)};
        }

        internal void RegisterDbSet<T,T2>(List<T> entityData, Mock<T2> unitOfWork) where T : class where T2 : class, IUnitOfWork
        {
            var dbset = new TestDbSet<T>(entityData);
            unitOfWork.Setup(uw => uw.GetSet<T>()).Returns(dbset);
//            unitOfWork.Setup(uw => uw.GetFromCachedSet<T>()).Returns(entityData);
        }

        internal List<T> CreateCodeData<T>(Type enumType) where T : ICode, new()
        {
            return Enum.GetNames(enumType).Select(type => new T { Code = type }).ToList();
        }

        internal Dictionary<Guid, string> CreateCodeGuidList(Type enumType)
        {
            return Enum.GetNames(enumType).ToDictionary(type => type.ToString().GetGuid(), type => type.ToString());
        }
        
        internal Dictionary<Guid, string> CreateLanguageCodeGuidList()
        {
            string[] codes = {"fi", "sv", "en", "se", "smn", "sms"};
            return codes.ToDictionary(type => type.ToString().GetGuid(), type => type.ToString());
        }
//        internal Mock<ILanguageCache> SetupLanguageCacheMock(Mock<ILanguageCache> languageCacheMock = null)
//        {
//            languageCacheMock = languageCacheMock ?? new Mock<ILanguageCache>();
//            var languages = CreateCodeGuidList(typeof (LanguageCode));
//            languageCacheMock.Setup(i => i.Filter(It.IsAny<Guid>(), It.IsAny<LanguageCode>())).Returns(true);
//            languageCacheMock.Setup(i => i.Filter(It.IsAny<List<ILocalizable>>(), It.IsAny<LanguageCode>())).Returns((List<ILocalizable> i) => i.FirstOrDefault());
//            languageCacheMock.Setup(i => i.Get(It.IsAny<string>())).Returns((string id) => id.GetGuid());
//            languageCacheMock.Setup(i => i.GetByValue(It.IsAny<Guid>())).Returns((Guid id) => languages.TryGet(id));
//            return languageCacheMock;
//        }
        internal Mock<ITypesCache> SetupTypesCacheMock<T>(Type typesEnum = null) where T : IType, IEntityIdentifier
        {
//            typesCacheMock = typesCacheMock ?? new Mock<ITypesCache>();

            var types = new Dictionary<Guid, string>();
            if (typesEnum != null)
            {
                types = CreateCodeGuidList(typesEnum);
            }
            typesCacheMock.Setup(i => i.Get<T>(It.IsAny<string>())).Returns((string id) => string.IsNullOrEmpty(id) ? Guid.Empty : id.GetGuid());
            typesCacheMock.Setup(i => i.GetByValue<T>(It.IsAny<Guid>())).Returns((Guid id) => (typesEnum != null) ? types.TryGet(id) : id.ToString());
            typesCacheMock.Setup(i => i.Compare<T>(It.IsAny<Guid>(), It.IsAny<string>())).Returns((Guid id, string code) => id == code.GetGuid());
            return typesCacheMock;
        }        
        
        internal Mock<IEntityTreesCache> SetupEntityTreesCacheMock<T>() where T : IType
        {
            entityTreeCacheMock.Setup(i => i.Get<T>(It.IsAny<string>())).Returns((string id) => string.IsNullOrEmpty(id) ? Guid.Empty : id.GetGuid());
            return entityTreeCacheMock;
        }

        private Mock<ICacheManager> SetupCacheManager()
        {
            LanguageCache = new TestLanguageCache(CreateLanguageCodeGuidList());
            PublishingStatusCache = new TestPublishingStatusCache(CreateCodeGuidList(typeof(PublishingStatus)));
            typesCacheMock = new Mock<ITypesCache>();
            entityTreeCacheMock = new Mock<IEntityTreesCache>();
            languageCacheOrder = new Mock<ILanguageOrderCache>();
            
            CacheManagerMock = CacheManagerMock ?? new Mock<ICacheManager>();
            CacheManagerMock.Setup(i => i.LanguageCache).Returns(LanguageCache);
            CacheManagerMock.Setup(i => i.PublishingStatusCache).Returns(PublishingStatusCache);
            CacheManagerMock.Setup(i => i.EntityTreesCache).Returns(entityTreeCacheMock.Object);
            CacheManagerMock.Setup(i => i.TypesCache).Returns(typesCacheMock.Object);
            CacheManagerMock.Setup(i => i.LanguageOrderCache).Returns(languageCacheOrder.Object);
            return CacheManagerMock;
        }

        protected Mock<T> RegisterServiceMock<T>(Action<Mock<T>> setup = null) where T : class 
        {
            var mock = new Mock<T>();
            ResolveManager.RegisterInstance(CreateRegisterServiceInfo(mock.Object));
            setup?.Invoke(mock);
            return mock;
        }

        protected void SetTasksConfiguration(VTasksConfiguration instance, string propertyName, object propertyValue)
        {
            var type = typeof(VTasksConfiguration);
            var property = type.GetProperty(propertyName);
            property.SetValue(instance, propertyValue);
        }
    }
}
