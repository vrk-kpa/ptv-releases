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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<Law, V4VmOpenApiLaw>), RegisterType.Transient)]
    internal class OpenApiLawTranslator : Translator<Law, V4VmOpenApiLaw>
    {
        private readonly ILanguageCache languageCache;

        public OpenApiLawTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override V4VmOpenApiLaw TranslateEntityToVm(Law entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.Names, o => o.Names)
                .AddCollection(i => i.WebPages, o => o.WebPages)
                .GetFinal();
        }

        public override Law TranslateVmToEntity(V4VmOpenApiLaw vModel)
        {
            var exists = vModel.Id.IsAssigned();

            if (exists)
            {
                vModel.Names.ForEach(n => n.OwnerReferenceId = vModel.Id);
                vModel.WebPages.ForEach(w => w.OwnerReferenceId = vModel.Id);
            }

            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id, w => w.UseDataContextCreate(x => true))
                .AddCollection(i => i.Names, o => o.Names, false)
                .AddCollection(i => i.WebPages, o => o.WebPages, false)
                .GetFinal();
        }
    }
}
