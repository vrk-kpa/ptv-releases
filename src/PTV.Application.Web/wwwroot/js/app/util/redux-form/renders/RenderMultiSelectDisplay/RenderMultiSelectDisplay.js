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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { Label } from 'sema-ui-components'
import NoDataLabel from 'appComponents/NoDataLabel'
import Tooltip from 'appComponents/Tooltip'
import styles from './styles.scss'
import { fieldPropTypes } from 'redux-form/immutable'

export const RenderMultiSelectDisplay = ({
  input,
  options,
  label = '',
  tooltip,
  getCustomValue,
  preview,
  required,
  sortLanguage,
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
          text.push(rest.formatDisplay ? rest.formatDisplay(option) : option.label)
        }
      })
    const hasText = text.length > 0
    const sortable = text.length > 1 && sortLanguage
    text = sortable ? text.sort((a, b) => a.localeCompare(b, sortLanguage)) : text
    if (preview) return hasText ? <div className={styles.rowAllign}>{text.map((x, index) => <span key={index}>{x}</span>)}</div> : null

    return <div>
      <Label labelText={label} />
      {tooltip && <Tooltip tooltip={tooltip} indent='i0' withinText />}
      <div className={styles.overflow}>{hasText
        ? <div className={styles.rowAllign}>{text.map((x, index) => <span key={index}>{x}</span>)}</div>
        : <NoDataLabel required={required} />}
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
  input: fieldPropTypes.input,
  getCustomValue: PropTypes.func,
  options: PropTypes.oneOfType([
    PropTypes.array,
    ImmutablePropTypes.map
  ]),
  label: PropTypes.string,
  tooltip: PropTypes.string,
  preview: PropTypes.bool,
  required: PropTypes.bool,
  sortLanguage: PropTypes.any
}

export default RenderMultiSelectDisplay
