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
import { Set, Map } from 'immutable'
import createPhoneTransformer from 'util/redux-form/submitFilters/PhoneNumbers'
import {
  getFilteredList,
  languageMap
} from 'util/redux-form/submitFilters/Mappers'
import { getConnectionsMainEntity } from 'selectors/selections'

const mainEntityKeys = ['unificRootId', 'id', 'channelTypeId']
const mainEntityFilter = keyIn(mainEntityKeys)
export const mainEntityTransformer = (values, dispatch, state) => {
  const mainEntityType = getConnectionsMainEntity(state)
  values = values.update('connections', connections => {
    return connections.map(connection => {
      return connection.update('mainEntity', mainEntity => {
        return mainEntity.filter(mainEntityFilter)
      })
    })
  })
  values = values.set('mainEntityType', mainEntityType)
  return values
}
const childKeys = [
  'unificRootId',
  'basicInformation',
  'digitalAuthorization',
  'contactDetails',
  'openingHours',
  'id'
]
const childFilter = keyIn(childKeys)
export const childsTransformer = values => {
  return values.update('connections', connections => {
    return connections.map(connection => {
      return connection.update('childs', childs => {
        return childs.map(child => {
          return child.filter(childFilter)
        })
      })
    })
  })
}
export const astiChildsTransformer = values => {
  return values.update('connections', connections => {
    return connections.map(connection => {
      return Map({
        childs: connection
          .get('childs')
          .concat(connection.get('astiChilds')),
        mainEntity: connection.get('mainEntity')
      })
    })
  })
}
export const phoneNumbersTransformer = (values, dispatch, state) => {
  return values.update('connections', connections => {
    return connections.map(connection => {
      return connection.update('childs', childs => {
        return childs.map(child => {
          return child.updateIn(['contactDetails'], contactDetails => {
            return createPhoneTransformer()(contactDetails, dispatch, state)
          })
        })
      })
    })
  })
}
export const faxNumbersTransformer = (values, dispatch, state) => {
  return values.update('connections', connections => {
    return connections.map(connection => {
      return connection.update('childs', childs => {
        return childs.map(child => {
          return child.updateIn(['contactDetails'], contactDetails => {
            return createPhoneTransformer('faxNumbers')(contactDetails, dispatch, state)
          })
        })
      })
    })
  })
}

const getHolidayList = list => {
  const holidayCodes = list.filter(h => h.get('active')).keySeq()
  return holidayCodes.map(code => Map({
    isClosed: !((list.getIn([code, 'type']) && list.getIn([code, 'type']) === 'open') || false),
    code: code,
    active: list.getIn([code, 'active']),
    intervals: list.getIn([code, 'intervals'])
  }))
}

export const openingHoursTransformer = (values, dispatch, state) => {
  return values.update('connections', connections => {
    return connections.map(connection => {
      return connection.update('childs', childs => {
        return childs.map(child => {
          if (!child.has('openingHours')) return child
          return child.update('openingHours', openingHours => {
            openingHours = openingHours.update('holidayHours', getHolidayList)
            return openingHours.map(hours => {
              return getFilteredList(hours)
            })
          })
        })
      })
    })
  })
}

export const collectionsTransformer = (values, dispatch, state) => {
  return values.update('connections', connections => {
    return connections.map(connection => {
      return connection.update('childs', childs => {
        return childs.map(child => {
          if (!child.has('contactDetails')) return child
          return child.update('contactDetails', contactDetails => {
            return contactDetails.map((contactDetail, key) => {
              if (key === 'postalAddresses') return getFilteredList(contactDetail)
              return languageMap(getFilteredList)(contactDetail)
            })
          })
        })
      })
    })
  })
}

const connectionsKeys = ['connections', 'mainEntityType']
const connectionsFilter = keyIn(connectionsKeys)
export const connectionsTransformer = values => values.filter(connectionsFilter)

function keyIn (keys) {
  var keySet = Set(keys)
  return function (v, k) {
    return keySet.has(k)
  }
}
