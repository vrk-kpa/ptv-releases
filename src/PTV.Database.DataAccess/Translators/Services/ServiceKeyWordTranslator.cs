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
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceKeyword, VmKeywordItem>), RegisterType.Transient)]
    internal class ServiceKeyWordTranslator : Translator<ServiceKeyword, VmKeywordItem>
    {
        public ServiceKeyWordTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmKeywordItem TranslateEntityToVm(ServiceKeyword entity)
        {
            return CreateEntityViewModelDefinition<VmKeywordItem>(entity)
               .AddNavigation(input => input.Keyword.Name, output => output.Name)
               .AddSimple(input => input.Keyword.Id, output => output.Id)
               .GetFinal();
        }

        public override ServiceKeyword TranslateVmToEntity(VmKeywordItem vModel)
        {
            var translatorDef = CreateViewModelEntityDefinition<ServiceKeyword>(vModel)
                .UseDataContextUpdate(i => true, i => o => i.Id == o.KeywordId && i.OwnerReferenceId == o.ServiceVersionedId,
                    def =>
                    {
                        def.UseDataContextCreate(i => true);
                    });
            if (!vModel.Id.IsAssigned())
            {
                translatorDef.AddNavigation(i => i, o => o.Keyword);
            }
            else
            {
                translatorDef.AddSimple(i => i.Id.Value, o => o.KeywordId);
            }
            var entity = translatorDef.GetFinal();
            if (entity.Keyword != null && entity.KeywordId != entity.Keyword.Id)
            {
                entity.KeywordId = entity.Keyword.Id;
            }
            return entity;
        }
    }
}
