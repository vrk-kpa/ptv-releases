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

using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelPhone, V4VmOpenApiPhone>), RegisterType.Transient)]
    internal class OpenApiServiceChannelPhoneTranslator : Translator<ServiceChannelPhone, V4VmOpenApiPhone>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceChannelPhoneTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V4VmOpenApiPhone TranslateEntityToVm(ServiceChannelPhone entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceChannelPhoneTranslator!");
        }

        public override ServiceChannelPhone TranslateVmToEntity(V4VmOpenApiPhone vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            var typeid = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString());
            var exists = vModel.OwnerReferenceId.IsAssigned();

            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextUpdate(input => exists,
                    input => output => 
                        input.OwnerReferenceId.Value == output.ServiceChannelVersionedId 
                        && typeid == output.Phone.TypeId 
                        && languageCache.Get(input.Language) == output.Phone.LocalizationId 
                        && input.Number == output.Phone.Number 
                        && input.PrefixNumber == output.Phone.PrefixNumber.Code
                        && input.AdditionalInformation == output.Phone.AdditionalInformation,
                    e => e.UseDataContextCreate(x => true));

            if (exists)
            {
                var serviceChannelPhone = translation.GetFinal();
                if (serviceChannelPhone.Created > DateTime.MinValue)
                {
                    vModel.Id = serviceChannelPhone.PhoneId;
                }
            }

            return translation
                .AddNavigation(input => vModel, output => output.Phone)
                .GetFinal();
        }
    }
}
