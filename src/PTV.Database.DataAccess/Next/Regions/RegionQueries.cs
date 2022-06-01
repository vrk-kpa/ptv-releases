using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Database.DataAccess.Services;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.Regions
{
    [RegisterService(typeof(IRegionQueries), RegisterType.Transient)]
    internal class RegionQueries : IRegionQueries
    {
        private readonly IMunicipalityCache municipalityCache;
        private readonly IAreaCache areaCache;
        private readonly ILanguageCache languageCache;

        public RegionQueries(IMunicipalityCache municipalityCache, IAreaCache areaCache, ILanguageCache languageCache)
        {
            this.municipalityCache = municipalityCache;
            this.areaCache = areaCache;
            this.languageCache = languageCache;
        }
        
        public DateTime GetLastUpdate()
        {
            return CoreExtensions.Max(areaCache.GetLastUpdate(), municipalityCache.GetLastUpdate());
        }

        public List<AreaModel> GetProvinces()
        {
            return areaCache.GetProvinces().Select(x => x.ToModel(languageCache, municipalityCache)).ToList();
        }

        public List<AreaModel> GetHospitalRegions()
        {
            return areaCache.GetHospitalRegions().Select(x => x.ToModel(languageCache, municipalityCache)).ToList();
        }

        public List<AreaModel> GetBusinessRegions()
        {
            return areaCache.GetBusinessRegions().Select(x => x.ToModel(languageCache, municipalityCache)).ToList();
        }

        public List<MunicipalityModel> GetMunicipalities()
        {
            return municipalityCache.GetAll().Select(x => x.ToModel(languageCache)).ToList();
        }
    }
}