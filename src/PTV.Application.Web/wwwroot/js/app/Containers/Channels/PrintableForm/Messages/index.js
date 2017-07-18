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
  mainTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Header.Title',
    defaultMessage: 'Lisää uusi kanava: tulostettava lomake'
  },
  mainTitleView: {
    id: 'Containers.Channels.AddPrintableFormChannel.Header.Title.View',
    defaultMessage: 'Lisää uusi kanava: tulostettava lomake'
  },
  mainText: {
    id: 'Containers.Channels.AddPrintableFormChannel.Description',
    defaultMessage: 'Tässä osiossa voit lisätä uuden tulostettavan lomakkeen. Tulostettavalla lomakkeella tarkoitetaan tiedostomuotoista lomaketta, jolle annetaan verkko-osoite organisaation omalla palvelimella olevaan tiedostoon.'
  },
  mainTextView: {
    id: 'Containers.Channels.ViewPrintableFormChannel.Description',
    defaultMessage: 'Tällä sivulla voit katsella, muokata ja julkaista asiointikanavan tietoja.'
  },
  subTitle1: {
    id: 'Components.AddPrintableFormChannel.SubTitle1',
    defaultMessage: 'Vaihe 1/1: Perustiedot'
  },
  subTitle1View: {
    id: 'Components.AddPrintableFormChannel.SubTitle1.View',
    defaultMessage: 'Vaihe 1/1: Perustiedot'
  },
  supportTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Support.Title',
    defaultMessage: 'Käytön tuki'
  }
})

export const deleteMessages = defineMessages({
  text: {
    id: 'Containers.Channels.PrintableFormChannel.Delete.Text',
    defaultMessage: 'Oletko varma, että haluat poistaa kanavakuvauksen?'
  },
  buttonOk: {
    id: 'Containers.Channels.PrintableFormChannel.Delete.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Channels.PrintableFormChannel.Delete.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const withdrawMessages = defineMessages({
  text: {
    id: 'Containers.Channels.PrintableFormChannel.Withdraw.Text',
    defaultMessage: 'Are you sure, you want to withdraw printable form channel?'
  },
  buttonOk: {
    id: 'Containers.Channels.PrintableFormChannel.Withdraw.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Channels.PrintableFormChannel.Withdraw.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const restoreMessages = defineMessages({
  text: {
    id: 'Containers.Channels.PrintableFormChannel.Restore.Text',
    defaultMessage: 'Are you sure, you want to restore printable form channel?'
  },
  buttonOk: {
    id: 'Containers.Channels.PrintableFormChannel.Restore.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Channels.PrintableFormChannel.Restore.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const urlAttachmentsMessages = defineMessages({
  attachmentsInfo: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Attachment.Tooltip',
    defaultMessage: 'Voit antaa linkkejä verkkoasiointikanavaan liittyviin ohjedokumentteihin tai -sivuihin. Liitteen tulee sijaita ulkoisessa verkko-osoitteessa, palvelutietovarantoon ei voi ladata tai tallentaa liitetiedostoja. Kirjoita liitteelle selkeä, kuvaava nimi, tarvittaessa tarkempi kuvausteksti ja linkki liitteeseen.'
  },
  attachmentsButton: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Attachment.Button.AddAttachment.Title',
    defaultMessage: 'Lisää liite'
  },
  attachmentsLabel: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Attachment.Title',
    defaultMessage: 'Liitteet ja lisätietolinkit'
  },
  label: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.AttachmentUrl.Title',
    defaultMessage: 'Verkko-osoite'
  },
  tooltip:  {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.AttachmentUrl.Tooltip',
    defaultMessage: 'Anna mahdollisimman tarkka verkko-osoite, josta dokumentti tai verkkosivu avautuu. Kopioi ja liitä verkko-osoite ja tarkista verkko-osoitteen toimivuus Testaa osoite -painikkeesta.'
  },
  placeholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.AttachmentUrl.Placeholder',
    defaultMessage: 'Kopioi ja liitä tarkka verkko-osoite.'
  },
  button: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.AttachmentUrl.Button.Title',
    defaultMessage: 'Testaa osoite'
  },
  checkerInfo:  {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.AttachmentUrl.Icon.Tooltip',
    defaultMessage: 'Verkko-osoitetta ei löytynyt, tarkista sen muoto.'
  }
})

export const cancelMessages = defineMessages({
  text: {
    id: 'Containers.Channels.PrintableFormChannel.Cancel.Text',
    defaultMessage: 'Oletko varma, että haluat keskeyttää uuden kanavan luonnin? Tekemäsi muutokset katoavat.'
  },
  buttonOk: {
    id: 'Containers.Channels.PrintableFormChannel.Cancel.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Channels.PrintableFormChannel.Cancel.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const channelDescriptionMessages = defineMessages({
  nameTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Name.Title',
    defaultMessage: 'Nimi'
  },
  namePlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Name.Placeholder',
    defaultMessage: 'Kirjoita asiointikanavaa kuvaava, asiakaslähtöinen nimi.'
  },
  organizationLabel: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Organization.Title',
    defaultMessage: 'Organisaatio'
  },
  organizationInfo: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Organization.Tooltip',
    defaultMessage: 'Valitse alasvetovalikosta organisaatio tai alaorganisaatio, joka vastaa palvelupisteestä. Mikäli palvelupiste on käytössä useilla alaorganisaatioilla, valitse pelkkä organisaation päätaso.'
  },
  shortDescriptionLabel: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.ShortDescription.Title',
    defaultMessage: 'Lyhyt kuvaus'
  },
  shortDescriptionInfo: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.ShortDescription.Tooltip',
    defaultMessage: 'Kirjoita lyhyt kuvaus eli tiivistelmä palvelupisteestä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään palvelun.'
  },
  shortDescriptionPlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.ShortDescription.Placeholder',
    defaultMessage: 'Kirjoita lyhyt tiivistelmä hakukoneita varten.'
  },
  descriptionLabel: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  descriptionInfo: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Description.Tooltip',
    defaultMessage: 'Kuvaa palvelupiste mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kuvaa yleisesti, mitä asioita palvelupisteessä voi hoitaa. Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa vain palvelupistettä, älä palvelua järjestävää organisaatiota tai sen tehtäviä!'
  },
  descriptionPlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Description.Placeholder',
    defaultMessage: 'Kirjoita selkeä ja ymmärrettävä kuvausteksti.'
  }
})

export const formIdentifierMessages = defineMessages({
    // User support general
  title: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.FormIdentifier.Title',
    defaultMessage: 'Lomaketunnus'
  },
  tooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.FormIdentifier.Tooltip',
    defaultMessage: 'Lomaketunnus'
  },
  placeholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.FormIdentifier.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  }
})

export const emailMessages = defineMessages({
  title: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  tooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Email.Tooltip',
    defaultMessage: 'Mikäli verkkoasioinnin käyttöön on mahdollista saada tukea sähköpostitse, kirjoita kenttään tukipalvelun sähköpostiosoite. Älä kirjoita kenttään organisaation yleisiä sähköpostiosoitteita, esim. kirjaamoon. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi.'
  },
  placeholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Email.Placeholder',
    defaultMessage: 'esim. palvelupiste@organisaatio.fi'
  }
})

export const phoneNumberMessages = defineMessages({
  title: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneNumber.Title',
    defaultMessage: 'Puhelinnumero'
  },
  tooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneNumber.Tooltip',
    defaultMessage: 'Mikäli verkkoasioinnin käyttöön on mahdollista saada tukea puhelimitse, kirjoita kenttään tukipalvelun puhelinnumero. Kirjoita puhelinnumero kansainvälisessä muodossa ilman välilyöntejä (esim. +358451234567). Älä kirjoita kenttään organisaation yleisiä puhelinnumeroita, esim. vaihteen numeroa. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi.'
  },
  placeholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneNumber.PlaceHolder',
    defaultMessage: 'esim. +35845123467'
  },
  chargeTypeTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneCost.Title',
    defaultMessage: 'Puhelun maksullisuus'
  },
  chargeTypeTooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneCost.Tootltip',
    defaultMessage: 'Ensimmäinen vaihtoehto tarkoittaa, että puhelu maksaa lankaliittymästä soitettaessa paikallisverkkomaksun (pvm), matkapuhelimesta soitettaessa matkapuhelinmaksun (mpm) tai ulkomailta soitettaessa ulkomaanpuhelumaksun. Valitse "täysin maksuton" vain, jos puhelu on soittajalle kokonaan maksuton. Jos puhelun maksu muodostuu muulla tavoin, valitse "muu maksu" ja kuvaa puhelun hintatiedot omassa kentässään.'
  },
  phoneCostAllCosts: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneAllCosts.Title',
    defaultMessage: 'Paikallisverkkomaksu (pvm), Matkapuhelinmaksu (mpm), Ulkomaanpuhelumaksu'
  },
  phoneCostFree: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneCostFree.Title',
    defaultMessage: 'Täysin maksuton'
  },
  phoneCostOther: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneCostOther.Title',
    defaultMessage: 'Muu maksu, anna tarkemmat tiedot:'
  },
  costDescriptionTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneCostOtherDescription.Title',
    defaultMessage: 'Puhelun hintatiedot'
  },
  costDescriptionPlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneCostOtherDescription.Placeholder',
    defaultMessage: 'esim. Pvm:n lisäksi jonotuksesta veloitetaan...'
  },
  costDescriptionTooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneCostOtherDescription.Tooltip',
    defaultMessage: 'Kerro sanallisesti, mitä puhelinnumeroon soittaminen maksaa.'
  },
  infoTitle:{
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.AdditionalInformation.Title',
    defaultMessage: 'Lisätieto'
  },
  infoTooltip:{
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.AdditionalInformation.Tooltip',
    defaultMessage: 'Voit tarvittaessa antaa puhelinnumerolle lisätiedon, jolla voit tarkentaa, mistä numerosta on kyse. Esimerkiksi vaihde tai asiakaspalvelu.'
  },
  infoPlaceholder:{
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.AdditionalInformation.Placeholder',
    defaultMessage:'esim. Vaihde'
  },
  prefixTitle:{
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhonePrefix.Title',
    defaultMessage:'Maan suuntanumero'
  },
  prefixPlaceHolder:{
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhonePrefix.Placeholder',
    defaultMessage:'esim. +358'
  },
  prefixTooltip:{
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhonePrefix.Tooltip',
    defaultMessage:'esim. +358'
  },
  finnishServiceNumberName: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.FinishServiceNumber.Name',
    defaultMessage: 'Suomalainen palvelunumero'
  }
})

export const webPageMessages = defineMessages({
  title: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Section.Title',
    defaultMessage: 'Lisää verkkosivu'
  },
  nameTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Name.Title',
    defaultMessage: 'Verkkosivun nimi'
  },
  nameTooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Name.Tooltip',
    defaultMessage: 'Anna palvelupisteen verkkosivuille havainnollinen nimi.'
  },
  namePlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Name.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  typeTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Type.Title',
    defaultMessage: 'Tiedostomuoto'
  },
  typeTooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Type.Tooltip',
    defaultMessage: 'Valitse alasvetovalikosta lomakkeen tiedostomuoto. Jos lomakkeesta on tarjolla useampi eri tiedostomuoto, vie kukin tiedostomuoto erikseen.'
  },
  urlLabel: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Url.Title',
    defaultMessage: 'Verkko-osoite'
  },
  urlTooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Url.Tooltip',
    defaultMessage: 'Syötä verkko-osoite, josta verkkoasiointikanava löytyy. Anna mahdollisimman tarkka verkko-osoite, josta verkkoasiointi avautuu, älä esimerkiksi organisaatiosi verkkoasioinnin etusivua. Kopioi ja liitä verkko-osoite ja  tarkista verkko-osoitteen toimivuus Testaa osoite -painikkeesta.'
  },
  urlPlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Url.Placeholder',
    defaultMessage: 'Kopioi ja liitä tarkka verkko-osoite.'
  },
  urlButton: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Url.Button.Title',
    defaultMessage: 'Testaa osoite'
  },
  urlCheckerInfo: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Url.Icon.Tooltip',
    defaultMessage: 'Verkko-osoitetta ei löytynyt, tarkista sen muoto.'
  }
})

export const areaInformationMessages = defineMessages({
  areaInformationTitle: {
    id: 'Containers.Channels.PrintableFormChannel.InformationArea.Title',
    defaultMessage: 'Alue, joilla tulostettava lomake on käytettävissä'
  },
  areaInformationTooltip: {
    id: 'Containers.Channels.PrintableFormChannel.InformationArea.Tooltip',
    defaultMessage: 'Alue, joilla tulostettava lomake on käytettävissä'
  }
})
