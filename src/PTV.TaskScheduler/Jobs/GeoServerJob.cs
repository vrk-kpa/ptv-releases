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
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Models.Jobs;
using PTV.Framework;
using PTV.TaskScheduler.Configuration;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class GeoServerJob : BaseJob
    {

        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager, VmJobSummary jobSummary)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<GeoServerJobConfiguration>(context.JobDetail);

            var geoServerService = serviceProvider.GetService<IGeoServerService>();
            if (geoServerService == null) throw new Exception("Could not load geoServerService!");

            // run materialized views update commands
            if (!jobData.MaterializedViewNames.IsNullOrEmpty() && !jobData.MaterializedViewRefreshCommand.IsNullOrEmpty())
            {
                 //var commands = new List<string>();
                 jobData.MaterializedViewNames.WhereNotNull().ForEach(viewName =>
                 {
                     ExecuteCommandInternal(geoServerService, string.Format(jobData.MaterializedViewRefreshCommand, viewName), jobSummary);
                 });
            }

            // run update scripts
            if (!jobData.UpdateScripts.IsNullOrEmpty())
            {
                jobData.UpdateScripts.ForEach(scriptName =>
                {
                    ExecuteCommandInternal(geoServerService, $"select {scriptName}()", jobSummary);
                });
            }
            return null;
        }

        private void ExecuteCommandInternal(IGeoServerService service, string command, VmJobSummary jobSummary)
        {
            var commandResult = service.ExecuteCommand(command);
            TaskSchedulerLogger.LogJobInfo(jobSummary, commandResult, JobStatusService);
        }
    }
}
