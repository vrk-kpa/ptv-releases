using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Next.ServiceChannels;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Next.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Next.GeneralDescriptions
{
    [RegisterService(typeof(IGeneralDescriptionQueries), RegisterType.Transient)]
    internal class GeneralDescriptionQueries : IGeneralDescriptionQueries
    {
        private readonly IContextManager contextManager;
        private readonly IGeneralDescriptionService gdService;
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly IGdServiceChannelQuery gdChannels;

        public GeneralDescriptionQueries(IGeneralDescriptionService gdService,
            ITypesCache typesCache,
            IContextManager contextManager,
            ILanguageCache languageCache,
            IGdServiceChannelQuery gdChannels)
        {
            this.gdService = gdService;
            this.typesCache = typesCache;
            this.contextManager = contextManager;
            this.languageCache = languageCache;
            this.gdChannels = gdChannels;
        }

        public GdSearchResultModel Search(GdSearchModel searchParameters)
        {
            var result = gdService.SearchServiceGds(searchParameters.Map(typesCache));
            return result.Map(this.typesCache);
        }

        public GeneralDescriptionModel Get(Guid versionedId, List<PublishingStatus> acceptedStatuses)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var repository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();

                var all = repository.All();
                if (acceptedStatuses.Any())
                {
                    var ids = acceptedStatuses.Select(x => typesCache.Get<PublishingStatusType>(x.ToString())).ToList();
                    all = all.Where(x => ids.Contains(x.PublishingStatusId));
                }

                var gd = all.Include(x => x.UnificRoot)
                    .ThenInclude(x => x.StatutoryServiceGeneralDescriptionServiceChannels)
                    .Include(x => x.LanguageAvailabilities)    
                    .Include(x => x.Names)
                    .Include(x => x.Descriptions)
                    .Include(x => x.Languages)
                    .Include(x => x.StatutoryServiceRequirements)
                    .Include(x => x.Keywords).ThenInclude(x => x.Keyword)
                    .Include(x => x.OntologyTerms).ThenInclude(x => x.OntologyTerm).ThenInclude(x => x.Names)
                    .FirstOrDefault(x => x.Id == versionedId);
                if (gd == null)
                {
                    return null;
                }

                var channels = GetChannels(gd);

                var gdWithClassifications = GetClassifications(repository, gd.Id);
                var laws = GetLaws(unitOfWork, gd.Id);

                var result = gd.ToModel(this.typesCache, this.languageCache, channels)
                    .FillInCategorization(gdWithClassifications, this.languageCache)
                    .FillInAdditionalInfo(laws, languageCache);

                return result;
            });
        }

        public GeneralDescriptionModel GetPublishedByUnificRootId(Guid unificRootId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var repository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var publishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());

                var gd = repository.All()
                    .Include(x => x.UnificRoot)
                    .ThenInclude(x => x.StatutoryServiceGeneralDescriptionServiceChannels)
                    .Include(x => x.LanguageAvailabilities)    
                    .Include(x => x.Names)
                    .Include(x => x.Descriptions)
                    .Include(x => x.Languages)
                    .Include(x => x.StatutoryServiceRequirements)
                    .Include(x => x.OntologyTerms)
                    .Include(x => x.Keywords).ThenInclude(x => x.Keyword)
                    .FirstOrDefault(x => x.UnificRootId == unificRootId && x.PublishingStatusId == publishedId);
                if (gd == null)
                {
                    return null;
                }

                var channels = GetChannels(gd);

                var gdWithClassifications = GetClassifications(repository, gd.Id);
                var laws = GetLaws(unitOfWork, gd.Id);

                var result = gd.ToModel(this.typesCache, this.languageCache, channels)
                    .FillInCategorization(gdWithClassifications, this.languageCache)
                    .FillInAdditionalInfo(laws, languageCache);

                return result;
            });
        }

        private StatutoryServiceGeneralDescriptionVersioned GetClassifications(IStatutoryServiceGeneralDescriptionVersionedRepository repository,
            Guid versionedId)
        {
            return repository.All()
                .Include(x => x.TargetGroups).ThenInclude(x => x.TargetGroup)
                .Include(x => x.IndustrialClasses)
                .Include(x => x.ServiceClasses)
                .Include(x => x.LifeEvents)
                .Include(x => x.OntologyTerms).ThenInclude(x => x.OntologyTerm).ThenInclude(x => x.Names)
                .FirstOrDefault(x => x.Id == versionedId);
        }

        private List<Law> GetLaws(IUnitOfWork unitOfWork, Guid versionedId)
        {
            var lawRepo = unitOfWork.CreateRepository<IStatutoryServiceLawRepository>();
            return lawRepo.All()
                .Include(x => x.Law).ThenInclude(x => x.Names)
                .Include(x => x.Law).ThenInclude(x => x.WebPages).ThenInclude(x => x.WebPage)
                .Where(x => x.StatutoryServiceGeneralDescriptionVersionedId == versionedId)
                .Select(x => x.Law)
                .Distinct()
                .ToList();
        }

        private List<GdServiceChannelModel> GetChannels(StatutoryServiceGeneralDescriptionVersioned gd)
        {
            var channelIds = gd.UnificRoot.StatutoryServiceGeneralDescriptionServiceChannels.Select(i => i.ServiceChannelId).ToList();
            return  gdChannels.GetForGeneralDescription(channelIds);
        }
    }
}
