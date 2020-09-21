﻿/**
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
**/

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelPhone, V4VmOpenApiPhoneWithType>), RegisterType.Transient)]
    internal class OpenApiServiceChannelPhoneWithTypeTranslator : Translator<ServiceChannelPhone, V4VmOpenApiPhoneWithType>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceChannelPhoneWithTypeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V4VmOpenApiPhoneWithType TranslateEntityToVm(ServiceChannelPhone entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceChannelPhoneWithTypeTranslator!");
        }

        public override ServiceChannelPhone TranslateVmToEntity(V4VmOpenApiPhoneWithType vModel)
        {
            var exists = vModel.OwnerReferenceId.IsAssigned();
            var typeId = typesCache.Get<PhoneNumberType>(vModel.Type);

            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !exists)
                .UseDataContextUpdate(i => exists, i => o => 
                        i.OwnerReferenceId.Value == o.ServiceChannelVersionedId 
                        && typeId == o.Phone.TypeId 
                        && languageCache.Get(i.Language) == o.Phone.LocalizationId 
                        && i.Number == o.Phone.Number 
                        && i.PrefixNumber == o.Phone.PrefixNumber.Code
                        && i.AdditionalInformation == o.Phone.AdditionalInformation,
                    e => e.UseDataContextCreate(x => true));


            if (exists)
            {
                var entity = definition.GetFinal();
                if (entity.Created != DateTime.MinValue)
                {
                    vModel.Id = entity.PhoneId;
                }
            }

            definition.AddNavigation(i => i, o => o.Phone);

            return definition.GetFinal();
        }
    }
}
