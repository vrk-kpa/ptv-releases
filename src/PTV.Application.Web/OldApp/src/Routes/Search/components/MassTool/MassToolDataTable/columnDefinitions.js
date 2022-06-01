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
import { getMassToolType } from '../selectors'
import { getName, getStatus } from 'appComponents/MassPublish/selectors'
import { PTVIcon } from 'Components'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import Language from 'appComponents/LanguageBar/Language'
import ComposedCell from 'appComponents/Cells/ComposedCell'
import ValueCell from 'appComponents/Cells/ValueCell'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import { Button } from 'sema-ui-components'
import { change } from 'redux-form/immutable'
import { formTypesEnum, massToolTypes } from 'enums'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { injectIntl } from 'util/react-intl'
import styles from '../styles.scss'
import { messages } from '../messages'
import { clearSelection, showInfoNotification } from '../actions'
import { mergeInUIState } from 'reducers/ui'
import MassToolDataTableCheckbox from './MassToolDataTableCheckbox'
import { moveToStep } from 'util/redux-form/HOC/withMassToolForm/actions'

const Name = compose(
  connect((state, ownProps) => ({
    name: getName(state, ownProps)
  }))
)(({ name }) => <div className={styles.textOverflow}>{name}</div>)

const RemoveButton = compose(
  injectIntl,
  injectFormName,
  connect((state, { formName }) => ({
    massToolType: getMassToolType(state, { formName }),
    formName
  }), {
    change,
    showInfoNotification
  })
)(({ change, formName, massToolType, id, unificRootId, intl: { formatMessage }, showInfoNotification, ...rest }) => {
  const handleRemoveEntity = () => {
    const fieldName = `selected.${massToolType === massToolTypes.ARCHIVE ? id : unificRootId}`
    change(formName, fieldName, false)
    showInfoNotification(formatMessage)
  }
  return <Button onClick={handleRemoveEntity} children={formatMessage(messages.cartRemoveButton)} {...rest} />
})

const closeDialog = mergeInUIState => {
  mergeInUIState({
    key: 'massToolCartDialog',
    value: {
      isOpen: false
    }
  })
}

const ClearCartButton = compose(
  injectIntl,
  connect(null, {
    mergeInUIState,
    clearSelection
  })
)(({ mergeInUIState, clearSelection, intl: { formatMessage }, ...rest }) => {
  const handleClearCart = () => {
    clearSelection()
    closeDialog(mergeInUIState)
  }
  return <Button onClick={handleClearCart} children={formatMessage(messages.cartClearButton)} {...rest} />
})

const LanguageIcon = compose(
  connect((state, ownProps) => ({
    statusId: getStatus(state, ownProps)
  }))
)(Language)

const EntityLink = compose(
  connect(null, {
    mergeInUIState,
    moveToStep
  })
)(({ id, meta: { type }, step, language, moveToStep, mergeInUIState }) => {
  const handleEntityLinkClick = () => {
    closeDialog(mergeInUIState)
    moveToStep(step)
    window.scrollTo(0, 0)
  }
  return (
    <Button link onClick={handleEntityLinkClick}>
      <Name id={id} type={type} language={language} />
    </Button>
  )
})

export const getColumnDefinition = ({ formatMessage, tableType }) => onSort => {
  const sharedSelectionColumns = [
    {
      property: 'languagesAvailabilities',
      header: {
        label: formatMessage(CellHeaders.languages)
      },
      cell: {
        formatters: [(_, { rowData: { languagesAvailabilities, publishingStatusId } }) => {
          return <LanguageBarCell
            languagesAvailabilities={languagesAvailabilities}
            publishingStatusId={publishingStatusId} />
        }]
      }
    }, {
      property: 'name',
      header: {
        formatters: [
          (_, { column }) => (
            <ColumnHeaderName
              name={formatMessage(CellHeaders.name)}
              column={column}
              contentType={tableType === 'summary'
                ? formTypesEnum.MASSTOOLFORM
                : formTypesEnum.MASSTOOLSELECTIONFORM}
              onSort={onSort}
            />
          )
        ]
      },
      cell: {
        formatters: [(_, { rowData: { id, type, language } }) => {
          return (
            <Name id={id} type={type} language={language} />
          )
        }]
      }
    }
  ]

  const selectionColumn = [
    {
      property: 'selection',
      header: {
        label: formatMessage(CellHeaders.rowSelection)
      },
      cell: {
        formatters: [
          (_, { rowData: { id, unificRootId, canBeRestore } }) => (
            <MassToolDataTableCheckbox
              id={id}
              unificRootId={unificRootId}
              canBeRestore={canBeRestore}
              hideOnScroll
            />
          )
        ]
      }
    }
  ]

  const removeButtonColumn = [
    {
      header: {
        props: {
          className: styles.centeredCell
        },
        formatters: [
          (_, { column }) => {
            return (
              <ClearCartButton
                secondary
                small
              />
            )
          }
        ]
      },
      cell: {
        props: {
          className: styles.centeredCell
        },
        formatters: [
          (_, { rowData }) => {
            return (
              <RemoveButton
                secondary
                small
                id={rowData.id}
                unificRootId={rowData.unificRootId}
              />
            )
          }
        ]
      }
    }
  ]

  const cartReviewColumns = [
    {
      property: 'language',
      header: {
        formatters: [
          (_, { column }) => (
            <ColumnHeaderName
              name={formatMessage(CellHeaders.languages)}
              column={column}
              contentType={formTypesEnum.MASSTOOLFORM}
              onSort={onSort}
            />
          )
        ]
      },
      cell: {
        formatters: [
          (_, { rowData: { id, type, language, languageId } }) => (
            <LanguageIcon
              id={id}
              type={type}
              language={language}
              languageId={languageId}
            />
          )
        ]
      }
    }, {
      property: 'name',
      header: {
        formatters: [
          (_, { column }) => (
            <ColumnHeaderName
              name={formatMessage(CellHeaders.name)}
              column={column}
              contentType={formTypesEnum.MASSTOOLFORM}
              onSort={onSort}
            />
          )
        ]
      },
      cell: {
        formatters: [(_, { rowData }) => (
          <EntityLink {...rowData} />
        )]
      }
    }, {
      property: 'acceptanceStatus',
      header: {
        formatters: [
          (_, { column }) => (
            <ColumnHeaderName
              name={formatMessage(CellHeaders.acceptanceStatus)}
              column={column}
              contentType={formTypesEnum.MASSTOOLFORM}
              onSort={onSort}
            />
          )
        ]
      },
      cell: {
        formatters: [
          (_, { rowData: { approved, acceptanceStatus } }) => {
            const iconName = approved ? 'icon-ok' : 'icon-exclamation-mark'
            const iconWrapClass = approved ? styles.approvedIcon : styles.unapprovedIcon
            return <ComposedCell inTable icon={
              <div className={iconWrapClass}>
                <PTVIcon name={iconName} componentClass={styles.statusIcon} width={12} height={12} />
              </div>
            }>
              <ValueCell value={acceptanceStatus} />
            </ComposedCell>
          }
        ]
      }
    }
  ]

  const cartSelectionColumns = sharedSelectionColumns.concat(removeButtonColumn)
  const summaryColumns = selectionColumn.concat(sharedSelectionColumns)

  return {
    'review': cartReviewColumns,
    'summary': summaryColumns,
    'selection': cartSelectionColumns
  }[tableType]
}
