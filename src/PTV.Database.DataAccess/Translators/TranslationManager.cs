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
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators
{
    [RegisterService(typeof(ITranslationEntity), RegisterType.Transient)]
    [RegisterService(typeof(ITranslationViewModel), RegisterType.Transient)]
    internal class TranslationManager : ITranslationEntity, ITranslationViewModel
    {
        private readonly IResolveManager resolveManager;
        private LanguageCode languageCode = LanguageCode.fi;
        private Guid? languageId = null;

        public TranslationManager(IResolveManager resolveManager, IPrefilteringManager prefilteringManager)
        {
            this.resolveManager = resolveManager;
        }

        public void SetLanguage(LanguageCode language)
        {
            this.languageCode = language;
            var languageCache = resolveManager.Resolve<ILanguageCache>();
            this.languageId = languageCache.Get(language);
        }

        public void SetLanguage(Guid? language)
        {
            this.languageId = language;
            if (language.HasValue)
            {
                var languageCache = resolveManager.Resolve<ILanguageCache>();
                var lang = languageCache.GetByValue(language.Value);
                var lCode = (LanguageCode)(Enum.Parse(typeof(LanguageCode), lang));
                this.languageCode = lCode;
            }
        }

        IReadOnlyList<TTranslateTo> ITranslationEntity.TranslateAll<TTranslateFrom, TTranslateTo>(IQueryable<TTranslateFrom> query)
        {
            var translator = resolveManager.Resolve<ITranslator<TTranslateFrom, TTranslateTo>>();
            if (languageId != null)
            {
                translator.SetLanguage(languageId.Value);
            }
            var executedQuery = query.ToList();// prefilteringManager.ApplyFilters(query).ToList();
            return executedQuery.Select(translator.TranslateEntityToVm).ToList();
        }

        IReadOnlyList<TTranslateTo> ITranslationEntity.TranslateAll<TTranslateFrom, TTranslateTo>(IEnumerable<TTranslateFrom> enumerable)
        {
            var translator = resolveManager.Resolve<ITranslator<TTranslateFrom, TTranslateTo>>();
            if (languageId != null)
            {
                translator.SetLanguage(languageId.Value);
            }
            
            return enumerable.Select(translator.TranslateEntityToVm).ToList();
        }

        IReadOnlyList<TTranslateTo> ITranslationEntity.TranslateAll<TTranslateFrom, TTranslateTo>(ICollection<TTranslateFrom> collection)
        {
            var translator = resolveManager.Resolve<ITranslator<TTranslateFrom, TTranslateTo>>();
            if (languageId != null)
            {
                translator.SetLanguage(languageId.Value);
            }
            return collection.Select(translator.TranslateEntityToVm).ToList();
        }

        public TTranslateTo Translate<TTranslateFrom, TTranslateTo>(TTranslateFrom entity) where TTranslateFrom : class where TTranslateTo : class
        {
            var translator = resolveManager.Resolve<ITranslator<TTranslateFrom, TTranslateTo>>();
            if (languageId != null)
            {
                translator.SetLanguage(languageId.Value);
            }
            return translator.TranslateEntityToVm(entity);
        }

        TTranslateTo ITranslationEntity.TranslateFirst<TTranslateFrom, TTranslateTo>(IQueryable<TTranslateFrom> query)
        {
            var translator = resolveManager.Resolve<ITranslator<TTranslateFrom, TTranslateTo>>();
            if (languageId != null)
            {
                translator.SetLanguage(languageId.Value);
            }
            return translator.TranslateEntityToVm(query.FirstOrDefault());
        }

        TTranslateTo ITranslationViewModel.Translate<TTranslateFrom, TTranslateTo>(TTranslateFrom viewModel, ITranslationUnitOfWork unitofWork)
        {
            var translator = resolveManager.Resolve<ITranslator<TTranslateTo, TTranslateFrom>>();
            if (languageId != null)
            {
                translator.SetLanguage(languageId.Value);
            }
            translator.UnitOfWork = unitofWork;
            return translator.TranslateVmToEntity(viewModel);
        }

        IReadOnlyList<TTranslateTo> ITranslationViewModel.TranslateAll<TTranslateFrom, TTranslateTo>(IEnumerable<TTranslateFrom> model, ITranslationUnitOfWork unitOfWork)
        {
            var translator = resolveManager.Resolve<ITranslator<TTranslateTo, TTranslateFrom>>();
            if (languageId != null)
            {
                translator.SetLanguage(languageId.Value);
            }
            translator.UnitOfWork = unitOfWork;
            return model.Select(i => translator.TranslateVmToEntity(i)).ToList();
        }
    }
}
