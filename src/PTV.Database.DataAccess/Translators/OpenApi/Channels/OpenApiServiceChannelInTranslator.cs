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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    abstract class OpenApiServiceChannelInTranslator<TVmOpenApiServiceChannelIn> : OpenApiServiceChannelBaseTranslator<TVmOpenApiServiceChannelIn> where TVmOpenApiServiceChannelIn : class, IVmOpenApiServiceChannelIn
    {
        protected OpenApiServiceChannelInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {}

        public override TVmOpenApiServiceChannelIn TranslateEntityToVm(ServiceChannel entity)
        {
            return base.TranslateEntityToVm(entity);
        }
        public override ServiceChannel TranslateVmToEntity(TVmOpenApiServiceChannelIn vModel)
        {
            return CreateVmToChannelDefinitions(vModel)//base.TranslateVmToEntity(vModel);
                .GetFinal();
        }

        protected ITranslationDefinitions<TVmOpenApiServiceChannelIn, ServiceChannel> CreateVmToChannelDefinitions(TVmOpenApiServiceChannelIn vModel)
        {
            var definition = CreateBaseVmEntityDefinitions(vModel);

            if (!string.IsNullOrEmpty(vModel.OrganizationId))
            {
                definition.AddSimple(i => Guid.Parse(i.OrganizationId), o => o.OrganizationId);
            }
            if (vModel.ServiceChannelNames != null && vModel.ServiceChannelNames.Count > 0)
            {
                definition.AddCollection(i => i.ServiceChannelNames.Select(n => new VmOpenApiLocalizedListItem()
                { Value = n.Value, Language = n.Language, Type = NameTypeEnum.Name.ToString(), OwnerReferenceId = vModel.Id }).ToList(), o => o.ServiceChannelNames);
            }

            return definition;
        }
    }
}
