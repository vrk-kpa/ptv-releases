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
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Framework;

namespace PTV.DataImport.Console.Tasks
{
    [RegisterService(typeof(ImportMissingLanguageAvailabilityForTranslationOrderTask), RegisterType.Transient)]
    public class ImportMissingLanguageAvailabilityForTranslationOrderTask
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public ImportMissingLanguageAvailabilityForTranslationOrderTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreateServiceDataForGeneralDescriptionsJsonTask>();

            logger.LogDebug("ImportMissingLanguageAvailabilityForTranslationOrderTask .ctor");
        }

        public void UpdateData()
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                var translationService = serviceScope.ServiceProvider.GetService<ITranslationService>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    System.Console.WriteLine("Update all missing language version for translation is running...");
                    translationService.UpdateAllMissingLanguageVersionInTranslation(unitOfWork);
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    System.Console.WriteLine("Update all missing language version for translation is complete.");
                });
            }
            System.Console.WriteLine();
        }
    }
}
