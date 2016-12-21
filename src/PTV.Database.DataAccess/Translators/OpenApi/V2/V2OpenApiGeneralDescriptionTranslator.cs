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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.V2
{
    [RegisterService(typeof(ITranslator<V2VmOpenApiGeneralDescription, VmOpenApiGeneralDescription>), RegisterType.Transient)]
    internal class V2OpenApiGeneralDescriptionTranslator : Translator<V2VmOpenApiGeneralDescription, VmOpenApiGeneralDescription>
    {
        public V2OpenApiGeneralDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiGeneralDescription TranslateEntityToVm(V2VmOpenApiGeneralDescription entity)
        {
            var vm = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Type, o => o.Type)
                .AddNavigation(i => i.ServiceChargeType, o => o.ServiceChargeType)
                .GetFinal();

            vm.Names = entity.Names;
            vm.Descriptions = entity.Descriptions;
            vm.Languages = entity.Languages;
            vm.ServiceClasses = entity.ServiceClasses;
            vm.OntologyTerms = entity.OntologyTerms;
            vm.TargetGroups = entity.TargetGroups;
            vm.LifeEvents = entity.LifeEvents;
            vm.IndustrialClasses = entity.IndustrialClasses;
            vm.Requirements = entity.Requirements;
            vm.Laws = entity.Laws;
            return vm;
        }

        public override V2VmOpenApiGeneralDescription TranslateVmToEntity(VmOpenApiGeneralDescription vModel)
        {
            var vm = CreateViewModelEntityDefinition(vModel)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Type, o => o.Type)
                .AddNavigation(i => i.ServiceChargeType, o => o.ServiceChargeType)
                .GetFinal();

            vm.Names = vModel.Names;
            vm.Descriptions = vModel.Descriptions;
            vm.Languages = vModel.Languages;
            vm.ServiceClasses = vModel.ServiceClasses;
            vm.OntologyTerms = vModel.OntologyTerms;
            vm.TargetGroups = vModel.TargetGroups;
            vm.LifeEvents = vModel.LifeEvents;
            vm.IndustrialClasses = vModel.IndustrialClasses;
            vm.Requirements = vModel.Requirements;
            vm.Laws = vModel.Laws;
            return vm;
        }
    }
}
