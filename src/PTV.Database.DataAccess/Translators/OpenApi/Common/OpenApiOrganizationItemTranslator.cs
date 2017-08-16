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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<Organization, VmOpenApiItem>), RegisterType.Transient)]
    internal class OpenApiOrganizationItemTranslator : Translator<Organization, VmOpenApiItem>
    {
        private readonly ITypesCache typesCache;

        public OpenApiOrganizationItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiItem TranslateEntityToVm(Organization entity)
        {
            var definition = CreateEntityViewModelDefinition<VmOpenApiItem>(entity);

            if (entity != null && entity.Versions?.Count > 0)
            {
                var publishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                definition.AddPartial(i => i.Versions.FirstOrDefault(x => x.PublishingStatusId == publishedId), o => o);
            }
            else if (entity != null && entity.Id.IsAssigned())
            {
                var organization = new OrganizationVersioned { UnificRootId = entity.Id };
                definition.AddPartial(i => organization, o => o);
            }
            else
            {
                definition.AddPartial(i => (OrganizationVersioned)null, o => o);
            }

            return definition.GetFinal();
        }

        public override Organization TranslateVmToEntity(VmOpenApiItem vModel)
        {
            throw new NotImplementedException("Translator VmOpenApiItem -> Organization is not implemented.");
        }
    }
}
