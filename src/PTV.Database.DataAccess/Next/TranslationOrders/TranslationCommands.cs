using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.TranslationOrders
{
    [RegisterService(typeof(ITranslationCommands), RegisterType.Transient)]
    internal class TranslationCommands : ITranslationCommands
    {
        private readonly IServiceService service;
        private readonly ITypesCache typesCache;
        private readonly ITranslationCompanyCache translationCompanyCache;

        public TranslationCommands(
            IServiceService service, 
            ITypesCache typesCache, 
            ITranslationCompanyCache translationCompanyCache)
        {
            this.service = service;
            this.typesCache = typesCache;
            this.translationCompanyCache = translationCompanyCache;
        }
        
        public bool OrderService(Guid serviceId, TranslationOrderModel model, out Guid newId)
        {
            var result = service.SendServiceEntityToTranslation(new VmTranslationOrderInput
            {
                AdditionalInformation = model.AdditionalInformation,
                EntityId = serviceId,
                RequiredLanguages = model.Targets.Select(x => typesCache.Get<Language>(x.ToString())).ToList(),
                SenderEmail = model.Email,
                SenderName = model.Subscriber,
                SourceLanguage = typesCache.Get<Language>(model.Source.ToString()),
                TranslationCompanyId = translationCompanyCache.GetAll().Single().Id
            });

            newId = result.Services.FirstOrDefault()?.Id ?? Guid.Empty;
            return result.Services.Count == 1;
        }
    }
}