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
    nameTitle: {
        id: "Containers.Services.LandingPage.Search.Name.Title",
        defaultMessage: "Nimi"
    },
    nameTooltip: {
        id: "Containers.Services.LandingPage.Search.Name.Tooltip",
        defaultMessage: "Voit hakea palveluita myös palveluluokan mukaan. Palveluluokka on aihetunniste, jonka avulla palvelut voidaan ryhmitellä ja löytää. Palvelu voi kuulua useaan eri luokkaan. Valitse pudotusvalikosta haluamasi palveluluokka."
    },
    namePlaceholder: {
        id: "Containers.Services.LandingPage.Search.Name.Placeholder",
        defaultMessage: "Hae palvelun nimellä"
    },
    organizationComboTitle: {
        id: "Containers.Services.LandingPage.Search.Organization.Title",
        defaultMessage: "Organisaatio"
    },
    organizationComboTooltip: {
        id: "Containers.Services.LandingPage.Search.Organization.Tooltip",
        defaultMessage: "Valitse pudotusvalikosta haluamasi organisaatio tai organisaatiotaso."
    },
    serviceClassComboTitle: {
        id: "Containers.Services.LandingPage.Search.ServiceClass.Title",
        defaultMessage: "Palveluluokka"
    },
    serviceClassComboTooltip: {
        id: "Containers.Services.LandingPage.Search.ServiceClass.Tooltip",
        defaultMessage: "Voit hakea palveluita myös palveluluokan mukaan. Palveluluokka on on aihetunniste, jonka avulla palvelut voidaan ryhmitellä ja löytää. Palvelu voi kuulua useaan eri luokkaan. Valitse pudotusvalikosta haluamasi palveluluokka."
    },
    ontologyKeysTitle: {
        id: "Containers.Services.LandingPage.Search.OntologyWord.Title",
        defaultMessage: "Ontologiakäsite"
    },
    ontologyKeysTooltip: {
        id: "Containers.Services.LandingPage.Search.OntologyWord.Tooltip",
        defaultMessage: "Voit hakea palveluita myös ontologiakäsitteen mukaan. Palvelutietovarannossa palveluiden asiasisältö kuvataan ontologiakäsitteillä, jotka ovat tietokoneluettavia, mahdollisimman yksiselitteisiä käsitteitä. Kirjoita kenttään palvelun asiasisältöön liittyvä sana ja valitse ennakoivan tekstinsyötön tarjoamista vaihtoehdoista ontologiakäsite."
    },
    ontologyKeysPlaceholder: {
        id: "Containers.Services.LandingPage.Search.OntologyWord.Placeholder",
        defaultMessage: "Hae ontologiakäsitteellä"
    },
    publishingStatusTooltip: {
        id: "Containers.Services.LandingPage.Search.PublishingStatus.Tooltip",
        defaultMessage: "Voit hakea luonnostilassa olevia tai julkaistuja palveluja. Luonnostilassa olevat palvelut näkyvät vain ylläpitäjille palvelutietovarannossa."
    },
    serviceResultCount: {
        id: "Containers.Services.LandingPage.Search.SearchResult.Count.Title",
        defaultMessage: "Hakutuloksia: "
    },
    serviceResultMoreThanMax: {
        id: "Containers.Services.LandingPage.Search.SearchResult.Count.Description.Title",
        defaultMessage: "There is more than already shown results. Please be more descriptive in the criteria."
    },
    pageHeaderTitle: {
        id: "Containers.Services.LandingPage.Header.Title",
        defaultMessage: "Palvelut"
    },
    pageHeaderDescription: {
        id: "Containers.Services.LandingPage.Header.Description",
        defaultMessage: "Palvelut-osiossa voit lisätä uusia palveluita ja liittää niihin  asiointikanavia sekä hakea ja muokata olemassa olevia oman organisaatiosi palveluita. Luo uusi Lisää palvelu -painikkeella tai hae palveluita yhdellä tai useammalla hakuehdolla ja valitse haluttu rivi muokattavaksi."
    },
    pageHeaderAddServiceButton: {
        id: "Containers.Services.LandingPage.Header.Button.AddService",
        defaultMessage: "Lisää palvelu"
    },
    serviceSearchResultHeaderPublishingStatus: {
        id: "Containers.Services.LandingPage.SearchResult.Header.PublishingStatus",
        defaultMessage: "Tila"
    },
    serviceSearchResultHeaderName: {
        id: "Containers.Services.LandingPage.SearchResult.Header.Name",
        defaultMessage: "Nimi"
    },
    serviceSearchResultHeaderServiceType: {
        id: "Containers.Services.LandingPage.SearchResult.Header.ServiceType",
        defaultMessage: "Palvelutyyppi"
    },    
    serviceSearchResultHeaderServiceClass: {
        id: "Containers.Services.LandingPage.SearchResult.Header.ServiceClass",
        defaultMessage: "Palveluluokka"
    },
    serviceSearchResultHeaderOntologyWords: {
        id: "Containers.Services.LandingPage.SearchResult.Header.OntologyWords",
        defaultMessage: "Ontologiakäsitteet"
    },
    serviceSearchResultHeaderConnectedChannels: {
        id: "Containers.Services.LandingPage.SearchResult.Header.ConnectedChannels",
        defaultMessage: "Kanavat"
    },
    serviceSearchResultHeaderEdited: {
        id: "Containers.Services.LandingPage.SearchResult.Header.Edited",
        defaultMessage: "Muokattu"
    },
    serviceSearchResultHeaderModifier: {
        id: "Containers.Services.LandingPage.SearchResult.Header.Modifier",
        defaultMessage: "Muokkaaja"
    },    
    serviceTypeComboTitle: {
        id: "Containers.Services.LandingPage.Search.ServiceType.Title",
        defaultMessage: "Palvelutyyppi"
    },
    serviceTypeComboTooltip: {
        id: "Containers.Services.LandingPage.Search.ServiceType.Tooltip",
        defaultMessage: "Palvelutyyppi"
    },
});