using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using VmWebPageChannel = PTV.Domain.Model.Models.V2.Channel.VmWebPageChannel;

namespace PTV.Database.DataAccess.Translators.Channels.V2.WebPage
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmWebPageChannel>), RegisterType.Transient)]
    internal class WebPageChannelMainTranslator : Translator<ServiceChannelVersioned, VmWebPageChannel>
    {
        private ServiceChannelTranslationDefinitionHelper definitionHelper;

        public WebPageChannelMainTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ServiceChannelTranslationDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
        }

        public override VmWebPageChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddPartial(input => input.WebpageChannels?.FirstOrDefault(), output => output);

            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddChannelBaseDefinition(definition);
            return definition.GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmWebPageChannel vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                    .DisableAutoTranslation()
                    .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                    .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                    .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                    .AddLanguageAvailability(i => i, o => o)
                    .AddNavigation(i => ServiceChannelTypeEnum.WebPage.ToString(), o => o.Type)
                    .AddNavigationOneMany(i => i, o => o.WebpageChannels)
                ;
            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddChannelBaseDefinition(definition);
            return definition.GetFinal();
        }

    }
}