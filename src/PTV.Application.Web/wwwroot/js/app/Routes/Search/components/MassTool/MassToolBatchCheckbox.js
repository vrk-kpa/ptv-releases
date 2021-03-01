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
import { compose } from 'redux'
import { Checkbox } from 'sema-ui-components'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import withMassToolActive from './withMassToolActive'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withWindowDimensionsContext from 'util/redux-form/HOC/withWindowDimensions/withWindowDimensionsContext'
import { batchSelection, showInfoNotification } from './actions'
import { getIsBatchCheckboxChecked, getIsAnyAvailable } from './selectors'
import {
  MASSTOOL_SELECTION_LIMIT,
  formTypesEnum,
  BREAKPOINT_LG
} from 'enums'
import cx from 'classnames'
import styles from './styles.scss'

const messages = defineMessages({
  batchSelection: {
    id: 'MassTool.BatchSelection.Title',
    defaultMessage: 'Select all'
  },
  limitNotification: {
    id: 'MassTool.BatchSelection.LimitNotification.Title',
    defaultMessage: '(Voit valita kerrallaan enintään {limit} kieliversiota sisältökoriin.)'
  }
})

const MassToolBatchCheckbox = ({
  checked,
  updateUI,
  isAnyAvailable,
  batchSelection,
  colSpan,
  intl: { formatMessage },
  taskType,
  isChecked,
  dimensions,
  showInfoNotification
}) => {
  const handleOnChange = () => {
    batchSelection(isChecked, taskType)
    showInfoNotification(formatMessage)
  }
  const batchSelectionRowClass = cx(
    styles.batchSelectionRow,
    {
      [styles.tablet]: dimensions.windowWidth <= BREAKPOINT_LG
    }
  )
  return (
    <tbody className={batchSelectionRowClass}>
      <tr>
        <td colSpan={colSpan}>
          <Checkbox
            label={formatMessage(messages.batchSelection)}
            checked={isChecked}
            onChange={handleOnChange}
            disabled={!isAnyAvailable}
            className={styles.batchSelection}
          />
          <span className={styles.limitNotification}>
            {formatMessage(messages.limitNotification, { limit: MASSTOOL_SELECTION_LIMIT })}
          </span>
        </td>
      </tr>
    </tbody>
  )
}

MassToolBatchCheckbox.propTypes = {
  checked: PropTypes.bool,
  updateUI: PropTypes.func,
  isAnyAvailable: PropTypes.bool,
  batchSelection: PropTypes.func,
  colSpan: PropTypes.number,
  intl: intlShape,
  taskType: PropTypes.string,
  isChecked: PropTypes.bool,
  dimensions: PropTypes.object
}

export default compose(
  injectIntl,
  withMassToolActive,
  injectFormName,
  connect((state, { formName, taskType }) => {
    const isChecked = getIsBatchCheckboxChecked(state, {
      taskType,
      formName: formTypesEnum.MASSTOOLSELECTIONFORM
    })
    const isAnyAvailable = getIsAnyAvailable(state, {
      taskType,
      formName: formTypesEnum.MASSTOOLSELECTIONFORM
    })
    return {
      isChecked,
      isAnyAvailable
    }
  }, {
    batchSelection,
    showInfoNotification: (formatMessage) => showInfoNotification(formatMessage)
  }),
  withWindowDimensionsContext
)(MassToolBatchCheckbox)
