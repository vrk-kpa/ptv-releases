using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Common.V2
{
    [RegisterService(typeof(ITranslator<WebPage, VmUrl>), RegisterType.Transient)]
    internal class UrlTranslator : Translator<WebPage, VmUrl>
    {
        public UrlTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmUrl TranslateEntityToVm(WebPage entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Url, o => o.UrlAddress)
                .GetFinal();
        }

        public override WebPage TranslateVmToEntity(VmUrl vModel)
        {
            throw new NotSupportedException();
        }
    }
}