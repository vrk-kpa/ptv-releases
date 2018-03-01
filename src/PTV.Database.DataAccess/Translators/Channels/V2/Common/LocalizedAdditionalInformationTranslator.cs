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
    [RegisterService(typeof(ITranslator<AddressAdditionalInformation, VmLocalizedAdditionalInformation>), RegisterType.Transient)]
    internal class LocalizedAdditionalInformationTranslator : Translator<AddressAdditionalInformation, VmLocalizedAdditionalInformation>
    {
        public LocalizedAdditionalInformationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmLocalizedAdditionalInformation TranslateEntityToVm(AddressAdditionalInformation entity)
        {
            throw new NotImplementedException();
        }

        public override AddressAdditionalInformation TranslateVmToEntity(VmLocalizedAdditionalInformation vModel)
        {
            if (vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId);
            }
            return CreateViewModelEntityDefinition<AddressAdditionalInformation>(vModel)
                .UseDataContextCreate(i => !i.OwnerReferenceId.HasValue)
                .UseDataContextLocalizedUpdate(i => i.OwnerReferenceId.HasValue, i => o => (i.OwnerReferenceId == o.AddressId), def => def.UseDataContextCreate(i => i.OwnerReferenceId.IsAssigned()))              
                .AddNavigation(i => i.Content, o => o.Text)
                .AddRequestLanguage(output => output)
                .GetFinal();
        }
    }
}
