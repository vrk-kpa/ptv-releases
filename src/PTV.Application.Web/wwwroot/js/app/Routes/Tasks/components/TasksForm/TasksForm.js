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
import PropTypes from 'prop-types'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import { taskTypesEnum, taskTypes, getKey, formAllTypes } from 'enums'
import withInit from 'util/redux-form/HOC/withInit'
import { loadTasksEntities } from '../../actions'
import {
  DataTable
} from 'util/redux-form/fields'
import { reduxForm } from 'redux-form/immutable'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import Cell from 'appComponents/Cells/Cell'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import NameCellWithNavigation from 'appComponents/Cells/NameCellWithNavigation'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import { getTasksServicesEntities, getTasksIsFetching,
  getTasksIsMoreAvailable,
  getTaskTypeLoadMoreIsFetching } from '../../selectors'
import { mergeInUIState } from 'reducers/ui'
import { createGetEntityAction } from 'actions'
import { buttonMessages } from 'Routes/messages'
import { Button, Spinner, Label } from 'sema-ui-components'
import styles from './styles.scss'

const messages = defineMessages({
  emptyLabel: {
    id: 'Routes.Tasks.Accordion.EmptyLabel.Title',
    defaultMessage: 'No tasks'
  }
})

const getColumnsDefinition = ({
  radioOnChange,
  previewOnClick,
  searchResults,
  formName,
  formProperty,
  taskType,
  formatMessage }) => (onSort) => {
  return [
    {
      property: 'languagesAvailabilities',
      header: {
        label: formatMessage(CellHeaders.languages)
      },
      cell: {
        formatters: [
          (languagesAvailabilities, { rowData }) => <Cell dWidth='dw120' ldWidth='ldw160'>
            <LanguageBarCell clickable {...rowData} />
          </Cell>
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
                name={formatMessage(CellHeaders.name)}
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
          (name, { rowData }) => {
            return <Cell dWidth='dw260' ldWidth='ldw350' inline>
              <NameCellWithNavigation viewIcon viewOnClick={previewOnClick}{...rowData} />
            </Cell>
          }
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
                name={formatMessage(CellHeaders.modified)}
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
          modifiedBy => <Cell dWidth='dw160' ldWidth='ldw220'>
            <ModifiedByCell componentClass='small' modifiedBy={modifiedBy} />
          </Cell>
        ]
      }
    },
    (taskType !== taskTypesEnum.CHANNELSWITHOUTSERVICES && taskType !== taskTypesEnum.SERVICESWITHOUTCHANNELS) ? {
      property: 'expireOn',
      header: {
        formatters: [
          (_, { column }) => (
            <div>
              <ColumnHeaderName
                name={formatMessage(CellHeaders.expireOn)}
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
          expireOn => <Cell dWidth='dw90' ldWidth='ldw120'>
            <ModifiedAtCell modifiedAt={expireOn} />
          </Cell>
        ]
      }
    } : {}
  ]
}

const EmptyLabel = compose(
  injectIntl
)(({ intl: { formatMessage } }) => (
  <Label labelText={formatMessage(messages.emptyLabel)} />)
)

const TasksForm = ({
  intl: { formatMessage },
  searchResults,
  taskType,
  loadPreviewEntity,
  mergeInUIState,
  loadMore,
  form,
  isMoreAvailable,
  isLoadMoreFetching,
  submit,
  taskCount
}) => {
  if (taskCount === 0) {
    return <EmptyLabel />
  }
  const _handlePreviewClick = (entityId, { subEntityType }) => {
    const previewEntityformName = subEntityType && getKey(formAllTypes, subEntityType.toLowerCase()) || ''
    mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: previewEntityformName,
        isOpen: true,
        entityId: null
      }
    })
    loadPreviewEntity(entityId, previewEntityformName)
  }
  const handleLoadMore = () => {
    loadMore({ taskType, form, loadMoreEntities: true })
  }

  return (
    <div>
      <div>
        <DataTable
          name={`draftServicesTaskId${taskType}`}
          rows={searchResults}
          columnsDefinition={getColumnsDefinition({
            previewOnClick: _handlePreviewClick,
            formName: form,
            formProperty: `draftServicesTaskId${taskType}`,
            formatMessage,
            taskType
          })}
          // borderless
          scrollable
          columnWidths={['20%', '40%', '25%', '15%']}
          sortOnClick={submit}
        />
      </div>
      {isMoreAvailable && <div className={styles.buttonGroup}>
        <Button
          children={isLoadMoreFetching && <Spinner /> || formatMessage(buttonMessages.showMore)}
          onClick={handleLoadMore}
          disabled={isLoadMoreFetching}
          small secondary
        />
      </div>}
    </div>
  )
}

const loadMore = (props) => ({ dispatch, getState }) => {
  dispatch(loadTasksEntities(getState(), props))
}

const onSubmit = (values, dispatch, props) => {
  dispatch(({ dispatch, getState }) => {
    dispatch(
      loadTasksEntities(getState(), props)
    )
  })
}

const init = (props) => ({ dispatch, getState }) => {
  dispatch(loadTasksEntities(getState(), props))
}

TasksForm.propTypes = {
  form: PropTypes.string.isRequired,
  taskType: PropTypes.string.isRequired,
  searchResults: PropTypes.string.isRequired,
  intl: intlShape.isRequired,
  loadPreviewEntity: PropTypes.func,
  mergeInUIState: PropTypes.func,
  submit: PropTypes.func,
  loadMore: PropTypes.func,
  isMoreAvailable: PropTypes.bool,
  isLoadMoreFetching: PropTypes.bool,
  taskCount: PropTypes.number
}

export default compose(
  injectIntl,
  withInit({
    init
  }),
  connect(
    (state, ownProps) => ({
      isLoading: getTasksIsFetching(state, { taskType: taskTypes[ownProps.taskType] }),
      searchResults: getTasksServicesEntities(state, { taskType: taskTypes[ownProps.taskType] }),
      isMoreAvailable: getTasksIsMoreAvailable(state, { taskType: taskTypes[ownProps.taskType] }),
      isLoadMoreFetching: getTaskTypeLoadMoreIsFetching(state, { taskType: taskTypes[ownProps.taskType] })
    })
    , {
      mergeInUIState,
      loadPreviewEntity: createGetEntityAction,
      loadMore
    }),
  withBubbling,
  reduxForm({
    onSubmit
  })
)(TasksForm)
