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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceHoursAdditionalInformation, VmStringText>), RegisterType.Transient)]
    internal class ServiceHourAdditionalInformationTranslator : Translator<ServiceHoursAdditionalInformation, VmStringText>
    {
        public ServiceHourAdditionalInformationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmStringText TranslateEntityToVm(ServiceHoursAdditionalInformation entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceHoursAdditionalInformation TranslateVmToEntity(VmStringText vModel)
        {
            return CreateViewModelEntityDefinition<ServiceHoursAdditionalInformation>(vModel)
                .UseDataContextLocalizedUpdate(input => input.Id.IsAssigned(), input => output => output.ServiceChannelServiceHoursId == input.Id, def => def.UseDataContextCreate(i => true))
                .AddNavigation(input => input.Text, output => output.Text)
                .AddRequestLanguage(output => output)
                .GetFinal();
        }
    }
}
