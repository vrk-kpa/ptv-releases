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
import React, { PropTypes } from 'react'
import {
  Checkbox,
  SearchField,
  TextField,
  Select
} from 'sema-ui-components'
import InfoBox from 'appComponents/InfoBox'

export const renderSearchField = ({
  input,
  label,
  tooltip,
  inlineLabel,
  ...rest
}) => (
  <div>
    { !inlineLabel &&
      <div className='form-field tooltip-label'>
        <strong>
          {label}
          <InfoBox tip={tooltip} />
        </strong>
      </div>
    }
    <SearchField {...rest}
      label={inlineLabel ? label : null}
    >
      <input {...input} />
    </SearchField>
  </div>
)
export const renderCheckBox = ({
  input,
  ...rest
}) => (
  <Checkbox
    checked={!!input.value}
    {...input}
    {...rest}
  />
)
export const renderSelect = ({
  input,
  ...rest
}) => (
  <Select
    {...input}
    {...rest}
  />
)
export const renderTextField = ({
  input,
  label,
  inlineLabel,
  ...rest
}) => (
  <div>
    { !inlineLabel &&
      <div className='form-field'>
        <strong>
          {label}
        </strong>
      </div>
    }
    <TextField
      label={inlineLabel ? label : null}
      {...input}
      {...rest}
    />
  </div>
)
