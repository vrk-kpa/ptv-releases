using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.UserAccessRightsGroup
{
    [RegisterService(typeof(ITranslator<Model.Models.UserAccessRightsGroup, VmUserAccessRightsGroupSimple>), RegisterType.Scope)]
    internal class UserAccessRightsGroupToExternalTranslator : Translator<Model.Models.UserAccessRightsGroup, VmUserAccessRightsGroupSimple>
    {
        public UserAccessRightsGroupToExternalTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmUserAccessRightsGroupSimple TranslateEntityToVm(Model.Models.UserAccessRightsGroup entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.Code, o => o.Code)
                .AddCollection(i => i.UserAccessRightsGroupNames, o => o.Name)
                .GetFinal();
        }

        public override Model.Models.UserAccessRightsGroup TranslateVmToEntity(VmUserAccessRightsGroupSimple vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<Model.Models.UserAccessRightsGroup, VmUserAccessRightsGroup>), RegisterType.Scope)]
    internal class UserAccessRightsGroupToInternalTranslator : Translator<Model.Models.UserAccessRightsGroup, VmUserAccessRightsGroup>
    {
        public UserAccessRightsGroupToInternalTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmUserAccessRightsGroup TranslateEntityToVm(Model.Models.UserAccessRightsGroup entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.Code, o => o.Code)
                .AddSimple(i => (AccessRightEnum)i.AccessRightFlag, o => o.AccessRightFlag)
                .AddSimple(i => i.UserRole.ConvertToEnum<UserRoleEnum>(UserRoleEnum.Shirley), o => o.UserRole)
                .GetFinal();
        }

        public override Model.Models.UserAccessRightsGroup TranslateVmToEntity(VmUserAccessRightsGroup vModel)
        {
            throw new NotImplementedException();
        }
    }
}