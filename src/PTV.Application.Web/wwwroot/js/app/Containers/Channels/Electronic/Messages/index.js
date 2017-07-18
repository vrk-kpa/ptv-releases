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
    id: 'Containers.Channels.AddElectronicChannel.Header.Title',
    defaultMessage: 'Lisää uusi kanava: verkkoasiointi'
  },
  mainTitleView: {
    id: 'Containers.Channels.AddElectronicChannel.Header.Title.View',
    defaultMessage: 'Lisää uusi kanava: verkkoasiointi'
  },
  mainText: {
    id: 'Containers.Channels.AddElectronicChannel.Header.Description',
    defaultMessage: 'Tällä sivulla voit lisätä uuden verkkoasiointikanavan. Verkkoasioinnilla tarkoitetaan asiointikanavaa, jossa asiointi tapahtuu sähköisesti.'
  },
  mainTextView: {
    id: 'Containers.Channels.ViewElectronicChannel.Header.Description',
    defaultMessage: 'Tällä sivulla voit katsella, muokata ja julkaista asiointikanavan tietoja.'
  },
  subTitle1: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Header.Title',
    defaultMessage: 'Vaihe 1/2: Perustiedot'
  },
  subTitle1View: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Header.Title.View',
    defaultMessage: 'Vaihe 1/2: Perustiedot'
  },
  subTitle2: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.Header.Title',
    defaultMessage: 'Vaihe 2/2: Aukioloajat'
  },
  subTitle2View: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.Header.Title.View',
    defaultMessage: 'Vaihe 2/2: Aukioloajat'
  },
  authenticationLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Authentication.Title',
    defaultMessage: 'Tunnistautumisen tiedot'
  },
  supportTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Support.Title',
    defaultMessage: 'Käytön tuki'
  }
})

export const deleteMessages = defineMessages({
  text: {
    id: 'Containers.Channels.ElectronicChannel.Delete.Text',
    defaultMessage: 'Oletko varma, että haluat poistaa kanavakuvauksen?'
  },
  buttonOk: {
    id: 'Containers.Channels.ElectronicChannel.Delete.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Channels.ElectronicChannel.Delete.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const withdrawMessages = defineMessages({
  text: {
    id: 'Containers.Channels.ElectronicChannel.Withdraw.Text',
    defaultMessage: 'Are you sure, you want to withdraw electronic channel?'
  },
  buttonOk: {
    id: 'Containers.Channels.ElectronicChannel.Withdraw.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Channels.ElectronicChannel.Withdraw.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const restoreMessages = defineMessages({
  text: {
    id: 'Containers.Channels.ElectronicChannel.Restore.Text',
    defaultMessage: 'Are you sure, you want to restore electronic channel?'
  },
  buttonOk: {
    id: 'Containers.Channels.ElectronicChannel.Restore.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Channels.ElectronicChannel.Restore.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const authenticationListMessages = defineMessages({
  authenticationListLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AuthenticationList.Title',
    defaultMessage: 'Vaatiiko verkkoasiointi tunnistautumisen'
  },
  authenticationListInfo: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AuthenticationList.Tooltip',
    defaultMessage: 'Valitse alasvetovalikosta kyllä tai ei sen mukaan, vaatiiko verkkoasiointi tunnistautumisen vai ei. Tunnistautumisella tarkoitetaan mitä tahansa tapaa, jolla käyttäjä joutuu kirjautumaan tai tunnistautumaan verkkoasioinnissa.'
  }
})

export const authenticationSignMessages = defineMessages({
  authenticationSignLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AuthenticationSign.Title',
    defaultMessage: 'Vaatiiko verkkoasiointi sähköisen allekirjoituksen'
  },
  authenticationSignInfo: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AuthenticationSign.Tooltip',
    defaultMessage: 'Osa verkkoasioinneista pitää allekirjoittaa sähköisesti ennen lähettämistä. Valitse kyllä tai ei sen mukaan, vaatiiko verkkoasiointi sähköisen allekirjoituksen. Jos verkkoasiointi vaatii sähköisen allekirjoituksen, valitse alasvetovalikosta vaadittavien sähköisten allekirjoitusten lukumäärä. Useampi allekirjoitus vaaditaan joissakin asioinneissa, joissa useamman henkilön tulee vakuuttaa tiedot oikeiksi.'
  },
  authenticationNumberSignLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AuthenticationNumberSign.Title',
    defaultMessage: 'Allekirjoitusten lukumäärä'
  }
})

export const cancelMessages = defineMessages({
  text: {
    id: 'Containers.Channels.ElectronicChannel.Cancel.Text',
    defaultMessage: 'Oletko varma, että haluat keskeyttää uuden kanavan luonnin? Tekemäsi muutokset katoavat.'
  },
  buttonOk: {
    id: 'Containers.Channels.ElectronicChannel.Cancel.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Channels.ElectronicChannel.Cancel.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const urlMessages = defineMessages({
  label: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.UrlChecker.Title',
    defaultMessage: 'Verkko-osoite'
  },
  tooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.UrlChecker.Tooltip',
    defaultMessage: 'Syötä verkko-osoite, josta verkkoasiointikanava löytyy. Anna mahdollisimman tarkka verkko-osoite, josta verkkoasiointi avautuu, älä esimerkiksi organisaatiosi verkkoasioinnin etusivua. Kopioi ja liitä verkko-osoite ja  tarkista verkko-osoitteen toimivuus Testaa osoite -painikkeesta.'
  },
  placeholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.UrlChecker.Placeholder',
    defaultMessage: 'Kopioi ja liitä tarkka verkko-osoite.'
  },
  button: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.UrlChecker.Button.Title',
    defaultMessage: 'Testaa osoite'
  },
  checkerInfo:  {
    id: 'Containers.Channels.AddElectronicChannel.Step1.UrlChecker.Icon.Tooltip',
    defaultMessage: 'Verkko-osoitetta ei löytynyt, tarkista sen muoto.'
  }
})

export const channelDescriptionMessages = defineMessages({
  nameTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Name.Title',
    defaultMessage: 'Nimi'
  },
  namePlaceholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Name.Placeholder',
    defaultMessage: 'Kirjoita asiointikanavaa kuvaava, asiakaslähtöinen nimi.'
  },
  organizationLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Organization.Title',
    defaultMessage: 'Organisaatio'
  },
  organizationInfo: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Organization.Tooltip',
    defaultMessage: 'Valitse alasvetovalikosta organisaatio tai alaorganisaatio, joka vastaa verkkoasiointikanavasta. Mikäli verkkoasiointikanava on käytössä useilla alaorganisaatioilla, valitse pelkkä organisaation päätaso.'
  },
  shortDescriptionLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.ShortDescription.Title',
    defaultMessage: 'Lyhyt kuvaus'
  },
  shortDescriptionInfo: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.ShortDescription.Tooltip',
    defaultMessage: 'Kirjoita lyhyt kuvaus eli tiivistelmä verkkoasiointikanavan keskeisestä sisällöstä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään asiointikanavan.'
  },
  shortDescriptionPlaceholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.ShortDescription.Placeholder',
    defaultMessage: 'Kirjoita lyhyt tiivistelmä hakukoneita varten.'
  },
  descriptionLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  descriptionInfo: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Description.Tooltip',
    defaultMessage: 'Kuvaa verkkoasiointikanava mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kerro, mitä verkkoasiointi pitää sisällään ja miten se toimii. Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa vain verkkoasiointikanavaa, älä siihen kytkettyä palvelua tai palvelua järjestävää organisaatiota tai sen tehtäviä!'
  },
  descriptionPlaceholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Description.Placeholder',
    defaultMessage: 'Kirjoita selkeä ja ymmärrettävä kuvausteksti.'
  }
})

export const emailMessages = defineMessages({
  title: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  tooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Tooltip',
    defaultMessage: 'Mikäli verkkoasioinnin käyttöön on mahdollista saada tukea sähköpostitse, kirjoita kenttään tukipalvelun sähköpostiosoite. Älä kirjoita kenttään organisaation yleisiä sähköpostiosoitteita, esim. kirjaamoon. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi.'
  },
  placeholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Placeholder',
    defaultMessage: 'esim. palvelupiste@organisaatio.fi'
  }
})

export const phoneNumberMessages = defineMessages({
  title: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneNumber.Title',
    defaultMessage: 'Puhelinnumero'
  },
  tooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneNumber.Tooltip',
    defaultMessage: 'Mikäli verkkoasioinnin käyttöön on mahdollista saada tukea puhelimitse, kirjoita kenttään tukipalvelun puhelinnumero. Kirjoita puhelinnumero kansainvälisessä muodossa ilman välilyöntejä (esim. +358451234567). Älä kirjoita kenttään organisaation yleisiä puhelinnumeroita, esim. vaihteen numeroa. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi.'
  },
  placeholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneNumber.PlaceHolder',
    defaultMessage: 'esim. +35845123467'
  },
  chargeTypeTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCost.Title',
    defaultMessage: 'Puhelun maksullisuus'
  },
  chargeTypeTooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCost.Tootltip',
    defaultMessage: 'Ensimmäinen vaihtoehto tarkoittaa, että puhelu maksaa lankaliittymästä soitettaessa paikallisverkkomaksun (pvm), matkapuhelimesta soitettaessa matkapuhelinmaksun (mpm) tai ulkomailta soitettaessa ulkomaanpuhelumaksun. Valitse "täysin maksuton" vain, jos puhelu on soittajalle kokonaan maksuton. Jos puhelun maksu muodostuu muulla tavoin, valitse "muu maksu" ja kuvaa puhelun hintatiedot omassa kentässään.'
  },
  phoneCostAllCosts: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneAllCosts.Title',
    defaultMessage: 'Paikallisverkkomaksu (pvm), Matkapuhelinmaksu (mpm), Ulkomaanpuhelumaksu'
  },
  phoneCostFree: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCostFree.Title',
    defaultMessage: 'Täysin maksuton'
  },
  phoneCostOther: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCostOther.Title',
    defaultMessage: 'Muu maksu, anna tarkemmat tiedot:'
  },
  costDescriptionTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCostOtherDescription.Title',
    defaultMessage: 'Puhelun hintatiedot'
  },
  costDescriptionPlaceholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCostOtherDescription.Placeholder',
    defaultMessage: 'esim. Pvm:n lisäksi jonotuksesta veloitetaan...'
  },
  costDescriptionTooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCostOtherDescription.Tooltip',
    defaultMessage: 'Kerro sanallisesti, mitä puhelinnumeroon soittaminen maksaa.'
  },
  infoTitle:{
    id: 'Containers.Channels.AddElectronicChannel.Step1.AdditionalInformation.Title',
    defaultMessage: 'Lisätieto'
  },
  infoTooltip:{
    id: 'Containers.Channels.AddElectronicChannel.Step1.AdditionalInformation.Tooltip',
    defaultMessage: 'Voit tarvittaessa antaa puhelinnumerolle lisätiedon, jolla voit tarkentaa, mistä numerosta on kyse. Esimerkiksi vaihde tai asiakaspalvelu.'
  },
  infoPlaceholder:{
    id: 'Containers.Channels.AddElectronicChannel.Step1.AdditionalInformation.Placeholder',
    defaultMessage:'esim. Vaihde'
  },
  prefixTitle:{
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhonePrefix.Title',
    defaultMessage:'Maan suuntanumero'
  },
  prefixPlaceHolder:{
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhonePrefix.Placeholder',
    defaultMessage:'esim. +358'
  },
  prefixTooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhonePrefix.Tooltip',
    defaultMessage:'esim. +358'
  },
  finnishServiceNumberName: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.FinishServiceNumber.Name',
    defaultMessage: 'Suomalainen palvelunumero'
  }
})

export const urlAttachmentMessages = defineMessages({
  attachmentsLabel: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Attachment.Title',
    defaultMessage: 'Liitteet ja lisätietolinkit'
  },
  attachmentsInfo: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Attachment.Tooltip',
    defaultMessage: 'Voit antaa linkkejä verkkoasiointikanavaan liittyviin ohjedokumentteihin tai -sivuihin. Liitteen tulee sijaita ulkoisessa verkko-osoitteessa, palvelutietovarantoon ei voi ladata tai tallentaa liitetiedostoja. Kirjoita liitteelle selkeä, kuvaava nimi, tarvittaessa tarkempi kuvausteksti ja linkki liitteeseen.'
  },
  attachmentsButton: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Attachment.Button.AddAttachment.Title',
    defaultMessage: 'Lisää liite'
  },
  label: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AttachmentUrl.Title',
    defaultMessage: 'Verkko-osoite'
  },
  tooltip:  {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AttachmentUrl.Tooltip',
    defaultMessage: 'Anna mahdollisimman tarkka verkko-osoite, josta dokumentti tai verkkosivu avautuu. Kopioi ja liitä verkko-osoite ja tarkista verkko-osoitteen toimivuus Testaa osoite -painikkeesta.'
  },
  placeholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AttachmentUrl.Placeholder',
    defaultMessage: 'Kopioi ja liitä tarkka verkko-osoite.'
  },
  button: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AttachmentUrl.Button.Title',
    defaultMessage: 'Testaa osoite'
  },
  checkerInfo:  {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AttachmentUrl.Icon.Tooltip',
    defaultMessage: 'Verkko-osoitetta ei löytynyt, tarkista sen muoto.'
  }
})

export const openingHoursMessages = defineMessages({
  showOpeningHours: {
    id: 'Containers.Channels.Common.OpeningHours.ShowOpeningHours',
    defaultMessage: 'Aukioloajat'
  },
  collapsedInfo: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.CollapsedInfo',
    defaultMessage: 'Muokattu'
  },
  mainTooltipNormal: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.MainTooltipNormal',
    defaultMessage: 'This is a tooltip for NOH'
  },
  mainLabelNormal: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.MainLabelNormal',
    defaultMessage: 'Normaali aukioloaika'
  },
  defaultTitle_openingHoursNormal: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.DefaultTitleNormal',
    defaultMessage: 'Normaaliaukioloajat'
  },
  validOnward: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.ValidityType.Onward',
    defaultMessage: 'Toistaiseksi voimassa oleva'
  },
  validPeriod: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.ValidityType.Period',
    defaultMessage: 'Voimassa ajanjaksolla'
  },
  additionalInformation: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.AdditionalInformation.Title',
    defaultMessage: 'Lisätieto'
  },
  nonstopOpeningHours: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.Nonstop.Title',
    defaultMessage: 'Joka päivä ympäri vuorokauden käytettävissä'
  },
  startDate: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.StartDate.Title',
    defaultMessage: 'Alkaa'
  },
  endDate: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.EndDate.Title',
    defaultMessage: 'Päättyy'
  },
  mainTooltipSpecial: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.MainTooltipSpecial',
    defaultMessage: 'This is a tooltip for SOH'
  },
  mainLabelSpecial: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.MainLabelSpecial',
    defaultMessage: 'Vuorokauden yli menevät ajat'
  },
  defaultTitle_openingHoursSpecial: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.DefaultTitleSpecial',
    defaultMessage: 'Aukioloaikojen täyttämisohjeet'
  },
  previewTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.PreviewTitle',
    defaultMessage: 'Esikatselu'
  },
  previewTooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.PreviewTooltip',
    defaultMessage: 'This is a tooltip for Preview'
  },
  previewInstructions1: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.PreviewInstructions1',
    defaultMessage: 'Valitse viikonpäivät ja kellonajat, näet esikatselun lisättyäsi tietoa.'
  },
  previewInstructions2: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.PreviewInstructions2',
    defaultMessage: "Jos aukioloajalle on voimassaoloaika, valitse 'Voimassa ajanjaksolla'."
  },
  weekday_mo: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.Monday',
    defaultMessage: 'Maanantai'
  },
  weekday_tu: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.Tuesday',
    defaultMessage: 'Tiistai'
  },
  weekday_we: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.Wednesday',
    defaultMessage: 'Keskiviikko'
  },
  weekday_th: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.Thursday',
    defaultMessage: 'Torstai'
  },
  weekday_fr: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.Friday',
    defaultMessage: 'Perjantai'
  },
  weekday_sa: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.Saturday',
    defaultMessage: 'Lauantai'
  },
  weekday_su: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.Sunday',
    defaultMessage: 'Sunnuntai'
  },
  mainTooltipExceptional: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.MainTooltipExceptional',
    defaultMessage: 'This is a tooltip for EOH'
  },
  mainLabelExceptional: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.MainLabelExceptional',
    defaultMessage: 'Poikkeusaukioloaika'
  },
  defaultTitle_openingHoursExceptional: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.DefaultTitleExceptional',
    defaultMessage: 'Poikkeusaukioloajat'
  },
  validDaySingle: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.ValidityType.DaySingle',
    defaultMessage: 'Päivä'
  },
  validDayRange: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.ValidityType.DayRange',
    defaultMessage: 'Ajanjakso'
  },
  closedDaySingle: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.ValidityType.ClosedDaySingle',
    defaultMessage: 'Suljettu koko päivän'
  },
  closedDayRange: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.ValidityType.ClosedDayRange',
    defaultMessage: 'Suljettu koko ajanjakso'
  },
  closedMessage: {
    id: 'Containers.Channels.AddElectronicChannel.Step2.OpeningHours.ValidityType.ClosedMessage',
    defaultMessage: 'Suljettu'
  }
})

export const areaInformationMessages = defineMessages({
  areaInformationTitle: {
    id: 'Containers.Channels.ElectronicChannel.InformationArea.Title',
    defaultMessage: 'Alue, joilla verkkoasiointi on käytettävissä'
  },
  areaInformationTooltip: {
    id: 'Containers.Channels.ElectronicChannel.InformationArea.Tooltip',
    defaultMessage: 'Alue, joilla verkkoasiointi on käytettävissä'
  }
})
