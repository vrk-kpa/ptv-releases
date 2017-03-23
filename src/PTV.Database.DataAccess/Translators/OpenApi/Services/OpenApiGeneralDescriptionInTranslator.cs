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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, IVmOpenApiGeneralDescriptionIn>), RegisterType.Transient)]
    internal class OpenApiGeneralDescriptionInTranslator : OpenApiGeneralDescriptionBaseTranslator<IVmOpenApiGeneralDescriptionIn>
    {
        public OpenApiGeneralDescriptionInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManage) : base(resolveManager, translationPrimitives, cacheManage)
        {
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(IVmOpenApiGeneralDescriptionIn vModel)
        {
            var definitions = CreateBaseVmEntityDefinitions(vModel);

            if (vModel.ServiceClasses?.Count > 0)
            {
                definitions.AddCollection(i => i.ServiceClasses, o => o.ServiceClasses);
            }

            if (vModel.OntologyTerms?.Count > 0)
            {
                definitions.AddCollection(i => i.OntologyTerms, o => o.OntologyTerms);
            }

            if (vModel.TargetGroups?.Count > 0)
            {
                definitions.AddCollection(i => i.TargetGroups, o => o.TargetGroups);
            }

            if (vModel.LifeEvents?.Count > 0)
            {
                definitions.AddCollection(i => i.LifeEvents, o => o.LifeEvents);
            }

            if (vModel.IndustrialClasses?.Count > 0)
            {
                definitions.AddCollection(i => i.IndustrialClasses, o => o.IndustrialClasses);
            }
            
            return definitions.GetFinal();
        }
    }
}
