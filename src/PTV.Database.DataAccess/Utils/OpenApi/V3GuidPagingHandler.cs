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

using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using System;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class V3GuidPagingHandler<TEntity, TRoot, TName, TLanguageAvailability> : EntitiesByStatusPagingHandler<V3VmOpenApiGuidPage, VmOpenApiItem, TEntity, TRoot, TName, TLanguageAvailability>
        where TEntity : class, IEntityIdentifier, IAuditing, IVersionedVolume<TRoot>
        where TRoot : VersionedRoot<TEntity>
        where TName : IName
        where TLanguageAvailability : ILanguageAvailabilityBase
    {
        public V3GuidPagingHandler(
            EntityStatusExtendedEnum entityStatus,
            DateTime? date,
            DateTime? dateBefore,
            IPublishingStatusCache publishingStatusCache,
            ITypesCache typesCache,
            int pageNumber,
            int pageSize
            ): base(entityStatus, date, dateBefore, publishingStatusCache, typesCache, pageNumber, pageSize)
        { }
    }
}
