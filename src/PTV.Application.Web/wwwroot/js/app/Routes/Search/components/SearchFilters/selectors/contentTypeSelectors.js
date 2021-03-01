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
import { createSelector } from 'reselect'
import { getFormValueWithPath } from 'selectors/base'
import { searchContentTypeEnum, contentTypes } from 'enums'
import { messages } from '../messages'

export const getFormContentTypes = createSelector(
  getFormValueWithPath(props => 'contentTypes'),
  contentTypes => contentTypes
)

const getContentTypeMessage = enumType => {
  return enumType && messages[enumType]
}

const createContentTypeFilterObject = enumType => {
  return {
    message: getContentTypeMessage(enumType),
    value: enumType
  }
}

const formContainsContentType = selectedContentTypes => enumType => {
  return (selectedContentTypes &&
    selectedContentTypes.get(enumType) &&
    enumType) || null
}

const tryPushContentType = resultArray => enumType => {
  enumType &&
    resultArray.push(createContentTypeFilterObject(enumType))
}

export const getContentTypeMessages = createSelector(
  getFormContentTypes,
  selectedContentTypes => {
    const arrayQuery = formContainsContentType(selectedContentTypes)
    const serviceService = arrayQuery(searchContentTypeEnum.SERVICESERVICE)
    const serviceProfessional = arrayQuery(searchContentTypeEnum.SERVICEPROFESSIONAL)
    const servicePermit = arrayQuery(searchContentTypeEnum.SERVICEPERMIT)
    const eChannel = arrayQuery(searchContentTypeEnum.ECHANNEL)
    const webPage = arrayQuery(searchContentTypeEnum.WEBPAGE)
    const printableForm = arrayQuery(searchContentTypeEnum.PRINTABLEFORM)
    const phone = arrayQuery(searchContentTypeEnum.PHONE)
    const serviceLocation = arrayQuery(searchContentTypeEnum.SERVICELOCATION)
    const organization = arrayQuery(searchContentTypeEnum.ORGANIZATION)
    const serviceCollection = arrayQuery(searchContentTypeEnum.SERVICECOLLECTION)
    const generalDescription = arrayQuery(searchContentTypeEnum.GENERALDESCRIPTION)
    const result = []
    const pushFn = tryPushContentType(result)

    if (serviceService && serviceProfessional && servicePermit) {
      pushFn(searchContentTypeEnum.SERVICE)
    } else {
      pushFn(serviceService)
      pushFn(serviceProfessional)
      pushFn(servicePermit)
    }

    if (eChannel && webPage && printableForm && phone && serviceLocation) {
      pushFn(searchContentTypeEnum.CHANNEL)
    } else {
      pushFn(eChannel)
      pushFn(webPage)
      pushFn(printableForm)
      pushFn(phone)
      pushFn(serviceLocation)
    }

    pushFn(organization)
    pushFn(serviceCollection)
    pushFn(generalDescription)

    return result
  }
)

export const getAreAllContentTypesSelected = createSelector(
  getFormContentTypes,
  selectedContentTypes => selectedContentTypes.size === contentTypes.length
)

export const getAreNoContentTypesSelected = createSelector(
  getFormContentTypes,
  selectedContentTypes => !selectedContentTypes || selectedContentTypes.size === 0
)

export const getIsOnlyGDSelected = createSelector(
  getFormContentTypes,
  selectedContentTypes => {
    return selectedContentTypes.size === 1 &&
      !!selectedContentTypes.get(searchContentTypeEnum.GENERALDESCRIPTION)
  }
)

export const getAreServiceSubfiltersVisible = createSelector(
  [getFormContentTypes, getIsOnlyGDSelected],
  (selectedContentTypes, isOnlyGD) => {
    const notServiceList = selectedContentTypes.filter((value, key) => {
      return key !== searchContentTypeEnum.SERVICE &&
        key !== searchContentTypeEnum.SERVICESERVICE &&
        key !== searchContentTypeEnum.SERVICEPROFESSIONAL &&
        key !== searchContentTypeEnum.SERVICEPERMIT
    })

    return (selectedContentTypes.size !== 0 && notServiceList.size === 0) || isOnlyGD
  }
)
