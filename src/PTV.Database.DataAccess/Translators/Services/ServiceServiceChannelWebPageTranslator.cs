using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannelWebPage, VmWebPage>), RegisterType.Transient)]
    internal class ServiceServiceChannelWebPageTranslator : Translator<ServiceServiceChannelWebPage, VmWebPage>
    {
        private readonly ILanguageCache languageCache;
        public ServiceServiceChannelWebPageTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override VmWebPage TranslateEntityToVm(ServiceServiceChannelWebPage entity)
        {
            throw new System.NotImplementedException();
        }

        public override ServiceServiceChannelWebPage TranslateVmToEntity(VmWebPage vModel)
        {
            if (vModel == null) return null;
            bool existsById = vModel.Id.IsAssigned();
            Guid languageId = vModel.LocalizationId.IsAssigned()
                ? vModel.LocalizationId.Value
                : languageCache.Get(RequestLanguageCode.ToString());

            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !existsById)
                .UseDataContextUpdate(
                    input => existsById,
                    input => output => (!input.Id.IsAssigned() || input.Id == output.WebPageId) &&
                                       (!input.OwnerReferenceId.IsAssigned() || output.ServiceServiceChannel.ServiceId == input.OwnerReferenceId) &&
                                       (!input.OwnerReferenceId2.IsAssigned() || output.ServiceServiceChannel.ServiceChannelId == input.OwnerReferenceId2) &&
                                       (languageId == output.WebPage.LocalizationId),
                    create => create.UseDataContextCreate(c => true)
                );

            return translation
                .AddNavigation(input => input, output => output.WebPage)
                .GetFinal();
        }
    }
}
