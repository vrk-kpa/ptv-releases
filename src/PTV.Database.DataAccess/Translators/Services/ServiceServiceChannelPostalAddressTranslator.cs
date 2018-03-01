using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using PTV.Framework.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannelAddress, VmAddressSimple>), RegisterType.Transient)]
    internal class ServiceServiceChannelPostalAddressTranslator : Translator<ServiceServiceChannelAddress, VmAddressSimple>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;
        public ServiceServiceChannelPostalAddressTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmAddressSimple TranslateEntityToVm(ServiceServiceChannelAddress entity)
        {
            var result = CreateEntityViewModelDefinition(entity)
               .AddPartial(i => i.Address)
               .GetFinal();
            return result;
        }

        public override ServiceServiceChannelAddress TranslateVmToEntity(VmAddressSimple vModel)
        {
            if (vModel == null) return null;
            bool existsById = vModel.Id.IsAssigned();
            //var existsByOwnerReference = !vModel.Id.IsAssigned() && vModel.OwnerReferenceId.IsAssigned();

            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !existsById /*&& !existsByOwnerReference*/)
                .UseDataContextUpdate(
                    input => existsById /*|| existsByOwnerReference*/,
                    input => output => (!input.Id.IsAssigned() || input.Id == output.AddressId) &&
                                       (!input.OwnerReferenceId.IsAssigned() || output.ServiceServiceChannel.ServiceId == input.OwnerReferenceId) &&
                                       (!input.OwnerReferenceId2.IsAssigned() || output.ServiceServiceChannel.ServiceChannelId == input.OwnerReferenceId2),
                    create => create.UseDataContextCreate(c => true)
                );

           /* if (existsByOwnerReference)
            {
                var serviceChannelAddress = translation.GetFinal();
                if (serviceChannelAddress.Created > DateTime.MinValue)
                {
                    vModel.Id = serviceChannelAddress.AddressId;
                }
            }*/
            vModel.AddressCharacter = AddressCharacterEnum.Postal;
            var result = translation
                .AddNavigation(input => input, output => output.Address)
                .AddSimple(
                    input => typesCache.Get<AddressCharacter>(AddressCharacterEnum.Postal.ToString()),
                    output => output.CharacterId
                )
                .GetFinal();
            return result;
        }
    }
}
