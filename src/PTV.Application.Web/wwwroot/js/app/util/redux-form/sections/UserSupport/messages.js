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
  userSupportTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Support.Title',
    defaultMessage: 'Käytön tuki'
  },
  dialCodeLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhonePrefix.Title',
    defaultMessage: 'Maan suuntanumero'
  },
  dialCodePlaceholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhonePrefix.Placeholder',
    defaultMessage: 'esim. +358'
  },
  dialCodeTooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhonePrefix.Tooltip',
    defaultMessage: 'esim. +358'
  },
  phoneNumberLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneNumber.Title',
    defaultMessage: 'Puhelinnumero'
  },
  phoneNumberTooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneNumber.Tooltip',
    defaultMessage: 'Mikäli verkkoasioinnin käyttöön on mahdollista saada tukea puhelimitse, kirjoita kenttään tukipalvelun puhelinnumero. Kirjoita puhelinnumero kansainvälisessä muodossa ilman välilyöntejä (esim. +358451234567). Älä kirjoita kenttään organisaation yleisiä puhelinnumeroita, esim. vaihteen numeroa. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi.'
  },
  phoneNumberPlaceholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneNumber.PlaceHolder',
    defaultMessage: 'esim. +35845123467'
  },
  phoneNumberInfoLabel:{
    id: 'Containers.Channels.AddElectronicChannel.Step1.AdditionalInformation.Title',
    defaultMessage: 'Lisätieto'
  },
  phoneNumberInfoTooltip:{
    id: 'Containers.Channels.AddElectronicChannel.Step1.AdditionalInformation.Tooltip',
    defaultMessage: 'Voit tarvittaessa antaa puhelinnumerolle lisätiedon, jolla voit tarkentaa, mistä numerosta on kyse. Esimerkiksi vaihde tai asiakaspalvelu.'
  },
  phoneNumberInfoPlaceholder:{
    id: 'Containers.Channels.AddElectronicChannel.Step1.AdditionalInformation.Placeholder',
    defaultMessage:'esim. Vaihde'
  },
  localServiceNumberLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.FinishServiceNumber.Name',
    defaultMessage: 'Suomalainen palvelunumero'
  },
  chargeTypeLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCost.Title',
    defaultMessage: 'Puhelun maksullisuus'
  },
  chargeTypeTooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCost.Tootltip',
    defaultMessage: 'Ensimmäinen vaihtoehto tarkoittaa, että puhelu maksaa lankaliittymästä soitettaessa paikallisverkkomaksun (pvm), matkapuhelimesta soitettaessa matkapuhelinmaksun (mpm) tai ulkomailta soitettaessa ulkomaanpuhelumaksun. Valitse "täysin maksuton" vain, jos puhelu on soittajalle kokonaan maksuton. Jos puhelun maksu muodostuu muulla tavoin, valitse "muu maksu" ja kuvaa puhelun hintatiedot omassa kentässään.'
  },
  phoneCostDescriptionLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCostOtherDescription.Title',
    defaultMessage: 'Puhelun hintatiedot'
  },
  phoneCostDescriptionPlaceholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCostOtherDescription.Placeholder',
    defaultMessage: 'esim. Pvm:n lisäksi jonotuksesta veloitetaan...'
  },
  phoneCostDescriptionTooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCostOtherDescription.Tooltip',
    defaultMessage: 'Kerro sanallisesti, mitä puhelinnumeroon soittaminen maksaa.'
  },
  emailLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  emailTooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Tooltip',
    defaultMessage: 'Mikäli verkkoasioinnin käyttöön on mahdollista saada tukea sähköpostitse, kirjoita kenttään tukipalvelun sähköpostiosoite. Älä kirjoita kenttään organisaation yleisiä sähköpostiosoitteita, esim. kirjaamoon. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi.'
  },
  emailPlaceholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Placeholder',
    defaultMessage: 'esim. palvelupiste@organisaatio.fi'
  }
})
