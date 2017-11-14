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
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.V2.Common;
using System.Collections.Generic;
using System.Linq;
using VmDigitalAuthorization = PTV.Domain.Model.Models.V2.Common.VmDigitalAuthorization;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.Common.V2
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmConnectionInput>), RegisterType.Transient)]
    internal class ConnectionInputTranslator : Translator<ServiceServiceChannel, VmConnectionInput>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        public ConnectionInputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmConnectionInput TranslateEntityToVm(ServiceServiceChannel entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmConnectionInput vModel)
        {
            var descriptions = new List<VmDescription>();
            descriptions.AddNullRange(vModel.BasicInformation?.Description?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.Description)));
            descriptions.AddNullRange(vModel.BasicInformation?.AdditionalInformation?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ChargeTypeAdditionalInfo)));

            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(input => true, input => output => input.MainEntityId == output.ServiceId && input.ConnectedEntityId == output.ServiceChannelId,
                def => def.UseDataContextCreate(input => true).AddSimple(input => input.MainEntityId, output => output.ServiceId)
                )
                .AddSimple(i => i.ConnectedEntityId, o => o.ServiceChannelId)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .AddSimple(i => i.BasicInformation?.ChargeType, o => o.ChargeTypeId)
                .AddCollection(i => descriptions, o => o.ServiceServiceChannelDescriptions, true)
                .AddCollectionWithRemove(i => i.DigitalAuthorization?.DigitalAuthorizations?.Select(x => new VmDigitalAuthorization() { Id = x, OwnerReferenceId = i.MainEntityId, OwnerReferenceId2 = i.ConnectedEntityId }), o => o.ServiceServiceChannelDigitalAuthorizations, x => true)
                .GetFinal();
        }
        private VmDescription CreateDescription(string language, string value, VmConnectionInput vModel, DescriptionTypeEnum typeEnum, bool isEmpty = false)
        {
            return new VmDescription
            {
                Description = isEmpty ? null : value,
                TypeId = typesCache.Get<DescriptionType>(typeEnum.ToString()),
                OwnerReferenceId = vModel.MainEntityId,
                OwnerReferenceId2 = vModel.ConnectedEntityId,
                LocalizationId = languageCache.Get(language)
            };
        }
    }
}