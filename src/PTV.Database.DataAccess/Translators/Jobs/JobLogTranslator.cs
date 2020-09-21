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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Jobs
{
    [RegisterService(typeof(ITranslator<JobLog, VmJobLog>), RegisterType.Scope)]
    internal class JobLogTranslator : Translator<JobLog, VmJobLog>
    {
        private readonly IJobStatusCache jobStatusCache;

        public JobLogTranslator(
            IResolveManager resolveManager, 
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) 
            : base(resolveManager, translationPrimitives)
        {
            this.jobStatusCache = cacheManager.JobStatusCache;
        }

        public override VmJobLog TranslateEntityToVm(JobLog entity)
        {
            return CreateEntityViewModelDefinition<VmJobLog>(entity)
                .AddSimple(i => i.TimeUtc, o => o.TimeUtc)
                .AddNavigation(i => NLog.LogLevel.FromString(i.NlogLevel), o => o.Level)
                .AddSimple(i => i.JobStatusTypeId.HasValue
                    ? jobStatusCache.GetEnumByValue(i.JobStatusTypeId.Value)
                    : null, o => o.JobStatus)
                .AddNavigation(i => i.Message, o => o.Message)
                .AddNavigation(i => !i.Exception.IsNullOrEmpty() ? new VmJobException (i.Exception, i.StackTrace) : null,
                    o => o.Exception)
                .AddSimple(i => i.Id, o => o.Id)
                .GetFinal();
        }

        public override JobLog TranslateVmToEntity(VmJobLog vModel)
        {
            return CreateViewModelEntityDefinition<JobLog>(vModel)
                .UseDataContextUpdate(i => true,
                    i => o => i.TimeUtc == o.TimeUtc && i.Message == o.Message,
                    t => t.UseDataContextCreate(i => true))
                .AddSimple(i => i.TimeUtc, o => o.TimeUtc)
                .AddNavigation(i => i.Level.Name, o => o.NlogLevel)
                .AddSimple(i => i.JobStatus.HasValue
                    ? jobStatusCache.Get(i.JobStatus.Value) as Guid?
                    : null, o => o.JobStatusTypeId)
                .AddNavigation(i => i.Message, o => o.Message)
                .AddNavigation(i => i.Exception?.Message, o => o.Exception)
                .AddNavigation(i => i.Exception?.StackTrace, o => o.StackTrace)
                .GetFinal();
        }
    }
}
