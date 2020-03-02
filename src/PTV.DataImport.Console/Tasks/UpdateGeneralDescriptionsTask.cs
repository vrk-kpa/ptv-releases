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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;

namespace PTV.DataImport.Console.Tasks
{
    [RegisterService(typeof(UpdateCreateGeneralDescriptionsTask), RegisterType.Transient)]
    public class UpdateCreateGeneralDescriptionsTask
    {
        private const string SourceDirectory = "";
        private readonly string generalDescriptionsGeneratedFile = "gd.json";
        private readonly string generalDescriptionsReferenceCodeFile = Path.Combine(SourceDirectory, "general description mapping.txt");

        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public UpdateCreateGeneralDescriptionsTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreateServiceDataForGeneralDescriptionsJsonTask>();

            logger.LogDebug("CreateGeneralDescriptionsJsonTask .ctor");
        }

        public void ImportDataFromJSON()
        {
            var readAllText = File.ReadAllText(Path.Combine(SourceDirectory, generalDescriptionsGeneratedFile));
            var importedGeneralDescriptions = JsonConvert.DeserializeObject<List<ImportStatutoryServiceGeneralDescription>>(readAllText, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            System.Console.WriteLine($"JSON file contains {importedGeneralDescriptions.Count} general descriptions");
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var service = serviceScope.ServiceProvider.GetService<IGeneralDescriptionUpdateService>();
                var referenceCodeFile = new FileInfo(generalDescriptionsReferenceCodeFile);
                if (referenceCodeFile.Exists)
                {
                    var lines = File.ReadAllLines(generalDescriptionsReferenceCodeFile);
                    System.Console.WriteLine($"Matching {lines.Length} with reference code");
                    var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                    scopedCtxMgr.ExecuteWriter(unitOfWork =>
                    {
                        var generalDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>();
                        var names = lines.Select(x => x.Split(';')).ToDictionary(x => x[1].Trim('"'), x => x[0].Trim('"'));
                        var generalDescriptions =
                            unitOfWork.ApplyIncludes(
                                generalDescriptionRepository.All().Where(x => names.Keys.Contains(x.Name) && x.StatutoryServiceGeneralDescriptionVersioned.ReferenceCode == null)
                                ,
                                q => q.Include(x => x.StatutoryServiceGeneralDescriptionVersioned)).ToList();
                        System.Console.WriteLine($"Found {generalDescriptions.Count} general descriptions.");
                        foreach (var generalDescription in generalDescriptions)
                        {
                            generalDescription.StatutoryServiceGeneralDescriptionVersioned.ReferenceCode = names.TryGet(generalDescription.Name);
                            if (names.TryGet(generalDescription.Name) == null)
                            {
                                System.Console.WriteLine($"'{generalDescription.Name}'");
                            }
                        }

                        unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    });

                }
                service?.CreateOrUpdateGeneralDescriptions(importedGeneralDescriptions);
            }
            System.Console.WriteLine();
        }

        public void DownloadFromDatabase()
        {
            //var readAllText = File.ReadAllText(Path.Combine(SourceDirectory, GeneralDescriptionsGeneratedFile));
            //var importedGeneralDescriptions = JsonConvert.DeserializeObject<List<ImportStatutoryServiceGeneralDescription>>(readAllText, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var service = serviceScope.ServiceProvider.GetService<IGeneralDescriptionUpdateService>();
                var res = service.DownloadGeneralDescriptions();
                System.Console.WriteLine($"General descriptions from DB: {res.Count}");
                File.WriteAllText("gd.json", JsonConvert.SerializeObject(res, Formatting.Indented));
            }
            System.Console.WriteLine();
        }
    }
}
