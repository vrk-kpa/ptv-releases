using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Languages
{
    [RegisterService(typeof(ITranslator<ILanguageAvailability, VmLanguageAvailabilityInfo>), RegisterType.Transient)]
    internal class LanguageAvailabilityTranslator : Translator<ILanguageAvailability, VmLanguageAvailabilityInfo>
    {
        public LanguageAvailabilityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmLanguageAvailabilityInfo TranslateEntityToVm(ILanguageAvailability entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.LanguageId, o => o.LanguageId)
                .AddSimple(i => i.StatusId, o => o.StatusId)
                .GetFinal();
        }

        public override ILanguageAvailability TranslateVmToEntity(VmLanguageAvailabilityInfo vModel)
        {
            throw new NotImplementedException();
        }
    }
}