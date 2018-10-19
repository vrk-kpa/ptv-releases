﻿/**
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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services.V2
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmAreaInformation>), RegisterType.Transient)]
    internal class OrganizationAreaTranslator : Translator<OrganizationVersioned, VmAreaInformation>
    {
        private ITypesCache typesCache;
        public OrganizationAreaTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = cacheManager.TypesCache;
        }

        public override VmAreaInformation TranslateEntityToVm(OrganizationVersioned entity)
        {
            var defaultAreaType = entity.OrganizationAreaMunicipalities.Any() 
                ? typesCache.Get<AreaType>(AreaTypeEnum.Municipality.ToString())
                : entity.OrganizationAreas.FirstOrDefault()?.Area.AreaTypeId;

            return CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddSimple(input => defaultAreaType, output => output.AreaType)
                .AddSimple(input => input.AreaInformationTypeId, output => output.AreaInformationTypeId)
                .AddSimpleList(input => input.OrganizationAreaMunicipalities.Select(x => x.MunicipalityId), output => output.Municipalities)
                .AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString())).Select(x => x.AreaId), output => output.Provinces)
                .AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString())).Select(x => x.AreaId), output => output.BusinessRegions)
                .AddSimpleList(input => input.OrganizationAreas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString())).Select(x => x.AreaId), output => output.HospitalRegions)
                .GetFinal();
        }

        public override OrganizationVersioned TranslateVmToEntity(VmAreaInformation vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .AddSimple(i => i.AreaInformationTypeId, o => o.AreaInformationTypeId);

            if (vModel != null 
                && typesCache.Compare<AreaInformationType>(vModel.AreaInformationTypeId, AreaInformationTypeEnum.AreaType.ToString())
                && !typesCache.Compare<OrganizationType>(vModel.OrganizationType, OrganizationTypeEnum.Municipality.ToString()))
            {
                definition
                    .AddCollection(
                        i => i.BusinessRegions.Union(i.HospitalRegions).Union(i.Provinces).Select(x => new VmListItem
                        {
                            Id = x,
                            OwnerReferenceId = i.OwnerReferenceId
                        }), o => o.OrganizationAreas, TranslationPolicy.FetchData)
                    .AddCollection(
                        i => i.Municipalities.Select(x => new VmListItem
                        {
                            Id = x,
                            OwnerReferenceId = i.OwnerReferenceId
                        }), o => o.OrganizationAreaMunicipalities, TranslationPolicy.FetchData);
            }
            else
            {
                definition.AddCollection(i => new List<VmListItem>(), o => o.OrganizationAreas, TranslationPolicy.FetchData);
                definition.AddCollection(i => new List<VmListItem>(), o => o.OrganizationAreaMunicipalities, TranslationPolicy.FetchData);
            }
            return definition.GetFinal();
        }
    }
}
