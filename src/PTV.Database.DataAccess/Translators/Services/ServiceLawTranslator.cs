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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceLaw, VmLaw>), RegisterType.Transient)]
    internal class ServiceLawTranslator : Translator<ServiceLaw, VmLaw>
    {
        public ServiceLawTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmLaw TranslateEntityToVm(ServiceLaw entity)
        {;
            throw new NotImplementedException();
        }

        public override ServiceLaw TranslateVmToEntity(VmLaw vModel)
        {
            var translationDefinition = CreateViewModelEntityDefinition<ServiceLaw>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => true)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                .AddNavigation(input => input, output => output.Law);

            var entity = translationDefinition.GetFinal();
            return entity;
        }

    }
}
