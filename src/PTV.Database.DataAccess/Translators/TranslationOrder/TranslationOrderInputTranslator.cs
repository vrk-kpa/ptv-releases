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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.TranslationOrder;

namespace PTV.Database.DataAccess.Translators.TranslationOrder
{
    [RegisterService(typeof(ITranslator<Model.Models.TranslationOrder, VmTranslationOrderInput>), RegisterType.Transient)]
    internal class TranslationOrderInputTranslator : Translator<Model.Models.TranslationOrder, VmTranslationOrderInput>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;

        public TranslationOrderInputTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmTranslationOrderInput TranslateEntityToVm(Model.Models.TranslationOrder entity)
        {
            throw new NotImplementedException();
        }

        public override Model.Models.TranslationOrder TranslateVmToEntity(VmTranslationOrderInput vModel)
        {
            var isNew = !vModel.Id.IsAssigned();

            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                .AddSimple(i => i.SourceLanguage, o => o.SourceLanguageId)
                .AddSimple(i => i.TargetLanguage, o => o.TargetLanguageId)
                .AddSimple(i => i.TranslationCompanyId, o => o.TranslationCompanyId)
                .AddSimple(i => i.OrderIdentifier, o => o.OrderIdentifier)
                .AddSimple(i => i.OrganizationIdentifier, o => o.OrganizationIdentifier)
                .AddNavigation(i => i.AdditionalInformation, o => o.AdditionalInformation)
                .AddNavigation(i => i.SenderName, o => o.SenderName)
                .AddNavigation(i => i.SenderEmail, o => o.SenderEmail)
                .AddNavigation(i => i.SourceEntityName, o => o.SourceEntityName)
                .AddNavigation(i => i.OrganizationName, o => o.OrganizationName)
                .AddNavigation(i => i.OrganizationBusinessCode, o => o.OrganizationBusinessCode);

                if (isNew)
                {
                    var translationOrderStateCreated = typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.ReadyToSend.ToString()); 
                    definition.AddCollection(i =>
                            new List<VmTranslationOrderStateInput>()
                            {
                                new VmTranslationOrderStateInput()
                                {
                                    TranslationOrderId = i.Id,
                                    TranslationStateId = translationOrderStateCreated,
                                    Last =  true,
                                }
                            },
                        o => o.TranslationOrderStates);
                }

            var entity = definition.GetFinal();
            return entity;
        }
    };
}