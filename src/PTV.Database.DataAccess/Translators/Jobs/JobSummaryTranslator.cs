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
    [RegisterService(typeof(ITranslator<JobSummary, VmJobSummary>), RegisterType.Scope)]
    internal class JobSummaryTranslator : Translator<JobSummary, VmJobSummary>
    {
        private readonly IJobExecutionCache jobExecutionCache;

        public JobSummaryTranslator(
            IResolveManager resolveManager, 
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) 
            : base(resolveManager, translationPrimitives)
        {
            this.jobExecutionCache = cacheManager.JobExecutionCache;
        }

        public override VmJobSummary TranslateEntityToVm(JobSummary entity)
        {
            return CreateEntityViewModelDefinition<VmJobSummary>(entity)
                .AddSimple(i => i.StartUtc, o => o.StartUtc)
                .AddSimple(
                    i => i.JobExecutionTypeId.HasValue ? jobExecutionCache.GetEnumByValue(i.JobExecutionTypeId.Value) : null,
                    o => o.ExecutionType)
                .AddNavigation(i => i.JobType, o => o.JobType)
                .AddNavigation(i => i.OperationName, o => o.OperationId)
                .AddCollection(i => i.JobLogs, o => o.Logs)
                .AddSimple(i => i.Id, o => o.Id)
                .GetFinal();
        }

        public override JobSummary TranslateVmToEntity(VmJobSummary vModel)
        {
            return CreateViewModelEntityDefinition<JobSummary>(vModel)
                .UseDataContextUpdate(i => true,
                    i => o => i.JobType == o.JobType && i.OperationId == o.OperationName && i.StartUtc == o.StartUtc,
                    t => t.UseDataContextCreate(i => true))
                .AddSimple(i => i.StartUtc, o => o.StartUtc)
                .AddSimple(i => i.ExecutionType.HasValue ? jobExecutionCache.Get(i.ExecutionType.Value) as Guid? : null,
                    o => o.JobExecutionTypeId)
                .AddNavigation(i => i.JobType, o => o.JobType)
                .AddNavigation(i => i.OperationId, o => o.OperationName)
                .GetFinal();
        }
    }
}
