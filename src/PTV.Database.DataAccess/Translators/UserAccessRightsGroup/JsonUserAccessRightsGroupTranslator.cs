using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.UserAccessRightsGroup
{
    [RegisterService(typeof(ITranslator<Model.Models.UserAccessRightsGroup, VmJsonUserAccessRightsGroup>), RegisterType.Scope)]
    internal class JsonUserAccessRightsGroupTranslator : Translator<Model.Models.UserAccessRightsGroup, VmJsonUserAccessRightsGroup>
    {
        public JsonUserAccessRightsGroupTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmJsonUserAccessRightsGroup TranslateEntityToVm(Model.Models.UserAccessRightsGroup entity)
        {
            throw new System.NotImplementedException();
        }

        public override Model.Models.UserAccessRightsGroup TranslateVmToEntity(VmJsonUserAccessRightsGroup vModel)
        {
            var definition = CreateViewModelEntityDefinition<Model.Models.UserAccessRightsGroup>(vModel)
                .UseDataContextUpdate(i => true, i => o => i.Code == o.Code, def => def.UseDataContextCreate(x => true, o => o.Id,
                    i => Guid.NewGuid()));

            var entity = definition.GetFinal();
            vModel.Names.ForEach(x => x.OwnerReferenceId = entity.Id);
            return definition
                .AddCollection(input => input.Names, output => output.UserAccessRightsGroupNames)
                .AddNavigation(input => input.Code, output => output.Code)
                .AddNavigation(input => input.UserRole, output => output.UserRole)
                .AddSimple(input => input.AccessRightFlag, output => output.AccessRightFlag)
                .GetFinal();
        }
    }
}