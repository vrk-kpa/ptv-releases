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
import CheckboxField from 'util/redux-form/fields/Checkbox'
import { Checkbox } from 'sema-ui-components'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import styles from './styles.scss'
import { connect } from 'react-redux'
import { getIsPublishedUnapproved } from 'selectors/massTool'
import { ErrorsForApproved } from './Errors'

const PublishCheckbox = ({
  rowIndex,
  autoApproved,
  hasError,
  formName,
  unificRootId
}) => {
  return (
    <span className={styles.publishCheckbox}>
      {autoApproved && <Checkbox checked disabled /> ||
      hasError && <ErrorsForApproved formName={formName} unificRootId={unificRootId} /> ||
      <CheckboxField
        name={`review[${rowIndex}].approved`}
      />}
    </span>
  )
}

PublishCheckbox.propTypes = {
  rowIndex: PropTypes.number.isRequired,
  autoApproved: PropTypes.bool,
  hasError: PropTypes.bool,
  formName: PropTypes.string.isRequired,
  unificRootId: PropTypes.string.isRequired
}

export default compose(
  injectFormName,
  connect(
    (state, { unificRootId, formName }) => ({
      hasError: getIsPublishedUnapproved(state, { id: unificRootId, formName })
    })
  )
)(PublishCheckbox)
