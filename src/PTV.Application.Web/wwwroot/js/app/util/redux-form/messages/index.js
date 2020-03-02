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
import { defineMessages } from 'util/react-intl'
import { entityConcreteTypesEnum } from 'enums'

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
  organizationType: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.Type.Title',
    defaultMessage: 'Organisaatiotyyppi *'
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
  postalAddresses: {
    id: 'Containers.Cahnnels.PostalAddress.Title',
    defaultMessage: 'Postiosoite (eri kuin käyntiosoite)'
  },
  signatureCount: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AuthenticationNumberSign.Title',
    defaultMessage: 'Allekirjoitusten lukumäärä'
  },
  street: {
    id: 'Containers.Channels.Address.Street.Title',
    defaultMessage: 'Katuosoite'
  },
  coordinates: {
    id: 'Containers.Channels.Address.Coordinates.Title',
    defaultMessage: 'Koordinaatit'
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
  deliveryAddresses: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.DeliveryAddress.Title',
    defaultMessage: 'Toimitusosoite'
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
  },
  emails: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  email: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  chargeDescription: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCostOtherDescription.Title',
    defaultMessage: 'Puhelun hintatiedot'
  },
  serviceVouchers: {
    id: 'Containers.Services.AddService.Step1.VoucherSection.Title',
    defaultMessage: 'Palvelusetelipalvelut'
  },
  streetAddressType: {
    id: 'Containers.Channels.Address.Type.Street',
    defaultMessage: 'Katuosoite'
  },
  postofficeboxAddressType: {
    id: 'Containers.Channels.Address.Type.POBox',
    defaultMessage: 'Postilokero-osoite'
  },
  foreignAddressType: {
    id: 'Containers.Channels.Address.Type.Foreign',
    defaultMessage: 'Ulkomainen osoite'
  },
  otherAddressType: {
    id: 'Containers.Channels.Address.Type.Other',
    defaultMessage: 'Muu sijaintitieto'
  },
  noaddressAddressType: {
    id: 'Containers.Channels.Address.Type.NoAddress',
    defaultMessage: 'Toimitustieto sanallisesti'
  },
  poBox : {
    id: 'Containers.Channels.Address.POBox.Title',
    defaultMessage: 'Postilokero-osoite'
  },
  postalCode: {
    id: 'Containers.Channels.Address.PostalCode.Title',
    defaultMessage: 'Postinumero'
  },
  organization: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Organization.Title',
    defaultMessage: 'Organisaatio'
  },
  isOpenNonStop: {
    id: 'Containers.Channels.Common.OpeningHours.Nonstop.Title',
    defaultMessage: 'Aina avoinna'
  },
  isReservation: {
    id: 'Containers.Channels.Common.OpeningHours.Reservation.Title',
    defaultMessage: 'Avoinna ajanvarauksella'
  },
  openingHours: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.Header.Title',
    defaultMessage: 'Vaihe 2/2: Aukioloajat'
  },
  electronicInvoicingAddresses: {
    id: 'Containers.Manage.Organizations.Manage.ElectronicInvoicingAddressCollection.Title',
    defaultMessage: 'Verkkolaskutusosoite'
  },
  provinceLabel: {
    id: 'ReduxForm.Fields.ProvincesList.Label',
    defaultMessage: 'Maakunta'
  },
  municipalityLabel: {
    id: 'ReduxForm.Fields.MunicipalitiesList.Label',
    defaultMessage: 'Kunta'
  },
  hospitalRegionLabel: {
    id: 'ReduxForm.Fields.HospitalRegionsList.Label',
    defaultMessage: 'Sairaanhoitopiiri'
  },
  businessRegionLabel: {
    id: 'ReduxForm.Fields.BusinessRegionsList.Label',
    defaultMessage: 'Yrityspalvelujen seutualue'
  },
  userInstruction: {
    id: 'Containers.Services.AddService.Step1.ServiceUserInstruction.Title',
    defaultMessage: 'Toimintaohjeet'
  },
  alternateName: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.AlternativeName.Title',
    defaultMessage: 'Vaihtoehtoinen nimi'
  },
  invalidItemTooltip:{
    id: 'Util.ReduxForm.Fields.FintoItem.Invalid.Tooltip',
    defaultMessage: 'Tämä Finto-asiasana ei ole enää käytössä.',
    description: { en: 'The Finto term is no longer in use.' }
  },
  accessibilityClassifications: {
    id: 'Util.ReduxForm.Sections.AccessibilityClassification.Title',
    defaultMessage: 'Saavutettavuustiedot'
  },
  wcagLevelType: {
    id: 'Util.ReduxForm.Fields.WcagLevelType.Title',
    defaultMessage: 'WCAG-tasot'
  }
})

export const entityTypesMessages = defineMessages({
  addNewContentTitle: {
    id: 'FrontPage.Buttons.AddNewContent.Title',
    defaultMessage: 'Luo uusi'
  },
  services: {
    id: 'FrontPage.AddNewMenu.Services.LinkTitle',
    defaultMessage: 'Palvelukuvaus'
  },
  serviceCollections: {
    id: 'FrontPage.AddNewMenu.ServiceCollection.LinkTitle',
    defaultMessage: 'Palvelukokonaisuus'
  },
  channels: {
    id: 'FrontPage.AddNewMenu.Channels.LinkTitle',
    defaultMessage: 'Asiontikanava'
  },
  eChannelLinkTitle: {
    id: 'FrontPage.AddNewMenu.ElectronicChannel.LinkTitle',
    defaultMessage: 'Verkkoasiointi'
  },
  webPageLinkTitle: {
    id: 'FrontPage.AddNewMenu.WebPage.LinkTitle',
    defaultMessage: 'Verkkosivu'
  },
  printableFormLinkTitle: {
    id: 'FrontPage.AddNewMenu.PrintableForm.LinkTitle',
    defaultMessage: 'Tulostettava lomake'
  },
  phoneLinkTitle: {
    id: 'FrontPage.AddNewMenu.Phone.LinkTitle',
    defaultMessage: 'Puhelinasiointi'
  },
  serviceLocationLinkTitle: {
    id: 'FrontPage.AddNewMenu.ServiceLocation.LinkTitle',
    defaultMessage: 'Palvelupiste'
  },
  generalDescriptions: {
    id: 'FrontPage.AddNewMenu.GeneralDescription.LinkTitle',
    defaultMessage: 'Pohjakuvaukset'
  },
  organizations: {
    id: 'FrontPage.AddNewMenu.Organizations.LinkTitle',
    defaultMessage: 'Organisaatiokuvaukset'
  }
})

export const entityConcreteTexts = {
  [entityConcreteTypesEnum.SERVICE]: entityTypesMessages.services,
  [entityConcreteTypesEnum.SERVICECOLLECTION]: entityTypesMessages.serviceCollections,
  [entityConcreteTypesEnum.ELECTRONICCHANNEL]: entityTypesMessages.eChannelLinkTitle,
  [entityConcreteTypesEnum.PHONECHANNEL]: entityTypesMessages.phoneLinkTitle,
  [entityConcreteTypesEnum.PRINTABLEFORMCHANNEL]: entityTypesMessages.printableFormLinkTitle,
  [entityConcreteTypesEnum.SERVICELOCATIONCHANNEL]: entityTypesMessages.serviceLocationLinkTitle,
  [entityConcreteTypesEnum.WEBPAGECHANNEL]: entityTypesMessages.webPageLinkTitle,
  [entityConcreteTypesEnum.GENERALDESCRIPTION]: entityTypesMessages.generalDescriptions,
  [entityConcreteTypesEnum.ORGANIZATION]: entityTypesMessages.organizations,
  // the concrete type is removed during routing,
  // but this removal initiate render of components before unmount and it cause undefine message for intl
  [null]: entityTypesMessages.services,
  [undefined]: entityTypesMessages.services
}

export const commonAppMessages = defineMessages({
  emptySearch: {
    id: 'Util.ReduxForm.EmptySearch.Text',
    defaultMessage: '- nothing found -'
  },
  emptySelection: {
    id: 'Util.ReduxForm.EmptySelection.Text',
    defaultMessage: '- nothing selected -'
  },
  searchFilter: {
    id: 'Util.ReduxForm.SearchFilter.Title',
    defaultMessage: 'Hakutulokset:'
  },
  translationArrived: {
    id: 'TranslationOrder.Arrived.Title',
    defaultMessage: 'Käännös saapunut',
    description: 'Util.ReduxForm.Renders.RenderLanguageTabSwitcher.TranslationArrived.Tooltip'
  },
  translationOrdered: {
    id: 'TranslationOrder.Ordered.Title',
    defaultMessage: 'Käännös tilattu',
    description: 'Util.ReduxForm.Renders.RenderLanguageAvailabilitiesTable.TranslationOrdered.Title'
  },
  copyLink:{
    id: 'CopyToClipboard.Link.Title',
    defaultMessage: 'Copy to clipboard'
  },
  toggleDetailsButtonTitle: {
    id: 'ToggleDetailsButton.Title',
    defaultMessage: 'Show details'
  }
})
