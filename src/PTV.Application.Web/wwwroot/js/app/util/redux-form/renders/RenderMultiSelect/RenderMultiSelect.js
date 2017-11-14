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
import { Select } from 'sema-ui-components'
import { List, OrderedSet } from 'immutable'
import { compose } from 'redux'
import {
  withFormFieldAnchor,
  withErrorMessage
} from 'util/redux-form/HOC'
import { Popup } from 'appComponents'
import { PTVIcon } from 'Components'
import cx from 'classnames'
import styles from './styles.scss'

export const RenderMultiSelect = ({
  input,
  label,
  tooltip,
  getCustomValue,
  options,
  renderAsTags,
  isReadOnly,
  display,
  error,
  errorMessage,
  ...rest
}) => {
  let value = getCustomValue
    ? getCustomValue(input.value, '')
    : (input && input.value) || []

  if (List.isList(value)) {
    value = value.toJS()
  }
  if (OrderedSet.isOrderedSet(value)) {
    value = value.toJS()
  }
  const labelChildren = () => (
    tooltip &&
    <Popup
      trigger={<PTVIcon name='icon-tip2' width={30} height={30} />}
      content={tooltip}
    />
  )
  const wrapClass = cx(
    styles.autosizeOff,
    {
      [styles.tagList]: renderAsTags,
      [styles.display]: (isReadOnly || display)
    }
  )
  return (
    <div className={wrapClass}>
      <Select
        label={label}
        labelChildren={rest.labelPosition !== 'inside' && labelChildren()}
        {...input}
        value={value}
        onChange={obj => {
          input.onChange(obj && OrderedSet(obj.map(x => x.value) || OrderedSet())) // This is just awful tbh
        }}
        options={options}
        multi
        size='full'
        searchable
        clearable
        {...rest}
        onBlur={() => {}} // onBlurResetsInput doesn't work react-select #751
        onBlurResetsInput={false}
        error={error}
        errorMessage={errorMessage}
      />
    </div>
  )
}
RenderMultiSelect.propTypes = {
  input: PropTypes.object.isRequired,
  error: PropTypes.bool,
  errorMessage: PropTypes.string,
  label: PropTypes.string.isRequired,
  tooltip: PropTypes.string,
  options: PropTypes.array,
  getCustomValue: PropTypes.func,
  renderAsTags: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  display: PropTypes.bool
}

RenderMultiSelect.defaultProps = {
  labelPosition: 'top'
}

export default compose(
  withErrorMessage,
  withFormFieldAnchor
)(RenderMultiSelect)
