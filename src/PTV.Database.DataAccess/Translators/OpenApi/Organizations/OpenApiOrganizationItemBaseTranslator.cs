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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    internal abstract class OpenApiOrganizationItemBaseTranslator<TModel> : Translator<OrganizationVersioned, TModel> where TModel : class, IVmOpenApiItem
    {
        private ILanguageCache languageCache;
        private ITypesCache typeCache;

        public OpenApiOrganizationItemBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typeCache = cacheManager.TypesCache;
        }

        public override TModel TranslateEntityToVm(OrganizationVersioned entity)
        {
            return CreateEntityToVmBaseDefinitions(entity).GetFinal();
        }

        public override OrganizationVersioned TranslateVmToEntity(TModel vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiOrganizationItemBaseTranslator.");
        }

        protected ITranslationDefinitions<OrganizationVersioned, TModel> CreateEntityToVmBaseDefinitions(OrganizationVersioned entity)
        {
            if (entity == null)
            {
                return null;
            }

            var nameTypeId = typeCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var definitions = CreateEntityViewModelDefinition(entity)
                // We have to use unique root id for the organization!
                .AddSimple(i => i.UnificRootId, o => o.Id);

            var languageIdFi = languageCache.Get(DomainConstants.DefaultLanguage);
            // Primarily let's use finnish name.
            if (entity.OrganizationNames.Any(e => e.TypeId == nameTypeId && e.LocalizationId == languageIdFi))
            {
                definitions.AddNavigation(i => i.OrganizationNames.Where(n => n.TypeId == nameTypeId && n.LocalizationId == languageIdFi).FirstOrDefault(), o => o.Name);
            }
            else
            {
                definitions.AddNavigation(i => i.OrganizationNames?.FirstOrDefault(n => n.TypeId == nameTypeId)?.Name, o => o.Name);
            }

            return definitions;
        }
    }
}
