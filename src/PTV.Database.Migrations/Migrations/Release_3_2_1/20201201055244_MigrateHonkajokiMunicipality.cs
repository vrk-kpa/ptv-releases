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
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Framework.Logging;

namespace PTV.Database.Migrations.Migrations.Release_3_2_1
{
    public partial class MigrateHonkajokiMunicipality : Migration
    {

        private VmJobLogEntry entry = new VmJobLogEntry
        {
            ExecutionType = "Deployment",
            JobStatus = "Running",
            JobType = "PTVConsole",
            OperationId = nameof(MigrateHonkajokiMunicipality),
            UserName = "PTVConsole"
        };
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            this.AddMigrationAction(serviceProvider =>
            {
                var contextManager = serviceProvider.GetService<IContextManager>();
                var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<MigrateHonkajokiMunicipality>();
                contextManager.ExecuteWriter(unitOfWork => SwapMunicipalities(unitOfWork, logger));
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }

        private void SwapMunicipalities(IUnitOfWorkWritable unitOfWork, ILogger<MigrateHonkajokiMunicipality> logger)
        {
            Console.WriteLine($"{DateTime.UtcNow} Removing Honkajoki municipality.");
            logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Removing Honkajoki municipality.");
            var municipalityRepo = unitOfWork.CreateRepository<IMunicipalityRepository>();

            var honkajoki = municipalityRepo.All().FirstOrDefault(x => x.Code == "099");
            var kankaanpaa = municipalityRepo.All().FirstOrDefault(x => x.Code == "214");

            if (honkajoki == null || kankaanpaa == null)
            {
                Console.WriteLine($"{DateTime.UtcNow} No Honkajoki or Kankaanpää municipality were found. Skipping migration.");
                logger.LogSchedulerWarn(entry, $"{DateTime.UtcNow} No Honkajoki or Kankaanpää municipality were found. Skipping migration.");
                return;
            }

            SwapPOBoxes(unitOfWork, honkajoki, kankaanpaa, logger);
            SwapAreas(unitOfWork, honkajoki, kankaanpaa, logger);
            SwapClsAddressPoints(unitOfWork, honkajoki, kankaanpaa, logger);
            SwapClsAddressStreets(unitOfWork, honkajoki, kankaanpaa, logger);
            SwapOrganizationAreaMunicipalities(unitOfWork, honkajoki, kankaanpaa, logger);
            SwapOrganizationVersioned(unitOfWork, honkajoki, kankaanpaa, logger);
            SwapPostalCodes(unitOfWork, honkajoki, kankaanpaa, logger);
            SwapServiceAreaMunicipality(unitOfWork, honkajoki, kankaanpaa, logger);
            SwapChannelAreaMunicipality(unitOfWork, honkajoki, kankaanpaa, logger);

            municipalityRepo.Remove(honkajoki);
            unitOfWork.Save(SaveMode.AllowAnonymous);
            Console.WriteLine($"{DateTime.UtcNow} Honkajoki replaced by Kankaanpää.");
            logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Honkajoki replaced by Kankaanpää.");
        }


        private void SwapChannelAreaMunicipality(IUnitOfWorkWritable unitOfWork, Municipality source, Municipality destination, ILogger<MigrateHonkajokiMunicipality> logger)
        {
            var repo = unitOfWork.CreateRepository<IServiceChannelAreaMunicipalityRepository>();
            var toRemove = repo.All()
                .Where(x => x.MunicipalityId == source.Id)
                .ToList();
            var areasWithDestination = repo.All()
                .Where(x => x.MunicipalityId == destination.Id)
                .Select(x => x.ServiceChannelVersionedId)
                .ToHashSet();
            var toAdd = new List<ServiceChannelAreaMunicipality>();

            Console.WriteLine($"{DateTime.UtcNow} Swapping {toRemove.Count} channel areas.");
            logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Swapping {toRemove.Count} channel areas.");
            foreach (var channelVersionedId in toRemove.Select(x => x.ServiceChannelVersionedId).Distinct())
            {
                if (!areasWithDestination.Contains(channelVersionedId))
                {
                    toAdd.Add(new ServiceChannelAreaMunicipality { ServiceChannelVersionedId = channelVersionedId, Municipality = destination });
                }
            }

            repo.Remove(toRemove);
            toAdd.ForEach(item => repo.Add(item));
            unitOfWork.Save(SaveMode.AllowAnonymous);
        }

        private void SwapServiceAreaMunicipality(IUnitOfWorkWritable unitOfWork, Municipality source, Municipality destination, ILogger<MigrateHonkajokiMunicipality> logger)
        {
            var repo = unitOfWork.CreateRepository<IServiceAreaMunicipalityRepository>();
            var toRemove = repo.All()
             .Where(x => x.MunicipalityId == source.Id)
             .ToList();
            var servicesWithDestination = repo.All()
             .Where(x => x.MunicipalityId == destination.Id)
             .Select(x => x.ServiceVersionedId)
             .ToHashSet();
            var toAdd = new List<ServiceAreaMunicipality>();

            Console.WriteLine($"{DateTime.UtcNow} Swapping {toRemove.Count} service areas.");
            logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Swapping {toRemove.Count} service areas.");
            foreach (var serviceVersionedId in toRemove.Select(x => x.ServiceVersionedId).Distinct())
            {
                if (!servicesWithDestination.Contains(serviceVersionedId))
                {
                    toAdd.Add(new ServiceAreaMunicipality { ServiceVersionedId = serviceVersionedId, Municipality = destination });
                }
            }

            repo.Remove(toRemove);
            toAdd.ForEach(item => repo.Add(item));
            unitOfWork.Save(SaveMode.AllowAnonymous);
        }

        private void SwapPostalCodes(IUnitOfWorkWritable unitOfWork, Municipality source, Municipality destination, ILogger<MigrateHonkajokiMunicipality> logger)
        {
            var repo = unitOfWork.CreateRepository<IPostalCodeRepository>();
            var toChange = repo.All()
                .Where(x => x.MunicipalityId == source.Id)
                .ToList();

            Console.WriteLine($"{DateTime.UtcNow} Swapping {toChange.Count} postal codes.");
            logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Swapping {toChange.Count} postal codes.");
            foreach (var code in toChange)
            {
                code.Municipality = destination;
            }

            unitOfWork.Save(SaveMode.AllowAnonymous);
        }

        private void SwapOrganizationVersioned(IUnitOfWorkWritable unitOfWork, Municipality source, Municipality destination, ILogger<MigrateHonkajokiMunicipality> logger)
        {
            var repo = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var toChange = repo.All()
                .Where(x => x.MunicipalityId == source.Id)
                .ToList();

            Console.WriteLine($"{DateTime.UtcNow} Swapping {toChange.Count} organizations.");
            logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Swapping {toChange.Count} organizations.");
            foreach (var organization in toChange)
            {
                organization.Municipality = destination;
            }

            unitOfWork.Save(SaveMode.AllowAnonymous, PreSaveAction.DoNotSetAudits);
        }

        private void SwapOrganizationAreaMunicipalities(IUnitOfWorkWritable unitOfWork, Municipality source, Municipality destination, ILogger<MigrateHonkajokiMunicipality> logger)
        {
            var repo = unitOfWork.CreateRepository<IOrganizationAreaMunicipalityRepository>();
            var toRemove = repo.All()
                .Where(x => x.MunicipalityId == source.Id)
                .ToList();
            var organizationsWithDestination = repo.All()
                .Where(x => x.MunicipalityId == destination.Id)
                .Select(x => x.OrganizationVersionedId)
                .ToHashSet();
            var toAdd = new List<OrganizationAreaMunicipality>();

            Console.WriteLine($"{DateTime.UtcNow} Swapping {toRemove.Count} organization areas.");
            logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Swapping {toRemove.Count} organization areas.");
            foreach (var organizationVersionedId in toRemove.Select(x => x.OrganizationVersionedId).Distinct())
            {
                if (!organizationsWithDestination.Contains(organizationVersionedId))
                {
                    toAdd.Add(new OrganizationAreaMunicipality
                    {
                        Municipality = destination,
                        OrganizationVersionedId = organizationVersionedId
                    });
                }
            }

            repo.Remove(toRemove);
            toAdd.ForEach(item => repo.Add(item));
            unitOfWork.Save(SaveMode.AllowAnonymous);
        }

        private void SwapClsAddressStreets(IUnitOfWorkWritable unitOfWork, Municipality source, Municipality destination, ILogger<MigrateHonkajokiMunicipality> logger)
        {
            var repo = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
            var toChange = repo.All()
                .Where(x => x.MunicipalityId == source.Id)
                .ToList();

            Console.WriteLine($"{DateTime.UtcNow} Swapping {toChange.Count} streets.");
            logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Swapping {toChange.Count} streets.");
            foreach (var street in toChange)
            {
                street.Municipality = destination;
            }

            unitOfWork.Save(SaveMode.AllowAnonymous);
        }

        private void SwapClsAddressPoints(IUnitOfWorkWritable unitOfWork, Municipality source, Municipality destination, ILogger<MigrateHonkajokiMunicipality> logger)
        {
            var repo = unitOfWork.CreateRepository<IClsAddressPointRepository>();
            var toChange = repo.All()
                .Where(x => x.MunicipalityId == source.Id)
                .ToList();

            Console.WriteLine($"{DateTime.UtcNow} Swapping {toChange.Count} CLS points.");
            logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Swapping {toChange.Count} CLS points.");
            foreach (var point in toChange)
            {
                point.Municipality = destination;
            }

            unitOfWork.Save(SaveMode.AllowAnonymous);
        }

        private void SwapAreas(IUnitOfWorkWritable unitOfWork, Municipality source, Municipality destination, ILogger logger)
        {
            var repo = unitOfWork.CreateRepository<IAreaMunicipalityRepository>();
            var toRemove = repo.All()
                .Where(x => x.MunicipalityId == source.Id)
                .ToList();
            var areasWithDestination = repo.All()
                .Where(x => x.MunicipalityId == destination.Id)
                .Select(x => x.AreaId)
                .ToHashSet();
            var toAdd = new List<AreaMunicipality>();

            Console.WriteLine($"{DateTime.UtcNow} Swapping {toRemove.Count} areas.");
            logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Swapping {toRemove.Count} areas.");
            foreach (var areaId in toRemove.Select(x => x.AreaId).Distinct())
            {
                if (!areasWithDestination.Contains(areaId))
                {
                    toAdd.Add(new AreaMunicipality { AreaId = areaId, Municipality = destination });
                }
            }

            repo.Remove(toRemove);
            toAdd.ForEach(item => repo.Add(item));
            unitOfWork.Save(SaveMode.AllowAnonymous);
        }

        private void SwapPOBoxes(IUnitOfWorkWritable unitOfWork, Municipality source, Municipality destination, ILogger<MigrateHonkajokiMunicipality> logger)
        {
            var repo = unitOfWork.CreateRepository<IAddressPostOfficeBoxRepository>();
            var toChange = repo.All()
                .Where(x => x.MunicipalityId == source.Id)
                .ToList();

            Console.WriteLine($"{DateTime.UtcNow} Swapping {toChange.Count} P.O.Boxes.");
            logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Swapping {toChange.Count} P.O.Boxes.");
            foreach (var box in toChange)
            {
                box.Municipality = destination;
            }

            unitOfWork.Save(SaveMode.AllowAnonymous);
        }

    }
}
