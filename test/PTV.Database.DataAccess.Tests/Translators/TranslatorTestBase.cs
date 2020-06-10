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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Interfaces.Caches;
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
using PTV.Database.DataAccess.EntityCloners;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.TextManager;
using PTV.Domain.Logic.Channels;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public abstract class TranslatorTestBase : TestBase
    {
        private Mock<IVersioningManager> versioningManagerMock;
        protected ITranslationPrimitives TranslationPrimitives { get; private set; }

        protected ITranslationEntity TranslationManagerEntities { get; }

        protected ITranslationViewModel TranslationManagerVModels { get; }

        protected TranslatorTestBase()
        {
            TranslationPrimitives = new TranslationPrimitives(ResolveManager);
            var prefilteringManager = new PrefilteringManagerTest();
            var translationManager = new TranslationManager(ResolveManager);
            TranslationManagerEntities = translationManager;
            TranslationManagerVModels = translationManager;

            ResolveManager.RegisterInstance(new TestRegisterServiceInfo { RegisterAs = typeof(ICacheManager), Instance = CacheManager });
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo { RegisterAs = typeof(IPrefilteringManager), Instance = prefilteringManager });
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo { RegisterAs = typeof(ModelUtility), Instance = new ModelUtility() });

            versioningManagerMock = new Mock<IVersioningManager>();

            ResolveManager.RegisterInstance(CreateRegisterServiceInfo(versioningManagerMock.Object));
            ResolveManager.RegisterInstance(CreateRegisterServiceInfo(new Mock<ICloningManager>().Object));
        }

        internal void RegisterEntityForVersionManager<TEntity, TRoot>() where TEntity : class, IVersionedVolume<TRoot>, new () where TRoot : IVersionedRoot, new()
        {
            versioningManagerMock.Setup(i => i.CreateEntityVersion(It.IsAny<ITranslationUnitOfWork>(), It.IsAny<TEntity>(), It.IsAny<VersioningMode>(), It.IsAny<PublishingStatus?>())).Returns((ITranslationUnitOfWork uow, TEntity entity, VersioningMode versioningMode, PublishingStatus? status) => entity);
        }

        internal IReadOnlyCollection<TModel> RunTranslationEntityToModelTest<TEntity, TModel>(IReadOnlyCollection<object> translators, IReadOnlyCollection<TEntity> toTranslate) where TEntity : class where TModel : class
        {
            ResolveManager.RegisterInstances(translators);
            // Workaround to skip the performance monitor
            var translator = ResolveManager.Resolve<ITranslator<TEntity, TModel>>();
            var list = new List<TModel>();
            toTranslate.ForEach(tt => list.Add(translator.TranslateEntityToVm(tt)));
            list.Count.Should().Be(toTranslate.Count);
            return list;
        }

        internal TModel RunTranslationEntityToModelTest<TEntity, TModel>(IReadOnlyCollection<object> translators, TEntity toTranslate) where TEntity : class where TModel : class
        {
            ResolveManager.RegisterInstances(translators);
            // Workaround to skip the performance monitor
            var translator = ResolveManager.Resolve<ITranslator<TEntity, TModel>>();
            var translation = translator.TranslateEntityToVm(toTranslate);
            return translation;
        }

        internal IReadOnlyCollection<TEntity> RunTranslationModelToEntityTest<TModel, TEntity>(IReadOnlyCollection<object> translators, IReadOnlyCollection<TModel> toTranslate, ITranslationUnitOfWork unitOfWork=null) where TEntity : class where TModel : class
        {
            ResolveManager.RegisterInstances(translators);
            // Workaround to skip the performance monitor
            var translator = ResolveManager.Resolve<ITranslator<TEntity, TModel>>();
            translator.UnitOfWork = unitOfWork;
            var list = new List<TEntity>(); // Workaround to skip the performance monitor
            toTranslate.ForEach(tt => list.Add(translator.TranslateVmToEntity(tt)));
            list.Count.Should().Be(toTranslate.Count);
            return list;
        }

        internal TEntity RunTranslationModelToEntityTest<TModel, TEntity>(IReadOnlyCollection<object> translators, TModel toTranslate, ITranslationUnitOfWork unitOfWork=null) where TEntity : class where TModel : class
        {
            ResolveManager.RegisterInstances(translators);
            var translator = ResolveManager.Resolve<ITranslator<TEntity, TModel>>();
            translator.UnitOfWork = unitOfWork;
            var translation = translator.TranslateVmToEntity(toTranslate);
            return translation;
        }

        internal TestRegisterServiceInfo RegisterTranslatorMock<TSource, TTarget>() where TSource : new()
        {
            return RegisterTranslatorMock<TSource, TTarget>(target => new TSource());
        }

        internal TestRegisterServiceInfo RegisterTranslatorMock<TSource, TTarget>
        (
            Func<TTarget, TSource> toEntityFunc,
            Func<TSource, TTarget> toViewModelFunc = null,
            Action<TSource, VersioningMode> setTargetEntityAction = null,
            Action<TTarget, VersioningMode> setTargetViewAction = null
        )
        {
            var translatorMock = new Mock<ITranslator<TSource, TTarget>>();
            return RegisterTranslatorMock(translatorMock, unitOfWorkMockSetup.Object, toEntityFunc, toViewModelFunc, setTargetEntityAction, setTargetViewAction);
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

        internal TestRegisterServiceInfo RegisterTranslatorMock<TSource, TTarget>
        (
            Mock<ITranslator<TSource, TTarget>> translator,
            IUnitOfWork unitOfWork, Func<TTarget, TSource> toEntityFunc,
            Func<TSource,TTarget> toViewModelFunc = null,
            Action<TSource, VersioningMode> setTargetEntityAction = null,
            Action<TTarget, VersioningMode> setTargetViewAction = null
        )
        {
            translator.Setup(x => x.UnitOfWork).Returns(unitOfWork);
            if (setTargetEntityAction != null)
            {
                translator.Setup(x => x.SetTargetInstance(It.IsAny<TSource>(), It.IsAny<VersioningMode>())).Callback(setTargetEntityAction);
            }
            if (setTargetViewAction != null)
            {
                translator.Setup(x => x.SetTargetInstance(It.IsAny<TTarget>(), It.IsAny<VersioningMode>())).Callback(setTargetViewAction);
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

        public PublishingStatus? GetEnumValue(Guid value)
        {
            if (Enum.TryParse<PublishingStatus>(GetByValue(value), out var result))
            {
                return result;
            }

            return null;
        }
    }

    internal class TestLanguageCache : ILanguageCache, IInternalLanguageCache
    {
        private Dictionary<Guid, string> languages;

        public TestLanguageCache(Dictionary<Guid, string> languages)
        {
            this.languages = languages;
            this.AllowedLanguageIds = languages.Keys.ToList();
            this.AllowedLanguageCodes = languages.Values.ToList();
            this.TranslationOrderLanguageIds = languages.Keys.ToList();
            this.TranslationOrderLanguageCodes = languages.Values.ToList();
            this.LanguageCodes = languages.Values.ToList();
        }

        public Guid Get(string key)
        {
            return key.GetGuid();
        }

        public string GetByValue(Guid value)
        {
            return languages.TryGet(value);
        }

        public bool Exists(string key)
        {
            return true;
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

        public List<string> AllowedLanguageCodes { get; }
        public List<Guid> AllowedLanguageIds { get; }
        public List<string> TranslationOrderLanguageCodes { get; }
        public List<Guid> TranslationOrderLanguageIds { get; }
        public List<string> LanguageCodes { get; }
        public void CheckAndRefresh()
        {
            throw new NotImplementedException();
        }
    }
}
