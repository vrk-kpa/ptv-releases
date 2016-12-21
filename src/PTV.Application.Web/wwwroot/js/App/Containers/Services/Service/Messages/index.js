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
import { defineMessages } from 'react-intl';

export const messages = defineMessages({
    mainTitle: {
        id: "Containers.Services.AddService.Header.Title",
        defaultMessage: "Lisää uusi palvelu"
    },
    mainText: {
        id: "Containers.Services.AddService.Header.Description",
        defaultMessage: "Tässä osiossa voit lisätä uuden palvelun ja liittää siihen asiointikanavia. Kuvaile palvelua mahdollisimman selkeästi, kattavasti ja asiakasystävällisesti."
    },
    subTitle1: {
        id: "Containers.Services.AddService.Step1.Header.Title",
        defaultMessage: "Vaihe 1/4: Palvelun perustiedot"
    },
    subTitle2: {
        id: "Containers.Services.AddService.Step2.Header.Title",
        defaultMessage: "Vaihe 2/4: Luokittelu ja ontologiakäsitteet"
    },
    subTitle3: {
        id: "Containers.Services.AddService.Step3.Header.Title",
        defaultMessage: "Vaihe 3/4: Palvelun tuottaminen ja alueelliset tiedot"
    },
    subTitle4: {
        id: "Containers.Services.AddService.Step4.Header.Title",
        defaultMessage: "Vaihe 4/4: Liitä kanavia palveluun"
    },
    mainTitleView: {
        id: "Containers.Services.ViewService.Header.Title",
        defaultMessage: "Palvelun katselu ja muokkaus"
    },
    mainTextView: {
        id: "Containers.Services.ViewService.Header.Description",
        defaultMessage: "Tällä sivulla voit katsella, muokata ja julkaista palvelun tietoja."
    },
    subTitle1View: {
        id: "Containers.Services.ViewService.Step1.Header.Title",
        defaultMessage: "1/4 Palvelun perustiedot"
    },
    subTitle2View: {
        id: "Containers.Services.ViewService.Step2.Header.Title",
        defaultMessage: "2/4 Luokittelu ja ontologiakäsitteet"
    },
    subTitle3View: {
        id: "Containers.Services.ViewService.Step3.Header.Title",
        defaultMessage: "3/4 Palvelun tuottaminen ja alueelliset tiedot"
    },
    subTitle4View: {
        id: "Containers.Services.ViewService.Step4.Header.Title",
        defaultMessage: "4/4 Liitetyt kanavat"
    }
});

export const cancelMessages = defineMessages({
    text: {
        id: "Containers.Service.AddService.Cancel.Text",
        defaultMessage: "Oletko varma, että haluat keskeyttää uuden kanavan luonnin? Tekemäsi muutokset katoavat."
    },
    buttonOk: {
        id: "Containers.Service.AddService.Cancel.Accept",
        defaultMessage: "Kyllä"
    },
    buttonCancel: {
        id: "Containers.Service.AddService.Cancel.Cancel",
        defaultMessage: "Jatka muokkausta"
    }
});

export const deleteMessages = defineMessages({
    text: {
        id: "Containers.Service.AddService.Delete.Text",
        defaultMessage: "Oletko varma, että haluat poistaa palvelukuvauksen?"
    },
    buttonOk: {
        id: "Containers.Service.AddService.Delete.Accept",
        defaultMessage: "Kyllä"
    },
    buttonCancel: {
        id: "Containers.Service.AddService.Delete.Cancel",
        defaultMessage: "Jatka muokkausta"
    }
});

export const saveDraftMessages = defineMessages({
    text: {
        id: "Containers.Service.ViewService.SaveDraft.Text",
        defaultMessage: "Luonnos on tallennettu. Voit julkaista palvelun sivulla olevasta Julkaise painikkeesta tai jatkaa Palvelut -sivulle."
    },
    buttonOk: {
        id: "Containers.Service.ViewService.SaveDraft.Accept",
        defaultMessage: "Ok"
    },
    buttonCancel: {
        id: "Containers.Service.ViewService.SaveDraft.Cancel",
        defaultMessage: "Palaa Palvelut -sivulle"
    }
});

export const serviceTypeMessages = defineMessages({
   title:{
        id: "Containers.Services.AddService.Step1.ServiceType.Title",
        defaultMessage: "Palvelutyyppi"
    },
    tooltip:{
        id: "Containers.Services.AddService.Step1.ServiceType.Tooltip",
        defaultMessage: "Palvelutyyppi"
    },  
});  

export const serviceChargeTypeMessages = defineMessages({
    title: {
        id: "Containers.Services.AddService.Step1.ChargeType.Title",
        defaultMessage: "Maksullisuuden tiedot"
    },    
    tooltip: {
        id: "Containers.Services.AddService.Step1.ChargeType.Tooltip",
        defaultMessage: "Missing"
    },
    additionalInfoTitle: {
        id: "Containers.Services.AddService.Step1.ChargeTypeAdditionalInfo.Title",
        defaultMessage: "Maksullisuuden lisätieto"
    }, 
});  

export const serviceAvailableLanguageMessages = defineMessages({
    title: {
        id: "Containers.Services.AddService.Step1.AvailableLanguages.Title",
        defaultMessage: "Kielet, joilla palvelu on saatavilla"
    },
    placeholder: {
        id: "Containers.Services.AddService.Step1.AvailableLanguages.Placeholder",
        defaultMessage: "Kirjoita ja valitse listasta palvelun kielet."
    },
    tooltip: {
        id: "Containers.Services.AddService.Step1.AvailableLanguages.Tooltip",
        defaultMessage: "Valitse tähän ne kielet, joilla palvelua tarjotaan asiakkaalle. Aloita kielen nimen kirjoittaminen, niin saat näkyviin kielilistan, josta voit valita kielet."
    },
});  

export const serviceCoverageMessages = defineMessages({
    title : {
        id : "Containers.Services.AddService.ServiceCoverage.Title",
        defaultMessage: "Alue, jolla palvelu on saatavilla"
    },
    tooltip : {
        id : "Containers.Services.AddService.ServiceCoverage.Tooltip",
        defaultMessage: 'Jos palvelua voi käyttää vain esim. tietyssä fyysisessä paikassa tai tietyssä kunnassa, valitse "Palvelu on saatavilla seuraavissa kunnissa"”", jolloin näkyviin tulee valikko, josta voit valita kunnan tai kunnat. Jos palvelun käyttö ei ole sidoksissa tiettyyn paikkaan tai alueeseen, valitse "Palvelu on saatavilla koko maassa".'
    },
    municipalitiesTitle : {
        id : "Containers.Services.AddService.Municipalities.Title",
        defaultMessage: "Kunnat, joissa palvelua tarjotaan"
    },
    municipalitiesTooltip : {
        id : "Containers.Services.AddService.Municipalities.Tooltip",
        defaultMessage: "Kirjota ja valtise listasta kunnat."
    },
});

export const serviceTypeAdditionalInfoMessages = defineMessages({
    permissionTitle:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.Permission.Title",
        defaultMessage: "Lupaan liittyvät lisätiedot*"
    },
    noticeTitle:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.Notice.Title",
        defaultMessage: "Ilmoitukseen liittyvät lisätiedot*"
    },     
    registrationTitle:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.Registration.Title",
        defaultMessage: "Rekisteröintiin liittyvät lisätiedot*"
    },
    tasksTitle:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.Tasks.Title",
        defaultMessage: "Velvoitteet"
    },
    tasksTooltip:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.Tasks.Tooltip",
        defaultMessage: "Kuvaa lyhyesti lupaan/ilmoitukseen/rekisteröintiin mahdollisesti liittyvät velvoitteet. Esim. ilmoitettujen tietojen muutoksista ja toiminnan lakkaamisesta on ilmoitettava viipymättä."
    },
    deadlineTitle:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.DeadLine.Title",
        defaultMessage: "Määräaika"
    },
    deadlinePlaceholder:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.DeadLine.Placeholder",
        defaultMessage: "Kirjoita määräaikaan liittyvät tiedot."
    },
    deadlineTootltip:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.DeadLine.Tooltip",
        defaultMessage: "Kuvaa lyhyesti, jos lupaa on haettava / ilmoitus tai rekisteröinti on tehtävä tiettyyn määräaikaan mennessä. Esim. Ilmoitus on tehtävä viimeistään neljä (4) viikkoa ennen toiminnan aloittamista tai olennaista muuttamista."
    },
    processingTimeTitle:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.ProcessingTime.Title",
        defaultMessage: "Käsittelyaika"
    },
    processingTimePlaceholder:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.ProcessingTime.Placeholder",
        defaultMessage: "Kirjoita käsittelyaikaan liittyvät tiedot."
    },
    processingTimeTootltip:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.ProcessingTime.Tootltip",
        defaultMessage: "Kuvaa lyhyesti, miten kauan asian käsittely viranomaistasolla kestää. Esim. Lupa käsitellään kuuden (6) kuukauden kuluessa hakemuksen vastaanottamisesta tai, jos hakemus on puutteellinen, siitä kun hakija on antanut asian ratkaisemista varten tarvittavat asiakirjat ja selvitykset."
    },
    validityTimeTitle:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.ValidityTime.Title",
        defaultMessage: "Voimassaoloaika"
    },
    validityTimePlaceholder:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.ValidityTime.Placeholder",
        defaultMessage: "Kirjoita voimassaoloaikaan liittyvät tiedot."
    },
    validityTimeTootltip:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.ValidityTime.Tootltip",
        defaultMessage: "Kuvaa lyhyesti tieto siitä, miten kauan lupa/ilmoitus/rekisteröinti on voimassa. Esim. Lupa on voimassa kolme (3) vuotta. / Lupa on voimassa toistaiseksi."
    },
    tasksPermissionPlaceholder:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.Tasks.Permission.Placeholder",
        defaultMessage: "Kuvaa lyhyesti, jos luvan saaneella on velvollisuuksia."
    },  
    tasksNoticePlaceholder:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.Tasks.Notice.Placeholder",
        defaultMessage: "Kuvaa lyhyesti, jos ilmoitukseen liittyy velvollisuuksia."
    },
    tasksRegistrationPlaceholder:{
        id: "Containers.Services.AddService.Step1.ServiceType.AdditionalInfo.Tasks.Registration.Placeholder",
        defaultMessage: "Kuvaa lyhyesti, jos rekisteröinnin tekemiseen liittyy velvollisuuksia."
    },
});

export const connectGeneralDescriptionMessages = defineMessages({
    title: {
		id: "Containers.Services.AddService.Step1.Service.ConnectGeneralDescription.Title",
		defaultMessage: "Pohjakuvauksen liittäminen palveluun"
	},
    tooltip: {
		id: "Containers.Services.AddService.Step1.Service.ConnectGeneralDescription.Tooltip",
		defaultMessage: "Palvelun pohjakuvaus on yleispätevä ja valtakunnallisesti kattava palvelun kuvaus. Valitse haluatko liittää palveluusi pohjakuvauksen. Kun pohjakuvaus liitetään, sen tiedot kopioituvat pohjaksi uudelle palvelulle."
	},
    placeholder: {
		id: "Containers.Services.AddService.Step1.Service.ConnectGeneralDescription.Description.Placeholder",
		defaultMessage: "Kirjoita tähän kuvaus, jossa tarkennat pohjakuvauksessa annettuja tietoja."
	},
	optionNoDescriptionTitle: {
		id: "Containers.Services.AddService.Step1.ConnectGeneralDescription.NoDescription.Title",
		defaultMessage: "Ei pohjakuvausta"
	},
	optionConnectDescriptionTitle: {
		id: "Containers.Services.AddService.Step1.ConnectGeneralDescription.ConnectDescription.Title",
		defaultMessage: "Liitä pohjakuvaus"
	},
    optionConnectedDescriptionTitle: {
		id: "Containers.Services.AddService.Step1.ConnectGeneralDescription.ConnectedDescription.Title",
		defaultMessage: "Liitetty pohjakuvaus"
	},
	connectLink: {
		id: "Containers.Services.AddService.Step1.GeneralDescription.Link.Connect.Title",
		defaultMessage: "Liitä pohjakuvaus"
	},
    changeLink: {
		id: "Containers.Services.AddService.Step1.GeneralDescription.Link.Change.Title",
		defaultMessage: "Vaihda pohjakuvaus"
	},
    // connectedGeneralDescriptionNotification: {
	// 	id: "Containers.Services.AddService.Step1.GeneralDescription.Notification",
	// 	defaultMessage: "Palvelun pohjakuvauksesta tulevat perustiedot on esitäytetty palvelulle. Palvelun nimeä lukuun ottamatta et voi muuttaa tai poistaa tietoja, mutta tarvittaessa voit lisätä niitä."
	// },
    connectedTitle: {
		id: "Containers.Services.ViewService.Step1.ConnectedGeneralDescription.Title",
		defaultMessage: "Palveluun on liitetty pohjakuvaus: "
	}        
});
export const serviceNamesMessages = defineMessages({
    nameTitle: {
        id: "Containers.Services.AddService.Step1.Name.Title",
        defaultMessage: "Nimi"
    },
    namePlaceholder: {
        id: "Containers.Services.AddService.Step1.Name.Placeholder",
        defaultMessage: "Kirjoita palvelua kuvaava, asiakaslähtöinen nimi."
    },
    nameTooltip: {
        id: "Containers.Services.AddService.Step1.Name.Tooltip",
        defaultMessage: "Kirjoita palvelun nimi mahdollisimman kuvaavasti. Älä kirjoita organisaation nimeä palvelun nimeen. Jos olet käyttänyt palvelun pohjakuvausta, muokkaa nimeä vain jos se on ehdottoman välttämätöntä!"
    },
	alternateNameTitle: {
		id: "Containers.Services.AddService.Step1.AlternateName.Title",
		defaultMessage: "Vaihtoehtoinen nimi"
	},
    alternateNamePlaceholder: {
		id: "Containers.Services.AddService.Step1.AlternateName.Placeholder",
		defaultMessage: "Kirjoita palvelulle tarvittaessa muu nimi."
	},
    alternateNameTooltip: {
		id: "Containers.Services.AddService.Step1.AlternateName.Tooltip",
		defaultMessage: "Kirjoita palvelulle tarvittaessa muu nimi, jolla käyttäjä saattaa hakea palvelua."
	},
});

export const serviceDescriptionMessages = defineMessages({    
    shortDescriptionTitle: {
        id: "Containers.Services.AddService.Step1.ShortDescription.Title",
        defaultMessage: "Lyhyt kuvaus"
    },
    shortDescriptionPlaceholder: {
        id: "Containers.Services.AddService.Step1.ShortDescription.Placeholder",
        defaultMessage: "Kirjoita lyhyt tiivistelmä hakukoneita varten."
    },
    shortDescriptionTooltip: {
        id: "Containers.Services.AddService.Step1.ShortDescription.Tooltip",
        defaultMessage: "Kirjoita lyhyt kuvaus eli tiivistelmä palvelun keskeisestä sisällöstä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään palvelun."
    },
    descriptionTitle: {
		id: "Containers.Services.AddService.Step1.Description.Title",
		defaultMessage: "Kuvaus"
	},
    descriptionPlaceholder: {
		id: "Containers.Services.AddService.Step1.Description.PlaceHolder",
		defaultMessage: "Kirjoita palvelulle selkeä ja ymmärrettävä kuvaus."
	},
    descriptionTooltip: {
		id: "Containers.Services.AddService.Step1.Description.Tooltip",
		defaultMessage: "Kuvaa palvelu mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kerro, mitä palvelu pitää sisällään, miten sitä tarjotaan asiakkaalle, mihin tarpeeseen se vastaa ja mihin sillä pyritään. Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa asiakkaalle tarjottavaa palvelua, älä palvelua järjestävää organisaatiota tai sen tehtäviä! Voit jakaa tekstiä kappaleisiin ja tarvittaessa käyttää luettelomerkkejä. Jos olet liittänyt palveluun pohjakuvauksen, käytä kuvauskenttä sen kertomiseen, miten pohjakuvauksessa kuvattu palvelu on sinun organisaatiossasi/seudullasi/kunnassasi järjestetty ja mitä erityispiirteitä tällä palvelulla on. Älä toista pohjakuvauksessa jo kerrottuja asioita. "
	},
	conditionOfServiceUsageTitle: {
		id: "Containers.Services.AddService.Step1.ConditionOfServiceUsage.Title",
		defaultMessage: "Palvelun käytön edellytykset"
	},
    conditionOfServiceUsageServicePlaceholder: {
		id: "Containers.Services.AddService.Step1.ConditionOfServiceUsage.Service.Placeholder",
		defaultMessage: "Kuvaa lyhyesti, jos palvelun käyttöön liittyy ehtoja tai edellytyksiä, kirjoita ne tähän kenttään."
	},
    conditionOfServiceUsagePermissionPlaceholder: {
		id: "Containers.Services.AddService.Step1.ConditionOfServiceUsage.Permission.Placeholder",
		defaultMessage: "Kuvaa lyhyesti, jos luvan saamiseen liittyy ehtoja tai edellytyksiä, kirjoita ne tähän kenttään."
	},
    conditionOfServiceUsageNoticePlaceholder: {
		id: "Containers.Services.AddService.Step1.ConditionOfServiceUsage.Notice.Placeholder",
		defaultMessage: "Kuvaa lyhyesti, jos ilmoituksen tekemiseen liittyy ehtoja tai edellytyksiä, kirjoita ne tähän kenttään."
	},
    conditionOfServiceUsageRegistrationPlaceholder: {
		id: "Containers.Services.AddService.Step1.ConditionOfServiceUsage.Registration.Placeholder",
		defaultMessage: "Kuvaa lyhyesti, jos rekisteröinnin tekemiseen liittyy ehtoja tai edellytyksiä, kirjoita ne tähän kenttään."
	},
    conditionOfServiceUsageTooltip: {
		id: "Containers.Services.AddService.Step1.ConditionOfServiceUsage.Tooltip",
		defaultMessage: 'Mikäli käyttäjä saa palvelua vain tietyin edellytyksin, kuvaa sanallisesti nämä edellytykset, ehdot ja kriteerit. Esim. "Palvelun piiriin kuuluvat vain työttömyysuhan alla olevat henkilöt". Myös mikäli palvelun onnistunut käyttö edellyttää esim. tiettyjen tietojen tai dokumenttien hankkimista, kuvaa se tähän: Esim. "Muutosverokorttia varten tarvitset tiedot vuoden alusta kertyneistä tuloista ja maksamistasi veroista, sekä arvion koko vuoden tuloista."'
	},
	serviceUserInstructionTitle: {
		id: "Containers.Services.AddService.Step1.ServiceUserInstruction.Title",
		defaultMessage: "Toimintaohjeet"
	},
    serviceUserInstructionPlaceholder: {
		id: "Containers.Services.AddService.Step1.ServiceUserInstruction.Placeholder",
		defaultMessage: "Mikäli palvelun saamiseksi on toimittava tietyllä tavalla, kirjoita ohjeistus tähän kenttään."
	},
    serviceUserInstructionTooltip: {
		id: "Containers.Services.AddService.Step1.ServiceUserInstruction.Tooltip",
		defaultMessage: 'Jos palvelun käyttö tai etuuden hakeminen edellyttää tiettyä toimintajärjestystä tai tietynlaista toimintatapaa, kuvaa se tähän mahdollisimman tarkasti. Esimerkiksi jos jokin asiointikanava on ensisijainen, kerro se tässä. "Käytä ilmoittautumiseen ensisijaisesti verkkoasiointia."'
	}
});

export const serviceTargetGroupMessages = defineMessages({
    errorMessageIsRequired: {
        id: "Containers.Services.AddService.Step2.TargetGroup.ValidationError.IsRequired",
        defaultMessage: "Valitse vähintään yksi kohderyhmä."
    },
});

export const serviceServiceClassMessages = defineMessages({
    title: {
        id: "Containers.Services.AddService.Step2.ServiceClass.Title",
        defaultMessage: "Palveluluokka"
    },
    tooltip: {
        id: "Containers.Services.AddService.Step2.ServiceClass.Tooltip",
        defaultMessage: "Palvelulle valitaan aihepiirin mukaan palveluluokka. Valitse vähintään yksi mahdollisimman tarkka palveluluokka. Mikäli olet käyttänyt palvelun pohjakuvausta, kenttään on kopioitu valmiiksi pohjakuvauksen palveluluokka/luokat. Voit tarvittaessa lisätä palveluluokkia."
    },
    targetListHeader: {
        id: "Containers.Services.AddService.Step2.ServiceClass.TargetList.Header",
        defaultMessage: "Lisätyt palveluluokat:"
    },
    errorMessageIsRequired: {
        id: "Containers.Services.AddService.Step2.ServiceClass.ValidationError.IsRequired",
        defaultMessage: "Valitse vähintään yksi palveluluokka."
    },
});

export const serviceOntologyTermMessages = defineMessages({
    title: {
        id: "Containers.Services.AddService.Step2.OntologyTerms.Title",
        defaultMessage: "Ontologiakäsitteet"
    },
    tooltip: {
        id: "Containers.Services.AddService.Step2.OntologyTerms.Tooltip",
        defaultMessage: "Palvelun asiasisältö kuvataan ontologiakäsitteillä, joiden käyttö helpottaa palveluiden löytämistä. Kirjoita palvelun asiasisältöä mahdollisimman tarkasti kuvaava sana ja valitse ennakoivan tekstinsyötön tarjoamista vaihtoehdoista ontologiakäsite. Valitse palvelun kuvaamiseksi vähintään yksi ja enintään kymmenen käsitettä. Mikäli olet käyttänyt palvelun pohjakuvausta, kenttään on kopioitu valmiiksi pohjakuvauksen ontologiakäsitteet. Voit tarvittaessa lisätä uusia käsitteitä. "
    },
    errorMessageIsRequired: {
        id: "Containers.Services.AddService.Step2.OntologyTerms.ValidationError.IsRequired",
        defaultMessage: "Valitse vähintään yksi ontologiakäsite."
    },
    targetListHeader: {
        id: "Containers.Services.AddService.Step2.OntologyTerms.TargetList.Header",
        defaultMessage: "Lisätyt ontologiakäsitteet:"
    },

});

export const serviceLifeEventMessages = defineMessages({
    title: {
        id: "Containers.Services.AddService.Step2.LifeEvent.Title",
        defaultMessage: "Elämäntilanne"
    },
    tooltip: {
        id: "Containers.Services.AddService.Step2.LifeEvent.Tooltip",
        defaultMessage: "Palvelu luokitellaan tarvittaessa elämäntilanteen mukaan. Jos palvelu ei suoraan liity tiettyyn elämäntilanteeseen, jätä valinta tekemättä. Mikäli olet käyttänyt palvelun pohjakuvausta, kenttään on kopioitu valmiiksi pohjakuvauksen mahdolliset elämäntilanteet. Voit tarvittaessa lisätä elämäntilanteita. "
    },
    targetListHeader: {
        id: "Containers.Services.AddService.Step2.LifeEvent.TargetList.Header",
        defaultMessage: "Lisätyt elämäntilanteet:"
    },

});

export const serviceIndustrialClassMessages = defineMessages({
    title: {
        id: "Containers.Services.AddService.Step2.IndustrialClass.Title",
        defaultMessage: "Toimialaluokka"
    },
    tooltip: {
        id: "Containers.Services.AddService.Step2.IndustrialClass.Tooltip",
        defaultMessage: "Valitse toimiala(t), jota lupa/ilmoitus/rekisteröinti koskee."
    },
    targetListHeader: {
        id: "Containers.Services.AddService.Step2.IndustrialClass.TargetList.Header",
        defaultMessage: "Lisätyt toimialaluokat:"
    },
});

export const serviceOrganizerMessages = defineMessages({
    title : {
        id : "Containers.Services.AddService.Step3.Organizers.Title",
        defaultMessage: "Palvelun vastuutaho"
    },
    tooltip : {
        id : "Containers.Services.AddService.Step3.Organizers.Tooltip",
        defaultMessage: "Palvelun vastuutaho on organisaatio, joka vastaa palvelun järjestämisestä. Vastuutaho voi olla samalla myös palvelun tuottaja (käytännön toteuttaja), mutta voi olla, että palvelun tuottaa osittain tai kokonaan muu organisaatio tai useampi. Valitse vähintään yksi vastuuorganisaatio. Mikäli palvelusta vastaa useampi organisaatio, valitse ne kaikki."
    },
    sourceListSearchPlaceholder : {
        id : "Containers.Services.AddService.Step3.Organizers.SourceList.Search.Placeholder",
        defaultMessage: "Kirjoita ja valitse listasta palvelun vastuutaho."
    },
    targetListHeader : {
        id : "Containers.Services.AddService.Step3.Organizers.TargetList.Header",
        defaultMessage: "Lisätyt vastuutahot:"
    },
});


 
 