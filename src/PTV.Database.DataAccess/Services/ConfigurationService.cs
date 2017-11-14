using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services
{
    public interface IConfigurationService
    {
        List<RequestFilterConfigurationData> GetRequestFilterConfiguration(string interfaceTag = null);
    }

    [RegisterService(typeof(IConfigurationService), RegisterType.Transient)]
    public class ConfigurationService : IConfigurationService
    {
        private readonly IContextManager contextManager;

        public ConfigurationService(IContextManager contextManager)
        {
            this.contextManager = contextManager;
        }

        public List<RequestFilterConfigurationData> GetRequestFilterConfiguration(string interfaceTag = null)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var cfgRep = unitOfWork.CreateRepository<ICFGRequestFilterRepository>();
                var query = cfgRep.All();
                if (interfaceTag != null)
                {
                    query = query.Where(i => i.Interface == interfaceTag);
                }
                var allData = query.ToList();
                return allData.Select(i => new RequestFilterConfigurationData()
                {
                    UserName = i.UserName,
                    IPAddress = i.IPAddress,
                    ConcurrentRequests = i.ConcurrentRequests
                }).ToList();
            });
        }
    }


    public class RequestFilterConfigurationData
    {
        public string IPAddress { get; set; }
        public string UserName { get; set; }
        public int ConcurrentRequests { get; set; }
    }

}
