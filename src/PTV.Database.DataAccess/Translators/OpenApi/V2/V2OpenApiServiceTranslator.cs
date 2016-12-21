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
using System;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.V2
{
    [RegisterService(typeof(ITranslator<V2VmOpenApiService, VmOpenApiService>), RegisterType.Transient)]
    internal class V2OpenApiServiceTranslator : Translator<V2VmOpenApiService, VmOpenApiService>
    {
        public V2OpenApiServiceTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiService TranslateEntityToVm(V2VmOpenApiService entity)
        {
            if (entity == null)
            {
                return null;
            }

            var vm = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Type, o => o.Type)
                .AddSimple(i => i.StatutoryServiceGeneralDescriptionId, o => o.StatutoryServiceGeneralDescriptionId)
                .AddNavigation(i => i.ServiceChargeType, o => o.ServiceChargeType)
                .AddNavigation(i => i.ServiceCoverageType, o => o.ServiceCoverageType)
                .AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus)
                .GetFinal();

            vm.ServiceNames = entity.ServiceNames;
            vm.ServiceDescriptions = entity.ServiceDescriptions;
            vm.Languages = entity.Languages;
            vm.Keywords = entity.Keywords.Select(k => k.Value).ToList();
            vm.Municipalities = entity.Municipalities;
            vm.Requirements = entity.Requirements;
            vm.ServiceAdditionalInformations = entity.ServiceAdditionalInformations;
            vm.ServiceClasses = entity.ServiceClasses;
            vm.OntologyTerms = entity.OntologyTerms;
            vm.TargetGroups = entity.TargetGroups;
            vm.LifeEvents = entity.LifeEvents;
            vm.IndustrialClasses = entity.IndustrialClasses;
            vm.ServiceChannels = entity.ServiceChannels.Select(s => s.ServiceChannelId).ToList();
            vm.Organizations = entity.Organizations;

            return vm;
        }

        public override V2VmOpenApiService TranslateVmToEntity(VmOpenApiService vModel)
        {
            throw new NotImplementedException();
        }
    }
}
