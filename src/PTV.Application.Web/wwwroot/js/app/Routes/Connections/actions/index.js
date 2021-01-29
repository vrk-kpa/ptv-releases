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
import { getFormValues, initialize, change } from 'redux-form/immutable'
import {
  getConnectionsUISorting,
  getLanguageCode,
  getConnectionsInitialOrder,
  getChannelResultPageNumber,
  getServiceResultPageNumber,
  getServiceSearchResultsIds,
  getChannelSearchResultsIds
} from 'Routes/Connections/selectors'
import { mergeInUIState } from 'reducers/ui'
import {
  setContentLanguage,
  setConnectionsMainEntity
} from 'reducers/selections'
import { getChilds } from 'Routes/Connections/components/Childs/selectors'
import { API_CALL_CLEAN, apiCall3 } from 'actions'
import { formTypesEnum, languageOrder, entityTypesEnum } from 'enums'
import { Map } from 'immutable'
import { EntitySchemas } from 'schemas'

export const makeCurrentFormStateInitial = () => ({ dispatch, getState }) => {
  const state = getState()
  const getCurrentFormValues = getFormValues('connectionsWorkbench')
  const formValues = getCurrentFormValues(state)
  dispatch(initialize('connectionsWorkbench', formValues))
}

export const removeSorting = ({ type, connectionIndex }) => ({ dispatch, getState }) => {
  const state = getState()
  const sortMap = getConnectionsUISorting(state)
  const newSortMap = Number.isInteger(connectionIndex)
    ? sortMap.filter((v, k) => k !== `connection${connectionIndex}`)
    : type === 'main'
      ? sortMap.filter((v, k) => k !== 'mainConnections')
      : sortMap.filter((v, k) => k === 'mainConnections')
  dispatch(mergeInUIState({
    key: 'uiData',
    value: { sorting: newSortMap }
  }))
}

export const toggleConnectionOrdering = connectionIndex => ({ dispatch, getState }) => {
  const children = getChilds(getState(), { connectionIndex })
  dispatch(mergeInUIState({
    key: 'activeConnectionUiData',
    value: {
      activeIndex: connectionIndex,
      order: children.map(child => child.get('id'))
    }
  }))
  Number.isInteger(connectionIndex) && removeSorting({ connectionIndex })({ dispatch, getState })
}

export const restoreInitialConnectionOrder = connectionIndex => ({ dispatch, getState }) => {
  const state = getState()
  const children = getChilds(state, { connectionIndex })
  const initialOrder = getConnectionsInitialOrder(state)
  const resetChildren = initialOrder.map(id => children.find(child => child.get('id') === id))
  dispatch(change(
    formTypesEnum.CONNECTIONSWORKBENCH, `connections[${connectionIndex}].childs`, resetChildren
  ))
}

export const setContentLanguageForPreviewConection = (parentLA, childLA) => ({ dispatch, getState }) => {
  const state = getState()
  const parentLanguages = parentLA.map(la => la.get('languageId')).toSet()
  const childLanguages = childLA.map(la => la.get('languageId')).toSet()
  // find the first language used in main and connected entities based on language order
  const sharedLanguage = parentLanguages
    .intersect(childLanguages)
    .map(id => {
      const code = getLanguageCode(state, { id })
      return Map({
        id,
        code,
        order: languageOrder[code]
      })
    })
    .sortBy(language => language.get('order'))
    .first() || Map()
  dispatch(setContentLanguage({
    id: sharedLanguage.get('id'),
    code: sharedLanguage.get('code'),
    languageKey: 'connectionDialogPreview'
  }))
}

export const clearAfterStopSearch = () => ({ dispatch }) => {
  dispatch({
    type: API_CALL_CLEAN,
    keys: ['connections', 'serviceSearch']
  })
  dispatch(setConnectionsMainEntity(null))
}

export const searchEntities = entity => ({ dispatch, getState }) => {
  const state = getState()
  if (entity === entityTypesEnum.SERVICES) {
    const getCurrentFormValues = getFormValues('searchServicesConnections')
    const formValues = getCurrentFormValues(state)
    const pageNumber = getServiceResultPageNumber(state)
    const prevEntities = getServiceSearchResultsIds(state)
    searchServices(formValues, dispatch, pageNumber, prevEntities)
  }
  if (entity === entityTypesEnum.CHANNELS) {
    const getCurrentFormValues = getFormValues('searchChannelsConnections')
    const formValues = getCurrentFormValues(state)
    const pageNumber = getChannelResultPageNumber(state)
    const prevEntities = getChannelSearchResultsIds(state)
    searchChannels(formValues, dispatch, pageNumber, prevEntities)
  }
}

export const searchServices = (formValues, dispatch, pageNumber, prevEntities) => {
  const selectedPublishingStatuses = formValues.get('selectedPublishingStatuses').filter(value => value).keySeq()
  formValues = formValues.set('selectedPublishingStatuses', selectedPublishingStatuses)
  // Will be removed after use multiselect
  formValues = formValues.set('serviceClasses', formValues.get('serviceClasses') ? [formValues.get('serviceClasses')] : [])
  formValues = formValues.set('ontologyTerms', formValues.get('ontologyTerms') ? [formValues.get('ontologyTerms')] : [])
  formValues = formValues.set('areaInformationTypes', formValues.get('areaInformationTypes') ? [formValues.get('areaInformationTypes')] : [])
  formValues = formValues.set('targetGroups', formValues.get('targetGroups') ? [formValues.get('targetGroups')] : [])
  formValues = formValues.set('lifeEvents', formValues.get('lifeEvents') ? [formValues.get('lifeEvents')] : [])
  formValues = formValues.set('industrialClasses', formValues.get('industrialClasses') ? [formValues.get('industrialClasses')] : [])
  formValues = formValues.set('pageNumber', pageNumber)
  formValues = formValues.toJS()
  if (!pageNumber) {
    dispatch({
      type: API_CALL_CLEAN,
      keys: ['connections', 'serviceSearch']
    })
  }
  dispatch(
    apiCall3({
      keys: [
        'connections',
        'serviceSearch'
      ],
      payload: {
        endpoint: 'service/GetConnectionsServices',
        data: { ...formValues, prevEntities }
      },
      saveRequestData: true,
      schemas: EntitySchemas.GET_SEARCH(EntitySchemas.SERVICE),
      clearRequest: ['prevEntities']
    })
  )
}

export const searchChannels = (formValues, dispatch, pageNumber, prevEntities) => {
  const selectedPublishingStatuses = formValues
    .get('selectedPublishingStatuses')
    .filter(value => value)
    .keySeq()
  formValues = formValues.set(
    'selectedPublishingStatuses',
    selectedPublishingStatuses
  )
  // Will be removed after use multiselect
  formValues = formValues.set(
    'areaInformationTypes',
    formValues.get('areaInformationTypes')
      ? [formValues.get('areaInformationTypes')]
      : []
  )
  formValues = formValues.set('pageNumber', pageNumber)
  formValues = formValues.toJS()
  if (!pageNumber) {
    dispatch({
      type: API_CALL_CLEAN,
      keys: ['connections', 'channelSearch']
    })
  }
  dispatch(
    apiCall3({
      keys: [
        'connections',
        'channelSearch'
      ],
      payload: {
        endpoint: 'channel/GetConnectionsChannels',
        data: { ...formValues, prevEntities }
      },
      saveRequestData: true,
      schemas: EntitySchemas.GET_SEARCH(EntitySchemas.CHANNEL)
    })
  )
}

