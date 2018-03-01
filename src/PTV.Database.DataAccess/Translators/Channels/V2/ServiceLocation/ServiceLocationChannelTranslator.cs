using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels.V2.ServiceLocation
{
    [RegisterService(typeof(ITranslator<ServiceLocationChannel, VmServiceLocationChannel>), RegisterType.Transient)]
    internal class ServiceLocationChannelTranslator : Translator<ServiceLocationChannel, VmServiceLocationChannel>
    {
        private ILanguageCache languageCache;
        public ServiceLocationChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override VmServiceLocationChannel TranslateEntityToVm(ServiceLocationChannel entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddCollection(input => input.Addresses.Where(x => x.Character.Code == AddressCharacterEnum.Visiting.ToString()).Select(x => x.Address).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.VisitingAddresses)
                .AddCollection(input => input.Addresses.Where(x => x.Character.Code == AddressCharacterEnum.Postal.ToString()).Select(x => x.Address).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.PostalAddresses)
                .GetFinal();
        }

        public override ServiceLocationChannel TranslateVmToEntity(VmServiceLocationChannel vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.ServiceChannelVersionedId)
                .Propagation((vm, sl) =>
                    {
                        var order = 1;
                        vModel.VisitingAddresses.ForEach(item =>
                        {
                            item.AddressCharacter = AddressCharacterEnum.Visiting;
                            item.OwnerReferenceId = sl.Id;
                            item.OrderNumber = order++;
                        });
                        order = 1;
                        vModel.PostalAddresses.ForEach(item =>
                        {
                            item.AddressCharacter = AddressCharacterEnum.Postal;
                            item.OwnerReferenceId = sl.Id;
                            item.OrderNumber = order++;
                        });
                    })
                ;
            definition.AddCollectionWithRemove(i => i.VisitingAddresses.Concat(i.PostalAddresses), o => o.Addresses, x => true);
            var entity = definition.GetFinal();
            return entity;
        }

    }
}