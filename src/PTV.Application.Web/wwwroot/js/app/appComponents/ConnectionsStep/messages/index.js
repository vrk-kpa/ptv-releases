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
import { defineMessages } from 'util/react-intl'

export const connectionMessages = defineMessages({
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
  servicesOrderInfo: {
    id: 'Connections.Services.OrderInfo.Title',
    defaultMessage: 'In this section, you can change the order in which the connected services are viewed by name, service type, and organization. However, you cannot save the order. To save the desired order, you need to go to the Connections tab. The links must be saved first.' // eslint-disable-line
  },
  servicesOrderInfoWithLink1: {
    id: 'Connections.Services.OrderInfoWithLink1.Title',
    defaultMessage: 'In this section, you can change the order in which the connected services are viewed by name, service type, and organization. However, you cannot save the order. To save the desired order, you need to go to the' // eslint-disable-line
  },
  orderInfoWithLink2: {
    id: 'Connections.OrderInfoWithLink2.Title',
    defaultMessage: 'tab.'
  },
  channelsOrderInfo: {
    id: 'Connections.Channels.OrderInfo.Title',
    defaultMessage: 'Tässä osiossa voit muuttaa liitettyjen asiointikanavien tarkastelujärjestystä nimen, kanavatyypin ja muokkausajankohdan mukaan. Et voi kuitenkaan tallentaa järjestystä. Halutun järjestyksen tallentamiseksi sinun täytyy siirtyä Liitokset välilehdelle. Liitokset tulee tallentaa ensin.' // eslint-disable-line
  },
  channelsOrderInfoWithLink1: {
    id: 'Connections.Channels.OrderInfoWithLink1.Title',
    defaultMessage: 'Tässä osiossa voit muuttaa liitettyjen asiointikanavien tarkastelujärjestystä nimen, kanavatyypin ja muokkausajankohdan mukaan. Et voi kuitenkaan tallentaa järjestystä. Halutun järjestyksen tallentamiseksi sinun täytyy siirtyä' // eslint-disable-line
  },
  orderInfoLink: {
    id: 'Connections.OrderInfoLink.Title',
    defaultMessage: 'Liitokset'
  }
})
