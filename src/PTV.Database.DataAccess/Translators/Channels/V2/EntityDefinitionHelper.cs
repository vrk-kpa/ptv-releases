using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework;

namespace PTV.Database.DataAccess.Translators.Channels.V2
{
    [RegisterService(typeof(EntityDefinitionHelper), RegisterType.Scope)]
    internal class EntityDefinitionHelper
    {
        public ITranslationDefinitionsForContextUsage<TIn, TOut> GetDefinitionWithCreateOrUpdate<TIn, TOut>(ITranslationDefinitionsForContextUsage<TIn, TOut> definition) where TIn : VmEntityBase where TOut : IEntityIdentifier
        {
            return definition
                .DisableAutoTranslation()
                .UseDataContextCreate(input => !input.Id.IsAssigned(), output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => input.Id.IsAssigned(), input => output => input.Id == output.Id);

        }

        public ITranslationDefinitions<TIn, TOut> AddLanguageAvailabilitiesDefinition<TIn, TOut, TLanguagesAvailability>(ITranslationDefinitions<TIn, TOut> definition, ILanguageOrderCache orderCache) where TIn: IMultilanguagedEntity<TLanguagesAvailability> where TOut: ILanguagesAvailabilities where TLanguagesAvailability : LanguageAvailability
        {
            return definition.AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                i => i.LanguageAvailabilities.OrderBy(x => orderCache.Get(x.LanguageId)),
                o => o.LanguagesAvailabilities);
        }
    }
}