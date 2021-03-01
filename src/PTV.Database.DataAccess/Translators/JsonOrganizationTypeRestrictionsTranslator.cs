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

using System;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Models.Import;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;
using PTV.Framework.Interfaces;


namespace PTV.Database.DataAccess.Translators
{
    [RegisterService(typeof(ITranslator<Model.Models.RestrictionFilter, VmJsonOrganizationTypeRestriction>), RegisterType.Scope)]
    internal class JsonOrganizationTypeRestrictionsTranslator : Translator<Model.Models.RestrictionFilter, VmJsonOrganizationTypeRestriction>
    {
        public JsonOrganizationTypeRestrictionsTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmJsonOrganizationTypeRestriction TranslateEntityToVm(RestrictionFilter entity)
        {
            throw new NotImplementedException();
        }

        public override RestrictionFilter TranslateVmToEntity(VmJsonOrganizationTypeRestriction vModel)
        {
            var definition = CreateViewModelEntityDefinition<Model.Models.RestrictionFilter>(vModel)
                .UseDataContextUpdate(i => true, i => o => vModel.FilterName == o.FilterName, def => def.UseDataContextCreate(x => true, o => o.Id,
                i => Guid.NewGuid()));

            var entity = definition.GetFinal();
            vModel.Organizations.ForEach(x => x.RestrictionTypeId = entity.Id);
            return definition
                .AddNavigation(i => i.FilterName, o => o.FilterName)
                .AddSimple(i => Enum.Parse<ERestrictionFilterType>(i.FilterType, true), o => o.FilterType)
                .AddNavigation(i => i.EntityColumnName, o => o.ColumnName)
                .AddNavigation(i => i.EntityType, o => o.EntityType)
                .AddNavigation(i => new VmRestrictedType { TypeName = i.TypeName, Value = i.TypeValue},  o => o.RestrictedType)
                .AddCollection(i => i.Organizations, o => o.OrganizationFilters)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<Model.Models.OrganizationFilter, VmJsonOrganizationTypeRelation>), RegisterType.Scope)]
    internal class JsonOrganizationsTypeRelationTranslator : Translator<Model.Models.OrganizationFilter, VmJsonOrganizationTypeRelation>
    {
        public JsonOrganizationsTypeRelationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmJsonOrganizationTypeRelation TranslateEntityToVm(OrganizationFilter entity)
        {
            throw new NotImplementedException();
        }

        public override OrganizationFilter TranslateVmToEntity(VmJsonOrganizationTypeRelation vModel)
        {
            return CreateViewModelEntityDefinition<Model.Models.OrganizationFilter>(vModel)
                .AddSimple(i => i.RestrictionTypeId, o => o.FilterId)
                .AddSimple(i => i.Id, o => o.OrganizationId)
                .GetFinal();
        }
    }


    [RegisterService(typeof(ITranslator<RestrictionFilter, VmRestrictionFilter>),RegisterType.Scope)]
    internal class RestrictionFilterTranslator : Translator<RestrictionFilter, VmRestrictionFilter>
    {
        public RestrictionFilterTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmRestrictionFilter TranslateEntityToVm(RestrictionFilter entity)
        {
            var result = CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.FilterName, o=> o.FilterName)
                .AddNavigation(i => i.ColumnName, o=> o.ColumnName)
                .AddCollection(i => i.OrganizationFilters, o => o.OrganizationFilters)
                .AddNavigation(i => i.RestrictedType, o => o.RestrictedType)
                //.AddSimple(i => i.FilterType, o => o.FilterType)
                .AddSimple(i => i.BlockOtherTypes, o => o.BlockOtherTypes)
                .GetFinal();
            result.FilterType = (EVmRestrictionFilterType)(int)entity.FilterType;
            return result;
        }

        public override RestrictionFilter TranslateVmToEntity(VmRestrictionFilter vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<OrganizationFilter, VmOrganizationFilter>),RegisterType.Scope)]
    internal class OrganizationFilterTranslator : Translator<OrganizationFilter, VmOrganizationFilter>
    {
        public OrganizationFilterTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOrganizationFilter TranslateEntityToVm(OrganizationFilter entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.OrganizationId, o => o.OrganizationId)
                .GetFinal();
        }

        public override OrganizationFilter TranslateVmToEntity(VmOrganizationFilter vModel)
        {
            throw new NotImplementedException();
        }
    }


    [RegisterService(typeof(ITranslator<RestrictedType, VmRestrictedType>), RegisterType.Scope)]
    internal class RestrictedTypeFilterTranslator : Translator<RestrictedType, VmRestrictedType>
    {
        public RestrictedTypeFilterTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmRestrictedType TranslateEntityToVm(RestrictedType entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.TypeName, o=> o.TypeName)
                .AddSimple(i => i.Value, o => o.Value)
                .GetFinal();
        }

        public override RestrictedType TranslateVmToEntity(VmRestrictedType vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .AddNavigation(i => i.TypeName, o => o.TypeName)
                .AddSimple(i => i.Value, o => o.Value)
                .GetFinal();
        }
    }
}
