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
import { Checkbox } from 'sema-ui-components'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import withMassToolActive from './withMassToolActive'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { batchSelection } from './actions'
import { getDomainSearchCount } from 'Routes/Search/selectors'
import withState from 'util/withState'
import styles from './styles.scss'

const messages = defineMessages({
  batchSelection: {
    id: 'MassTool.BatchSelection.Title',
    defaultMessage: 'Select all'
  }
})

const MassToolBatchCheckbox = ({
  checked,
  updateUI,
  searchCount,
  batchSelection,
  intl: { formatMessage }
}) => {
  const disabled = searchCount === 0
  const handleOnChange = () => {
    updateUI('checked', !checked)
    batchSelection(checked)
  }
  return (
    <tbody>
      <tr>
        <td colSpan={6}>
          <Checkbox
            label={formatMessage(messages.batchSelection)}
            checked={checked}
            onChange={handleOnChange}
            disabled={disabled}
            className={styles.batchSelection}
          />
        </td>
      </tr>
    </tbody>
  )
}

MassToolBatchCheckbox.propTypes = {
  checked: PropTypes.bool,
  updateUI: PropTypes.func,
  searchCount: PropTypes.number,
  batchSelection: PropTypes.func,
  intl: intlShape
}

export default compose(
  injectIntl,
  withMassToolActive,
  injectFormName,
  withState({
    redux: true,
    key: 'MassToolBatchSelect',
    initialState: {
      checked: false
    }
  }),
  connect((state, { formName }) => {
    const searchCount = getDomainSearchCount(state)
    return {
      searchCount
    }
  }, {
    batchSelection
  })
)(MassToolBatchCheckbox)
