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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannel, VmChannelListItem>), RegisterType.Transient)]
    internal class ChannelListItemTranslator : Translator<ServiceChannel, VmChannelListItem>
    {
        private ITypesCache typesCache;
        public ChannelListItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmChannelListItem TranslateEntityToVm(ServiceChannel entity)
        {
            var definition = CreateEntityViewModelDefinition<VmChannelListItem>(entity)
               .AddLocalizable(i => i.ServiceChannelNames.Where(k => k.Type.Code == NameTypeEnum.Name.ToString()), o => o.Name)
               .AddNavigation(i => i.Type as TypeBase, o => o.Type)
               .AddSimple(i => i.PublishingStatusId ?? Guid.Empty, o => o.PublishingStatus)
               .AddLocalizable(i => i.Phones.Select(x => x.Phone), o => o.PhoneNumber)
               .AddNavigation(i => i.PrintableFormChannels?.FirstOrDefault()?.FormIdentifier, o => o.FormIdentifier)
               .AddSimple(i => i.ServiceServiceChannels.Count(x => x.Service.PublishingStatusId != typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString())), o => o.ConnectedServices)
               .AddSimple(i => i.Modified, o => o.Modified)
               .AddSimple(i => i.Id, o => o.Id);
            var phone = definition.GetFinal();

            if (phone.PhoneNumber == null)
            {
                definition.AddNavigation(i => i.Phones.Select(x => x.Phone).FirstOrDefault(), o => o.PhoneNumber);
            }

            return phone;
        }

        public override ServiceChannel TranslateVmToEntity(VmChannelListItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
