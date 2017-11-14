using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours
{
    [RegisterService(typeof(ITranslator<ServiceHours, VmNormalHours>), RegisterType.Transient)]
    internal class NormalOpeningHourTranslator2 : Translator<ServiceHours, VmNormalHours>
    {
        private ITypesCache typesCache;
        private Channels.ServiceChannelTranslationDefinitionHelper definitionHelper;

        public NormalOpeningHourTranslator2(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache, Channels.ServiceChannelTranslationDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
            this.definitionHelper = definitionHelper;
        }

        public override VmNormalHours TranslateEntityToVm(ServiceHours entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddPartial(input => input, output => output as VmOpeningHour)
                .AddDictionary(GetHours, output => output.DailyHours, key => key.Day.ToString())
                .AddSimple(input => input.DailyOpeningTimes.Count == 0, output => output.IsNonStop);
            var result = definition.GetFinal();
            if (!result.IsNonStop)
            {
                Enum.GetNames(typeof(WeekDayEnum)).ForEach(x =>
                {
                    if (!result.DailyHours.ContainsKey(x))
                    {
                        result.DailyHours.Add(x, null);
                    }
                });
            }
            return result;
        }

        private static IEnumerable<DailyOpeningHoursEntitiesModel> GetHours(ServiceHours input)
        {
            return input.DailyOpeningTimes
                .GroupBy(x => x.DayFrom)
                .Select(x => new DailyOpeningHoursEntitiesModel {
                    Day = (WeekDayEnum)Enum.ToObject(typeof(WeekDayEnum), x.Key),
                    Hours = x.Select(i => i).ToList()
                });
        }

        public override ServiceHours TranslateVmToEntity(VmNormalHours vModel)
        {
            if (vModel == null)
            {
                return null;
            }
            var definition = definitionHelper.GetDefinitionWithCreateOrUpdate(CreateViewModelEntityDefinition(vModel))
                .AddPartial(i => i as VmOpeningHour);


            var entity = definition.GetFinal();
            if (vModel.IsNonStop)
            {
                definition.AddCollectionWithRemove(
                    input => new List<VmNormalDailyOpeningHour>(),
                    output => output.DailyOpeningTimes, TranslationPolicy.FetchData,
                    x => x.OpeningHourId == vModel.Id
                );
            }
            else
            {
                definition.AddCollectionWithRemove(input => input.DailyHours?.Where(x => x.Value != null && x.Value.Active).SelectMany(x =>
                {
                    if (Enum.TryParse(x.Key, true, out WeekDayEnum day))
                    {
                        int order = 0;
                        x.Value.Intervals.ForEach(interval =>
                        {
                            interval.DayFrom = day;
                            interval.OwnerReferenceId = input.Id;
                            interval.Order = order++;
                        });
                    }
                    return x.Value.Intervals;
                }), output => output.DailyOpeningTimes, x => true);
            }
            return entity;
        }
    }

    [RegisterService(typeof(ITranslator<ServiceChannelServiceHours, VmNormalHours>), RegisterType.Transient)]
    internal class ServiceChannelNormalOpeningHourTranslator2 : Translator<ServiceChannelServiceHours, VmNormalHours>
    {
        public ServiceChannelNormalOpeningHourTranslator2(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmNormalHours TranslateEntityToVm(ServiceChannelServiceHours entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceChannelServiceHours TranslateVmToEntity(VmNormalHours vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            bool exists = vModel.Id.IsAssigned();

            return CreateViewModelEntityDefinition< ServiceChannelServiceHours>(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextUpdate(input => exists, input => output => input.Id == output.ServiceHoursId)
                .AddNavigation(input => input, output => output.ServiceHours)
                .GetFinal();
        }
    }
}