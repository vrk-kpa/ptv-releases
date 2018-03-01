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
import React, { PropTypes, Component } from 'react'
import { Iterable, Map } from 'immutable'
import { PTVAutoComboBox } from '../../Components'
import { callApiDirect } from '../../Middleware/Api'

export const AsyncComboBox = (props) => {
  const formatOption = (option, formatPropsFuncName) => {
    return props[formatPropsFuncName] ? props[formatPropsFuncName](option) : option
  }

  const renderOption = (formatPropsFuncName) => (option) => {
    const item = formatOption(option, formatPropsFuncName)
    return item ? item.name : ''
  }

  const getOptions = (input, callBack) => {
    if (input == '' || input.length < props.minCharCount) {
      callBack(null, { options: [] })
      return
    }
    const call = callApiDirect(props.endpoint, props.getCallData ? props.getCallData(input) : input)
                .then((json) => {
                  return { options: json.model, // .map(getOption),
                    complete: true }
                })
    return call
  }

  const filterOption = () => true

  return (
    <PTVAutoComboBox
      {...props}
      async
        // name= {props.name}
        // value = { props.value }
      values={getOptions}
      valueRenderer={renderOption('formatValue')}
      optionRenderer={renderOption('formatOption')}
      changeCallback={props.onChange}
        // readOnly={ props.readOnly }
      filterOption={filterOption}
      />
  )
}

AsyncComboBox.defaultProps = {
  minCharCount: 3
}
