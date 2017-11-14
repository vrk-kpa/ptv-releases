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


using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.DataMapper.ConsoleApp.Models;
using PTV.DataMapper.ConsoleApp.Services.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Framework;
using PTV.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 168

namespace PTV.DataMapper.ConsoleApp.Services
{
    public class DataHandler : IDataHandler
    {
        private readonly AppSettings settings;
        private IOrganizationService organizationService;
        private IChannelService channelService;
        private IServiceService serviceService;
        private ICodeService codeService;
        private ILogger<IDataHandler> logger;

        private string userName;
        private List<int> alreadyUpdatedChannels;
        private List<int> alreadyUpdatedServices;

        // Logging information
        private int serviceChannelCount;
        private int serviceCount;
        private int serviceUpdateFailedCount;
        private int serviceEndpointNotFoundCount;
        private List<int> failedServices;
        private int serviceUpdateSuccessCount;

        /// <summary>
        /// Gets the reference to AppSettings.
        /// </summary>
        protected AppSettings Settings { get { return settings; } private set { } }

        /// <summary>
        /// Gets the username.
        /// </summary>
        protected string UserName { get { return userName; } private set { } }

        public DataHandler(
            IOptions<AppSettings> settings,
            IOrganizationService organizationService,
            IChannelService channelService,
            IServiceService serviceService,
            ICodeService codeService,
            ILogger<DataHandler> logger)
        {
            this.settings = settings.Value;
            this.organizationService = organizationService;
            this.channelService = channelService;
            this.serviceService = serviceService;
            this.codeService = codeService;
            this.logger = logger;
        }

        public void ProcessAllData(IServiceProvider serviceProvider)
        {
            foreach (var source in Settings.DataSources)
            {
                ResetValues();
                try
                {
                    if (IsAuthorizedUser(source.PTVUsername, source.PTVPassword))
                    {
                        // Get the organization
                        var org = GetOrganization(source.Oid);

                        IDataMapper mapper = new DataMapper(org, source, serviceProvider);
                        if (!string.IsNullOrEmpty(source.DebugServices))
                        {
                            mapper = new TestDataMapper(org, source, serviceProvider);
                        }
                        UpdateServices(mapper, mapper.GetSourceServices());
                        //else
                        //{
                        //    // Update the service channels related to fetched organization
                        //    GetAndUpdateServiceChannels(mapper, mapper.GetSourceServiceChannels());
                        //}
                    }
                    else
                    {
                        logger.LogError($"Invalid user: {UserName}.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error occured while updating source: {source.Oid}. {ex.Message} {ex.InnerException}");
                }

                LogSummary();
            }
        }

        private void ResetValues()
        {
            alreadyUpdatedChannels = new List<int>();
            alreadyUpdatedServices = new List<int>();
            failedServices = new List<int>();
            serviceChannelCount = serviceCount = serviceUpdateFailedCount = serviceEndpointNotFoundCount = serviceUpdateSuccessCount = 0;
        }

        private bool IsAuthorizedUser(string userName, string password)
        {
            this.userName = userName;

            // Authenticate the user - check that the user exists
            var client = new TokenClient(
             Settings.STS,
             "ptv_api_client",
             "openapi", AuthenticationStyle.BasicAuthentication);

            var token = client.RequestResourceOwnerPasswordAsync(userName, password, "dataEventRecords openid").Result;
            return token.AccessToken != null && string.IsNullOrEmpty(token.Error);
        }

        private IVmOpenApiOrganizationVersionBase GetOrganization(string oid)
        {
            var org = organizationService.GetOrganizationByOid(oid, 0);
            if (org == null)
            {
                var msg = $"Main organization not found with Oid { oid }. Please add the main organization manually.";
                logger.LogDebug(msg);
                Console.WriteLine(msg);
                throw new Exception(msg);
            }

            logger.LogDebug($"Organization: {org.Oid} {org.OrganizationNames.FirstOrDefault().Value}.");
            Console.WriteLine($"Organization: {org.Oid} {org.OrganizationNames.FirstOrDefault().Value}.");

            return org;
        }

        private IVmOpenApiServiceChannelBase GetAndUpdateServiceChannel(IDataMapper mapper, int channelId)
        {
            IVmOpenApiServiceChannelBase vmResult = new VmOpenApiServiceLocationChannelVersionBase();
            if (alreadyUpdatedChannels.Contains(channelId))
            {
                // The channel was already updated so lets just get the corresponding guid for source id.
                if (!Settings.DebugData)
                {
                    vmResult.Id = channelService.GetServiceChannelBySource(channelId.ToString(), UserName);
                }
            }
            else
            {
                try
                {
                    var vmChannel = mapper.MapServiceChannel(channelId);
                    if (!Settings.DebugData)
                    {
                        if (PostalCodeExists(vmChannel.Addresses))
                        {
                            try
                            {
                                vmResult = channelService.AddServiceChannel(vmChannel, false, 0, UserName);
                            }
                            catch (ExternalSourceExistsException ex)
                            {
                                // The external source for service channel is already in use so the item was already created into PTV. Let's update the data.
                                vmResult = channelService.SaveServiceChannel(vmChannel, false, 0, UserName);
                            }
                        }
                    }

                    alreadyUpdatedChannels.Add(channelId);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error occured while updating a channel { channelId }. { ex.Message }.");
                    //throw ex;
                }
            }
            return vmResult;
        }

        private List<Guid> GetAndUpdateServiceChannels(IDataMapper mapper, List<int> channels)
        {
            var list = new List<Guid>();
            serviceChannelCount += channels.Count();

            foreach (var channelId in channels)
            {
                var result = GetAndUpdateServiceChannel(mapper, channelId);
                if (result.Id.HasValue) { list.Add(result.Id.Value); }
            }
            return list;
        }

        private IVmOpenApiServiceBase GetAndUpdateService(IDataMapper mapper, int serviceId)
        {
            IVmOpenApiServiceBase vmResult = new VmOpenApiServiceVersionBase();
            if (alreadyUpdatedServices.Contains(serviceId))
            {
                return vmResult;
            }

            try
            {
                var vmService = mapper.MapService(serviceId);
                if (!Settings.DebugData)
                {
                    try
                    {
                        vmResult = serviceService.AddService(vmService, false, 0, UserName);
                    }
                    catch (ExternalSourceExistsException ex)
                    {
                        // The external source for service is already in use so the item was already created into PTV. Let's update the data.
                        vmResult = serviceService.SaveService(vmService, false, 0, vmService.SourceId, UserName);
                    }
                }
                else { vmResult = serviceService.GetServiceBySource(vmService.SourceId, 0, userName: UserName); }
                serviceUpdateSuccessCount++;
                alreadyUpdatedServices.Add(serviceId);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error occured while updating a service: { serviceId }. { ex.Message }");
                serviceUpdateFailedCount++;
                if (!failedServices.Contains(serviceId))
                {
                    failedServices.Add(serviceId);
                }
                if (ex is EndPointNotFoundException)
                {
                    serviceEndpointNotFoundCount++;
                }
            }
            return vmResult;
        }

        private void UpdateServices(IDataMapper mapper, List<int> serviceList)
        {
            serviceCount += serviceList.Count();
            foreach (var serviceId in serviceList)
            {
                var result = GetAndUpdateService(mapper, serviceId);
                if (result != null && result.Id.HasValue)
                {
                    var guidList = new List<Guid>();
                    guidList.AddRange(GetAndUpdateServiceChannels(mapper, mapper.GetSourceServiceChannels(serviceId)));

                    // Add the service channels for a service
                    if (!Settings.DebugData && guidList.Count > 0)
                    {
                        UpdateChannelsForService(result.Id.Value, guidList);
                    }
                }
            }
        }

        private void UpdateChannelsForService(Guid serviceId, IList<Guid> serviceChannels)
        {
            try
            {
                // Add the services for service channel
                serviceService.AddChannelsForService(serviceId, serviceChannels, false, UserName);
            }
            catch (Exception)
            {
                // TODO
            }
        }

        private bool PostalCodeExists(IList<V5VmOpenApiAddressWithTypeIn> addresses)
        {
            var errorMsg = new StringBuilder();
            addresses.ForEach(a =>
            {
                var postalInfo = codeService.GetPostalCodeByCode(a.PostalCode);
                if (postalInfo == null)
                {
                    errorMsg.AppendLine($"Postalcode {a.PostalCode} does not exist!");
                }
            });
            if (errorMsg.Length > 0)
            {
                throw new Exception(errorMsg.ToString());
            }

            return true;
        }

        private void LogSummary()
        {
            // Log some summary of the import.
            Console.WriteLine("*************************************************************");
            Console.WriteLine($"Summary: service channel count { serviceChannelCount }.");
            Console.WriteLine($"Summary: services failed { serviceUpdateFailedCount }.");
            Console.WriteLine($"Summary: services succeeded { serviceUpdateSuccessCount }.");
            Console.WriteLine($"Summary: services no endpoint found { serviceEndpointNotFoundCount }.");
            StringBuilder msgFailedServices = new StringBuilder();
            failedServices.ForEach(f => msgFailedServices.Append(f + ", "));
            Console.WriteLine($"Summary: failed services: { msgFailedServices.ToString() }.");
            Console.WriteLine("*************************************************************");

            logger.LogDebug($"Summary: service channel count { serviceChannelCount }, services failed { serviceUpdateFailedCount }, services succeeded { serviceUpdateSuccessCount }, services no endpoint found { serviceEndpointNotFoundCount }.");
            logger.LogDebug("*************************************************************");
        }
    }
}

#pragma warning restore 168

