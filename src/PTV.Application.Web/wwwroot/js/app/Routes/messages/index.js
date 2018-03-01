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

export const messages = defineMessages({
  languageVersionTitleNew: {
    id: 'Routes.Form.LanguageVersion.New.Title',
    defaultMessage: 'Uusi kieliversio'
  },
  organizationLabel: {
    id: 'FrontPage.SelectOrganization.Title',
    defaultMessage: 'Organisaatio'
  },
  organizationTooltip: {
    id: 'FrontPage.SelectOrganization.Tooltip',
    defaultMessage: 'Valitse pudotusvalikosta haluamasi organisaatio tai organisaatiotaso.'
  },
  organizationPlaceholder: {
    id: 'Component.Select.Placeholder',
    defaultMessage: '- valitse -'
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
