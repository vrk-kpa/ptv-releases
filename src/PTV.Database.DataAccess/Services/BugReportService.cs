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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Models;
using PTV.Database.DataAccess.Interfaces.Repositories;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IBugReportService), RegisterType.Transient)]
    internal class BugReportService : ServiceBase, IBugReportService
    {
        private IContextManager contextManager;
        private readonly ITranslationEntity translationManager;

        public BugReportService(
            IContextManager contextManager,
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
              versioningManager
            )
        {
            this.translationManager = translationManagerToVm;
            this.contextManager = contextManager;
        }

        public List<VmBugReport> GetAllBugReports()
        {
            var result = new List<VmBugReport>();
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var bugReportRepository = unitOfWork.CreateRepository<IBugReportRepository>();
                var resultTemp = bugReportRepository.All()
                    .Select(x => new BugReport
                    {
                        Id = x.Id,
                        Created = x.Created,
                        Description = x.Description,
                        CreatedBy = x.CreatedBy,
                        Modified = x.Modified,
                        ModifiedBy = x.ModifiedBy,
                        Name = x.Name,
                        Version = x.Version
                    });
                result = translationManager.TranslateAll<BugReport, VmBugReport>(resultTemp).ToList();
            });
            return result;
        }

        public VmBugReport GetBugReportById(Guid id)
        {
            var result = new VmBugReport();
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var bugReportRepository = unitOfWork.CreateRepository<IBugReportRepository>();
                result = translationManager
                    .TranslateFirst<BugReport, VmBugReport>(bugReportRepository.All().Where(x => x.Id == id));
            });
            return result;
        }

        public void SaveBugReport(VmBugReport vm)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var bugReport = TranslationManagerToEntity.Translate<VmBugReport, BugReport>(vm, unitOfWork);
                var bugReportRepository = unitOfWork.CreateRepository<IBugReportRepository>();
                bugReportRepository.Add(bugReport);
                unitOfWork.Save(parentEntity: bugReport);
            });
        }
    }
}

