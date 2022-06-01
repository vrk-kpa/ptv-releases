using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.Import.Finto
{
    [RegisterService(typeof(IFintoImportService<OntologyTerm>), RegisterType.Transient)]
    internal class OntologyTermImportService : FintoImportServiceBase, IFintoImportService<OntologyTerm>
    {
        private readonly IContextManager contextManager;
        private readonly DataUtils dataUtils;
        private readonly ITranslationViewModel translationVmtoEnt;
        private ILogger<OntologyTermImportService> logger;

        public OntologyTermImportService(IContextManager contextManager, DataUtils dataUtils, ITranslationViewModel translationVmtoEnt, ILoggerFactory loggerFactory)
        {
            this.contextManager = contextManager;
            this.dataUtils = dataUtils;
            this.translationVmtoEnt = translationVmtoEnt;
            logger = loggerFactory.CreateLogger<OntologyTermImportService>();
        }

        public void SeedFintoItems(List<VmServiceViewsJsonItem> sourceItems, string userName)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SeedFintoItems(sourceItems, unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous, userName: userName);
            });
        }

        public string SeedFintoItems(List<VmServiceViewsJsonItem> ontologyItems, IUnitOfWorkWritable unitOfWork)
        {
            if (ontologyItems.Count == 0)
            {
                var message = "Ontology term source data empty";
                logger.LogInformation(message);
                return message;
            }

            logger.LogInformation("Loading Ontology terms...");
            var ontologyTermRepository = unitOfWork.CreateRepository<IOntologyTermRepository>();
            var current = ontologyTermRepository.All().ToDictionary(x => x.Uri);

            var ontologyTermParentRepository = unitOfWork.CreateRepository<IOntologyTermParentRepository>();
            var ontologyTermNameRepository = unitOfWork.CreateRepository<IOntologyTermNameRepository>();

            var main = GetMigratedFintoItems(ontologyItems);
            var sourceItems = ontologyItems.ToDictionary(x => x.Id);
            var forUpdate = new List<OntologyTerm>();
            var forInsert = new List<OntologyTerm>();

            var ontologyYso = main.Select(fintoItem =>
            {

                var replacedBy = fintoItem.Value.Main;
                OntologyTerm entity = null;
                if (fintoItem.Value.Replaced.Count == 0)
                {
                    entity = GetOntologyTermEntity(current, replacedBy, replacedBy, forUpdate, forInsert);
                }
                else
                {
                    List<OntologyTerm> existing = new List<OntologyTerm>();
                    List<OntologyTerm> inserting = new List<OntologyTerm>();
                    GetOntologyTermEntity(current, replacedBy, replacedBy, existing, new List<OntologyTerm>());

                    foreach (var item in fintoItem.Value.Replaced)
                    {
                        GetOntologyTermEntity(current, replacedBy, item.Item, existing, inserting);
                    }
                    existing.ForEach(ot =>
                    {
                        if (entity == null)
                        {
                            entity = ot;
                        }
                        else
                        {
                            ot.Code = "duplicated";
                        }
                    });
                    forUpdate.AddRange(existing);
                    if (entity == null && inserting.Count > 0)
                    {
                        entity = inserting.First();
                        forInsert.Add(entity);
                    }
                    else if (entity == null)
                    {
                        logger.LogError("Nothing to update for ontology");
                        throw new Exception("Nothing to update for ontology");
                    }
                }
                return entity;
            }).ToList();

            var ontologyTermParents = new List<OntologyTermParent>();
            dataUtils.JoinHierarchy(ontologyYso.ToDictionary(x => x.Uri), item => sourceItems[item.Uri].BroaderURIs, (parent, child) =>
            {
                ontologyTermParents.Add(new OntologyTermParent
                {
                    ChildId = child.Id,
                    ParentId = parent.Id
                });
            });

            logger.LogInformation($"Current ontology terms count: {current.Count}.");
            logger.LogInformation($"Importing ontology terms: {ontologyYso.Count}. Updating {forUpdate.Count}. New {forInsert.Count}. Not matched {current.Count - forUpdate.Count}.");

            logger.LogInformation("Removing old hierarchy..");
            ontologyTermParentRepository.DeleteAll();
            ontologyTermNameRepository.BatchDelete(x => x.OntologyTermId, forUpdate.Select(x => x.Id));

            logger.LogInformation("Inserting Ontology terms...");
            string userName = unitOfWork.GetUserNameForAuditing(SaveMode.AllowAnonymous);

            ontologyTermRepository.BatchUpdate(new OntologyTerm(), ot => ot.IsValid, null, null, userName);
            ontologyTermRepository.BatchInsert(forInsert, userName);
            ontologyTermRepository.BatchUpdate(forUpdate, ot => ot.Id, userName);
            ontologyTermParentRepository.BatchInsert(ontologyTermParents, userName);
            ontologyTermNameRepository.BatchInsert(ontologyYso.SelectMany(x => x.Names), userName);

            SeedExactMatches(ontologyItems, ontologyYso, unitOfWork);
            return
                $"Ontology terms update: {forInsert.Count} added, {forUpdate.Count} updated,  not matched {current.Count - forUpdate.Count}.";
        }

        private OntologyTerm GetOntologyTermEntity(Dictionary<string, OntologyTerm> current, VmServiceViewsJsonItem main, VmServiceViewsJsonItem replaced, List<OntologyTerm> forUpdate, List<OntologyTerm> forInsert)
        {
            var entity = translationVmtoEnt.Translate<VmServiceViewsJsonItem, OntologyTerm>(main);

            var otExisting = current.TryGet(replaced.Id) ?? current.TryGet(main.Id);
            if (otExisting != null)
            {
                entity.Id = otExisting.Id;
                entity.Created = otExisting.Created;
                entity.CreatedBy = otExisting.CreatedBy;
                forUpdate.Add(entity);
            }
            else
            {
                entity.Id = entity.Id.IsAssigned() ? entity.Id : Guid.NewGuid();
                forInsert.Add(entity);
            }
            entity.Names.ForEach(x => x.OntologyTermId = entity.Id);
            return entity;
        }

        private void SeedExactMatches(List<VmServiceViewsJsonItem> ontologyItems, List<OntologyTerm> ontologyYso, IUnitOfWorkWritable unitOfWork)
        {
            var userName = unitOfWork.GetUserNameForAuditing(SaveMode.AllowAnonymous);

            logger.LogInformation("Delete old ontology term exact matches...");
            var ontologyExactMatchRep = unitOfWork.CreateRepository<IOntologyTermExactMatchRepository>();
            ontologyExactMatchRep.DeleteAll();

            logger.LogInformation("Delete old exact matches...");
            var exactMatchRep = unitOfWork.CreateRepository<IExactMatchRepository>();
            exactMatchRep.DeleteAll();

            logger.LogInformation("Insert new exact matches...");
            var exactMatches = ontologyItems.Select(a => a.Id).Distinct().Select(x => new ExactMatch { Id = Guid.NewGuid(), Uri = x }).ToList();
            exactMatchRep.BatchInsert(exactMatches, userName);
            logger.LogInformation($"Inserted {exactMatches.Count} matches.");

            logger.LogInformation("Insert new ontology term exact matches...");
            var ontologyExactMatches = new List<OntologyTermExactMatch>();
            var sourceItems = ontologyItems.ToDictionary(x => x.Id);
            var exactMatchesItems = exactMatches.ToDictionary(x => x.Uri);
            
            foreach (var ontologyTerm in ontologyYso)
            {
                var exactMatch = sourceItems[ontologyTerm.Uri].Id;
                ontologyExactMatches.Add(new OntologyTermExactMatch
                {
                    ExactMatchId = exactMatchesItems[exactMatch].Id,
                    OntologyTermId = ontologyTerm.Id
                });
            }

            ontologyExactMatchRep.BatchInsert(ontologyExactMatches, userName);
        }
    }
}
