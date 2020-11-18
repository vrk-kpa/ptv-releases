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

export const channelDescriptionMessages = defineMessages({
  nameTitle: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.Name.Title',
    defaultMessage: 'Nimi'
  },
  namePlaceholder: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.Name.Placeholder',
    defaultMessage: 'Kirjoita asiointikanavaa kuvaava, asiakaslähtöinen nimi.'
  },
  nameTooltip: {
    id: 'Containers.Channels.AddServiceLocationChannel.Name.Tooltip',
    defaultMessage: 'Service loaction channel name tooltip.'
  },
  alternativeNameTooltip: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.AlternativeName.Tooltip',
    defaultMessage: 'Alternative name tooltip'
  },
  alternativeNamePlaceholder: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.AlternativeName.Placeholder',
    defaultMessage: 'Alternative name placeholder'
  },
  organizationLabel: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.Organization.Title',
    defaultMessage: 'Organisaatio'
  },
  organizationInfo: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.Organization.Tooltip',
    defaultMessage: 'Valitse alasvetovalikosta organisaatio tai alaorganisaatio, joka vastaa palvelupisteestä. Mikäli palvelupiste on käytössä useilla alaorganisaatioilla, valitse pelkkä organisaation päätaso.'
  },
  shortDescriptionLabel: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.ShortDescription.Title',
    defaultMessage: 'Lyhyt kuvaus'
  },
  shortDescriptionInfo: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.ShortDescription.Tooltip',
    defaultMessage: 'Kirjoita lyhyt kuvaus eli tiivistelmä palvelupisteestä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään palvelun.'
  },
  shortDescriptionPlaceholder: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.ShortDescription.Placeholder',
    defaultMessage: 'Kirjoita lyhyt tiivistelmä hakukoneita varten.'
  },
  descriptionLabel: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  descriptionInfo: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.Description.Tooltip',
    defaultMessage: 'Kuvaa palvelupiste mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kuvaa yleisesti, mitä asioita palvelupisteessä voi hoitaa. Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa vain palvelupistettä, älä palvelua järjestävää organisaatiota tai sen tehtäviä!'
  },
  descriptionPlaceholder: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.Description.Placeholder',
    defaultMessage: 'Kirjoita selkeä ja ymmärrettävä kuvausteksti.'
  },
  languagesTitle: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.Language.Title',
    defaultMessage: 'Kielet, joilla palvelupisteessä palvellaan'
  },
  languagesTooltip: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.Language.Tooltip',
    defaultMessage: 'Valitse tähän ne kielet, joilla palvelupisteessä palvellaan asiakkaita. Aloita kielen nimen kirjoittaminen, niin saat näkyviin kielilistan, josta voit valita kielet.'
  },
  languagesPlaceholder: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step1.Language.Placeholder',
    defaultMessage: 'Kirjoita ja valitse listasta kielet'
  },
  visitingAddressGroupTitle: {
    id: 'Routes.Channels.ServiceLocation.VisitingAddress.GroupTitle',
    defaultMessage: 'Service location'
  },
  visitingAddressGroupTooltip: {
    id: 'Routes.Channels.ServiceLocation.VisitingAddress.GroupTooltip',
    defaultMessage: 'You can enter the location of the service location either by typing the street address in Finland, the coordinates of the service location (Other location information) or the foreign address'
  }
})
