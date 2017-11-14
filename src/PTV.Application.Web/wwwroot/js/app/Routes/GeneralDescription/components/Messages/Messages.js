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

export const generalDescriptionNamesMessages = defineMessages({
  nameTitle: {
    id: 'Containers.GeneralDescription.Name.Title',
    defaultMessage: 'Nimi'
  },
  namePlaceholder: {
    id: 'Containers.GeneralDescription.Name.Placeholder',
    defaultMessage: 'Kirjoita palvelulle nimi.'
  },
  nameTooltip: {
    id: 'Containers.GeneralDescription.Name.Tooltip',
    defaultMessage: 'Not defined'
  }
})

export const generalDescriptionDescriptionMessages = defineMessages({
  shortDescriptionTitle: {
    id: 'Containers.GeneralDescription.ShortDescription.Title',
    defaultMessage: 'Tiivistelmä'
  },
  shortDescriptionPlaceholder: {
    id: 'Containers.GeneralDescription.ShortDescription.Placeholder',
    defaultMessage: 'Kirjoita lyhyt tiivistelmä hakukoneita varten.'
  },
  shortDescriptionTooltip: {
    id: 'Containers.GeneralDescription.ShortDescription.Tooltip',
    defaultMessage: 'Kirjoita lyhyt kuvaus eli tiivistelmä palvelun keskeisestä sisällöstä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään palvelun.'
  },
  descriptionTitle: {
    id: 'Containers.GeneralDescription.Description.Title',
    defaultMessage: 'Pääkuvaus'
  },
  descriptionPlaceholder: {
    id: 'Containers.GeneralDescription.Description.PlaceHolder',
    defaultMessage: 'Kirjoita palvelulle selkeä ja ymmärrettävä kuvaus.'
  },
  descriptionTooltip: {
    id: 'Containers.GeneralDescription.Description.Tooltip',
    defaultMessage: 'Kuvaa palvelu mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kerro, mitä palvelu pitää sisällään, miten sitä tarjotaan asiakkaalle, mihin tarpeeseen se vastaa ja mihin sillä pyritään. Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa asiakkaalle tarjottavaa palvelua, älä palvelua järjestävää organisaatiota tai sen tehtäviä! Voit jakaa tekstiä kappaleisiin ja tarvittaessa käyttää luettelomerkkejä. Jos olet liittänyt palveluun pohjakuvauksen, käytä kuvauskenttä sen kertomiseen, miten pohjakuvauksessa kuvattu palvelu on sinun organisaatiossasi/seudullasi/kunnassasi järjestetty ja mitä erityispiirteitä tällä palvelulla on. Älä toista pohjakuvauksessa jo kerrottuja asioita. '
  },
  conditionOfServiceUsageTitle: {
    id: 'Containers.GeneralDescription.ConditionOfServiceUsage.Title',
    defaultMessage: 'Ehdot ja kriteerit'
  },
  conditionOfServiceUsageServicePlaceholder: {
    id: 'Containers.GeneralDescription.ConditionOfServiceUsage.Service.Placeholder',
    defaultMessage: 'Kuvaa lyhyesti, jos palvelun käyttöön liittyy ehtoja tai edellytyksiä, kirjoita ne tähän kenttään.'
  },
  conditionOfServiceUsagePermissionPlaceholder: {
    id: 'Containers.GeneralDescription.ConditionOfServiceUsage.Permission.Placeholder',
    defaultMessage: 'Kuvaa lyhyesti, jos luvan saamiseen liittyy ehtoja tai edellytyksiä, kirjoita ne tähän kenttään.'
  },
  conditionOfServiceUsageTooltip: {
    id: 'Containers.GeneralDescription.ConditionOfServiceUsage.Tooltip',
    defaultMessage: 'Mikäli käyttäjä saa palvelua vain tietyin edellytyksin, kuvaa sanallisesti nämä edellytykset, ehdot ja kriteerit. Esim. "Palvelun piiriin kuuluvat vain työttömyysuhan alla olevat henkilöt". Myös mikäli palvelun onnistunut käyttö edellyttää esim. tiettyjen tietojen tai dokumenttien hankkimista, kuvaa se tähän: Esim. "Muutosverokorttia varten tarvitset tiedot vuoden alusta kertyneistä tuloista ja maksamistasi veroista, sekä arvion koko vuoden tuloista."'
  },
  serviceUserInstructionTitle: {
    id: 'Containers.GeneralDescription.ServiceUserInstruction.Title',
    defaultMessage: 'Toimintaohjeet'
  },
  serviceUserInstructionPlaceholder: {
    id: 'Containers.GeneralDescription.ServiceUserInstruction.Placeholder',
    defaultMessage: 'Mikäli palvelun saamiseksi on toimittava tietyllä tavalla, kirjoita ohjeistus tähän kenttään.'
  },
  serviceUserInstructionTooltip: {
    id: 'Containers.GeneralDescription.ServiceUserInstruction.Tooltip',
    defaultMessage: 'Jos palvelun käyttö tai etuuden hakeminen edellyttää tiettyä toimintajärjestystä tai tietynlaista toimintatapaa, kuvaa se tähän mahdollisimman tarkasti. Esimerkiksi jos jokin asiointikanava on ensisijainen, kerro se tässä. "Käytä ilmoittautumiseen ensisijaisesti verkkoasiointia."'
  },
  errorMessageDescriptionIsRequired: {
    id: 'Containers.GeneralDescription.Description..ValidationError.IsRequired',
    defaultMessage: 'Pääkuvaus tai taustakuvaus. Toinen tieto on pakollinen, täytä ainakin toisen kentän tiedot.'
  },
  generalDescriptionStep2Title: {
    id: 'Containers.GeneralDescription.Step2.Title',
    defaultMessage: 'Luokittelu ja asiasanat'
  }
})

export const typeProfessionalAdditionalInfoMessages = defineMessages({
  title:{
    id: 'Containers.GeneralDescription.ServiceType.ProfessionalAdditionalInfo.Title',
    defaultMessage: 'Ammattipätevyyteen liittyvät lisätiedot'
  },
  deadlineTitle:{
    id: 'Containers.GeneralDescription.ServiceType.ProfessionalAdditionalInfo.DeadLine.Title',
    defaultMessage: 'Määräaika'
  },
  deadlinePlaceholder:{
    id: 'Containers.GeneralDescription.ServiceType.ProfessionalAdditionalInfo.DeadLine.Placeholder',
    defaultMessage: 'Kirjoita määräaikaan liittyvät tiedot.'
  },
  deadlineTootltip:{
    id: 'Containers.GeneralDescription.ServiceType.ProfessionalAdditionalInfo.DeadLine.Tooltip',
    defaultMessage: 'Kuvaa lyhyesti, jos lupaa on haettava / ilmoitus tai rekisteröinti on tehtävä tiettyyn määräaikaan mennessä. Esim. Ilmoitus on tehtävä viimeistään neljä (4) viikkoa ennen toiminnan aloittamista tai olennaista muuttamista.'
  },
  processingTimeTitle:{
    id: 'Containers.GeneralDescription.ServiceType.ProfessionalAdditionalInfo.ProcessingTime.Title',
    defaultMessage: 'Käsittelyaika'
  },
  processingTimePlaceholder:{
    id: 'Containers.GeneralDescription.ServiceType.ProfessionalAdditionalInfo.ProcessingTime.Placeholder',
    defaultMessage: 'Kirjoita käsittelyaikaan liittyvät tiedot.'
  },
  processingTimeTootltip:{
    id: 'Containers.GeneralDescription.ServiceType.ProfessionalAdditionalInfo.ProcessingTime.Tootltip',
    defaultMessage: 'Kuvaa lyhyesti, miten kauan asian käsittely viranomaistasolla kestää. Esim. Lupa käsitellään kuuden (6) kuukauden kuluessa hakemuksen vastaanottamisesta tai, jos hakemus on puutteellinen, siitä kun hakija on antanut asian ratkaisemista varten tarvittavat asiakirjat ja selvitykset.'
  },
  validityTimeTitle:{
    id: 'Containers.GeneralDescription.ServiceType.ProfessionalAdditionalInfo.ValidityTime.Title',
    defaultMessage: 'Voimassaoloaika'
  },
  validityTimePlaceholder:{
    id: 'Containers.GeneralDescription.ServiceType.ProfessionalAdditionalInfo.ValidityTime.Placeholder',
    defaultMessage: 'Kirjoita voimassaoloaikaan liittyvät tiedot.'
  },
  validityTimeTootltip:{
    id: 'Containers.GeneralDescription.ServiceType.ProfessionalAdditionalInfo.ValidityTime.Tootltip',
    defaultMessage: 'Kuvaa lyhyesti tieto siitä, miten kauan lupa/ilmoitus/rekisteröinti on voimassa. Esim. Lupa on voimassa kolme (3) vuotta. / Lupa on voimassa toistaiseksi.'
  }
})
