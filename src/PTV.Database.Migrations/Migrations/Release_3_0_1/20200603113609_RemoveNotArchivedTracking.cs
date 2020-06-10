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
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;

namespace PTV.Database.Migrations.Migrations.Release_3_0_1
{
    public partial class RemoveNotArchivedTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} - Remove not archived content from tracking table.");
            
            this.AddMigrationAction(serviceProvider =>
            {
                var contextManager = serviceProvider.GetService<IContextManager>();
                var versionigManager = serviceProvider.GetService<IVersioningManager>();
                var commonService = serviceProvider.GetService<ICommonServiceInternal>();
                contextManager.ExecuteWriter(unitOfWork => RemoveNotArchivedContent(unitOfWork, versionigManager, commonService));
            });
        }

        private void RemoveNotArchivedContent(IUnitOfWorkWritable unitOfWork, IVersioningManager manager, ICommonServiceInternal commonService)
        {
            var serviceCount = 0;
            var channelCount = 0;
            var repo = unitOfWork.CreateRepository<ITrackingEntityVersionedRepository>();
            var servicesIds = repo.All().Where(x=>x.EntityType == EntityTypeEnum.Service.ToString()).Select(x=>x.UnificRootId).ToList();
            foreach (var id in servicesIds)
            {
                var status = manager.GetLatestVersionPublishingStatus<ServiceVersioned>(unitOfWork, id);
                if (status != PublishingStatus.Deleted)
                {
                    commonService.RemoveArchiveTracking(unitOfWork, id);
                    serviceCount++;
                }
            }
            var channelsIds = repo.All().Where(x=>x.EntityType == EntityTypeEnum.Channel.ToString()).Select(x=>x.UnificRootId).ToList();
            foreach (var id in channelsIds)
            {
                var status = manager.GetLatestVersionPublishingStatus<ServiceChannelVersioned>(unitOfWork, id);
                if (status != PublishingStatus.Deleted)
                {
                    commonService.RemoveArchiveTracking(unitOfWork, id);
                    channelCount++;
                }
            }

            unitOfWork.Save(SaveMode.AllowAnonymous);
            Console.WriteLine($"{DateTime.UtcNow} - Removed {serviceCount} services and {channelCount} channels.");
        }
        
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
