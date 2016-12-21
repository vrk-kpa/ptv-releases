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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.V2
{
    [RegisterService(typeof(ITranslator<V2VmOpenApiServiceInBase, IVmOpenApiServiceInBase>), RegisterType.Transient)]
    internal class V2OpenApiServiceInTranslator : Translator<V2VmOpenApiServiceInBase, IVmOpenApiServiceInBase>
    {
        public V2OpenApiServiceInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override IVmOpenApiServiceInBase TranslateEntityToVm(V2VmOpenApiServiceInBase entity)
        {
            throw new NotImplementedException();
        }

        public override V2VmOpenApiServiceInBase TranslateVmToEntity(IVmOpenApiServiceInBase vModel)
        {
            var vm = CreateViewModelEntityDefinition(vModel)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.SourceId, o => o.SourceId)
                .AddNavigation(i => i.Type, o => o.Type)
                .AddNavigation(i => i.StatutoryServiceGeneralDescriptionId, o => o.StatutoryServiceGeneralDescriptionId)
                .AddNavigation(i => i.ServiceChargeType, o => o.ServiceChargeType)
                .AddNavigation(i => i.ServiceCoverageType, o => o.ServiceCoverageType)
                .AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus)
                .AddSimple(i => i.DeleteAllLifeEvents, o => o.DeleteAllLifeEvents)
                .AddSimple(i => i.DeleteAllIndustrialClasses, o => o.DeleteAllIndustrialClasses)
                .AddSimple(i => i.DeleteAllKeywords, o => o.DeleteAllKeywords)
                .AddSimple(i => i.DeleteAllMunicipalities, o => o.DeleteAllMunicipalities)
                .GetFinal();

            vm.ServiceNames = vModel.ServiceNames;
            vm.ServiceDescriptions = vModel.ServiceDescriptions;
            vm.Languages = vModel.Languages;
            vm.Keywords = new List<VmOpenApiLanguageItem>();
            vModel.Keywords.ForEach(k => vm.Keywords.Add(new VmOpenApiLanguageItem()
            {
                Value = k,
                Language = LanguageCode.fi.ToString()
            }));
            vm.Municipalities = vModel.Municipalities;
            vm.Requirements = vModel.Requirements;
            vm.ServiceAdditionalInformations = vModel.ServiceAdditionalInformations;
            vm.ServiceClasses = vModel.ServiceClasses;
            vm.OntologyTerms = vModel.OntologyTerms;
            vm.TargetGroups = vModel.TargetGroups;
            vm.LifeEvents = vModel.LifeEvents;
            vm.IndustrialClasses = vModel.IndustrialClasses;
            vm.ServiceOrganizations = vModel.ServiceOrganizations;
            return vm;
        }
    }
}
