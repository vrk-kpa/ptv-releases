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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace PTV.Framework
{
    /// <summary>
    /// Class holder for all performance measurement records
    /// </summary>
    [RegisterService(typeof(PerformanceStatisticsHolder), RegisterType.Singleton)]
    public class PerformanceStatisticsHolder
    {
        private const int MaxMeasuresRecords = 1000000;
        private const int MaxWorstMeasuresRecords = 50;
        
        private readonly ConcurrentQueue<RequestStatisticsMeasure> requestMeasures = new ConcurrentQueue<RequestStatisticsMeasure>();

        private readonly PerformanceMeasuringSettings settings;

        public PerformanceStatisticsHolder(IOptions<PerformanceMeasuringSettings> settings)
        {
            this.settings = settings.Value;
            if (this.settings.MaxMeasuresRecords <= 0)
            {
                this.settings.MaxMeasuresRecords = MaxMeasuresRecords;
            }
            if (this.settings.MaxWorstRecords <= 0)
            {
                this.settings.MaxWorstRecords = MaxWorstMeasuresRecords;
            }
        }

        public void Add(DateTime timestamp, IList<MeasuredRecord> data)
        {
            var rowMeasure = new RequestStatisticsMeasure(timestamp, data);
            requestMeasures.Enqueue(rowMeasure);
            if (requestMeasures.Count > settings.MaxMeasuresRecords)
            {
                requestMeasures.TryDequeue(out var removedItem);
            }
        }

        public List<RequestStatisticsMeasure> CalculateWorstMeasurements()
        {
            return requestMeasures.OrderByDescending(i => i.Data.LastOrDefault()?.Time).Take(settings.MaxWorstRecords).ToList();
        }

        public IList<RequestStatisticsMeasure> GetStatistics => requestMeasures.ToList();
    }
}