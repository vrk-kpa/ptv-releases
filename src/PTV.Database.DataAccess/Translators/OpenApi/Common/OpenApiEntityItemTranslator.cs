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
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<EntityBase, VmOpenApiEntityItem>), RegisterType.Transient)]
    internal class OpenApiEntityItemTranslator : Translator<EntityBase, VmOpenApiEntityItem>
    {
        public OpenApiEntityItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        { }

        public override VmOpenApiEntityItem TranslateEntityToVm(EntityBase entity)
        {
            if (entity is ServiceVersioned)
            {
                var service = entity as ServiceVersioned;
                return CreateEntityViewModelDefinition(service)
                    .AddSimple(i => service.UnificRootId, o => o.Id)
                    .AddNavigation(i => typeof(Service).Name, o => o.Type)
                    .AddSimple(i => i.Modified, o => o.Modified)
                    .GetFinal();
            }
            else if (entity is ServiceChannelVersioned)
            {
                var serviceChannel = entity as ServiceChannelVersioned;
                return CreateEntityViewModelDefinition(serviceChannel)
                    .AddSimple(i => serviceChannel.UnificRootId, o => o.Id)
                    .AddNavigation(i => typeof(ServiceChannel).Name, o => o.Type)
                    .AddSimple(i => i.Modified, o => o.Modified)
                    .GetFinal();
            }
            else if (entity is OrganizationVersioned)
            {
                var organization = entity as OrganizationVersioned;
                return CreateEntityViewModelDefinition(organization)
                    .AddSimple(i => organization.UnificRootId, o => o.Id)
                    .AddNavigation(i => typeof(Organization).Name, o => o.Type)
                    .AddSimple(i => i.Modified, o => o.Modified)
                    .GetFinal();
            }
            return null;
        }

        public override EntityBase TranslateVmToEntity(VmOpenApiEntityItem vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiEntityItemTranslator!");
        }
    }
}
