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
import React from 'react'
import { defineMessages } from 'react-intl'
import { NameCell, LanguageBarCell } from 'appComponents'
import { ServiceTypeCell, ChannelTypeCell, OrganizationCell } from 'util/redux-form/components'

const messages = defineMessages({
  name: {
    id: 'FrontPage.Shared.Search.Header.Name',
    defaultMessage: 'Nimi'
  },
  languages: {
    id: 'AppComponents.RadioButtonCell.Header.Languages',
    defaultMessage: 'Kieli ja tila'
  },
  edited: {
    id: 'FrontPage.Shared.Search.Header.Edited',
    defaultMessage: 'Muokattu'
  },
  modified: {
    id: 'FrontPage.Shared.Search.Header.Modifier',
    defaultMessage: 'Muokkaaja'
  },
  channelType: {
    id: 'appComponents.CellHeaders.ChannelType',
    defaultMessage: 'Kanavatyyppi'
  },
  serviceType: {
    id: 'appComponents.CellHeaders.ServiceType',
    defaultMessage: 'Palvelutyyppi'
  },
  channelSearchBoxOrganizationTitle: {
    id: 'Containers.ServiceAndChannels.ChannelSearch.SearchBox.Organization.Title',
    defaultMessage: 'Organisaatio'
  }
})

export const channelColumnsDefinition = (formatMessage, id, type) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: formatMessage(messages.languages)
      },
      cell: {
        formatters: [
          languagesAvailabilities => {
            return <LanguageBarCell showMissing languagesAvailabilities={languagesAvailabilities} id={id} type={type} />
          }
        ]
      }
    },
    {
      property: 'name',
      header: {
        label: formatMessage(messages.name)
      },
      cell: {
        formatters: [
          name => {
            return <NameCell name={name} />
          }
        ]
      }
    },
    {
      property: 'channelTypeId',
      header: {
        label: formatMessage(messages.channelType)
      },
      cell: {
        formatters: [
          channelTypeId => {
            return <ChannelTypeCell channelTypeId={channelTypeId} />
          }
        ]
      }
    },
    {
      property: 'organizationId',
      header: {
        label: formatMessage(messages.channelSearchBoxOrganizationTitle)
      },
      cell: {
        formatters: [
          organizationId => {
            return <OrganizationCell OrganizationIds={[organizationId]} />
          }
        ]
      }
    }
  ]
}

export const serviceColumnsDefinition = (formatMessage, id, type) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: formatMessage(messages.languages)
      },
      cell: {
        formatters: [
          languagesAvailabilities => {
            return <LanguageBarCell showMissing languagesAvailabilities={languagesAvailabilities} id={id} type={type} />
          }
        ]
      }
    },
    {
      property: 'name',
      header: {
        label: formatMessage(messages.name)
      },
      cell: {
        formatters: [
          name => {
            return <NameCell name={name} />
          }
        ]
      }
    },
    {
      property: 'serviceTypeId',
      header: {
        label: formatMessage(messages.serviceType)
      },
      cell: {
        formatters: [
          serviceTypeId => {
            return <ServiceTypeCell serviceTypeId={serviceTypeId} />
          }
        ]
      }
    },
    {
      property: 'organizationId',
      header: {
        label: formatMessage(messages.channelSearchBoxOrganizationTitle)
      },
      cell: {
        formatters: [
          organizationId => {
            return <OrganizationCell OrganizationIds={[organizationId]} />
          }
        ]
      }
    }
  ]
}
