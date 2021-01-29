﻿/**
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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using VmDigitalAuthorization = PTV.Domain.Model.Models.V2.Common.Connections.VmDigitalAuthorization;

namespace PTV.Database.DataAccess.Translators.Common.V2
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannelDigitalAuthorization, VmDigitalAuthorization>), RegisterType.Transient)]
    internal class DigitalAuthorizationTranslator : Translator<ServiceServiceChannelDigitalAuthorization, VmDigitalAuthorization>
    {
        public DigitalAuthorizationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmDigitalAuthorization TranslateEntityToVm(ServiceServiceChannelDigitalAuthorization entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannelDigitalAuthorization TranslateVmToEntity(VmDigitalAuthorization vModel)
        {
            return CreateViewModelEntityDefinition<ServiceServiceChannelDigitalAuthorization>(vModel)
                .UseDataContextUpdate(i => true, i => o => (i.OwnerReferenceId == o.ServiceId) && (i.OwnerReferenceId2 == o.ServiceChannelId) && (i.Id == o.DigitalAuthorizationId), def => def.UseDataContextCreate(i => true))
                .AddSimple(input => input.Id, output => output.DigitalAuthorizationId)
                .GetFinal();
        }
    }
}
