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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmChannelListItem>), RegisterType.Transient)]
    internal class ChannelListItemTranslator : Translator<ServiceChannelVersioned, VmChannelListItem>
    {
        private ITypesCache typesCache;
        private ILanguageOrderCache languageOrderCache;

        public ChannelListItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache, ILanguageOrderCache languageOrderCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
            this.languageOrderCache = languageOrderCache;
        }

        public override VmChannelListItem TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition<VmChannelListItem>(entity)
               .AddNavigation(i => VersioningManager.ApplyLanguageFilterFallback(i.ServiceChannelNames.Where(k => k.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())), RequestLanguageId)?.Name ?? "N/A", o => o.Name)
                //.AddPartial(i => i.Type, o => o as IVmEntityType)
               .AddSimple(i => EntityTypeEnum.Channel, o => o.EntityType)
               .AddNavigation(i => typesCache.GetByValue<ServiceChannelType>(i.TypeId), o => o.SubEntityType)
               .AddSimple(i => i.TypeId, o => o.TypeId)
               .AddSimple(i => i.UnificRootId, o => o.UnificRootId)
               .AddSimple(i => i.PublishingStatusId, o => o.PublishingStatusId)
               .AddLocalizable(i => i.Phones.Select(x => x.Phone), o => o.PhoneNumber)
               .AddLocalizable(i => i.PrintableFormChannels?.FirstOrDefault()?.FormIdentifiers, o => o.FormIdentifier)
               .AddSimple(i => i.Modified, o => o.Modified)
               .AddSimple(i => i.Id, o => o.Id)
               .AddSimple(i => i.UnificRootId, o => o.RootId)
               .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(i => i.LanguageAvailabilities.OrderBy(x => languageOrderCache.Get(x.LanguageId)), o => o.LanguagesAvailabilities)
               .AddNavigation(i => i.Versioning, o => o.Version)
               .AddSimple(i => i.OrganizationId, o => o.OrganizationId);

            if (entity.UnificRoot != null)
            {
                definition.AddSimple(i => i.UnificRoot.ServiceServiceChannels.Count(
                    x => x.Service.Versions.Any(y => y.PublishingStatusId != typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()))), o => o.ConnectedServices);
            }

            var phone = definition.GetFinal();
            if (phone.PhoneNumber == null)
            {
                definition.AddNavigation(i => i.Phones.Select(x => x.Phone).FirstOrDefault(), o => o.PhoneNumber);
            }

            return phone;
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmChannelListItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
