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
import { Label } from 'sema-ui-components'
import { compose } from 'redux'
import { injectIntl, intlShape } from 'react-intl'
import { messages } from '../messages'
import styles from './styles.scss'

const RenderTextFieldDisplay = ({
  input,
  label = '',
  getCustomValue,
  compare,
  preview,
  intl: { formatMessage },
  ...rest
}) => {
  const value = getCustomValue
    ? getCustomValue(input.value, '', compare)
    : input.value || ''
  if (preview) return value ? <div>{value}</div> : null
  return <div>
    <Label labelText={label} />
    <div className={styles.overflow}>{value || <Label infoLabel labelText={formatMessage(messages.emptyMessage)} />}</div>
  </div>
}
RenderTextFieldDisplay.propTypes = {
  input: PropTypes.object.isRequired,
  preview: PropTypes.bool,
  label: PropTypes.string,
  getCustomValue: PropTypes.func,
  compare: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl,
)(RenderTextFieldDisplay)
