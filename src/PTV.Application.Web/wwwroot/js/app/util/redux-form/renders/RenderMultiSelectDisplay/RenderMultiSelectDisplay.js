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
import { Label } from 'sema-ui-components'
import { injectIntl, intlShape } from 'react-intl'
import { messages } from '../messages'
import styles from './styles.scss'

export const RenderMultiSelectDisplay = ({
  input,
  options,
  label = '',
  getCustomValue,
  preview,
  intl: { formatMessage },
  ...rest
}) => {
  let inputValue = getCustomValue
    ? getCustomValue(input.value, [])
    : (input && input.value) || []

  const renderItems = () => {
    let text = []
    typeof inputValue === 'string' ? console.error('PTV: string is not allowed as input value')
      : inputValue.map(value => {
        const option = options && options.find && options.find(x => x.value === value)
        if (option) {
          text.push(option.label)
        }
      })
    const hasText = text.length > 0
    if (preview) return hasText ? <div>{text.join(', ')}</div> : null

    return <div>
      <Label labelText={label} />
      <div className={styles.overflow}>{hasText
        ? text.join(', ')
        : <Label infoLabel labelText={formatMessage(messages.emptyMessage)} />}
      </div>
    </div>
  }

  return (
    <div>
      {renderItems()}
    </div>
  )
}
RenderMultiSelectDisplay.propTypes = {
  input: PropTypes.object.isRequired,
  getCustomValue: PropTypes.func,
  options: PropTypes.array.isRequired,
  label: PropTypes.string,
  preview: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl
)(RenderMultiSelectDisplay)
