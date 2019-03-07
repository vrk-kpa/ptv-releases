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
using System.Diagnostics;
using System.Linq;

namespace PTV.Framework
{
    [RegisterService(typeof(IPerformanceMonitorManager), RegisterType.Scope)]
    public class PerformanceMonitorManager : IPerformanceMonitorManager
    {
        private List<MeasuredRecord> measures = new List<MeasuredRecord>();
        private readonly Dictionary<Guid, MeasuredRecord> onGoing = new Dictionary<Guid, MeasuredRecord>();
        private readonly List<string> ignoredClasses = new List<string>();

        public void Assign(IPerformanceMonitorManager performanceMonitorManager)
        {
            this.measures = (performanceMonitorManager as PerformanceMonitorManager)?.measures;
        }

        public void AddIgnoredClass(Type classType)
        {
            var strName = classType.Name;
            if (!ignoredClasses.Contains(strName))
            {
                ignoredClasses.Add(strName);
            }
        }

        public void AddIgnoredClass<T>()
        {
            AddIgnoredClass(typeof(T));
        }

        public Guid StartMeasuring()
        {
            return StartMeasuring(null);
        }
        
        public Guid StartMeasuring(string origin)
        {
            if (string.IsNullOrEmpty(origin))
            {
                var stack = new StackTrace().GetFrames();
                origin = string.Join('/', stack.Select(i => i?.GetMethod()).Where(i => i != null).Where(i => i.DeclaringType?.Name != this.GetType().Name).Where(i => !ignoredClasses.Contains(i.DeclaringType?.Name)).Take(2).Select(i => $"{i.DeclaringType?.Name ?? "unknown"}.{i.Name}").Reverse());
            }
            var measureId = Guid.NewGuid(); 
            onGoing[measureId] = new MeasuredRecord(origin, Environment.TickCount);
            return measureId;
        }

        public void StopMeasuring(Guid measureId)
        {
            StopMeasuring(measureId, null);
        }

        public void StopMeasuring(Guid measureId, string additionalText)
        {
            if (!onGoing.TryGetValue(measureId, out var measuring)) return;
            var time = Environment.TickCount - measuring.Time;
            measures.Add(new MeasuredRecord(measuring.Origin.Affix(additionalText), time));
            onGoing.Remove(measureId);
        }
        
        public void MeasureAction(Action action, string additionalText = null)
        {
            var measuringId = StartMeasuring(null);
            try
            {
                action();
            }
            finally
            {
                StopMeasuring(measuringId, additionalText);
            }
        }
        
        public T MeasureAction<T>(Func<T> action, string additionalText = null)
        {
            var measuringId = StartMeasuring(null);
            try
            {
                return action();
            }
            finally
            {
                StopMeasuring(measuringId, additionalText);
            }
        }

        public IList<MeasuredRecord> GetMeasures() => measures;
    }
}