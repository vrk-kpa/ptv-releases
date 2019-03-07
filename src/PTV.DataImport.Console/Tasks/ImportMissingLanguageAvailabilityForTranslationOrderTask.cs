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
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using PTV.Framework;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.V2;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    [RegisterService(typeof(ImportMissingLanguageAvailabilityForTranslationOrderTask), RegisterType.Transient)]
    public class ImportMissingLanguageAvailabilityForTranslationOrderTask
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public ImportMissingLanguageAvailabilityForTranslationOrderTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreateServiceDataForGeneralDescriptionsJsonTask>();

            _logger.LogDebug("ImportMissingLanguageAvailabilityForTranslationOrderTask .ctor");
        }

        public void UpdateData()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                var translationService = serviceScope.ServiceProvider.GetService<ITranslationService>();
                
                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    Console.WriteLine($"Update all missing language version for translation is running...");
                    translationService.UpdateAllMissingLanguageVersionInTranslation(unitOfWork);
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    Console.WriteLine($"Update all missing language version for translation is complete.");
                });
            }
            Console.WriteLine();
        }
    }
}
