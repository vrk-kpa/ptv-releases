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
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmAreaInformation>), RegisterType.Transient)]
    internal class OrganizationAreaInformationTranslator : Translator<OrganizationVersioned, VmAreaInformation>
    {
        private ITypesCache typesCache;
        public OrganizationAreaInformationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmAreaInformation TranslateEntityToVm(OrganizationVersioned entity)
        {
            var info = CreateEntityViewModelDefinition<VmAreaInformation>(entity)
                .AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString())).Select(x => x.AreaId), output => output.AreaBusinessRegions)
                .AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString())).Select(x => x.AreaId), output => output.AreaHospitalRegions)
                .AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString())).Select(x => x.AreaId), output => output.AreaProvince);

            if (entity.TypeId == typesCache.Get<OrganizationType>(OrganizationTypeEnum.Municipality.ToString()) &&
                entity.MunicipalityId.IsAssigned())
            {
                info.AddSimpleList(input => new List<Guid>() { input.MunicipalityId.Value }, output => output.AreaMunicipality);
                info.AddSimple(input => typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString()), output => output.AreaInformationTypeId);
            }
            else
            {
                info.AddSimpleList(input => input.OrganizationAreaMunicipalities.Select(x => x.MunicipalityId), output => output.AreaMunicipality);
                info.AddSimple(input => input.AreaInformationTypeId, output => output.AreaInformationTypeId);
            }

            return info.GetFinal();
        }

        public override OrganizationVersioned TranslateVmToEntity(VmAreaInformation vModel)
        {
            throw new NotSupportedException();
        }

    }
}
