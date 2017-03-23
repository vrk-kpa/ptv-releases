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

using PTV.Database.DataAccess.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using PTV.DataMapper.ConsoleApp.Models;
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;

namespace PTV.DataMapper.ConsoleApp.Services
{
    public class TestDataMapper : DataMapper
    {
        private IGeneralDescriptionService generalDescriptionService;
        private Dictionary<int, string> serviceNameMapping;
        public TestDataMapper(IVmOpenApiOrganizationVersionBase organization, DataSource source, IServiceProvider serviceProvider) : base(organization, source, serviceProvider)
        {
            generalDescriptionService = serviceProvider.GetService<IGeneralDescriptionService>();

            serviceNameMapping = new Dictionary<int, string>();
            serviceNameMapping.Add(25510, "Aineisto- ja tietopalvelu");
            serviceNameMapping.Add(25512, "Aineisto- ja tietopalvelu");
            serviceNameMapping.Add(25514, "Aineisto- ja tietopalvelu");
            serviceNameMapping.Add(26444, "Perusopetus");
            serviceNameMapping.Add(25302, "Kunnan vuokra-asunnot");
        }

        public override IVmOpenApiServiceInVersionBase MapService(int id)
        {
            var vm = base.MapService(id);

            // Set Finto items
            var generalDescriptionName = serviceNameMapping[id];
            var descSearch = generalDescriptionService.SearchGeneralDescriptions(new VmGeneralDescriptionSearchForm()
            {
                Name = generalDescriptionName
            });

            if (descSearch != null && descSearch.GeneralDescriptions.Count > 0)
            {
                var desc = generalDescriptionService.GetGeneralDescriptionVersionBase(descSearch.GeneralDescriptions.FirstOrDefault().Id, 0);
                if (vm.TargetGroups.Count <= 0)
                {
                    desc.TargetGroups.ForEach(g => vm.TargetGroups.Add(g.Uri));
                }
                if (vm.OntologyTerms.Count <= 0)
                {
                    desc.OntologyTerms.ForEach(t => vm.OntologyTerms.Add(t.Uri));
                }
                if (vm.ServiceClasses.Count <= 0)
                {
                    desc.ServiceClasses.ForEach(s => vm.ServiceClasses.Add(s.Uri));
                }
                if (vm.LifeEvents.Count <= 0)
                {
                    desc.LifeEvents.ForEach(l => vm.LifeEvents.Add(l.Uri));
                }
            }

            ValidateObject(vm);

            return vm;
        }
    }
}
