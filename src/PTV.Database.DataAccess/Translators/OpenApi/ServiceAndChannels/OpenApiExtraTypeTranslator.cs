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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannelExtraType, VmOpenApiExtraType>), RegisterType.Transient)]
    internal class OpenApiExtraTypeTranslator : Translator<ServiceServiceChannelExtraType, VmOpenApiExtraType>
    {
        private readonly ITypesCache typesCache;

        public OpenApiExtraTypeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiExtraType TranslateEntityToVm(ServiceServiceChannelExtraType entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.ServiceId, o => o.ServiceGuid)
                .AddSimple(i => i.ServiceChannelId, o => o.ChannelGuid)
                .AddPartial(i => i.ExtraSubType)
                .AddCollection(i => i.ServiceServiceChannelExtraTypeDescriptions, o => o.Description)
                .GetFinal();
        }

        public override ServiceServiceChannelExtraType TranslateVmToEntity(VmOpenApiExtraType vModel)
        {
            var codeId = typesCache.Get<ExtraSubType>(vModel.Code);
            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(i => true, i => o => i.ChannelGuid == o.ServiceChannelId && i.ServiceGuid == o.ServiceId &&
                    codeId == o.ExtraSubTypeId, def => def.UseDataContextCreate(i => true))
                .AddSimple(i => i.ServiceGuid, o => o.ServiceId)
                .AddSimple(i => i.ChannelGuid, o => o.ServiceChannelId)
                .AddSimple(i => codeId, o => o.ExtraSubTypeId);


            if (vModel.Description?.Count > 0)
            {
                var entity = definition.GetFinal();
                if (entity.Created != DateTime.MinValue)
                {
                    // We are updating existing entity
                    vModel.Description.ForEach(d =>
                    {
                        d.OwnerReferenceId = entity.Id;
                    });
                }
                definition.AddCollection(i => i.Description, o => o.ServiceServiceChannelExtraTypeDescriptions, false);
            }

            return definition.GetFinal();
        }
    }
}
