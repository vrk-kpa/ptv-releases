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

import PTVTextEditor from './PTVTextEditor'
import * as PTVValidatorsTypes from '../PTVValidators'
import { isRequiredAvailable } from '../PTVValidations'

export const PTVTextEditorNotEmpty = (props) => {
  let validators = isRequiredAvailable(props.validators) ?
    props.validators
    : Array.isArray(props.validators)
      ? props.validators.concat([PTVValidatorsTypes.IS_NOT_EMPTY])
      : [PTVValidatorsTypes.IS_NOT_EMPTY]
  // fix for bug PTV-1825
  validators = props.maxLength ? validators.concat([PTVValidatorsTypes.IS_MORE_THAN_MAX]) : validators
  return (
    <PTVTextEditor {...props}
      validators={validators}
  />
  )
}

export default PTVTextEditorNotEmpty
