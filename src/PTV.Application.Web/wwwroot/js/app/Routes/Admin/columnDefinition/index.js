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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { adminTaskTypesEnum } from 'enums'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import TranslationLanguagesCell from 'appComponents/Cells/TranslationLanguagesCell'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import ComposedCell from 'appComponents/Cells/ComposedCell'
import Cell from 'appComponents/Cells/Cell'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import OrderStateCell from 'appComponents/TranslationOrderDialog/components/OrderStateCell'
import NameCellWithNavigation from 'appComponents/Cells/NameCellWithNavigation'
import NameCell from 'appComponents/Cells/NameCell'
import { TrashIcon } from 'appComponents/Icons'
import { injectIntl } from 'util/react-intl'
import { mergeInUIState } from 'reducers/ui'
import { messages } from 'Routes/Admin/messages'
import styles from './styles.scss'
import {
  Button
} from 'sema-ui-components'
import { RenderToggleTaskButton } from 'util/redux-form/renders'

const nameDef = ({ formatMessage, previewOnClick, formName, onSort,
  customProperty, cellProps = {}, withNavigation = true, forceTranslate = false }) => {
  const dWidth = cellProps.dWidth || 'dw260'
  const ldWidth = cellProps.ldWidth || 'ldw350'
  return {
    property: 'name',
    customClass: cellProps.customClass,
    header: {
      formatters: [
        (_, { column, props:{ intl: { formatMessage } } }) => {
          return (
            <ColumnHeaderName
              name={formatMessage(CellHeaders.name)}
              column={column}
              contentType={formName}
              onSort={onSort}
            />
          )
        }
      ]
    },
    cell: {
      formatters: [
        (name, { rowData }) => {
          const id = customProperty && rowData[customProperty] || rowData.id
          return (
            <Cell
              dWidth={dWidth}
              ldWidth={ldWidth}
              inline
            >
              {withNavigation
                ? <NameCellWithNavigation
                  viewIcon={rowData.name || rowData.customName}
                  viewOnClick={previewOnClick}
                  {...rowData}
                  id={id} />
                : <NameCell name={forceTranslate && formatMessage(name) || name} />}

            </Cell>
          )
        }
      ]
    }
  }
}


const translationLanguageDef = (formatMessage, cellProps) => ({
  property: 'sourceLanguage',
  customClass: cellProps.cellWidth ? styles[cellProps['cellWidth']] : undefined,
  header: {
    formatters: [
      (_, { props:{ intl: { formatMessage } } }) => (
        <div>
          {formatMessage(CellHeaders.languages)}
        </div>
      )
    ]
  },
  cell: {
    formatters: [
      (sourceLanguage, { rowData: { targetlanguage, languagesAvailabilities } }) =>
        sourceLanguage && targetlanguage && languagesAvailabilities && (
          <TranslationLanguagesCell
            sourceLanguage={sourceLanguage}
            targetLanguage={targetlanguage}
            languagesAvailabilities={languagesAvailabilities}
          />
        )
    ]
  }
})

const OrderedDateWithOrdererDef = (formatMessage, formName, onSort, cellProps) => ({
  property: 'sentAt',
  customClass: cellProps.cellWidth ? styles[cellProps['cellWidth']] : undefined,
  header: {
    formatters: [
      (_, { column, props:{ intl: { formatMessage } } }) => (
        <div>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.orderDateWithOrderer)}
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
      (_, { rowData: { sentAt, senderEmail } }) => (
        <ComposedCell
          inTable
          main={<ModifiedAtCell inline modifiedAt={sentAt} />}
          sub={<Cell>{senderEmail}</Cell>}
        />
      )
    ]
  }
})

const nextFireDef = (formatMessage, formName, onSort, cellProps) => ({
  property: 'nextFireTimeUtc',
  customClass: cellProps.cellWidth ? styles[cellProps['cellWidth']] : undefined,
  header: {
    formatters: [
      (_, { column, props:{ intl: { formatMessage } } }) => (
        <div>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.nextFireTime)}
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
      (_, { rowData: { nextFireTimeUtc } }) => (
        <ComposedCell
          inTable
          main={<ModifiedAtCell inline modifiedAt={nextFireTimeUtc} />}
        />
      )
    ]
  }
})

const lastJobStatusDef = (formatMessage, formName, onSort, cellProps) => ({
  property: 'lastJobStatus',
  customClass: cellProps.cellWidth ? styles[cellProps['cellWidth']] : undefined,
  header: {
    formatters: [
      (_, { column, props:{ intl: { formatMessage } } }) => (
        <div>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.jobStatus)}
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
      (_, { rowData: { lastJobStatus } }) => (
        <ComposedCell
          inTable
          main={lastJobStatus}
        />
      )
    ]
  }
})

const ErrorCodeDef = (formatMessage, formName, onSort, cellProps) => ({
  property: 'translationStateTypeId',
  customClass: cellProps.cellWidth ? styles[cellProps['cellWidth']] : undefined,
  header: {
    formatters: [
      (_, { column, props:{ intl: { formatMessage } } }) => (
        <div>
          <ColumnHeaderName
            name={formatMessage(CellHeaders.errorCode)}
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
      errorCodeId => <OrderStateCell id={errorCodeId} />
    ]
  }
})

const DeleteTranslationOrder = compose(
  injectIntl,
  connect((state, { id, taskType }) => ({
    // showIcon: getIsDeleteButtonVisibleForRow(state, { id, taskType })
  }), {
    mergeInUIState
  })
)(({
  id,
  mergeInUIState,
  showIcon
}) => {
  const handleOnDeleteTranslationOrderClick = () => {
    mergeInUIState({
      key: 'deleteTranslationOrderDialog',
      value: {
        isOpen: true,
        id
      }
    })
  }
  // return showIcon && (
  return <TrashIcon onClick={handleOnDeleteTranslationOrderClick} />
  // )
})

const ForceJob = compose(
  injectIntl,
  connect((state, { id, taskType }) => ({
    showIcon: taskType === adminTaskTypesEnum.SCHEDULEDTASKS
  }), {
    mergeInUIState
  })
)(({
  id,
  mergeInUIState,
  showIcon,
  intl: { formatMessage },
  titleMessage,
  jobName,
  isRunning
}) => {
  const handleOnForceJobClick = () => {
    mergeInUIState({
      key: 'forceJobDialog',
      value: {
        isOpen: true,
        id,
        jobName
      }
    })
  }
  return showIcon && <Button small secondary disabled={isRunning} onClick={handleOnForceJobClick}>{formatMessage(titleMessage)}</Button>
})

const failedTranslationOrdersActionDef = (formatMessage, cellProps) => ({
  property: 'actions',
  customClass: cellProps.cellWidth ? styles[cellProps['cellWidth']] : undefined,
  cell: {
    formatters: [
      (_, { rowData: { id, errorDescription } }) => {
        return (
          <div className='d-flex align-items-center justify-content-between'>
            <RenderToggleTaskButton id={id} taskType={adminTaskTypesEnum.FAILEDTRANSLATIONORDERS} />
            <DeleteTranslationOrder id={id} taskType={adminTaskTypesEnum.FAILEDTRANSLATIONORDERS} />
          </div>
        )
      }
    ]
  }
})

const scheduledJobActionDef = (formatMessage, cellProps = {}) => ({
  property: 'actions',
  customClass: cellProps.cellWidth ? styles[cellProps['cellWidth']] : undefined,
  cell: {
    formatters: [
      (_, { rowData: { code, name, isRunning } }) => (
        <div className={styles.actions}>
          <RenderToggleTaskButton
            id={code}
            taskType={adminTaskTypesEnum.SCHEDULEDTASKS}
          />
          <ForceJob
            id={code}
            taskType={adminTaskTypesEnum.SCHEDULEDTASKS}
            titleMessage={messages.forceJobButtonTitle}
            jobName={name}
            isRunning={isRunning}
          />
        </div>
      )
    ]
  }
})

export const getAdminTaskColumnDefinition = taskType => ({
  previewOnClick,
  formName,
  formatMessage
}) => onSort => {
  switch (taskType) {
    case adminTaskTypesEnum.FAILEDTRANSLATIONORDERS:
      return [
        translationLanguageDef(formatMessage, { cellWidth: 'fb15' }),
        nameDef({
          formatMessage,
          previewOnClick,
          formName,
          onSort: false,
          customProperty: 'entityVersionedId',
          cellProps: { customClass: styles['fb25'], ldWidth: 'ldw200', dWidth: 'dw160' }
        }),
        OrderedDateWithOrdererDef(formatMessage, formName, false, { cellWidth: 'fb20' }),
        ErrorCodeDef(formatMessage, formName, false, { cellWidth: 'fb20' }),
        failedTranslationOrdersActionDef(formatMessage, { cellWidth: 'fb20' })
      ]
    case adminTaskTypesEnum.SCHEDULEDTASKS:
      return [
        nameDef({
          formatMessage,
          formName,
          cellProps: { customClass: styles['fb35'] },
          withNavigation: false,
          onSort
        }),
        nextFireDef(formatMessage, formName, onSort, { cellWidth: 'fb20' }),
        lastJobStatusDef(formatMessage, formName, onSort, { cellWidth: 'fb15' }),
        scheduledJobActionDef(formatMessage, { cellWidth: 'fb30' })
      ]
  }
}
