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
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Utils;
using PTV.ExternalSources;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Import;
using PTV.Database.Model.Interfaces;
using PTV.ExternalSources.Resources.Finto;
using PTV.ExternalSources.Resources.FintoFi;
using PTV.ExternalSources.Resources.Types;

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
                Dictionary<string, ImportLawGroup> laws = new Dictionary<string, ImportLawGroup>();
                foreach (var generalDescription in importedGeneralDescriptions)
                {
                    if (generalDescription.Laws == null)
                    {
                        continue;
                    }
                    generalDescription.UniqueLaws = generalDescription.Laws.Select(x =>
                    {
                        ImportLawGroup law;
                        if (laws.TryGetValue(x.LawFi.Name, out law))
                        {
                            return law;
                        }
                        laws.Add(x.LawFi.Name, x);
                        return x;
                    }).ToList();
                }
                var lawEntities = translationManagerToEntity.TranslateAll<ImportLawGroup, Law>(laws.Values).ToDictionary(x => x.Id);
                var generalDescriptions = translationManagerToEntity.TranslateAll<ImportStatutoryServiceGeneralDescription, StatutoryServiceGeneralDescription>(
                        importedGeneralDescriptions, unitOfWork);

                foreach (var gd in generalDescriptions)
                {
                    gd.StatutoryServiceLaws.ForEach(law =>
                        {
                            Law x;
                            if (!lawEntities.TryGetValue(law.LawId, out x))
                            {
                                throw new Exception($"{law.LawId}");
                            }
                            else
                            {
                                law.Law = x;
                            }
                        }
                    );
                    gd.ServiceClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.ServiceClasses, q => q.StatutoryServiceGeneralDescriptionId == gd.Id, sc => sc.ServiceClass?.Id ?? sc.ServiceClassId);
                    gd.IndustrialClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.IndustrialClasses, q => q.StatutoryServiceGeneralDescriptionId == gd.Id, sc => sc.IndustrialClass?.Id ?? sc.IndustrialClassId);
                    gd.OntologyTerms = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.OntologyTerms.Where(x => x != null).ToList(), q => q.StatutoryServiceGeneralDescriptionId == gd.Id, sc => sc.OntologyTerm?.Id ?? sc.OntologyTermId);
                    gd.LifeEvents = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.LifeEvents, q => q.StatutoryServiceGeneralDescriptionId == gd.Id, sc => sc.LifeEvent?.Id ?? sc.LifeEventId);
                    gd.TargetGroups = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, gd.TargetGroups, q => q.StatutoryServiceGeneralDescriptionId == gd.Id, sc => sc.TargetGroup?.Id ?? sc.TargetGroupId);
                }

                var ds =
                    generalDescriptions.Select(
                        x =>
                            new
                            {
                                x,
                                x.OntologyTerms.Count,
                                onto = x.OntologyTerms.Select(i => i.OntologyTermId).ToList()
                            });

                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }

    }
}
