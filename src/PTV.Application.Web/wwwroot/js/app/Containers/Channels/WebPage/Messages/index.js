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
        id: "Containers.Channels.AddWebPageChannel.Header.Title",
        defaultMessage: "Lisää uusi kanava: verkkosivu"
    },
    mainTitleView: {
        id: "Containers.Channels.AddWebPageChannel.Header.Title.View",
        defaultMessage: "Lisää uusi kanava: verkkosivu"
    },
    mainText: {
        id: "Containers.Channels.AddWebPageChannel.Header.Description",
        defaultMessage: "Verkkosivu on kanava, jossa annetaan lisätietoa palvelusta. Verkkosivu voi olla linkki organisaation kotisivujen tietylle alasivulle tai palvelua koskevaan ohjeeseen tai se voi olla myös linkki esimerkiksi chattipalveluun. Verkkosivulta saa lisätietoa ja neuvontaa palvelun käyttöön ottamiseen, mutta siellä ei ole mahdollista aloittaa asiointiprosessia tai panna asiaa vireille."
    },
    mainTextView: {
        id: "Containers.Channels.ViewWebPageChannel.Header.Description",
        defaultMessage: "Tällä sivulla voit katsella, muokata ja julkaista asiointikanavan tietoja."
    },
    subTitle1: {
        id: "Containers.Channels.AddWebPageChannel.Step1.Header.Title",
        defaultMessage: "Vaihe 1/1: Perustiedot"
    },
    subTitle1View: {
        id: "Containers.Channels.AddWebPageChannel.Step1.Header.Title.View",
        defaultMessage: "Vaihe 1/1: Perustiedot"
    },
    supportTitle: {
        id: "Containers.Channels.AddWebPageChannel.Step1.Support.Title",
        defaultMessage: "Käytön tuki"
    }
});

export const deleteMessages = defineMessages({
    text: {
        id: "Containers.Channels.WebPageChannel.Delete.Text",
        defaultMessage: "Oletko varma, että haluat poistaa kanavakuvauksen?"
    },
    buttonOk: {
        id: "Containers.Channels.WebPageChannel.Delete.Accept",
        defaultMessage: "Kyllä"
    },
    buttonCancel: {
        id: "Containers.Channels.WebPageChannel.Delete.Cancel",
        defaultMessage: "Jatka muokkausta"
    }
});

export const withdrawMessages = defineMessages({
  text: {
    id: 'Containers.Channels.WebPageChannel.Withdraw.Text',
    defaultMessage: 'Are you sure, you want to withdraw web page channel?'
  },
  buttonOk: {
    id: 'Containers.Channels.WebPageChannel.Withdraw.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Channels.WebPageChannel.Withdraw.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const restoreMessages = defineMessages({
  text: {
    id: 'Containers.Channels.WebPageChannel.Restore.Text',
    defaultMessage: 'Are you sure, you want to restore web page channel?'
  },
  buttonOk: {
    id: 'Containers.Channels.WebPageChannel.Restore.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Channels.WebPageChannel.Restore.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

export const languageMessages = defineMessages({
    title: {
        id: "Containers.Channels.AddWebPageChannel.Step1.LanguageProvided.Title",
        defaultMessage: "Kielet, joilla verkkosivu on saatavilla"
        },
    tooltip: {
        id: "Containers.Channels.AddWebPageChannel.Step1.LanguageProvided.Tooltip",
        defaultMessage: "Valitse tähän ne kielet, joilla verkkosivu tarjotaan asiakkaalle. Aloita kielen nimen kirjoittaminen, niin saat näkyviin kielilistan, josta voit valita kielet."
        },
    placeholder: {
        id: "Containers.Channels.AddWebPageChannel.Step1.LanguageProvided.Placeholder",
        defaultMessage: "Kirjoita ja valitse listasta verkkosivun kielet."
        },
});

export const cancelMessages = defineMessages({
    text: {
        id: "Containers.Channels.WebPageChannel.Cancel.Text",
        defaultMessage: "Oletko varma, että haluat keskeyttää uuden kanavan luonnin? Tekemäsi muutokset katoavat."
    },
    buttonOk: {
        id: "Containers.Channels.WebPageChannel.Cancel.Accept",
        defaultMessage: "Kyllä"
    },
    buttonCancel: {
        id: "Containers.Channels.WebPageChannel.Cancel.Cancel",
        defaultMessage: "Jatka muokkausta"
    }
});

export const channelDescriptionMessages = defineMessages({
    nameTitle: {
        id: "Containers.Channels.AddWebPageChannel.Step1.Name.Title",
        defaultMessage: "Nimi"
    },
    namePlaceholder: {
        id: "Containers.Channels.AddWebPageChannel.Step1.Name.Placeholder",
        defaultMessage: "Kirjoita verkkosivukanavaa kuvaava, asiakaslähtöinen nimi."
    },
	organizationLabel: {
        id: "Containers.Channels.AddWebPageChannel.Step1.Organization.Title",
        defaultMessage: "Organisaatio"
    },
    organizationInfo: {
        id: "Containers.Channels.AddWebPageChannel.Step1.Organization.Tooltip",
        defaultMessage: "Valitse alasvetovalikosta organisaatio tai alaorganisaatio, joka vastaa verkkosivusta. Mikäli verkkosivusta vastaa usempi alaorganisaatio, valitse pelkkä organisaation päätaso."
    },
    shortDescriptionLabel: {
        id: "Containers.Channels.AddWebPageChannel.Step1.ShortDescription.Title",
        defaultMessage: "Lyhyt kuvaus"
    },
    shortDescriptionInfo: {
        id: "Containers.Channels.AddWebPageChannel.Step1.ShortDescription.Tooltip",
        defaultMessage: "Kirjoita lyhyt kuvaus eli tiivistelmä verkkosivun keskeisestä sisällöstä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään asiointikanavan."
    },
    shortDescriptionPlaceholder: {
        id: "Containers.Channels.AddWebPageChannel.Step1.ShortDescription.Placeholder",
        defaultMessage: "Kirjoita lyhyt tiivistelmä hakukoneita varten.  "
    },
	descriptionLabel: {
		id: "Containers.Channels.AddWebPageChannel.Step1.Description.Title",
		defaultMessage: "Kuvaus"
	},
    descriptionInfo: {
		id: "Containers.Channels.AddWebPageChannel.Step1.Description.Tooltip",
		defaultMessage: "Kuvaa verkkosivun sisältö mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kuvaa mistä asiasta verkkosivu tarjoaa lisätietoa ja miten sitä voi käyttää (erityisesti, jos kyseessä on esim. demo, chat tai etäpalvelu). Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa vain verkkosivukanavaa, älä palvelua järjestävää organisaatiota tai sen tehtäviä!"
	},
    descriptionPlaceholder: {
		id: "Containers.Channels.AddWebPageChannel.Step1.Description.Placeholder",
		defaultMessage: "Kirjoita selkeä ja ymmärrettävä kuvausteksti."
	},
});

export const emailMessages = defineMessages({
     title: {
        id: "Containers.Channels.AddWebPageChannel.Step1.Email.Title",
        defaultMessage: "Sähköposti"
        },
     tooltip: {
        id: "Containers.Channels.AddWebPageChannel.Step1.Email.Tooltip",
        defaultMessage: "Mikäli tulostettavan lomakkeen käyttöön on mahdollista saada tukea sähköpostitse, kirjoita kenttään tukipalvelun sähköpostiosoite. Älä kirjoita kenttään organisaation yleisiä sähköpostiosoitteita, esim. kirjaamoon. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi."
        },
     placeholder: {
        id: "Containers.Channels.AddWebPageChannel.Step1.Email.Placeholder",
        defaultMessage: "Esim. osoite@organisaatio.fi"
        },
   });

export const phoneNumberMessages = defineMessages({    
     title: {
        id: "Containers.Channels.AddWebPageChannel.Step1.PhoneNumber.Title",
        defaultMessage: "Puhelinnumero"
        },
     tooltip: {
        id: "Containers.Channels.AddWebPageChannel.Step1.PhoneNumber.Tooltip",
        defaultMessage: "Mikäli verkkosivukanavan käyttöön on mahdollista saada tukea puhelimitse, kirjoita kenttään tukipalvelun puhelinnumero. Kirjoita puhelinnumero kansainvälisessä muodossa ilman välilyöntejä (esim. +358451234567). Älä kirjoita kenttään organisaation yleisiä puhelinnumeroita, esim. vaihteen numeroa. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi."
        },
     placeholder: {
        id: "Containers.Channels.AddWebPageChannel.Step1.PhoneNumber.PlaceHolder",
        defaultMessage: "esim. +35845123467"
        },
     chargeTypeTitle: {
        id: "Containers.Channels.AddWebPageChannel.Step1.PhoneCost.Title",
        defaultMessage: "Puhelun maksullisuus"
        },
     chargeTypeTooltip: {
        id: "Containers.Channels.AddWebPageChannel.Step1.PhoneCost.Tootltip",
        defaultMessage: 'Ensimmäinen vaihtoehto tarkoittaa, että puhelu maksaa lankaliittymästä soitettaessa paikallisverkkomaksun (pvm), matkapuhelimesta soitettaessa matkapuhelinmaksun (mpm) tai ulkomailta soitettaessa ulkomaanpuhelumaksun.Valitse "täysin maksuton" vain, jos puhelu on soittajalle kokonaan maksuton. Jos puhelun maksu muodostuu muulla tavoin, valitse "muu maksu" ja kuvaa puhelun hintatiedot omassa kentässään.'
        },
     phoneCostAllCosts: {
        id: "Containers.Channels.AddWebPageChannel.Step1.PhoneAllCosts.Title",
        defaultMessage: "Paikallisverkkomaksu (pvm), Matkapuhelinmaksu (mpm), Ulkomaanpuhelumaksu"
        },
     phoneCostFree: {
        id: "Containers.Channels.AddWebPageChannel.Step1.PhoneCostFree.Title",
        defaultMessage: "Täysin maksuton"
        },
     phoneCostOther: {
        id: "Containers.Channels.AddWebPageChannel.Step1.PhoneCostOther.Title",
        defaultMessage: "Muu maksu, anna tarkemmat tiedot:"
        },
     costDescriptionTitle: {
        id: "Containers.Channels.AddWebPageChannel.Step1.PhoneCostOtherDescription.Title",
        defaultMessage: "Puhelun hintatiedot"
        },
     costDescriptionPlaceholder: {
        id: "Containers.Channels.AddWebPageChannel.Step1.PhoneCostOtherDescription.Placeholder",
        defaultMessage: "esim. Pvm:n lisäksi jonotuksesta veloitetaan..."
        },
     costDescriptionTooltip: {
        id: "Containers.Channels.AddWebPageChannel.Step1.PhoneCostOtherDescription.Tooltip",
        defaultMessage: "Kerro sanallisesti, mitä puhelinnumeroon soittaminen maksaa."
        },
    infoTitle:{
        id: "Containers.Channels.AddWebPageChannel.Step1.AdditionalInformation.Title",
        defaultMessage: "Lisätieto"
        },
    infoTooltip:{
        id: "Containers.Channels.AddWebPageChannel.Step1.AdditionalInformation.Tooltip",
        defaultMessage: "Voit tarvittaessa antaa puhelinnumerolle lisätiedon, jolla voit tarkentaa, mistä numerosta on kyse. Esimerkiksi vaihde tai asiakaspalvelu."
        },
    infoPlaceholder:{
        id: "Containers.Channels.AddWebPageChannel.Step1.AdditionalInformation.Placeholder",
        defaultMessage:"esim. Vaihde"
        },
    prefixTitle:{
        id: "Containers.Channels.AddWebPageChannel.Step1.PhonePrefix.Title",
        defaultMessage:"Maan suuntanumero"
        },
    prefixPlaceHolder:{
        id: "Containers.Channels.AddWebPageChannel.Step1.PhonePrefix.Placeholder",
        defaultMessage:"esim. +358"
        },
    prefixTooltip:{
        id: "Containers.Channels.AddWebPageChannel.Step1.PhonePrefix.Tooltip",
        defaultMessage:"esim. +358"
        },
    finnishServiceNumberName: {
        id: "Containers.Channels.AddWebPageChannel.Step1.FinishServiceNumber.Name",
        defaultMessage: "Suomalainen palvelunumero"
    }
});

export const urlMessages = defineMessages({
    label: {
        id: "Containers.Channels.AddWebPageChannel.Step1.UrlChecker.Title",
        defaultMessage: "Verkko-osoite"
        },
    tooltip: {
        id: "Containers.Channels.AddWebPageChannel.Step1.UrlChecker.Tooltip",
        defaultMessage: "Syötä verkko-osoite, josta verkkoasiointikanava löytyy. Anna mahdollisimman tarkka verkko-osoite, josta verkkoasiointi avautuu, älä esimerkiksi organisaatiosi verkkoasioinnin etusivua. Kopioi ja liitä verkko-osoite ja  tarkista verkko-osoitteen toimivuus Testaa osoite -painikkeesta."
        },
    placeholder: {
        id: "Containers.Channels.AddWebPageChannel.Step1.UrlChecker.Placeholder",
        defaultMessage: "Kopioi ja liitä tarkka verkko-osoite."
        },
    button: {
        id: "Containers.Channels.AddWebPageChannel.Step1.UrlChecker.Button.Title",
        defaultMessage: "Testaa osoite"
        },
    checkerInfo: {
        id: "Containers.Channels.AddWebPageChannel.Step1.UrlChecker.Icon.Tooltip",
        defaultMessage: "Verkko-osoitetta ei löytynyt, tarkista sen muoto."
        },
})
