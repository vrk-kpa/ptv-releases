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
using PTV.DataImport.ConsoleApp.Services;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using PTV.DataImport.ConsoleApp.Models;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Import;
using PTV.ExternalSources;
using PTV.Framework;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    [RegisterService(typeof(UpdateCreateGeneralDescriptionsTask), RegisterType.Transient)]
    public class UpdateCreateGeneralDescriptionsTask
    {
        private static readonly string GeneralDescriptionsGeneratedFile = Path.Combine("Generated", "GeneralDescription.json");
        private static readonly string GeneralDescriptionsReferenceCodeFile = Path.Combine("Generated", "general description mapping.txt");
        private const string DefaultCreatedBy = "CreateGeneralDescriptionsJsonTask";

        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public UpdateCreateGeneralDescriptionsTask(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreateGeneralDescriptionsJsonTask>();

            _logger.LogDebug("CreateGeneralDescriptionsJsonTask .ctor");
        }

        public void ImportDataFromJSON()
        {
            var importedGeneralDescriptions = JsonConvert.DeserializeObject<List<ImportStatutoryServiceGeneralDescription>>(File.ReadAllText(GeneralDescriptionsGeneratedFile), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var service = serviceScope.ServiceProvider.GetService<IGeneralDescriptionUpdateService>();
                FileInfo referenceCodeFile = new FileInfo(GeneralDescriptionsReferenceCodeFile);
                if (referenceCodeFile.Exists)
                {
                    var lines = File.ReadAllLines(GeneralDescriptionsReferenceCodeFile);
                    Console.WriteLine($"Matching {lines.Length} with reference code");
                    var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                    scopedCtxMgr.ExecuteWriter(unitOfWork =>
                    {
                        var generalDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>();
                        var names = lines.Select(x => x.Split(';')).ToDictionary(x => x[1].Trim('"'), x => x[0].Trim('"'));
                        var generalDescriptions =
                            unitOfWork.ApplyIncludes(
                                generalDescriptionRepository.All().Where(x => names.Keys.Contains(x.Name)
                                &&
                                x.StatutoryServiceGeneralDescription.ReferenceCode == null
                                )
                                ,
                                q => q.Include(x => x.StatutoryServiceGeneralDescription)).ToList();
                        Console.WriteLine($"Found {generalDescriptions.Count} general descriptions.");
                        foreach (var generalDescription in generalDescriptions)
                        {
                            generalDescription.StatutoryServiceGeneralDescription.ReferenceCode = names.TryGet(generalDescription.Name);
                            if (names.TryGet(generalDescription.Name) == null)
                            {
                                Console.WriteLine($"'{generalDescription.Name}'");
                            }
                        }

                        unitOfWork.Save(SaveMode.AllowAnonymous);
                    });

                }
                service?.CreateOrUpdateGeneralDescriptions(importedGeneralDescriptions);
            }
            Console.WriteLine();
        }
    }
}
