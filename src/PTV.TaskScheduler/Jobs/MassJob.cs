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
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.Logging;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using PTV.TaskScheduler.Models;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class MassJob : BaseJob
    {
        public override JobTypeEnum JobType => JobTypeEnum.MassTool;

        private string fireInstanceId;
        private IMassService massService;

        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            fireInstanceId = context.FireInstanceId;
            if (fireInstanceId.IsNullOrEmpty()) throw new Exception("Could not load FireInstanceId!");
            
            massService = serviceProvider.GetService<IMassService>();
            if (massService == null) throw new Exception("Could not load massService!");

            var currentTime = DateTime.UtcNow;
            massService.PublishScheduledLanguageVersions(currentTime);
            massService.ArchiveScheduledLanguageVersions(currentTime);
            
            return null;
        }
    }
}