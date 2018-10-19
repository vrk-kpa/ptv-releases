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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Logging;
using PTV.TaskScheduler.Configuration;
using PTV.TaskScheduler.Enums;
using PTV.TaskScheduler.Models;
using Quartz;

namespace PTV.TaskScheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    internal class SoteJob : BaseJob
    {
        public override JobTypeEnum JobType => JobTypeEnum.Sote;
        private UrlBaseConfiguration urlBaseConfiguration;
        private string fireInstanceId;
        private ISoteService soteService;
        private VmJobLogEntry logInfo;

        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
        {
            
            var jobData = QuartzScheduler.GetJobDataConfiguration<SoteJobDataConfiguration>(context.JobDetail);
            if (jobData.PalvelunJarjestajatUrl.IsNullOrEmpty()) return null;
            if (jobData.PalvelunTuottajatUrl.IsNullOrEmpty()) return null;
            
            urlBaseConfiguration = GetApplicationJobConfiguration<SoteJobDataConfiguration>(context);
            if (urlBaseConfiguration == null) throw new Exception("Could not load SoteJobDataConfiguration!");

            fireInstanceId = context.FireInstanceId;
            if (fireInstanceId.IsNullOrEmpty()) throw new Exception("Could not load FireInstanceId!");

            soteService = serviceProvider.GetService<ISoteService>();
            if (soteService == null) throw new Exception("Could not load SoteService!");
            
            // set log info
            logInfo = new VmJobLogEntry {JobType = JobType.ToString(), UserName = context.JobDetail.Key.Name, OperationId = fireInstanceId};
            
            // download responsible organization
            var urlOfJarjestajat = ParseUrlBaseConfiguration(jobData.PalvelunJarjestajatUrl, urlBaseConfiguration);
            var responsibleOrganizationsContent = GetJsonData(urlOfJarjestajat, urlBaseConfiguration);
            if (responsibleOrganizationsContent.IsNullOrEmpty()) return $"Unable download palvelun jarjestajat data from '{urlOfJarjestajat}'.";
            var responsibleOrganizations = JsonConvert.DeserializeObject<List<VmJsonJarjestajat>>(responsibleOrganizationsContent);

            // get list of pairs oid - unificRootId
            var organizationsByOid = soteService.GetOrganizationIdsByOid(logInfo);

            // skip handling of responsible organizations which are not handling by PTV
            var notExistingResponsibleOrganizations = responsibleOrganizations
                .Where(ro => !organizationsByOid.ContainsKey(ro.soteOid))
                .Select(ro => ro.soteOid)
                .ToList();
            if (notExistingResponsibleOrganizations.Any())
            {
                LogWarn($"Responsible region(s) with following soteOid are not handled by PTV and are not processed: [{string.Join(", ", notExistingResponsibleOrganizations)}]");
            }

            var soteModel = new List<VmJsonSoteOrganization>();

            // handle responsible organisations
            foreach (var responsibleOrganization in responsibleOrganizations.Where(ro => organizationsByOid.ContainsKey(ro.soteOid)))
            {

                // get main organization list for responsible organization
                var mainOrganizations = DownloadTuottajatList(jobData.PalvelunTuottajatUrl, responsibleOrganization.soteOid);
                if (mainOrganizations.IsNullOrEmpty()) continue;

                // log all main organizations, which are not handled in PTV
                var notExistingMainOrganizations = mainOrganizations
                    .Where(mo => !organizationsByOid.ContainsKey(mo.palveluntuottajaOid))
                    .Select(mo => mo.palveluntuottajaOid)
                    .ToList();
                if (notExistingMainOrganizations.Any())
                {
                    LogWarn($"Main organizations with oid: [{string.Join(',', notExistingMainOrganizations)}] for responsible organization '{responsibleOrganization.soteOid}' are not handled in PTV and will be skipped.");
                }

                // handle main organizations
                soteModel.AddRange(
                    from mainOrganization
                    in mainOrganizations.Where(mo => organizationsByOid.ContainsKey(mo.palveluntuottajaOid))
                    where mainOrganization.palveluyksikot != null
                        from soteOrganization
                        in mainOrganization.palveluyksikot.Where(so => !string.IsNullOrEmpty(so.palveluyksikkoOid))
                        select new VmJsonSoteOrganization
                        {
                            Oid = soteOrganization.palveluyksikkoOid,
                            MainOrganizationId = organizationsByOid[mainOrganization.palveluntuottajaOid],
                            Name = soteOrganization.nimi,
                            AreaInformationType = AreaInformationTypeEnum.WholeCountry.ToString(),
                            ValidFrom = ParseDateTime(soteOrganization.alkamispaiva),
                            ValidTo = ParseDateTime(soteOrganization.paattymispaiva),
                            Type = ParseOrganizationType(soteOrganization),
                            ContactInfo = ParseContactInfo(soteOrganization),
                            ServiceLocations = ParseServiceLocation(soteOrganization.toimipisteet)
                        });
            }
            
            soteService.ImportData(soteModel, logInfo);
            return null;
        }

        private List<VmJsonTuottajat> DownloadTuottajatList(string baseUrl, string soteOid)
        {
            var urlOfTuottajat = ParseUrlBaseConfiguration(baseUrl, urlBaseConfiguration, soteOid);
            var mainOrganizationsContent = ProxyDownload(urlOfTuottajat, urlBaseConfiguration);
            var mainOrganizations = JsonConvert.DeserializeObject<List<VmJsonTuottajat>>(mainOrganizationsContent);

            if (!mainOrganizations.IsNullOrEmpty()) return mainOrganizations.ToList();
            
            TaskSchedulerLogger.LogJobWarn(fireInstanceId, JobType, $"No main organization(s) has been found for responsible region '{soteOid}'.");
            return null;
        }

        private void LogWarn(string message)
        {
            TaskSchedulerLogger.LogJobWarn(fireInstanceId, JobType, message);
        }
        
        private static VmJsonSoteContactInfo ParseContactInfo(VmPalvelunTuottajatBaseWithLocation contactInfo)
        {
            return new VmJsonSoteContactInfo
            {
                PhoneNumber = contactInfo.puhelin,
                Address = string.IsNullOrEmpty(contactInfo.postinumero) || string.IsNullOrEmpty(contactInfo.katuosoite)
                    ? null
                    : new VmJsonSoteAddress
                        {
                            StreetAddress = ParseAddress(contactInfo.katuosoite),
                            PostalCode = contactInfo.postinumero,
                            PostOffice = contactInfo.postitoimipaikka,
                            MunicipalityName = contactInfo.sijaintikunta
                        }
            };
        }
        
        private static VmJsonSoteStreetAddress ParseAddress(string addressInText)
        {
            var match = Regex.Match(addressInText, @"[,;]+|\s\d");
            return match.Success
                ? new VmJsonSoteStreetAddress {StreetName = addressInText.Substring(0, match.Index).Trim(), StreetNumber = addressInText.Substring(match.Index + 1).Trim()}
                : new VmJsonSoteStreetAddress {StreetName = addressInText.Trim(), StreetNumber = null};
        }

        private static List<VmJsonSoteServiceLocation> ParseServiceLocation(List<VmJsonToimipisteet> soteServiceLocations)
        {
            var result = new List<VmJsonSoteServiceLocation>();
            
            //if (soteServiceLocations == null) return empty collection;
            if (soteServiceLocations.IsNullOrEmpty()) return result;
            
            //  
            soteServiceLocations.ForEach(sl => result.Add(new VmJsonSoteServiceLocation
            {
                Oid = sl.toimipisteOid,
                Name = sl.nimi,
                ContactInfo = ParseContactInfo(sl)
            }));
            return result;
        }

        private static string ParseOrganizationType(VmPalvelunTuottajatBase soteOrganization)
        {
            bool.TryParse(soteOrganization.julkinen, out var isPublic);
            return isPublic ? OrganizationTypeEnum.SotePublic.ToString() : OrganizationTypeEnum.SotePrivate.ToString();
        }

        private static DateTime? ParseDateTime(string dateTime)
        {
            return dateTime.IsNullOrEmpty()
                ? (DateTime?) null
                : DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        
    }
}