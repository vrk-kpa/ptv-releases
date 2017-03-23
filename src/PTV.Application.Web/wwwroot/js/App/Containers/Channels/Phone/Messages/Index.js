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
        id: "Containers.Channels.AddPhoneChannel.Header.Title",
        defaultMessage: "Lisää uusi kanava: puhelinasiointi"
    },
    mainTitleView: {
        id: "Containers.Channels.AddPhoneChannel.Header.Title.View",
        defaultMessage: "Lisää uusi kanava: puhelinasiointi"
    },
    mainText: {
        id: "Containers.Channels.AddPhoneChannel.Description",
        defaultMessage: "Puhelinasiointikanavan kautta voi saada lisätietoa palvelusta ja hoitaa palveluprosessiin liittyvää asiointia. Puhelinasiointikanavia on kolmenlaisia: puhelu, tekstiviesti ja faksi. Tässä osiossa voit lisätä uusia puhelinasiointikanavia sekä hakea ja muokata olemassa olevia."
    },
    mainTextView: {
        id: "Containers.Channels.ViewPhoneChannel.Description",
        defaultMessage: "Tällä sivulla voit katsella, muokata ja julkaista asiointikanavan tietoja."
    },
    subTitle1: {
        id: "Components.AddPhoneChannel.SubTitle1",
        defaultMessage: "Vaihe 1/2: Perustiedot"
    },
    subTitle1View: {
        id: "Components.AddPhoneChannel.SubTitle1.View",
        defaultMessage: "Vaihe 1/2: Perustiedot"
    },
    subTitle2: {
        id: "Components.AddPhoneChannel.SubTitle2",
        defaultMessage: "Vaihe 2/2: Aukioloajat"
    },
    subTitle2View: {
        id: "Components.AddPhoneChannel.SubTitle2.View",
        defaultMessage: "Vaihe 2/2: Aukioloajat"
    }
});

export const deleteMessages = defineMessages({
    text: {
        id: "Containers.Channels.PhoneChannel.Delete.Text",
        defaultMessage: "Oletko varma, että haluat poistaa kanavakuvauksen?"
    },
    buttonOk: {
        id: "Containers.Channels.PhoneChannel.Delete.Accept",
        defaultMessage: "Kyllä"
    },
    buttonCancel: {
        id: "Containers.Channels.PhoneChannel.Delete.Cancel",
        defaultMessage: "Jatka muokkausta"
    }
});

export const cancelMessages = defineMessages({
    text: {
        id: "Containers.Channels.PhoneChannel.Cancel.Text",
        defaultMessage: "Oletko varma, että haluat keskeyttää uuden kanavan luonnin? Tekemäsi muutokset katoavat."
    },
    buttonOk: {
        id: "Containers.Channels.PhoneChannel.Cancel.Accept",
        defaultMessage: "Kyllä"
    },
    buttonCancel: {
        id: "Containers.Channels.PhoneChannel.Cancel.Cancel",
        defaultMessage: "Jatka muokkausta"
    }
});

export const channelDescriptionMessages = defineMessages({
    nameTitle: {
        id: "Containers.Channels.AddPhoneChannel.Step1.Name.Title",
        defaultMessage: "Nimi"
    },
    namePlaceholder: {
        id: "Containers.Channels.AddPhoneChannel.Step1.Name.Placeholder",
        defaultMessage: "Kirjoita asiointikanavaa kuvaava, asiakaslähtöinen nimi."
    },
	organizationLabel: {
        id: "Containers.Channels.AddPhoneChannel.Step1.Organization.Title",
        defaultMessage: "Organisaatio"
    },
    organizationInfo: {
        id: "Containers.Channels.AddPhoneChannel.Step1.Organization.Tooltip",
        defaultMessage: "Valitse alasvetovalikosta organisaatio tai alaorganisaatio, joka vastaa puhelinasiointikanavasta. Mikäli puhelinasiointi on käytössä useilla alaorganisaatioilla, valitse pelkkä organisaation päätaso."
    },
    shortDescriptionLabel: {
        id: "Containers.Channels.AddPhoneChannel.Step1.ShortDescription.Title",
        defaultMessage: "Lyhyt kuvaus"
    },
    shortDescriptionInfo: {
        id: "Containers.Channels.AddPhoneChannel.Step1.ShortDescription.Tooltip",
        defaultMessage: "Kirjoita lyhyt kuvaus eli tiivistelmä puhelinasiointikanavan keskeisestä sisällöstä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään asiointikanavan."
    },
    shortDescriptionPlaceholder: {
        id: "Containers.Channels.AddPhoneChannel.Step1.ShortDescription.Placeholder",
        defaultMessage: "Kirjoita lyhyt tiivistelmä hakukoneita varten."
    },
	descriptionLabel: {
		id: "Containers.Channels.AddPhoneChannel.Step1.Description.Title",
		defaultMessage: "Kuvaus"
	},
    descriptionInfo: {
		id: "Containers.Channels.AddPhoneChannel.Step1.Description.Tooltip",
		defaultMessage: "Kuvaa puhelinasiointikanavan sisältö mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kuvaa, mistä asiasta puhelinasiointikanava tarjoaa lisätietoa ja miten sitä voi käyttää. Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa vain puhelinasiointia, älä palvelua järjestävää organisaatiota tai sen tehtäviä!"
	},
    descriptionPlaceholder: {
		id: "Containers.Channels.AddPhoneChannel.Step1.Description.Placeholder",
		defaultMessage: "Kirjoita selkeä ja ymmärrettävä kuvausteksti."
	},
});

export const languageMessages = defineMessages({
    title: {
        id: "Containers.Channels.AddPhoneChannel.Step1.LanguageProvided.Title",
        defaultMessage: "Kielet, joilla puhelinasiointi on saatavilla"
        },
    tooltip: {
        id: "Containers.Channels.AddPhoneChannel.Step1.LanguageProvided.Tooltip",
        defaultMessage: "Valitse tähän ne kielet, joilla puhelinasiointia tarjotaan asiakkaalle. Aloita kielen nimen kirjoittaminen, niin saat näkyviin kielilistan, josta voit valita kielet. "
        },
    placeholder: {
        id: "Containers.Channels.AddPhoneChannel.Step1.LanguageProvided.Placeholder",
        defaultMessage: "Kirjoita ja valitse listasta puhelinasioinnin kielet."
        },
});

export const urlMessages = defineMessages({
    label: {
        id: "Containers.Channels.AddPhoneChannel.Step1.UrlCheckerLabel.Title",
        defaultMessage: "Verkko-osoite"
        },
    tooltip: {
        id: "Containers.Channels.AddPhoneChannel.Step1.UrlCheckerLabel.Tooltip",
        defaultMessage: "Mikäli puhelinasioinnilla on omat verkkosivut, joilta on mahdollista saada lisätietoa puhelinasiointikanavasta ja sen käytöstä, lisää tähän verkko-osoite."
        },
    placeholder: {
        id: "Containers.Channels.AddPhoneChannel.Step1.UrlCheckerLabel.Placeholder",
        defaultMessage: "Kopioi ja liitä tarkka verkko-osoite."
        },
    button: {
        id: "Containers.Channels.AddPhoneChannel.Step1.UrlCheckerButton.Title",
        defaultMessage: "Testaa osoite"
        },
    checkerInfo: {
        id: "Containers.Channels.AddPhoneChannel.Step1.UrlChecker.Icon.Tooltip",
        defaultMessage: "Verkko-osoitetta ei löytynyt, tarkista sen muoto."
        },
});

export const emailMessages = defineMessages({
     title: {
        id: "Containers.Channels.AddPhoneChannel.Step1.Email.Title",
        defaultMessage: "Sähköposti"
        },
     tooltip: {
        id: "Containers.Channels.AddPhoneChannel.Step1.Email.Tooltip",
        defaultMessage: "Mikäli puhelinasioinnin käyttöön on mahdollista saada tukea sähköpostitse, kirjoita kenttään tukipalvelun sähköpostiosoite. Älä kirjoita kenttään organisaation yleisiä sähköpostiosoitteita, esim. kirjaamoon. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi."
        },
     placeholder: {
        id: "Containers.Channels.AddPhoneChannel.Step1.Email.Placeholder",
        defaultMessage: "esim. osoite@organisaatio.fi"
        },
   });

export const supportMessages = defineMessages({
    supportLabel: {
        id: "Containers.Channels.AddPhoneChannel.Step1.SupportLabel",
        defaultMessage: "Käytön tuki"
        },
});

export const phoneNumberMessages = defineMessages({
     title: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneNumber.Title",
        defaultMessage: "Puhelinnumero"
        },
     tooltip: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneNumber.Tooltip",
        defaultMessage: "Kirjoita puhelinnumero kansainvälisessä muodossa ilman välilyöntejä (esim. +358451234567)."
        },
     placeholder: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneNumber.PlaceHolder",
        defaultMessage: "esim. +358451234567"
        },
     chargeTypeTitle: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneCost.Title",
        defaultMessage: "Puhelun maksullisuus"
        },
     chargeTypeTooltip: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneCost.Tootltip",
        defaultMessage: 'Ensimmäinen vaihtoehto tarkoittaa, että puhelu maksaa lankaliittymästä soitettaessa paikallisverkkomaksun (pvm), matkapuhelimesta soitettaessa matkapuhelinmaksun (mpm) tai ulkomailta soitettaessa ulkomaanpuhelumaksun. Valitse "täysin maksuton" vain, jos puhelu on soittajalle kokonaan maksuton. Jos puhelun maksu muodostuu muulla tavoin, valitse "muu maksu" ja kuvaa puhelun hintatiedot omassa kentässään.'
        },
     phoneCostAllCosts: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneAllCosts.Title",
        defaultMessage: "Paikallisverkkomaksu (pvm), Matkapuhelinmaksu (mpm), Ulkomaanpuhelumaksu"
        },
     phoneCostFree: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneCostFree.Title",
        defaultMessage: "Täysin maksuton"
        },
     phoneCostOther: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneCostOther.Title",
        defaultMessage: "Muu maksu, anna tarkemmat tiedot:"
        },
     costDescriptionTitle: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneCostOtherDescription.Title",
        defaultMessage: "Puhelun hintatiedot"
        },
     costDescriptionPlaceholder: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneCostOtherDescription.Placeholder",
        defaultMessage: "esim. Pvm:n lisäksi jonotuksesta veloitetaan..."
        },
     costDescriptionTooltip: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneCostOtherDescription.Tooltip",
        defaultMessage: "Kerro sanallisesti, mitä puhelinnumeroon soittaminen maksaa."
        },
      infoTitle:{
        id: "Containers.Channels.AddPhoneChannel.Step1.AdditionalInformation.Title",
        defaultMessage: "Lisätieto"
        },
      infoTooltip:{
        id: "Containers.Channels.AddPhoneChannel.Step1.AdditionalInformation.Tooltip",
        defaultMessage: "Voit tarvittaessa antaa puhelinnumerolle lisätiedon, jolla voit tarkentaa, mistä numerosta on kyse. Esimerkiksi vaihde tai asiakaspalvelu."
        },
      infoPlaceholder:{
        id: "Containers.Channels.AddPhoneChannel.Step1.AdditionalInformation.Placeholder",
        defaultMessage:"esim. Vaihde"
        },
      prefixTitle:{
        id: "Containers.Channels.AddPhoneChannel.Step1.PhonePrefix.Title",
        defaultMessage:"Maan suuntanumero"
        },
      prefixPlaceHolder:{
        id: "Containers.Channels.AddPhoneChannel.Step1.PhonePrefix.Placeholder",
        defaultMessage:"esim. +358"
        },
      prefixTooltip: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhonePrefix.Tooltip",
        defaultMessage:"esim. +358"
      }, 
      phoneTypesTitle: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneTypes.Title",
        defaultMessage: "Puhelinkanavan tyyppi"
        },
     phoneTypesTooltip: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneTypes.Tooltip",
        defaultMessage: "Puhelinkanavan tyyppi"
        },	
    finnishServiceNumberName: {
        id: "Containers.Channels.AddPhoneChannel.Step1.PhoneTypes.FinishServiceNumber.Name",
        defaultMessage: "Suomalainen palvelunumero"
        }   
});

export const openingHoursMessages = defineMessages({
    collapsedInfo: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.CollapsedInfo",
        defaultMessage: "Muokattu"
    },
    mainTooltipNormal: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.MainTooltipNormal",
        defaultMessage: "This is a tooltip for NOH"
    },
    mainLabelNormal: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.MainLabelNormal",
        defaultMessage: "Normaali aukioloaika"
    },
    defaultTitle_openingHoursNormal: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.DefaultTitleNormal",
        defaultMessage: "Normaaliaukioloajat"
    },
    validOnward: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.ValidityType.Onward",
        defaultMessage: "Toistaiseksi voimassa oleva"
    },
    validPeriod: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.ValidityType.Period",
        defaultMessage: "Voimassa ajanjaksolla"
    },
    additionalInformation: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.AdditionalInformation.Title",
        defaultMessage: "Lisätieto"
    },
    nonstopOpeningHours: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.Nonstop.Title",
        defaultMessage: "Joka päivä ympäri vuorokauden käytettävissä"
    },
    startDate: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.StartDate.Title",
        defaultMessage: "Alkaa"
    },
    endDate: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.EndDate.Title",
        defaultMessage: "Päättyy"
    },
    mainTooltipSpecial: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.MainTooltipSpecial",
        defaultMessage: "This is a tooltip for SOH"
    },
    mainLabelSpecial: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.MainLabelSpecial",
        defaultMessage: "Vuorokauden yli menevät ajat"
    },
    defaultTitle_openingHoursSpecial: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.DefaultTitleSpecial",
        defaultMessage: "Aukioloaikojen täyttämisohjeet"
    },
    previewTitle: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.PreviewTitle",
        defaultMessage: "Esikatselu"
    },
    previewTooltip: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.PreviewTooltip",
        defaultMessage: "This is a tooltip for Preview"
    },
    previewInstructions1: {
        id: "Containers.Channels.AddElectronicChannel.Step2.OpeningHours.PreviewInstructions1",
        defaultMessage: "Valitse viikonpäivät ja kellonajat, näet esikatselun lisättyäsi tietoa."
    },
    previewInstructions2: {
        id: "Containers.Channels.AddElectronicChannel.Step2.OpeningHours.PreviewInstructions2",
        defaultMessage: "Jos aukioloajalle on voimassaoloaika, valitse 'Voimassa ajanjaksolla'."
    },
    weekday_mo: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.Monday",
        defaultMessage: "Maanantai"
    },
    weekday_tu: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.Tuesday",
        defaultMessage: "Tiistai"
    },
    weekday_we: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.Wednesday",
        defaultMessage: "Keskiviikko"
    },
    weekday_th: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.Thursday",
        defaultMessage: "Torstai"
    },
    weekday_fr: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.Friday",
        defaultMessage: "Perjantai"
    },
    weekday_sa: {
        id: "Containers.Channels.AddPhoneChannel.Step2.OpeningHours.Saturday",
        defaultMessage: "Lauantai"
    },
    weekday_su: {
        id: "Containers.Channels.AddElectronicChannel.Step2.OpeningHours.Sunday",
        defaultMessage: "Sunnuntai"
    },
    mainTooltipExceptional: {
        id: "Containers.Channels.AddElectronicChannel.Step2.OpeningHours.MainTooltipExceptional",
        defaultMessage: "This is a tooltip for EOH"
    },
    mainLabelExceptional: {
        id: "Containers.Channels.AddElectronicChannel.Step2.OpeningHours.MainLabelExceptional",
        defaultMessage: "Poikkeusaukioloaika"
    },
    defaultTitle_openingHoursExceptional: {
        id: "Containers.Channels.AddElectronicChannel.Step2.OpeningHours.DefaultTitleExceptional",
        defaultMessage: "Poikkeusaukioloajat"
    },
    validDaySingle: {
        id: "Containers.Channels.AddElectronicChannel.Step2.OpeningHours.ValidityType.DaySingle",
        defaultMessage: "Päivä"
    },
    validDayRange: {
        id: "Containers.Channels.AddElectronicChannel.Step2.OpeningHours.ValidityType.DayRange",
        defaultMessage: "Ajanjakso"
    },
    closedDaySingle: {
        id: "Containers.Channels.AddElectronicChannel.Step2.OpeningHours.ValidityType.ClosedDaySingle",
        defaultMessage: "Suljettu koko päivän"
    },
    closedDayRange: {
        id: "Containers.Channels.AddElectronicChannel.Step2.OpeningHours.ValidityType.ClosedDayRange",
        defaultMessage: "Suljettu koko ajanjakso"
    },
    closedMessage: {
        id: "Containers.Channels.AddElectronicChannel.Step2.OpeningHours.ValidityType.ClosedMessage",
        defaultMessage: "Suljettu"
    }
});