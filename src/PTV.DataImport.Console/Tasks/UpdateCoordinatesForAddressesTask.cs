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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    public class UpdateCoordinatesForAddressesTask
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        private const string DefaultCreatedBy = "UpdateCoordinatesForAddresses";

        public UpdateCoordinatesForAddressesTask(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceProvider = serviceProvider;

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreateMunicipalityOrganizationsTask>();

            _logger.LogDebug("CreateMunicipalityOrganizationsTask .ctor");
        }


        public void SwitchCoordinates()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var coordinateRepository = unitOfWork.CreateRepository<ICoordinateRepository>();
                    coordinateRepository.All().ForEach(coordinate =>
                    {
                        var x = coordinate.Latitude;
                        coordinate.Latitude = coordinate.Longtitude;
                        coordinate.Longtitude = x;
                    });
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                });
            }
        }

        public void UpdateAddresses()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                var tmToVm = serviceScope.ServiceProvider.GetService<ITranslationEntity>();
                var tmToEntity = serviceScope.ServiceProvider.GetService<ITranslationViewModel>();
                var mapServiceProvider = serviceScope.ServiceProvider.GetService<MapServiceProvider>();
                IEnumerable<AddressInfo> result = null;
                scopedCtxMgr.ExecuteReader(unitOfWork =>
                {
                    var addressRepository = unitOfWork.CreateRepository<IAddressRepository>();
                    var allAddresses = addressRepository.All();
                    var addressesWithoutCoordinates = allAddresses.Where(a => !a.Coordinates.Any());

                    var addresses = unitOfWork.ApplyIncludes(addressesWithoutCoordinates, q =>
                       q.Include(i => i.StreetNames).ThenInclude(i => i.Localization)
                       .Include(i => i.PostalCode).ThenInclude(i => i.Municipality));

                    result = tmToVm.TranslateAll<Address, AddressInfo>(addresses);

                    //result.AsParallel().ForEach(_applySplit);
                    //var wrongFormatData = result.Where(x => x.State == CoordinateStates.WrongFormatReceived);

                    //result = result.Where(x => x.State != CoordinateStates.WrongFormatReceived);
                    //var allCount = allAddresses.Count();
                    //var allWithoutCoordinates = addresses.Count();
                    //var allWithValidStreet = result.Count();
                    //tmToEntity.TranslateAll<AddressInfo, Address>(wrongFormatData, unitOfWork);
                    //unitOfWork.Save(SaveMode.AllowAnonymous);

                    Console.WriteLine($"Addresses without coordinates loaded: {addresses.Count()}.");

                });

                var addressesWithCoordinates = mapServiceProvider.GetCoordinates(result.ToList(), false, true).Result;

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    tmToEntity.TranslateAll<AddressInfo, Address>(addressesWithCoordinates, unitOfWork);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                    var allAddresses = unitOfWork.CreateRepository<IAddressRepository>().All();
                    var addresses = unitOfWork.ApplyIncludes(allAddresses, q => q.Include(i => i.Coordinates));

                    // main coordinate type
                    var coordinateTypeRepository = unitOfWork.CreateRepository<ICoordinateTypeRepository>();
                    var mainCoordinateTypeCode = CoordinateTypeEnum.Main.ToString();
                    var mainCoordinateTypeId = coordinateTypeRepository.All().Where(x => x.Code == mainCoordinateTypeCode).First().Id;

                    // addresses without coordinates
                    var countWithoutCoordinates = addresses.Count(x => x.Coordinates.Count == 0);

                    // main coordinates statuses
                    var coordinateRepository = unitOfWork.CreateRepository<ICoordinateRepository>().All();
                    var mainCoordinates = coordinateRepository.Where(c => c.TypeId == mainCoordinateTypeId);
                    var countNotReceived = mainCoordinates.Count(c => c.CoordinateState == CoordinateStates.NotReceived.ToString());
                    var countWrongFormatReceived = mainCoordinates.Count(c => c.CoordinateState == CoordinateStates.WrongFormatReceived.ToString());
                    var countOk = mainCoordinates.Count(c => c.CoordinateState == CoordinateStates.Ok.ToString());
                    var otherStates = mainCoordinates.Count() - countOk - countNotReceived - countWrongFormatReceived;
                    Console.WriteLine($"Addresses Ok: {countOk}, Addresses without coordinates: {countWithoutCoordinates}, Addresses not received: {countNotReceived}, Addresses wrong format received: {countWrongFormatReceived}, Addresses in other states: {otherStates}");
                });
            }
        }

        private Action<AddressInfo> _applySplit = (addressInfo) =>
        {
            if (string.IsNullOrEmpty(addressInfo.Street))
            {
                addressInfo.State = CoordinateStates.WrongFormatReceived;
                return;
            }
            var split = addressInfo.Street.Split(' ').Select(x => x.Trim()).ToArray();
            if (split.Length >= 2 && split.Length <= 3 && split[0].All(char.IsLetter) &&
                ((split[1].All(char.IsDigit)) || !Regex.Match(split[1], @"^[\d-]+$").Success))
            {
                addressInfo.Street = split[0];

                if (split.Length == 3 && split[2].Length == 1 && char.IsLetter(split[2].First()))
                {

                    addressInfo.StreetNumber = split[1] + " " + split[2];
                }
                else
                {
                    addressInfo.StreetNumber = split[1];
                }
                return;
            }
            else if (split.Length == 1 && split[0].All(char.IsLetter))
            {
                return;
            }
            addressInfo.State = CoordinateStates.WrongFormatReceived;
        };
    }
}
