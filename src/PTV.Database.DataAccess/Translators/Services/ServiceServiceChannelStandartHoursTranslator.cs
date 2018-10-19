using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannelServiceHours, VmNormalHours>), RegisterType.Transient)]
    internal class ServiceServiceChannelStandartHoursTranslator : Translator<ServiceServiceChannelServiceHours, VmNormalHours>
    {
        public ServiceServiceChannelStandartHoursTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
        }

        public override VmNormalHours TranslateEntityToVm(ServiceServiceChannelServiceHours entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannelServiceHours TranslateVmToEntity(VmNormalHours vModel)
        {
            if (vModel == null) return null;
            bool existsById = vModel.Id.IsAssigned();
         
            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !existsById)
                .UseDataContextUpdate(
                    input => existsById,
                    input => output => (!input.Id.IsAssigned() || input.Id == output.ServiceHoursId) &&
                                       (!input.OwnerReferenceId.IsAssigned() || output.ServiceServiceChannel.ServiceId == input.OwnerReferenceId) &&
                                       (!input.OwnerReferenceId2.IsAssigned() || output.ServiceServiceChannel.ServiceChannelId == input.OwnerReferenceId2),
                    create => create.UseDataContextCreate(c => true)
                );

            var result = translation
                .AddNavigation(input => input, output => output.ServiceHours)
                .GetFinal();
            return result;
        }
    }
}
