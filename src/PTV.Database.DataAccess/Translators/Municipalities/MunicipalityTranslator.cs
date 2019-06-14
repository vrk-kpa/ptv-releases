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
using PTV.Domain.Model.Models.Import;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.Municipalities
{
//    [RegisterService(typeof(ITranslator<Municipality, VmMunicipality>), RegisterType.Scope)]
//    internal class MunicipalityTranslator : Translator<Municipality, VmMunicipality>
//    {
//        public MunicipalityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
//        {
//        }
//
//        public override VmMunicipality TranslateEntityToVm(Municipality entity)
//        {
//            return CreateEntityViewModelDefinition(entity)
//                .AddSimple(i => i.Id, o => o.Id)
//                .AddNavigation(i => i.Code, o => o.MunicipalityCode)
//                .AddLocalizable(i => i.MunicipalityNames, o => o.Name)
//                .GetFinal();
//        }
//
//        public override Municipality TranslateVmToEntity(VmMunicipality vModel)
//        {
//            return CreateViewModelEntityDefinition<Municipality>(vModel)
//                .UseDataContextUpdate(i => true, i => o => i.MunicipalityCode == o.Code, def => def.UseDataContextCreate(x => true, o => o.Id, i => Guid.NewGuid()))
//                //.AddNavigation(input => input.Name, output => output.MunicipalityNames) //TODO MUNICIPALITY NAME ADD TRANSLATOR
//                .AddNavigation(input => input.MunicipalityCode, output => output.Code)
//                .AddNavigation(input => input.Description, output => output.Description)
//                .GetFinal();
//        }
//    }

    [RegisterService(typeof(ITranslator<Municipality, VmJsonMunicipality>), RegisterType.Scope)]
    internal class JsonMunicipalityTranslator : Translator<Municipality, VmJsonMunicipality>
    {
        public JsonMunicipalityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmJsonMunicipality TranslateEntityToVm(Municipality entity)
        {
            throw new NotSupportedException();
        }

        public override Municipality TranslateVmToEntity(VmJsonMunicipality vModel)
        {
            bool isnew = false;
            var definition = CreateViewModelEntityDefinition<Municipality>(vModel)
                .UseDataContextUpdate(i => true, i => o => i.MunicipalityCode == o.Code, def => def.UseDataContextCreate(x => true, o => o.Id,
                    i =>
                    {
                        isnew = true;
                        return Guid.NewGuid();
                    }));
            var entity = definition.GetFinal();
            if (!isnew)
            {
                vModel.Names.ForEach(x => x.OwnerReferenceId = entity.Id);
            }

            return definition
                .AddCollection(input => input.Names, output => output.MunicipalityNames)
                .AddNavigation(input => input.MunicipalityCode, output => output.Code)
                .AddSimple(input => !input.IsRemoved, output => output.IsValid)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<MunicipalityName, VmJsonName>), RegisterType.Scope)]
    internal class JsonNameMunicipalityTranslator : Translator<MunicipalityName, VmJsonName>
    {
        private ILanguageCache languageCache;
        public JsonNameMunicipalityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override VmJsonName TranslateEntityToVm(MunicipalityName entity)
        {
            throw new NotSupportedException();
        }

        public override MunicipalityName TranslateVmToEntity(VmJsonName vModel)
        {
            return CreateViewModelEntityDefinition<MunicipalityName>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), i => o => (i.OwnerReferenceId == o.MunicipalityId && o.LocalizationId == languageCache.Get(i.Language)), def => def.UseDataContextCreate(i => true))
                .AddNavigation(i => i.Name, o => o.Name)
                .AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId)
                .GetFinal();
        }
    }
}
