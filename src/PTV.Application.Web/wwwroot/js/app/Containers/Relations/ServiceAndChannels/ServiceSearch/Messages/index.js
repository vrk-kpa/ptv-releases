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
    
    serviceSearchBoxHeaderTitle: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchBox.Header.Title",
        defaultMessage: "Hae ja lisää palveluja"
    }, 
    serviceSearchBoxCollapsibleHeaderTitle: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchBox.Collapsible.Header.Title",
        defaultMessage: "Rajaa hakua"
    },     
    nameTitle: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.Name.Title",
        defaultMessage: "Nimi"
    },
    nameTooltip: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.Name.Tooltip",
        defaultMessage: "Voit hakea palveluita myös palveluluokan mukaan. Palveluluokka on aihetunniste, jonka avulla palvelut voidaan ryhmitellä ja löytää. Palvelu voi kuulua useaan eri luokkaan. Valitse pudotusvalikosta haluamasi palveluluokka."
    },
    namePlaceholder: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.Name.Placeholder",
        defaultMessage: "Hae..."
    },
    organizationComboTitle: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.Organization.Title",
        defaultMessage: "Organisaatio"
    },
    organizationComboTooltip: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.Organization.Tooltip",
        defaultMessage: "Valitse pudotusvalikosta haluamasi organisaatio tai organisaatiotaso."
    },
    serviceClassComboTitle: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.ServiceClass.Title",
        defaultMessage: "Palveluluokka"
    },
    serviceClassComboTooltip: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.ServiceClass.Tooltip",
        defaultMessage: "Voit hakea palveluita myös palveluluokan mukaan. Palveluluokka on on aihetunniste, jonka avulla palvelut voidaan ryhmitellä ja löytää. Palvelu voi kuulua useaan eri luokkaan. Valitse pudotusvalikosta haluamasi palveluluokka."
    },
    ontologyKeysTitle: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.OntologyWord.Title",
        defaultMessage: "Ontologiakäsite"
    },
    ontologyKeysTooltip: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.OntologyWord.Tooltip",
        defaultMessage: "Voit hakea palveluita myös ontologiakäsitteen mukaan. Palvelutietovarannossa palveluiden asiasisältö kuvataan ontologiakäsitteillä, jotka ovat tietokoneluettavia, mahdollisimman yksiselitteisiä käsitteitä. Kirjoita kenttään palvelun asiasisältöön liittyvä sana ja valitse ennakoivan tekstinsyötön tarjoamista vaihtoehdoista ontologiakäsite."
    },
    ontologyKeysPlaceholder: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.OntologyWord.Placeholder",
        defaultMessage: "Hae ontologiakäsitteellä"
    },
    publishingStatusTooltip: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.PublishingStatus.Tooltip",
        defaultMessage: "Voit hakea luonnostilassa olevia tai julkaistuja palveluja. Luonnostilassa olevat palvelut näkyvät vain ylläpitäjille palvelutietovarannossa."
    },
    serviceResultCount: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.SearchResult.Count.Title",
        defaultMessage: "Hakutuloksia: "
    },
    serviceResultMoreThanMax: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.SearchResult.Count.Description.Title",
        defaultMessage: "There is more than already shown results. Please be more descriptive in the criteria."
    },
    pageHeaderTitle: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Header.Title",
        defaultMessage: "Palvelut"
    },
    pageHeaderDescription: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Header.Description",
        defaultMessage: "Palvelut-osiossa voit lisätä uusia palveluita ja liittää niihin  asiointikanavia sekä hakea ja muokata olemassa olevia oman organisaatiosi palveluita. Luo uusi Lisää palvelu -painikkeella tai hae palveluita yhdellä tai useammalla hakuehdolla ja valitse haluttu rivi muokattavaksi."
    },
    pageHeaderAddServiceButton: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Header.Button.AddService",
        defaultMessage: "Lisää palvelu"
    },
    serviceSearchResultHeaderPublishingStatus: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Header.PublishingStatus",
        defaultMessage: "Tila"
    },
    serviceSearchResultHeaderName: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Header.Name",
        defaultMessage: "Nimi"
    },
    serviceResultTableHeaderDetailTitle: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Detail.Title",
        defaultMessage:"Tarkemmat tiedot"
    },
    serviceSearchResultHeaderServiceType: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Header.ServiceType",
        defaultMessage: "Palvelutyyppi"
    },    
    serviceSearchResultHeaderServiceClass: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Header.ServiceClass",
        defaultMessage: "Palveluluokka"
    },
    serviceSearchResultHeaderOntologyWords: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Header.OntologyWords",
        defaultMessage: "Ontologiakäsitteet"
    },
    serviceSearchResultHeaderConnectedChannels: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Header.ConnectedChannels",
        defaultMessage: "Kanavat"
    },
    serviceSearchResultHeaderEdited: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Header.Edited",
        defaultMessage: "Muokattu"
    },
    serviceSearchResultHeaderModifier: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Header.Modifier",
        defaultMessage: "Muokkaaja"
    },    
    serviceTypeComboTitle: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.ServiceType.Title",
        defaultMessage: "Palvelutyyppi"
    },
    serviceTypeComboTooltip: {
        id: "Containers.ServiceAndChannels.ServiceSearch.Search.ServiceType.Tooltip",
        defaultMessage: "Palvelutyyppi"
    },
});

export const detailMessages = defineMessages({    
    serviceDetailTitle: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Detail.Title",
        defaultMessage: "Tarkemmat tiedot"
    },
    serviceDetailName: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Detail.Name.Title",
        defaultMessage: "Nimi"
    },   
    serviceDetailConnected: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Detail.Connected.Title",
        defaultMessage: "Liitetty"
    },
    serviceDetailConnectedBy: {
        id: "Containers.ServiceAndChannels.ServiceSearch.SearchResult.Detail.ConnectedBy.Title",
        defaultMessage: "Liittäjä"
    }
});
