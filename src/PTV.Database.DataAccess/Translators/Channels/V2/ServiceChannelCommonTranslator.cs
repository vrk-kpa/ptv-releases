using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using VmServiceChannel = PTV.Domain.Model.Models.V2.Channel.VmServiceChannel;
using PTV.Domain.Model.Models.V2.Channel;

namespace PTV.Database.DataAccess.Translators.Channels.V2
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmServiceChannel>), RegisterType.Transient)]
    internal class ServiceChannelCommonTranslator : Translator<ServiceChannelVersioned, VmServiceChannel>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;

        public ServiceChannelCommonTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmServiceChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var step = CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddPartial(i => i, o => o as VmChannelHeader)
                .AddSimple(i => i.Id, o => o.Id)
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.ShortDescription), output => output.ShortDescription,
                    desc => languageCache.GetByValue(desc.LocalizationId))
                .AddDictionary(input => GetDescription(input, DescriptionTypeEnum.Description), output => output.Description, desc => languageCache.GetByValue(desc.LocalizationId))
                .AddSimple(input => input.ConnectionTypeId, output => output.ConnectionTypeId)
                .AddSimple(input => input.UnificRootId, outupt => outupt.UnificRootId)
                .AddSimple(input => input.OrganizationId, output => output.OrganizationId)
                .AddNavigation(input => input, output => output.AreaInformation)
                .AddDictionaryList(i => i.Phones.Select(x => x.Phone).OrderBy(x => x.OrderNumber), o => o.PhoneNumbers, x => languageCache.GetByValue(x.LocalizationId))
                .AddDictionaryList(i => i.Emails.Select(x => x.Email).OrderBy(x => x.OrderNumber), o => o.Emails, x => languageCache.GetByValue(x.LocalizationId))
                .AddCollection(i => i.UnificRoot.ServiceServiceChannels, o => o.ConnectedServicesUnific);
                //.AddCollection(input => input.UnificRoot.ServiceServiceChannels.Where(x => x.Service.Versions.All(v => v.PublishingStatusId != typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString()))), output => output.Connections);
            return step.GetFinal();
        }

        private IEnumerable<IName> GetName(ServiceChannelVersioned serviceChannelVersioned, NameTypeEnum type)
        {
            return serviceChannelVersioned.ServiceChannelNames.Where(x => typesCache.Compare<NameType>(x.TypeId, type.ToString()));
        }

        private IEnumerable<IDescription> GetDescription(ServiceChannelVersioned serviceChannelVersioned, DescriptionTypeEnum type)
        {
            return serviceChannelVersioned.ServiceChannelDescriptions.Where(x => typesCache.Compare<DescriptionType>(x.TypeId, type.ToString()));
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmServiceChannel vModel)
        {
            var descriptions = new List<VmDescription>();
            if (vModel.ShortDescription != null && vModel.ShortDescription.Any())
            {
                descriptions.AddRange(vModel.ShortDescription.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ShortDescription)));
            }

            if (vModel.Description != null && vModel.Description.Any())
            {
                descriptions.AddRange(vModel.Description?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.Description)));
            }

            return CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .AddSimple(i => i.OrganizationId, o => o.OrganizationId)
                .AddCollection(i => i.Name.Select(pair => CreateName(pair.Key, pair.Value, vModel)), o => o.ServiceChannelNames, true)
                .AddCollection(i => descriptions, o => o.ServiceChannelDescriptions, true)
                .AddPartial(i =>
                {
                    if (i.AreaInformation == null)
                    {
                        return new VmAreaInformation();
                    }
                    i.AreaInformation.OwnerReferenceId = i.Id;
                    return i.AreaInformation;
                })
                .AddSimple(i => i.ConnectionTypeId.IsAssigned()
                        // ReSharper disable once PossibleInvalidOperationException
                        ? i.ConnectionTypeId.Value
                        : typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString())
                    , o => o.ConnectionTypeId)
                .AddCollectionWithRemove(input => input.PhoneNumbers?.SelectMany(pair =>
                {
                    var localizationId = languageCache.Get(pair.Key);
                    var phoneNumberOrder = 0;
                    return pair.Value?.Select(at =>
                    {   
                        if (at != null)
                        {
                            at.LanguageId = localizationId;
                            at.OrderNumber = phoneNumberOrder++;
                        }
                        return at;
                    }) ?? pair.Value;
                }), output => output.Phones, x => true)
                .AddCollectionWithRemove(input => input.Emails?.SelectMany(pair =>
                {
                    var localizationId = languageCache.Get(pair.Key);
                    var emailOrderNumber = 0;
                    return pair.Value?.Select(x =>
                    {
                        if (x != null)
                        {
                            x.LanguageId = localizationId;
                            x.OrderNumber = emailOrderNumber++;
                        }
                        return x;
                    });
                }), output => output.Emails, x => true)
                .GetFinal();
        }

        private VmName CreateName(string language, string value, VmServiceChannel vModel)
        {
            return new VmName
            {
                Name = value,
                TypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString()),
                OwnerReferenceId = vModel.Id,
                LocalizationId = languageCache.Get(language)
            };
        }
        private VmDescription CreateDescription(string language, string value, VmServiceChannel vModel, DescriptionTypeEnum typeEnum)
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