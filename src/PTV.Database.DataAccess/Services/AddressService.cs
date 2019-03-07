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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IAddressService), RegisterType.Transient)]
    internal class AddressService : ServiceBase, IAddressService
    {
        private MapServiceProvider mapServiceProvider;
        private readonly IResolveManager resolveManager;
        private readonly IContextManager contextManager;
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        public AddressService(IContextManager contextManager, IResolveManager resolveManager, MapServiceProvider mapServiceProvider, ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            ICacheManager cacheManager,
            IVersioningManager versioniningManager)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, versioniningManager)
        {
            this.resolveManager = resolveManager;
            this.mapServiceProvider = mapServiceProvider;
            this.contextManager = contextManager;
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
        }

        public VmAddressSimpleBase GetAddress(VmGetCoordinatesForAddressIn model)
        {
            if (!model.Id.IsAssigned()) return null;
            VmAddressSimpleBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var addressQuery = unitOfWork.CreateRepository<IAddressRepository>().All().Where(x => x.Id == model.Id);
                var address = unitOfWork.ApplyIncludes(addressQuery, q => q
                    .Include(x => x.Coordinates)).FirstOrDefault();
                TranslationManagerToVm.SetLanguage(languageCache.Get(model.Language));
                result = address != null ? TranslationManagerToVm.Translate<Address, VmAddressSimpleBase>(address) : null;
            });

            return result;
        }

        public VmAddressSimpleBase GetAddressWithCoordinates(VmGetCoordinatesForAddressIn model)
        {
            return new VmAddressSimpleBase()
            {
                Id = model.Id,
                Coordinates = new List<VmCoordinate>() { GetCoordinates(model) }
            };
        }

        public VmCoordinate GetCoordinates(VmGetCoordinatesForAddressIn model)
        {
            var newGuid = Guid.NewGuid();
            var addressInfo = new AddressInfo() { Id = newGuid, MunicipalityCode = model.MunicipalityCode, Street = model.StreetName, StreetNumber = model.StreetNumber};

            var coordinatesResult = mapServiceProvider.GetCoordinates(new List<AddressInfo>() {addressInfo}).Result.First();
            var coordinate = new VmCoordinate()
            {
                CoordinateState = coordinatesResult.State.ToString(),
                Longitude = coordinatesResult.Longitude ?? 0,
                Latitude = coordinatesResult.Latitude ?? 0,
                IsMain = true,
                Id = model.MainCoordinateId ?? Guid.NewGuid()
            };

            return coordinate;
        }

        
        public void UpdateAddress(IList<Guid> addressIds)
        {
            if (!addressIds.Any()) return;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var coordinateRep = unitOfWork.CreateRepository<ICoordinateRepository>();
                coordinateRep.All().Where(x => x.TypeId == typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString()) && addressIds.Contains(x.AddressId)).ForEach(x => x.CoordinateState = CoordinateStates.Loading.ToString() + Guid.NewGuid().ToString());
                unitOfWork.Save();
            });

            resolveManager.RunInThread(rm =>
            {
                IReadOnlyList<AddressInfo> result = null;
                var contextManager = rm.Resolve<IContextManager>();
                var tmToVm = rm.Resolve<ITranslationEntity>();
                var tmToEntity = rm.Resolve<ITranslationViewModel>();

                contextManager.ExecuteReader(unitOfWork =>
                {
                    var addressRepositiory = unitOfWork.CreateRepository<IAddressRepository>();
                    var addresses = addressRepositiory.All().Where(x => addressIds.Contains(x.Id));
                    addresses = unitOfWork.ApplyIncludes(addresses, q =>
                        q
                            .Include(i => i.ClsAddressPoints)
                                .ThenInclude(i => i.AddressStreet)
                                .ThenInclude(i => i.StreetNames)
                                .ThenInclude(i => i.Localization)
                            .Include(i => i.ClsAddressPoints)
                                .ThenInclude(i => i.AddressStreet)
                                .ThenInclude(i => i.StreetNumbers)
                                .ThenInclude(i => i.PostalCode)
                                .ThenInclude(i => i.PostalCodeNames)
                                .ThenInclude(i => i.Localization)
                            .Include(i => i.ClsAddressPoints)
                                .ThenInclude(i => i.PostalCode)
                                .ThenInclude(i => i.PostalCodeNames)
                                .ThenInclude(i => i.Localization)
                            .Include(i => i.ClsAddressPoints)
                                .ThenInclude(i => i.Municipality)
                                .ThenInclude(i => i.MunicipalityNames)
                                .ThenInclude(i => i.Localization)
                            .Include(i => i.ClsAddressPoints)
                                .ThenInclude(i => i.AddressStreetNumber)
                            .Include(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostOfficeBoxNames).ThenInclude(i => i.Localization)
                            .Include(i => i.AddressPostOfficeBoxes).ThenInclude(i => i.PostalCode).ThenInclude(i => i.Municipality)
                            .Include(i => i.AddressForeigns).ThenInclude(i => i.ForeignTextNames).ThenInclude(i => i.Localization)
                    );

                    result = tmToVm.TranslateAll<Address, AddressInfo>(addresses);
                });
                if (!result.IsNullOrEmpty())
                {
                    var coordinates = mapServiceProvider.GetCoordinates(result).Result;

                    if (!coordinates.IsNullOrEmpty())
                    {
                        contextManager.ExecuteWriter(unitOfWork =>
                        {
                            tmToEntity.TranslateAll<AddressInfo, Address>(coordinates, unitOfWork);
                            unitOfWork.Save();
                        });
                    }
                }
            });
        }
    }
}
