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
  services: {
    id: 'withConnectionStep.channel.title',
    defaultMessage: 'Liitetyt palvelut ({connectionCount})'
  },
  generalDescriptions: {
    id: 'withConnectionStep.gd.title',
    defaultMessage: 'Ehdotetut asiointikanavat ({connectionCount})'
  },
  channels: {
    id: 'withConnectionStep.service.title',
    defaultMessage: 'Liitetyt asiontikanavat ({connectionCount})'
  },
  serviceCollections: {
    id: 'withConnectionStep.serviceCollection.title',
    defaultMessage: 'Liitetyt palvelut ({connectionCount})'
  },
  gdTooltip: {
    id: 'withConnectionStep.gd.tooltip',
    defaultMessage: 'Suggested channels tooltip'
  },
  serviceTooltip: {
    id: 'Connection.Step.Service.Tooltip',
    defaultMessage: 'Suggested channels tooltip'
  },
  serviceChannelsTooltip: {
    id: 'Connection.Step.ServiceChannel.Tooltip',
    defaultMessage: 'Suggested services tooltip'
  },
  serviceCollectionsTooltip: {
    id: 'Connection.Step.ServiceCollections.Tooltip',
    defaultMessage: 'Suggested services tooltip'
  },
  servicesASTI: {
    id: 'withConnectionStep.channelASTI.title',
    defaultMessage: 'ASTI-järjestelmässä liitetyt palvelupisteet ({connectionCount})'
  },
  channelsASTI: {
    id: 'withConnectionStep.serviceASTI.title',
    defaultMessage: 'ASTI-järjestelmässä liitetyt palvelut ({connectionCount})'
  },
  servicesASTIHelp: {
    id: 'withConnectionStep.channelASTIHelp.text',
    defaultMessage: 'Asiointipisteiden sopimusjärjestelmässä (ASTI) on tähän palveluun liitetty seuraavat palvelupisteet. Näitä liitoksia ei voi poistaa Palvelutietovarannossa.' // eslint-disable-line
  },
  channelsASTIHelp: {
    id: 'withConnectionStep.serviceASTIHelp.text',
    defaultMessage: 'Asiointipisteiden sopimusjärjestelmässä (ASTI) on tähän palvelupisteeseen liitetty seuraavat palvelut. Näitä liitoksia ei voi poistaa Palvelutietovarannossa.' // eslint-disable-line
  }
})
