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
import { defineMessages } from 'react-intl'

export default defineMessages({
  name: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Name.Title',
    defaultMessage: 'Nimi'
  },
  shortDescription: {
    // 'ReduxForm.Fields.ShortDescription.Label'
    id: 'Containers.Services.AddService.Step1.ShortDescription.Title',
    defaultMessage: 'Lyhyt kuvaus'
  },
  description: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  fundingType: {
    id: 'Containers.Services.AddService.Step1.FundingType.Title',
    defaultMessage: 'Rahoitustyyppi'
  },
  languages: {
    id: 'Containers.Services.AddService.Step1.AvailableLanguages.Title',
    defaultMessage: 'Kielet, joilla palvelu on saatavilla'
  },
  responsibleOrganizations : {
    id : 'Containers.Services.AddService.Step1.Organization.Title',
    defaultMessage: 'Palvelun vastuuorganisaatio'
  },
  targetGroups: {
    id: 'Containers.Services.AddService.Step2.TargetGroup.Title',
    defaultMessage: 'Kohderyhmä'
  },
  serviceClasses: {
    id: 'Containers.Services.AddService.Step2.ServiceClass.Title',
    defaultMessage: 'Palveluluokka'
  },
  ontologyTerms: {
    id: 'Containers.Services.AddService.Step2.OntologyTerms.Title',
    defaultMessage: 'Ontologiakäsitteet'
  },
  serviceProducers: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.Header.Title',
    defaultMessage : 'Palvelun toteutustapa ja tuottaja'
  },
  visitingAddresses: {
    id: 'Containers.Cahnnels.VistingAddress.Title',
    defaultMessage: 'Käyntiosoite'
  },
  signatureCount: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AuthenticationNumberSign.Title',
    defaultMessage: 'Allekirjoitusten lukumäärä'
  },
  street : {
    id: 'Containers.Channels.Address.Street.Title',
    defaultMessage: 'Katuosoite'
  },
  formFiles: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Section.Title',
    defaultMessage: 'Lisää verkkosivu'
  },
  urlAddress: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.UrlChecker.Title',
    defaultMessage: 'Verkko-osoite'
  },
  phoneNumbers: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneNumber.Title',
    defaultMessage: 'Puhelinnumero'
  },
  phoneNumber: {
    id: 'Containers.Manage.Organizations.Manage.Step1.PhoneNumber.Title',
    defaultMessage: 'Puhelinnumero'
  },
  backgroundDescription: {
    id: 'Containers.GeneralDescription.BackgroundDescription.Description.Title',
    defaultMessage: 'Taustakuvaus'
  },
  areaType: {
    id: 'Containers.Services.AddService.Step1.AreaInformation.AreaType.Title',
    defaultMessage: 'Aluetyyppi'
  },
  areaInformation: {
    id: 'ReduxForm.Common.AreaInformation',
    defaultMessage: 'Alue'
  },
  foreignAddressText: {
    id: 'Containers.Channels.Address.Foreign.Title',
    defaultMessage: 'Vapaasti täydennettävä osoite'
  },
  faxNumbers: {
    id: 'Containers.Channels.Common.FaxNumber.Title',
    defaultMessage: 'Faksinumero'
  },
  laws: {
    id: 'Containers.Services.AddService.Step1.Laws.Title',
    defaultMessage: 'Linkki lakitietoihin'
  },
  attachments: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Attachment.Title',
    defaultMessage: 'Liitteet ja lisätietolinkit'
  },
  webPages: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step2.WebPage.Section.Title',
    defaultMessage: 'Lisää verkkosivu'
  }
})
