using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.EnumTypes
{
    [RegisterService(typeof(IEnumQueries), RegisterType.Transient)]
    internal class EnumQueries : IEnumQueries
    {
        private readonly IOntologyTermDataCacheInternal ontologyTermDataCache;
        private readonly ILanguageCache languageCache;
        private readonly IWebHostEnvironment environment;
        private readonly AnnotationServiceProvider annotationProvider;
        private readonly IIndustrialClassCacheInternal industrialClassCache;
        private readonly IDigitalAuthorizationCacheInternal digitalAuthorizationCache;
        private readonly ILifeEventCacheInternal lifeEventCache;
        private readonly IServiceClassCacheInternal serviceClassCache;
        private readonly ITargetGroupCacheInternal targetGroupCache;
        private readonly IHolidayDateCache holidayDateCache;
        private readonly IServiceNumberCache serviceNumberCache;
        private readonly IDialCodeCache dialCodeCache;
        private readonly IPostalCodeCache postalCodeCache;
        private readonly ICountryCache countryCache;
        private readonly ILogger<EnumQueries> logger;

        public EnumQueries(
            IOntologyTermDataCache ontologyTermDataCache,
            AnnotationServiceProvider annotationProvider,
            IIndustrialClassCacheInternal industrialClassCache,
            IDigitalAuthorizationCacheInternal digitalAuthorizationCache,
            ILifeEventCacheInternal lifeEventCache,
            IServiceClassCacheInternal serviceClassCache,
            ITargetGroupCacheInternal targetGroupCache,
            IHolidayDateCache holidayDateCache,
            IServiceNumberCache serviceNumberCache,
            IDialCodeCache dialCodeCache,
            ILanguageCache languageCache,
            IWebHostEnvironment environment,
            IPostalCodeCache postalCodeCache,
            ICountryCache countryCache,
            ILogger<EnumQueries> logger)
        {
            this.ontologyTermDataCache = ontologyTermDataCache as IOntologyTermDataCacheInternal;
            this.annotationProvider = annotationProvider;
            this.industrialClassCache = industrialClassCache;
            this.digitalAuthorizationCache = digitalAuthorizationCache;
            this.lifeEventCache = lifeEventCache;
            this.serviceClassCache = serviceClassCache;
            this.targetGroupCache = targetGroupCache;
            this.languageCache = languageCache;
            this.environment = environment;
            this.holidayDateCache = holidayDateCache;
            this.serviceNumberCache = serviceNumberCache;
            this.dialCodeCache = dialCodeCache;
            this.postalCodeCache = postalCodeCache;
            this.countryCache = countryCache;
            this.logger = logger;
        }

        public List<OntologyModel> GetOntologyAll()
        {
            return ontologyTermDataCache.GetAllValid().Select(x => x.MapFamily(languageCache))
                .ToList();
        }

        public IEnumerable<OntologyModel> GetOntologyById(IEnumerable<Guid> ids)
        {
            var ontologies = ontologyTermDataCache.GetById(ids).Where(x => x != null);
            foreach (var ontology in ontologies)
            {
                yield return ontology.MapFamily(languageCache);
            }
        }

        public List<OntologyModel> SearchOntology(string expression, string languageCode)
        {
            var languageId = languageCache.Get(languageCode);
            return ontologyTermDataCache.SearchByName(expression, languageCode, new List<Guid> { languageId })
                .Select(x => x.MapFamily(languageCache)).ToList();
        }

        public List<OntologyModel> SearchByName(string name, string languageCode)
        {
            var languageId = languageCache.Get(languageCode);
            return ontologyTermDataCache.SearchOntologyTermByName(name, languageCode, new List<Guid> { languageId })
                .Select(x => x.MapFamily(languageCache)).ToList();
        }

        public DateTime CheckOntology()
        {
            return ontologyTermDataCache.GetLastUpdate();
        }

        public (AnnotationStates State, List<OntologyModel> Ontologies) GetAnnotations(ServiceInfo serviceInfo)
        {
            // Fake response for localhost
            if (environment.IsDevelopment())
            {
                var rng = new Random();
                var take = 10;
                var skip = rng.Next(0, ontologyTermDataCache.GetAllValid().Count - take);
                var result = ontologyTermDataCache.GetAllValid().Skip(skip).Take(take).Select(x => x.MapFamily(languageCache))
                    .ToList();
                return (AnnotationStates.Ok, result);
            }
            
            var annotationResult = annotationProvider.GetAnnotations(serviceInfo).GetAwaiter().GetResult();
            switch (annotationResult.State)
            {
                case AnnotationStates.Ok:
                    var uris = annotationResult.Annotations.Select(x => x.Uri).ToList();
                    var ontologies = uris.Select(x => ontologyTermDataCache.GetByUri(x)).Where(x => x != null);
                    return (AnnotationStates.Ok, ontologies.Select(x => x.MapFamily(languageCache)).ToList());
                default:
                    logger.LogError($"Annotation query failed and returned {annotationResult}.");
                    return (annotationResult.State, new List<OntologyModel>());
            }
        }

        public List<IndustrialClassModel> GetIndustrialClassAll()
        {
            // return industrialClassCache.GetAllValid().Select(x => x.ToModel(languageCache)).ToList();
            return industrialClassCache.Get125Flat();
        }

        public DateTime CheckIndustrialClass()
        {
            return industrialClassCache.GetLastUpdate();
        }

        public List<DigitalAuthorizationModel> GetDigitalAuthorizationAll()
        {
            return digitalAuthorizationCache.GetAsFintoTree().Select(x => x.ToModel(languageCache)).ToList();
        }

        public DateTime CheckDigitalAuthorization()
        {
            return digitalAuthorizationCache.GetLastUpdate();
        }

        public List<LifeEventModel> GetLifeEventAll()
        {
            return lifeEventCache.GetAllValid().Select(x => x.ToModel(languageCache)).ToList();
        }

        public DateTime CheckLifeEvent()
        {
            return lifeEventCache.GetLastUpdate();
        }

        public List<ServiceClassModel> GetServiceClassAll()
        {
            return serviceClassCache.GetAllValid().Select(x => x.ToModel(languageCache)).ToList();
        }

        public DateTime CheckServiceClass()
        {
            return serviceClassCache.GetLastUpdate();
        }

        public List<TargetGroupModel> GetTargetGroupAll()
        {
            return targetGroupCache.GetAllValid().Select(x => x.ToModel(languageCache)).ToList();
        }

        public DateTime CheckTargetGroup()
        {
            return targetGroupCache.GetLastUpdate();
        }

        public List<HolidayModel> GetHolidayAll()
        {
            return holidayDateCache.GetAll().Select(x => x.ToModel(languageCache)).ToList();
        }

        public DateTime CheckHoliday()
        {
            return holidayDateCache.GetAll().Max(x => x.Modified);
        }

        public List<ServiceNumberModel> GetServiceNumberAll()
        {
            return serviceNumberCache.GetAll().Select(x => x.ToModel()).ToList();
        }

        public DateTime CheckServiceNumber()
        {
            return serviceNumberCache.GetAll().Max(x => x.Modified);
        }

        public DateTime CheckDialCode()
        {
            var all = dialCodeCache.GetDialCodes();
            return CoreExtensions.Max(all.Max(x => x.Modified), all.Max(x => x.Country.Modified));
        }

        public List<DialCodeModel> GetDialCodeAll()
        {
            return dialCodeCache.GetDialCodes().Select(x => x.ToModel()).ToList();
        }

        public DateTime CheckLanguage()
        {
            return languageCache.GetLastUpdate();
        }

        public List<LanguageModel> GetLanguageAll()
        {
            return (languageCache as IInternalLanguageCache)?.GetAll()?.Select(x => x.ToModel(languageCache))?.ToList();
        }

        public List<PostalCodeModel> GetAllPostalCodes()
        {
            var all = postalCodeCache.GetAll();
            return postalCodeCache.GetAll().Select(x => x.ToModel(languageCache)).ToList();
        }

        public DateTime CheckPostalCode()
        {
            var all = postalCodeCache.GetAll();
            return CoreExtensions.Max(all.Max(x => x.Modified), all.SelectMany(x => x.PostalCodeNames.Select(x => x.Modified)).Max());
        }

        public List<CountryModel> GetAllCountries()
        {
            return countryCache.GetAll().Select(x => x.ToModel(languageCache)).ToList();
        }

        public DateTime CheckCountries()
        {
            var all = countryCache.GetAll();
            return CoreExtensions.Max(all.Max(x => x.Modified), all.SelectMany(x => x.CountryNames.Select(x => x.Modified)).Max());
        }
    }
}