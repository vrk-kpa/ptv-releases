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
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Database.Model.Interfaces;
using PTV.Database.DataAccess.Translators.Common;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<PostalCode, VmPostalCode>), RegisterType.Transient)]
    internal class PostalCodeTranslator : Translator<PostalCode, VmPostalCode>
    {
        private TranslatedItemDefinitionHelper definitionHelper;
        public PostalCodeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, TranslatedItemDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
        }

        public override VmPostalCode TranslateEntityToVm(PostalCode entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.Code, o => o.Code)
                .AddSimple(i => i.MunicipalityId, o => o.MunicipalityId)
                .AddSimple(i => i.Id, o => o.Id);

            definitionHelper.AddTranslations(definition, entity?.Code);
            return definition.GetFinal();
        }

        /// <summary>
        /// Translate vm to entity
        /// </summary>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public override PostalCode TranslateVmToEntity(VmPostalCode vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(x => true, i => o => i.Code == o.Code)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<PostalCode, VmJsonPostalCode>), RegisterType.Transient)]
    internal class PostalCodeJsonTranslator : Translator<PostalCode, VmJsonPostalCode>
    {
        public PostalCodeJsonTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmJsonPostalCode TranslateEntityToVm(PostalCode entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.Code, o => o.Code)               
                .AddNavigation(i => i.Municipality?.Code, o => o.MunicipalityCode)
                .AddSimple(i => i.MunicipalityId, o => o.MunicipalityId)
                .GetFinal();
        }

        /// <summary>
        /// Translate vm to entity
        /// </summary>
        /// <param name="vModel"></param>
        /// <returns></returns>
        public override PostalCode TranslateVmToEntity(VmJsonPostalCode vModel)
        {
            bool isnew = false;
            var definitiion = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(x => true, i => o => i.Code == o.Code, def =>
                {
                    def.UseDataContextCreate(i => true, o => o.Id, i =>
                    {
                        isnew = true;
                        return Guid.NewGuid();
                    });
                    def.AddNavigation(i => i.Code, o => o.Code);
                });
            var entity = definitiion.GetFinal();
            if (!isnew)
            {
                vModel.Names.ForEach(x => x.OwnerReferenceId = entity.Id);
            }

            return definitiion.AddCollection(i => i.Names, o => o.PostalCodeNames)
                .AddSimple(i => i.MunicipalityId, o => o.MunicipalityId)
                .GetFinal();
        }
    }    
}
