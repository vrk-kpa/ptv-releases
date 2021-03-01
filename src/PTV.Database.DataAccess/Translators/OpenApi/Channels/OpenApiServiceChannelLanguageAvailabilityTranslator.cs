﻿/**
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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using System;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelLanguageAvailability, VmOpenApiLanguageAvailability>), RegisterType.Transient)]
    internal class OpenApiServiceChannelLanguageAvailabilityTranslator : Translator<ServiceChannelLanguageAvailability, VmOpenApiLanguageAvailability>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceChannelLanguageAvailabilityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiLanguageAvailability TranslateEntityToVm(ServiceChannelLanguageAvailability entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceChannelLanguageAvailability TranslateVmToEntity(VmOpenApiLanguageAvailability vModel)
        {
            var langId = languageCache.Get(vModel.Language);
            var definition = CreateViewModelEntityDefinition<ServiceChannelLanguageAvailability>(vModel)
                .UseDataContextUpdate(
                  i => i.OwnerReferenceId.IsAssigned(),
                  i => o => i.OwnerReferenceId.Value == o.ServiceChannelVersionedId && langId == o.LanguageId,
                  def => def.UseDataContextCreate(i => true)
                )
                .AddSimple(i => typesCache.Get<PublishingStatusType>(i.PublishingStatus), o => o.StatusId)
                .AddSimple(i => langId, o => o.LanguageId);
            if (vModel.PublishAt.HasValue)
            {
                definition
                    .AddSimple(i => i.PublishAt.Value.Date, o => o.PublishAt) // Get only the date, time is ignore (always 00:00:00)
                    .AddSimple(i => DateTime.UtcNow, o => o.Reviewed)
                    .AddNavigation(i => i.ReviewedBy, o => o.ReviewedBy);
            }
            if (vModel.ArchiveAt.HasValue)
            {
                definition
                    .AddSimple(i => i.ArchiveAt.Value.Date, o => o.ArchiveAt) // Get only the date, time is ignore (always 00:00:00)
                    .AddSimple(i => DateTime.UtcNow, o => o.SetForArchived)
                    .AddNavigation(i => i.ReviewedBy, o => o.SetForArchivedBy);
            }
            return definition.GetFinal();
        }
    }
}
