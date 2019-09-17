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

using System.Collections.Generic;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Domain.Model.Models.V2.Channel;

namespace PTV.Database.DataAccess.Translators.Channels
{
/* SOTE has been disabled (SFIPTV-1177)    
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmJsonSoteServiceLocation>), RegisterType.Transient)]
    internal class ServiceChannelSoteTranslator : Translator<ServiceChannelVersioned, VmJsonSoteServiceLocation>
    {
        
        private readonly ITypesCache typesCache;
        
        public ServiceChannelSoteTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) 
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmJsonSoteServiceLocation TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            throw new System.NotImplementedException("Translator ServiceChannelVersioned -> VmJsonSoteServiceLocation is not implemented.");
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmJsonSoteServiceLocation vModel)
        {
            if (!vModel.OrganizationId.IsAssigned()) return null;
            
            var channelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
            var extraTypeId = typesCache.Get<ExtraType>(ExtraTypeEnum.Sote.ToString());
            var connectionTypeId = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.NotCommon.ToString());
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            
            var translation = CreateViewModelEntityDefinition(vModel)
                .DefineEntitySubTree(i => i.Include(p => p.Phones).ThenInclude(p => p.Phone).ThenInclude(et => et.ExtraTypes))
                .UseDataContextCreate(i => !i.Id.IsAssigned())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(), input => output => input.Id == output.Id)
                .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                .AddLanguageAvailability(i => i, o => o)
                .AddSimple(i => channelTypeId, o => o.TypeId)
                .AddLocalizable(i => new VmDispalyNameType{NameTypeId = nameTypeId}, o => o.DisplayNameTypes)
                .AddSimple(i => connectionTypeId, o => o.ConnectionTypeId)
                .AddSimple(i => i.OrganizationId, o => o.OrganizationId);

            translation.AddCollectionWithRemove(i => i.ContactInfo == null || i.ContactInfo.PhoneNumber.IsNullOrEmpty()
                    ? new List<VmJsonSoteServiceLocation>()
                    : new List<VmJsonSoteServiceLocation> {i}
                , o => o.Phones,
                x => x.Phone.ExtraTypes.Select(t => t.ExtraTypeId).Contains(extraTypeId));
            
            if (vModel.ContactInfo?.Address != null)
            {
                translation.AddCollectionWithRemove(i => i.ContactInfo?.Address == null
                        ? new List<VmJsonSoteServiceLocation>()
                        : new List<VmJsonSoteServiceLocation> {i}
                    , o => o.Addresses,
                    x => x.Address.ExtraTypes.Select(t => t.ExtraTypeId).Contains(extraTypeId));
            }
            
            // translate names
            var names = new List<VmName> { new VmName {Name = vModel.Name, TypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString())} };
            if (!string.IsNullOrEmpty(vModel.AlternateName))
            {
                names.Add(new VmName {Name = vModel.AlternateName, TypeId = typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString())});
            }
            translation.Propagation((i, o) =>
            {
                names.ForEach(n =>
                {
                    n.LocalizationId = i.LocalizationId;
                    n.OwnerReferenceId = o.Id;
                });
            });
            translation.AddCollectionWithKeep(i => names, o => o.ServiceChannelNames, r => true);

            // translate oid
            var vmChannelHeader = new VmChannelHeader{Oid = vModel.Oid};
            translation.Propagation((i, o) => { vmChannelHeader.UnificRootId = o.UnificRootId; });
            translation.AddNavigation(i => vmChannelHeader, o => o.UnificRoot);

            return translation.GetFinal();
        }
    }
*/    
}