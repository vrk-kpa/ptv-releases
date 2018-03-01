/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours
{
    [RegisterService(typeof(ITranslator<ServiceHours, VmOpeningHour>), RegisterType.Transient)]
    internal class OpeningHourTranslator : Translator<ServiceHours, VmOpeningHour>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;

        public OpeningHourTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmOpeningHour TranslateEntityToVm(ServiceHours entity)
        {
            var result = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o=> o.Id)
                .AddSimple(input => input.OpeningHoursFrom.ToEpochTime(), output => output.DateFrom)
                .AddSimple(input => input.OpeningHoursTo.ToEpochTime(), output => output.DateTo)
                .AddSimple(input => input.OpeningHoursTo.HasValue, output => output.IsPeriod)
                .AddSimple(input => input.OrderNumber, output => output.OrderNumber)
                .AddDictionary(input => input.AdditionalInformations.Cast<IText>(), output => output.Name, x => languageCache.GetByValue(x.LocalizationId))
                .GetFinal();

            return result;
        }

        private VmLanguageText CreateName(string language, string value, VmOpeningHour vModel)
        {
            return new VmLanguageText
            {
                Text = value,
                Id = vModel.Id,
                LocalizationId = languageCache.Get(language)
            };
        }

        public override ServiceHours TranslateVmToEntity(VmOpeningHour vModel)
        {
            var entity = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !vModel.Id.IsAssigned(), output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => vModel.Id.IsAssigned(), input => output => input.Id == output.Id)
                .AddCollection(i => i.Name?.Select(pair => CreateName(pair.Key, pair.Value, vModel)), o => o.AdditionalInformations, true)
                .AddSimple(
                    input => (input.IsPeriod || input.ServiceHoursType == ServiceHoursTypeEnum.Exception)
                        ? input.DateFrom.FromEpochTime()
                        : null,
                    output => output.OpeningHoursFrom
                )
                .AddSimple(input => input.IsPeriod ? input.DateTo.FromEpochTime() : null, output => output.OpeningHoursTo)
                .AddSimple(input => typesCache.Get<ServiceHourType>(input.ServiceHoursType.ToString()), output => output.ServiceHourTypeId)
                .AddSimple(input => input.OrderNumber, output => output.OrderNumber)
                .GetFinal();

            return entity;
        }

    }
}