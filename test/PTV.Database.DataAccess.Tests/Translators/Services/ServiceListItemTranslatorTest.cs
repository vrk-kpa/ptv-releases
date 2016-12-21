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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

using Xunit;
using PTV.Database.DataAccess.Translators.Values;
using PTV.Database.DataAccess.Translators.Types;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class ServiceListItemTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        /// <summary>
        /// Constructor tests setup
        /// </summary>
        public ServiceListItemTranslatorTest()
        {
            translators = new List<object>()
            {
                new ServiceListItemTranslator(ResolveManager, TranslationPrimitives),
                new PublishingStatusTranslator(ResolveManager, TranslationPrimitives),
                new TypeBaseListItemTranslator(ResolveManager, TranslationPrimitives),
                new DateTimeLongValueTranslator()
            };
        }



        /// <summary>
        /// test for ServiceTranslator entity - > vm
        /// </summary>
        [Fact]
        public void TranslateServicesToViewModel()
        {
            var service = CreateService();
            var toTranslate = new List<Service>() { service };
            var translations = RunTranslationEntityToModelTest<Service, VmServiceListItem>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(translations.Count,toTranslate.Count);
            Assert.Equal(translation.Modified, service.Modified.ToEpochTime());
            Assert.Equal(translation.ModifiedBy, service.ModifiedBy);
            Assert.Equal(translation.Id, service.Id);
            Assert.Equal(translation.Name, service.ServiceNames.First().Name);
            Assert.Equal(translation.OrganizationName, service.OrganizationServices.Select(o => o.Organization.OrganizationNames.First().Name).Aggregate((s1, s2) => s1 + ", " + s2));
            Assert.Equal(translation.ServiceClassName, String.Join(", ", service.ServiceServiceClasses.Select(k => k.ServiceClass.Label).ToArray()));
            Assert.Equal(translation.ServiceTypeId, service.TypeId);
            Assert.Equal(translation.ServiceType, service.Type.Code);
        }

        private Service CreateService()
        {
            var organization = new Organization() { OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "testOrganizationName", Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } } };
            var serviceType = CreateServiceType();

            return new Service()
            {
                Created = DateTime.Now,
                CreatedBy = "testCreator",
                Id = Guid.NewGuid(),
                //Type = new ServiceType() { Id = Guid.NewGuid(), Code = "test" },
                ServiceNames = new List<ServiceName>() { new ServiceName() { Name = "testServiceName", Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } },
                OrganizationServices = new List<OrganizationService> { new OrganizationService { Organization = organization } },
                ServiceServiceClasses = new List<ServiceServiceClass>()
                {
                    new ServiceServiceClass() { ServiceClass = new ServiceClass() { Label = "testServiceClasName 1"} },
                    new ServiceServiceClass() { ServiceClass = new ServiceClass() { Label = "testServiceClasName 2"} }
                },

                Type = serviceType
            };
        }

        private ServiceType CreateServiceType()
        {
            return new ServiceType()
            {
                Code = ServiceTypeEnum.Service.ToString(),
                Id = Guid.NewGuid()
            };
        }

    }
}
