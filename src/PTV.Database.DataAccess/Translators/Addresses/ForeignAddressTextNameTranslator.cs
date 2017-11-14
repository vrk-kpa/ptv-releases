using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<AddressForeignTextName, VmAddressSimple>), RegisterType.Transient)]
    internal class ForeignAddressTextNameTranslator : Translator<AddressForeignTextName, VmAddressSimple>
    {
        public ForeignAddressTextNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmAddressSimple TranslateEntityToVm(AddressForeignTextName entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddNavigation(input => input.Name, output => output.ForeignAddressText)
                .GetFinal();
        }

        public override AddressForeignTextName TranslateVmToEntity(VmAddressSimple vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextLocalizedUpdate(input => CoreExtensions.IsAssigned((Guid?) input.Id), input => output => output.AddressForeignId == input.Id, d => d.UseDataContextCreate(i => true))
                .AddNavigation(input => input.ForeignAddressText, output => output.Name)
                .AddRequestLanguage(output => output)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<AddressForeignTextName, string>), RegisterType.Transient)]
    internal class ForeignAddressTextNameStringTranslator : Translator<AddressForeignTextName, string>
    {
        public ForeignAddressTextNameStringTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
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
            throw new NotImplementedException();
        }
    }
}