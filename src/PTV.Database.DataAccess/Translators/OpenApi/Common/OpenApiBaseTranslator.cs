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
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Enums;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    internal abstract class OpenApiBaseTranslator<TTranslateFrom, TTranslateTo> : Translator<TTranslateFrom, TTranslateTo> where TTranslateFrom : class where TTranslateTo : class
    {
        protected OpenApiBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives): base(resolveManager, translationPrimitives)
        {
        }

        protected class ServiceHourOrderComparer<TModelOpeningTime> : IComparer<IVmOpenApiServiceHourBase<TModelOpeningTime>>
             where TModelOpeningTime : IVmOpenApiDailyOpeningTime
        {
            // Type priority order for returning ServiceHours
            private readonly ServiceHoursTypeEnum[] serviceHourTypeOrder = new ServiceHoursTypeEnum[] { ServiceHoursTypeEnum.Standard, ServiceHoursTypeEnum.Special, ServiceHoursTypeEnum.Exception };

            public int Compare(IVmOpenApiServiceHourBase<TModelOpeningTime> x, IVmOpenApiServiceHourBase<TModelOpeningTime> y)
            {
                // Equal service hour types are ordered by Validity
                if (x.ServiceHourType == y.ServiceHourType)
                {
                    // Exception types are ordered by ValidFrom date
                    if (x.ServiceHourType == ServiceHoursTypeEnum.Exception.ToString())
                    {
                        if (!x.ValidFrom.HasValue)
                        {
                            return -1;
                        }
                        if (!y.ValidFrom.HasValue)
                        {
                            return 1;
                        }
                        return x.ValidFrom.Value.CompareTo(y.ValidFrom.Value);
                    }

                    // Other types are ordered by ValidForNow (with reverse boolean logic, thus y->x instead of x->y)
                    return y.ValidForNow.CompareTo(x.ValidForNow);
                }

                // Return in type priority order
                return Array.IndexOf(serviceHourTypeOrder, Enum.Parse(typeof(ServiceHoursTypeEnum), x.ServiceHourType))
                    .CompareTo(Array.IndexOf(serviceHourTypeOrder, Enum.Parse(typeof(ServiceHoursTypeEnum), y.ServiceHourType)));
            }
        }
    }
}
