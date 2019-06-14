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
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Enums;
using System.Linq;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Translators.TranslationOrder
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmTranslationOrderEntityBase>), RegisterType.Transient)]
    internal class ChannelLanguageAvailabilityTranslator : Translator<ServiceChannelVersioned, VmTranslationOrderEntityBase>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public ChannelLanguageAvailabilityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmTranslationOrderEntityBase TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmTranslationOrderEntityBase vModel)
        {
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var names = vModel.LanguagesAvailabilities.Where(lang => lang.IsNew).Select(lang => new VmName() { LocalizationId = lang.LanguageId, Name = (vModel.SourceName ?? String.Empty) + " [" + languageCache.GetByValue(lang.LanguageId).ToUpper() + "]", TypeId = nameTypeId });

            var transaltionDefinition = CreateViewModelEntityDefinition<ServiceChannelVersioned>(vModel)
                
                .UseDataContextUpdate(i => true, i => o => vModel.Id == o.Id)
                .UseVersioning<ServiceChannelVersioned, ServiceChannel>(i => i, vModel.KeepInPreviousState ? VersioningMode.KeepInPreviousState | VersioningMode.KeepStateOfLanguages : 0)
                .AddLanguageAvailability(i => i, o => o)
                .AddCollectionWithKeep(i => names, o => o.ServiceChannelNames, t => true);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }
    }
}