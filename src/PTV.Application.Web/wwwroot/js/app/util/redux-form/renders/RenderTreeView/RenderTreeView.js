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
import { TreeView } from './TreeView'
import { OrderedSet } from 'immutable'
import { fieldPropTypes } from 'redux-form/immutable'
import RenderError from 'util/redux-form/renders/RenderError'
import { contextUsageTypesEnum } from 'util/redux-form/HOC/withErrorContext'

export const RenderTreeView = ({
  input,
  label,
  tooltip,
  getCustomValue,
  inlineLabel,
  customOnChange,
  customOnBlur,
  ...rest
}) => {
  let value = getCustomValue
    ? getCustomValue(input.value)
    : (input && input.value) || OrderedSet()
  const onChange = customOnChange && customOnChange(input) || input.onChange
  const onBlur = customOnBlur && customOnBlur(input) || input.onBlur
  return (
    <div>
      <RenderError
        input={input}
        errorContextType={contextUsageTypesEnum.addSimpleContext}
        {...rest}
      >
        <TreeView
          label={label}
          {...input}
          onChange={(id, checked) => {
            const newValue = (value || OrderedSet())
            onChange(checked ? newValue.add(id) : newValue.delete(id))
          }}
          onBlur={() => {
            onBlur(value || OrderedSet())
          }}
          onValueChange={newValue => {
            onChange(newValue || OrderedSet())
          }}
          value={value}
          {...rest}
        />
      </RenderError>
    </div>
  )
}

RenderTreeView.propTypes = {
  input: fieldPropTypes.input,
  meta: PropTypes.object.isRequired,
  label: PropTypes.string,
  tooltip: PropTypes.string,
  getCustomValue: PropTypes.func,
  customOnChange: PropTypes.func,
  customOnBlur: PropTypes.func,
  inlineLabel: PropTypes.bool,
  NodesComponent: PropTypes.func.isRequired,
  NodeComponent: PropTypes.func.isRequired,
  error: PropTypes.bool,
  errorMessage: PropTypes.string
}

export default RenderTreeView
