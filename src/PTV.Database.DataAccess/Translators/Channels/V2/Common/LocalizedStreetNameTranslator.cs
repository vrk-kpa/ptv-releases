using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels.V2.Common
{
    [RegisterService(typeof(ITranslator<StreetName, VmLocalizedStreetName>), RegisterType.Transient)]
    internal class LocalizedStreetNameTranslator : Translator<StreetName, VmLocalizedStreetName>
    {
        public LocalizedStreetNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmLocalizedStreetName TranslateEntityToVm(StreetName entity)
        {
            throw new NotImplementedException();
        }

        public override StreetName TranslateVmToEntity(VmLocalizedStreetName vModel)
        {
            if (vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId);
            }
            return CreateViewModelEntityDefinition<StreetName>(vModel)
                .UseDataContextCreate(i => !i.OwnerReferenceId.HasValue)
                .UseDataContextLocalizedUpdate(i => i.OwnerReferenceId.HasValue, i => o => (i.OwnerReferenceId == o.AddressStreetId), def => def.UseDataContextCreate(i => i.OwnerReferenceId.IsAssigned()))
                .AddNavigation(i => i.Name, o => o.Name)
                .AddRequestLanguage(output => output)
                .GetFinal();
        }
    }
}
