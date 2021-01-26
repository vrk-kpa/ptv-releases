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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework.Logging;

namespace PTV.Database.Migrations.Migrations.Release_2_7_2
{
    public partial class FillExpirationDates2 : Migration
    {
        private VmJobLogEntry entry = new VmJobLogEntry
        {
            ExecutionType = "Deployment",
            JobStatus = "Running",
            JobType = "PTVConsole",
            OperationId = nameof(FillExpirationDates2),
            UserName = "PTVConsole"
        }; 
        
        private const int BatchSize = 1000;
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var serviceBatchCounter = 0;
            var channelBatchCounter = 0;
            var utcNow = DateTime.UtcNow;

            this.AddMigrationAction(serviceProvider =>
            {
                var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<FillExpirationDates2>();
                var publishingStatusCache = serviceProvider.GetService<IPublishingStatusCache>();
                var contextManager = serviceProvider.GetService<IContextManager>();
                var expirationService = serviceProvider.GetService<IExpirationService>(); 
                
                var allowedStatuses = new List<Guid>
                {
                    publishingStatusCache.Get(PublishingStatus.Draft),
                    publishingStatusCache.Get(PublishingStatus.Modified),
                    publishingStatusCache.Get(PublishingStatus.Published)
                };

                while (EntitiesWithoutExpirationExist<ServiceVersioned>(contextManager, allowedStatuses))
                {
                    Console.WriteLine($"{DateTime.UtcNow} Processing service batch {++serviceBatchCounter}");
                    logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Processing service batch {serviceBatchCounter}");
                    FillDates<ServiceVersioned, ServiceLanguageAvailability>(contextManager,
                        expirationService, utcNow, allowedStatuses);
                }

                while (EntitiesWithoutExpirationExist<ServiceChannelVersioned>(contextManager, allowedStatuses))
                {
                    Console.WriteLine($"{DateTime.UtcNow} Processing channel batch {++channelBatchCounter}");
                    logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} Processing channel batch {channelBatchCounter}");
                    FillDates<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(contextManager,
                        expirationService, utcNow, allowedStatuses);
                }

                Console.WriteLine($"{DateTime.UtcNow}  No more entities without expiration date.");
                logger.LogSchedulerInfo(entry, $"{DateTime.UtcNow} No more entities without expiration date.");
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
        
        private void FillDates<TEntity, TLanguageAvailability>(IContextManager contextManager,
            IExpirationService expirationService, DateTime utcNow, List<Guid> allowedStatuses)
            where TEntity : class, IAuditing, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var repo = unitOfWork.CreateRepository<IRepository<TEntity>>();
                var batch = repo.All()
                    .Where(x => x.Expiration == null && allowedStatuses.Contains(x.PublishingStatusId))
                    .OrderBy(x => x.Id)
                    .Take(BatchSize)
                    .ToList();

                foreach (var entity in batch)
                {
                    expirationService.PolyfillExpirationDate<TEntity, TLanguageAvailability>(unitOfWork, entity, utcNow);
                }

                unitOfWork.Save(SaveMode.AllowAnonymous, PreSaveAction.DoNotSetAudits);
            });
        }

        private bool EntitiesWithoutExpirationExist<T>(IContextManager contextManager, List<Guid> allowedStatuses)
            where T : class, IExpirable, IVersionedVolume, new()
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var repo = unitOfWork.CreateRepository<IRepository<T>>();

                return repo.All().Any(x => x.Expiration == null && allowedStatuses.Contains(x.PublishingStatusId));
            });
        }
    }
}
