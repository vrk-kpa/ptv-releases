﻿/**
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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<OrganizationEInvoicing, VmElectronicInvoicingAddress>), RegisterType.Transient)]
    internal class ElectronicInvoicingAddressTranslator : Translator<OrganizationEInvoicing, VmElectronicInvoicingAddress>
    {
        private readonly ILanguageCache languageCache;
        
        public ElectronicInvoicingAddressTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override VmElectronicInvoicingAddress TranslateEntityToVm(OrganizationEInvoicing entity)
        {
            return CreateEntityViewModelDefinition<VmElectronicInvoicingAddress>(entity)
                .AddSimple(i => i.Id, o => o.Id) 
                .AddNavigation(i => i.ElectronicInvoicingAddress, o => o.ElectronicInvoicingAddress)
                .AddNavigation(i => i.OperatorCode, o => o.OperatorCode)
                .AddDictionary(input => entity.EInvoicingAdditionalInformations, output => output.AdditionalInformation, k => languageCache.GetByValue(k.LocalizationId))
                .GetFinal();
        }

        public override OrganizationEInvoicing TranslateVmToEntity(VmElectronicInvoicingAddress vModel)
        {
            var exists = vModel.Id.IsAssigned();
            
            var translationDefinitions = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id, def => def.UseDataContextCreate(i => true, output => output.Id, input => Guid.NewGuid()))
                .AddNavigation(i => i.ElectronicInvoicingAddress, o => o.ElectronicInvoicingAddress)
                .AddNavigation(i => i.OperatorCode, o => o.OperatorCode)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber);
            
            translationDefinitions.AddCollection(i => i.AdditionalInformation?.Select(
                    pair => new VmLocalizedAdditionalInformation()
                    {
                        Content = pair.Value,
                        LocalizationId = languageCache.Get(pair.Key),
                        OwnerReferenceId = i.Id
                    }),
                o => o.EInvoicingAdditionalInformations, true);

            var entity = translationDefinitions.GetFinal();
            return entity;
        }
    }
}
