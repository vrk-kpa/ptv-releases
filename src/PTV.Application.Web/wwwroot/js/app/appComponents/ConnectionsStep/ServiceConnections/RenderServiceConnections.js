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
    const sortDirection = sorting.getIn(['selectedConnections', 'column']) === column.property
      ? sorting.getIn(['selectedConnections', 'sortDirection']) || sortDirectionTypes.DESC : sortDirectionTypes.DESC
    const newSorting = sorting.mergeIn(['selectedConnections'],
      { column: column.property,
        sortDirection: sortDirection === sortDirectionTypes.DESC && sortDirectionTypes.ASC || sortDirectionTypes.DESC })
    mergeInUIState({ key: 'uiData', value: { sorting: newSorting } })
  }
  const tableCellFirstClass = cx(
    styles.tableCell,
    styles.tableCellFirst
  )
  return (
    <div className={styles.table}>
      <div className={styles.tableHead}>
        <div className={styles.tableRow}>
          <div className='row'>
            <div className='col-lg-3'>
              <div className={tableCellFirstClass}>
                <HeaderFormatter label={CellHeaders.languages} />
              </div>
            </div>
            <div className='col-lg-7'>
              <div className={styles.tableCell}>
                <ColumnHeaderName
                  name={formatMessage(CellHeaders.nameOrg)}
                  column={{ property:'name' }}
                  contentType={'selectedConnections'}
                  onSort={isReadOnly && onSort}
                />
              </div>
            </div>
            <div className='col-lg-4'>
              <div className={styles.tableCell}>
                <ColumnHeaderName
                  name={formatMessage(CellHeaders.serviceType)}
                  column={{ property:'serviceType' }}
                  contentType={'selectedConnections'}
                  onSort={isReadOnly && onSort}
                />
              </div>
            </div>
            <div className='col-lg-4'>
              <div className={styles.tableCell}>
                <ColumnHeaderName
                  name={formatMessage(CellHeaders.modifiedInfo)}
                  column={{ property:'modified' }}
                  contentType={'selectedConnections'}
                  onSort={isReadOnly && onSort}
                />
              </div>
            </div>
            <div className='col-lg-6'>
              <div className={styles.tableCell}>
                <HeaderFormatter label={CellHeaders.actions} />
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className={styles.tableBody}>
        {fields && fields.map((field, index) => {
          const isOpen = openedIndex === index
          const handleOnRemove = () => fields.remove(index)
          const handleOnOpen = () => updateUI({
            openedIndex: isOpen
              ? null
              : index
          })
          return (
            <div key={index}>
              <Field
                name={field}
                component={RenderServiceTableRow}
                field={field}
                index={index}
                onClick={handlePreviewOnClick}
                onRemove={handleOnRemove}
                onOpen={handleOnOpen}
                isOpen={isOpen}
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
    isAsti: formName === formTypesEnum.ASTICONNECTIONS
  }), {
    mergeInUIState,
    loadPreviewEntity: createGetEntityAction
  }),
  withFormStates
)(RenderServiceConnections)
