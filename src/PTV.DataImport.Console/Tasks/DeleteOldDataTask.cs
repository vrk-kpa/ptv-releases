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
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Domain.Model.Enums;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    public class DeleteOldDataTask
    {
        private readonly IServiceProvider serviceProvider;

        public DeleteOldDataTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void DeleteOldData()
        {
            Console.WriteLine("* Delete old data - Services and Channels *");
            Console.Write("How old data should be deleted, enter date (YYYY-MM-DD): ");
            var date = Console.ReadLine();
            var dateTime = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (dateTime == default(DateTime))
            {
                Console.WriteLine("Wrong Date");
                return;
            }
            var ranGen = new Random();

            int verificationNumber = ranGen.Next(1000, 9999);
            Console.Write($"Are you really sure to delete services and channels? Type '{verificationNumber}' to confirm: ");
            var verificationString = Console.ReadLine();
            if (verificationNumber.ToString() != verificationString)
            {
                Console.WriteLine("Terminated.");
                return;
            }

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    var psRep = unitOfWork.CreateRepository<IPublishingStatusTypeRepository>();
                    var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    var psDraft = psRep.All().Where(i => i.Code == PublishingStatus.Draft.ToString()).Select(i => i.Id).First();
                    var psDelete = psRep.All().Where(i => i.Code == PublishingStatus.Deleted.ToString()).Select(i => i.Id).First();
                    var servicesToDelete = serviceRep.All().Where(i => i.Modified < dateTime && i.PublishingStatusId == psDraft).ToList();
                    var channelsToDelete = channelRep.All().Where(i => i.Modified < dateTime && i.PublishingStatusId == psDraft).ToList();
                    Console.WriteLine($"Archiving {servicesToDelete.Count} services and {channelsToDelete.Count} channels...");
                    servicesToDelete.ForEach(i => i.PublishingStatusId = psDelete);
                    channelsToDelete.ForEach(i => i.PublishingStatusId = psDelete);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                    Console.WriteLine($"Done.");
                });
            }
        }
    }
}
