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
import { property } from 'lodash'
import callApi from 'util/callApi'
import { SubmissionError, initialize } from 'redux-form/immutable'
import { Map } from 'immutable'
// import { normalize } from 'normalizr'
// import { camelizeKeys } from 'humps'
import { browserHistory } from 'react-router'
import { setSelectedEntityId } from 'reducers/selections'
import { setReadOnly, setIsAddingNewLanguage, clearNewlyAddedLanguage, setSubmitSuccess } from 'reducers/formStates'
import { mergeInUIState } from 'reducers/ui'
import { normalizeResponse } from 'Middleware/Api'
import { getShowInfosAction, getShowErrorsAction } from 'reducers/notifications'
import { formPaths } from 'enums'

export const handleOnSubmit = ({
  url,
  validators,
  transformers,
  schema,
  notificationForm
}) => async (values, dispatch, ownProps) => {
  let response = null
  let state = null
  dispatch(({ getState }) => {
    state = getState()
    values = state.getIn(['form', ownProps.form, 'values'])
  })

  if (!values) return
  if (validators) {
    const errors = validators
      .reduce((errors, validator) => errors.mergeDeep(validator(values)), Map())
    if (!errors.isEmpty()) {
      throw new SubmissionError(errors.toJS())
    }
  }
  try {
    if (Array.isArray(transformers) && transformers.length) {
      values = transformers.reduce((acc, val) => val(acc, dispatch, state), values)
    }
    response = await callApi(url, values)
  } catch (err) {
    console.error('Form submission error\n', err)
    throw new SubmissionError({
      _error: 'Submision failed for uknown reasons'
    })
  }
  const errors = property('messages.errors')(response)
  if (errors && errors.length > 0) {
    dispatch(getShowErrorsAction(notificationForm || ownProps.form, true)(errors))
    throw new SubmissionError({
      _error: errors
    })
  }
  const infos = property('messages.infos')(response)
  if (infos && infos.length > 0) {
    dispatch(getShowInfosAction(notificationForm || ownProps.form, dispatch)(infos))
  }
  if (response && response.data && schema) {
    const type = `${url.toUpperCase().replace('/', '_')}_RESPONSE`
    const normalizedReponse = normalizeResponse({ json: response, schemas: schema })
    dispatch({
      keys: [ownProps.form],
      type,
      response: normalizedReponse
    })
  }
  return (response && response.data) || null
}

export const handleOnSubmitSuccess = ({ id, action }, dispatch, form, initCallback) => {
  dispatch(setSubmitSuccess(form))
  browserHistory.replace(`${formPaths[form]}/${id}`)
  dispatch(setSelectedEntityId(id))
  dispatch(
    setIsAddingNewLanguage({
      form: form,
      value: false
    })
  )
  dispatch(clearNewlyAddedLanguage(form))
  dispatch(
    setReadOnly({
      form: form,
      value: true
    })
  )
  dispatch(({ dispatch, getState }) => {
    const state = getState()
    const initialValues = initCallback(state)
    dispatch(
      initialize(
        form,
        initialValues,
        { keepDirty: false }
      )
    )
  })
  action && action.toLowerCase() === 'saveandvalidate' && dispatch(mergeInUIState({
    key: `${form}PublishingDialog`,
    value: {
      isOpen: true
    }
  }))
}
