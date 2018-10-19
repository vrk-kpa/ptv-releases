using System;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.UserAccessRightsGroup
{
    [RegisterService(typeof(ITranslator<UserAccessRightsGroupName, VmSimpleName>), RegisterType.Scope)]
    internal class UserAccessRightsGroupNameToExternalTranslator : Translator<UserAccessRightsGroupName, VmSimpleName>
    {
        private readonly ITypesCache typesCache;

        public UserAccessRightsGroupNameToExternalTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmSimpleName TranslateEntityToVm(UserAccessRightsGroupName entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.Name, o => o.Value)
                .AddNavigation(i => typesCache.GetByValue<Language>(i.LocalizationId), o => o.Localization)
                .GetFinal();
        }

        public override UserAccessRightsGroupName TranslateVmToEntity(VmSimpleName vModel)
        {
            throw new NotImplementedException();
        }
    }
}