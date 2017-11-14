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
import { TreeView } from './TreeView'
import { OrderedSet } from 'immutable'

export const RenderTreeView = ({
  input,
  label,
  tooltip,
  getCustomValue,
  inlineLabel,
  localizedOnChange,
  error,
  errorMessage,
  ...rest
}) => {
  let value = getCustomValue
    ? getCustomValue(input.value)
    : (input && input.value) || OrderedSet()
  const onChange = localizedOnChange && localizedOnChange(input) || input.onChange
  return (
    <div>
      <TreeView
      label={label}
        {...input}
        onChange={(id, checked) => {
          const newValue = (value || OrderedSet())
          onChange(checked ? newValue.add(id) : newValue.delete(id))
        }}
        value={value}
        {...rest}
        />
      {error &&
        <div
          style={{
            color: '#C13832',
            lineHeight: '20px',
            fontSize: '13px',
            marginTop: '5px',
            marginBottom: '5px',
            textAlign: 'left',
            display: 'inline-block',
            fontWeight: '600'
          }}
        >
          {errorMessage}
        </div>
    }
    </div>
  )
}

RenderTreeView.propTypes = {
  input: PropTypes.object.isRequired,
  meta: PropTypes.object.isRequired,
  label: PropTypes.string,
  tooltip: PropTypes.string,
  getCustomValue: PropTypes.func,
  localizedOnChange: PropTypes.func,
  inlineLabel: PropTypes.bool,
  NodesComponent: PropTypes.func.isRequired,
  NodeComponent: PropTypes.func.isRequired,
  error: PropTypes.bool,
  errorMessage: PropTypes.string
}

export default RenderTreeView
