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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Xunit;
using PTV.Framework.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.TextManager;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public abstract class TranslatorTestBase : TestBase
    {
        private Mock<IVersioningManager> versioningManagerMock;
        protected ITranslationPrimitives TranslationPrimitives { get; private set; }
        protected ILoggerFactory LoggerFactory { get; private set; }

        protected ITranslationEntity TranslationManagerEntities { get; }

        protected ITranslationViewModel TranslationManagerVModels { get; }

        internal TranslatedInstanceStorage TranslatedInstanceStorage { get; private set; }

        protected TranslatorTestBase()
        {
            TranslationPrimitives = new TranslationPrimitives(ResolveManager);
            var prefilteringManager = new PrefilteringManagerTest();
            var translationManager = new TranslationManager(ResolveManager, prefilteringManager);
            LoggerFactory = new LoggerFactory();
            TranslationManagerEntities = translationManager;
            TranslationManagerVModels = translationManager;

//            var languageCacheMock = new Mock<ILanguageCache>();
//            languageCacheMock.Setup(i => i.Filter(It.IsAny<Guid>(), It.IsAny<LanguageCode>())).Returns(true);
//            languageCacheMock.Setup(i => i.Filter(It.IsAny<List<ILocalizable>>(), It.IsAny<LanguageCode>())).Returns((List<ILocalizable> i) => i.FirstOrDefault());
//            languageCacheMock.Setup(i => i.Get(It.IsAny<string>())).Returns("test".GetGuid());
//            var languageCache = languageCacheMock.Object;

            ResolveManager.RegisterInstance(new TestRegisterServiceInfo(){ RegisterAs = typeof(ICacheManager), Instance = CacheManager });
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo() { RegisterAs = typeof(IPrefilteringManager), Instance = prefilteringManager });
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo() { RegisterAs = typeof(ITextManager), Instance = new TextManager(LoggerFactory) });


            TranslatedInstanceStorage = new TranslatedInstanceStorage();
            ResolveManager.RegisterInstance(TranslatedInstanceStorage);
            versioningManagerMock = new Mock<IVersioningManager>();
            versioningManagerMock.Setup(i => i.ApplyPublishingStatusFilter(It.IsAny<IQueryable<IVersionedVolume>>())).Returns((IQueryable<IVersionedVolume> query) => query);
            versioningManagerMock.Setup(i => i.FilterByPublishingStatus(It.IsAny<IVersionedVolume>())).Returns((IVersionedVolume query) => true);

            ResolveManager.RegisterInstance(CreateRegisterServiceInfo(versioningManagerMock.Object));
        }

        internal void RegisterEntityForVersionManager<TEntity>() where TEntity : class, IVersionedVolume, new ()
        {
            versioningManagerMock.Setup(i => i.CreateModifiedVersion(It.IsAny<ITranslationUnitOfWork>(), It.IsAny<TEntity>(), It.IsAny<VersioningMode>())).Returns((ITranslationUnitOfWork uow, TEntity entity, VersioningMode versioningMode) => entity);
        }

        internal IReadOnlyCollection<TModel> RunTranslationEntityToModelTest<TEntity, TModel>(IReadOnlyCollection<object> translators, IReadOnlyCollection<TEntity> toTranslate) where TEntity : class where TModel : class
        {
            ResolveManager.RegisterInstances(translators);
            var translations = TranslationManagerEntities.TranslateAll<TEntity, TModel>(toTranslate.AsQueryable());
            translations.Count.Should().Be(toTranslate.Count);
            return translations;
        }

        internal IReadOnlyCollection<TEntity> RunTranslationModelToEntityTest<TModel, TEntity>(IReadOnlyCollection<object> translators, IReadOnlyCollection<TModel> toTranslate, IUnitOfWork unitOfWork=null) where TEntity : class where TModel : class
        {
            ResolveManager.RegisterInstances(translators);
            var translations = TranslationManagerVModels.TranslateAll<TModel, TEntity>(toTranslate, unitOfWork);
            translations.Count.Should().Be(toTranslate.Count);
            return translations;
        }

        internal TestRegisterServiceInfo RegisterTranslatorMock<TSource, TTarget>(Mock<ITranslator<TSource, TTarget>> translator, IUnitOfWork unitOfWork, Func<TSource,TTarget> toViewModelFunc = null ) where TSource : new()
        {
//            translator.Setup(x => x.UnitOfWork).Returns(unitOfWork);
////            if (toEntityFunc != null)
////            {
////                translator.Setup(tr => tr.TranslateVmToEntity(It.IsAny<TTarget>())).Returns(toEntityFunc);
////            }
////            else
////            {
//                translator.Setup(tr => tr.TranslateVmToEntity(It.IsAny<TTarget>())).Returns(new TSource());
////            }
//            if (toViewModelFunc != null)
//            {
//                translator.Setup(tr => tr.TranslateEntityToVm(It.IsAny<TSource>())).Returns(toViewModelFunc);
//            }
//            else
//            {
//                translator.Setup(tr => tr.TranslateEntityToVm(It.IsAny<TSource>())).Returns(null);
//            }
//            { }
////            }
//            return CreateRegisterServiceInfo(translator.Object);
            return RegisterTranslatorMock(translator, unitOfWork, (target) => new TSource(), toViewModelFunc);
        }

        internal TestRegisterServiceInfo RegisterTranslatorMock<TSource, TTarget>(Mock<ITranslator<TSource, TTarget>> translator, IUnitOfWork unitOfWork, Func<TTarget, TSource> toEntityFunc, Func<TSource,TTarget> toViewModelFunc = null, Action<TSource> setTargetAction = null   )
        {
            translator.Setup(x => x.UnitOfWork).Returns(unitOfWork);
            if (setTargetAction != null)
            {
                translator.Setup(x => x.SetTargetInstance(It.IsAny<TSource>())).Callback(setTargetAction);
            }
            if (toEntityFunc != null)
            {
                translator.Setup(tr => tr.TranslateVmToEntity(It.IsAny<TTarget>())).Returns(toEntityFunc);
            }
            else
            {
                translator.Setup(tr => tr.TranslateVmToEntity(It.IsAny<TTarget>())).Returns(null);
            }
            if (toViewModelFunc != null)
            {
                translator.Setup(tr => tr.TranslateEntityToVm(It.IsAny<TSource>())).Returns(toViewModelFunc);
            }
            else
            {
                translator.Setup(tr => tr.TranslateEntityToVm(It.IsAny<TSource>())).Returns(null);
            }
            return CreateRegisterServiceInfo(translator.Object);
        }
    }

    internal class TestPublishingStatusCache : IPublishingStatusCache
    {
        private readonly Dictionary<Guid, string> publishingStatuses;

        public TestPublishingStatusCache(Dictionary<Guid, string> publishingStatuses)
        {
            this.publishingStatuses = publishingStatuses;
        }

        public Guid Get(PublishingStatus publishingStatus)
        {
            return Get(publishingStatus.ToString());
        }

        public Guid Get(string key)
        {
            return key.GetGuid();
        }

        public string GetByValue(Guid value)
        {
            return publishingStatuses.TryGet(value);
        }
    }

    internal class TestLanguageCache : ILanguageCache
    {
        private Dictionary<Guid, string> languages;

        public TestLanguageCache(Dictionary<Guid, string> languages)
        {
            this.languages = languages;
        }

        public Guid Get(string key)
        {
            return key.GetGuid();
        }

        public string GetByValue(Guid value)
        {
            return languages.TryGet(value);
        }

        public bool Filter(Guid languageId, LanguageCode filterBy)
        {
            return true; //languageId == filterBy.ToString().GetGuid();
        }

        public T Filter<T>(IEnumerable<T> filterable, LanguageCode filterBy) where T : class, ILocalizable
        {
            return filterable.FirstOrDefault();
        }

        public IEnumerable<T> FilterCollection<T>(IEnumerable<T> filterable, LanguageCode filterBy) where T : class, ILocalizable
        {
            return filterable;
        }

        public Guid Get(LanguageCode key)
        {
            return Get(key.ToString());
        }

        public bool Filter(Guid languageId, Guid filterBy)
        {
            return languageId == filterBy;
        }

        public T Filter<T>(IEnumerable<T> filterable, Guid filterBy) where T : class, ILocalizable
        {
            return filterable.FirstOrDefault();
        }

        public IEnumerable<T> FilterCollection<T>(IEnumerable<T> filterable, Guid filterBy) where T : class, ILocalizable
        {
            return filterable;
        }

        public LanguageCode GetByValueEnum(Guid key)
        {
            return DomainConstants.DefaultLanguage;
        }
    }
}
