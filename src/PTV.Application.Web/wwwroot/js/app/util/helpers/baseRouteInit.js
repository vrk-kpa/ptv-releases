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

export const createLoadActionByEntityType = concreteType => ({
  [entityConcreteTypesEnum.ELECTRONICCHANNEL]: getElectronicChannel,
  [entityConcreteTypesEnum.PHONECHANNEL]: getPhoneChannel,
  [entityConcreteTypesEnum.PRINTABLEFORMCHANNEL]: getPrintableFormChannel,
  [entityConcreteTypesEnum.SERVICELOCATIONCHANNEL]: getServiceLocationChannel,
  [entityConcreteTypesEnum.WEBPAGECHANNEL]: getWebPageChannel,
  [entityConcreteTypesEnum.SERVICE]: getService,
  [entityConcreteTypesEnum.GENERALDESCRIPTION]: getGeneralDescription,
  [entityConcreteTypesEnum.ORGANIZATION]: getOrganization
})[concreteType]

export const getEntityTypeFromConcreteType = concreteType => {
  if (
    entityConcreteTypesEnum.ELECTRONICCHANNEL === concreteType ||
    entityConcreteTypesEnum.PHONECHANNEL === concreteType ||
    entityConcreteTypesEnum.PRINTABLEFORMCHANNEL === concreteType ||
    entityConcreteTypesEnum.SERVICELOCATIONCHANNEL === concreteType ||
    entityConcreteTypesEnum.WEBPAGECHANNEL === concreteType
  ) {
    return entityTypesEnum.CHANNELS
  }
  return {
    [entityConcreteTypesEnum.SERVICE]: entityTypesEnum.SERVICES,
    [entityConcreteTypesEnum.GENERALDESCRIPTION]: entityTypesEnum.GENERALDESCRIPTIONS,
    [entityConcreteTypesEnum.ORGANIZATION]: entityTypesEnum.ORGANIZATIONS
  }[concreteType]
}

export const getFormNameFromConcreteType = concreteType => ({
  [entityConcreteTypesEnum.ELECTRONICCHANNEL]: formTypesEnum.ELECTRONICCHANNELFORM,
  [entityConcreteTypesEnum.PHONECHANNEL]: formTypesEnum.PHONECHANNELFORM,
  [entityConcreteTypesEnum.PRINTABLEFORMCHANNEL]: formTypesEnum.PRINTABLEFORM,
  [entityConcreteTypesEnum.SERVICELOCATIONCHANNEL]: formTypesEnum.SERVICELOCATIONFORM,
  [entityConcreteTypesEnum.WEBPAGECHANNEL]: formTypesEnum.WEBPAGEFORM,
  [entityConcreteTypesEnum.SERVICE]: formTypesEnum.SERVICEFORM,
  [entityConcreteTypesEnum.GENERALDESCRIPTION]: formTypesEnum.GENERALDESCRIPTIONFORM,
  [entityConcreteTypesEnum.ORGANIZATION]: formTypesEnum.ORGANIZATIONFORM
})[concreteType]

export const routeInit = (store, nextState, entityConcreteType) => {
  const { dispatch, getState } = store
  const { params: { id } } = nextState
  const formName = getFormNameFromConcreteType(entityConcreteType)
  const entityType = getEntityTypeFromConcreteType(entityConcreteType)
  dispatch(setCompareMode({
    form: formName,
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
        type: entityType,
        concreteType: entityConcreteType }),
      setReadOnly({ form: formName, value: true }),
      setReadOnly({ form: formTypesEnum.CONNECTIONS, value: true }),
      setReadOnly({ form: formTypesEnum.ASTICONNECTIONS, value: true }),
      enableForm(formName)
    ].forEach(dispatch)

    if (!getSubmitSuccess(formName)(getState())) {
      const loadAction = createLoadActionByEntityType(entityConcreteType)
      dispatch(loadAction(id))
    } else {
      dispatch(resetSubmitSuccess(formName))
    }
  } else {
    [
      setFormLoading({ form: formName, value: true }),
      setSelectedEntity({ id: null,
        type: entityType,
        concreteType: entityConcreteType }),
      setReadOnly({ form: formName, value: false }),
      setIsAddingNewLanguage({ form: formName, value: true }),
      disableForm(formName)
    ].forEach(dispatch)
    setTimeout(() => dispatch(setFormLoading({ form: formName, value: false })), 1)
  }
}

