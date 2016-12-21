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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public abstract class TranslatorTestBase : TestBase
    {
        protected IResolveManagerTest ResolveManager { get; private set; }
        protected ITranslationPrimitives TranslationPrimitives { get; private set; }
        protected ILoggerFactory LoggerFactory { get; private set; }

        protected ITranslationEntity TranslationManagerEntities { get; }

        protected ITranslationViewModel TranslationManagerVModels { get; }

        private ICacheManager cacheManager;
        internal TranslatedInstanceStorage TranslatedInstanceStorage { get; private set; }

        protected TranslatorTestBase()
        {
            ResolveManager = new ResolveManagerTest();
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
            var cacheManagerMock = new Mock<ICacheManager>();
            cacheManagerMock.Setup(i => i.LanguageCache).Returns(new TestLanguageCache());
            cacheManager = cacheManagerMock.Object;
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo(){ RegisterAs = typeof(ICacheManager), Instance = cacheManager });
            ResolveManager.RegisterInstance(new TestRegisterServiceInfo() { RegisterAs = typeof(IPrefilteringManager), Instance = prefilteringManager });
            TranslatedInstanceStorage = new TranslatedInstanceStorage();
            ResolveManager.RegisterInstance(TranslatedInstanceStorage);
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

        internal TestRegisterServiceInfo RegisterTranslatorMock<TSource, TTarget>(Mock<ITranslator<TSource, TTarget>> translator, IUnitOfWork unitOfWork, Func<TTarget, TSource> toEntityFunc = null, Func<TSource,TTarget> toViewModelFunc = null ) where TSource : EntityBase, new()
        {
            translator.Setup(x => x.UnitOfWork).Returns(unitOfWork);
            if (toEntityFunc != null)
            {
                translator.Setup(tr => tr.TranslateVmToEntity(It.IsAny<TTarget>())).Returns(toEntityFunc);
            }
            else
            {
                translator.Setup(tr => tr.TranslateVmToEntity(It.IsAny<TTarget>())).Returns(new TSource());
            }
            if (toViewModelFunc != null)
            {
                translator.Setup(tr => tr.TranslateEntityToVm(It.IsAny<TSource>())).Returns(toViewModelFunc);
            }
            else
            {
                translator.Setup(tr => tr.TranslateEntityToVm(It.IsAny<TSource>())).Returns(null);
            }
            { }
//            }
            return CreateRegisterServiceInfo(translator.Object);
        }
    }

    internal class TestLanguageCache : ILanguageCache
    {
        public Guid Get(string key)
        {
            return key.GetGuid();
        }

        public string GetByValue(Guid value)
        {
            throw new NotImplementedException();
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
    }
}
