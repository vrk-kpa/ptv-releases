using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.PrintableForm;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.Channels.V2.PrintableForm
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmPrintableFormInput>), RegisterType.Transient)]
    internal class PrintableFormInputMainTranslator : Translator<ServiceChannelVersioned, VmPrintableFormInput>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly ServiceChannelTranslationDefinitionHelper definitionHelper;

        public PrintableFormInputMainTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager,
            ServiceChannelTranslationDefinitionHelper definitionHelper
        ) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
            this.definitionHelper = definitionHelper;
        }

        public override VmPrintableFormInput TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            throw new NotImplementedException();
        }
        public override ServiceChannelVersioned TranslateVmToEntity(VmPrintableFormInput vm)
        {
            var definition = CreateViewModelEntityDefinition(vm)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id)
                .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                .AddLanguageAvailability(i => i, o => o)
                .AddSimple(i => typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString()), o => o.TypeId)
                .AddNavigationOneMany(i => i, o => o.PrintableFormChannels);
            definitionHelper
                .AddAttachmentsDefinition(definition, vm, vm.Id)
                .AddChannelBaseDefinition(definition);

            definition.Propagation((m, s) =>
            {
                var order = 1;
                vm.DeliveryAddresses?.Where(item => item != null).ForEach(item =>
                {
                    item.AddressCharacter = AddressCharacterEnum.Delivery;
                    item.OrderNumber = order++;
                    item.OwnerReferenceId = s.Id;
                });
            });
            
            definition.AddCollectionWithRemove(i => i.DeliveryAddresses, o => o.Addresses, x => true);
            
            return definition.GetFinal();
        }

        private VmName CreateName(string language, string value, VmChannelBase vModel, NameTypeEnum typeEnum)
        {
            return new VmName
            {
                Name = value,
                TypeId = typesCache.Get<NameType>(typeEnum.ToString()),
                OwnerReferenceId = vModel.Id,
                LocalizationId = languageCache.Get(language)
            };
        }

        private VmDescription CreateDescription(string language, string value, VmChannelBase vModel, DescriptionTypeEnum typeEnum)
        {
            return new VmDescription
            {
                Description = value,
                TypeId = typesCache.Get<DescriptionType>(typeEnum.ToString()),
                OwnerReferenceId = vModel.Id,
                LocalizationId = languageCache.Get(language)
            };
        }
    }
}

// AddCollection(i => i.ServiceClasses?.Select(x => new VmListItem() { Id = x, OwnerReferenceId = i.Id }), o => o.ServiceServiceClasses)