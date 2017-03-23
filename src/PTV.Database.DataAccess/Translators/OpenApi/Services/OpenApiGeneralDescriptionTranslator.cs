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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>), RegisterType.Transient)]
    internal class OpenApiGeneralDescriptionTranslator : OpenApiGeneralDescriptionBaseTranslator<VmOpenApiGeneralDescriptionVersionBase>
    {
        public OpenApiGeneralDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManage) : base(resolveManager, translationPrimitives, cacheManage)
        {
        }

        public override VmOpenApiGeneralDescriptionVersionBase TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            if (entity == null)
            {
                return null;
            }

            return base.CreateBaseEntityVmDefinitions(entity)
                // We have to use unique root id as id!
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddCollection(i => i.ServiceClasses.Select(j => j.ServiceClass).ToList(), o => o.ServiceClasses)
                .AddCollection(i => i.OntologyTerms.Select(j => j.OntologyTerm).ToList(), o => o.OntologyTerms)
                .AddCollection(i => i.TargetGroups.Select(j => j.TargetGroup).ToList(), o => o.TargetGroups)
                .AddCollection(i => i.LifeEvents.Select(j => j.LifeEvent).ToList(), o => o.LifeEvents)
                .AddCollection(i => i.IndustrialClasses.Select(j => j.IndustrialClass).ToList(), o => o.IndustrialClasses)
                .GetFinal();
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(VmOpenApiGeneralDescriptionVersionBase vModel)
        {
            throw new NotImplementedException();
        }
    }
}
