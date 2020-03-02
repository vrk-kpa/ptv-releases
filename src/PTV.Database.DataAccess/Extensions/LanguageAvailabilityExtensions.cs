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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;

namespace PTV.Database.DataAccess.Extensions
{
    internal static class LanguageAvailabilityExtensions
    {
        public static IQueryable<T> WithEntityId<T>(this IQueryable<T> source, Guid id)
            where T : ILanguageAvailability
        {
            switch (source)
            {
                case IQueryable<ServiceLanguageAvailability> services:
                    return services.Where(s => s.ServiceVersionedId == id) as IQueryable<T>;
                case IQueryable<ServiceChannelLanguageAvailability> channels:
                    return channels.Where(c => c.ServiceChannelVersionedId == id) as IQueryable<T>;
                case IQueryable<ServiceCollectionLanguageAvailability> collections:
                    return collections.Where(sc => sc.ServiceCollectionVersionedId == id) as IQueryable<T>;
                case IQueryable<OrganizationLanguageAvailability> organizations:
                    return organizations.Where(o => o.OrganizationVersionedId == id) as IQueryable<T>;
                case IQueryable<GeneralDescriptionLanguageAvailability> gds:
                    return gds.Where(gd => gd.StatutoryServiceGeneralDescriptionVersionedId == id) as IQueryable<T>;
                default:
                    throw new NotImplementedException();
            }
        }

        public static IQueryable<T> WithEntityId<T>(this IQueryable<T> source, IEnumerable<Guid> ids)
            where T : ILanguageAvailability
        {
            switch (source)
            {
                case IQueryable<ServiceLanguageAvailability> services:
                    return services.Where(s => ids.Contains(s.ServiceVersionedId)) as IQueryable<T>;
                case IQueryable<ServiceChannelLanguageAvailability> channels:
                    return channels.Where(c => ids.Contains(c.ServiceChannelVersionedId)) as IQueryable<T>;
                case IQueryable<ServiceCollectionLanguageAvailability> collections:
                    return collections.Where(sc => ids.Contains(sc.ServiceCollectionVersionedId)) as IQueryable<T>;
                case IQueryable<OrganizationLanguageAvailability> organizations:
                    return organizations.Where(o => ids.Contains(o.OrganizationVersionedId)) as IQueryable<T>;
                case IQueryable<GeneralDescriptionLanguageAvailability> gds:
                    return gds.Where(gd => ids.Contains(gd.StatutoryServiceGeneralDescriptionVersionedId)) as IQueryable<T>;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
