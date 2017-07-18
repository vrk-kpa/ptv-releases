using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IBugReportService
    {
        List<VmBugReport> GetAllBugReports();
        VmBugReport GetBugReportById(Guid id);
        void SaveBugReport(VmBugReport bugReport);
    }
}
