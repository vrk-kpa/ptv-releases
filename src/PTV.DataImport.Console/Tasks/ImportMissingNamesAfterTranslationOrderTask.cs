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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model;
using PTV.Framework;
using PTV.LocalAuthentication;

namespace PTV.DataImport.Console.Tasks
{
    [RegisterService(typeof(ImportTestAccountsTask), RegisterType.Transient)]
    public class ImportTestAccountsTask
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public ImportTestAccountsTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            logger = this.serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<ImportMissingNamesAfterTranslationOrderTask>();

            logger.LogDebug("ImportTestAccountsTask .ctor");
        }

        public void ImportAndUpdateUsers()
        {
            var usersList = JsonConvert.DeserializeObject<List<StsJsonUser>>(File.ReadAllText("TestTrnUsers.json") ?? string.Empty);
            if (!usersList.Any())
            {
                System.Console.WriteLine("No users found in TestTrnUsers.json file.");
                return;
            }
            System.Console.WriteLine($"{usersList.Count} users loaded from file.");
            var stsUserManager = serviceProvider.GetRequiredService<IStsPtvUserManager>();
            var notImported = stsUserManager.ImportUserJsonList(usersList);
            if (notImported.NoSavedUsers.Any())
            {
                System.Console.WriteLine($"Not imported users because of error in definition:{Environment.NewLine}{string.Join(Environment.NewLine,notImported.NoSavedUsers)}");
            }
            if (notImported.NoSavedMappings.Any())
            {
                System.Console.WriteLine($"Not imported users because of error in mappings, role or organization not found:{Environment.NewLine}{string.Join(Environment.NewLine,notImported.NoSavedMappings.Select(i => $"UserId:{i.Item1}/OrganizationId:{i.Item2}"))}");
            }
        }
    }



    [RegisterService(typeof(ImportMissingNamesAfterTranslationOrderTask), RegisterType.Transient)]
    public class ImportMissingNamesAfterTranslationOrderTask
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public ImportMissingNamesAfterTranslationOrderTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<ImportMissingNamesAfterTranslationOrderTask>();

            logger.LogDebug("ImportMissingNamesAfterTranslationOrderTask .ctor");
        }

        public void UpdateData()
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                var translationService = serviceScope.ServiceProvider.GetService<ITranslationService>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    System.Console.WriteLine("Import all missing entity names for translation is running...");
                    translationService.AddAllMissingEntityNamesAfterTranslationOrder(unitOfWork);
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    System.Console.WriteLine("Import all missing entity names for translation is is complete.");
                });
            }
            System.Console.WriteLine();
        }
    }
}
