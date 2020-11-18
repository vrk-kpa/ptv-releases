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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{

    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmConnection>), RegisterType.Transient)]
    internal class ServiceServiceChannelConnectionTranslator : Translator<ServiceServiceChannel, VmConnection>
    {
        public ServiceServiceChannelConnectionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmConnection TranslateEntityToVm(ServiceServiceChannel entity)
        {
            return CreateEntityViewModelDefinition(entity)
              .AddNavigation(i => i.ServiceChannelId.ToString() + i.ServiceId.ToString(), o => o.Id)
              .AddSimple(i => entity.RequestedForServiceChannel ?? VersioningManager.ApplyPublishingStatusFilterFallback(i.ServiceChannel.Versions)?.Id ?? Guid.Empty, o => o.ChannelId)
              .AddSimple(i => VersioningManager.ApplyPublishingStatusFilterFallback(i.Service.Versions)?.Id ?? Guid.Empty, o => o.ServiceId)
              .AddSimple(i => i.Modified, o => o.Modified)
              .AddNavigation(i => i.ModifiedBy, o => o.ModifiedBy)
              .GetFinal();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmConnection vModel)
        {
            throw new NotImplementedException();
        }
    }
}
