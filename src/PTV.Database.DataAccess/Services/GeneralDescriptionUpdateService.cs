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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Domain.Model.Models.Import;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IGeneralDescriptionUpdateService), RegisterType.Transient)]
    internal class GeneralDescriptionUpdateService : IGeneralDescriptionUpdateService
    {
        private IContextManager contextManager;
        private ITranslationEntity transaltionManager;
        private ITranslationViewModel translationManagerToEntity;

        public GeneralDescriptionUpdateService(IContextManager contextManager, ITranslationViewModel translationManagerToEntity, ITranslationEntity transaltionManager)
        {
            this.contextManager = contextManager;
            this.translationManagerToEntity = translationManagerToEntity;
            this.transaltionManager = transaltionManager;
        }

        public void CreateOrUpdateGeneralDescriptions(List<ImportStatutoryServiceGeneralDescription> importedGeneralDescriptions)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
//                Dictionary<string, ImportLaw> laws = new Dictionary<string, ImportLaw>();
//                foreach (var generalDescription in importedGeneralDescriptions)
//                {
//                    if (generalDescription.Laws == null)
//                    {
//                        continue;
//                    }
//                    generalDescription.UniqueLaws = generalDescription.Laws.Select(x =>
//                    {
//                        ImportLaw law;
//                        if (laws.TryGetValue(x.LawReference, out law))
//                        {
//                            return law;
//                        }
//                        laws.Add(x.LawReference, x);
//                        return x;
//                    }).ToList();
//                }
                var lawEntities = translationManagerToEntity.TranslateAll<ImportLaw, Law>(importedGeneralDescriptions.SelectMany(i => i.Laws), unitOfWork);//.ToDictionary(x => x.Id);
                unitOfWork.Save(SaveMode.AllowAnonymous);
                var generalDescriptions = translationManagerToEntity.TranslateAll<ImportStatutoryServiceGeneralDescription, StatutoryServiceGeneralDescriptionVersioned>(
                        importedGeneralDescriptions, unitOfWork);

//                foreach (var gd in generalDescriptions)
//                {
//
//                    gd.StatutoryServiceLaws = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.StatutoryServiceLaws, q => q.StatutoryServiceGeneralDescriptionVersionedId == gd.Id, sc => sc.LawId);
//                    gd.ServiceClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.ServiceClasses.Where(x => x != null).ToList(), q => q.StatutoryServiceGeneralDescriptionVersionedId == gd.Id, sc => sc.ServiceClass?.Id ?? sc.ServiceClassId);
//                    gd.IndustrialClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.IndustrialClasses, q => q.StatutoryServiceGeneralDescriptionVersionedId == gd.Id, sc => sc.IndustrialClass?.Id ?? sc.IndustrialClassId);
//                    gd.OntologyTerms = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.OntologyTerms.Where(x => x != null).ToList(), q => q.StatutoryServiceGeneralDescriptionVersionedId == gd.Id, sc => sc.OntologyTerm?.Id ?? sc.OntologyTermId);
//                    gd.LifeEvents = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.LifeEvents.Where(x => x != null).ToList(), q => q.StatutoryServiceGeneralDescriptionVersionedId == gd.Id, sc => sc.LifeEvent?.Id ?? sc.LifeEventId);
//                    gd.TargetGroups = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.TargetGroups.Where(x => x != null).ToList(), q => q.StatutoryServiceGeneralDescriptionVersionedId == gd.Id, sc => sc.TargetGroup?.Id ?? sc.TargetGroupId);
//                }

                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }

        public List<ImportStatutoryServiceGeneralDescription> DownloadGeneralDescriptions()
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var gds = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>().All();
                gds = unitOfWork.ApplyIncludes(gds, i => i
                    .Include(j => j.Names)
                    .Include(x => x.ServiceClasses).ThenInclude(x => x.ServiceClass).ThenInclude(x => x.Names)
                    .Include(x => x.OntologyTerms).ThenInclude(x => x.OntologyTerm).ThenInclude(x => x.Names)
                    .Include(x => x.LifeEvents).ThenInclude(x => x.LifeEvent).ThenInclude(x => x.Names)
                    .Include(x => x.IndustrialClasses).ThenInclude(x => x.IndustrialClass).ThenInclude(x => x.Names)
                    .Include(x => x.TargetGroups).ThenInclude(x => x.TargetGroup)
                    .Include(j => j.Descriptions)
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.Versioning)
                    .Include(j => j.StatutoryServiceRequirements)
                    .Include(j => j.StatutoryServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.Names)
                    .Include(j => j.StatutoryServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.WebPages).ThenInclude(j => j.WebPage)
                    .Include(j => j.StatutoryServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.WebPages).ThenInclude(j => j.Localization)
                    .Include(j => j.UnificRoot).ThenInclude(j => j.StatutoryServiceGeneralDescriptionServiceChannels).ThenInclude(j => j.ServiceChannel).ThenInclude(j => j.Versions).ThenInclude(j => j.ServiceChannelNames)
                    .Include(j => j.UnificRoot).ThenInclude(j => j.StatutoryServiceGeneralDescriptionServiceChannels).ThenInclude(j => j.ServiceChannel).ThenInclude(j => j.Versions).ThenInclude(j => j.LanguageAvailabilities)
                    .Include(j => j.UnificRoot).ThenInclude(j => j.StatutoryServiceGeneralDescriptionServiceChannels)
                );

                return transaltionManager
                    .TranslateAll<StatutoryServiceGeneralDescriptionVersioned, ImportStatutoryServiceGeneralDescription
                    >(gds).ToList();
            });
        }
    }
}
