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
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Localization.Services.Model;
using PTV.Localization.Services.Services;
using PTV.TaskScheduler.Enums;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class LocalizationJob : BaseJob
    {
        private string fireInstanceId;

        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            fireInstanceId = context.FireInstanceId;
            if (fireInstanceId.IsNullOrEmpty()) throw new Exception("Could not load FireInstanceId!");
            var jobConfiguration = serviceProvider.GetService<IConfiguration>();
            var transifexConfiguration = serviceProvider.GetService<IOptions<TransifexConfiguration>>().Value;
            jobConfiguration.GetSection($"{JobKey}:JobData").Bind(transifexConfiguration);

            var downloadService = serviceProvider.GetService<IDownloadService>();
            var messageService = serviceProvider.GetService<IMessagesService>();

            var translationData = downloadService.Run();
            var messages = translationData.Translations.Select(data => new VmLanguageMessages
                {LanguageCode = data.Value.Language, Texts = data.Value.Translation});

            messageService.SaveMessages(messages);
            return $"Downloaded messages for languages {string.Join(", ", translationData.Translations.Values.Select(x => $"{x.Language}: {x.Translation.Count}"))}";
        }
    }
}
