using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Database.DataAccess.Translators.Common.V2
{
    [RegisterService(typeof(ITranslator<AddressForeignTextName, string>), RegisterType.Transient)]
    internal class ForeignAddressTranslator : Translator<AddressForeignTextName, string>
    {
        public ForeignAddressTranslator (IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(AddressForeignTextName entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Name, output => output)
                .GetFinal();
        }

        public override AddressForeignTextName TranslateVmToEntity(string vModel)
        {
            throw new NotSupportedException();
        }
    }
}
