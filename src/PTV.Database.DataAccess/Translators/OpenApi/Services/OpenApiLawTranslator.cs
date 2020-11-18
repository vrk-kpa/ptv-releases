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

using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<Law, V4VmOpenApiLaw>), RegisterType.Transient)]
    internal class OpenApiLawTranslator : Translator<Law, V4VmOpenApiLaw>
    {
        public OpenApiLawTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override V4VmOpenApiLaw TranslateEntityToVm(Law entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.Names, o => o.Names)
                // PTV-2768 - production database includes services where several web pages for a law is attach (same language).
                // Fix is done in a same way than into UI side. See LawTranslator.cs
                .AddCollectionWithRemove(
                    i => i.WebPages.GroupBy(x => x.LocalizationId)
                        .Select(x => x.OrderByDescending(y => y.Modified).First()),
                    o => o.WebPages,
                    o => true)
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
                .AddCollection(i => i.WebPages.Where(x => !x.Url.IsNullOrWhitespace()), o => o.WebPages, false)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .GetFinal();
        }
    }
}
