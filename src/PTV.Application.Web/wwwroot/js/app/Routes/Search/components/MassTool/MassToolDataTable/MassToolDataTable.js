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
import { injectIntl, intlShape } from 'util/react-intl'
import { DataTable } from 'util/redux-form/fields'
import { getColumnDefinition } from './columnDefinitions'
// import { getCartSelectionColumns, getCartReviewColumns, getSummaryWithSelectionColumns } from './columnDefinitions'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getSortedSelectedEntitiesJS, getSelectedEntitiesForMassOperation } from '../selectors'
import { getSortedReviewListJS } from 'appComponents/MassPublish/selectors'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { formTypesEnum } from 'enums'
import cx from 'classnames'
import styles from '../styles.scss'

const MassToolDataTable = ({
  intl: { formatMessage },
  inReview,
  inSummary,
  rows
}) => {
  const tableType = inReview ? 'review' : inSummary ? 'summary' : 'selection'
  const name = {
    'review': 'reviewEntitiesForMassOperation',
    'summary': 'summaryEntitiesForMassOperation',
    'selection': 'selectedEntitiesForMassOperation'
  }[tableType]
  const columnWidths = {
    'review': ['25%', '45%', '30%'],
    'summary': ['15%', '25%', '60%'],
    'selection': ['25%', '50%', '25%']
  }[tableType]
  const massToolTableClass = cx(
    styles.massToolCartTable,
    {
      [styles.summary]: inSummary
    }
  )
  return (
    <DataTable
      name={name}
      rows={rows}
      columnsDefinition={getColumnDefinition({ formatMessage, tableType })}
      scrollable
      columnWidths={columnWidths}
      sortOnClick={() => { }}
      tight
      className={massToolTableClass}
    />
  )
}

MassToolDataTable.propTypes = {
  intl: intlShape,
  rows: PropTypes.array,
  inReview: PropTypes.bool,
  inSummary: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, { formName, intl: { formatMessage } }) => {
    const selected = getSelectedEntitiesForMassOperation(state, { formName })
    const inReview = getShowReviewBar(state)
    return {
      rows: inReview
        ? getSortedReviewListJS(state, {
          formName: formTypesEnum.MASSTOOLFORM,
          showAll: true,
          contentType: formTypesEnum.MASSTOOLFORM,
          formatMessage
        })
        : getSortedSelectedEntitiesJS(state, {
          formName,
          selected,
          contentType: formName
        }),
      inReview
    }
  })
)(MassToolDataTable)
