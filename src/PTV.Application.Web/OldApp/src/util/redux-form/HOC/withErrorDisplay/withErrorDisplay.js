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
import { RenderError } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { ErrorUpdateProvider } from 'util/redux-form/HOC/withErrorContext/context'
import { contextUsageTypesEnum } from 'util/redux-form/HOC/withErrorContext'

const getProps = (props, mapProps) => (
  typeof mapProps === 'function' && mapProps(props) || {}
)

const withErrorDisplay = (fieldName, { errorContextType = contextUsageTypesEnum.addDynamicContext, mapProps, ...rest } = {}) => {
  if (errorContextType === contextUsageTypesEnum.addDynamicContext) {
    return WrappedComponent =>
      props => (
        <div>
          <ErrorUpdateProvider fieldName={fieldName}>
            <WrappedComponent {...props} />
            <Field
              name={fieldName}
              component={RenderError}
              errorContextType={errorContextType}
              {...getProps(props, mapProps)}
            />
          </ErrorUpdateProvider>
        </div>
      )
  }
  return WrappedComponent =>
    props => (
      <div>
        <Field
          name={fieldName}
          component={RenderError}
          errorContextType={errorContextType}
          {...getProps(props, mapProps)}
        >
          <WrappedComponent {...props} />
        </Field>
      </div>
    )
}

export default withErrorDisplay
