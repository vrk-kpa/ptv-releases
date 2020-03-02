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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.V2
{
    [RegisterService(typeof(ILawService), RegisterType.Transient)]
    internal class LawService : ServiceBase, ILawService
    {
        public LawService(
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IVersioningManager versioningManager)
            : base(
                translationManagerToVm,
                translationManagerToEntity,
                publishingStatusCache,
                userOrganizationChecker,
                versioningManager)
        {
        }

        public IReadOnlyList<ServiceLaw> SaveServiceLaws(List<VmLaw> vmLaws, Guid versionedId, IUnitOfWorkWritable unitOfWork)
        {
            var serviceLawRepo = unitOfWork.CreateRepository<IServiceLawRepository>();
            var lawRepo = unitOfWork.CreateRepository<ILawRepository>();
            
            var toRemove = serviceLawRepo.All()
                .Include(x => x.Law)
                .Where(x => x.ServiceVersionedId == versionedId)
                .ToList();

            serviceLawRepo.Remove(toRemove);
            lawRepo.Remove(toRemove.Select(x => x.Law));
            unitOfWork.Save();
            
            var lawEntities = TranslationManagerToEntity.TranslateAll<VmLaw, Law>(vmLaws, unitOfWork);
            var serviceLawEntities = lawEntities.Select((law, index) => new ServiceLaw
            {
                Law = law,
                OrderNumber = index,
                ServiceVersionedId = versionedId
            }).ToList();
            serviceLawEntities.ForEach(e => serviceLawRepo.Add(e));
            unitOfWork.Save();

            return serviceLawEntities;
        }

        public IReadOnlyList<StatutoryServiceLaw> SaveGeneralDescriptionLaws(List<VmLaw> vmLaws, Guid versionedId, IUnitOfWorkWritable unitOfWork)
        {
            var gdLawRepo = unitOfWork.CreateRepository<IStatutoryServiceLawRepository>();
            var lawRepo = unitOfWork.CreateRepository<ILawRepository>();
            
            var toRemove = gdLawRepo.All()
                .Include(x => x.Law)
                .Where(x => x.StatutoryServiceGeneralDescriptionVersionedId == versionedId)
                .ToList();

            gdLawRepo.Remove(toRemove);
            lawRepo.Remove(toRemove.Select(x => x.Law));
            unitOfWork.Save();
            
            var lawEntities = TranslationManagerToEntity.TranslateAll<VmLaw, Law>(vmLaws, unitOfWork);
            var gdLawEntities = lawEntities.Select((law, index) => new StatutoryServiceLaw
            {
                Law = law,
                OrderNumber = index,
                StatutoryServiceGeneralDescriptionVersionedId = versionedId
            }).ToList();
            gdLawEntities.ForEach(e => gdLawRepo.Add(e));
            unitOfWork.Save();
            
            return gdLawEntities;
        }
    }
}
