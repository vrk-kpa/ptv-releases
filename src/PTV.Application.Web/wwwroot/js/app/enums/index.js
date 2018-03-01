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

export const customEnumFactory = (enums, setterFunc, keyFunc) => {
  let result = {}
  if (Array.isArray(enums)) {
    enums.map((e, i) => result[keyFunc ? keyFunc(e) : e] = setterFunc ? setterFunc(e, i) : i)
  } else {
    console.error('Given parameter is not array of the strings!')
  }
  return result
}
export const errorDialogKey = 'serverErrorDialog'

export const stringEnumFactory = enums => customEnumFactory(enums, e => e)
export const stringUpperEnumFactory = enums => customEnumFactory(enums, e => e, e => e.toUpperCase())
export const enumFactory = enums => customEnumFactory(enums)

export const getKey = (obj, val) => Object.keys(obj).find(key => obj[key].toLowerCase() === val)

export const addressUseCases = ['delivery', 'postal', 'visiting']
export const addressUseCasesEnum = stringUpperEnumFactory(addressUseCases)

export const addressTypes = ['deliveryAddress', 'visitingAddresses', 'postalAddresses']
export const addressTypesEnum = stringUpperEnumFactory(addressTypes)

export const entityTypes = ['services', 'channels', 'generalDescriptions', 'organizations']
export const entityTypesEnum = stringUpperEnumFactory(entityTypes)

export const userRoleTypes = ['eeva', 'pete', 'shirley']
export const userRoleTypesEnum = stringUpperEnumFactory(userRoleTypes)

export const securityOrganizationCheckTypes = stringEnumFactory([
  'byOrganization',
  'ownOrganization',
  'otherOrganization'
])
export const permisionTypes = {
  create: 1,
  read: 2,
  update: 4,
  delete: 8,
  publish: 16
}

export const channelsTypes = [
  'electronicChannel',
  'phoneChannel',
  'printableFormChannel',
  'serviceLocationChannel',
  'webPageChannel'
]
export const entityConcreteTypes = [
  'service',
  'generalDescription',
  'organization',
  ...channelsTypes
]
export const entityConcreteTypesEnum = stringUpperEnumFactory(entityConcreteTypes)
export const channelTypesEnum = stringUpperEnumFactory(channelsTypes)

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
  'connectionsWorkbench'
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
  [formTypesEnum.ORGANIZATIONFORM]: 'organization'
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
  [formTypesEnum.GENERALDESCRIPTIONSEARCHFORM]: '/noPath'
}

export const formApiPaths = {
  [formTypesEnum.SERVICEFORM]: 'service',
  [formTypesEnum.GENERALDESCRIPTIONFORM]: 'generalDescription',
  [formTypesEnum.ORGANIZATIONFORM]: 'organization',
  [formTypesEnum.ELECTRONICCHANNELFORM]: 'channel',
  [formTypesEnum.PHONECHANNELFORM]: 'channel',
  [formTypesEnum.PRINTABLEFORM]: 'channel',
  [formTypesEnum.SERVICELOCATIONFORM]: 'channel',
  [formTypesEnum.WEBPAGEFORM]: 'channel'
  // [formTypesEnum.GENERALDESCRIPTIONSEARCHFORM]: 'noPath'
}

export const formEntityTypes = {
  [formTypesEnum.SERVICEFORM]: entityTypesEnum.SERVICES,
  [formTypesEnum.GENERALDESCRIPTIONFORM]: entityTypesEnum.GENERALDESCRIPTIONS,
  [formTypesEnum.ORGANIZATIONFORM]: entityTypesEnum.ORGANIZATIONS,
  [formTypesEnum.ELECTRONICCHANNELFORM]: entityTypesEnum.CHANNELS,
  [formTypesEnum.PHONECHANNELFORM]: entityTypesEnum.CHANNELS,
  [formTypesEnum.PRINTABLEFORM]: entityTypesEnum.CHANNELS,
  [formTypesEnum.SERVICELOCATIONFORM]: entityTypesEnum.CHANNELS,
  [formTypesEnum.WEBPAGEFORM]: entityTypesEnum.CHANNELS,
  [formTypesEnum.GENERALDESCRIPTIONSEARCHFORM]: 'noType'
}

export const formEntityConcreteTypes = {
  [formTypesEnum.SERVICEFORM]: entityConcreteTypesEnum.SERVICE,
  [formTypesEnum.GENERALDESCRIPTIONFORM]: entityConcreteTypesEnum.GENERALDESCRIPTION,
  [formTypesEnum.ORGANIZATIONFORM]: entityConcreteTypesEnum.ORGANIZATION,
  [formTypesEnum.ELECTRONICCHANNELFORM]: entityConcreteTypesEnum.ELECTRONICCHANNEL,
  [formTypesEnum.PHONECHANNELFORM]: entityConcreteTypesEnum.PHONECHANNEL,
  [formTypesEnum.PRINTABLEFORM]: entityConcreteTypesEnum.PRINTABLEFORMCHANNEL,
  [formTypesEnum.SERVICELOCATIONFORM]: entityConcreteTypesEnum.SERVICELOCATIONCHANNEL,
  [formTypesEnum.WEBPAGEFORM]: entityConcreteTypesEnum.WEBPAGECHANNEL
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
  'publishEntity']

export const formActionsTypesEnum = stringUpperEnumFactory(formActionsTypes)

export const formGetActionTypes = {
  [formTypesEnum.SERVICEFORM]: 'GetService',
  [formTypesEnum.GENERALDESCRIPTIONFORM]: 'GetGeneralDescription',
  [formTypesEnum.ORGANIZATIONFORM]: 'GetOrganization',
  [formTypesEnum.ELECTRONICCHANNELFORM]: 'GetElectronicChannel',
  [formTypesEnum.PHONECHANNELFORM]: 'GetPhoneChannel',
  [formTypesEnum.PRINTABLEFORM]: 'GetPrintableFormChannel',
  [formTypesEnum.SERVICELOCATIONFORM]: 'GetServiceLocationChannel',
  [formTypesEnum.WEBPAGEFORM]: 'GetWebPageChannel'
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
