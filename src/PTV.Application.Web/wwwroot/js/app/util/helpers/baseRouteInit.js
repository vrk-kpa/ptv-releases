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
import { entityConcreteTypesEnum, entityTypesEnum, formTypesEnum } from 'enums'
import { setSelectedEntity, setComparisionLanguage } from 'reducers/selections'
import { getSubmitSuccess } from 'selectors/formStates'
import {
  setReadOnly,
  setFormLoading,
  setIsAddingNewLanguage,
  disableForm,
  enableForm,
  resetSubmitSuccess,
  setCompareMode
} from 'reducers/formStates'
import { getChannel as getElectronicChannel } from 'Routes/Channels/routes/Electronic/actions'
import { getChannel as getPhoneChannel } from 'Routes/Channels/routes/Phone/actions'
import { getChannel as getPrintableFormChannel } from 'Routes/Channels/routes/PrintableForm/actions'
import { getChannel as getServiceLocationChannel } from 'Routes/Channels/routes/ServiceLocation/actions'
import { getChannel as getWebPageChannel } from 'Routes/Channels/routes/WebPage/actions'
import { getService } from 'Routes/Service/actions'
import { getOrganization } from 'Routes/Organization/actions'
import { getGeneralDescription } from 'Routes/GeneralDescription/actions'
import { API_CALL_CLEAN } from 'Containers/Common/Actions'

export const routeInit = (store, nextState, entityConcreteType) => {
  const { dispatch, getState } = store
  const { params: { id } } = nextState
  let routeSetting
  switch (entityConcreteType) {
    case entityConcreteTypesEnum.ELECTRONICCHANNEL:
      routeSetting = {
        entityType:entityTypesEnum.CHANNELS,
        formType:formTypesEnum.ELECTRONICCHANNELFORM,
        action:getElectronicChannel
      }
      break
    case entityConcreteTypesEnum.PHONECHANNEL:
      routeSetting = {
        entityType:entityTypesEnum.CHANNELS,
        formType:formTypesEnum.PHONECHANNELFORM,
        action:getPhoneChannel
      }
      break
    case entityConcreteTypesEnum.PRINTABLEFORMCHANNEL:
      routeSetting = {
        entityType:entityTypesEnum.CHANNELS,
        formType:formTypesEnum.PRINTABLEFORM,
        action:getPrintableFormChannel
      }
      break
    case entityConcreteTypesEnum.SERVICELOCATIONCHANNEL:
      routeSetting = {
        entityType:entityTypesEnum.CHANNELS,
        formType:formTypesEnum.SERVICELOCATIONFORM,
        action:getServiceLocationChannel
      }
      break
    case entityConcreteTypesEnum.WEBPAGECHANNEL:
      routeSetting = {
        entityType:entityTypesEnum.CHANNELS,
        formType:formTypesEnum.WEBPAGEFORM,
        action:getWebPageChannel
      }
      break
    case entityConcreteTypesEnum.SERVICE:
      routeSetting = {
        entityType:entityTypesEnum.SERVICES,
        formType:formTypesEnum.SERVICEFORM,
        action:getService
      }
      break
    case entityConcreteTypesEnum.GENERALDESCRIPTION:
      routeSetting = {
        entityType:entityTypesEnum.GENERALDESCRIPTIONS,
        formType:formTypesEnum.GENERALDESCRIPTIONFORM,
        action:getGeneralDescription
      }
      break
    case entityConcreteTypesEnum.ORGANIZATION:
      routeSetting = {
        entityType:entityTypesEnum.ORGANIZATIONS,
        formType:formTypesEnum.ORGANIZATIONFORM,
        action:getOrganization
      }
      break
  }

  dispatch(setCompareMode({
    form: routeSetting.formType,
    value: false
  }))
  dispatch(setComparisionLanguage({
    id: null,
    code: null
  }))
  dispatch({
    type: API_CALL_CLEAN,
    keys: ['connectionsChannelSearch']
  })
  dispatch({
    type: API_CALL_CLEAN,
    keys: ['connectionsServiceSearch']
  })
  if (id) {
    [
      setSelectedEntity({ id,
        type: routeSetting.entityType,
        concreteType: entityConcreteType }),
      setReadOnly({ form: routeSetting.formType, value: true }),
      setReadOnly({ form: formTypesEnum.CONNECTIONS, value: true }),
      enableForm(routeSetting.formType)
    ].forEach(dispatch)

    if (!getSubmitSuccess(routeSetting.formType)(getState())) {
      dispatch(routeSetting.action(id))
    } else {
      dispatch(resetSubmitSuccess(routeSetting.formType))
    }
  } else {
    [
      setFormLoading({ form: routeSetting.formType, value: true }),
      setSelectedEntity({ id: null,
        type: routeSetting.entityType,
        concreteType: entityConcreteType }),
      setReadOnly({ form: routeSetting.formType, value: false }),
      setIsAddingNewLanguage({ form: routeSetting.formType, value: true }),
      disableForm(routeSetting.formType)
    ].forEach(dispatch)
    setTimeout(() => dispatch(setFormLoading({ form: routeSetting.formType, value: false })), 1)
  }
}

