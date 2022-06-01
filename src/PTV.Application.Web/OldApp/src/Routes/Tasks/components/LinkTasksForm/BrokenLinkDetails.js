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
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import { messages } from './messages'
import PropTypes from 'prop-types'
import { compose } from 'recompose'
import { injectIntl, intlShape } from 'util/react-intl'
import Table from 'appComponents/Table'
import sharedStyles from 'Routes/Tasks/styles.scss'
import { PreviewIcon } from 'appComponents/Icons'
import styles from './styles.scss'
import ComposedCell from 'appComponents/Cells/ComposedCell'
import NameCell from 'appComponents/Cells/NameCell'
import { getKey, formAllTypes, formAllTypesBasedOnSubTypes, allContentTypesEnum } from 'enums'
import { createGetEntityAction } from 'actions'
import { mergeInUIState } from 'reducers/ui'
import { connect } from 'react-redux'
import { getSelectedLanguage } from 'Intl/Selectors'
import ContentTypeCell from 'appComponents/Cells/ContentTypeCell'
import {
  getBrokenLinkContentMoreAvaliable,
  getBrokenLinkContentIsFetching
} from 'Routes/Tasks/selectors'
import {
  loadMoreLinkContent
} from 'Routes/Tasks/actions'
import { buttonMessages } from 'Routes/messages'
import { Spinner, Button } from 'sema-ui-components'

const getDisplayName = (names, language) => {
  if (!names || !language) {
    return null
  }
  const firstKey = Object.keys(names)[0]
  return names[language] || names.fi || names[firstKey]
}

const getColumnDefinitions = (formatMessage, taskType, language, mergeInUIState, loadPreviewEntity) => onSort => {
  return [
    {
      property: 'actions',
      customClass: sharedStyles['fb5'],
      cell: {
        formatters: [
          (_, { rowData }) => {
            const entityId = rowData.entityId
            const subEntityType = (rowData.subEntityType || '').toLowerCase()

            const handlePreviewClick = () => {
              const previewEntityformName = subEntityType && getKey(formAllTypes, subEntityType) ||
                formAllTypesBasedOnSubTypes[subEntityType] || ''
              mergeInUIState({
                key: 'entityPreviewDialog',
                value: {
                  sourceForm: previewEntityformName,
                  isOpen: true,
                  entityId: null
                }
              })
              loadPreviewEntity(entityId, previewEntityformName, true)
            }

            if (subEntityType === allContentTypesEnum.CONNECTION) return null
            return (
              <PreviewIcon onClick={handlePreviewClick} />
            )
          }
        ]
      }
    },
    {
      property: 'name',
      customClass: sharedStyles['fb60'],
      header: {
        formatters: [
          (_, { column , props:{ intl: { formatMessage } } }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.contentTableName)}
                column={column}
                contentType={taskType}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (name, { rowData }) => (
            <ComposedCell
              inTable
              main={<NameCell name={getDisplayName(rowData.name, language)} bold id={`main${rowData.entityId}`} />}
              sub={<NameCell name={getDisplayName(rowData.organizationName, language)} id={`sub${rowData.entityId}`} />}
            />
          )
        ]
      }
    },
    {
      property: 'type',
      customClass: sharedStyles['fb35'],
      header: {
        formatters: [
          (_, { column, props:{ intl: { formatMessage } } }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(messages.contentTableType)}
                column={column}
                contentType={taskType}
              />
            </div>
          )
        ]
      },
      cell: {
        formatters: [
          (type, { rowData }) => (
            <ContentTypeCell {...rowData} formatMessage={formatMessage} />
          )
        ]
      }
    }
  ]
}

const BrokenLinkDetails = props => {
  const {
    id,
    content,
    taskType,
    intl: { formatMessage },
    mergeInUIState,
    loadPreviewEntity,
    language,
    moreAvailable,
    loadMoreLinkContent,
    isLoadMoreFetching
  } = props
  const handleLoadMore = () => {
    loadMoreLinkContent(id, taskType)
  }
  return (
    <div>
      <Table
        columnsDefinition={getColumnDefinitions(formatMessage, taskType, language, mergeInUIState, loadPreviewEntity)}
        name={`brokenLinkDetails${id}`}
        rows={content}
        bodyClassName={styles.innerTableWrapper}
      />
      {moreAvailable && (
        <div className={styles.tableFooter} >
          <div className={styles.buttonGroup} >
            <Button
              children={isLoadMoreFetching && <Spinner /> || formatMessage(buttonMessages.showMore)}
              onClick={handleLoadMore}
              disabled={isLoadMoreFetching}
              small secondary
            />
          </div>
        </div>
      )}
    </div>
  )
}

BrokenLinkDetails.propTypes = {
  id: PropTypes.string,
  content: PropTypes.array,
  taskType: PropTypes.string,
  intl: intlShape,
  mergeInUIState: PropTypes.func,
  loadPreviewEntity: PropTypes.func,
  language: PropTypes.string,
  moreAvailable: PropTypes.bool,
  loadMoreLinkContent: PropTypes.func,
  isLoadMoreFetching: PropTypes.bool
}

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    language: getSelectedLanguage(state, ownProps),
    moreAvailable: getBrokenLinkContentMoreAvaliable(state, { id : ownProps.id }),
    isLoadMoreFetching: getBrokenLinkContentIsFetching(state, { id : ownProps.id })
  }), {
    mergeInUIState,
    loadPreviewEntity: createGetEntityAction,
    loadMoreLinkContent
  })
)(BrokenLinkDetails)
