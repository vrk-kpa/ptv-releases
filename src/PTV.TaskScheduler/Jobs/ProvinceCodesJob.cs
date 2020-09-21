﻿/**
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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Jobs;
using PTV.TaskScheduler.Configuration;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class ProvinceCodesJob : YPlatformCodesJob
    {
        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager, VmJobSummary jobSummary)
        {
            var jobData = QuartzScheduler.GetJobDataConfiguration<AreaCodeJobDataConfiguration>(context.JobDetail);
            var yPlatformConfiguration = GetApplicationJobConfiguration<YPlatformConfiguration>(context);

            var urlBase = string.Format(jobData.Url, yPlatformConfiguration.UrlBase, yPlatformConfiguration.ApiVersion,
                yPlatformConfiguration.Province);
            if (!IsConfigurationValid(urlBase, yPlatformConfiguration.Province, yPlatformConfiguration))
            {
                return $"{nameof(ProvinceCodesJob)} has invalid configuration, tested URL {urlBase}.";
            }

            var provinces = DownloadAreas(urlBase, yPlatformConfiguration);
            var validProvinces = GetValidAreas(provinces, AreaTypeEnum.Province);
            var result = ImportAreas(validProvinces, context, serviceProvider, contextManager);

            var relationsUrl = string.Format(jobData.MunicipalityUrl, yPlatformConfiguration.UrlBase, yPlatformConfiguration.ApiVersion,
                yPlatformConfiguration.Municipality, yPlatformConfiguration.ProvinceRelationship);
            var relations = DownloadRelations(relationsUrl, yPlatformConfiguration);
            result += " " + ImportRelations(validProvinces, relations, context, serviceProvider, contextManager);
            
            return result;
        }
    }
}