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
            var exportItems = new List<ExportItem>();
            foreach (var item in ontologyTermCache.GetAllValid())
            {
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

                exportItems.Add(exportItem);
            }

            SerializeToFile(Path.Combine(folderPath, "OntologyTerms.json"), exportItems);
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
            
            var exportItems = new List<ExportItem>();
            
            foreach (var key in cache.Select(x => x.Key))
            {
                foreach (var item in cache[key])
                {
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
                    
                    exportItems.Add(exportItem);
                }
            }
            
            SerializeToFile(Path.Combine(folderPath, "IndustrialClasses.json"), exportItems);
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
            
            var exportItems = new List<ExportItem>();
            foreach (var key in cache.Select(x => x.Key))
            {
                foreach (var item in cache[key])
                {
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
                    
                    exportItems.Add(exportItem);
                }
            }
            
            SerializeToFile(Path.Combine(folderPath, "LifeEvents.json"), exportItems);
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
            
            var exportItems = new List<ExportItem>();
            foreach (var key in cache.Select(x => x.Key))
            {
                foreach (var item in cache[key])
                {
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
                    
                    exportItems.Add(exportItem);
                }
            }
            
            SerializeToFile(Path.Combine(folderPath, "ServiceClasses.json"), exportItems);
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
            
            var exportItems = new List<ExportItem>();
            foreach (var key in cache.Select(x => x.Key))
            {
                foreach (var item in cache[key])
                {
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

                    exportItems.Add(exportItem);
                }
            }
            
            SerializeToFile(Path.Combine(folderPath, "TargetGroups.json"), exportItems);
        }

        private void SerializeToFile(string pathToFile, List<ExportItem> items)
        {
            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            File.WriteAllText(pathToFile, json);
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
