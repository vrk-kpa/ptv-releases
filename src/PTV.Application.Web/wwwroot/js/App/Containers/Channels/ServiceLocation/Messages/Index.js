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
        id: "Containers.Channels.AddLocationChannel.Header.Title",
        defaultMessage: "Lisää uusi kanava: palvelupiste"
    },
    mainTitleView: {
        id: "Containers.Channels.AddLocationChannel.Header.Title.View",
        defaultMessage: "Lisää uusi kanava: palvelupiste"
    },
    mainText: {
        id: "Containers.Channels.AddLocationChannel.Descritpion",
        defaultMessage: "Palvelupiste on asiointikanava, jossa voit asioida tai ottaa käyttöön palvelun. Palvelupiste on aina jokin fyysinen paikka, jolla on osoite tai muu sijaintitieto. Esimerkiksi verotoimisto tai puisto ovat palvelupisteitä. Tässä osiossa voit lisätä organisaatiollesi uuden palvelupisteen."
    },
    mainTextView: {
        id: "Containers.Channels.ViewLocationChannel.Descritpion",
        defaultMessage: "Tällä sivulla voit katsella, muokata ja julkaista asiointikanavan tietoja."
    },
    subTitle1: {
        id: "Containers.Channels.AddLocationChannel.StepContainer1.Title",
        defaultMessage: "Vaihe 1/4: Perustiedot"
    },
    subTitle1View: {
        id: "Containers.Channels.AddLocationChannel.StepContainer1.Title.View",
        defaultMessage: "Vaihe 1/4: Perustiedot"
    },
    subTitle2: {
        id: "Containers.Channels.AddLocationChannel.StepContainer2.Title",
        defaultMessage: "Vaihe 2/4: Yhteystiedot"
    },
    subTitle2View: {
        id: "Containers.Channels.AddLocationChannel.StepContainer2.Title.View",
        defaultMessage: "Vaihe 2/4: Yhteystiedot"
    },
    subTitle3: {
        id: "Containers.Channels.AddLocationChannel.StepContainer3.Title",
        defaultMessage: "Vaihe 3/4: Käyntiosoite*"
    },
    subTitle3View: {
        id: "Containers.Channels.AddLocationChannel.StepContainer3.Title.View",
        defaultMessage: "Vaihe 3/4: Käyntiosoite*"
    },
    subTitle4: {
        id: "Containers.Channels.AddLocationChannel.StepContainer4.Title",
        defaultMessage: "Vaihe 4/4: Aukioloajat"
    },
    subTitle4View: {
        id: "Containers.Channels.AddLocationChannel.StepContainer4.Title.View",
        defaultMessage: "Vaihe 4/4: Aukioloajat"
    },
    subTitle5: {
        id: "Containers.Channels.AddLocationChannel.StepContainer5.Title",
        defaultMessage: "Vaihe 5/5: Luokittelut ja ontologiasanat"
    },
    cancelButton: {
        id: "Containers.Channels.AddLocationChannel.StepContainer.CancelButton",
        defaultMessage: "Keskeytä"
    },
    nextAndSaveAllButton: {
        id: "Containers.Channels.AddLocationChannel.StepContainer.NextAndSaveAllButton",
        defaultMessage: "Jatka"
    },
    saveAllDraftButton: {
        id: "Containers.Channels.AddLocationChannel.StepContainer.SaveAllDraftButton",
        defaultMessage: "Tallenna luonnos"
    },
    messageServiceSaved: {
        id: "Containers.Channels.AddLocationChannel.StepContainer.MessageServiceSaved",
        defaultMessage: "Onnittelut! Palvelun tallennus onnistui."
    },
    showContacts: {
        id: "Containers.Channels.Common.ShowContacts",
        defaultMessage: "Lisää yhteystiedot"
    },
});

export const deleteMessages = defineMessages({
    text: {
        id: "Containers.Channels.LocationChannel.Delete.Text",
        defaultMessage: "Oletko varma, että haluat poistaa kanavakuvauksen?"
    },
    buttonOk: {
        id: "Containers.Channels.LocationChannel.Delete.Accept",
        defaultMessage: "Kyllä"
    },
    buttonCancel: {
        id: "Containers.Channels.LocationChannel.Delete.Cancel",
        defaultMessage: "Jatka muokkausta"
    }
});

export const cancelMessages = defineMessages({
    text: {
        id: "Containers.Channels.LocationChannel.Cancel.Text",
        defaultMessage: "Oletko varma, että haluat keskeyttää uuden kanavan luonnin? Tekemäsi muutokset katoavat."
    },
    buttonOk: {
        id: "Containers.Channels.LocationChannel.Cancel.Accept",
        defaultMessage: "Kyllä"
    },
    buttonCancel: {
        id: "Containers.Channels.LocationChannel.Cancel.Cancel",
        defaultMessage: "Jatka muokkausta"
    }
});

export const channelDescriptionMessages = defineMessages({
    nameTitle: {
        id: "Containers.Channels.AddServiceLocationChannel.Step1.Name.Title",
        defaultMessage: "Nimi"
    },
    namePlaceholder: {
        id: "Containers.Channels.AddServiceLocationChannel.Step1.Name.Placeholder",
        defaultMessage: "Kirjoita asiointikanavaa kuvaava, asiakaslähtöinen nimi."
    },
	organizationLabel: {
        id: "Containers.Channels.AddServiceLocationChannel.Step1.Organization.Title",
        defaultMessage: "Organisaatio"
    },
    organizationInfo: {
        id: "Containers.Channels.AddServiceLocationChannel.Step1.Organization.Tooltip",
        defaultMessage: "Valitse alasvetovalikosta organisaatio tai alaorganisaatio, joka vastaa palvelupisteestä. Mikäli palvelupiste on käytössä useilla alaorganisaatioilla, valitse pelkkä organisaation päätaso."
    },
    shortDescriptionLabel: {
        id: "Containers.Channels.AddServiceLocationChannel.Step1.ShortDescription.Title",
        defaultMessage: "Lyhyt kuvaus"
    },
    shortDescriptionInfo: {
        id: "Containers.Channels.AddServiceLocationChannel.Step1.ShortDescription.Tooltip",
        defaultMessage: "Kirjoita lyhyt kuvaus eli tiivistelmä palvelupisteestä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään palvelun."
    },
    shortDescriptionPlaceholder: {
        id: "Containers.Channels.AddServiceLocationChannel.Step1.ShortDescription.Placeholder",
        defaultMessage: "Kirjoita lyhyt tiivistelmä hakukoneita varten."
    },
	descriptionLabel: {
		id: "Containers.Channels.AddServiceLocationChannel.Step1.Description.Title",
		defaultMessage: "Kuvaus"
	},
    descriptionInfo: {
		id: "Containers.Channels.AddServiceLocationChannel.Step1.Description.Tooltip",
		defaultMessage: "Kuvaa palvelupiste mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kuvaa yleisesti, mitä asioita palvelupisteessä voi hoitaa. Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa vain palvelupistettä, älä palvelua järjestävää organisaatiota tai sen tehtäviä!"
	},
    descriptionPlaceholder: {
		id: "Containers.Channels.AddServiceLocationChannel.Step1.Description.Placeholder",
		defaultMessage: "Kirjoita selkeä ja ymmärrettävä kuvausteksti."
	},
});

export const restrictionRegionMessages = defineMessages({
    restrictedRegionTitle: {
        id: 'Containers.Channels.AddServiceLocationChannel.Step1.RestrictedRegion.Title',
        defaultMessage: 'Toimialue rajattu'
    },
    restrictedRegionTooltip: {
        id: 'Containers.Channels.AddServiceLocationChannel.Step1.RestrictedRegion.Tooltip',
        defaultMessage: 'Mikäli palvelupiste palvelee ainoastaan tiettyjen kuntien asukkaita tai sille on määritelty tarkka toimialue, valitse Kyllä. Tällaisia palvelupisteitä ovat esimerkiksi käräjäoikeudet. Käyttöliittymässä on oletusvalintana Ei-vaihtoehto.'
    },
    municipalitiesLabel: {
        id: 'Containers.Channels.AddServiceLocationChannel.Step1.Municipalities.Title',
        defaultMessage: 'Toimialueen kunnat'
    },
    municipalitiesTooltip: {
        id: 'Containers.Channels.AddServiceLocationChannel.Step1.Municipalities.Tooltip',
        defaultMessage: 'Mikäli palvelupisteen toimialue on rajattu, määrittele tässä toimialueen kunnat. Kirjoita kenttään haluamasi kunnan nimi ja valitse listasta. '
    },
    municipalitiesPlaceholder: {
        id: 'Containers.Channel.AddServiceLocationChannel.Step1.Municipalities.Placeholder',
        defaultMessage: 'Kirjoita ja valitse listasta kunnat'
    }
});

export const supportLanguageProvidedMessages = defineMessages({
    title: {
        id: 'Containers.Channels.AddServiceLocationChannel.Step1.Language.Title',
        defaultMessage: 'Kielet, joilla palvelupisteessä palvellaan'
    },
    tooltip: {
        id: 'Containers.Channels.AddServiceLocationChannel.Step1.Language.Tooltip',
        defaultMessage: 'Valitse tähän ne kielet, joilla palvelupisteessä palvellaan asiakkaita. Aloita kielen nimen kirjoittaminen, niin saat näkyviin kielilistan, josta voit valita kielet.'
    },
    placeholder: {
        id: 'Containers.Channels.AddServiceLocationChannel.Step1.Language.Placeholder',
        defaultMessage: 'Kirjoita ja valitse listasta kielet'
    }

});

export const emailMessages = defineMessages({
  title: {
    id: 'Containers.Channels.Common.Email.Title',
    defaultMessage: 'Sähköpostiosoite'
  },
  placeholder: {
    id: 'Containers.Channels.Common.Email.Placeholder',
    defaultMessage: 'Esim. osoite@organisaatio.fi'
  },
  // tooltip: {
  //   id: 'Containers.Channels.Common.Email.Tooltip',
  //   defaultMessage: `Kirjoita organisaation yleinen sähköpostiosoite, esim. kirjaamoon tai asiakaspalveluun.
  //                    Voit antaa useita sähköpostiosoitteita "uusi sähköpostiosoite" -painikkeella.`
  // },
  infoLabel: {
    id: 'Containers.Channels.Common.Email.InfoTitle',
    defaultMessage: 'Lisätieto'
  },
  infoPlaceholder: {
    id: 'Containers.Channels.Common.Email.InfoPlaceholder',
    defaultMessage: 'esim. Asiakaspalvelu'
  },
  infoTooltip: {
    id: 'Containers.Channels.Common.Email.InfoTooltip',
    defaultMessage: 'esim. Asiakaspalvelu'
  }
})

export const webPageMessages = defineMessages({
     title: {
        id: "Containers.Channels.AddPrintableFormChannel.Step2.WebPage.Section.Title",
        defaultMessage: "Lisää verkkosivu"
        },
     nameTitle: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Name.Title",
        defaultMessage: "Verkkosivun nimi"
        },
     nameTooltip: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Name.Tooltip",
        defaultMessage: "Anna palvelupisteen verkkosivuille havainnollinen nimi."
        },
     typeTitle: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Type.Title",
        defaultMessage: "Verkkosivun tyyppi"
        },
     typeTooltip: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Type.Tooltip",
        defaultMessage: "Valitse verkkosivun tyyppi (kotisivu tai sosiaalisen median palvelu)."
        },
     urlLabel: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Url.Title",
        defaultMessage: "Verkko-osoite"
        },
     urlTooltip: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Url.Tooltip",
        defaultMessage: "Syötä verkko-osoite, josta verkkoasiointikanava löytyy. Anna mahdollisimman tarkka verkko-osoite, josta verkkoasiointi avautuu, älä esimerkiksi organisaatiosi verkkoasioinnin etusivua. Kopioi ja liitä verkko-osoite ja  tarkista verkko-osoitteen toimivuus Testaa osoite -painikkeesta."
        },
     urlPlaceholder: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Url.Placeholder",
        defaultMessage: "Kopioi ja liitä tarkka verkko-osoite."
        },
     urlButton: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Url.Button.Title",
        defaultMessage: "Testaa osoite"
        },
     urlCheckerInfo: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Url.Icon.Tooltip",
        defaultMessage: "Verkko-osoitetta ei löytynyt, tarkista sen muoto."
     }
   });

export const faxMessages = defineMessages({
    title: {
        id: 'Containers.Channels.Common.FaxNumber.Title',
        defaultMessage: 'Faksinumero'
    },
    tooltip: {
      id: "Containers.Channels.Common.FaxNumber.Tooltip",
      defaultMessage: "To be defined..."
    },
    placeholder: {
        id: 'Containers.Channels.Common.FaxNumber.Placeholder',
        defaultMessage: 'esim. 451234567'
    },
    prefixTitle:{
         id: "Containers.Channels.Common.FaxPrefix.Title",
         defaultMessage:"Maan suuntanumero"
    },    
    prefixPlaceHolder:{
         id: "Containers.Channels.Common.FaxPrefix.Placeholder",
         defaultMessage:"esim. +358"
    },
    prefixTooltip: {
         id: "Containers.Channels.AddServiceLocationChannel.Step2.FaxPrefix.Tooltip",
         defaultMessage:"esim. +358"
    },
    finnishServiceNumberName:{
        id: "Containers.Channels.AddServiceLocationChannel.Step2.FaxPrefix.FinishServiceNumber.Name",
        defaultMessage: "Suomalainen palvelunumero"
     },
    chargeTypeTitle: {
      id: "Containers.Channels.Common.Number.PhoneCost.Title",
      defaultMessage: "Puhelun maksullisuus"
      },
    chargeTypeTooltip: {
      id: "Containers.Channels.Common.Number.PhoneCost.Tootltip",
      defaultMessage: 'Ensimmäinen vaihtoehto tarkoittaa, että puhelu maksaa lankaliittymästä soitettaessa paikallisverkkomaksun (pvm), matkapuhelimesta soitettaessa matkapuhelinmaksun (mpm) tai ulkomailta soitettaessa ulkomaanpuhelumaksun. Valitse "täysin maksuton" vain, jos puhelu on soittajalle kokonaan maksuton. Jos puhelun maksu muodostuu muulla tavoin, valitse "muu maksu" ja kuvaa puhelun hintatiedot omassa kentässään.'
      },
    phoneCostAllCosts: {
      id: "Containers.Channels.Common.Number.PhoneAllCosts.Title",
      defaultMessage: "Paikallisverkkomaksu (pvm), Matkapuhelinmaksu (mpm), Ulkomaanpuhelumaksu"
      },
    phoneCostFree: {
      id: "Containers.Channels.Common.Number.PhoneCostFree.Title",
      defaultMessage: "Täysin maksuton"
      },
    phoneCostOther: {
      id: "Containers.Channels.Common.Number.PhoneCostOther.Title",
      defaultMessage: "Muu maksu, anna tarkemmat tiedot:"
      },
    costDescriptionTitle: {
      id: "Containers.Channels.Common.Number.PhoneCostOtherDescription.Title",
      defaultMessage: "Puhelun hintatiedot"
      },
    costDescriptionPlaceholder: {
      id: "Containers.Channels.Common.Number.PhoneCostOtherDescription.Placeholder",
      defaultMessage: "esim. Pvm:n lisäksi jonotuksesta veloitetaan..."
      },
    costDescriptionTooltip: {
      id: "Containers.Channels.Common.Number.PhoneCostOtherDescription.Tooltip",
      defaultMessage: "Kerro sanallisesti, mitä puhelinnumeroon soittaminen maksaa."
      },
    infoTitle:{
      id: "Containers.Channels.Common.Number.AdditionalInformation.Title",
      defaultMessage: "Lisätieto"
    },
    infoTooltip:{
      id: "Containers.Channels.Common.Number.AdditionalInformation.Tooltip",
      defaultMessage: "Voit tarvittaessa antaa puhelinnumerolle lisätiedon, jolla voit tarkentaa, mistä numerosta on kyse. Esimerkiksi vaihde tai asiakaspalvelu."
    },
    infoPlaceholder:{
      id: "Containers.Channels.Common.Number.AdditionalInformation.Placeholder",
      defaultMessage:"esim. Vaihde"
    },
});

export const phoneNumberMessages = defineMessages({
     title: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.PhoneNumber.Title",
        defaultMessage: "Puhelinnumero"
        },
     tooltip: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.PhoneNumber.Tooltip",
        defaultMessage: "Kirjoita puhelinnumero kansainvälisessä muodossa ilman välilyöntejä (esim. +358451234567)."
        },
     placeholder: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.PhoneNumber.PlaceHolder",
        defaultMessage: "esim. +358451234567"
        },
     chargeTypeTitle: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.PhoneCost.Title",
        defaultMessage: "Puhelun maksullisuus"
        },
     chargeTypeTooltip: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.PhoneCost.Tootltip",
        defaultMessage: 'Ensimmäinen vaihtoehto tarkoittaa, että puhelu maksaa lankaliittymästä soitettaessa paikallisverkkomaksun (pvm), matkapuhelimesta soitettaessa matkapuhelinmaksun (mpm) tai ulkomailta soitettaessa ulkomaanpuhelumaksun. Valitse "täysin maksuton" vain, jos puhelu on soittajalle kokonaan maksuton. Jos puhelun maksu muodostuu muulla tavoin, valitse "muu maksu" ja kuvaa puhelun hintatiedot omassa kentässään.'
        },
     phoneCostAllCosts: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.PhoneAllCosts.Title",
        defaultMessage: "Paikallisverkkomaksu (pvm), Matkapuhelinmaksu (mpm), Ulkomaanpuhelumaksu"
        },
     phoneCostFree: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.PhoneCostFree.Title",
        defaultMessage: "Täysin maksuton"
        },
     phoneCostOther: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.PhoneCostOther.Title",
        defaultMessage: "Muu maksu, anna tarkemmat tiedot:"
        },
     costDescriptionTitle: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.PhoneCostOtherDescription.Title",
        defaultMessage: "Puhelun hintatiedot"
        },
     costDescriptionPlaceholder: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.PhoneCostOtherDescription.Placeholder",
        defaultMessage: "esim. Pvm:n lisäksi jonotuksesta veloitetaan..."
        },
     costDescriptionTooltip: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.PhoneCostOtherDescription.Tooltip",
        defaultMessage: "Kerro sanallisesti, mitä puhelinnumeroon soittaminen maksaa."
        },
     infoTitle:{
        id: "Containers.Channels.AddServiceLocationChannel.Step2.AdditionalInformation.Title",
        defaultMessage: "Lisätieto"
     },
     infoTooltip:{
        id: "Containers.Channels.AddServiceLocationChannel.Step2.AdditionalInformation.Tooltip",
        defaultMessage: "Voit tarvittaessa antaa puhelinnumerolle lisätiedon, jolla voit tarkentaa, mistä numerosta on kyse. Esimerkiksi vaihde tai asiakaspalvelu."
     },
     infoPlaceholder:{
         id: "Containers.Channels.AddServiceLocationChannel.Step2.AdditionalInformation.Placeholder",
         defaultMessage:"esim. Vaihde"
     },
     prefixTitle:{
         id: "Containers.Channels.AddServiceLocationChannel.Step2.PhonePrefix.Title",
         defaultMessage:"Maan suuntanumero"
     },
     prefixPlaceHolder:{
         id: "Containers.Channels.AddServiceLocationChannel.Step2.PhonePrefix.Placeholder",
         defaultMessage:"esim. +358"
     },
     prefixTooltip: {
         id: "Containers.Channels.AddServiceLocationChannel.Step2.PhonePrefix.Tooltip",
         defaultMessage:"esim. +358"
     }, 
     finnishServiceNumberName:{
        id: "Containers.Channels.AddServiceLocationChannel.Step2.FinishServiceNumber.Name",
        defaultMessage: "Suomalainen palvelunumero"
     }   
});

export const addressMessages = defineMessages({
    addDifferentMail: {
        id: "Containers.Channel.AddServiceLocationChannel.Step3.Address.PostalAddressSelected.Checkbox",
        defaultMessage: "Lisää postiosoite (eri kuin käyntiosoite)"
    }
});

export const openingHoursMessages = defineMessages({
    collapsedInfo: {
        id: "Containers.Channels.AddServiceLocationChannel.Step2.OpeningHours.CollapsedInfo",
        defaultMessage: "Muokattu"
    },
    mainTooltipNormal: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.MainTooltipNormal",
        defaultMessage: "This is a tooltip for NOH"
    },
    mainLabelNormal: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.MainLabelNormal",
        defaultMessage: "Normaali aukioloaika"
    },
    defaultTitle_openingHoursNormal: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.DefaultTitleNormal",
        defaultMessage: "Normaaliaukioloajat"
    },
    validOnward: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.ValidityType.Onward",
        defaultMessage: "Toistaiseksi voimassa oleva"
    },
    validPeriod: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.ValidityType.Period",
        defaultMessage: "Voimassa ajanjaksolla"
    },
    additionalInformation: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.AdditionalInformation.Title",
        defaultMessage: "Lisätieto"
    },
    nonstopOpeningHours: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.Nonstop.Title",
        defaultMessage: "Joka päivä ympäri vuorokauden käytettävissä"
    },
    startDate: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.StartDate.Title",
        defaultMessage: "Alkaa"
    },
    endDate: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.EndDate.Title",
        defaultMessage: "Päättyy"
    },
    mainTooltipSpecial: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.MainTooltipSpecial",
        defaultMessage: "This is a tooltip for SOH"
    },
    mainLabelSpecial: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.MainLabelSpecial",
        defaultMessage: "Vuorokauden yli menevät ajat"
    },
    defaultTitle_openingHoursSpecial: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.DefaultTitleSpecial",
        defaultMessage: "Aukioloaikojen täyttämisohjeet"
    },
    previewTitle: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.PreviewTitle",
        defaultMessage: "Esikatselu"
    },
    previewTooltip: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.PreviewTooltip",
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
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.Monday",
        defaultMessage: "Maanantai"
    },
    weekday_tu: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.Tuesday",
        defaultMessage: "Tiistai"
    },
    weekday_we: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.Wednesday",
        defaultMessage: "Keskiviikko"
    },
    weekday_th: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.Thursday",
        defaultMessage: "Torstai"
    },
    weekday_fr: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.Friday",
        defaultMessage: "Perjantai"
    },
    weekday_sa: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.Saturday",
        defaultMessage: "Lauantai"
    },
    weekday_su: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.Sunday",
        defaultMessage: "Sunnuntai"
    },
    mainTooltipExceptional: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.MainTooltipExceptional",
        defaultMessage: "This is a tooltip for EOH"
    },
    mainLabelExceptional: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.MainLabelExceptional",
        defaultMessage: "Poikkeusaukioloaika"
    },
    defaultTitle_openingHoursExceptional: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.DefaultTitleExceptional",
        defaultMessage: "Poikkeusaukioloajat"
    },
    validDaySingle: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.ValidityType.DaySingle",
        defaultMessage: "Päivä"
    },
    validDayRange: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.ValidityType.DayRange",
        defaultMessage: "Ajanjakso"
    },
    closedDaySingle: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.ValidityType.ClosedDaySingle",
        defaultMessage: "Suljettu koko päivän"
    },
    closedDayRange: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.ValidityType.ClosedDayRange",
        defaultMessage: "Suljettu koko ajanjakso"
    },
    closedMessage: {
        id: "Containers.Channels.AddServiceLocationChannel.Step4.OpeningHours.ValidityType.ClosedMessage",
        defaultMessage: "Suljettu"
    }
});
