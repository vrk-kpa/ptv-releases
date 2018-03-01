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

export const serviceNamesMessages = defineMessages({
  nameTitle: {
    id: 'Containers.Services.AddService.Step1.Name.Title',
    defaultMessage: 'Nimi'
  },
  namePlaceholder: {
    id: 'Containers.Services.AddService.Step1.Name.Placeholder',
    defaultMessage: 'Kirjoita palvelua kuvaava, asiakaslähtöinen nimi.'
  },
  nameTooltip: {
    id: 'Containers.Services.AddService.Step1.Name.Tooltip',
    defaultMessage: 'Kirjoita palvelun nimi mahdollisimman kuvaavasti. Älä kirjoita organisaation nimeä palvelun nimeen. Jos olet käyttänyt palvelun pohjakuvausta, muokkaa nimeä vain jos se on ehdottoman välttämätöntä!'
  },
  alternateNameTitle: {
    id: 'Containers.Services.AddService.Step1.AlternateName.Title',
    defaultMessage: 'Vaihtoehtoinen nimi'
  },
  alternateNamePlaceholder: {
    id: 'Containers.Services.AddService.Step1.AlternateName.Placeholder',
    defaultMessage: 'Kirjoita palvelulle tarvittaessa muu nimi.'
  },
  alternateNameTooltip: {
    id: 'Containers.Services.AddService.Step1.AlternateName.Tooltip',
    defaultMessage: 'Kirjoita palvelulle tarvittaessa muu nimi, jolla käyttäjä saattaa hakea palvelua.'
  },
  mainOrganisationTitle : {
    id : 'Containers.Services.AddService.Step1.MainOrganization.Title',
    defaultMessage: 'Organisaatio'
  },
  responsibleOrganisationTitle : {
    id : 'Containers.Services.AddService.Step1.OtherOrganization.Title',
    defaultMessage: 'Palvelun vastuuorganisaatio'
  }
})

export const serviceDescriptionMessages = defineMessages({
  shortDescriptionTitle: {
    id: 'Containers.Services.AddService.Step1.ShortDescription.Title',
    defaultMessage: 'Lyhyt kuvaus'
  },
  shortDescriptionPlaceholder: {
    id: 'Containers.Services.AddService.Step1.ShortDescription.Placeholder',
    defaultMessage: 'Kirjoita lyhyt tiivistelmä hakukoneita varten.'
  },
  shortDescriptionTooltip: {
    id: 'Containers.Services.AddService.Step1.ShortDescription.Tooltip',
    defaultMessage: 'Kirjoita lyhyt kuvaus eli tiivistelmä palvelun keskeisestä sisällöstä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään palvelun.'
  },
  descriptionTitle: {
    id: 'Containers.Services.AddService.Step1.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  descriptionPlaceholder: {
    id: 'Containers.Services.AddService.Step1.Description.PlaceHolder',
    defaultMessage: 'Kirjoita palvelulle selkeä ja ymmärrettävä kuvaus.'
  },
  descriptionTooltip: {
    id: 'Containers.Services.AddService.Step1.Description.Tooltip',
    defaultMessage: 'Kuvaa palvelu mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kerro, mitä palvelu pitää sisällään, miten sitä tarjotaan asiakkaalle, mihin tarpeeseen se vastaa ja mihin sillä pyritään. Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa asiakkaalle tarjottavaa palvelua, älä palvelua järjestävää organisaatiota tai sen tehtäviä! Voit jakaa tekstiä kappaleisiin ja tarvittaessa käyttää luettelomerkkejä. Jos olet liittänyt palveluun pohjakuvauksen, käytä kuvauskenttä sen kertomiseen, miten pohjakuvauksessa kuvattu palvelu on sinun organisaatiossasi/seudullasi/kunnassasi järjestetty ja mitä erityispiirteitä tällä palvelulla on. Älä toista pohjakuvauksessa jo kerrottuja asioita. '
  },
  conditionOfServiceUsageTitle: {
    id: 'Containers.Services.AddService.Step1.ConditionOfServiceUsage.Title',
    defaultMessage: 'Palvelun käytön edellytykset'
  },
  serviceUserInstructionTitle: {
    id: 'Containers.Services.AddService.Step1.ServiceUserInstruction.Title',
    defaultMessage: 'Toimintaohjeet'
  },
  conditionOfServiceUsagePermissionPlaceholder: {
    id: 'Containers.Services.AddService.Step1.ConditionOfServiceUsage.Permission.Placeholder',
    defaultMessage: 'Kuvaa lyhyesti, jos luvan saamiseen liittyy ehtoja tai edellytyksiä, kirjoita ne tähän kenttään.'
  },
  conditionOfServiceUsageNoticePlaceholder: {
    id: 'Containers.Services.AddService.Step1.ConditionOfServiceUsage.Notice.Placeholder',
    defaultMessage: 'Kuvaa lyhyesti, jos ilmoituksen tekemiseen liittyy ehtoja tai edellytyksiä, kirjoita ne tähän kenttään.'
  },
  conditionOfServiceUsageRegistrationPlaceholder: {
    id: 'Containers.Services.AddService.Step1.ConditionOfServiceUsage.Registration.Placeholder',
    defaultMessage: 'Kuvaa lyhyesti, jos rekisteröinnin tekemiseen liittyy ehtoja tai edellytyksiä, kirjoita ne tähän kenttään.'
  },
  additionalDescription: {
    id: 'Containers.Services.ViewService.Step1.AdditionalDescription.Title',
    defaultMessage: 'Lisäkuvaus'
  },
  optionConnectedDescriptionTitle: {
    id: 'Containers.Services.AddService.Step1.ConnectGeneralDescription.ConnectedDescription.Title',
    defaultMessage: 'Liitetty pohjakuvaus'
  },
  backgroundAreaTitle: {
    id: 'Containers.Services.AddService.Step1.Laws.Area.BackgroundDescription.Title',
    defaultMessage: 'Tausta ja lainsäädäntö'
  },
  backgroundDescriptionTitle: {
    id: 'Containers.Services.AddService.Step1.Laws.BackgroundDescription.Title',
    defaultMessage: 'Taustakuvaus'
  },
  lawsTitle: {
    id: 'Containers.Services.AddService.Step1.Laws.Title',
    defaultMessage: 'Linkki lakitietoihin'
  },
  lawNameTitle: {
    id: 'Containers.Services.AddService.Step1.Laws.Name.Title',
    defaultMessage: 'Nimi'
  },
  lawUrlLabel: {
    id: 'Containers.Services.AddService.Step1.Laws.Url.Title',
    defaultMessage: 'Verkko-osoite'
  },
  languageVersionNotAvailable: {
    id: 'Common.LanguageVersion.NotAvailable',
    defaultMessage: 'Language version not available'
  },
  dataNotAvailable: {
    id: 'Common.Data.NotAvailable',
    defaultMessage: 'Data not available'
  }
})

export const typeProfessionalAdditionalInfoMessagesServices = defineMessages({
  obligationTitle:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.Title',
    defaultMessage: 'Lupaan tai velvoitteeseen liittyvät lisätiedot'
  },
  qualificationTitle:{
    id: 'Containers.GeneralDescription.ServiceType.ProfessionalAdditionalInfo.Title',
    defaultMessage: 'Ammattipätevyyteen liittyvät lisätiedot'
  },
  deadlineTitle:{
    id: 'Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.DeadLine.Title',
    defaultMessage: 'Määräaika'
  },
  deadlinePlaceholder:{
    id: 'Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.DeadLine.Placeholder',
    defaultMessage: 'Kirjoita määräaikaan liittyvät tiedot.'
  },
  deadlineTootltip:{
    id: 'Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.DeadLine.Tooltip',
    defaultMessage: 'Kuvaa lyhyesti, jos lupaa on haettava / ilmoitus tai rekisteröinti on tehtävä tiettyyn määräaikaan mennessä. Esim. Ilmoitus on tehtävä viimeistään neljä (4) viikkoa ennen toiminnan aloittamista tai olennaista muuttamista.'
  },
  qualificationDeadlineTitle:{
    id: 'Containers.Services.AddService.Step1.ServiceType.ProfessionalAdditionalInfo.DeadLine.Title',
    defaultMessage: 'Määräaika'
  },
  qualificationDeadlinePlaceholder:{
    id: 'Containers.Services.AddService.Step1.ServiceType.ProfessionalAdditionalInfo.DeadLine.Placeholder',
    defaultMessage: 'Kirjoita määräaikaan liittyvät tiedot.'
  },
  qualificationDeadlineTootltip:{
    id: 'Containers.Services.AddService.Step1.ServiceType.ProfessionalAdditionalInfo.DeadLine.Tooltip',
    defaultMessage: 'Kuvaa lyhyesti, jos lupaa on haettava / ilmoitus tai rekisteröinti on tehtävä tiettyyn määräaikaan mennessä. Esim. Ilmoitus on tehtävä viimeistään neljä (4) viikkoa ennen toiminnan aloittamista tai olennaista muuttamista.'
  },
  processingTimeTitle:{
    id: 'Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.ProcessingTime.Title',
    defaultMessage: 'Käsittelyaika'
  },
  processingTimePlaceholder:{
    id: 'Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.ProcessingTime.Placeholder',
    defaultMessage: 'Kirjoita käsittelyaikaan liittyvät tiedot.'
  },
  processingTimeTootltip:{
    id: 'Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.ProcessingTime.Tootltip',
    defaultMessage: 'Kuvaa lyhyesti, miten kauan asian käsittely viranomaistasolla kestää. Esim. Lupa käsitellään kuuden (6) kuukauden kuluessa hakemuksen vastaanottamisesta tai, jos hakemus on puutteellinen, siitä kun hakija on antanut asian ratkaisemista varten tarvittavat asiakirjat ja selvitykset.'
  },
  qualificationProcessingTimeTitle:{
    id: 'Containers.Services.AddService.Step1.ServiceType.ProfessionalAdditionalInfo.ProcessingTime.Title',
    defaultMessage: 'Käsittelyaika'
  },
  qualificationProcessingTimePlaceholder:{
    id: 'Containers.Services.AddService.Step1.ServiceType.ProfessionalAdditionalInfo.ProcessingTime.Placeholder',
    defaultMessage: 'Kirjoita käsittelyaikaan liittyvät tiedot.'
  },
  qualificationProcessingTimeTootltip:{
    id: 'Containers.Services.AddService.Step1.ServiceType.ProfessionalAdditionalInfo.ProcessingTime.Tootltip',
    defaultMessage: 'Kuvaa lyhyesti, miten kauan asian käsittely viranomaistasolla kestää. Esim. Lupa käsitellään kuuden (6) kuukauden kuluessa hakemuksen vastaanottamisesta tai, jos hakemus on puutteellinen, siitä kun hakija on antanut asian ratkaisemista varten tarvittavat asiakirjat ja selvitykset.'
  },
  validityTimeTitle:{
    id: 'Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.ValidityTime.Title',
    defaultMessage: 'Voimassaoloaika'
  },
  validityTimePlaceholder:{
    id: 'Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.ValidityTime.Placeholder',
    defaultMessage: 'Kirjoita voimassaoloaikaan liittyvät tiedot.'
  },
  validityTimeTootltip:{
    id: 'Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.ValidityTime.Tootltip',
    defaultMessage: 'Kuvaa lyhyesti tieto siitä, miten kauan lupa/ilmoitus/rekisteröinti on voimassa. Esim. Lupa on voimassa kolme (3) vuotta. / Lupa on voimassa toistaiseksi.'
  },
  qualificationValidityTimeTitle:{
    id: 'Containers.Services.AddService.Step1.ServiceType.ProfessionalAdditionalInfo.ValidityTime.Title',
    defaultMessage: 'Voimassaoloaika'
  },
  qualificationValidityTimePlaceholder:{
    id: 'Containers.Services.AddService.Step1.ServiceType.ProfessionalAdditionalInfo.ValidityTime.Placeholder',
    defaultMessage: 'Kirjoita voimassaoloaikaan liittyvät tiedot.'
  },
  qualificationValidityTimeTootltip:{
    id: 'Containers.Services.AddService.Step1.ServiceType.ProfessionalAdditionalInfo.ValidityTime.Tootltip',
    defaultMessage: 'Kuvaa lyhyesti tieto siitä, miten kauan lupa/ilmoitus/rekisteröinti on voimassa. Esim. Lupa on voimassa kolme (3) vuotta. / Lupa on voimassa toistaiseksi.'
  }
})

export const typeProfessionalAdditionalInfoMessages = defineMessages({
  obligationTitle:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.Title',
    defaultMessage: 'Lupaan tai velvoitteeseen liittyvät lisätiedot'
  },
  qualificationTitle:{
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
