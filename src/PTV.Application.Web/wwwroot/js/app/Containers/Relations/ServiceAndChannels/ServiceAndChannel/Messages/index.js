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

export const serviceAccordionMessages = defineMessages({
  connectedChannelsCount: {
    id: 'Containers.Relations.ServiceAndChannel.ServiceAccordion.ChannelsCount.Title',
    defaultMessage: 'Liitoksia:'
  },
  hideConnectedChannelsTitle: {
    id: 'Containers.Relations.ServiceAndChannel.ServiceAccordion.HideConnectedChannels.Title',
    defaultMessage: 'Piilota liitetyt asiointikanavat'
  },
  showConnectedChannelsTitle: {
    id: 'Containers.Relations.ServiceAndChannel.ServiceAccordion.ShowConnectedChannels.Title',
    defaultMessage: 'Näytä liitetyt asiointikanavat'
  }
})

export const channelRelationAdditionalInformationMessages = defineMessages({
  generalTab: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Tabs.GeneralTab.Title',
    defaultMessage: 'Perustiedot'
  },
  digitalAuthorizationTab: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Tabs.DigitalAuthorizationTab.Title',
    defaultMessage: 'Asiointivaltuudet'
  },
  heading: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Heading',
    defaultMessage: 'Lisää liitostiedot'
  },
  headerTitle: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Header.Title',
    defaultMessage: 'Lisää tiedot, jotka pätevät liitoksessa.'
  },
  descriptionTitle: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  descriptionTooltip: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Description.Tooltip',
    defaultMessage: 'Kuvaus'
  },
  descriptionPlaceholder: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.Description.Placeholder',
    defaultMessage: 'Kirjoita liitosta kuvaava kuvaus.'
  },
  chargeTypeTitle: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.ChargeType.Title',
    defaultMessage: 'Maksullisuuden tiedot'
  },
  chargeTypeTooltip: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.ChargeType.Tooltip',
    defaultMessage: 'Missing'
  },
  chargeTypeAdditionalInfoTitle: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.ChargeTypeAdditionalInfo.Title',
    defaultMessage: 'Maksullisuuden lisätieto'
  },
  chargeTypeAdditionalInfoPlaceholder: {
    id: 'Containers.Relations.ServiceAndChannel.AdditionalInformation.ChargeTypeAdditionalInfo.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  }
})

export const channelRelationAccordionMessages = defineMessages({
  addAdditionalInformation: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.AddAdditionalInformation.Title',
    defaultMessage: 'Lisää liitostietoja'
  },
  editAdditionalInformation: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.EditAdditionalInformation.Title',
    defaultMessage: 'Muokkaa liitostietoja'
  },
  showPublishAdditionalDetail: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.showAdditionalDetail.Title',
    defaultMessage: 'Näytä liitostiedot'
  },
  hidePublishAdditionalDetail: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.hidePublishAdditionalDetail.Title',
    defaultMessage: 'Piilota liitostiedot'
  }
})

export const channelRelationDigitalAuthorizationMessages = defineMessages({
  title: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.Title',
    defaultMessage: 'Digitaalinen lupa'
  },
  tooltip: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.Tooltip',
    defaultMessage: 'Digitaalinen lupa'
  },
  targetListHeader: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.TargetList.Header',
    defaultMessage: 'Lisätyt tiedot'
  },
  info: {
    id: 'Containers.Relations.ServiceAndChannel.ChannelRelation.DigitalAuthorization.Info',
    defaultMessage: 'Notification text that user need to be check that selections are same as done under Digital authorizations'
  }
})

