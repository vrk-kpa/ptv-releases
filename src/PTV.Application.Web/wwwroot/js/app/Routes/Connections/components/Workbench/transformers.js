import { Set, Map } from 'immutable'
import createPhoneTransformer from 'util/redux-form/submitFilters/PhoneNumbers'
import {
  getFilteredList,
  languageMap
} from 'util/redux-form/submitFilters/Mappers'

const mainEntityKeys = ['unificRootId', 'id']
const mainEntityFilter = keyIn(mainEntityKeys)
export const mainEntityTransformer = values => {
  return values.update('connections', connections => {
    return connections.map(connection => {
      return connection.update('mainEntity', mainEntity => {
        return mainEntity.filter(mainEntityFilter)
      })
    })
  })
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
          return child.updateIn(['contactDetails', 'phoneNumbers'], phoneNumbers => {
            return createPhoneTransformer()(phoneNumbers, dispatch, state)
          })
        })
      })
    })
  })
}

export const openingHoursTransformer = (values, dispatch, state) => {
  return values.update('connections', connections => {
    return connections.map(connection => {
      return connection.update('childs', childs => {
        return childs.map(child => {
          if (!child.has('openingHours')) return child
          return child.update('openingHours', openingHours => {
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
