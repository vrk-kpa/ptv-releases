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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { Label } from 'sema-ui-components'
import { NoDataLabel } from 'appComponents'
import styles from './styles.scss'

export const RenderAsyncSelectDisplay = ({
  input,
  options,
  label = '',
  preview,
  required,
  ...rest
}) => {
  if (preview) {
    const option = input && input.value && options && options.get(input.value)
    return option ? <div>{option.get('name')}</div>
      : null
  }
  return (
    <div>
      {label && <Label labelText={label} />}
      <div className={styles.overflow}>{input && input.value && options && options.get && options.get(input.value)
        ? rest.formatDisplay ? rest.formatDisplay(options.get(input.value)) : (options.get(input.value).get &&
          options.get(input.value).get('label') || options.get(input.value).label)
        : <NoDataLabel required={required} />}
      </div>
    </div>
  )
}

RenderAsyncSelectDisplay.propTypes = {
  input: PropTypes.object.isRequired,
  error: PropTypes.bool,
  errorMessage: PropTypes.string,
  label: PropTypes.string,
  tooltip: PropTypes.string,
  options: PropTypes.oneOfType([
    ImmutablePropTypes.orderedMap.isRequired,
    ImmutablePropTypes.map.isRequired
  ]),
  inlineLabel: PropTypes.bool,
  preview: PropTypes.bool,
  required: PropTypes.bool
}

export default RenderAsyncSelectDisplay
