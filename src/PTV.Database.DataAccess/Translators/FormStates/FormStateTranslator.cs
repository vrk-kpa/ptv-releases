using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Database.DataAccess.Translators.FormStates
{
    [RegisterService(typeof(ITranslator<FormState, VmFormState>), RegisterType.Transient)]
    internal class FormStateTranslator : Translator<FormState, VmFormState>
    {
        public FormStateTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives
        ) : base(
            resolveManager,
            translationPrimitives
        ) {}

        public override VmFormState TranslateEntityToVm(FormState entity)
        {
            return CreateEntityViewModelDefinition(entity).GetFinal();
        }

        public override FormState TranslateVmToEntity(VmFormState vModel)
        {
            return CreateViewModelEntityDefinition(vModel).GetFinal();
        }
    }
}
