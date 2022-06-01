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

export default defineMessages({
  // URL
  rule1: {
    id: 'QualityAgent.Rule.1.Title',
    defaultMessage: 'Älä kirjoita palveluun liitetyn asiointikanavan verkko-osoitetta tähän kenttään.'
  },
  serviceChannelrule1: {
    id: 'QualityAgent.ServiceChannel.Rule.1.Title',
    defaultMessage: 'Vältä verkko-osoitteiden kirjoittamista tähän kenttään.'
  },
  organizationrule1: {
    id: 'QualityAgent.Organization.Rule.1.Title',
    defaultMessage: 'Kirjoita verkko-osoite sille varattuun kenttään.',
    description: {
      sv: 'Skriv webadressen i fältet som reserverats för den.',
      en: 'Write the web address into the field reserved for it.'
    }
  },
  // E-mail address
  rule2: {
    id: 'QualityAgent.Rule.2.Title',
    defaultMessage: 'Kirjoita sähköpostiosoite tähän kenttään ainoastaan jos se on palvelun ainoa asiointikanava.'
  },
  serviceChannelrule2: {
    id: 'QualityAgent.ServiceChannel.Rule.2.Title',
    defaultMessage: 'Kirjoita sähköpostiosoite sille varattuun kenttään.'
  },
  organizationrule2: {
    id: 'QualityAgent.Organization.Rule.2.Title',
    defaultMessage: 'Kirjoita sähköpostiosoite sille varattuun kenttään.',
    description: {
      sv: 'Skriv e-postadressen i fältet som reserverats för den.',
      en: 'Write the email address into the field reserved for it.'
    }
  },
  // Passive verbs
  rule3: {
    id: 'QualityAgent.Rule.3.Title',
    defaultMessage: 'Vältä passiivin käyttöä. Passiivi voi tehdä tekstistä vaikeaselkoista.'
  },
  // Conditional Responses
  rule4: {
    id: 'QualityAgent.Rule.4.Title',
    defaultMessage: 'Vältä lauseenvastikkeita. Kirjoita esimerkiksi "kun saavut" mieluummin kuin "saapuessasi".'
  },
  // The abstract differs from the description of the channel or service and the bottom view
  rule5: {
    id: 'QualityAgent.Rule.5.Title',
    defaultMessage: 'Tarkista, että tiivistelmässä mainitut asiat kerrotaan myös kuvaus-kentässä.'
  },
  // The service description plays the base text
  rule6: {
    id: 'QualityAgent.Rule.6.Title',
    defaultMessage: 'Älä toista pohjakuvauksen tekstiä muissa kentissä.'
  },
  // Long sentences
  rule7: {
    id: 'QualityAgent.Rule.7.Title',
    defaultMessage: 'Pyri lyhentämään pitkät virkkeet alle 25 sanaan.'
  },
  // References to lawsuit and / or articles.
  rule8: {
    id: 'QualityAgent.Rule.8.Title',
    defaultMessage: 'Vältä lakipykälien ja artiklojen mainitsemista. Ne eivät yleensä ole oleellisia lukijalle.'
  },
  // Grammar errors
  rule9: {
    id: 'QualityAgent.Rule.9.Title',
    defaultMessage: 'Tekstissä on mahdollisesti kielioppivirheitä.'
  },
  // Writing errors
  rule10: {
    id: 'QualityAgent.Rule.10.Title',
    defaultMessage: 'Tekstissä on mahdollisesti kirjoitusvirheitä.'
  },
  // Visiting Address
  rule11: {
    id: 'QualityAgent.Rule.11.Title',
    defaultMessage: 'Älä kirjoita palveluun liitetyn asiointikanavan osoitetta tähän kenttään.'
  },
  serviceChannelrule11: {
    id: 'QualityAgent.ServiceChannel.Rule.11.Title',
    defaultMessage: 'Vältä osoitteiden kirjoittamista tähän kenttään.'
  },
  organizationrule11: {
    id: 'QualityAgent.Organization.Rule.11.Title',
    defaultMessage: 'Kirjoita osoite sille varattuun kenttään.',
    description: {
      sv: 'Skriv adressen i fältet som reserverats för den.',
      en: 'Write the address into the field reserved for it.'
    }
  },
  // Phone number
  rule12: {
    id: 'QualityAgent.Rule.12.Title',
    defaultMessage: 'Älä kirjoita palveluun liitetyn asiointikanavan puhelinnumeroa tähän kenttään.'
  },
  serviceChannelrule12: {
    id: 'QualityAgent.ServiceChannel.Rule.12.Title',
    defaultMessage: 'Vältä puhelinumeroiden kirjoittamista tähän kenttään.'
  },
  // Personal Name
  rule13: {
    id: 'QualityAgent.Rule.13.Title',
    defaultMessage: 'Vältä henkilöiden nimien mainitsemista tekstissä. Käytä mieluummin työ- tai virkanimikettä.'
  },
  // Opening hours
  rule14: {
    id: 'QualityAgent.Rule.14.Title',
    defaultMessage: 'Ilmoita palveluajat asiointikanavan tiedoissa ja tarvittaessa palvelun toimintaohjeissa.'
  },
  serviceChannelrule14: {
    id: 'QualityAgent.ServiceChannel.Rule.14.Title',
    defaultMessage: 'Ilmoita palveluajat sille varatussa kentässä.'
  },
  // Date or year
  rule15: {
    id: 'QualityAgent.Rule.15.Title',
    defaultMessage: 'Jos mainitset päivämäärän tai vuosiluvun, muista pitää tieto ajantasaisena myös jatkossa.',
    description: {
      sv: 'Om du nämner datum eller årtal, kom ihåg att också hålla den informationen uppdaterad i framtiden.',
      en: 'If you mention date or year, remember to keep this information up-to-date in the future.'
    }
  }
})
