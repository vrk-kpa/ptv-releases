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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { withProps } from 'recompose'

// Components
import { SimpleTable } from 'sema-ui-components'
import EmptyTableMessage from 'appComponents/EmptyTableMessage'
import {
  getApprovedColumnDefinition,
  getNotApprovedColumnDefinition
} from './ColumnDefinitions'
import { getReviewListJS } from './selectors'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { moveToStep } from 'util/redux-form/HOC/withMassToolForm/actions'
import styles from './styles.scss'

const MassPublishTable = ({
  type,
  rows,
  columns,
  emptyMessage,
  approved,
  ...rest
}) => {
  const columnWidths = approved && ['30%', '70%'] || ['60%', '40%']
  return (
    <SimpleTable
      columns={columns}
      scrollable
      maxHeight={300}
      columnWidths={columnWidths}
      tight
      className={styles.massPublishTable}
    >
      <SimpleTable.Header />
      {rows.length === 0
        ? <EmptyTableMessage message={emptyMessage} />
        : <SimpleTable.Body
          rowKey='index'
          rows={rows}
        />
      }
    </SimpleTable>
  )
}

MassPublishTable.propTypes = {
  type: PropTypes.string,
  rows: PropTypes.array,
  columns: PropTypes.array,
  emptyMessage: PropTypes.string,
  approved: PropTypes.bool
}

export default compose(
  injectFormName,
  connect((state, ownProps) => {
    const rows = getReviewListJS(state, ownProps)
    return {
      rows
    }
  }, {
    moveToStep
  }),
  withProps(ownProps => ({
    columns: ownProps.approved ? getApprovedColumnDefinition() : getNotApprovedColumnDefinition(ownProps)
  }))
)(MassPublishTable)
