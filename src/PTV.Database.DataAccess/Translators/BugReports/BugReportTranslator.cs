using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.BugReports
{
    [RegisterService(typeof(ITranslator<BugReport, VmBugReport>), RegisterType.Transient)]
    internal class BugReportTranslator : Translator<BugReport, VmBugReport>
    {
        public BugReportTranslator(
          IResolveManager resolveManager,
          ITranslationPrimitives translationPrimitives
        ) : base(
          resolveManager,
          translationPrimitives
        ) {}

        public override VmBugReport TranslateEntityToVm(BugReport entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .GetFinal();
        }

        public override BugReport TranslateVmToEntity(VmBugReport vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .GetFinal();
        }
    }
}
