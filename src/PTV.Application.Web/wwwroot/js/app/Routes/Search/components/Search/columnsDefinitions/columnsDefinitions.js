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
import { defineMessages } from 'util/react-intl'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'
import NameCell from 'appComponents/Cells/NameCellWithNavigation'
import Cell from 'appComponents/Cells/Cell'
import OrganizationCell from 'appComponents/Cells/OrganizationCell'
import ContentTypeCell from 'appComponents/Cells/ContentTypeCell'
import RelatedOrganizations from '../../RelatedOrganizations/RelatedOrganizations'
import cellHeaders from 'appComponents/Cells/CellHeaders'
import { formAllTypes, getKey } from 'enums'
import styles from './styles.scss'
import { MassToolCheckbox } from '../../MassTool'

const messages = defineMessages({
  sharedStatusAndLanguages: {
    id: 'Routes.Search.Util.ColumnsDefinitions.StatusAndLanguages.Title',
    defaultMessage: 'Kieli ja tila'
  },
  sharedPublishingStatus: {
    id: 'FrontPage.Shared.Search.Header.PublishingStatus',
    defaultMessage: 'Tila'
  },
  sharedName: {
    id: 'FrontPage.Shared.Search.Header.Name',
    defaultMessage: 'Nimi'
  },
  sharedEdited: {
    id: 'FrontPage.Shared.Search.Header.Edited',
    defaultMessage: 'Muokattu'
  },
  sharedModified: {
    id: 'FrontPage.Shared.Search.Header.Modifier',
    defaultMessage: 'Muokkaaja'
  },
  sharedContentType: {
    id: 'FrontPage.Shared.Search.Header.ContentType',
    defaultMessage: 'Sisältotyyppi'
  },
  sharedOrganization: {
    id: 'FrontPage.Shared.Search.Header.Organization',
    defaultMessage: 'Organisaatio'
  }
})

export const getEntitiesColumnsDefinition = (formatMessage, onSort, handlePreviewOnClick, isMassToolActive) => {
  let searchColumns = [
    {
      property: 'languageAvailabilities',
      header: {
        label: formatMessage(messages.sharedStatusAndLanguages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <Cell dWidth='dw60' ldWidth='ldw100'>
            <LanguageBarCell clickable {...rowData} mainEntityType={rowData.entityType} />
          </Cell>
        ]
      }
    },
    {
      property: 'name',
      header: {
        label: formatMessage(messages.sharedName),
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.sharedName)}
                column={column}
                contentType={'entities'}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData }) => {
            const handleOnClick = () => {
              const formName = rowData.subEntityType && getKey(formAllTypes, rowData.subEntityType.toLowerCase())
              handlePreviewOnClick(rowData.id, formName)
            }
            return <Cell dWidth='dw200' ldWidth='ldw250' inline>
              <NameCell
                viewIcon
                viewOnClick={handleOnClick}
                mainEntityType={rowData.entityType}
                viewCopiedTag={rowData.isCopyTagVisible}
                {...rowData}
                languageCode='unknown'
                formatMessage={formatMessage}
              />
            </Cell>
          }
        ]
      }
    },
    {
      property: 'entityType',
      header: {
        label: formatMessage(messages.sharedContentType),
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.sharedContentType)}
                column={column}
                contentType={'entities'}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData }) => {
            return <Cell dWidth='dw80' ldWidth='ldw140' inline>
              <ContentTypeCell {...rowData} formatMessage={formatMessage} />
            </Cell>
          }
        ]
      }
    },
    {
      property: 'organizationId',
      header: {
        label: formatMessage(messages.sharedOrganization)
      },
      cell: {
        formatters: [
          (organizationId, { rowData: { organizations, producers, entityType } }) =>
            <Cell dWidth='dw160' ldWidth='ldw200' inline>
              {entityType === 'Service'
                ? <RelatedOrganizations
                  organizationIds={organizations}
                  producerIds={producers}
                  className={styles.withShowMoreWrap}
                  popupProps={{ maxHeight: 'mH300' }}
                />
                : organizationId && <OrganizationCell organizationId={organizationId} /> || null}
            </Cell>
        ]
      }
    },
    {
      property: 'modified',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.sharedEdited)}
                column={column}
                contentType={'entities'}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          modifiedAt => <Cell dWidth='dw80' ldWidth='ldw100'>
            <ModifiedAtCell modifiedAt={modifiedAt} />
          </Cell>
        ]
      }
    },
    {
      property: 'modifiedBy',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.sharedModified)}
                column={column}
                contentType={'entities'}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          modifiedBy => <Cell dWidth='dw120' ldWidth='ldw160'>
            <ModifiedByCell modifiedBy={modifiedBy} />
          </Cell>
        ]
      }
    }
  ]
  const massSelectionColumn = {
    header: {
      label: formatMessage(cellHeaders.rowSelection)
    },
    cell: {
      formatters: [
        (languagesAvailabilities, { rowData }) => <Cell dWidth='dw30' ldWidth='ldw30'>
          <MassToolCheckbox
            id={rowData.id}
            className={styles.massToolCheckbox}
            {...rowData}
          />
        </Cell>
      ]
    }
  }
  return isMassToolActive ? [massSelectionColumn].concat(searchColumns) : searchColumns
}
