
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
import React from 'react'
import { defineMessages } from 'util/react-intl'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import HeaderFormatter from 'appComponents/HeaderFormatter/HeaderFormatter'
import OperationTypeCell from './OperationTypeCell'
import Cell from 'appComponents/Cells/Cell'
import NameCellWithNavigation from 'appComponents/Cells/NameCellWithNavigation'
import NameCell from 'appComponents/Cells/NameCell'
import styles from './styles.scss'

const messages = defineMessages({
  languages: {
    id: 'AppComponents.RadioButtonCell.Header.Languages',
    defaultMessage: 'Kieli ja tila'
  },
  name: {
    id: 'FrontPage.Shared.Search.Header.Name',
    defaultMessage: 'Nimi'
  },
  event: {
    id: 'Util.ReduxForm.HOC.withEntityHeader.event',
    defaultMessage: 'Tapahntuma'
  },
  edited: {
    id: 'FrontPage.Shared.Search.Header.Edited',
    defaultMessage: 'Muokattu'
  },
  modified: {
    id: 'FrontPage.Shared.Search.Header.Modifier',
    defaultMessage: 'Muokkaaja'
  }
})

const columns = [
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
    property: 'name',
    header: {
      label: <HeaderFormatter label={messages.name} />
    },
    cell: {
      formatters: [
        (name, { rowData }) => {
          if (rowData.showLink) {
            return <Cell dWidth='dw260' ldWidth='ldw350' inline>
              <NameCellWithNavigation viewIcon {...rowData} id={rowData.entityId} />
            </Cell>
          }

          return <Cell dWidth='dw260' ldWidth='ldw350' inline className={styles.noPreview}>
            <NameCell name={name} />
          </Cell>
        }
      ]
    }
  },
  {
    property: 'publishingStatus',
    header: {
      label: <HeaderFormatter label={messages.event} />
    },
    cell: {
      formatters: [
        (_, { rowData: {
          publishingStatus,
          versionMajor,
          versionMinor,
          historyAction,
          actionDate,
          templateOrganization,
          sourceLanguageId,
          targetLanguageIds,
          expirationPeriod
        } }) => (
          <OperationTypeCell
            versionMajor={versionMajor}
            versionMinor={versionMinor}
            publishingStatus={publishingStatus}
            historyAction={historyAction}
            actionDate={actionDate}
            organization={templateOrganization}
            sourceLanguageId={sourceLanguageId}
            targetLanguageIds={targetLanguageIds}
            expirationPeriod={expirationPeriod}
          />
        )
      ]
    }
  },
  {
    property: 'createdBy',
    header: {
      label: <HeaderFormatter label={messages.modified} />
    },
    cell: {
      formatters: [
        createdBy => <ModifiedByCell modifiedBy={createdBy} />
      ]
    }
  },
  {
    property: 'created',
    header: {
      label: <HeaderFormatter label={messages.edited} />
    },
    cell: {
      formatters: [
        created => <ModifiedAtCell modifiedAt={created} />
      ]
    }
  }
]
export default columns
