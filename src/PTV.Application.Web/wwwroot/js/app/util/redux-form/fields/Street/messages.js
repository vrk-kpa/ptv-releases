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

const messages = defineMessages({
  placeholder : {
    id: 'Containers.Channels.Address.Street.Placeholder',
    defaultMessage: 'esim. Mannerheimintie'
  },
  visitingTooltip : {
    id: 'Containers.Channels.VistingAddress.Street.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
  },
  postalTooltip : {
    id: 'Containers.Channels.PostalAddress.Street.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
  },
  deliveryTooltip : {
    id: 'Containers.Channels.DeliveryAddress.Street.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
  },
  visitingTooltip1 : {
    id: 'Containers.Channels.VistingAddress.Street.First.Tooltip',
    defaultMessage: 'Tooltip for the first visiting address'
  },
  postalTooltip1 : {
    id: 'Containers.Channels.PostalAddress.Street.First.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.',
    description: 'Containers.Channels.PostalAddress.Street.Tooltip'
  },
  deliveryTooltip1 : {
    id: 'Containers.Channels.DeliveryAddress.Street.First.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.',
    description: 'Containers.Channels.DeliveryAddress.Street.Tooltip'
  },
  menuStreetName: {
    id: 'Containers.Channels.Address.Street.Name',
    defaultMessage: 'Kadunnimi'
  },
  menuPostalCode: {
    id: 'Containers.Cahnnels.Address.PostalCode',
    defaultMessage: 'Postinumero'
  },
  menuPostOffice: {
    id: 'Containers.Channels.Address.PostOffice',
    defaultMessage: 'Postitoimipaikka'
  },
  visitingStreetNotFoundError: {
    id: 'Containers.Channels.Address.VisitingStreet.NameNotFound',
    defaultMessage: 'Osoitetta ei löydy järjestelmästä. Tarkista osoite tai lisää uusi osoite käyttämällä. Muu sijantitieto -elementtiä.',
    description: 'Containers.Channels.Address.Street.NameNotFound'
  },
  deliveryStreetNotFoundError: {
    id: 'Containers.Channels.Address.DeliveryStreet.NameNotFound',
    defaultMessage: 'Osoitetta ei löydy järjestelmästä. Tarkista osoite.'

  },
  postalStreetNotFoundError: {
    id: 'Containers.Channels.PostalAddress.PostalStreet.NameNotFound',
    defaultMessage: 'Osoitetta ei löydy järjestelmästä. Tarkista osoite.'
  }
})

export default messages
