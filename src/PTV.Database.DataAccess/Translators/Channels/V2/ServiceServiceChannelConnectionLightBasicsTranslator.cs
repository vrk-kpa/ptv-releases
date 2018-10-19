using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels.V2
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmConnectionLightBasics>), RegisterType.Transient)]
    internal class ServiceServiceChannelConnectionLightBasicsTranslator : Translator<ServiceServiceChannel, VmConnectionLightBasics>
    {
        public ServiceServiceChannelConnectionLightBasicsTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmConnectionLightBasics TranslateEntityToVm(ServiceServiceChannel entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.ServiceId, o => o.ServiceId)
                .AddSimple(i => i.ServiceChannelId, o => o.ChannelId)
                .AddNavigation(i => i.ModifiedBy, o => o.ModifiedBy)
                .AddSimple(i => i.Modified, o => o.Modified)
                .GetFinal();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmConnectionLightBasics vModel)
        {
            throw new NotImplementedException();
        }
    }
}