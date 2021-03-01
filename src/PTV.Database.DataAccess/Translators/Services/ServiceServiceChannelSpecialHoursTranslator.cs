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
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannelServiceHours, VmSpecialHours>), RegisterType.Transient)]
    internal class ServiceServiceChannelSpecialHoursTranslator : Translator<ServiceServiceChannelServiceHours, VmSpecialHours>
    {
        public ServiceServiceChannelSpecialHoursTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
        }

        public override VmSpecialHours TranslateEntityToVm(ServiceServiceChannelServiceHours entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannelServiceHours TranslateVmToEntity(VmSpecialHours vModel)
        {
            if (vModel == null) return null;
            bool existsById = vModel.Id.IsAssigned();

            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !existsById)
                .UseDataContextUpdate(
                    input => existsById,
                    input => output => (!input.Id.IsAssigned() || input.Id == output.ServiceHoursId) &&
                                       (!input.OwnerReferenceId.IsAssigned() || output.ServiceServiceChannel.ServiceId == input.OwnerReferenceId) &&
                                       (!input.OwnerReferenceId2.IsAssigned() || output.ServiceServiceChannel.ServiceChannelId == input.OwnerReferenceId2),
                    create => create.UseDataContextCreate(c => true)
                );

            var result = translation
                .AddNavigation(input => input, output => output.ServiceHours)
                .GetFinal();
            return result;
        }
    }
}
