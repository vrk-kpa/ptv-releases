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
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<Service, V11VmOpenApiServiceAndChannelRelationAstiInBase>), RegisterType.Transient)]
    internal class OpenApiServiceConnectionInTranslator : Translator<Service, V11VmOpenApiServiceAndChannelRelationAstiInBase>
    {
        public OpenApiServiceConnectionInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override V11VmOpenApiServiceAndChannelRelationAstiInBase TranslateEntityToVm(Service entity)
        {
            throw new NotImplementedException();
        }

        public override Service TranslateVmToEntity(V11VmOpenApiServiceAndChannelRelationAstiInBase vModel)
        {
            var definition = CreateViewModelEntityDefinition<Service>(vModel)
                    .UseDataContextUpdate(i => true, i => o => i.ServiceId == o.Id);

            if (vModel.DeleteAllChannelRelations || vModel.ChannelRelations?.Count > 0)
            {
                // The connection collection needs to be merged and missing connections removed manually (outside the translator)
                // because we have both regular and ASTI connections.
                // ASTI users can only remove ASTI connection - regular users can only remove regular connections.
                // More rules in https://confluence.csc.fi/display/PAL/ASTI-project and https://jira.csc.fi/browse/PTV-2065.
                definition.AddCollection(i => i.ChannelRelations, o => o.ServiceServiceChannels, true);
            }
            return definition.GetFinal();
        }
    }
}
