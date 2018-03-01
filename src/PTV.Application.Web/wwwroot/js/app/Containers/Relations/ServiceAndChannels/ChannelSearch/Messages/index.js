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
    
    channelSearchBoxHeaderTitle: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchBox.Header.Title",
        defaultMessage: "Hae ja lisää asiointikanavia"
    },
    channelSearchBoxCollapsibleHeaderTitle: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchBox.Collapsible.Header.Title",
        defaultMessage: "Rajaa hakua"
    }, 
    channelSearchBoxNameTitle: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchBox.Name.Title",
        defaultMessage: "Nimi"
    },
    channelSearchBoxNamePlaceholder: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchBox.Name.Placeholder",
        defaultMessage: "Hae..."
    },
    channelSearchBoxOrganizationTitle: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchBox.Organization.Title",
        defaultMessage: "Organisaatio"
    },
    channelSearchBoxOrganizationTooltip: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchBox.Organization.Tooltip",
        defaultMessage: "Valitse pudotusvalikosta haluamasi organisaatio tai organisaatiotaso."
    },
    channelSearchBoxPublishingStatusTooltip: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchBox.PublishingStatus.Tooltip",
        defaultMessage: "Voit hakea luonnostilassa olevia tai julkaistuja verkkoasiointikanavia. Luonnostilassa olevat näkyvät vain ylläpitäjille palvelutietovarannossa."
    },
    channelSearchBoxChannelSearchButtonTitle: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchBox.Button.Title",
        defaultMessage: "Hae"
    },        
    channelSearchBoxChannelTypeTitle: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchBox.ChannelType.Title",
        defaultMessage: "Kanavatyyppi"
    },
    channelSearchBoxChannelTypeTooltip: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchBox.ChannelType.Tooltip",
        defaultMessage: "Kanavatyyppi"
    }, 
    channelResultTableHeaderPublishingStatusTitle: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchResult.Header.PublishingStatus.Title",
        defaultMessage: "Tila"
    },
    channelResultTableHeaderNameTitle: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchResult.Header.Name.Title",
        defaultMessage: "Nimi"
    },
    channelResultTableHeaderDetailTitle:{
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchResult.Header.Detail.Title",
        defaultMessage: "Tarkemmat tiedot"
    },
    channelResultTableHeaderServicesCount: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchResult.Header.ServicesCount.Title",
        defaultMessage: "Palvelut"
    },
    channelResultTableDescriptionResultCount: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchResult.ResultCount.Title",
        defaultMessage: "Hakutuloksia: "
    },
  
})

export const detailMessages = defineMessages({    
    channelDetailTitle: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchResult.Detail.Title",
        defaultMessage: "Tarkemmat tiedot"
    },
    channelDetailName: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchResult.Detail.Name.Title",
        defaultMessage: "Nimi"
    },
    channelDetailType: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchResult.Detail.Type.Title",
        defaultMessage: "Kanavatyyppi"
    },
    channelDetailConnected: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchResult.Detail.Connected.Title",
        defaultMessage: "Liitetty"
    },
    channelDetailConnectedBy: {
        id: "Containers.ServiceAndChannels.ChannelSearch.SearchResult.Detail.ConnectedBy.Title",
        defaultMessage: "Liittäjä"
    }
});

    
    