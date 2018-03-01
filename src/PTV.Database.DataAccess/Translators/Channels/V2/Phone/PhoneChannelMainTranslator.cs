using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models;
using VmPhoneChannel = PTV.Domain.Model.Models.V2.Channel.VmPhoneChannel;
using VmWebPageChannel = PTV.Domain.Model.Models.V2.Channel.VmWebPageChannel;
using Microsoft.EntityFrameworkCore;

namespace PTV.Database.DataAccess.Translators.Channels.V2.Phone
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmPhoneChannel>), RegisterType.Transient)]
    internal class PhoneChannelMainTranslator : Translator<ServiceChannelVersioned, VmPhoneChannel>
    {
        private ServiceChannelTranslationDefinitionHelper definitionHelper;
        private ILanguageCache languageCache;

        public PhoneChannelMainTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ServiceChannelTranslationDefinitionHelper definitionHelper, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmPhoneChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddDictionary(input => input.WebPages.Select(x => x.WebPage), output => output.WebPage, key => languageCache.GetByValue(key.LocalizationId));

            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddChannelBaseDefinition(definition)
                .AddOpeningHoursDefinition(definition);
            return definition.GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmPhoneChannel vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                    .DisableAutoTranslation()
                    .DefineEntitySubTree(i => i.Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours))
                    .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                    .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                    .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                    .AddLanguageAvailability(i => i, o => o)
                    .AddNavigation(i => ServiceChannelTypeEnum.Phone.ToString(), o => o.Type)
                    .AddCollection(i => i.WebPage?.Select(pair => new VmWebPage
                    {
                        Id = pair.Value.Id,
                        UrlAddress = pair.Value.UrlAddress,
                        OwnerReferenceId = i.Id,
                        LocalizationId = languageCache.Get(pair.Key)
                    }), o => o.WebPages)
                ;
            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddChannelBaseDefinition(definition)
                .AddOpeningHoursDefinition(definition);
            return definition.GetFinal();
        }

    }
}