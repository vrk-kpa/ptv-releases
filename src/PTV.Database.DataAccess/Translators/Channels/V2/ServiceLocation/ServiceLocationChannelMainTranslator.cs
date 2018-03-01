using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using PTV.Framework.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PTV.Database.DataAccess.Translators.Channels.V2.ServiceLocation
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmServiceLocationChannel>), RegisterType.Transient)]
    internal class ServiceLocationChannelMainTranslator : Translator<ServiceChannelVersioned, VmServiceLocationChannel>
    {
        private ServiceChannelTranslationDefinitionHelper definitionHelper;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;

        public ServiceLocationChannelMainTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ServiceChannelTranslationDefinitionHelper definitionHelper, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmServiceLocationChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddPartial(input => input.ServiceLocationChannels?.FirstOrDefault(), output => output);

            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddChannelBaseDefinition(definition)
                .AddOpeningHoursDefinition(definition);
            definition
                .AddDictionaryList(i =>
                    i.Phones.Select(x => x.Phone).Where(x => typesCache.Compare<PhoneNumberType>(x.TypeId, PhoneNumberTypeEnum.Phone.ToString())).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created),
                    o => o.PhoneNumbers,
                    x => languageCache.GetByValue(x.LocalizationId))
                .AddDictionaryList(
                    i => i.Phones.Select(x => x.Phone).Where(x => typesCache.Compare<PhoneNumberType>(x.TypeId, PhoneNumberTypeEnum.Fax.ToString())).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created),
                    o => o.FaxNumbers,
                    x => languageCache.GetByValue(x.LocalizationId));
            definition.AddDictionaryList(i => i.WebPages.OrderBy(x => x.WebPage.OrderNumber).ThenBy(x => x.WebPage.Created), o => o.WebPages, key => languageCache.GetByValue(key.WebPage.LocalizationId));
            return definition.GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmServiceLocationChannel vModel)
        {
            MergeFaxNumbers(vModel);

            var definition = CreateViewModelEntityDefinition(vModel)
                    .DisableAutoTranslation()
                    .DefineEntitySubTree(i => i.Include(j => j.ServiceChannelServiceHours).ThenInclude(j => j.ServiceHours))
                    .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                    .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                    .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
                    .AddLanguageAvailability(i => i, o => o)
                    .AddNavigation(i => ServiceChannelTypeEnum.ServiceLocation.ToString(), o => o.Type)
                    .AddNavigationOneMany(i => i, o => o.ServiceLocationChannels)
                    .AddCollectionWithRemove(i => i.WebPages.SelectMany(pair =>
                    {
                        var localizationId = languageCache.Get(pair.Key);
                        var orderNumber = 0;
                        return pair.Value.Select(sv =>
                        {
                            sv.OwnerReferenceId = i.Id;
                            sv.LocalizationId = localizationId;
                            sv.OrderNumber = orderNumber++;
                            return sv;
                        });
                    }), o => o.WebPages, x => true)
                ;
            definitionHelper
                .AddLanguagesDefinition(definition)
                .AddChannelBaseDefinition(definition)
                .AddOpeningHoursDefinition(definition);
            return definition.GetFinal();
        }

        private void MergeFaxNumbers(VmServiceLocationChannel vModel)
        {
            if (vModel.FaxNumbers.Any())
            {
                var faxTypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString());

                foreach (var pair in vModel.FaxNumbers)
                {
                    pair.Value?.ForEach(x => x.TypeId = faxTypeId);
                }

                if (vModel.PhoneNumbers != null)
                {
                    vModel.PhoneNumbers = vModel.PhoneNumbers
                        .Concat(vModel.FaxNumbers)
                        .GroupBy(kv => kv.Key)
                        .ToDictionary(kv => kv.Key, kv => kv.SelectMany(g => g.Value).ToList());
                }
            }
        }
    }
}