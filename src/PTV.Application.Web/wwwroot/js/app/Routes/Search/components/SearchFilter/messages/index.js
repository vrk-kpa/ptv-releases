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
import { defineMessages } from 'util/react-intl'

export const messages = defineMessages({
  // FILTER TITLES
  contentTypeFilterTitle: {
    id: 'FrontPage.ContentTypeFilter.Title',
    defaultMessage: 'Sisältötyyppi'
  },
  organizationFilterTitle: {
    id: 'FrontPage.OrganizationFilter.Title',
    defaultMessage: 'Organisaatio'
  },
  languageFilterTitle: {
    id: 'FrontPage.LanguageFilter.Title',
    defaultMessage: 'Kieliversiot'
  },
  publishingStatusFilterTitle: {
    id: 'FrontPage.StateFilter.Title',
    defaultMessage: 'Tila'
  },
  targetGroupFitlerTitle: {
    id: 'FrontPage.TargetGroupFilter.Title',
    defaultMessage: 'Kohderyhmä'
  },
  serviceClassFilterTitle: {
    id: 'FrontPage.ServiceClassFilter.Title',
    defaultMessage: 'Palveluluokka'
  },
  keywordFilterTitle: {
    id: 'FrontPage.KeywordFilter.Title',
    defaultMessage: 'Asiasanat'
  },
  lifeEventFilterTitle: {
    id: 'FrontPage.LifeEventFilter.Title',
    defaultMessage: 'Elämäntilanne'
  },
  industrialClassFilterTitle: {
    id: 'FrontPage.IndustrialClassFilter.Title',
    defaultMessage: 'Toimiala'
  },
  serviceTypeFilterTitle: {
    id: 'FrontPage.ServiceTypeFilter.Title',
    defaultMessage: 'Palvelutyyppi'
  },
  generalDescriptionFilterTitle: {
    id: 'FrontPage.GeneralDescriptionTypeFilter.Title',
    defaultMessage: 'Käyttöaluetyyppi'
  },
  // CONTENT TYPES
  service: {
    id: 'FrontPage.SearchContentSwitcher.Services.Title',
    defaultMessage: 'Palvelukuvaus'
  },
  serviceService: {
    id: 'FrontPage.SearchContentSwitcher.Services.Service.Title',
    defaultMessage: 'Palvelu'
  },
  serviceProfessional: {
    id: 'FrontPage.SearchContentSwitcher.Services.ProfessionalQualification.Title',
    defaultMessage: 'Ammattipätevyys'
  },
  servicePermit: {
    id: 'FrontPage.SearchContentSwitcher.Services.PermitOrOtherObligation.Title',
    defaultMessage: 'Lupa tai muu velvoite'
  },
  channel: {
    id: 'FrontPage.SearchContentSwitcher.Channels.Title',
    defaultMessage: 'Asiointikanava'
  },
  generalDescription: {
    id: 'FrontPage.SearchContentSwitcher.GeneralDescriptions.Title',
    defaultMessage: 'Pohjakuvaus'
  },
  organization: {
    id: 'FrontPage.SearchContentSwitcher.Organizations.Title',
    defaultMessage: 'Organisaatiokuvaus'
  },
  eChannel: {
    id: 'FrontPage.SearchContentSwitcher.ElectronicChannel.Title',
    defaultMessage: 'Verkkoasiointi'
  },
  webPage: {
    id: 'FrontPage.SearchContentSwitcher.WebChannel.Title',
    defaultMessage: 'Verkkosivu'
  },
  printableForm: {
    id: 'FrontPage.SearchContentSwitcher.PrintableFormChannel.Title',
    defaultMessage: 'Tulostettava lomake'
  },
  phone: {
    id: 'FrontPage.SearchContentSwitcher.PhoneChannel.Title',
    defaultMessage: 'Puhelinasiointi'
  },
  serviceLocation: {
    id: 'FrontPage.SearchContentSwitcher.ServiceLocationChannel.Title',
    defaultMessage: 'Palvelupiste'
  },
  serviceCollection: {
    id: 'FrontPage.SearchContentSwitcher.ServiceCollection.Title',
    defaultMessage: 'Palvelukokonaisuus'
  },
  // MODAL DIALOG TITLE PREFIX
  commonSearchFilterTitle: {
    id: 'FrontPage.Modal.Prefix',
    defaultMessage: 'Hakusuodatin'
  },
  commonSearchLabel: {
    id: 'FrontPage.Modal.Search.Label',
    defaultMessage: 'Haku'
  },
  // ALL SELECTED LABEL
  allSelectedLabel: {
    id: 'FrontPage.Filter.AllSelected',
    defaultMessage: 'Kaikki'
  },
  // NOTHING SELECTED ERROR PREFIX
  nothingSelectedPrefix: {
    id: 'FrontPage.Filter.Nothing.Prefix',
    defaultMessage: 'Sinun pitää valita ainakin yksi'
  },
  // ORGANIZATION DIALOG
  organizationSearchPlaceholder: {
    id: 'FrontPage.OrganizationFilter.Search.Placeholder',
    defaultMessage: 'Write a search word'
  }
})
