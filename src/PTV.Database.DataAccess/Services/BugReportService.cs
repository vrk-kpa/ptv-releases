using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PTV.Database.DataAccess.Caches;
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
            IUserOrganizationChecker userOrganizationChecker)
            : base(
              translationManagerToVm,
              translationManagerToEntity,
              publishingStatusCache,
              userOrganizationChecker
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
                    .Select(x => new BugReport()
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

