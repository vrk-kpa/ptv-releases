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
import PropTypes from 'prop-types'
import { Field, fieldArrayPropTypes } from 'redux-form/immutable'
import { Map } from 'immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import HeaderFormatter from 'appComponents/HeaderFormatter'
import { mergeInUIState } from 'reducers/ui'
import { createGetEntityAction } from 'actions'
import styles from '../styles.scss'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import RenderServiceTableRow from './RenderServiceTableRow'
import withState from 'util/withState'
import { entityConcreteTypesEnum, formAllTypes, getKey, formTypesEnum } from 'enums'
import cx from 'classnames'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import ColumnHeaderName from 'appComponents/Cells/ColumnHeaderName'
import {
  injectIntl,
  intlShape
} from 'util/react-intl'

const RenderServiceConnections = ({
  fields,
  mergeInUIState,
  updateUI,
  openedIndex,
  sorting,
  loadPreviewEntity,
  isReadOnly,
  isAsti,
  contentType,
  groupIndex,
  useAdditionalInfo,
  intl: { formatMessage }
}) => {
  const handlePreviewOnClick = (id) => {
    const formName = getKey(formAllTypes, entityConcreteTypesEnum.SERVICE)
    mergeInUIState({
      key: 'entityPreviewDialog',
      value: {
        sourceForm: formName,
        isOpen: true,
        entityId: null
      }
    })
    loadPreviewEntity(id, formName)
  }
  const sortDirectionTypes = {
    ASC: 'asc',
    DESC: 'desc'
  }
  const onSort = (column) => {
    const sortDirection = sorting.getIn([contentType, 'column']) === column.property
      ? sorting.getIn([contentType, 'sortDirection']) || sortDirectionTypes.DESC : sortDirectionTypes.DESC
    const newSorting = sorting.mergeIn([contentType],
      { column: column.property,
        sortDirection: sortDirection === sortDirectionTypes.DESC && sortDirectionTypes.ASC || sortDirectionTypes.DESC })
    mergeInUIState({ key: 'uiData', value: { sorting: newSorting } })
  }
  const tableCellFirstClass = cx(
    styles.tableCell,
    styles.tableCellFirst
  )

  const rowWidthNormal = useAdditionalInfo ? 'col-lg-4' : 'col-lg-5'
  const rowWidthWide = useAdditionalInfo ? 'col-lg-8' : 'col-lg-9'

  return (
    <div className={styles.table}>
      <div className={styles.tableHead}>
        <div className={styles.tableRow}>
          <div className='row align-items-center'>
            <div className={'col-lg-4'}>
              <div className={tableCellFirstClass}>
                <HeaderFormatter label={CellHeaders.languages} />
              </div>
            </div>
            <div className={rowWidthWide}>
              <div className={styles.tableCell}>
                <ColumnHeaderName
                  name={formatMessage(CellHeaders.nameOrg)}
                  column={{ property:'name' }}
                  contentType={contentType}
                  onSort={isReadOnly && onSort}
                />
              </div>
            </div>
            <div className={rowWidthNormal}>
              <div className={styles.tableCell}>
                <ColumnHeaderName
                  name={formatMessage(CellHeaders.serviceType)}
                  column={{ property:'serviceType' }}
                  contentType={contentType}
                  // onSort={isReadOnly && onSort}
                />
              </div>
            </div>
            <div className={'col-lg-4'}>
              <div className={styles.tableCell}>
                <ColumnHeaderName
                  name={formatMessage(CellHeaders.modifiedInfo)}
                  column={{ property:'modified' }}
                  contentType={contentType}
                  onSort={isReadOnly && onSort}
                />
              </div>
            </div>
            {useAdditionalInfo && <div className='col-lg-4'>
              <div className={styles.tableCell}>
                <HeaderFormatter
                  label={CellHeaders.connectionInfo}
                  tooltip={CellHeaders.connectionInfoTooltip}
                  className={styles.headerCell}
                  tooltipProps={{
                    indent: 'i5'
                  }}
                />
              </div>
            </div>}
          </div>
        </div>
      </div>
      <div className={styles.tableBody}>
        {fields && fields.map((field, index) => {
          const handleOnRemove = () => fields.remove(index)
          return (
            <div key={index}>
              <Field
                name={field}
                component={RenderServiceTableRow}
                field={field}
                index={index}
                groupIndex={groupIndex}
                onClick={handlePreviewOnClick}
                onRemove={handleOnRemove}
                isAsti={isAsti}
                isRemovable={!isReadOnly}
                isReadOnly={isReadOnly}
              />
            </div>
          )
        })}
      </div>
    </div>
  )
}
RenderServiceConnections.propTypes = {
  fields: fieldArrayPropTypes.fields,
  mergeInUIState: PropTypes.func,
  updateUI: PropTypes.func,
  openedIndex: PropTypes.number,
  loadPreviewEntity: PropTypes.func,
  isReadOnly: PropTypes.bool,
  isAsti: PropTypes.bool,
  groupIndex: PropTypes.number,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  withState({
    redux: true,
    key: 'uiData',
    keepImmutable: true,
    initialState: {
      sorting: Map()
    }
  }),
  withState({
    initialState: {
      openedIndex: null
    }
  }),
  connect((state, { formName }) => ({
    isAsti: formName === formTypesEnum.ASTICONNECTIONS,
    contentType: formName === formTypesEnum.ASTICONNECTIONS ? 'selectedAstiConnections' : 'selectedConnections'
  }), {
    mergeInUIState,
    loadPreviewEntity: createGetEntityAction
  }),
  withFormStates
)(RenderServiceConnections)
