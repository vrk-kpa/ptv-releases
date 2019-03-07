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
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Domain.Logic.Utilities;

namespace PTV.DataImport.ConsoleApp.Tasks
{
//    [RegisterService(typeof(ImportLanguageStateCultureToLanguageTask), RegisterType.Transient)]
    public class ImportLanguageStateCultureToLanguageTask
    {
        private IServiceProvider serviceProvider;
        private ILogger logger;
        private LanguageStateCultureUtility cultureUtility;
        
        public ImportLanguageStateCultureToLanguageTask(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            this.serviceProvider = serviceProvider;
            this.logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<ImportLanguageStateCultureToLanguageTask>();
            this.logger.LogDebug("ImportLanguageStateCultureToLanguageTask .ctor");
            cultureUtility = serviceProvider.GetService<LanguageStateCultureUtility>();
        }

        public void ImportData()
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var languageRepository = unitOfWork.CreateRepository<ILanguageRepository>();
                    var allLanguages = languageRepository.All();

                    foreach (var language in allLanguages)
                    {
                        language.LanguageStateCulture = cultureUtility.GetLanguageStateCulture(language.Code);
                    }

                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    Console.WriteLine($"Import: {allLanguages.Count()} language state cultures is complete.");
                });
            }
        }


    }
}
