using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.TranslationOrders
{
    [RegisterService(typeof(ITranslationQueries), RegisterType.Transient)]
    internal class TranslationQueries : ITranslationQueries
    {
        private readonly IContextManager contextManager;
        private readonly ITranslationService translationService;
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public TranslationQueries(IContextManager contextManager, ITranslationService translationService,
            ILanguageCache languageCache, ITypesCache typesCache)
        {
            this.contextManager = contextManager;
            this.translationService = translationService;
            this.languageCache = languageCache;
            this.typesCache = typesCache;
        }

        public TranslationDetailModel GetDetails(Guid id)
        {
            var entity = contextManager.ExecuteReader(unitOfWork =>
            {
                var orderRepo = unitOfWork.CreateRepository<ITranslationOrderRepository>();
                return orderRepo.All().FirstOrDefault(x => x.Id == id);
            });

            return entity.ToModel(languageCache);
        }

        public List<TranslationHistoryModel> GetServiceHistory(Guid serviceId, LanguageEnum language)
        {
            var languageId = languageCache.Get(language.ToString());
            var result = contextManager.ExecuteReader(unitOfWork =>
                translationService.GetServiceTranslationOrderStates(unitOfWork, serviceId, languageId));

            return result.TranslationOrderStates.Select(x => x.ToNewModel(typesCache)).ToList();
        }
    }
}