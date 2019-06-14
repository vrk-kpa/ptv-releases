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
import { stringUpperEnumFactory } from './factory'
import { entityConcreteTypesEnum, entityTypesEnum } from './types/entities'
import { property, camelCase } from 'lodash'

const formTypes = [
  'serviceForm',
  'generalDescriptionForm',
  'organizationForm',
  'electronicChannelForm',
  'phoneChannelForm',
  'printableForm',
  'serviceLocationForm',
  'webPageForm',
  'generalDescriptionSearchForm',
  'connections',
  'ASTIConnections',
  'searchConnectionsForm',
  'preview',
  'translationOrderForm',
  'connectionsWorkbench',
  'copyEntityForm',
  'serviceCollectionForm',
  'frontPageSearch',
  'massToolForm',
  'massToolSelectionForm',
  'serviceLocationAddressSearchForm'
]
export const formTypesEnum = stringUpperEnumFactory(formTypes)

export const formChannelTypes = {
  [formTypesEnum.ELECTRONICCHANNELFORM]: 'EChannel',
  [formTypesEnum.PHONECHANNELFORM]: 'Phone',
  [formTypesEnum.PRINTABLEFORM]: 'PrintableForm',
  [formTypesEnum.SERVICELOCATIONFORM]: 'ServiceLocation',
  [formTypesEnum.WEBPAGEFORM]: 'WebPage'
}

export const formOtherTypes = {
  [formTypesEnum.SERVICEFORM]: 'service',
  [formTypesEnum.GENERALDESCRIPTIONFORM]: 'generalDescription',
  [formTypesEnum.ORGANIZATIONFORM]: 'organization',
  [formTypesEnum.SERVICECOLLECTIONFORM]: 'serviceCollection'
}

export const formAllTypes = {
  ...formChannelTypes,
  ...formOtherTypes
}

export const formPaths = {
  [formTypesEnum.SERVICEFORM]: '/service',
  [formTypesEnum.GENERALDESCRIPTIONFORM]: '/generalDescription',
  [formTypesEnum.ORGANIZATIONFORM]: '/organization',
  [formTypesEnum.ELECTRONICCHANNELFORM]: '/channels/electronic',
  [formTypesEnum.PHONECHANNELFORM]: '/channels/phone',
  [formTypesEnum.PRINTABLEFORM]: '/channels/printableForm',
  [formTypesEnum.SERVICELOCATIONFORM]: '/channels/serviceLocation',
  [formTypesEnum.WEBPAGEFORM]: '/channels/webPage',
  [formTypesEnum.GENERALDESCRIPTIONSEARCHFORM]: '/noPath',
  [formTypesEnum.SERVICECOLLECTIONFORM]:'/serviceCollection'
}

export const formApiPaths = {
  [formTypesEnum.SERVICEFORM]: 'service',
  [formTypesEnum.GENERALDESCRIPTIONFORM]: 'generalDescription',
  [formTypesEnum.ORGANIZATIONFORM]: 'organization',
  [formTypesEnum.ELECTRONICCHANNELFORM]: 'channel',
  [formTypesEnum.PHONECHANNELFORM]: 'channel',
  [formTypesEnum.PRINTABLEFORM]: 'channel',
  [formTypesEnum.SERVICELOCATIONFORM]: 'channel',
  [formTypesEnum.WEBPAGEFORM]: 'channel',
  [formTypesEnum.SERVICECOLLECTIONFORM]: 'serviceCollection'
  // [formTypesEnum.GENERALDESCRIPTIONSEARCHFORM]: 'noPath'
}

export const formMainEntityType = formApiPaths

export const formEntityTypes = {
  [formTypesEnum.SERVICEFORM]: entityTypesEnum.SERVICES,
  [formTypesEnum.GENERALDESCRIPTIONFORM]: entityTypesEnum.GENERALDESCRIPTIONS,
  [formTypesEnum.ORGANIZATIONFORM]: entityTypesEnum.ORGANIZATIONS,
  [formTypesEnum.ELECTRONICCHANNELFORM]: entityTypesEnum.CHANNELS,
  [formTypesEnum.PHONECHANNELFORM]: entityTypesEnum.CHANNELS,
  [formTypesEnum.PRINTABLEFORM]: entityTypesEnum.CHANNELS,
  [formTypesEnum.SERVICELOCATIONFORM]: entityTypesEnum.CHANNELS,
  [formTypesEnum.WEBPAGEFORM]: entityTypesEnum.CHANNELS,
  [formTypesEnum.GENERALDESCRIPTIONSEARCHFORM]: 'noType',
  [formTypesEnum.SERVICECOLLECTIONFORM]: entityTypesEnum.SERVICECOLLECTIONS
}

export const formEntityConcreteTypes = {
  [formTypesEnum.SERVICEFORM]: entityConcreteTypesEnum.SERVICE,
  [formTypesEnum.GENERALDESCRIPTIONFORM]: entityConcreteTypesEnum.GENERALDESCRIPTION,
  [formTypesEnum.ORGANIZATIONFORM]: entityConcreteTypesEnum.ORGANIZATION,
  [formTypesEnum.ELECTRONICCHANNELFORM]: entityConcreteTypesEnum.ELECTRONICCHANNEL,
  [formTypesEnum.PHONECHANNELFORM]: entityConcreteTypesEnum.PHONECHANNEL,
  [formTypesEnum.PRINTABLEFORM]: entityConcreteTypesEnum.PRINTABLEFORMCHANNEL,
  [formTypesEnum.SERVICELOCATIONFORM]: entityConcreteTypesEnum.SERVICELOCATIONCHANNEL,
  [formTypesEnum.WEBPAGEFORM]: entityConcreteTypesEnum.WEBPAGECHANNEL,
  [formTypesEnum.SERVICECOLLECTIONFORM]: entityConcreteTypesEnum.SERVICECOLLECTION
}

export const formActionsTypes = [
  'lockEntity',
  'unLockEntity',
  'withdrawEntity',
  'withdrawLanguage',
  'restoreEntity',
  'restoreLanguage',
  'archiveEntity',
  'archiveLanguage',
  'getValidatedEntity',
  'publishEntity',
  'scheduleEntity',
  'isConnectable',
  'removeEntity']

export const formActionsTypesEnum = stringUpperEnumFactory(formActionsTypes)

export const formGetActionTypes = {
  [formTypesEnum.SERVICEFORM]: 'GetService',
  [formTypesEnum.GENERALDESCRIPTIONFORM]: 'GetGeneralDescription',
  [formTypesEnum.ORGANIZATIONFORM]: 'GetOrganization',
  [formTypesEnum.ELECTRONICCHANNELFORM]: 'GetElectronicChannel',
  [formTypesEnum.PHONECHANNELFORM]: 'GetPhoneChannel',
  [formTypesEnum.PRINTABLEFORM]: 'GetPrintableFormChannel',
  [formTypesEnum.SERVICELOCATIONFORM]: 'GetServiceLocationChannel',
  [formTypesEnum.WEBPAGEFORM]: 'GetWebPageChannel',
  [formTypesEnum.SERVICECOLLECTIONFORM]: 'GetServiceCollection'
}

export const formActions = formActionsTypes.reduce((fA, action) => {
  fA[action] = formTypes.reduce((fT, form) => {
    const path = formApiPaths[form]
    if (path) {
      fT[form] = formApiPaths[form] + '/' + action
    }
    return fT
  }, {})
  return fA
}, {})

const entitySetup = {
  service: {
    service: {
      keyToState: 'service',
      path: '/service',
      type: 'services'
    }
  },
  channel: {
    eChannel: {
      keyToState: 'eChannel',
      path: '/channels/electronic',
      type: 'channels'
    },
    webPage: {
      keyToState: 'webPage',
      path: '/channels/webPage',
      type: 'channels'
    },
    printableForm: {
      keyToState: 'printableForm',
      path: '/channels/printableForm',
      type: 'channels'
    },
    phone: {
      keyToState: 'phone',
      path: '/channels/phone',
      type: 'channels'
    },
    serviceLocation: {
      keyToState: 'serviceLocation',
      path: '/channels/serviceLocation',
      type: 'channels'
    }
  },
  organization: {
    organization: {
      keyToState: 'organization',
      path: '/organization',
      type: 'organizations'
    }
  },
  generalDescription: {
    generalDescription: {
      keyToState: 'generalDescription',
      path: '/generalDescription',
      type: 'generalDescriptions'
    }
  },
  serviceCollection: {
    serviceCollection: {
      keyToState: 'serviceCollection',
      path: '/serviceCollection',
      type: 'serviceCollections'
    }
  }
}

export const getEntityInfo = (main, sub) => property(`${camelCase(main)}.${camelCase(sub)}`)(entitySetup) || {}
