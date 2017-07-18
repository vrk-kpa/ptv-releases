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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.Domain.Model.Models.Import;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IGeneralDescriptionUpdateService), RegisterType.Transient)]
    internal class GeneralDescriptionUpdateService : IGeneralDescriptionUpdateService
    {
        private IContextManager contextManager;
        private ITranslationViewModel translationManagerToEntity;
        private DataUtils dataUtils;
        private ILogger logger;

        public GeneralDescriptionUpdateService(IContextManager contextManager, ITranslationViewModel translationManagerToEntity, ILoggerFactory loggerFactory, DataUtils dataUtils)
        {
            this.contextManager = contextManager;
            this.translationManagerToEntity = translationManagerToEntity;
            this.dataUtils = dataUtils;
            this.logger = loggerFactory.CreateLogger<GeneralDescriptionUpdateService>();
        }

        public void CreateOrUpdateGeneralDescriptions(List<ImportStatutoryServiceGeneralDescription> importedGeneralDescriptions)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                Dictionary<string, ImportLaw> laws = new Dictionary<string, ImportLaw>();
                foreach (var generalDescription in importedGeneralDescriptions)
                {
                    if (generalDescription.Laws == null)
                    {
                        continue;
                    }
                    generalDescription.UniqueLaws = generalDescription.Laws.Select(x =>
                    {
                        ImportLaw law;
                        if (laws.TryGetValue(x.LawReference, out law))
                        {
                            return law;
                        }
                        laws.Add(x.LawReference, x);
                        return x;
                    }).ToList();
                }
                var lawEntities = translationManagerToEntity.TranslateAll<ImportLaw, Law>(laws.Values, unitOfWork);//.ToDictionary(x => x.Id);
                var generalDescriptions = translationManagerToEntity.TranslateAll<ImportStatutoryServiceGeneralDescription, StatutoryServiceGeneralDescriptionVersioned>(
                        importedGeneralDescriptions, unitOfWork);

                foreach (var gd in generalDescriptions)
                {

                    gd.StatutoryServiceLaws = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.StatutoryServiceLaws, q => q.StatutoryServiceGeneralDescriptionVersionedId == gd.Id, sc => sc.LawId);
                    gd.ServiceClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.ServiceClasses.Where(x => x != null).ToList(), q => q.StatutoryServiceGeneralDescriptionVersionedId == gd.Id, sc => sc.ServiceClass?.Id ?? sc.ServiceClassId);
                    gd.IndustrialClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.IndustrialClasses, q => q.StatutoryServiceGeneralDescriptionVersionedId == gd.Id, sc => sc.IndustrialClass?.Id ?? sc.IndustrialClassId);
                    gd.OntologyTerms = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.OntologyTerms.Where(x => x != null).ToList(), q => q.StatutoryServiceGeneralDescriptionVersionedId == gd.Id, sc => sc.OntologyTerm?.Id ?? sc.OntologyTermId);
                    gd.LifeEvents = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.LifeEvents.Where(x => x != null).ToList(), q => q.StatutoryServiceGeneralDescriptionVersionedId == gd.Id, sc => sc.LifeEvent?.Id ?? sc.LifeEventId);
                    gd.TargetGroups = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.TargetGroups.Where(x => x != null).ToList(), q => q.StatutoryServiceGeneralDescriptionVersionedId == gd.Id, sc => sc.TargetGroup?.Id ?? sc.TargetGroupId);
                }

                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }

    }
}
