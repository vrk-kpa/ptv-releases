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
import React, { PureComponent, PropTypes } from 'react'
import {
  getFormSyncErrors,
  getFormAsyncErrors,
  getFormSubmitErrors,
  hasSubmitFailed
} from 'redux-form/immutable'
import { Alert } from 'sema-ui-components'
import { connect } from 'react-redux'
import { List, fromJS, Iterable } from 'immutable'
import ImmutablePropTypes from 'react-immutable-proptypes'

const flat = (x) => {
  if (x.has('message')) {
    return List([x])
  }
  if (typeof x.map === 'function') {
    const r = x.toList().map(a => flat(a))
    return r.reduce((all, c) => c ? all.concat(c) : all)
  }
  return null
}

const getErrors = obj => flat(!Iterable.isIterable(obj) && fromJS(obj) || obj)

const renderError = error => {
  const label = error.get('label')
  return (
    <div style={{ margin: '5px' }}>
      <Alert warning>
        {label && `${label}: `}{error.get('message')}
      </Alert>
    </div>
  )
}

const renderErrors = (label, errors) => {
  return errors &&
    (<div>
      <b>{label}</b>
      {getErrors(errors).map(renderError)}
    </div>)
}

const withFormErrors = ComposedComponent => {
  class FormErrors extends PureComponent {
    static propTypes = {
      syncErrors: PropTypes.array,
      submitErrors: ImmutablePropTypes.map,
      error: PropTypes.string
    }


    render () {
      const {
        syncErrors,
        submitFailed,
        submitErrors,
        error
      } = this.props
      return (
        <div>
          {submitFailed && error &&
            <div style={{ margin: '5px' }}>
              <Alert warning>
                {'error'}
              </Alert>
            </div>
          }
          { submitFailed && renderErrors('Sync errors', syncErrors) }
          { submitFailed && renderErrors('Submit errors', submitErrors) }
          <ComposedComponent {...this.props} />
        </div>
      )
    }
  }
  return connect(
    (state, { form }) => ({
      syncErrors: getFormSyncErrors(form)(state),
      asyncErrors: getFormAsyncErrors(form)(state),
      submitErrors: getFormSubmitErrors(form)(state),
      submitFailed: hasSubmitFailed(form)(state)
    })
  )(FormErrors)
}

export default withFormErrors
