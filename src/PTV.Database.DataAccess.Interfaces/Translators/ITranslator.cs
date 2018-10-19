﻿/**
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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Interfaces.Translators
{
    internal interface ITranslator<TTranslateFrom, TTranslateTo> : PTV.Framework.ServiceManager.ITranslator, ITranslationDefinitionAccessor<TTranslateFrom, TTranslateTo>
    {
        ITranslationUnitOfWork UnitOfWork { get; set; }

        void SetLanguage(Guid languageId);

        void SetTranslationPolicy(TranslationPolicy translationPolicy);

        TTranslateTo TranslateEntityToVm(TTranslateFrom entity);

        TTranslateFrom TranslateVmToEntity(TTranslateTo vModel);

        ITranslationDefinitionsEntityToVModel<TTranslateFrom, TTranslateTo> CreateEntityViewModelDefinition<TInstantiate>(TTranslateFrom entity) where TInstantiate : TTranslateTo;

        ITranslationDefinitionsForContextUsage<TTranslateTo, TTranslateFrom> CreateViewModelEntityDefinition<TInstantiate>(TTranslateTo viewModel) where TInstantiate : TTranslateFrom;

        ITranslationDefinitionsEntityToVModel<TTranslateFrom, TTranslateTo> CreateEntityViewModelDefinition(TTranslateFrom entity);

        ITranslationDefinitionsForContextUsage<TTranslateTo, TTranslateFrom> CreateViewModelEntityDefinition(TTranslateTo viewModel);
    }

    internal interface ITranslationDefinitionAccessor<TTranslateFrom, TTranslateTo>
    {
        void SetTargetInstance(TTranslateTo instance, VersioningMode versioningMode);
        void SetTargetInstance(TTranslateFrom instance, VersioningMode versioningMode);
        void SetLanguageInternaly(Guid languageId, bool explicitlySet);
    }
}
