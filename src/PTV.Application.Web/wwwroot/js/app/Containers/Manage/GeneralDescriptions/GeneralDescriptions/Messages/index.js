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
    id: 'Containers.GeneralDescription.Add.Header.Title',
    defaultMessage: 'Lisää pohjakuvaus'
  },
  mainText: {
    id: 'Containers.GeneralDescription.Add.Header.Description',
    defaultMessage: 'Tällä sivulla voit lisätä uuden pohjakuvauksen. Kuvaile pohjakuvausta mandollisimman selkeästi, kattavasti ja asiakasystävällisesti.'
  },
  subTitle1: {
    id: 'Containers.GeneralDescription.Add.Step1.Header.Title',
    defaultMessage: 'Perustiedot'
  },
  mainTitleView: {
    id: 'Containers.GeneralDescription.View.Header.Title',
    defaultMessage: 'Not defined'
  },
  mainTextView: {
    id: 'Containers.GeneralDescription.View.Header.Description',
    defaultMessage: 'Not defined'
  },
  subTitle1View: {
    id: 'Containers.GeneralDescription.View.Step1.Header.Title',
    defaultMessage: 'Perustiedot'
  }
})

export const namesMessages = defineMessages({
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

export const cancelMessages = defineMessages({
  text: {
    id: 'Containers.GeneralDescription.Cancel.Text',
    defaultMessage: 'Oletko varma, että haluat keskeyttää muoaamisen? Menetät kaikki tiedot.'
  },
  buttonOk: {
    id: 'Containers.GeneralDescription.Cancel.Accept',
    defaultMessage: 'Keskeytä'
  },
  buttonCancel: {
    id: 'Containers.GeneralDescription.Cancel.Cancel',
    defaultMessage: 'Peruuta'
  }
})

export const deleteMessages = defineMessages({
  text: {
    id: 'Containers.GeneralDescription.Delete.Text',
    defaultMessage: 'Oletko varma, että haluat poistaa liitoksen pohjakuvaukseen? Menetät pohjakuvauksesta tulevat kuvaustekstit. Luokittelutiedot, kuten kohderyhmä, palveluluokka ja asiasanat jäävät palvelulle.'
  },
  buttonOk: {
    id: 'Containers.GeneralDescription.Delete.Accept',
    defaultMessage: 'Poista pohjakuvaus'
  },
  buttonCancel: {
    id: 'Containers.GeneralDescription.Delete.Cancel',
    defaultMessage: 'Peruuta'
  }
})

export const withdrawMessages = defineMessages({
  text: {
    id: 'Containers.GeneralDescription.Withdraw.Text',
    defaultMessage: 'Are you sure, you want to withdraw service?'
  },
  buttonOk: {
    id: 'Containers.GeneralDescription.Withdraw.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.GeneralDescription.Withdraw.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const restoreMessages = defineMessages({
  text: {
    id: 'Containers.GeneralDescription.Restore.Text',
    defaultMessage: 'Are you sure, you want to restore service?'
  },
  buttonOk: {
    id: 'Containers.GeneralDescription.Restore.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.GeneralDescription.Restore.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const saveDraftMessages = defineMessages({
  text: {
    id: 'Containers.GeneralDescription.SaveDraft.Text',
    defaultMessage: 'Luonnos on tallennettu. Voit julkaista palvelun sivulla olevasta Julkaise painikkeesta tai jatkaa Palvelut -sivulle.'
  },
  buttonOk: {
    id: 'Containers.GeneralDescription.SaveDraft.Accept',
    defaultMessage: 'Ok'
  },
  buttonCancel: {
    id: 'Containers.GeneralDescription.SaveDraft.Cancel',
    defaultMessage: 'Palaa Palvelut -sivulle'
  }
})

export const goBackMessages = defineMessages({
  text: {
    id: 'Containers.GeneralDescription.GoBack.Text',
    defaultMessage: 'Do you want leave unsaved page.'
  },
  buttonOk: {
    id: 'Containers.GeneralDescription.GoBack.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.GeneralDescription.GoBack.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const serviceTypeMessages = defineMessages({
  title:{
    id: 'Containers.GeneralDescription.ServiceType.Title',
    defaultMessage: 'Palvelutyyppi'
  },
  tooltip:{
    id: 'Containers.GeneralDescription.ServiceType.Tooltip',
    defaultMessage: 'Palvelutyyppi'
  }
})

export const chargeTypeMessages = defineMessages({
  title: {
    id: 'Containers.GeneralDescription.ChargeType.Title',
    defaultMessage: 'Maksullisuuden tiedot'
  },
  tooltip: {
    id: 'Containers.GeneralDescription.ChargeType.Tooltip',
    defaultMessage: 'Missing'
  },
  additionalInfoTitle: {
    id: 'Containers.GeneralDescription.ChargeTypeAdditionalInfo.Title',
    defaultMessage: 'Maksullisuuden lisätieto'
  },
  additionalInfoPlaceholder: {
    id: 'Containers.GeneralDescription.ChargeTypeAdditionalInfo.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  chargeTypeTitle:{
    id: 'Containers.GeneralDescription.ChargeTypes.Title',
    defaultMessage: 'Maksullisuuden tyyppi'
  }
})


export const typeAdditionalInfoMessages = defineMessages({
  title:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.Title',
    defaultMessage: 'Lupaan tai velvoitteeseen liittyvät lisätiedot'
  },
  deadlineTitle:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.DeadLine.Title',
    defaultMessage: 'Määräaika'
  },
  deadlinePlaceholder:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.DeadLine.Placeholder',
    defaultMessage: 'Kirjoita määräaikaan liittyvät tiedot.'
  },
  deadlineTootltip:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.DeadLine.Tooltip',
    defaultMessage: 'Kuvaa lyhyesti, jos lupaa on haettava / ilmoitus tai rekisteröinti on tehtävä tiettyyn määräaikaan mennessä. Esim. Ilmoitus on tehtävä viimeistään neljä (4) viikkoa ennen toiminnan aloittamista tai olennaista muuttamista.'
  },
  processingTimeTitle:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.ProcessingTime.Title',
    defaultMessage: 'Käsittelyaika'
  },
  processingTimePlaceholder:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.ProcessingTime.Placeholder',
    defaultMessage: 'Kirjoita käsittelyaikaan liittyvät tiedot.'
  },
  processingTimeTootltip:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.ProcessingTime.Tootltip',
    defaultMessage: 'Kuvaa lyhyesti, miten kauan asian käsittely viranomaistasolla kestää. Esim. Lupa käsitellään kuuden (6) kuukauden kuluessa hakemuksen vastaanottamisesta tai, jos hakemus on puutteellinen, siitä kun hakija on antanut asian ratkaisemista varten tarvittavat asiakirjat ja selvitykset.'
  },
  validityTimeTitle:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.ValidityTime.Title',
    defaultMessage: 'Voimassaoloaika'
  },
  validityTimePlaceholder:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.ValidityTime.Placeholder',
    defaultMessage: 'Kirjoita voimassaoloaikaan liittyvät tiedot.'
  },
  validityTimeTootltip:{
    id: 'Containers.GeneralDescription.ServiceType.AdditionalInfo.ValidityTime.Tootltip',
    defaultMessage: 'Kuvaa lyhyesti tieto siitä, miten kauan lupa/ilmoitus/rekisteröinti on voimassa. Esim. Lupa on voimassa kolme (3) vuotta. / Lupa on voimassa toistaiseksi.'
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



export const descriptionMessages = defineMessages({
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

export const serviceClassMessages = defineMessages({
  title: {
    id: 'Containers.GeneralDescription.ServiceClass.Title',
    defaultMessage: 'Palveluluokka'
  },
  tooltip: {
    id: 'Containers.GeneralDescription.ServiceClass.Tooltip',
    defaultMessage: 'Palvelulle valitaan aihepiirin mukaan palveluluokka. Valitse vähintään yksi mahdollisimman tarkka palveluluokka. Mikäli olet käyttänyt palvelun pohjakuvausta, kenttään on kopioitu valmiiksi pohjakuvauksen palveluluokka/luokat. Voit tarvittaessa lisätä palveluluokkia.'
  },
  targetListHeader: {
    id: 'Containers.GeneralDescription.ServiceClass.TargetList.Header',
    defaultMessage: 'Valinnat({count})'
  },
  errorMessageIsRequired: {
    id: 'Containers.GeneralDescription.ServiceClass.ValidationError.IsRequired',
    defaultMessage: 'Valitse vähintään yksi palveluluokka.'
  }
})

export const ontologyTermMessages = defineMessages({
  title: {
    id: 'Containers.GeneralDescription.OntologyTerms.Title',
    defaultMessage: 'Asiasanat'
  },
  tooltip: {
    id: 'Containers.GeneralDescription.OntologyTerms.Tooltip',
    defaultMessage: 'Palvelun asiasisältö kuvataan ontologiakäsitteillä, joiden käyttö helpottaa palveluiden löytämistä. Kirjoita palvelun asiasisältöä mahdollisimman tarkasti kuvaava sana ja valitse ennakoivan tekstinsyötön tarjoamista vaihtoehdoista ontologiakäsite. Valitse palvelun kuvaamiseksi vähintään yksi ja enintään kymmenen käsitettä. Mikäli olet käyttänyt palvelun pohjakuvausta, kenttään on kopioitu valmiiksi pohjakuvauksen ontologiakäsitteet. Voit tarvittaessa lisätä uusia käsitteitä. '
  },
  errorMessageIsRequired: {
    id: 'Containers.GeneralDescription.OntologyTerms.ValidationError.IsRequired',
    defaultMessage: 'Valitse vähintään yksi ontologiakäsite.'
  },
  targetListHeader: {
    id: 'Containers.GeneralDescription.OntologyTerms.TargetList.Header',
    defaultMessage: 'Valinnat({count})'
  },
  annotationTitle:{
    id: 'Containers.GeneralDescription.OntologyTerms.AnnotationTitle',
    defaultMessage: 'Palvelu ehdotta asiasanoja täyttämäsi sisällön mukaan. Hae suositellut asiasanat automaattisesti painikkeella Hae asiasanat tai poimi haluamasi sanat asiasanalistasta.'
  },
  annotationButton:{
    id: 'Containers.GeneralDescription.OntologyTerms.AnnotationButton',
    defaultMessage: 'Hae asiasanat'
  },
  annotationToolAlert:{
    id: 'Containers.GeneralDescription.OntologyTerms.AnnotationToolAlert',
    defaultMessage: 'Annotation tool is disabled due to technical reasons'
  },
  treeLink:{
    id: 'Containers.GeneralDescription.OntologyTerms.TreeLink',
    defaultMessage: 'Hae ja lisää asiasanoja listasta'
  },
  annotationTagPostfix:{
    id: 'Containers.GeneralDescription.OntologyTerms.Annotation.TagPostfix',
    defaultMessage: 'Ehdotettu asiasana'
  },
  annotationTagTooltip:{
    id: 'Containers.GeneralDescription.OntologyTerms.Annotation.TagTooltip',
    defaultMessage: 'Asiasanan sijainti käsitehierarkiassa:'
  }

})

export const lifeEventMessages = defineMessages({
  title: {
    id: 'Containers.GeneralDescription.LifeEvent.Title',
    defaultMessage: 'Elämäntilanne'
  },
  tooltip: {
    id: 'Containers.GeneralDescription.LifeEvent.Tooltip',
    defaultMessage: 'Palvelu luokitellaan tarvittaessa elämäntilanteen mukaan. Jos palvelu ei suoraan liity tiettyyn elämäntilanteeseen, jätä valinta tekemättä. Mikäli olet käyttänyt palvelun pohjakuvausta, kenttään on kopioitu valmiiksi pohjakuvauksen mahdolliset elämäntilanteet. Voit tarvittaessa lisätä elämäntilanteita. '
  },
  targetListHeader: {
    id: 'Containers.GeneralDescription.LifeEvent.TargetList.Header',
    defaultMessage: 'Valinnat({count})'
  }

})

export const industrialClassMessages = defineMessages({
  title: {
    id: 'Containers.GeneralDescription.IndustrialClass.Title',
    defaultMessage: 'Toimialaluokka'
  },
  tooltip: {
    id: 'Containers.GeneralDescription.IndustrialClass.Tooltip',
    defaultMessage: 'Valitse toimiala(t), jota lupa/ilmoitus/rekisteröinti koskee.'
  },
  targetListHeader: {
    id: 'Containers.GeneralDescription.IndustrialClass.TargetList.Header',
    defaultMessage: 'Valinnat({count})'
  }
})


export const lawsMessages = defineMessages({
  title: {
    id: 'Containers.GeneralDescription.Laws.Title',
    defaultMessage: 'Linkki lakitietoihin'
  },
  nameTitle: {
    id: 'Containers.GeneralDescription.Laws.Name.Title',
    defaultMessage: 'Nimi'
  },
  nameTooltip: {
    id: 'Containers.GeneralDescription.Laws.Name.Tooltip',
    defaultMessage: 'Anna palvelupisteen verkkosivuille havainnollinen nimi.'
  },
  urlLabel: {
    id: 'Containers.GeneralDescription.Laws.Url.Title',
    defaultMessage: 'Verkko-osoite'
  },
  urlTooltip: {
    id: 'Containers.GeneralDescription.Laws.Url.Tooltip',
    defaultMessage: 'Syötä verkko-osoite, josta verkkoasiointikanava löytyy. Anna mahdollisimman tarkka verkko-osoite, josta verkkoasiointi avautuu, älä esimerkiksi organisaatiosi verkkoasioinnin etusivua. Kopioi ja liitä verkko-osoite ja  tarkista verkko-osoitteen toimivuus Testaa osoite -painikkeesta.'
  },
  urlPlaceholder: {
    id: 'Containers.GeneralDescription.Laws.Url.Placeholder',
    defaultMessage: 'Kopioi ja liitä tarkka verkko-osoite.'
  },
  urlButton: {
    id: 'Containers.GeneralDescription.Laws.Url.Button.Title',
    defaultMessage: 'Testaa osoite'
  },
  urlCheckerInfo: {
    id: 'Containers.GeneralDescription.Laws.Url.Icon.Tooltip',
    defaultMessage: 'Verkko-osoitetta ei löytynyt, tarkista sen muoto.'
  },
  backgroundAreaTitle: {
    id: 'Containers.GeneralDescription.BackgroundDescription.Title',
    defaultMessage: 'Tausta ja lainsäädäntö'
  },
  backgroundAreaTooltip: {
    id: 'Containers.GeneralDescription.BackgroundDescription.Tooltip',
    defaultMessage: 'Tausta ja lainsäädäntö'
  },
  backgroundDescriptionTitle: {
    id: 'Containers.GeneralDescription.BackgroundDescription.Description.Title',
    defaultMessage: 'Taustakuvaus'
  },
  backgroundDescriptionTooltip: {
    id: 'Containers.GeneralDescription.BackgroundDescription.Description.Tooltip',
    defaultMessage: 'Taustakuvaus'
  },
  backgroundDescriptionPlaceholder: {
    id: 'Containers.GeneralDescription.BackgroundDescription.Description.Placeholder',
    defaultMessage: 'Taustakuvaus'
  },
  errorMessageDescriptionIsRequired: {
    id: 'Containers.GeneralDescription.Description..ValidationError.IsRequired',
    defaultMessage: 'Pääkuvaus tai taustakuvaus. Toinen tieto on pakollinen, täytä ainakin toisen kentän tiedot.'
  }
})

export const targetGroupMessages = defineMessages({
  targetGroupTitle: {
    id: 'Containers.GeneralDescription.TargetGroup.Title',
    defaultMessage: 'Kohderyhmä'
  },
  subTargetGroupTitle: {
    id: 'Containers.GeneralDescription.SubTargetGroup.Title',
    defaultMessage: 'Tarkenna tarvittaessa tätä kohderyhmää:'
  },
  subTargetLink: {
    id: 'Containers.GeneralDescription.SubTargetGroup.Link',
    defaultMessage: 'Tarkenna kohderyhmä'
  },
  targetGroupTooltip: {
    id: 'Containers.GeneralDescription.TargetGroup.Tooltip',
    defaultMessage: 'Palvelu luokitellaan kohderyhmän mukaan. Valitse kohderyhmä, jolle palvelu on suunnattu. Valitse vähintään yksi päätason kohderyhmä ja tarvittaessa tarkenna valintaa alakohderyhmällä. Jos palvelulla ei ole erityistä alakohderyhmää, älä valitse kaikkia alemman tason kohderyhmiä, vaan jätä alemman tason valinta kokonaan tekemättä. Mikäli olet käyttänyt palvelun pohjakuvausta, kenttään on kopioitu valmiiksi pohjakuvauksen kohderyhmä/t. Voit tarvittaessa lisätä kohderyhmiä. '
  },
  errorMessageIsRequired: {
    id: 'Containers.GeneralDescription.TargetGroup.ValidationError.IsRequired',
    defaultMessage: 'Valitse vähintään yksi kohderyhmä.'
  }
})

