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
import { createSelector } from 'reselect'
import { getFormValueWithPath } from 'selectors/base'
import { allContentTypesEnum } from 'enums'
import { messages } from '../messages'

const getFormContentTypes = createSelector(
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
    const serviceService = arrayQuery(allContentTypesEnum.SERVICESERVICE)
    const serviceProfessional = arrayQuery(allContentTypesEnum.SERVICEPROFESSIONAL)
    const servicePermit = arrayQuery(allContentTypesEnum.SERVICEPERMIT)
    const eChannel = arrayQuery(allContentTypesEnum.ECHANNEL)
    const webPage = arrayQuery(allContentTypesEnum.WEBPAGE)
    const printableForm = arrayQuery(allContentTypesEnum.PRINTABLEFORM)
    const phone = arrayQuery(allContentTypesEnum.PHONE)
    const serviceLocation = arrayQuery(allContentTypesEnum.SERVICELOCATION)
    const organization = arrayQuery(allContentTypesEnum.ORGANIZATION)
    const serviceCollection = arrayQuery(allContentTypesEnum.SERVICECOLLECTION)
    const generalDescription = arrayQuery(allContentTypesEnum.GENERALDESCRIPTION)
    const result = []
    const pushFn = tryPushContentType(result)

    if (serviceService && serviceProfessional && servicePermit) {
      pushFn(allContentTypesEnum.SERVICE)
    } else {
      pushFn(serviceService)
      pushFn(serviceProfessional)
      pushFn(servicePermit)
    }

    if (eChannel && webPage && printableForm && phone && serviceLocation) {
      pushFn(allContentTypesEnum.CHANNEL)
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
  selectedContentTypes => {
    // all except of 2 "group" values - service and channel
    const availableContentTypesCount = Object.keys(allContentTypesEnum).length - 2
    return selectedContentTypes.size === availableContentTypesCount
  }
)

export const getAreNoContentTypesSelected = createSelector(
  getFormContentTypes,
  selectedContentTypes => !selectedContentTypes || selectedContentTypes.size === 0
)

export const getIsOnlyGDSelected = createSelector(
  getFormContentTypes,
  selectedContentTypes => {
    return selectedContentTypes.size === 1 &&
      !!selectedContentTypes.get(allContentTypesEnum.GENERALDESCRIPTION)
  }
)

export const getAreServiceSubfiltersVisible = createSelector(
  [getFormContentTypes, getIsOnlyGDSelected],
  (selectedContentTypes, isOnlyGD) => {
    const notServiceList = selectedContentTypes.filter((value, key) => {
      return key !== allContentTypesEnum.SERVICE &&
        key !== allContentTypesEnum.SERVICESERVICE &&
        key !== allContentTypesEnum.SERVICEPROFESSIONAL &&
        key !== allContentTypesEnum.SERVICEPERMIT
    })

    return (selectedContentTypes.size !== 0 && notServiceList.size === 0) || isOnlyGD
  }
)
