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
using PTV.Database.DataAccess.Translators.Addresses;
using PTV.Database.Model.Models;

namespace PTV.Database.DataAccess.EntityCloners
{
    /// <summary>
    /// Register Base cloning definitions i.e. definition that has no another special requirement for copying and all related entities will not be copied anymore.
    /// </summary>
    public static class BaseEntityCloners
    {
        public static void RegisterBaseEntityCloners(IServiceCollection service)
        {
            service.RegisterBaseCloner<ServiceName>();
            service.RegisterBaseCloner<ServiceDescription>();
            service.RegisterBaseCloner<ServiceKeyword>();
            service.RegisterBaseCloner<ServiceLanguage>();
            service.RegisterBaseCloner<ServiceLanguageAvailability>();
            service.RegisterBaseCloner<ServiceLifeEvent>();
            service.RegisterBaseCloner<ServiceOntologyTerm>();
            service.RegisterBaseCloner<ServiceRequirement>();
            service.RegisterBaseCloner<ServiceServiceChannelDescription>();
            service.RegisterBaseCloner<ServiceServiceClass>();
            service.RegisterBaseCloner<ServiceTargetGroup>();
            service.RegisterBaseCloner<ServiceIndustrialClass>();
            service.RegisterBaseCloner<ServiceElectronicNotificationChannel>();
            service.RegisterBaseCloner<ServiceElectronicCommunicationChannel>();            
            service.RegisterBaseCloner<ServiceArea>();
            service.RegisterBaseCloner<ServiceAreaMunicipality>();
            service.RegisterBaseCloner<ServiceServiceChannelExtraTypeDescription>();

            service.RegisterBaseCloner<ServiceChannelName>();
            service.RegisterBaseCloner<ServiceChannelDescription>();
            service.RegisterBaseCloner<ServiceChannelKeyword>();
            service.RegisterBaseCloner<ServiceChannelLanguage>();
            service.RegisterBaseCloner<ServiceChannelLanguageAvailability>();
            service.RegisterBaseCloner<ServiceChannelOntologyTerm>();
            service.RegisterBaseCloner<ServiceChannelTargetGroup>();
            service.RegisterBaseCloner<ServiceChannelArea>();
            service.RegisterBaseCloner<ServiceChannelAreaMunicipality>();
            service.RegisterBaseCloner<Attachment>();
            service.RegisterBaseCloner<ServiceChannelServiceClass>();
            service.RegisterBaseCloner<ServiceHoursAdditionalInformation>();
            service.RegisterBaseCloner<DailyOpeningTime>();
            service.RegisterBaseCloner<ElectronicChannelUrl>();
            service.RegisterBaseCloner<PrintableFormChannelUrl>();
            service.RegisterBaseCloner<WebpageChannelUrl>();
            service.RegisterBaseCloner<PrintableFormChannelIdentifier>();
            service.RegisterBaseCloner<PrintableFormChannelReceiver>();

            service.RegisterBaseCloner<OrganizationName>();
            service.RegisterBaseCloner<OrganizationDescription>();
            service.RegisterBaseCloner<OrganizationLanguageAvailability> ();
            service.RegisterBaseCloner<OrganizationArea>();
            service.RegisterBaseCloner<OrganizationAreaMunicipality>();
            service.RegisterBaseCloner<OrganizationDisplayNameType>();

            // Common cloners
            service.RegisterBaseCloner<Email>();
            service.RegisterBaseCloner<Phone>();
            service.RegisterBaseCloner<StreetName>();
            service.RegisterBaseCloner<PostOfficeBoxName>();
            service.RegisterBaseCloner<AddressForeignTextName>();
            service.RegisterBaseCloner<Coordinate>();
            service.RegisterBaseCloner<AddressAdditionalInformation>();
            service.RegisterBaseCloner<WebPage>();

            service.RegisterBaseCloner<LawName>();

            service.RegisterBaseCloner<StatutoryServiceName>();
            service.RegisterBaseCloner<StatutoryServiceDescription>();
            //service.RegisterBaseCloner<StatutoryServiceLaw>();
            service.RegisterBaseCloner<StatutoryServiceLanguage>();
            service.RegisterBaseCloner<GeneralDescriptionLanguageAvailability>();
            service.RegisterBaseCloner<StatutoryServiceLifeEvent>();
            service.RegisterBaseCloner<StatutoryServiceOntologyTerm>();
            service.RegisterBaseCloner<StatutoryServiceRequirement>();
            service.RegisterBaseCloner<StatutoryServiceServiceClass>();
            service.RegisterBaseCloner<StatutoryServiceTargetGroup>();
            service.RegisterBaseCloner<StatutoryServiceIndustrialClass>();

            service.RegisterBaseCloner<ServiceProducerAdditionalInformation>();
            service.RegisterBaseCloner<ServiceProducerOrganization>();
        }

        public static void RegisterBaseCloner<T>(this IServiceCollection service) where T : class,new()
        {
            service.AddTransient<IEntityCloner<T>,BaseCloner<T>>();
        }
    }
}