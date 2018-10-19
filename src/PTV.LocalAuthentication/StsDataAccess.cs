using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.Logging;
using PTV.Framework.ServiceManager;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;

namespace PTV.LocalAuthentication
{
    
    public interface IStsDataAccess : IContextManager
    {
        
    }

    [RegisterService(typeof(IStsDataAccess), RegisterType.Transient)]
    public class StsDataAccess : ContextManagerBase<StsDbContext>, IStsDataAccess
    {
        public StsDataAccess(ILoggerFactory loggerFactory, IServiceProvider serviceProvider, IOptions<DataContextOptions> dataContextOptions) : base(loggerFactory, serviceProvider, dataContextOptions, true)
        {
        }
    }
}