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
using Newtonsoft.Json;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models.Base;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services
{
    public class ExportFintoService
    {
        private readonly IContextManager contextManager;
        private readonly OntologyTermDataCache ontologyTermCache;
        private readonly Guid finnish;
        private readonly Guid swedish;
        private readonly Guid english;

        public ExportFintoService(IServiceProvider serviceProvider)
        {
            contextManager = serviceProvider.GetService<IContextManager>();
            ontologyTermCache = serviceProvider.GetService<IOntologyTermDataCache>() as OntologyTermDataCache;
            var languageCache = serviceProvider.GetService<ILanguageCache>();
            finnish = languageCache.Get("fi");
            swedish = languageCache.Get("sv");
            english = languageCache.Get("en");
        }

        public void ExportFintoItems(string folderPath)
        {
            Console.WriteLine("Exporting target groups...");
            ExportTargetGroups(folderPath);
            Console.WriteLine("Exporting service classes...");
            ExportServiceClasses(folderPath);
            Console.WriteLine("Exporting life events...");
            ExportLifeEvents(folderPath);
            Console.WriteLine("Exporting industrial classes...");
            ExportIndustrialClasses(folderPath);
            Console.WriteLine("Exporting ontology terms...");
            ExportOntologyTerms(folderPath);
        }

        private void ExportOntologyTerms(string folderPath)
        {
            using var writer = new StreamWriter(Path.Combine(folderPath, "OntologyTerms.json"));
            var serializer = new JsonSerializer();
            var index = 0;
            writer.WriteLine("[");
            foreach (var item in ontologyTermCache.GetAllValid())
            {
                Console.WriteLine(index++);
                var exportItem = new ExportItem
                {
                    Id = item.Uri,
                    Label = ExportNames(item.Names),
                    Notation = item.Code,
                    ConceptType = item.OntologyType,
                    Notes = null,
                    BroaderUris = item.Parents.Select(x => x.Parent.Uri).ToList(),
                    NarrowerUris = item.Children.Select(x => x.Child.Uri).ToList(),
                    OldUri = item.Uri,
                    ExactMatchUris = ontologyTermCache.GetOntologyUrisForExactMatches(item.Uri).ToList()
                };
                
                serializer.Serialize(writer, exportItem);
                writer.Write(",");
            }
            writer.WriteLine("]");
        }

        private void ExportIndustrialClasses(string folderPath)
        {
            var cache = contextManager.ExecuteReader(unitOfWork =>
            {
                var repo = unitOfWork.CreateRepository<IIndustrialClassRepository>();
                return repo.All()
                    .Include(x => x.Names)
                    .Include(x => x.Descriptions)
                    .ToList()
                    .ToLookup(x => x.ParentYUri);
            });
            
            using var writer = new StreamWriter(Path.Combine(folderPath, "IndustrialClasses.json"));
            var serializer = new JsonSerializer();
            var index = 0;
            writer.WriteLine("[");
            foreach (var key in cache.Select(x => x.Key))
            {
                foreach (var item in cache[key])
                {
                    Console.WriteLine(index++);
                    var exportItem = new ExportItem
                    {
                        Id = item.YUri,
                        Label = ExportNames(item.Names),
                        Notation = item.Code,
                        ConceptType = item.OntologyType,
                        Notes = ExportDescriptions(item.Descriptions),
                        NarrowerUris = cache.TryGetOrDefault(item.YUri)?.Select(x => x.YUri).ToList(),
                        BroaderUris = item.ParentYUri == null ? null : new List<string> {item.ParentYUri},
                        OldUri = item.Uri,
                        ExactMatchUris = null
                    };
                    
                    serializer.Serialize(writer, exportItem);
                    writer.Write(",");
                }
            }
            writer.WriteLine("]");
        }

        private void ExportLifeEvents(string folderPath)
        {
            
            var cache = contextManager.ExecuteReader(unitOfWork =>
            {
                var repo = unitOfWork.CreateRepository<ILifeEventRepository>();
                return repo.All()
                    .Include(x => x.Names)
                    .ToList()
                    .ToLookup(x => x.ParentYUri);
            });
            
            using var writer = new StreamWriter(Path.Combine(folderPath, "LifeEvents.json"));
            var serializer = new JsonSerializer();
            var index = 0;
            writer.WriteLine("[");
            foreach (var key in cache.Select(x => x.Key))
            {
                foreach (var item in cache[key])
                {
                    Console.WriteLine(index++);
                    var exportItem = new ExportItem
                    {
                        Id = item.YUri,
                        Label = ExportNames(item.Names),
                        Notation = item.Code,
                        ConceptType = item.OntologyType,
                        Notes = null,
                        NarrowerUris = cache.TryGetOrDefault(item.YUri)?.Select(x => x.YUri).ToList(),
                        BroaderUris = item.ParentYUri == null ? null : new List<string> {item.ParentYUri},
                        OldUri = item.Uri,
                        ExactMatchUris = null
                    };
                    
                    serializer.Serialize(writer, exportItem);
                    writer.Write(",");
                }
            }
            writer.WriteLine("]");
        }

        private void ExportServiceClasses(string folderPath)
        {
            var cache = contextManager.ExecuteReader(unitOfWork =>
            {
                var repo = unitOfWork.CreateRepository<IServiceClassRepository>();
                return repo.All()
                    .Include(x => x.Names)
                    .Include(x => x.Descriptions)
                    .ToList()
                    .ToLookup(x => x.ParentYUri);
            });
            
            using var writer = new StreamWriter(Path.Combine(folderPath, "ServiceClasses.json"));
            var serializer = new JsonSerializer();
            var index = 0;
            writer.WriteLine("[");
            foreach (var key in cache.Select(x => x.Key))
            {
                foreach (var item in cache[key])
                {
                    Console.WriteLine(index++);
                    var exportItem = new ExportItem
                    {
                        Id = item.YUri,
                        Label = ExportNames(item.Names),
                        Notation = item.Code,
                        ConceptType = item.OntologyType,
                        Notes = ExportDescriptions(item.Descriptions),
                        NarrowerUris = cache.TryGetOrDefault(item.YUri)?.Select(x => x.YUri).ToList(),
                        BroaderUris = item.ParentYUri == null ? null : new List<string> {item.ParentYUri},
                        OldUri = item.Uri,
                        ExactMatchUris = null
                    };
                    
                    serializer.Serialize(writer, exportItem);
                    writer.Write(",");
                }
            }
            writer.WriteLine("]");
        }

        private void ExportTargetGroups(string folderPath)
        {
            var cache = contextManager.ExecuteReader(unitOfWork =>
            {
                var repo = unitOfWork.CreateRepository<ITargetGroupRepository>();
                return repo.All()
                    .Include(x => x.Names)
                    .ToList()
                    .ToLookup(x => x.ParentYUri);
            });
            
            using var writer = new StreamWriter(Path.Combine(folderPath, "TargetGroups.json"));
            var serializer = new JsonSerializer();
            var index = 0;
            writer.WriteLine("[");
            foreach (var key in cache.Select(x => x.Key))
            {
                foreach (var item in cache[key])
                {
                    Console.WriteLine(index++);
                    var exportItem = new ExportItem
                    {
                        Id = item.YUri,
                        Label = ExportNames(item.Names),
                        Notation = item.Code,
                        ConceptType = item.OntologyType,
                        Notes = null,
                        NarrowerUris = cache.TryGetOrDefault(item.YUri)?.Select(x => x.YUri).ToList(),
                        BroaderUris = item.ParentYUri == null ? null : new List<string> {item.ParentYUri},
                        OldUri = item.Uri,
                        ExactMatchUris = null
                    };
                    
                    serializer.Serialize(writer, exportItem);
                    writer.Write(",");
                }
            }
            writer.WriteLine("]");
        }

        private ExportTranslation ExportDescriptions<T>(ICollection<T> descriptions)
            where T: DescriptionBase
        {
            return new ExportTranslation
            {
                En = descriptions.FirstOrDefault(x => x.LocalizationId == english)?.Description,
                Fi = descriptions.FirstOrDefault(x => x.LocalizationId == finnish)?.Description,
                Sv = descriptions.FirstOrDefault(x => x.LocalizationId == swedish)?.Description
            };
        }

        private ExportTranslation ExportNames<T>(ICollection<T> names)
            where T: NameBase
        {
            return new ExportTranslation
            {
                En = names.FirstOrDefault(x => x.LocalizationId == english)?.Name,
                Fi = names.FirstOrDefault(x => x.LocalizationId == finnish)?.Name,
                Sv = names.FirstOrDefault(x => x.LocalizationId == swedish)?.Name
            };
        }

        private class ExportTranslation
        {
            public string Fi { get; set; }
            public string Sv { get; set; }
            public string En { get; set; }
        }
    
        private class ExportItem
        {
            public string Id { get; set; }
            public string Notation { get; set; }
            public string ConceptType { get; set; }
            public List<string> BroaderUris { get; set; }
            public List<string> NarrowerUris { get; set; }
            public ExportTranslation Label { get; set; }
            public List<string> ExactMatchUris { get; set; }
            public ExportTranslation Notes { get; set; }
            public string OldUri { get; set; }
        }
    }
}
