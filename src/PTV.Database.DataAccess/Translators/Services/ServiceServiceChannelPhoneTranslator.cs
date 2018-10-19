using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannelPhone, VmPhone>), RegisterType.Transient)]
    internal class ServiceServiceChannelPhoneTranslator : Translator<ServiceServiceChannelPhone, VmPhone>
    {
        private readonly ILanguageCache languageCache;
        public ServiceServiceChannelPhoneTranslator(
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

        public override VmPhone TranslateEntityToVm(ServiceServiceChannelPhone entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannelPhone TranslateVmToEntity(VmPhone vModel)
        {
            if (vModel == null) return null;
            bool existsById = vModel.Id.IsAssigned();

            Guid languageId = vModel.LanguageId.IsAssigned()
                ? vModel.LanguageId.Value
                : RequestLanguageId;

            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !existsById)
                .UseDataContextUpdate(
                    input => existsById,
                    input => output => (!input.Id.IsAssigned() || input.Id == output.PhoneId) &&
                                       (!input.OwnerReferenceId.IsAssigned() || output.ServiceServiceChannel.ServiceId == input.OwnerReferenceId) &&
                                       (!input.OwnerReferenceId2.IsAssigned() || output.ServiceServiceChannel.ServiceChannelId == input.OwnerReferenceId2) &&
                                       (languageId == output.Phone.LocalizationId),
                    create => create.UseDataContextCreate(c => true)
                );


            var result = translation
                .AddNavigation(input => input, output => output.Phone)
                .GetFinal();
            return result;
        }
    }
}
