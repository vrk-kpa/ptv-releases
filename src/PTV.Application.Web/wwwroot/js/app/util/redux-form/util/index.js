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
import { property } from 'lodash'
import { SubmissionError, initialize } from 'redux-form/immutable'
import { Map } from 'immutable'
import { setSelectedEntityId } from 'reducers/selections'
import { setReadOnly, setIsAddingNewLanguage, clearNewlyAddedLanguage, setSubmitSuccess } from 'reducers/formStates'
import { mergeInUIState } from 'reducers/ui'
import { normalizeResponse, asyncCallApi } from 'Middleware/Api'
import { getShowInfosAction, getShowErrorsAction } from 'reducers/notifications'
import { fixReviewAfterSave } from 'util/redux-form/HOC/withMassToolForm/actions'
import { formPaths } from 'enums'

export const handleOnSubmit = ({
  url,
  method,
  withAuth,
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
    response = await asyncCallApi(url, values, dispatch, new SubmissionError({
      _error: 'Submision failed for uknown reasons'
    }), method, withAuth)
  } catch (error) {
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

export const handleOnSubmitSuccess = ({ id, action, languagesAvailabilities }, dispatch, { form, history }, initCallback) => {
  // not sure if the form has history , don't have a time to check it
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
  action && action.toLowerCase() === 'saveandpublish' && dispatch(mergeInUIState({
    key: `${form}PublishingDialog`,
    value: {
      isOpen: true
    }
  }))
  dispatch(fixReviewAfterSave({ id, languagesAvailabilities }))
  if (formPaths[form] && history && `${formPaths[form]}/${id}` !== history.location.pathname) {
    dispatch(setSubmitSuccess(form))
    formPaths[form] && history && history.replace(`${formPaths[form]}/${id}`, {
      includeValidation: action && action.toLowerCase() === 'saveandvalidate'
    })
  }
}

const getPathPrexif = currentFieldPath => {
  if (!currentFieldPath) {
    return currentFieldPath
  }

  var lastDotIndex = currentFieldPath.lastIndexOf('.')
  if (lastDotIndex < 0) {
    return null
  }

  var prefix = currentFieldPath.substring(0, lastDotIndex)
  return prefix
}

export const combinePathAndField = (pathPrefix, fieldName) => {
  if (!pathPrefix) {
    return fieldName
  }
  return `${pathPrefix}.${fieldName}`
}

export const getPathForOtherField = (currentFieldPath, otherFieldName) => {
  const pathPrefix = getPathPrexif(currentFieldPath)
  return combinePathAndField(pathPrefix, otherFieldName)
}
