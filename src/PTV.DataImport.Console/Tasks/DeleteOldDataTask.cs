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
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Domain.Model.Enums;

namespace PTV.DataImport.Console.Tasks
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
            System.Console.WriteLine("* Delete old data - Services and Channels *");
            System.Console.Write("How old data should be deleted, enter date (YYYY-MM-DD): ");
            var date = System.Console.ReadLine();
            var dateTime = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (dateTime == default(DateTime))
            {
                System.Console.WriteLine("Wrong Date");
                return;
            }
            var ranGen = new Random();

            var verificationNumber = ranGen.Next(1000, 9999);
            System.Console.Write($"Are you really sure to delete services and channels? Type '{verificationNumber}' to confirm: ");
            var verificationString = System.Console.ReadLine();
            if (verificationNumber.ToString() != verificationString)
            {
                System.Console.WriteLine("Terminated.");
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
                    System.Console.WriteLine($"Archiving {servicesToDelete.Count} services and {channelsToDelete.Count} channels...");
                    servicesToDelete.ForEach(i => i.PublishingStatusId = psDelete);
                    channelsToDelete.ForEach(i => i.PublishingStatusId = psDelete);
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                    System.Console.WriteLine("Done.");
                });
            }
        }
    }
}
