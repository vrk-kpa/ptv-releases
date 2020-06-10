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
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
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
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Localization;

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
        private ILogger<AddressService> logger;

        public AddressService(IContextManager contextManager, IResolveManager resolveManager, MapServiceProvider mapServiceProvider, ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            ICacheManager cacheManager,
            IVersioningManager versioniningManager,
            ILoggerFactory loggerFactory)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, versioniningManager)
        {
            this.resolveManager = resolveManager;
            this.mapServiceProvider = mapServiceProvider;
            this.contextManager = contextManager;
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            logger = loggerFactory.CreateLogger<AddressService>();
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
            return new VmAddressSimpleBase
            {
                Id = model.Id,
                Coordinates = new List<VmCoordinate> { GetCoordinates(model) }
            };
        }

        public VmCoordinate GetCoordinates(VmGetCoordinatesForAddressIn model)
        {
            try
            {
                var newGuid = Guid.NewGuid();
                var addressInfo = new AddressInfo
                {
                    Id = newGuid, MunicipalityCode = model.MunicipalityCode, Street = model.StreetName,
                    StreetNumber = model.StreetNumber
                };

                var coordinatesResult = mapServiceProvider.GetCoordinates(new List<AddressInfo> {addressInfo}).Result
                    .First();
                var coordinate = new VmCoordinate
                {
                    CoordinateState = coordinatesResult.State.ToString(),
                    Longitude = coordinatesResult.Longitude ?? 0,
                    Latitude = coordinatesResult.Latitude ?? 0,
                    IsMain = true,
                    Id = model.MainCoordinateId ?? Guid.NewGuid()
                };

                return coordinate;
            }
            catch (AggregateException ex)
            {
                logger.LogError(ex.Message, ex);
                throw new CoordinateException();
            }
        }

        public void AddNewStreetAddresses(IUnitOfWorkWritable unitOfWork, IEnumerable<VmAddressSimple> addresses, Dictionary<(Guid,string),Guid> newStreets)
        {
            var vmAddressSimples = addresses.ToList();

            var tmToEntity = resolveManager.Resolve<ITranslationViewModel>();

            var addressesStreetToAdd = vmAddressSimples
                .Where(x => !x.Id.IsAssigned() && x.StreetType == AddressTypeEnum.Street.ToString() && x.StreetName.Any() && x.Municipality.HasValue)
                .GroupBy(x => x.StreetName.First().Value.Trim().ToLower() + x.Municipality)
                .Where(x=>x.Any())
                .Select(x=>x.First());

                var streets = addressesStreetToAdd
                    .Select(vModel =>
                        new VmStreet
                        {
                            IsValid = false,
                            StreetNumbers = new List<VmStreetNumber> {new VmStreetNumber {PostalCode = vModel.PostalCode}},
                            Id = vModel.Street?.Id ?? Guid.Empty,
                            MunicipalityId = vModel.Municipality ?? Guid.Empty,
                            Translation = new VmTranslationItem {Texts = vModel.StreetName.ToDictionary(x=>x.Key, z=>z.Value.Trim().FirstCharToUpper()), DefaultText = vModel.StreetName.FirstOrDefault().Value.Trim().FirstCharToUpper()}
                        }
                    );
                streets.ForEach(street =>
                {
                    var streetEntity = tmToEntity.Translate<VmStreet, ClsAddressStreet>(street, unitOfWork);
                    newStreets.Add((street.MunicipalityId, street.Translation.DefaultText), streetEntity.Id);
                });


        }


        public void AddNewStreetAddressNumbers(IUnitOfWorkWritable unitOfWork, IEnumerable<VmAddressSimple> addresses, Dictionary<(Guid,string),Guid> newStreets)
        {
            var municipalityRep = unitOfWork.CreateRepository<IMunicipalityRepository>();
            var vmAddressSimples = addresses.ToList();
            vmAddressSimples.Where(x=>x.StreetType == AddressTypeEnum.Street.ToString() && x.StreetName.Any() && x.Municipality.HasValue && !x.StreetNumber.IsNullOrEmpty()).ForEach(
                adr =>
                {
                    var streetName = adr.StreetName.FirstOrDefault().Value.Trim().FirstCharToUpper();
                    if(adr.Coordinates.All(y => y.CoordinateState != CoordinateStates.Ok.ToString()))
                    {
                        adr.Coordinates = new List<VmCoordinate>
                        {
                            GetCoordinates(new VmGetCoordinatesForAddressIn
                            {
                                MunicipalityCode = municipalityRep.All().FirstOrDefault(x => x.Id == adr.Municipality)
                                    ?.Code,
                                StreetName = streetName,
                                StreetNumber = adr.StreetNumber.Trim(),
                            })
                        };
                    }
                    if (newStreets.TryGetValue((adr.Municipality ?? Guid.Empty, streetName), out var id))
                    {
                        adr.Street = new VmStreet
                        {
                            Id = id
                        };
                    }

                });

            var tmToEntity = resolveManager.Resolve<ITranslationViewModel>();

            var addressesToAdd = vmAddressSimples
                .Where(x => !x.Id.IsAssigned() && x.StreetType == AddressTypeEnum.Street.ToString() && x.StreetName.Any() && x.Municipality.HasValue && !x.StreetNumber.IsNullOrEmpty())
                .GroupBy(x => x.StreetName.First().Value.Trim().ToLower() + x.StreetNumber.Trim() + x.Municipality)
                .Select(x => x.OrderByDescending(y => y.Coordinates.Count).First());
            tmToEntity.TranslateAll<VmAddressSimple, ClsAddressStreetNumber>(addressesToAdd, unitOfWork);

            var validStreetIds = addressesToAdd
                .Where(x => x.Coordinates.Any(y => y.CoordinateState == CoordinateStates.Ok.ToString()))
                .Select(x => x.Street.Id);

            MakeValidStreets(unitOfWork, validStreetIds.ToList());
        }


        public void UpdateAddress(IList<Guid> addressIds)
        {
            if (!addressIds.Any()) return;

            resolveManager.RunInThread(rm =>
            {
                var contextManager = rm.Resolve<IContextManager>();
                var tmToVm = rm.Resolve<ITranslationEntity>();
                var tmToEntity = rm.Resolve<ITranslationViewModel>();
                var okState = CoordinateStates.Ok.ToString();
                var mainCoordinateTypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString());

                var addresses = LoadAddressEntities(addressIds, contextManager);

                foreach (var address in addresses)
                {
                    var addressCoordinatesExist = AddressHasValidCoordinates(address, okState, mainCoordinateTypeId);
                    var clsCoordinatesExist = ClsHasValidCoordinates(address, okState, mainCoordinateTypeId);
                    // Do not overwrite existing valid coordinates
                    if (addressCoordinatesExist && clsCoordinatesExist) continue;

                    var addressInfo = tmToVm.Translate<Address, AddressInfo>(address);
                    if (addressInfo == null) continue;

                    var coordinates = mapServiceProvider.GetCoordinates(new List<AddressInfo>{ addressInfo }).Result;
                    if (coordinates.IsNullOrEmpty()) continue;

                    var coordinate = coordinates.First();
                    contextManager.ExecuteWriter(unitOfWork =>
                    {
                        var clsStreetNumber = GetUpdatedStreetNumber(unitOfWork, address.Id,
                            coordinate.State == CoordinateStates.Ok);

                        if (!addressCoordinatesExist)
                        {
                            UpdateAddressCoordinates(tmToEntity, coordinate, unitOfWork, address);
                        }

                        if (!clsCoordinatesExist && coordinate.State == CoordinateStates.Ok)
                        {
                            UpdateClsStreetNumber(clsStreetNumber, coordinate, unitOfWork, tmToEntity);
                        }

                        unitOfWork.Save();
                    });
                }
            });
        }

        private void UpdateClsStreetNumber(ClsAddressStreetNumber clsStreetNumber, AddressInfo coordinate, IUnitOfWorkWritable unitOfWork,
            ITranslationViewModel tmToEntity)
        {
            if (clsStreetNumber == null) return;

            var coordinateModel = new VmCoordinate
            {
                Latitude = coordinate.Latitude ?? 0,
                Longitude = coordinate.Longitude ?? 0,
                IsMain = true,
                CoordinateState = coordinate.State.ToString(),
                OwnerReferenceId = clsStreetNumber.Id
            };

            var streetNumberCoordinateRepository = unitOfWork.CreateRepository<IClsStreetNumberCoordinateRepository>();
            var clsCoordinate =
                tmToEntity.Translate<VmCoordinate, ClsStreetNumberCoordinate>(coordinateModel, unitOfWork);
            clsCoordinate.RelatedToId = clsStreetNumber.Id;
            streetNumberCoordinateRepository.Add(clsCoordinate);
        }

        private void UpdateAddressCoordinates(ITranslationViewModel tmToEntity, AddressInfo coordinate,
            IUnitOfWorkWritable unitOfWork, Address address)
        {
            var coordinateAddress = tmToEntity.Translate<AddressInfo, Address>(coordinate, unitOfWork);
            var addressCoordinatesRepository = unitOfWork.CreateRepository<IAddressCoordinateRepository>();
            foreach (var addressCoordinate in coordinateAddress.Coordinates)
            {
                addressCoordinate.RelatedToId = address.Id;
                addressCoordinatesRepository.Add(addressCoordinate);
            }
        }

        private ClsAddressStreetNumber GetUpdatedStreetNumber(IUnitOfWorkWritable unitOfWork, Guid addressId, bool coordinatesFound)
        {
            var clsPointRepo = unitOfWork.CreateRepository<IClsAddressPointRepository>();
            var clsPoint = clsPointRepo.All()
                .Include(i => i.AddressStreet)
                .Include(i => i.AddressStreetNumber)
                .SingleOrDefault(p => p.AddressId == addressId);

            if (coordinatesFound)
            {
                if (clsPoint != null)
                {
                    clsPoint.IsValid = true;
                }

                if (clsPoint?.AddressStreet != null)
                {
                    clsPoint.AddressStreet.IsValid = true;
                }

                if (clsPoint?.AddressStreetNumber != null)
                {
                    clsPoint.AddressStreetNumber.IsValid = true;
                }
            }

            return clsPoint?.AddressStreetNumber;
        }

        private void MakeValidStreets(IUnitOfWorkWritable unitOfWork, List<Guid> ids)
        {
            var clsStreetRepo = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
            clsStreetRepo.All()
                .Where(p => ids.Contains(p.Id))
                .ForEach(s => { s.IsValid = true; });
        }

        private bool AddressHasValidCoordinates(Address address, string okState, Guid mainCoordinateTypeId)
        {
            var addressCoordinates = address?.Coordinates ?? new List<AddressCoordinate>();

            return !addressCoordinates.IsNullOrEmpty()
                   && addressCoordinates.Any(c => c.CoordinateState == okState && c.TypeId == mainCoordinateTypeId);
        }

        private bool ClsHasValidCoordinates(Address address, string okState, Guid mainCoordinateTypeId)
        {
            var clsCoordinates = address?.ClsAddressPoints?.FirstOrDefault()?.AddressStreetNumber?.Coordinates
                                 ?? new List<ClsStreetNumberCoordinate>();
            return !clsCoordinates.IsNullOrEmpty()
                   && clsCoordinates.Any(c => c.CoordinateState == okState && c.TypeId == mainCoordinateTypeId);
        }

        private List<Address> LoadAddressEntities(IList<Guid> addressIds, IContextManager contextManager)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var addressRepository = unitOfWork.CreateRepository<IAddressRepository>();
                var entities = addressRepository.All()
                    .Include(i => i.Coordinates)
                    .Include(i => i.ClsAddressPoints)
                        .ThenInclude(i => i.AddressStreet)
                        .ThenInclude(i => i.StreetNames)
                        .ThenInclude(i => i.Localization)
                    .Include(i => i.ClsAddressPoints)
                        .ThenInclude(i => i.AddressStreetNumber)
                        .ThenInclude(i => i.Coordinates)
                    .Include(i => i.ClsAddressPoints)
                        .ThenInclude(i => i.PostalCode)
                        .ThenInclude(i => i.Municipality)
                    .Include(i => i.ClsAddressPoints)
                        .ThenInclude(i => i.Municipality)
                    .Include(i => i.AddressPostOfficeBoxes)
                        .ThenInclude(i => i.PostOfficeBoxNames)
                        .ThenInclude(i => i.Localization)
                    .Include(i => i.AddressPostOfficeBoxes)
                        .ThenInclude(i => i.PostalCode)
                        .ThenInclude(i => i.Municipality)
                    .Include(i => i.AddressPostOfficeBoxes)
                        .ThenInclude(i => i.Municipality)
                    .Include(i => i.AddressForeigns)
                        .ThenInclude(i => i.ForeignTextNames)
                        .ThenInclude(i => i.Localization)
                    .Where(x => addressIds.Contains(x.Id));

                return entities.ToList();
            });
        }
    }
}
