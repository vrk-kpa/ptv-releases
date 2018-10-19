using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels.V2.Electronic
{
//    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmElectronicChannel>), RegisterType.Transient)]
//    internal class ElectronicChannelMainTranslator : Translator<ServiceChannelVersioned, VmElectronicChannel>
//    {
//        public ElectronicChannelMainTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
//        {
//        }
//
//        public override VmElectronicChannel TranslateEntityToVm(ServiceChannelVersioned entity)
//        {
//            return CreateEntityViewModelDefinition(entity)
//                .AddSimple(i => i.Id, o => o.Id)
//                .AddNavigation(i => i, o => o.BasicInfo)
//                .AddNavigation(i => i, o => o.OpeningHours)
//                .GetFinal();
//        }
//
//        public override ServiceChannelVersioned TranslateVmToEntity(VmElectronicChannel vModel)
//        {
//            var definition = CreateViewModelEntityDefinition(vModel)
//                .DisableAutoTranslation()
//                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
//                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
//                .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o)
//                .AddLanguageAvailability(o => o)
//                .AddNavigation(i => ServiceChannelTypeEnum.EChannel.ToString(), o => o.Type)
//                .AddPartial(i => i.BasicInfo)
//                .AddPartial(i => i.OpeningHours)
//                ;
//            return definition.GetFinal();
//        }
//
//    }
}