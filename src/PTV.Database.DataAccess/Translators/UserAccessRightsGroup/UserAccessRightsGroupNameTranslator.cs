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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Municipalities
{
    [RegisterService(typeof(ITranslator<UserAccessRightsGroupName, string>), RegisterType.Transient)]
    internal class UserAccessRightsGroupNameTranslator : Translator<UserAccessRightsGroupName, string>
    {
        public UserAccessRightsGroupNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(UserAccessRightsGroupName entity)
        {
            return CreateEntityViewModelDefinition<string>(entity)
                .AddNavigation(i => i.Name, o => o)
                .GetFinal();
        }

        public override UserAccessRightsGroupName TranslateVmToEntity(string vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<UserAccessRightsGroupName, VmJsonName>), RegisterType.Scope)]
    internal class UserAccessRightsGroupNameJsonTranslator : Translator<UserAccessRightsGroupName, VmJsonName>
    {
        private ILanguageCache languageCache;
        public UserAccessRightsGroupNameJsonTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override VmJsonName TranslateEntityToVm(UserAccessRightsGroupName entity)
        {
            throw new NotSupportedException();
        }

        public override UserAccessRightsGroupName TranslateVmToEntity(VmJsonName vModel)
        {
            return CreateViewModelEntityDefinition<UserAccessRightsGroupName>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), i => o => (i.OwnerReferenceId == o.UserAccessRightsGroupId && o.LocalizationId == languageCache.Get(i.Language)), def => def.UseDataContextCreate(i => true))
                .AddNavigation(i => i.Name, o => o.Name)
                .AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId)
                .GetFinal();
        }
    }
}