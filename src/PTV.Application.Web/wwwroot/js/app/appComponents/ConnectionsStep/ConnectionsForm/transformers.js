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

import createPhoneTransformer from 'util/redux-form/submitFilters/PhoneNumbers'
import {
  getFilteredList,
  languageMap
} from 'util/redux-form/submitFilters/Mappers'

export const phoneNumbersTransformer = (values, dispatch, state) => {
  return values.update('selectedConnections', conections => {
    return conections.map(conection => {
      return conection.updateIn(['contactDetails', 'phoneNumbers'], phoneNumbers => {
        return createPhoneTransformer()(phoneNumbers, dispatch, state)
      })
    })
  })
}

export const openingHoursTransformer = (values, dispatch, state) => {
  return values.update('selectedConnections', conections => {
    return conections.map(conection => {
      if (!conection.has('openingHours')) return conection
      return conection.update('openingHours', openingHours => {
        return openingHours.map(hours => {
          return getFilteredList(hours)
        })
      })
    })
  })
}

export const collectionsTransformer = (values, dispatch, state) => {
  return values.update('selectedConnections', conections => {
    return conections.map(conection => {
      if (!conection.has('contactDetails')) return conection
      return conection.update('contactDetails', contactDetails => {
        return contactDetails.map((contactDetail, key) => {
          if (key === 'postalAddresses') return getFilteredList(contactDetail)
          return languageMap(getFilteredList)(contactDetail)
        })
      })
    })
  })
}
