using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.Common.V2.Connections
{
    [RegisterService(typeof(ITranslator<TrackingServiceServiceChannel, VmConnectionOperation>), RegisterType.Transient)]
    internal class ConnectionOperationTranslator : Translator<TrackingServiceServiceChannel, VmConnectionOperation>
    {
        protected ConnectionOperationTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives
        ) : base(
            resolveManager,
            translationPrimitives
        ) { }

        public override VmConnectionOperation TranslateEntityToVm(TrackingServiceServiceChannel entity)
        {
            var result = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.OperationType, o => o.OperationType)
                .AddNavigation(i => i.CreatedBy, o => o.CreatedBy)
                .AddSimple(i => i.Created, o => o.Created)
                .GetFinal();
            return result;
        }

        public override TrackingServiceServiceChannel TranslateVmToEntity(VmConnectionOperation vModel)
        {
            throw new NotImplementedException();
        }
    }
}
