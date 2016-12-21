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

using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTV.DataMapper.ConsoleApp.Models
{
    public class SourceService : SourceBase<IV2VmOpenApiServiceInBase>
    {
        private Dictionary<string, string> sourceTargetGroups;

        public SourceService()
        {
            sourceTargetGroups = new Dictionary<string, string>();
            sourceTargetGroups.Add("ASSOCIATIONS", "http://urn.fi/URN:NBN:fi:au:ptvl:KR1.7"); // Yhdistykset ja yhteisöt
            sourceTargetGroups.Add("CHILDREN_AND_FAMILIES", "http://urn.fi/URN:NBN:fi:au:ptvl:KR1.2"); // Lapset ja lapsiperheet
            sourceTargetGroups.Add("YOUTH", "http://urn.fi/URN:NBN:fi:au:ptvl:KR1.3"); // Nuoret
            sourceTargetGroups.Add("ELDERLY_PEOPLE", "http://urn.fi/URN:NBN:fi:au:ptvl:KR1.1"); // Ikäihmiset
            sourceTargetGroups.Add("DISABLED", "http://urn.fi/URN:NBN:fi:au:ptvl:KR1.6"); // Vammaiset
            sourceTargetGroups.Add("IMMIGRANTS", ""); // TODO - not found any corresponding target group.
            sourceTargetGroups.Add("ENTERPRISES", "http://urn.fi/URN:NBN:fi:au:ptvl:KR2"); // Yritykset
            sourceTargetGroups.Add("INDIVIDUALS", "http://urn.fi/URN:NBN:fi:au:ptvl:KR1"); // Kansalaiset
        }

        public int id { get; set; }
        public bool main_description { get; set; }
        public int service_id { get; set; }
        public string title { get; set; }
        public string name_synonyms { get; set; }
        public string description_short { get; set; }
        public string description_long { get; set; }
        public string prerequisites { get; set; }
        public string information { get; set; }
        public string servicemap_url { get; set; }
        public List<string> provided_languages { get; set; }
        public string[] responsible_depts { get; set; }
        public List<string> target_groups { get; set; }
        public object[] life_events { get; set; }
        public int[] provider_types { get; set; }
        public int[] errand_services { get; set; }
        public List<Link> links { get; set; }
        public object[] availabilities { get; set; }
        public int[] unit_ids { get; set; }

        public override IV2VmOpenApiServiceInBase ConvertToVm(string orgId, string code, int id = 0)
        {
            var langFi = LanguageCode.fi.ToString();

            IV2VmOpenApiServiceInBase vm = new V2VmOpenApiServiceIn()
            {
                SourceId = id.ToString(), // We need to use the id passed as parameter!
                Type = ServiceTypeEnum.Service.ToString(),
                ServiceNames = new List<VmOpenApiLocalizedListItem>()
                {
                    new VmOpenApiLocalizedListItem()
                    {
                        Value = GetStringWithMaxLength(title, 100),
                        Type = NameTypeEnum.Name.ToString(),
                        Language = langFi
                    },
                    new VmOpenApiLocalizedListItem()
                    {
                        Value = GetStringWithMaxLength(name_synonyms, 100),
                        Type = NameTypeEnum.AlternateName.ToString(),
                        Language = langFi
                    }
                },
                ServiceDescriptions = new List<VmOpenApiLocalizedListItem>()
                {
                    new VmOpenApiLocalizedListItem()
                    {
                        Value = description_long,
                        Type = DescriptionTypeEnum.Description.ToString(),
                        Language =langFi
                    },
                    new VmOpenApiLocalizedListItem()
                    {
                        Value = GetStringWithMaxLength(description_short, 150),
                        Type = DescriptionTypeEnum.ShortDescription.ToString(),
                        Language = langFi
                    },
                    new VmOpenApiLocalizedListItem()
                    {
                        Value = information,
                        Type = DescriptionTypeEnum.ServiceUserInstruction.ToString(),
                        Language = langFi
                    }
                },
                Languages = provided_languages.Count > 0 ? provided_languages : new List<string>() { langFi },
                TargetGroups = new List<string>(),
                ServiceClasses = new List<string>(),
                OntologyTerms = new List<string>(),
                LifeEvents = new List<string>(),
                IndustrialClasses = new List<string>(),
                Requirements = new List<VmOpenApiLanguageItem>()
                {
                    new VmOpenApiLanguageItem()
                    {
                        Value = prerequisites,
                        Language = langFi
                    }
                },
                ServiceCoverageType = ServiceCoverageTypeEnum.Local.ToString(),
                Municipalities = new List<string>() { code },
                ServiceOrganizations = new List<VmOpenApiServiceOrganization>()
                {
                    new VmOpenApiServiceOrganization()
                    {
                        OrganizationId = orgId,
                        RoleType = RoleTypeEnum.Responsible.ToString(),
                    },
                    new VmOpenApiServiceOrganization()
                    {
                        OrganizationId = orgId,
                        RoleType = RoleTypeEnum.Producer.ToString(),
                        ProvisionType = ProvisionTypeEnum.SelfProduced.ToString()
                    }
                },
                PublishingStatus = PublishingStatus.Draft.ToString()
            };

            if (target_groups.Count() > 0)
            {
                target_groups.ForEach(g =>
                {
                    var targetGroupUri = string.Empty;
                    try
                    {
                        targetGroupUri = sourceTargetGroups[g];
                    }
                    catch(Exception ex)
                    {
                        ErrorMsg = $"Could not find target goup { g }. { ex.Message }";
                    }
                    if (!string.IsNullOrEmpty(targetGroupUri))
                    {
                        vm.TargetGroups.Add(targetGroupUri);
                    }
                });
            }

            return vm;
        }
    }

    public class Link
    {
        public string type { get; set; }
        public string title { get; set; }
        public string url { get; set; }
    }
}
