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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.V2.Common;

namespace PTV.Database.DataAccess.Translators
{
    [RegisterService(typeof(ITranslator<ServiceVersioned, VmEntityUpdateName>), RegisterType.Transient)]
    internal class EntityUpdateNameTranslator : Translator<ServiceVersioned, VmEntityUpdateName>
    {
        private readonly ILanguageCache languageCache;
        private ITypesCache typesCache;
        public EntityUpdateNameTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmEntityUpdateName TranslateEntityToVm(ServiceVersioned entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceVersioned TranslateVmToEntity(VmEntityUpdateName vModel)
        {
            var names = new List<VmName>();
            names.AddNullRange(CreateNames(vModel, vModel.Name, NameTypeEnum.Name));

            var transaltionDefinition = CreateViewModelEntityDefinition<ServiceVersioned>(vModel)
                .UseDataContextUpdate(i => true, i => o => vModel.Id == o.Id)
                .UseVersioning<ServiceVersioned, Service>(o => o,
                    VersioningMode.KeepInPreviousState | VersioningMode.KeepStateOfLanguages)
                .AddLanguageAvailability(i => i)
                .AddCollectionWithKeep(i => names, o => o.ServiceNames, r => r.TypeId != typesCache.Get<NameType>(NameTypeEnum.Name.ToString()));

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }

        private IEnumerable<VmName> CreateNames(VmEntityUpdateName vModel, Dictionary<string, string> name, NameTypeEnum nameType)
        {
            return name?.Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(pair => CreateName(pair.Key, pair.Value, vModel, nameType));
        }

        private VmName CreateName(string language, string value, VmEntityUpdateName vModel, NameTypeEnum typeEnum)
        {
            return new VmName
            {
                Name = value,
                TypeId = typesCache.Get<NameType>(typeEnum.ToString()),
                OwnerReferenceId = vModel.Id,
                LocalizationId = languageCache.Get(language),
                Inherited = null
            };
        }
    }
}

