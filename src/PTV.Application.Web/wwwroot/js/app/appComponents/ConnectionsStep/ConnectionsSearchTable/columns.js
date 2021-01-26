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
import CellHeaders from 'appComponents/Cells/CellHeaders'
import NameCell from 'appComponents/Cells/NameCell'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import ChannelTypeCell from 'appComponents/Cells/ChannelTypeCell'
import OrganizationCell from 'appComponents/Cells/OrganizationCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'
import { PTVIcon } from 'Components'
import AddConnectionButton from 'appComponents/ConnectionsStep/AddConnectionButton'
import ConnectionTags from '../ConnectionTags'
import cx from 'classnames'
import { entityConcreteTypesEnum } from 'enums'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import ContentTypeCell from 'appComponents/Cells/ContentTypeCell'
import styles from '../styles.scss'

export const channelsColumnsDefinition = (intl, previewOnClick, formName) => (onSort) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: intl.formatMessage(CellHeaders.languages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <LanguageBarCell showMissing {...rowData} />
        ]
      }
    },
    {
      property: 'name',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={intl.formatMessage(CellHeaders.name)}
                column={column}
                contentType={formName}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData: { id, channelType, isSuggested, ...rest } }) => {
            const handleOnClick = () => {
              previewOnClick(id, channelType)
            }
            const tableCellInlineClass = cx(
              // styles.tableCell,
              styles.noPadding,
              styles.inline
            )
            return <div className={tableCellInlineClass}>
              <PTVIcon name='icon-eye' onClick={handleOnClick} />
              <div>
                <NameCell id={id} textClass={styles.wrapAnywhere} {...rest} />
                <ConnectionTags
                  isSuggestedChannel={isSuggested}
                  entityId={id}
                  location='search'
                />
              </div>
            </div>
          }
        ]
      }
    },
    {
      property: 'channelType',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={intl.formatMessage(CellHeaders.channelType)}
                column={column}
                contentType={formName}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (channelType, { rowData }) => <ChannelTypeCell channelTypeId={rowData.channelTypeId} />
        ]
      }
    },
    {
      property: 'organization',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={intl.formatMessage(CellHeaders.channelSearchBoxOrganizationTitle)}
                column={column}
                contentType={formName}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (organization, { rowData }) => <OrganizationCell organizationId={rowData.organizationId} />
        ]
      }
    },
    {
      cell: {
        formatters: [
          (_, { rowIndex, rowData }) => (
            <AddConnectionButton
              rowIndex={rowIndex}
              rowData={rowData}
              intl={intl}
            />
          )
        ]
      }
    }
  ]
}

export const servicesColumnsDefinition = (intl, previewOnClick, formName) => (onSort) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: intl.formatMessage(CellHeaders.languages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <LanguageBarCell showMissing {...rowData} />
        ]
      }
    },
    {
      property: 'name',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={intl.formatMessage(CellHeaders.name)}
                column={column}
                contentType={formName}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData: { id, ...rest } }) => {
            const handleOnClick = () => {
              previewOnClick(id, entityConcreteTypesEnum.SERVICE)
            }
            const tableCellInlineClass = cx(
              // styles.tableCell,
              styles.noPadding,
              styles.inline
            )
            return <div className={tableCellInlineClass}>
              <PTVIcon name='icon-eye' onClick={handleOnClick} />
              <div>
                <NameCell id={id} textClass={styles.wrapAnywhere} {...rest} />
                <ConnectionTags entityId={id} location='search' />
              </div>
            </div>
          }
        ]
      }
    },
    {
      property: 'subEntityType',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={intl.formatMessage(CellHeaders.serviceType)}
                column={column}
                contentType={formName}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (serviceType, { rowData }) => <ServiceTypeCell serviceTypeId={rowData.serviceType} />
        ]
      }
    },
    {
      property: 'organization',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={intl.formatMessage(CellHeaders.channelSearchBoxOrganizationTitle)}
                column={column}
                contentType={formName}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (organization, { rowData }) => <OrganizationCell organizationId={rowData.organizationId} />
        ]
      }
    },
    {
      cell: {
        formatters: [
          (_, { rowIndex, rowData }) => (
            <AddConnectionButton
              rowIndex={rowIndex}
              rowData={rowData}
            />
          )
        ]
      }
    }
  ]
}

export const serviceOrChannelColumnsDefinition = (intl, previewOnClick, formName) => (onSort) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: intl.formatMessage(CellHeaders.languages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <LanguageBarCell showMissing {...rowData} />
        ]
      }
    },
    {
      property: 'name',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={intl.formatMessage(CellHeaders.name)}
                column={column}
                contentType={formName}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData: { id, subEntityType, ...rest } }) => {
            const handleOnClick = () => {
              previewOnClick(id, subEntityType)
            }
            const tableCellInlineClass = cx(
              // styles.tableCell,
              styles.noPadding,
              styles.inline
            )
            return <div className={tableCellInlineClass}>
              <PTVIcon name='icon-eye' onClick={handleOnClick} />
              <div>
                <NameCell id={id} textClass={styles.wrapAnywhere} {...rest} />
                <ConnectionTags entityId={id} location='search' />
              </div>
            </div>
          }
        ]
      }
    },
    {
      property: 'subEntityType',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={intl.formatMessage(CellHeaders.contentType)}
                column={column}
                contentType={formName}
                onSort={onSort}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData }) => <ContentTypeCell {...rowData} formatMessage={intl.formatMessage} />
        ]
      }
    },
    {
      property: 'organization',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={intl.formatMessage(CellHeaders.channelSearchBoxOrganizationTitle)}
                column={column}
                contentType={formName}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (organization, { rowData }) => <OrganizationCell organizationId={rowData.organizationId} />
        ]
      }
    },
    {
      cell: {
        formatters: [
          (_, { rowIndex, rowData }) => (
            <AddConnectionButton
              rowIndex={rowIndex}
              rowData={rowData}
              storeKey={rowData.entityType === 'service' ? 'selectedServices' : 'selectedChannels'}
            />
          )
        ]
      }
    }
  ]
}
