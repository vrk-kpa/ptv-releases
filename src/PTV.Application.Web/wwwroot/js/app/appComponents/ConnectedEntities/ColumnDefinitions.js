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
import CellHeaders from 'appComponents/Cells/CellHeaders'
import NameCell from 'appComponents/Cells/NameCell'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import ChannelTypeCell from 'appComponents/Cells/ChannelTypeCell'
import OrganizationCell from 'appComponents/Cells/OrganizationCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'

export const channelColumnsDefinition = (formatMessage, id, type) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: formatMessage(CellHeaders.languages)
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
        label: formatMessage(CellHeaders.name)
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
        label: formatMessage(CellHeaders.channelType)
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
        label: formatMessage(CellHeaders.channelSearchBoxOrganizationTitle)
      },
      cell: {
        formatters: [
          organizationId => {
            return <OrganizationCell organizationId={organizationId} />
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
        label: formatMessage(CellHeaders.languages)
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
        label: formatMessage(CellHeaders.name)
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
      property: 'serviceType',
      header: {
        label: formatMessage(CellHeaders.serviceType)
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
        label: formatMessage(CellHeaders.channelSearchBoxOrganizationTitle)
      },
      cell: {
        formatters: [
          organizationId => {
            return <OrganizationCell organizationId={organizationId} />
          }
        ]
      }
    }
  ]
}

export const serviceCollectionColumnsDefinition = (formatMessage, id, type) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: formatMessage(CellHeaders.languages)
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
        label: formatMessage(CellHeaders.name)
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
      property: 'organizationId',
      header: {
        label: formatMessage(CellHeaders.channelSearchBoxOrganizationTitle)
      },
      cell: {
        formatters: [
          organizationId => {
            return <OrganizationCell organizationId={organizationId} />
          }
        ]
      }
    }
  ]
}
