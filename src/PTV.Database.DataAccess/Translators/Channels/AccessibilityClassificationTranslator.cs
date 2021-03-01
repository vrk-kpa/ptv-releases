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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<AccessibilityClassification, VmAccessibilityClassification>), RegisterType.Transient)]
    internal class AccessibilityClassificationTranslator : Translator<AccessibilityClassification, VmAccessibilityClassification>
    {
        private ITypesCache typesCache;
        public AccessibilityClassificationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmAccessibilityClassification TranslateEntityToVm(AccessibilityClassification entity)
        {
;
            return CreateEntityViewModelDefinition<VmAccessibilityClassification>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.AccessibilityClassificationLevelTypeId, o => o.AccessibilityClassificationLevelTypeId)
                .AddSimple(i => i.WcagLevelTypeId, o => o.WcagLevelTypeId)
                .AddNavigation(i => i.Name, o => o.Name)
                .AddNavigation(i => i.Url, o => o.UrlAddress)
                .GetFinal();
        }

        public override AccessibilityClassification TranslateVmToEntity(VmAccessibilityClassification vModel)
        {
            //Change vModel values
            UpdateModelValues(vModel);

            bool exists = vModel.Id.IsAssigned();

            var translationDefinition = CreateViewModelEntityDefinition<AccessibilityClassification>(vModel)
               .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
               .UseDataContextUpdate(input => exists, input => output => input.Id == output.Id)
               .AddSimple(i => vModel.AccessibilityClassificationLevelTypeId ?? typesCache.Get<AccessibilityClassificationLevelType>(AccessibilityClassificationLevelTypeEnum.Unknown.ToString()), o => o.AccessibilityClassificationLevelTypeId)
               .AddSimple(i => vModel.WcagLevelTypeId, o => o.WcagLevelTypeId)
               .AddNavigation(i => vModel.UrlAddress, o => o.Url)
               .AddNavigation(i => vModel.Name, o => o.Name)
               .AddRequestLanguage(output => output);

            var entity = translationDefinition.GetFinal();
            return entity;

        }

        private void UpdateModelValues(VmAccessibilityClassification vModel)
        {
            var acLevelTypeUnkownId =
                typesCache.Get<AccessibilityClassificationLevelType>(
                    AccessibilityClassificationLevelTypeEnum.Unknown.ToString());
            var accessibilityClassificationLevelType =
                typesCache.GetByValue<AccessibilityClassificationLevelType>(
                    vModel.AccessibilityClassificationLevelTypeId ?? acLevelTypeUnkownId);

            if (!string.IsNullOrEmpty(accessibilityClassificationLevelType) && Enum.IsDefined(typeof(AccessibilityClassificationLevelTypeEnum), accessibilityClassificationLevelType))
            {
                var accessibilityClassificationLevelTypeEnum = Enum.Parse(
                    typeof(AccessibilityClassificationLevelTypeEnum),
                    accessibilityClassificationLevelType, true);

                switch (accessibilityClassificationLevelTypeEnum)
                {
                    case AccessibilityClassificationLevelTypeEnum.Unknown:
                    {
                        vModel.WcagLevelTypeId = null;
                        vModel.Name = null;
                        vModel.UrlAddress = null;
                    }
                        break;
                    case AccessibilityClassificationLevelTypeEnum.NonCompliant:
                        vModel.WcagLevelTypeId = null;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
