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
import EntitySelectors from 'selectors/entities/entities'
import { compose } from 'redux'
import withInit from 'util/redux-form/HOC/withInit'
import withRoutePathId from 'util/redux-form/HOC/withRoutePathId'
import withCopyTemplate from 'util/redux-form/HOC/withCopyTemplate'

import {
  loadEntity,
  commonInit,
  openNewEntity,
  openNewEntityAsTemplate,
  getFormNameFromConcreteType,
  getEntityTypeFromConcreteType
} from './functions'

const callInitActions = (store, nextState, entityConcreteType, id, templateId, successCallback) => {
  const formName = getFormNameFromConcreteType(entityConcreteType)
  const entityType = getEntityTypeFromConcreteType(entityConcreteType)

  commonInit(store, { formName, entityType })
  if (templateId) {
    openNewEntityAsTemplate(store, { formName, entityConcreteType, entityType, templateId })
  } else if (id) {
    loadEntity(store, { id, formName, entityType, entityConcreteType, nextState, successCallback: successCallback })
  } else {
    openNewEntity(store, { formName, entityConcreteType, entityType })
  }
}

export const routeInit = (store, nextState, entityConcreteType) => {
  const { getState } = store
  const { match: { params: { id } }, location: { hash } } = nextState
  const entityType = getEntityTypeFromConcreteType(entityConcreteType)
  const isLoaded = EntitySelectors[entityType].getIsEntityLoaded(getState(), { id })
  if (isLoaded && hash) {
    return
  }
  callInitActions(store, nextState, entityConcreteType, id)
}

const getInitAction = options => {
  if (!options.entityConcreteType) {
    console.error('Missing entity type for route init. ', options)
  }
  const entityType = getEntityTypeFromConcreteType(options.entityConcreteType)

  return nextState => store => {
    const { match: { params: { id } }, location: { hash }, templateId, useCopy } = nextState
    const isLoaded = id && EntitySelectors[entityType].getIsEntityLoaded(store.getState(), { id })
    if (isLoaded && hash) {
      return
    }
    callInitActions(store, nextState, options.entityConcreteType, id, useCopy && templateId, options.successCallback)
    if (typeof options.init === 'function') {
      options.init(store, nextState)
    }
  }
}

const getShouldInitAction = options => (previousProps, nextProps, shouldInitializeDefault) => {
  let shouldInitializeResult = shouldInitializeDefault
  if (typeof options.shouldInitialize === 'function') {
    shouldInitializeResult = options.shouldInitialize(previousProps, nextProps, shouldInitializeDefault)
  }
  if (typeof nextProps.shouldInitialize === 'function') {
    shouldInitializeResult = nextProps.shouldInitialize(previousProps, nextProps, shouldInitializeDefault)
  }
  return shouldInitializeResult
}

export const withRouteInit = (options = {}) => compose(
  withRoutePathId,
  withCopyTemplate,
  withInit({
    init: getInitAction(options),
    shouldInitialize: getShouldInitAction(options)
  }))
