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

using System.Collections.Generic;
using System.Linq;
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
            return contextManager.ExecuteIsolatedReader(unitOfWork =>
            {
                var cfgRep = unitOfWork.CreateRepository<ICFGRequestFilterRepository>();
                var query = cfgRep.All();
                if (interfaceTag != null)
                {
                    query = query.Where(i => i.Interface == interfaceTag);
                }
                var allData = query.ToList();
                return allData.Select(i => new RequestFilterConfigurationData
                {
                    UserName = i.UserName ?? string.Empty,
                    IPAddress = i.IPAddress ?? string.Empty,
                    ConcurrentRequests = i.ConcurrentRequests.PositiveOrZero()
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
