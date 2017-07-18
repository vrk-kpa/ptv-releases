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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators
{
    internal abstract class Translator<TTranslateFrom, TTranslateTo> : ITranslator<TTranslateFrom, TTranslateTo> where TTranslateFrom : class where TTranslateTo : class
    {
        private readonly IResolveManager resolveManager;
        private readonly ITranslationPrimitives translationPrimitives;
        private TTranslateTo predefinedTargetTo;
        private TTranslateFrom predefinedTargetFrom;
        private Guid? requestLanguage = null;
        private LanguageCode? requestLanguageCode = null;
        protected IVersioningManager VersioningManager;
        protected ITextManager textManager;
        private readonly ILanguageCache languageCache;
        private TranslationPolicy translationPolicies = TranslationPolicy.Defaults;
        private bool languageIsSet = false;

        protected Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
        {
            this.resolveManager = resolveManager;
            this.translationPrimitives = translationPrimitives;
            this.languageCache = resolveManager.Resolve<ILanguageCache>();
            this.VersioningManager = resolveManager.Resolve<IVersioningManager>();
            this.textManager = resolveManager.Resolve<ITextManager>();
        }

        protected LanguageCode RequestLanguageCode => this.requestLanguageCode ?? DomainConstants.DefaultLanguage;
        protected Guid RequestLanguageId => this.requestLanguage ?? languageCache.Get(RequestLanguageCode);

        public void SetLanguage(LanguageCode language)
        {
            this.requestLanguageCode = language;
            this.requestLanguage = languageCache.Get(language);
            languageIsSet = true;
        }

        public void SetLanguage(Guid languageId)
        {
            this.requestLanguage = languageId;
            this.requestLanguageCode = languageCache.GetByValueEnum(languageId);
            languageIsSet = true;
        }

        public void SetTranslationPolicy(TranslationPolicy translationPolicy)
        {
            this.translationPolicies = translationPolicy;
        }

        void ITranslationDefinitionAccessor<TTranslateFrom, TTranslateTo>.SetLanguageInternaly(Guid languageId, bool explicitlySet)
        {
            this.requestLanguage = languageId;
            this.requestLanguageCode = languageCache.GetByValueEnum(languageId);
            this.languageIsSet = explicitlySet;
        }
        
        public ITranslationUnitOfWork UnitOfWork { get; set; }

        public abstract TTranslateTo TranslateEntityToVm(TTranslateFrom entity);

        public abstract TTranslateFrom TranslateVmToEntity(TTranslateTo vModel);

        public ITranslationDefinitionsEntityToVModel<TTranslateFrom, TTranslateTo> CreateEntityViewModelDefinition<TInstantiate>(TTranslateFrom entity)
            where TInstantiate : TTranslateTo
        {
            return CreateDefinition<TTranslateFrom, TTranslateTo, TInstantiate>(entity, null, TranslationDirection.EntityToViewModel, predefinedTargetTo);
        }

        public ITranslationDefinitionsVModelToEntity<TTranslateTo, TTranslateFrom> CreateViewModelEntityDefinition<TInstantiate>(TTranslateTo viewModel)
            where TInstantiate : TTranslateFrom
        {
            return CreateDefinition<TTranslateTo, TTranslateFrom, TInstantiate>(viewModel, UnitOfWork, TranslationDirection.ViewModelToEntity, predefinedTargetFrom);
        }

        public ITranslationDefinitionsEntityToVModel<TTranslateFrom, TTranslateTo> CreateEntityViewModelDefinition(TTranslateFrom entity)
        {
            return CreateDefinition<TTranslateFrom, TTranslateTo, TTranslateTo>(entity, null, TranslationDirection.EntityToViewModel, predefinedTargetTo);
        }


        public ITranslationDefinitionsVModelToEntity<TTranslateTo, TTranslateFrom> CreateViewModelEntityDefinition(TTranslateTo viewModel)
        {
            return CreateDefinition<TTranslateTo, TTranslateFrom, TTranslateFrom>(viewModel, UnitOfWork, TranslationDirection.ViewModelToEntity, predefinedTargetFrom);
        }

        void ITranslationDefinitionAccessor<TTranslateFrom, TTranslateTo>.SetTargetInstance(TTranslateTo instance)
        {
            this.predefinedTargetTo = instance;
        }

        void ITranslationDefinitionAccessor<TTranslateFrom, TTranslateTo>.SetTargetInstance(TTranslateFrom instance)
        {
            this.predefinedTargetFrom = instance;
        }

        private TranslationDefinitions<TIn, TOut> CreateDefinition<TIn, TOut, TInstantiate>(TIn entity,
            ITranslationUnitOfWork unitofwork, TranslationDirection translationDirection, TOut predefinedTarget) where TIn : class where TOut : class
        {
            var languageId = requestLanguage ?? languageCache.Get(RequestLanguageCode);
            var definition = new TranslationDefinitions<TIn, TOut>(resolveManager, translationPrimitives, unitofwork, translationDirection, VersioningManager,
                typeof(TInstantiate), predefinedTarget, languageId, translationPolicies);
            definition.SetTnit(entity);
            var internalAssignement = definition as ITranslationDefinitionAccessor<TIn, TOut>;
            internalAssignement?.SetLanguageInternaly(languageId, languageIsSet);
            return definition;
        }

    }

    public enum TranslationDirection
    {
        EntityToViewModel,
        ViewModelToEntity
    }
}