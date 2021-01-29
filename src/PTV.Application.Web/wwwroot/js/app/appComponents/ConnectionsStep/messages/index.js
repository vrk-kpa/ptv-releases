/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { defineMessages } from 'util/react-intl'

export const messages = defineMessages({
  connectionProfileButtonTitle: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.AddAdditionalInformation.Title',
    defaultMessage: 'Lisää liitostietoja'
  },
  modifiedByASTI: {
    id: 'appComponents.ConnectionStep.Connections.ModifiedByASTI.Text',
    defaultMessage: 'ASTI-järjestelmästä tuotu liitos'
  },
  notificationIconText: {
    id: 'appComponents.ConnectionStep.Connections.NotificationIcon.Text',
    defaultMessage: 'Content must be valid to able to collapse'
  },
  noConnectedServices: {
    id: 'Connections.ConnectedServices.Empty.Title',
    defaultMessage: 'No services connected'
  },
  noConnectedChannels: {
    id: 'Connections.ConnectedChannels.Empty.Title',
    defaultMessage: 'Ei liitettyjä asiointikanavia'
  },
  noConnectedContent: {
    id: 'Connections.ConnectedContent.Empty.Title',
    defaultMessage: 'No services or channels have been added yet.'
  },
  servicesOrderInfoWithLink1: {
    id: 'Connections.Services.OrderInfoWithLink1.Title',
    defaultMessage: 'In this section, you can change the order in which the connected services are viewed by name, service type, and organization. However, you cannot save the order. To save the desired order, you need to go to the' // eslint-disable-line
  },
  orderInfoWithLink2: {
    id: 'Connections.OrderInfoWithLink2.Title',
    defaultMessage: 'tab.'
  },
  channelsOrderInfoWithLink1: {
    id: 'Connections.Channels.OrderInfoWithLink1.Title',
    defaultMessage: 'Tässä osiossa voit muuttaa liitettyjen asiointikanavien tarkastelujärjestystä nimen, kanavatyypin ja muokkausajankohdan mukaan. Et voi kuitenkaan tallentaa järjestystä. Halutun järjestyksen tallentamiseksi sinun täytyy siirtyä' // eslint-disable-line
  },
  orderInfoLink: {
    id: 'Connections.OrderInfoLink.Title',
    defaultMessage: 'Liitokset'
  },
  connectionsNoDetailInformation: {
    id: 'Connections.noDetailInformation.Title',
    defaultMessage: 'Ei tietoja'
  },
  channelsConnectedTitle: {
    id: 'Containers.Services.AddService.Step4.Header.Title',
    defaultMessage: 'Liitetyt asiointikanavat'
  },
  channelsAvailableTitle: {
    id: 'Containers.Services.AddService.Step4.ConnectChannels.RelationLink.Add.Title',
    defaultMessage: 'Lisää asiointikanavia'
  },
  servicesConnectedTitle: {
    id: 'Containers.Channel.Common.ChannelServiceStep.Title',
    defaultMessage: 'Liitetyt pavelut'
  },
  servicesAvailableTitle: {
    id: 'Containers.Channel.Common.ChannelServiceStep.Add.Button',
    defaultMessage: 'Lisää palveluja'
  },
  gdConnectedTitle: {
    id: 'AppComponents.ConnectionStep.GDProposedChannelConnections.Title',
    defaultMessage: 'Ehdotetut asiointikanavat'
  },
  gdAvailableTitle: {
    id: 'Containers.Services.AddService.Step4.ConnectChannels.RelationLink.Add.Title',
    defaultMessage: 'Lisää asiointikanavia'
  },
  serviceCollectionsAvailableTitle: {
    id: 'AppComponents.ServiceCollections.AvailableContent.Title',
    defaultMessage: 'Lisää pavelut, asiointikanavia'
  },
  saveButton: {
    id: 'Components.Buttons.SaveButton',
    defaultMessage: 'Tallenna'
  },
  updateButton: {
    id: 'Components.Buttons.UpdateButton',
    defaultMessage: 'Muokkaa'
  },
  cancelUpdateButton: {
    id: 'Components.Buttons.CancelUpdateButton',
    defaultMessage: 'Keskeytä'
  },
  addButton: {
    id: 'Components.Buttons.AddButton',
    defaultMessage: 'Lisää'
  },
  connectedChannels: {
    id: 'Containers.ServiceCollections.ConnectedChannels.Title',
    defaultMessage: 'Liitetyt asiointikanavat'
  },
  connectedServices: {
    id: 'Containers.ServiceCollections.ConnectedServices.Title',
    defaultMessage: 'Liitetyt pavelut'
  }
})
