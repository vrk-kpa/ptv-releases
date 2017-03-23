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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    internal abstract class OpenApiAttachmentBaseTranslator<TVmOpenApiAttachment> : Translator<Attachment, TVmOpenApiAttachment> where TVmOpenApiAttachment : class, IVmOpenApiAttachment
    {
        private readonly ILanguageCache languageCache;

        protected OpenApiAttachmentBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override TVmOpenApiAttachment TranslateEntityToVm(Attachment entity)
        {
            return CreateBaseDefinitions(entity).GetFinal();
        }

        public override Attachment TranslateVmToEntity(TVmOpenApiAttachment vModel)
        {
            return CreateVmBaseDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<Attachment, TVmOpenApiAttachment> CreateBaseDefinitions(Attachment entity)
        {
            return CreateEntityViewModelDefinition<TVmOpenApiAttachment>(entity)
                .AddNavigation(i => i.Name, o => o.Name)
                .AddNavigation(i => i.Description, o => o.Description)
                .AddNavigation(i => i.Url, o => o.Url)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language);
        }

        protected ITranslationDefinitions<TVmOpenApiAttachment, Attachment> CreateVmBaseDefinitions(TVmOpenApiAttachment vModel)
        {
            var exists = vModel.Id.IsAssigned();

            return CreateViewModelEntityDefinition<Attachment>(vModel)
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id, e => e.UseDataContextCreate(x => true))
                .AddNavigation(i => i.Name, o => o.Name)
                .AddNavigation(i => i.Description, o => o.Description)
                .AddNavigation(i => i.Url, o => o.Url)
                .AddNavigation(i => i.Language, o => o.Localization);
        }
    }
}
