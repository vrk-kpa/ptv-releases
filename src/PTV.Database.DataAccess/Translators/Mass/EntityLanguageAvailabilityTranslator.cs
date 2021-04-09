/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services.V2;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Mass
{
    [RegisterService(typeof(ITranslator<IBaseInformation, VmPublishingModel>), RegisterType.Transient)]
    internal class EntityPublishLanguageAvailabilityTranslator : Translator<IBaseInformation, VmPublishingModel>
    {
        private readonly ITypesCache typesCache;

        public EntityPublishLanguageAvailabilityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmPublishingModel TranslateEntityToVm(IBaseInformation entity)
        {
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());

            return CreateEntityViewModelDefinition<VmPublishingModel>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddCollection(i => i.LanguageAvailabilitiesReference, o => o.LanguagesAvailabilities)
                .Propagation((i, o) =>
                {
                    o.LanguagesAvailabilities.ForEach(x => x.StatusId = publishingStatusId);
                })
                .GetFinal();
        }

        public override IBaseInformation TranslateVmToEntity(VmPublishingModel vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<IBaseInformation, VmArchivingModel>), RegisterType.Transient)]
    internal class EntityArchiveLanguageAvailabilityTranslator : Translator<IBaseInformation, VmArchivingModel>
    {
        public EntityArchiveLanguageAvailabilityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmArchivingModel TranslateEntityToVm(IBaseInformation entity)
        {
            return CreateEntityViewModelDefinition<VmArchivingModel>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddCollection(i => i.LanguageAvailabilitiesReference, o => o.LanguagesAvailabilities)
                .GetFinal();
        }

        public override IBaseInformation TranslateVmToEntity(VmArchivingModel vModel)
        {
            throw new NotImplementedException();
        }
    }
}
