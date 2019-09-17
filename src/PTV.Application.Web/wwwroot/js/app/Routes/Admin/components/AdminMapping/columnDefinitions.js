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
import React, { Fragment } from 'react'
import { defineMessages, FormattedMessage } from 'util/react-intl'
import NameCell from 'appComponents/Cells/NameCell'
import ValueCell from 'appComponents/Cells/ValueCell'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import OrganizationTypeCell from 'appComponents/Cells/OrganizationTypeCell'
import HeaderFormatter from 'appComponents/HeaderFormatter/HeaderFormatter'
import { PTVIcon } from 'Components'
import Cell from 'appComponents/Cells/Cell'
import { Button } from 'sema-ui-components'

const messages = defineMessages({
  name: {
    id: 'FrontPage.Shared.Search.Header.Name',
    defaultMessage: 'Nimi'
  },
  languages: {
    id: 'AppComponents.RadioButtonCell.Header.Languages',
    defaultMessage: 'Kieli ja tila'
  },
  type: {
    id: 'AdminMapping.OrganizationType.Title',
    defaultMessage: 'Organisaatiotyyppi'
  },
  servicesCount: {
    id: 'AdminMapping.ServicesCount.Title',
    defaultMessage: 'Palveluita'
  },
  channelsCount: {
    id: 'AdminMapping.ChannelsCount.Title',
    defaultMessage: 'Kanavia'
  },
  ptvOrganizationId: {
    id: 'FrontPage.SearchId.Placeholder',
    defaultMessage: 'PTV-tunniste'
  },
  sahaId: {
    id: 'AdminMapping.SahaId.Title',
    defaultMessage: 'SaHa-tunniste (GUID)'
  },
  updateButton: {
    id: 'Components.Buttons.UpdateButton',
    defaultMessage: 'Muokkaa'
  }
})

export const columns = ({ previewOnClick, removeClick, selectedOrganizations, formatMessage, updateClick, languageCode }) => [
  {
    property: 'languagesAvailabilities',
    header: {
      label: <HeaderFormatter label={messages.languages} />
    },
    cell: {
      formatters: [
        (languagesAvailabilities, { rowData }) => {
          return <LanguageBarCell {...rowData} />
        }
      ]
    }
  },
  {
    property: 'organizationName',
    header: {
      label: <HeaderFormatter label={messages.name} />
    },
    cell: {
      formatters: [
        (name, { rowData }) => {
          return <NameCell
            viewIcon
            viewOnClick={previewOnClick}
            name={name}
            id={rowData.entityId}
            languageCode={languageCode}
          />
        }
      ]
    }
  },
  {
    property: 'organizationTypeId',
    header: {
      label: <HeaderFormatter label={messages.type} />
    },
    cell: {
      formatters: [
        (organizationTypeId, { rowData }) => <OrganizationTypeCell {...rowData} />
      ]
    }
  },
  {
    property: 'serviceCount',
    header: {
      label: <HeaderFormatter label={messages.servicesCount} />
    },
    cell: {
      formatters: [
        serviceCount => <ValueCell value={serviceCount} />

      ]
    }
  },
  {
    property: 'channelCount',
    header: {
      label: <HeaderFormatter label={messages.channelsCount} />
    },
    cell: {
      formatters: [
        channelCount => <ValueCell value={channelCount} />

      ]
    }
  },
  {
    property: 'ptvOrganizationId',
    header: {
      label: <HeaderFormatter label={messages.ptvOrganizationId} />
    },
    cell: {
      formatters: [
        (ptvOrganizationId, { rowData }) =>
          <Fragment>
            <ValueCell value={ptvOrganizationId} textHandling='prewrap' />
            <Button link onClick={() => updateClick(ptvOrganizationId, rowData.sahaId, selectedOrganizations)}>{formatMessage(messages.updateButton)}</Button>
          </Fragment>
      ]
    }
  },
  {
    property: 'sahaId',
    header: {
      label: <HeaderFormatter label={messages.sahaId} />
    },
    cell: {
      formatters: [
        sahaId => <ValueCell value={sahaId} textHandling='prewrap' />

      ]
    }
  },
  {
    property: 'ptvOrganizationId',
    header: {},
    cell: {
      formatters: [
        (ptvOrganizationId, { rowData }) =>
          <Cell>
            <PTVIcon
              onClick={() => removeClick(ptvOrganizationId, rowData.sahaId, selectedOrganizations)}
              name={'icon-trash'} />
          </Cell>
      ]
    }
  }
]

