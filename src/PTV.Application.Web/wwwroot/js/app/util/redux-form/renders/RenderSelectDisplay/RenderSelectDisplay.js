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
import { Label } from 'sema-ui-components'
import NoDataLabel from 'appComponents/NoDataLabel'
import styles from './styles.scss'
import { Iterable } from 'immutable'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { fieldPropTypes } from 'redux-form/immutable'

const getLabel = (value, options, formatDisplay) => {
  if (!value || !options) {
    return null
  }
  if (Iterable.isIterable(options)) {
    const item = Iterable.isKeyed(options) ? options.get(value) : options.find(x => x.value === value)
    return formatDisplay ? formatDisplay(item) : item.get('label')
  } else if (options) {
    const item = options.find(x => x.value === value)
    return formatDisplay ? formatDisplay(item) : item.label
  }
  return null
}

export const RenderSelectDisplay = ({
  input,
  options,
  label = '',
  preview,
  required,
  defaultValue,
  ...rest
}) => {
  const value = input.value || typeof input.value === 'number' ? input.value : defaultValue
  if (preview) {
    return input && <div>{getLabel(value, options, rest.formatDisplay)}</div> || null
  }
  return (
    <div>
      {label && <Label labelText={label} />}
      <div className={styles.overflow}>{
        value &&
        getLabel(value, options, rest.formatDisplay) ||
        <NoDataLabel required={required} />}
      </div>
    </div>
  )
}

RenderSelectDisplay.propTypes = {
  input: fieldPropTypes.input,
  error: PropTypes.bool,
  errorMessage: PropTypes.string,
  label: PropTypes.string,
  tooltip: PropTypes.string,
  options: PropTypes.oneOfType([
    PropTypes.array.isRequired,
    ImmutablePropTypes.iterable.isRequired
  ]),
  inlineLabel: PropTypes.bool,
  preview: PropTypes.bool,
  required: PropTypes.bool,
  defaultValue: PropTypes.any
}

export default RenderSelectDisplay
