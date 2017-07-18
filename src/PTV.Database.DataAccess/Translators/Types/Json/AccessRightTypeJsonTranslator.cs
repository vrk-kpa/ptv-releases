using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Types.Base;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Privileges;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Types.Json
{
    [RegisterService(typeof(ITranslator<AccessRightType, VmJsonTypeItem>), RegisterType.Scope)]
    internal class AccessRightTypeJsonTranslator : TypeBaseJsonTranslator<AccessRightType, AccessRightName>
    {
        public AccessRightTypeJsonTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }
    }
}