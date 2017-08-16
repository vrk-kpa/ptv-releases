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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.DataMapper.ConsoleApp.Models;
using PTV.DataMapper.ConsoleApp.Services.Interfaces;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;

namespace PTV.DataMapper.ConsoleApp.Services
{
    public class DataMapper : IDataMapper
    {
        private IVmOpenApiOrganizationVersionBase organization;
        private IServiceProvider serviceProvider;
        private ILogger<DataMapper> logger;

        protected DataSource source;

        public DataMapper(IVmOpenApiOrganizationVersionBase organization, DataSource source, IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            this.organization = organization;
            this.source = source;
            this.serviceProvider = serviceProvider;
            this.logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<DataMapper>();
        }

        public List<int> GetSourceServiceChannels()
        {
            var channels = new List<int>();
            if (!string.IsNullOrEmpty(source.DebugChannels))
            {
                return ConvertStringToIntegerList(source.DebugChannels);
            }
            else
            {
                GetSourceData<List<SourceListItem>>(source.ChannelListUrl).ForEach(c => channels.Add(c.id));
            }

            return channels;
        }

        public List<int> GetSourceServiceChannels(int serviceId)
        {
            var channels = new List<int>();
            GetSourceData<List<SourceListItem>>(string.Format("{0}&service={1}", source.ChannelListUrl, serviceId)).ForEach(c => channels.Add(c.id));
            LogMsg($"Channel count: { channels.Count() }.");
            return channels;
        }

        public List<int> GetSourceServices()
        {
            if (!string.IsNullOrEmpty(source.DebugServices))
            {
                return ConvertStringToIntegerList(source.DebugServices);
            }
            else
            {
                var services = new List<int>();
                GetSourceData<List<SourceListItem>>(source.ServiceDetailUrl).ForEach(s =>
                {
                    if (s.main_description) services.Add(s.id);
                });
                return services;
            }
        }

        public virtual IVmOpenApiServiceInVersionBase MapService(int id)
        {
            IVmOpenApiServiceInVersionBase vmService = new VmOpenApiServiceInVersionBase();
            try
            {
                var serviceDescriptionUrl = source.ServiceUrl + id;
                LogMsg($"Service url: {serviceDescriptionUrl}", true);

                var serviceDescriptions = GetSourceData<List<SourceListItem>>(serviceDescriptionUrl);
                LogMsg($"Service description count: { serviceDescriptions.Count }.");

                // TODO - what do we do if there is several descriptions for a service!
                var serviceDetailUrl = source.ServiceDetailUrl + serviceDescriptions.FirstOrDefault().id;
                LogMsg($"Service detail url: { serviceDetailUrl }.");

                var serviceDetail = GetSourceData<SourceService>(serviceDetailUrl);
                vmService = serviceDetail.ConvertToVm(organization.Id.ToString(), organization.Oid, id);
                if (!string.IsNullOrEmpty(serviceDetail.ErrorMsg))
                {
                    LogMsg(serviceDetail.ErrorMsg);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error occured while mapping a service { id }. { ex.Message }.");
                throw;
            }
            return vmService;
        }

        public VmOpenApiServiceLocationChannelInVersionBase MapServiceChannel(int id)
        {
            var vmChannel = new VmOpenApiServiceLocationChannelInVersionBase();
            try
            {
                var url = source.ChannelDetailUrl + id;
                LogMsg($"Channel url: {url}");

                var channelDetails = GetSourceData<SourceChannel>(url);
                vmChannel = channelDetails.ConvertToVm(organization.Id.ToString(), organization.Oid);
                if (!string.IsNullOrEmpty(channelDetails.ErrorMsg))
                {
                    LogMsg(channelDetails.ErrorMsg);
                }

                ValidateObject(vmChannel);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error occured while mapping a channel { id }. { ex.Message }.");
                throw;
            }

            return vmChannel;
        }

        public void ValidateObject(Object obj)
        {
            var valContext = new ValidationContext(obj);
            var valResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(obj, valContext, valResults, true);
            if (!isValid)
            {
                StringBuilder msg = new StringBuilder();
                valResults.ForEach(v => msg.AppendLine(v.ToString()));
                throw new Exception($"Error validating model: { msg.ToString() } { JsonConvert.SerializeObject(obj) }");
            }
        }

        protected T GetSourceData<T>(string url)
        {
            var reader = new DataReader(url);
            return reader.GetData<T>();
        }

        private List<int> ConvertStringToIntegerList(string str)
        {
            var intList = new List<int>();
            var list = str.Split(',').ToList();
            list.ForEach(c =>
            {
                int id = 0;
                int.TryParse(c, out id);
                if (id != 0)
                {
                    intList.Add(id);
                }
            });
            return intList;
        }

        private void LogMsg(string msg, bool newLine = false)
        {
            if (newLine)
            {
                Console.WriteLine();
                logger.LogDebug(Environment.NewLine);
            }
            logger.LogDebug(msg);
            Console.WriteLine(msg);
        }
    }
}
