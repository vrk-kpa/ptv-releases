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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.DataAccess.Extensions
{
    internal static class TranslationOrderExtensions
    {
        public static IQueryable<T> WithUnificRootId<T>(this IQueryable<T> source, Guid id)
            where T : IEntityTranslationOrder
        {
            switch (source)
            {
                case IQueryable<GeneralDescriptionTranslationOrder> services:
                    return services.Where(s => s.StatutoryServiceGeneralDescriptionId == id) as IQueryable<T>;
                case IQueryable<ServiceChannelTranslationOrder> channels:
                    return channels.Where(c => c.ServiceChannelId == id) as IQueryable<T>;
                case IQueryable<ServiceTranslationOrder> collections:
                    return collections.Where(sc => sc.ServiceId == id) as IQueryable<T>;
                default:
                    throw new NotImplementedException();
            }
        }

        public static IQueryable<T> WithUnificRootId<T>(this IQueryable<T> source, IEnumerable<Guid> ids)
            where T : IEntityTranslationOrder
        {
            switch (source)
            {
                case IQueryable<GeneralDescriptionTranslationOrder> services:
                    return services.Where(s => ids.Contains(s.StatutoryServiceGeneralDescriptionId)) as IQueryable<T>;
                case IQueryable<ServiceChannelTranslationOrder> channels:
                    return channels.Where(c => ids.Contains(c.ServiceChannelId)) as IQueryable<T>;
                case IQueryable<ServiceTranslationOrder> collections:
                    return collections.Where(sc => ids.Contains(sc.ServiceId)) as IQueryable<T>;
                default:
                    throw new NotImplementedException();
            }
        }
        public static IQueryable<Guid> SelectUnificRootId<T>(this IQueryable<T> source)
            where T : IEntityTranslationOrder
        {
            switch (source)
            {
                case IQueryable<GeneralDescriptionTranslationOrder> services:
                    return services.Select(s => s.StatutoryServiceGeneralDescriptionId);
                case IQueryable<ServiceChannelTranslationOrder> channels:
                    return channels.Select(c => c.ServiceChannelId);
                case IQueryable<ServiceTranslationOrder> collections:
                    return collections.Select(sc => sc.ServiceId);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
