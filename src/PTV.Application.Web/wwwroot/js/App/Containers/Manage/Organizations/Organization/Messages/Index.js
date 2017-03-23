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

export const deleteMessages = defineMessages({
    text: {
        id: "Containers.Manage.Organizations.Manage.Delete.Text",
        defaultMessage: "Oletko varma, että haluat keskeyttää uuden organisaation luonnin? Tekemäsi muutokset katoavat."
    },
    buttonOk: {
        id: "Containers.Manage.Organizations.Manage.Delete.Accept",
        defaultMessage: "Kyllä"
    },
    buttonCancel: {
        id: "Containers.Manage.Organizations.Manage.Delete.Cancel",
        defaultMessage: "Jatka muokkausta"
    }
});

export const messages = defineMessages({
    mainTitle: {
        id: "Containers.Manage.Organizations.Manage.Main.Title",
        defaultMessage: "Uusi organisaatio"
    },
    mainTitleView: {
        id: "Containers.Manage.Organizations.Manage.Main.Title.View",
        defaultMessage: "Organisaation rakenteen katselu ja muokkaus"
    },
    mainDescription: {
        id: "Containers.Manage.Organizations.Manage.Main.Description",
        defaultMessage: "Missing"
    },
    mainDescriptionView: {
        id: "Containers.Manage.Organizations.Manage.Main.Description.View",
        defaultMessage: "Ohje käytöstä tähän. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."
    },
    subTitleStep1: {
        id: "Containers.Manage.Organizations.Manage.Main.SubTitle.Step1",
        defaultMessage: "Vaihe 1/1: Perustiedot"
    },
    subTitleViewStep1: {
        id: "Containers.Manage.Organizations.Manage.Main.SubTitle.View.Step1",
        defaultMessage: "Vaihe 1/1: Perustiedot"
    },
});

export const cancelMessages = defineMessages({
    text: {
        id: "Containers.Organization.AddOrganization.Cancel.Text",
        defaultMessage: "Oletko varma, että haluat keskeyttää uuden kanavan luonnin? Tekemäsi muutokset katoavat."
    },
    buttonOk: {
        id: "Containers.Organization.AddOrganization.Cancel.Accept",
        defaultMessage: "Kyllä"
    },
    buttonCancel: {
        id: "Containers.Organization.AddOrganization.Cancel.Cancel",
        defaultMessage: "Jatka muokkausta"
    }
});

export const saveDraftMessages = defineMessages({
    text: {
        id: "Containers.Organization.ViewOrganization.SaveDraft.Text",
        defaultMessage: "Luonnos on tallennettu. Voit julkaista palvelun sivulla olevasta Julkaise painikkeesta tai jatkaa Palvelut -sivulle."
    },
    buttonOk: {
        id: "Containers.Organization.ViewOrganization.SaveDraft.Accept",
        defaultMessage: "Ok"
    },
    buttonCancel: {
        id: "Containers.Organization.ViewOrganization.SaveDraft.Cancel",
        defaultMessage: "Palaa Palvelut -sivulle"
    }
});


export const emailMessages = defineMessages({
     title: {
        id: "Containers.Manage.Organizations.Manage.Step2.Email.Title",
        defaultMessage: "Sähköpostiosoite"
        },
     placeholder: {
        id: "Containers.Manage.Organizations.Manage.Step2.Email.Placeholder",
        defaultMessage: "esim. asiakaspalvelu@yritys.fi"
        },
     tooltip: {
        id: "Containers.Manage.Organizations.Manage.Step2.Email.Tooltip",
        defaultMessage: "Kirjoita organisaation yleinen sähköpostiosoite, esim. kirjaamoon tai asiakaspalveluun. Voit antaa useita sähköpostiosoitteita 'uusi sähköpostiosoite' -painikkeella."
        },
     infoLabel: {
        id: "Containers.Manage.Organizations.Manage.Step2.Email.InfoTitle",
        defaultMessage: "Lisätieto"
        },
     infoPlaceholder: {
        id: "Containers.Manage.Organizations.Manage.Step2.Email.InfoPlaceholder",
        defaultMessage: "esim. Asiakaspalvelu"
        },
     infoTooltip: {
        id: "Containers.Manage.Organizations.Manage.Step2.Email.InfoTooltip",
        defaultMessage: "esim. Asiakaspalvelu"
        }
   });

export const phoneNumberMessages = defineMessages({
     title: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneNumber.Title",
        defaultMessage: "Puhelinnumero"
        },
     tooltip: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneNumber.Tooltip",
        defaultMessage: 'Kirjoita puhelinnumero kansainvälisessä muodossa. Kirjoita ensin maan suuntanumero ja anna kansallinen numero ilman alkunollaa. Jos numeroon ei voi soittaa ulkomailta, voit antaa numeron ilman ulkomaansuuntanumeroa. Voit antaa useita puhelinnumeroita "uusi puhelinnumero" -painikkeella.'
        },
     placeholder: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneNumber.PlaceHolder",
        defaultMessage: "esim. 451234567 (numero ilman alkunollaa)"
        },
     chargeTypeTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneCost.Title",
        defaultMessage: "Puhelun maksullisuus"
        },
     chargeTypeTooltip: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneCost.Tootltip",
        defaultMessage: 'Ensimmäinen vaihtoehto tarkoittaa, että puhelu maksaa lankaliittymästä soitettaessa paikallisverkkomaksun (pvm), matkapuhelimesta soitettaessa matkapuhelinmaksun (mpm) tai ulkomailta soitettaessa ulkomaanpuhelumaksun. Valitse "täysin maksuton" vain, jos puhelu on soittajalle kokonaan maksuton. Jos puhelun maksu muodostuu muulla tavoin, valitse "muu maksu" ja kuvaa puhelun hintatiedot omassa kentässään.'
        },
     phoneCostAllCosts: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneAllCosts.Title",
        defaultMessage: "Paikallisverkkomaksu (pvm), Matkapuhelinmaksu (mpm), Ulkomaanpuhelumaksu"
        },
     phoneCostFree: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneCostFree.Title",
        defaultMessage: "Täysin maksuton"
        },
     phoneCostOther: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneCostOther.Title",
        defaultMessage: "Muu maksu, anna tarkemmat tiedot:"
        },
     costDescriptionTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneCostOtherDescription.Title",
        defaultMessage: "Puhelun hintatiedot"
        },
     costDescriptionPlaceholder: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneCostOtherDescription.Placeholder",
        defaultMessage: "esim. Pvm:n lisäksi jonotuksesta veloitetaan..."
        },
     costDescriptionTooltip: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneCostOtherDescription.Tooltip",
        defaultMessage: "Kerro sanallisesti, mitä puhelinnumeroon soittaminen maksaa."
        },
      infoTitle:{
        id: "Containers.Manage.Organizations.Manage.Step1.AdditionalInformation.Title",
        defaultMessage: "Lisätieto"
        },
      infoTooltip:{
        id: "Containers.Manage.Organizations.Manage.Step1.AdditionalInformation.Tooltip",
        defaultMessage: "Voit tarvittaessa antaa puhelinnumerolle lisätiedon, jolla voit tarkentaa, mistä numerosta on kyse. Esimerkiksi vaihde tai asiakaspalvelu."
        },
      infoPlaceholder:{
        id: "Containers.Manage.Organizations.Manage.Step1.AdditionalInformation.Placeholder",
        defaultMessage:"esim. Vaihde"
        },
      prefixTitle:{
        id: "Containers.Manage.Organizations.Manage.Step1.PhonePrefix.Title",
        defaultMessage:"Maan suuntanumero"
        },
      prefixPlaceHolder:{
        id: "Containers.Manage.Organizations.Manage.Step1.PhonePrefix.Placeholder",
        defaultMessage:"esim. +358"
        },
      prefixTooltip:{
        id: "Containers.Manage.Organizations.Manage.Step1.PhonePrefix.Tooltip",
        defaultMessage:"esim. +358"
        },
      phoneTypesTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneTypes.Title",
        defaultMessage: "Puhelinkanavan tyyppi"
        },
     phoneTypesTooltip: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneTypes.Tooltip",
        defaultMessage: "Puhelinkanavan tyyppi"
        },
     finnishServiceNumberName: {
        id: "Containers.Manage.Organizations.Manage.Step1.PhoneTypes.FinishServiceNumber.Name",
        defaultMessage: "Suomalainen palvelunumero"
        }     
});

export const webPageMessages = defineMessages({
    title: {
        id: "Containers.Manage.Organizations.Manage.Step2.WebPageSection.Title",
        defaultMessage: "Lisää verkkosivu"
        },
    nameTitle: {
        id: "Containers.Manage.Organizations.Manage.Step2.WebPage.Name.Title",
        defaultMessage: "Verkkosivun nimi"
        },
    nameTooltip: {
        id: "Containers.Manage.Organizations.Manage.Step2.WebPage.Name.Tooltip",
        defaultMessage: "Anna palvelupisteen verkkosivuille havainnollinen nimi."
        },
    typeTitle: {
        id: "Containers.Manage.Organizations.Manage.Step2.WebPage.Type.Title",
        defaultMessage: "Tiedostomuoto"
        },
    typeTooltip: {
        id: "Containers.Manage.Organizations.Manage.Step2.WebPage.Type.Tooltip",
        defaultMessage: "Valitse alasvetovalikosta lomakkeen tiedostomuoto. Jos lomakkeesta on tarjolla useampi eri tiedostomuoto, vie kukin tiedostomuoto erikseen."
        },
     urlLabel: {
        id: "Containers.Manage.Organizations.Manage.Step2.WebPage.Url.Title",
        defaultMessage: "Verkko-osoite"
        },
     urlTooltip: {
        id: "Containers.Manage.Organizations.Manage.Step2.WebPage.Url.Tooltip",
        defaultMessage: "Anna verkkosivun osoite muodossa www.suomi.fi. Kun klikkaat Lisää uusi -painiketta, osoitteen alkuun lisätään http:// automaattisesti. Voit lisätä useita verkkosivuja yksi kerrallaan Lisää uusi -painikkeella."
        },
     urlPlaceholder: {
        id: "Containers.Manage.Organizations.Manage.Step2.WebPage.Url.Placeholder",
        defaultMessage: "Kopioi ja liitä tarkka verkko-osoite."
        },
     urlButton: {
        id: "Containers.Manage.Organizations.Manage.Step2.WebPage.Url.Button.Title",
        defaultMessage: "Testaa osoite"
        },
     urlCheckerInfo: {
        id: "Containers.Manage.Organizations.Manage.Step2.WebPage.Url.Icon.Tooltip",
        defaultMessage: "Verkko-osoitetta ei löytynyt, tarkista sen muoto."
     },
     orderTitle: {
        id: "Containers.Manage.Organizations.Manage.Step2.WebPage.Order.Title",
        defaultMessage: "Esitysjärjestys"
     }
});

export const groupLevelContainerMessages = defineMessages({
    organizationLevelTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.Level.Title",
        defaultMessage: "Organisaatiotaso * Valitse taso, jonka alle organisaatio lisätään."
    },
    organizationLevelMainOrganization: {
        id: "Containers.Manage.Organizations.Manage.Step1.Level.MainOrganization.Title",
        defaultMessage: "Pääorganisaatio"
    },
    organizationLevelSubOrganization: {
        id: "Containers.Manage.Organizations.Manage.Step1.Level.SubOrganization.Title",
        defaultMessage: "Alaorganisaatio"
    },
    organizationMainOrganizationTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.Level.MainOrganization.Select.Title",
        defaultMessage: "Organisaatio"
    },
    organizationMainOrganizationPlaceholder: {
        id: "Containers.Manage.Organizations.Manage.Step1.Level.MainOrganization.Select.Placeholder",
        defaultMessage: "Kirjoita organisaation nimi"
    }
});

export const typeMunicipalityContainerMessages = defineMessages({

    organizationLevelTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.Level.Title",
        defaultMessage: "Organisaatiotaso * Valitse taso, jonka alle organisaatio lisätään."
    },
    organizationLevelMainOrganization: {
        id: "Containers.Manage.Organizations.Manage.Step1.Level.MainOrganization.Title",
        defaultMessage: "Pääorganisaatio"
    },
    organizationLevelSubOrganization: {
        id: "Containers.Manage.Organizations.Manage.Step1.Level.SubOrganization.Title",
        defaultMessage: "Alaorganisaatio"
    },
    organizationMainOrganizationTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.Level.MainOrganization.Select.Title",
        defaultMessage: "Organisaatio"
    },
    organizationMainOrganizationPlaceholder: {
        id: "Containers.Manage.Organizations.Manage.Step1.Level.MainOrganization.Select.Placeholder",
        defaultMessage: "Kirjoita organisaation nimi"
    },
    organizationTypeTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.Organization.Type.Title",
        defaultMessage: "Organisaatiotyyppi"
    },
    municipalityNameTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.Municpality.Name.Title",
        defaultMessage: "Kuntanimi"
    },
    municipalityNamePlaceholder: {
        id: "Containers.Manage.Organizations.Manage.Step1.Municpality.Name.Placeholder",
        defaultMessage: "Kirjoita kunnan nimi"
    },
    municipalityNameTooltip: {
        id: "Containers.Manage.Organizations.Manage.Step1.Municpality.Name.Tooltip",
        defaultMessage: "Kirjoita kunnan nimi."
    },
    municipalityNumberTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.Municpality.Number.Title",
        defaultMessage: "Kuntanumero"
    }
});

export const messagesDescriptionContainer = defineMessages({

    organizationNameTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.Organization.Name.Title",
        defaultMessage: "Organisaation nimi"
    },
    organizationNamePlaceholder: {
        id: "Containers.Manage.Organizations.Manage.Step1.Organization.Name.Placeholder",
        defaultMessage: "Kirjoita organisaation tai alaorganisaation nimi"
    },
    organizationAlternativeNameTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.Organization.AlternativeName.Title",
        defaultMessage: "Vaihtoehtoinen nimi"
    },
    organizationAlternativeNumberTooltip: {
        id: "Containers.Manage.Organizations.Manage.Step1.Organization.AlternativeNumber.Tooltip",
        defaultMessage: "esim. Kela"
    },
    businessIdTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.Organization.BusinessId.Title",
        defaultMessage: "Y-tunnus"
    },
    businessIdTooltip: {
        id: "Containers.Manage.Organizations.Manage.Step1.Organization.BusinessId.Tooltip",
        defaultMessage: "Kirjota kenttään organisaatiosi Y-tunnus. Jos et muista sitä, voit hakea Y-tunnuksen Yritys- ja yhteisötietojärjestelmästä (YTJ) [https://www.ytj.fi/yrityshaku.aspx?path=1547;1631;1678&kielikoodi=1]. | Tarkista oman organisaatiosi Y-tunnuksen käytön käytäntö: Joillain organisaatioilla on vain yksi yhteinen Y-tunnus, toisilla myös alaorganisaatioilla on omat Y-tunnuksensa. Varmista, että annat alaorganisaatiolle oikean Y-tunnuksen."
    },
    organizationIdTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.Organization.OrganizationId.Title",
        defaultMessage: "Organisaatiotunniste"
    },
    organizationIdTooltip: {
        id: "Containers.Manage.Organizations.Manage.Step1.Organization.OrganizationId.Tooltip",
        defaultMessage: "Jos organisaatiollasi tai toimialallasi on käytössä organisaatio-OID-luokitus (tällainen on olemassa esimerkiksi sote-sektorilla), kirjoita tähän tämän luokituksen mukainen organisaatiotunniste eli OID."
    },
    organizationDescriptionTitle: {
        id: "Containers.Manage.Organizations.Manage.Step1.Organization.Description.Title",
        defaultMessage: "Kuvaus"
    },
    organizationDescriptionPlaceholder: {
        id: "Containers.Manage.Organizations.Manage.Step1.Organization.Description.Placeholder",
        defaultMessage: "Anna palvelulle kuvaus."
    },
    organizationDescriptionTooltip: {
        id: "Containers.Manage.Organizations.Manage.Step1.Organization.Description.Tooltip",
        defaultMessage: "Kirjoita organisaatiolle lyhyt, käyttäjäystävällinen kuvaus. Mikä tämä (ala)organisaatio on ja mitä se tekee organisaation palveluita käyttävän asiakkaan näkökulmasta? Alä mainosta vaan kuvaa neutraalisti organisaation ydintehtävä. Kuvaus saa olla korkeintaan 500 merkkiä pitkä."
    },
    isAlternateNameUsedAsDisplayName: {
        id: "Containers.Manage.Organizations.Manage.Step1.isAlternateNameUsedAsDisplayName",
        defaultMessage: 'Ensisijaisesti käytettävä nimi'
    }

});


