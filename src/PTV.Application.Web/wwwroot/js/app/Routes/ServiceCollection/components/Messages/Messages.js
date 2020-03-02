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

export const serviceCollectionMessages = defineMessages({
  nameTitle: {
    id: 'Routes.ServiceCollection.Name.Title',
    defaultMessage: 'Nimi'
  },
  namePlaceholder: {
    id: 'Routes.ServiceCollection.Name.Placeholder',
    defaultMessage: 'Kirjoita palvelua kuvaava, asiakaslähtöinen nimi.'
  },
  nameTooltip: {
    id: 'Routes.ServiceCollection.Name.Tooltip',
    defaultMessage: 'Kirjoita palvelun nimi mahdollisimman kuvaavasti. Älä kirjoita organisaation nimeä palvelun nimeen. Jos olet käyttänyt palvelun pohjakuvausta, muokkaa nimeä vain jos se on ehdottoman välttämätöntä!'
  },
  descriptionTitle: {
    id: 'Routes.ServiceCollection.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  descriptionPlaceholder: {
    id: 'Routes.ServiceCollection.Description.PlaceHolder',
    defaultMessage: 'Kirjoita palvelulle selkeä ja ymmärrettävä kuvaus.'
  },
  descriptionTooltip: {
    id: 'Routes.ServiceCollection.Description.Tooltip',
    defaultMessage: 'Kuvaa palvelu mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kerro, mitä palvelu pitää sisällään, miten sitä tarjotaan asiakkaalle, mihin tarpeeseen se vastaa ja mihin sillä pyritään. Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa asiakkaalle tarjottavaa palvelua, älä palvelua järjestävää organisaatiota tai sen tehtäviä! Voit jakaa tekstiä kappaleisiin ja tarvittaessa käyttää luettelomerkkejä. Jos olet liittänyt palveluun pohjakuvauksen, käytä kuvauskenttä sen kertomiseen, miten pohjakuvauksessa kuvattu palvelu on sinun organisaatiossasi/seudullasi/kunnassasi järjestetty ja mitä erityispiirteitä tällä palvelulla on. Älä toista pohjakuvauksessa jo kerrottuja asioita. '
  },
  organizationTitle: {
    id: 'Routes.ServiceCollection.Organization.Title',
    defaultMessage: 'Vastuuorganisaatio'
  }
})
