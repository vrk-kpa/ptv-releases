/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the 'Software'), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
import { CALL_API } from 'util/symbols'
import { getShowErrorsAction, getShowInfosAction, getShowWarningAction } from 'reducers/notifications'
import { EntitySchemas } from 'schemas'
import {
  formGetActionTypes,
  formEntityTypes,
  formApiPaths,
  formTypesEnum,
  formActionsTypesEnum,
  formActions
} from 'enums'
import { change, reset } from 'redux-form/immutable'
import {
  getOrganizationAreaInformation,
  getDefaultAreaInformation
} from 'selectors/areaInformation'
import {
  getFundingTypeForOrganization
} from 'selectors/fundingType'
import { Map } from 'immutable'
import { property } from 'lodash'

const formSchemas = {
  [formTypesEnum.SERVICEFORM]: EntitySchemas.SERVICE_FORM,
  [formTypesEnum.SERVICECOLLECTIONFORM]: EntitySchemas.SERVICECOLLECTION,
  [formTypesEnum.GENERALDESCRIPTIONFORM]: EntitySchemas.GENERAL_DESCRIPTION,
  [formTypesEnum.ORGANIZATIONFORM]: EntitySchemas.ORGANIZATION,
  [formTypesEnum.ELECTRONICCHANNELFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.PHONECHANNELFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.PRINTABLEFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.SERVICELOCATIONFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.WEBPAGEFORM]: EntitySchemas.CHANNEL,
  [formTypesEnum.GENERALDESCRIPTIONSEARCHFORM]: '/noschema'
}

export const API_CALL_REQUEST = 'API_CALL_REQUEST'
export const API_CALL_SUCCESS = 'API_CALL_SUCCESS'
export const API_CALL_FAILURE = 'API_CALL_FAILURE'

const showMessages = (messages, customAction, defaultAction, dispatch) => {
  const action = typeof customAction === 'function' ? customAction : defaultAction
  if (action && messages && messages.length > 0) {
    dispatch(action(messages))
  }
}

const showAllMessages = ({
  key,
  showNotification,
  onErrorAction,
  onInfoAction,
  onWarningAction,
  messages
},
dispatch
) => {
  showMessages(
    property('errors')(messages), onErrorAction, showNotification && getShowErrorsAction(key, true), dispatch
  )
  showMessages(
    property('warnings')(messages), onWarningAction, showNotification && getShowErrorsAction(key, true), dispatch
  )
  showMessages(
    property('infos')(messages), onInfoAction, showNotification && getShowInfosAction(key, true), dispatch
  )
}

export const showResponseMessages = options => ({ dispatch }) => {
  showAllMessages(options, dispatch)
  // showMessages(
  //   messages.errors, onErrorAction, showNotification && getShowErrorsAction(key, true), dispatch
  // )
  // showMessages(
  //   messages.warnings, onWarningAction, showNotification && getShowErrorsAction(key, true), dispatch
  // )
  // showMessages(
  //   messages.infos, onInfoAction, showNotification && getShowInfosAction(key, true), dispatch
  // )
}

const getSuccessAction = ({
  keys,
  successCallback,
  showNotification,
  onErrorAction,
  onInfoAction,
  onWarningAction
}) =>
  (data, messages, original) => store => {
    typeof successCallback === 'function' && successCallback(data, messages, original, store)
    const key = Array.isArray(keys) ? keys.join('_') : keys
    showAllMessages({
      key,
      showNotification,
      onErrorAction,
      onInfoAction,
      onWarningAction,
      messages
    }, store.dispatch)
  }

export const apiCall3 = ({
  keys,
  payload,
  saveRequestData,
  schemas,
  clearRequest,
  successNextAction,
  successCallback,
  successAction,
  onErrorAction,
  onInfoAction,
  onWarningAction,
  showNotification = true,
  canAbort,
  requestProps
}) => ({ getState, dispatch }) => {
  const successActionInner = typeof successAction === 'function'
    ? successAction
    : getSuccessAction({
      keys,
      successCallback,
      showNotification,
      onErrorAction,
      onInfoAction,
      onWarningAction
    })

  return {
    keys,
    saveRequestData,
    clearRequest,
    [CALL_API]: {
      types: [API_CALL_REQUEST,
        API_CALL_SUCCESS,
        API_CALL_FAILURE],
      payload,
      schemas,
      successNextAction,
      successAction: successActionInner,
      canAbort,
      requestProps
    }
  }
}

export const API_CALL_CLEAN = 'API_CALL_CLEAN'

export const clearApiCall = (keys, response) => ({
  keys,
  type: API_CALL_SUCCESS,
  response: response
})

export const deleteApiCall = (keys) => ({
  keys,
  type: API_CALL_CLEAN
})

export const unLockEntity = (entityId, formName) =>
  apiCall3({
    keys: [formEntityTypes[formName], 'unLock'],
    payload: {
      endpoint: formActions[formActionsTypesEnum.UNLOCKENTITY][formName],
      data: { id: entityId }
    },
    schemas: formSchemas[formName],
    formName
  })

export const createGetEntityAction = (id, formName) => {
  return (
    apiCall3({
      keys: [formEntityTypes[formName], 'loadPreview'],
      payload: {
        endpoint: formApiPaths[formName] + '/' + formGetActionTypes[formName],
        data: { id }
      },
      schemas: formSchemas[formName],
      formName
    })
  )
}

const setAreaInformation = ({ formName, areaInformation, dispatch }) => {
  dispatch(
    change(
      formName,
      'areaInformation',
      areaInformation
    )
  )
}

const setFundingType = ({ formName, fundingType, dispatch }) => {
  dispatch(
    change(
      formName,
      'fundingType',
      fundingType
    )
  )
}

export const loadDefaultAreaInformationForOrganization = (organization, formName, useDefaultArea = false) =>
  ({ getState, dispatch }) => {
    if (!organization) {
      setAreaInformation({
        formName,
        areaInformation: useDefaultArea ? getDefaultAreaInformation(getState()) : Map(),
        dispatch
      })
      return
    }

    // commented out, to get refreshed organization data,
    // if this will cause performance issues, need to find other way how to refresh data
    // const areaInformation = getOrganizationAreaInformation(getState(), { organization })
    // if (areaInformation && areaInformation.size) {
    //   setAreaInformation({
    //     formName,
    //     areaInformation,
    //     dispatch
    //   })
    // } else {
    dispatch(apiCall3({
      keys: [formName, 'areaType'],
      payload: {
        endpoint: 'common/GetDefaultAreaInformation',
        data: { organizationId: organization }
      },
      schemas: EntitySchemas.ORGANIZATION_AREA_INFORMATION,
      formName,
      successNextAction: response =>
        setAreaInformation({
          formName,
          // getState is called in success callback to get fresh data from api call
          areaInformation: getOrganizationAreaInformation(getState(), { organization }),
          dispatch
        })
    }))
    // }
  }

export const loadOrganizationFundingType = (organization, formName) =>
  ({ getState, dispatch }) => {
    if (!organization) {
      setFundingType({
        formName,
        'fundingType': null,
        dispatch
      })
      return
    }
    setFundingType({
      formName,
      'fundingType': getFundingTypeForOrganization(getState(), { organization }),
      dispatch
    })
  }

export const resetForms = dirtyFormsMap => ({ dispatch }) => {
  dirtyFormsMap.forEach((v, k) => dispatch(reset(k)))
}
