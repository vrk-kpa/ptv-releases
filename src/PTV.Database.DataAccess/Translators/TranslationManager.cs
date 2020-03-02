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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators
{
    [RegisterService(typeof(ITranslationEntity), RegisterType.Transient)]
    [RegisterService(typeof(ITranslationViewModel), RegisterType.Transient)]
    internal class TranslationManager : ITranslationEntity, ITranslationViewModel
    {
        private readonly IResolveManager resolveManager;
        private Guid? languageId = null;
        private IHttpContextAccessor httpContextAccessorHolder = null;
        private IHttpContextAccessor HttpContext => httpContextAccessorHolder ?? (httpContextAccessorHolder = resolveManager.Resolve<IHttpContextAccessor>());
        private CancellationToken CancellationToken => HttpContext?.HttpContext?.RequestAborted ?? new CancellationToken(false);

        public TranslationManager(IResolveManager resolveManager/*, IPerformanceMonitorManager performanceMonitorManager , IPrefilteringManager prefilteringManager*/)
        {
            this.resolveManager = resolveManager;
        }

        public void SetLanguage(Guid? language)
        {
            this.languageId = language;
            if (language.HasValue)
            {
                var languageCache = resolveManager.Resolve<ILanguageCache>();
                var lang = languageCache.GetByValue(language.Value);
            }
        }

        IReadOnlyList<TTranslateTo> ITranslationEntity.TranslateAll<TTranslateFrom, TTranslateTo>(IQueryable<TTranslateFrom> query)
        {
            using (var scope = resolveManager.CreateScope())
            {
                var performanceMonitorManager = scope.ServiceProvider.GetService<IPerformanceMonitorManager>();
                performanceMonitorManager.AddIgnoredClass<TranslationManager>();
                return performanceMonitorManager.MeasureAction(() =>
                {
                    var translator = scope.ServiceProvider.GetService<ITranslator<TTranslateFrom, TTranslateTo>>();
                    if (languageId != null)
                    {
                        translator.SetLanguage(languageId.Value);
                    }
                    var executedQuery = Asyncs.HandleAsyncInSync(() => query.ToListAsync(CancellationToken));
                    return executedQuery.Select(translator.TranslateEntityToVm).ToList();
                }, " (Translator)");
            }
        }

        IReadOnlyList<TTranslateTo> ITranslationEntity.TranslateAll<TTranslateFrom, TTranslateTo>(IEnumerable<TTranslateFrom> enumerable)
        {
            using (var scope = resolveManager.CreateScope())
            {
                var performanceMonitorManager = scope.ServiceProvider.GetService<IPerformanceMonitorManager>();
                performanceMonitorManager.AddIgnoredClass<TranslationManager>();
                return performanceMonitorManager.MeasureAction(() =>
                {
                    var translator = scope.ServiceProvider.GetService<ITranslator<TTranslateFrom, TTranslateTo>>();
                    if (languageId != null)
                    {
                        translator.SetLanguage(languageId.Value);
                    }

                    return enumerable.Select(translator.TranslateEntityToVm).ToList();
                }, " (Translator)");
            }
        }

        IReadOnlyList<TTranslateTo> ITranslationEntity.TranslateAll<TTranslateFrom, TTranslateTo>(ICollection<TTranslateFrom> collection)
        {
            using (var scope = resolveManager.CreateScope())
            {
                var performanceMonitorManager = scope.ServiceProvider.GetService<IPerformanceMonitorManager>();
                performanceMonitorManager.AddIgnoredClass<TranslationManager>();
                return performanceMonitorManager.MeasureAction(() =>
                {
                    var translator = scope.ServiceProvider.GetService<ITranslator<TTranslateFrom, TTranslateTo>>();
                    if (languageId != null)
                    {
                        translator.SetLanguage(languageId.Value);
                    }

                    return collection.Select(translator.TranslateEntityToVm).ToList();
                }, " (Translator)");
            }
        }

        public TTranslateTo Translate<TTranslateFrom, TTranslateTo>(TTranslateFrom entity) where TTranslateFrom : class where TTranslateTo : class
        {
            using (var scope = resolveManager.CreateScope())
            {
                var performanceMonitorManager = scope.ServiceProvider.GetService<IPerformanceMonitorManager>();
                performanceMonitorManager.AddIgnoredClass<TranslationManager>();
                return performanceMonitorManager.MeasureAction(() =>
                {
                    var translator = scope.ServiceProvider.GetService<ITranslator<TTranslateFrom, TTranslateTo>>();
                    if (languageId != null)
                    {
                        translator.SetLanguage(languageId.Value);
                    }

                    return translator.TranslateEntityToVm(entity);
                }, " (Translator)");
            }
        }

        TTranslateTo ITranslationEntity.TranslateFirst<TTranslateFrom, TTranslateTo>(IQueryable<TTranslateFrom> query)
        {
            using (var scope = resolveManager.CreateScope())
            {
                var performanceMonitorManager = scope.ServiceProvider.GetService<IPerformanceMonitorManager>();
                performanceMonitorManager.AddIgnoredClass<TranslationManager>();
                return performanceMonitorManager.MeasureAction(() =>
                {
                    var translator = scope.ServiceProvider.GetService<ITranslator<TTranslateFrom, TTranslateTo>>();
                    if (languageId != null)
                    {
                        translator.SetLanguage(languageId.Value);
                    }

                    var fetched = Asyncs.HandleAsyncInSync(() => query.FirstOrDefaultAsync(CancellationToken));
                    return translator.TranslateEntityToVm(fetched);
                }, " (Translator)");
            }
        }

        TTranslateTo ITranslationViewModel.Translate<TTranslateFrom, TTranslateTo>(TTranslateFrom viewModel, ITranslationUnitOfWork unitOfWork)
        {
            using (var scope = resolveManager.CreateScope())
            {
                var performanceMonitorManager = scope.ServiceProvider.GetService<IPerformanceMonitorManager>();
                performanceMonitorManager.AddIgnoredClass<TranslationManager>();
                return performanceMonitorManager.MeasureAction(() =>
                {
                    var translator = scope.ServiceProvider.GetService<ITranslator<TTranslateTo, TTranslateFrom>>();
                    if (languageId != null)
                    {
                        translator.SetLanguage(languageId.Value);
                    }

                    translator.UnitOfWork = unitOfWork;
                    return translator.TranslateVmToEntity(viewModel);
                }, " (Translator)");
            }
        }

        IReadOnlyList<TTranslateTo> ITranslationViewModel.TranslateAll<TTranslateFrom, TTranslateTo>(IEnumerable<TTranslateFrom> model, ITranslationUnitOfWork unitOfWork)
        {
            using (var scope = resolveManager.CreateScope())
            {
                var performanceMonitorManager = scope.ServiceProvider.GetService<IPerformanceMonitorManager>();
                performanceMonitorManager.AddIgnoredClass<TranslationManager>();
                return performanceMonitorManager.MeasureAction(() =>
                {
                    var translator = scope.ServiceProvider.GetService<ITranslator<TTranslateTo, TTranslateFrom>>();
                    if (languageId != null)
                    {
                        translator.SetLanguage(languageId.Value);
                    }

                    translator.UnitOfWork = unitOfWork;
                    return model.Select(i => translator.TranslateVmToEntity(i)).ToList();
                }, " (Translator)");
            }
        }
    }
}
