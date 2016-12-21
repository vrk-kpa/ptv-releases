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
                    var addressRepository = unitOfWork.CreateRepository<IAddressRepository>();
                    var allAddresses = addressRepository.All().Where(i => i.Latitude != null && i.Longtitude != null).ToList();
                    allAddresses.ForEach(address =>
                    {
                        var x = address.Latitude;
                        address.Latitude = address.Longtitude;
                        address.Longtitude = x;
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
                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var addressRepository = unitOfWork.CreateRepository<IAddressRepository>();
                    var allAddresses = addressRepository.All();
                    var addresses = allAddresses.Where(x => string.IsNullOrEmpty(x.CoordinateState));
                    addresses = unitOfWork.ApplyIncludes(addresses, q =>
                        q.Include(i => i.StreetNames)
                        .Include(i => i.PostalCode).ThenInclude(i => i.Municipality));

                    result = tmToVm.TranslateAll<Address, AddressInfo>(addresses);
                    result.AsParallel().ForEach(_applySplit);
                    var wrongFormatData = result.Where(x => x.State == CoordinateStates.WrongFormatReceived);

                    result = result.Where(x => x.State != CoordinateStates.WrongFormatReceived);
                    var allCount = allAddresses.Count();
                    var allWithoutCoordinates = addresses.Count();
                    var allWithValidStreet = result.Count();
                    tmToEntity.TranslateAll<AddressInfo, Address>(wrongFormatData, unitOfWork);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                    Console.WriteLine($"Addresses loaded: {allCount}, Addresses without coordinates: {allWithoutCoordinates}, Addresses with valid street: {allWithValidStreet}");
                });

                var addressesWithCoordinates = mapServiceProvider.GetCoordiantes(result.ToList(), false, true).Result;

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    tmToEntity.TranslateAll<AddressInfo, Address>(addressesWithCoordinates, unitOfWork);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                    var addresses = unitOfWork.CreateRepository<IAddressRepository>().All();
                    var countWithoutCoordinates = addresses.Count(x => string.IsNullOrEmpty(x.CoordinateState));
                    var countNotReceived = addresses.Count(x => x.CoordinateState == CoordinateStates.NotReceived.ToString());
                    var countWrongFormatReceived = addresses.Count(x => x.CoordinateState == CoordinateStates.WrongFormatReceived.ToString());
                    var countOk = addresses.Count(x => x.CoordinateState == CoordinateStates.Ok.ToString());
                    var otherStates = addresses.Count() - countOk - countNotReceived - countWithoutCoordinates - countWrongFormatReceived;
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
