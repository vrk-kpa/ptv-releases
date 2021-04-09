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
import { compose } from 'redux'
import { connect } from 'react-redux'
import HeaderFormatter from 'appComponents/HeaderFormatter'
import { mergeInUIState } from 'reducers/ui'
import { createGetEntityAction } from 'actions'
import styles from './styles.scss'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import NoDataLabel from 'appComponents/NoDataLabel'
import RenderServiceCollectionTableRow from './RenderServiceCollectionTableRow'
import { entityConcreteTypesEnum, formAllTypes, getKey } from 'enums'
import cx from 'classnames'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'

const RenderServiceCollections = ({
  fields,
  mergeInUIState,
  updateUI,
  openedIndex,
  loadPreviewEntity,
  isReadOnly
}) => {
  const handlePreviewOnClick = (id) => {
    const formName = getKey(formAllTypes, entityConcreteTypesEnum.SERVICECOLLECTION.toLowerCase())
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
  const tableCellFirstClass = cx(
    styles.tableCell,
    styles.tableCellFirst
  )
  if (!fields || fields && fields.length === 0) {
    return <div className={styles.emptyCollection}><NoDataLabel /></div>
  }
  return (
    <div className={styles.table}>
      <div className={styles.tableHead}>
        <div className={styles.tableRow}>
          <div className='row'>
            <div className='col-lg-4'>
              <div className={tableCellFirstClass}>
                <HeaderFormatter label={CellHeaders.languages} />
              </div>
            </div>
            <div className='col-lg-13'>
              <div className={styles.tableCell}>
                <HeaderFormatter label={CellHeaders.nameOrg} />
              </div>
            </div>
            <div className='col-lg-7'>
              <div className={styles.tableCell}>
                <HeaderFormatter label={CellHeaders.modifiedInfo} />
              </div>
            </div>
          </div>
        </div>
      </div>
      <div className={styles.tableBody}>
        {fields && fields.map((field, index) =>
          <div key={index}>
            <Field
              name={field}
              component={RenderServiceCollectionTableRow}
              field={field}
              index={index}
              onClick={handlePreviewOnClick}
              isReadOnly={isReadOnly}
            />
          </div>
        )
        }
      </div>
    </div>
  )
}
RenderServiceCollections.propTypes = {
  fields: fieldArrayPropTypes.fields,
  mergeInUIState: PropTypes.func,
  updateUI: PropTypes.func,
  openedIndex: PropTypes.number,
  loadPreviewEntity: PropTypes.func,
  isReadOnly: PropTypes.bool
}

export default compose(
  injectFormName,
  connect((state, { formName }) => ({
  }), {
    mergeInUIState,
    loadPreviewEntity: createGetEntityAction
  }),
  withFormStates
)(RenderServiceCollections)
