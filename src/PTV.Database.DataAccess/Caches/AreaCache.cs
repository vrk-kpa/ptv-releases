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
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(IAreaCache), RegisterType.Singleton)]
    internal class AreaCache: IAreaCache
    {
        private readonly ITypesCache typesCache;
        private Dictionary<string, Area> provinces;
        private Dictionary<string, Area> businessRegions;
        private Dictionary<string, Area> hospitalRegions;
        private readonly IContextManager contextManager;

        public AreaCache(ITypesCache typesCache, IContextManager contextManager)
        {
            RefreshableCaches.Add(this);
            this.typesCache = typesCache;
            this.contextManager = contextManager;
            CreateCache(); 
        }

        private void CreateCache()
        {
            var provinceTypeId = typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString());
            var businessRegionTypeId = typesCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString());
            var hospitalRegionTypeId = typesCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString());

            contextManager.ExecuteReader(unitOfWork =>
            {
                var areaRepo = unitOfWork.CreateRepository<IAreaRepository>();
                var areaQuery = areaRepo.All()
                    .Include(x => x.AreaNames)
                    .Include(x => x.AreaMunicipalities);
                provinces = areaQuery.Where(x => x.AreaTypeId == provinceTypeId).ToDictionary(x => x.Code);
                businessRegions = areaQuery.Where(x => x.AreaTypeId == businessRegionTypeId).ToDictionary(x => x.Code);
                hospitalRegions = areaQuery.Where(x => x.AreaTypeId == hospitalRegionTypeId).ToDictionary(x => x.Code);
            });
        }
        
        public void CheckAndRefresh()
        {
            CreateCache();
        }

        public List<Area> GetProvinces()
        {
            return provinces.Values.ToList();
        }

        public List<Area> GetBusinessRegions()
        {
            return businessRegions.Values.ToList();
        }

        public List<Area> GetHospitalRegions()
        {
            return hospitalRegions.Values.ToList();
        }

        public DateTime GetLastUpdate()
        {
            return CoreExtensions.Max(
                hospitalRegions.Values.Max(x => x.Modified),
                hospitalRegions.Values.SelectMany(x => x.AreaNames.Select(y => y.Modified)).Max(),
                businessRegions.Values.Max(x => x.Modified),
                businessRegions.Values.SelectMany(x => x.AreaNames.Select(y => y.Modified)).Max(),
                provinces.Values.Max(x => x.Modified),
                provinces.Values.SelectMany(x => x.AreaNames.Select(y => y.Modified)).Max());
        }
    }
}
