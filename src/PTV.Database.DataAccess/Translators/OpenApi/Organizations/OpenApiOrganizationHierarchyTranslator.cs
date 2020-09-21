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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Domain.Model.Models.OpenApi;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmOpenApiOrganizationHierarchy>), RegisterType.Transient)]
    internal class OpenApiOrganizationHierarchyTranslator : Translator<OrganizationVersioned, VmOpenApiOrganizationHierarchy>
    {
        public OpenApiOrganizationHierarchyTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiOrganizationHierarchy TranslateEntityToVm(OrganizationVersioned entity)
        {
            if (entity == null) return null;

            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddCollection(i => i.OrganizationNames, o => o.OrganizationNames)
                .AddNavigation(i => i.Parent, o => o.Parent)
                .AddCollection(i => i.UnificRoot?.Children, o => o.SubOrganizations)
                .GetFinal();
        }

        public override OrganizationVersioned TranslateVmToEntity(VmOpenApiOrganizationHierarchy vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiOrganizationHierarchyTranslator");
        }
    }

    [RegisterService(typeof(ITranslator<Organization, VmOpenApiOrganizationParent>), RegisterType.Transient)]
    internal class OpenApiOrganizationParentTranslator
        : Translator<Organization, VmOpenApiOrganizationParent>
    {
        public OpenApiOrganizationParentTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiOrganizationParent TranslateEntityToVm(Organization entity)
        {
            if (entity == null) return null;

            var organizationVersioned = entity.Versions.FirstOrDefault();

            if (organizationVersioned == null) return null;

            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => organizationVersioned.UnificRootId, o => o.Id)
                .AddCollection(i => organizationVersioned.OrganizationNames, o => o.OrganizationNames)
                .AddNavigation(i => organizationVersioned.Parent, o => o.Parent)
                .GetFinal();
        }

        public override Organization TranslateVmToEntity(VmOpenApiOrganizationParent vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiOrganizationHierarchyTranslator");
        }
    }

    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmOpenApiOrganizationSub>), RegisterType.Transient)]
    internal class OpenApiSubOrganizationTranslator
        : Translator<OrganizationVersioned, VmOpenApiOrganizationSub>
    {

        public OpenApiSubOrganizationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiOrganizationSub TranslateEntityToVm(OrganizationVersioned entity)
        {
            if (entity == null) return null;

            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddCollection(i => i.OrganizationNames, o => o.OrganizationNames)
                .AddCollection(i => i.UnificRoot?.Children, o => o.SubOrganizations)
                .GetFinal();
        }

        public override OrganizationVersioned TranslateVmToEntity(VmOpenApiOrganizationSub vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiOrganizationHierarchyTranslator");
        }
    }
}
