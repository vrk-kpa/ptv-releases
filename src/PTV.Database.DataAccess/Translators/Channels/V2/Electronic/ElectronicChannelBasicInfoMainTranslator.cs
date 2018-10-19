using System;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using PTV.Framework.Interfaces;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.Channels.V2
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmElectronicChannel>), RegisterType.Transient)]
    internal class ElectronicChannelBasicInfoMainTranslator : Translator<ServiceChannelVersioned, VmElectronicChannel>
    {
        private readonly ServiceChannelTranslationDefinitionHelper definitionHelper;
        private ITypesCache typesCache;
        public ElectronicChannelBasicInfoMainTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ServiceChannelTranslationDefinitionHelper definitionHelper, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
            typesCache = cacheManager.TypesCache;
        }

        public override VmElectronicChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddSimple(input => input.Id, outupt => outupt.Id)
                .AddPartial(input => input.ElectronicChannels?.FirstOrDefault(), output => output);

            definitionHelper
                .AddOpeningHoursDefinition(definition)
                .AddAttachmentsDefinition(definition, entity)
                .AddChannelBaseDefinition(definition);
            return definition.GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmElectronicChannel vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .DefineEntitySubTree(i => i.Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours))
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id)
                .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                .AddLanguageAvailability(i => i, o => o)
                .AddNavigationOneMany(i => i, o => o.ElectronicChannels)
                .AddSimple(i => typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.EChannel.ToString()), o => o.TypeId);
            definitionHelper
                .AddAttachmentsDefinition(definition, vModel, vModel.Id)
                .AddChannelBaseDefinition(definition)
                .AddOpeningHoursDefinition(definition);
            return definition.GetFinal();
        }
    }
}