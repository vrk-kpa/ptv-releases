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
import { Map, List, Iterable } from 'immutable'
import shortId from 'shortid'
import { createSelector } from 'reselect'
import { formValueSelector, getFormValues, getFormSyncErrors } from 'redux-form/immutable'

export const getParameters = (state, props) => props
export const getParameterFromProps = (key, defaultValue = null) =>
  (state, props) => props && props[key] != null ? props[key] : defaultValue
export const getFormValue = (property, skipError = false, defaultFormName) => (state, props) => {
  const formName = props && props.formName || defaultFormName
  !(formName) && !skipError && console.error('form value is not specified', props)
  return formName && formValueSelector(formName)(state, property)
}
export const getValuesByFormName = (state, props) => {
  !(props && props.formName) && console.error('form value is not specified', props)
  return props && props.formName && getFormValues(props.formName)(state)
}
export const getFormValueWithPath = (getPath, returnAllifPathEmpty = true) =>
  (state, props) => {
    if (!(props && props.formName)) {
      console.error('form value is not specified', props)
      return null
    }
    const path = getPath(props)
    return path
      ? formValueSelector(props.formName)(state, path)
      : returnAllifPathEmpty && getFormValues(props.formName)(state) || null
  }
const formSyncErrorSelector = formName => createSelector(
  [
    getFormSyncErrors(formName),
    getParameterFromProps('path')
  ], (syncErrors, path) => syncErrors && syncErrors[path] || null
)
export const getFormSyncErrorWithPath = (getPath, returnAllifPathEmpty = true) =>
  (state, props) => {
    if (!(props && props.formName)) {
      console.error('form value is not specified', props)
      return null
    }
    const path = getPath(props)
    return path
      ? formSyncErrorSelector(props.formName)(state, { path })
      : returnAllifPathEmpty && getFormSyncErrors(props.formName)(state) || null
  }
export const getUiSelectorByKey = key => createSelector(
  state => state.getIn(['ui', key]),
  uiState => uiState || Map()
)
export const getUi = state => state.get('ui2') || Map()
export const getUiByKey = createSelector(
  [getParameterFromProps('uiKey'), getUi],
  (key, ui) => ui.get(key) || Map()
)
export const getCommon = state => state.get('common')
export const getServiceReducers = state => state.get('serviceReducers')
export const getOrganizationReducers = state => state.get('organizationReducers')
export const getForm = state => state.get('form') || Map()
export const getAuth = state => state.get('auth') || Map()
export const getGeneralDescriptionReducers = state => state.get('generalDescriptionReducers')
export const getChannelReducers = state => state.get('channelReducers')
export const getSelections = state => state.get('selections')
export const getSignalR = state => state.get('signalR')
export const getFormStates = state => state.get('formStates')
export const getNotifications = state => state.get('notifications')
export const joinTexts = (list, property) =>
  Array.isArray(list) ||
    Iterable.isIterable(list)
    ? list.map(x => x.get(property)).join(', ')
    : list
export const getPageModeState = state => state.get('pageModeState')
export const getJS = (map, defaultResult) => map && map.toJS ? map.toJS() : (defaultResult || null)
export const getList = map => map && map.toList ? map.toList() : List()
export const getArray = map => map && map.toArray ? map.toArray() : []
export const getMap = map => map || Map()
export const getObjectArray = map => getJS(getList(map), [])
export const getIdFromProps = getParameterFromProps('id')
export const getLanguageParameter = getParameterFromProps('language', 'fi')
export const areStepDataValid = (stepModel, newId, languageCode) =>
  stepModel.get('areDataValid') &&
  (newId == stepModel.get('result')) &&
  (languageCode === '' || languageCode === stepModel.get('language'))

export const createIsFetchingSelector = selector => createSelector(
  selector,
  data => data && data.get('isFetching') || false
)
export const getEntitiesForIds = (entities, ids, defaultResult = null, getDefaultItem) =>
  ids && ids.size > 0 ? ids.map(id => entities.get(id) || (typeof getDefaultItem === 'function' && getDefaultItem(id)) || null) : defaultResult
export const getEntitiesForIdsJS = (entities, ids) => getJS(getEntitiesForIds(entities, ids), [])
export const getEntityId = (pageModel, generateIfNotExists = true) =>
  (pageModel ? pageModel.get('id') : null) || (generateIfNotExists ? shortId.generate() : null)
export const getPageLanguageCode = (pageModel) => pageModel.get('languageCode') || ''
export const getApiCalls = createSelector(
  getCommon,
  common => common.get('apiCalls') || Map()
)

export const getEntities = createSelector(
  getCommon,
  common => common.get('entities') || Map()
)

export const createGetApiCall = key => createSelector(
  getApiCalls,
  apiCalls => (Array.isArray(key) ? apiCalls.getIn(key) : apiCalls.get(key)) || Map()
)

export const getIsFetching = (apiCallSelector) => createSelector(
  apiCallSelector,
  result => result.get('isFetching') || false
)

export const getApplication = createSelector(
  getApiCalls,
  common => common.get('application') || Map()
)
export const getEnumTypes = createSelector(
  getApplication,
  common => common.get('enumTypes') || Map()
)
export const getEnumTypesAreValid = createSelector(
  getEnumTypes,
  common => common.get('areDataValid') || false
)
export const getEnumTypesIsFetching = createSelector(
  getEnumTypes,
  common => common.get('isFetching') || false
)
export const getActiveFormField = formName => createSelector(
  getForm,
  formStates => formStates.getIn([formName, 'active']) || ''
)

export const getUiData = createSelector(
  getUi,
  common => common.get('uiData') || Map()
)

export const getUiSorting = createSelector(
  getUiData,
  ui => ui.get('sorting') || Map()
)

export const getUiSortingData = createSelector(
  [getUiSorting, getParameterFromProps('contentType')],
  (sortingData, contentType) => sortingData.get(contentType) || Map()
)

export const getUiSortingDataDirection = createSelector(
  [getUiSortingData, getParameterFromProps('column')],
  (sortingData, column) => sortingData.get('column') === column ? sortingData.get('sortDirection') : 'none'
)
