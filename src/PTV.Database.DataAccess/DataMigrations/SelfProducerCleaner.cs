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
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.DataMigrations
{
    public class SelfProducerCleaner
    {
        private IContextManager contextManager;
        private ITypesCache typesCache;

        public SelfProducerCleaner(IServiceProvider serviceProvider)
        {
            this.contextManager = serviceProvider.GetService<IContextManager>();
            this.typesCache = serviceProvider.GetService<ITypesCache>();
        }

        public void RemoveInvalidSelfProducers()
        {
            var selfProducerTypeId = typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString());
            Console.WriteLine("Loading data...");
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var serviceProducerOrganizationRepo =
                    unitOfWork.CreateRepository<IServiceProducerOrganizationRepository>();

                var invalidSelfProducers = serviceProducerOrganizationRepo.All()
                    .Include(spo => spo.ServiceProducer)
                    .ThenInclude(sp => sp.ServiceVersioned)
                    .ThenInclude(sv => sv.OrganizationServices)
                    // only self producers
                    .Where(spo => spo.ServiceProducer.ProvisionTypeId == selfProducerTypeId
                                  // which are different from the main organization
                                  && spo.OrganizationId != spo.ServiceProducer.ServiceVersioned.OrganizationId
                                  // and are not included in other responsible organizations
                                  && spo.ServiceProducer.ServiceVersioned.OrganizationServices.All(os =>
                                      os.OrganizationId != spo.OrganizationId))
                    .ToList();

                Console.WriteLine("Processing self producers...");
                foreach (var selfProducer in invalidSelfProducers)
                {
                    serviceProducerOrganizationRepo.Remove(selfProducer);
                }

                Console.WriteLine("Saving changes...");
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
            Console.WriteLine("Done.");
        }
    }
}
