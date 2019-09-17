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
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
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
    
    
// SOTE has been disabled (SFIPTV-1177)

//    [PersistJobDataAfterExecution]
//    [DisallowConcurrentExecution]
//    internal class SoteJob : BaseJob
//    {
//        public override JobTypeEnum JobType => JobTypeEnum.Sote;
//        private ISoteService soteService;
//        private VmJobLogEntry logInfo;
//
//        protected override string CallExecute(IJobExecutionContext context, IServiceProvider serviceProvider, IContextManager contextManager)
//        {
//            
//            var jobDataConfiguration = QuartzScheduler.GetJobDataConfiguration<SoteJobDataConfiguration>(context.JobDetail);
//            if (jobDataConfiguration.PalvelunJarjestajatUrl.IsNullOrEmpty()) return null;
//            if (jobDataConfiguration.PalvelunTuottajatUrl.IsNullOrEmpty()) return null;
//            
//            MergeApplicationJobConfiguration(context, jobDataConfiguration);
//            
//            soteService = serviceProvider.GetService<ISoteService>();
//            if (soteService == null) throw new Exception("Could not load SoteService!");
//            
//            // set log info
//            logInfo = new VmJobLogEntry {JobType = JobType.ToString(), UserName = context.JobDetail.Key.Name, OperationId = JobOperationId};
//            
//            // load certificate file
//            var certificate = LoadCertificate(jobDataConfiguration);
//
//            // download responsible organization
//            var urlOfJarjestajat = ParseUrlBaseConfiguration(jobDataConfiguration.PalvelunJarjestajatUrl, jobDataConfiguration);
//            var responsibleOrganizationsContent = GetJsonDataWithCertificate(urlOfJarjestajat, certificate, jobDataConfiguration, jobDataConfiguration.ProxyServerSettings);
//            if (responsibleOrganizationsContent.IsNullOrEmpty()) return $"Unable download palvelun jarjestajat data from '{urlOfJarjestajat}'.";
//            var responsibleOrganizations = JsonConvert.DeserializeObject<List<VmJsonJarjestajat>>(responsibleOrganizationsContent);
//
//            // get list of pairs oid - unificRootId
//            var organizationsByOid = soteService.GetOrganizationIdsByOid(logInfo);
//
//            // skip handling of responsible organizations which are not handling by PTV
//            var notExistingResponsibleOrganizations = responsibleOrganizations
//                .Where(ro => !organizationsByOid.ContainsKey(ro.soteOid))
//                .Select(ro => ro.soteOid)
//                .ToList();
//            if (notExistingResponsibleOrganizations.Any())
//            {
//                LogWarn($"Responsible region(s) with following soteOid are not handled by PTV and are not processed: [{string.Join(", ", notExistingResponsibleOrganizations)}]");
//            }
//
//            var soteModel = new List<VmJsonSoteOrganization>();
//
//            // handle responsible organisations
//            foreach (var responsibleOrganization in responsibleOrganizations.Where(ro => organizationsByOid.ContainsKey(ro.soteOid)))
//            {
//
//                // get main organization list for responsible organization
//                var mainOrganizations = DownloadTuottajatList(jobDataConfiguration, responsibleOrganization.soteOid, certificate);
//                if (mainOrganizations.IsNullOrEmpty()) continue;
//
//                // log all main organizations, which are not handled in PTV
//                var notExistingMainOrganizations = mainOrganizations
//                    .Where(mo => !organizationsByOid.ContainsKey(mo.palveluntuottajaOid))
//                    .Select(mo => mo.palveluntuottajaOid)
//                    .ToList();
//                if (notExistingMainOrganizations.Any())
//                {
//                    LogWarn($"Main organizations with oid: [{string.Join(',', notExistingMainOrganizations)}] for responsible organization '{responsibleOrganization.soteOid}' are not handled in PTV and will be skipped.");
//                }
//
//                var responsibleOrganizationId = organizationsByOid[responsibleOrganization.soteOid];
//                mainOrganizations
//                    .Where(mo => organizationsByOid.ContainsKey(mo.palveluntuottajaOid) && mo.palveluyksikot != null)
//                    .ForEach(mainOrganization =>
//                    {
//                        mainOrganization.palveluyksikot
//                            .Where(so => !string.IsNullOrEmpty(so.palveluyksikkoOid))
//                            .ForEach(soteOrganization =>
//                            {
//                                if (!ValidateContent(soteOrganization)) return;
//                                soteModel.Add(new VmJsonSoteOrganization
//                                {
//                                    Oid = soteOrganization.palveluyksikkoOid,
//                                    MainOrganizationId = organizationsByOid[mainOrganization.palveluntuottajaOid],
//                                    ResponsibleOrganizationId = responsibleOrganizationId,
//                                    Name = soteOrganization.nimi,
//                                    AreaInformationType = AreaInformationTypeEnum.WholeCountry.ToString(),
//                                    ValidFrom = ParseDateTime(soteOrganization.alkamispaiva),
//                                    ValidTo = ParseDateTime(soteOrganization.paattymispaiva),
//                                    Type = ParseOrganizationType(soteOrganization),
//                                    ContactInfo = ParseContactInfo(soteOrganization),
//                                    ServiceLocations = ParseServiceLocation(soteOrganization.toimipisteet)
//                                });
//                            });
//                    });
//            }
//            
//            soteService.ImportData(soteModel, logInfo);
//            return null;
//        }
//
//        private List<VmJsonTuottajat> DownloadTuottajatList(SoteJobDataConfiguration jobDataConfiguration, string soteOid, byte[] certificate)
//        {
//            var urlOfTuottajat = ParseUrlBaseConfiguration(jobDataConfiguration.PalvelunTuottajatUrl, jobDataConfiguration, soteOid);
//            var mainOrganizationsContent = GetJsonDataWithCertificate(urlOfTuottajat, certificate, jobDataConfiguration, jobDataConfiguration.ProxyServerSettings);
//            var mainOrganizations = JsonConvert.DeserializeObject<List<VmJsonTuottajat>>(mainOrganizationsContent);
//
//            if (!mainOrganizations.IsNullOrEmpty()) return mainOrganizations.ToList();
//            
//            LogWarn($"No main organization(s) has been found for responsible region '{soteOid}'.");
//            return null;
//        }
//
//        private void LogWarn(string message)
//        {
//            TaskSchedulerLogger.LogJobWarn(JobOperationId, JobType, message);
//        }
//        
//        private static VmJsonSoteContactInfo ParseContactInfo(VmPalvelunTuottajatBaseWithLocation contactInfo)
//        {
//            return new VmJsonSoteContactInfo
//            {
//                PhoneNumber = contactInfo.puhelin,
//                Address = string.IsNullOrEmpty(contactInfo.postinumero) || string.IsNullOrEmpty(contactInfo.katuosoite)
//                    ? null
//                    : new VmJsonSoteAddress
//                        {
//                            StreetAddress = ParseAddress(contactInfo.katuosoite),
//                            PostalCode = contactInfo.postinumero,
//                            PostOffice = contactInfo.postitoimipaikka,
//                            MunicipalityName = contactInfo.sijaintikunta
//                        }
//            };
//        }
//        
//        private static VmJsonSoteStreetAddress ParseAddress(string addressInText)
//        {
//            var match = Regex.Match(addressInText, @"[,;]+|\s\d");
//            return match.Success
//                ? new VmJsonSoteStreetAddress {StreetName = addressInText.Substring(0, match.Index).Trim(), StreetNumber = addressInText.Substring(match.Index + 1).Trim()}
//                : new VmJsonSoteStreetAddress {StreetName = addressInText.Trim(), StreetNumber = null};
//        }
//
//        private List<VmJsonSoteServiceLocation> ParseServiceLocation(List<VmJsonToimipisteet> soteServiceLocations)
//        {
//            var result = new List<VmJsonSoteServiceLocation>();
//            
//            //if (soteServiceLocations == null) return empty collection;
//            if (soteServiceLocations.IsNullOrEmpty()) return result;
//
//            soteServiceLocations.ForEach(sl =>
//            {
//                if (!ValidateContent(sl)) return;
//                result.Add(new VmJsonSoteServiceLocation
//                {
//                    Oid = sl.toimipisteOid,
//                    Name = sl.nimi,
//                    ContactInfo = ParseContactInfo(sl)
//                });
//            });
//            return result;
//        }
//
//        private static string ParseOrganizationType(VmPalvelunTuottajatBase soteOrganization)
//        {
//            bool.TryParse(soteOrganization.julkinen, out var isPublic);
//            return isPublic ? OrganizationTypeEnum.SotePublic.ToString() : OrganizationTypeEnum.SotePrivate.ToString();
//        }
//
//        private static DateTime? ParseDateTime(string dateTime)
//        {
//            return dateTime.IsNullOrEmpty()
//                ? (DateTime?) null
//                : DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
//        }
//        
//        private bool ValidateContent<T>(T objectToValidate) where T : VmPalvelunTuottajatBase  
//        {
//            var vc = new ValidationContext(objectToValidate);
//            var vr = new List<ValidationResult>();
//            if (Validator.TryValidateObject(objectToValidate, vc, vr, true)) return true;
//
//            var em = vr.Select(m => $"{m.ErrorMessage} [fields: {string.Join(", ", m.MemberNames)}]").ToList();
//            LogWarn($"Item with name '{objectToValidate.nimi}' is not valid: {string.Join(";\n", em)}");
//            return false;
//        }
//
//        private byte[] LoadCertificate(SoteJobDataConfiguration jobDataConfiguration)
//        {
//            // get certificate file
//            if (jobDataConfiguration.CertificateFileName.IsNullOrEmpty()) throw new Exception("Certificate file name is not set in configuration file.");
//            if (jobDataConfiguration.AuthorizationHeaderValue.IsNullOrEmpty()) throw new Exception("Authorization header value is not set in configuration file.");
//            var certificateFileName = Path.Combine(AppContext.BaseDirectory, jobDataConfiguration.CertificateFileName);
//            if (!File.Exists(certificateFileName)) throw new Exception($"Certification file '{certificateFileName}' does not exists.");
//
//            if (Startup.AppSettingsKey.IsNullOrEmpty())
//            {
//                return File.ReadAllBytes(certificateFileName);
//            }
//            
//            using (var fs = new FileStream(certificateFileName, FileMode.Open, FileAccess.Read))
//            {
//                return fs.Decrypt(Startup.AppSettingsKey).ToArray();
//            }
//        }
//    }
}
