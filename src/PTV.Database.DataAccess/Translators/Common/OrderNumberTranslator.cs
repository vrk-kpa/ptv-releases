using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Database.DataAccess.Translators.Common
{
    [RegisterService(typeof(ITranslator<IOrderable, IVmOrderable>), RegisterType.Transient)]
    internal class OrderNumberTranslator : Translator<IOrderable, IVmOrderable>
    {
        public OrderNumberTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
        }

        public override IVmOrderable TranslateEntityToVm(IOrderable entity)
        {
            return CreateEntityViewModelDefinition<IVmOrderable>(entity)
                .AddSimple(i => i.OrderNumber ?? 0, o => o.OrderNumber)
                .GetFinal();
        }

        public override IOrderable TranslateVmToEntity(IVmOrderable vModel)
        {
            return CreateViewModelEntityDefinition<IOrderable>(vModel)
                .AddSimple(i => i.OrderNumber ?? 0, o => o.OrderNumber)
                .GetFinal();

        }
    }
}
