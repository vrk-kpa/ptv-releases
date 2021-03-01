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

import createPhoneTransformer from 'util/redux-form/submitFilters/PhoneNumbers'
import { Map } from 'immutable'

import {
  getFilteredList,
  languageMap
} from 'util/redux-form/submitFilters/Mappers'

import {
  editorStateTransformer,
  Mappers
} from 'util/redux-form/submitFilters'

export const phoneNumbersTransformer = (values, dispatch, state) => {
  return values.update('selectedConnections', conections => {
    return conections.map(conection => {
      return conection.updateIn(['contactDetails'], contactDetail => {
        return createPhoneTransformer()(contactDetail, dispatch, state)
      })
    })
  })
}
export const faxNumbersTransformer = (values, dispatch, state) => {
  return values.update('selectedConnections', conections => {
    return conections.map(conection => {
      return conection.updateIn(['contactDetails'], contactDetail => {
        return createPhoneTransformer('faxNumbers')(contactDetail, dispatch, state)
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
  return values.update('selectedConnections', conections => {
    return conections.map(conection => {
      if (!conection.has('openingHours')) return conection
      return conection.update('openingHours', openingHours => {
        if (!openingHours) return openingHours
        openingHours = openingHours.update('holidayHours', getHolidayList)
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

const convertDateTime = connection => {
  return connection.update('modified', value => {
    if (!value || typeof value === 'string') {
      return value
    }
    return value.toISOString()
  })
}

// move all new items to the end of collection
export const sortItemTransformer = (values, dispatch, state) => {
  return values.update('selectedConnections', conections => {
    return conections.sort((con1, con2) => {
      if (con1.get('modifiedBy') && con2.get('modifiedBy') || !con1.get('modifiedBy') && !con2.get('modifiedBy')) { return 0 }
      if (con1.get('modifiedBy') && !con2.get('modifiedBy')) { return -1 }
      if (!con1.get('modifiedBy') && con2.get('modifiedBy')) { return 1 }
    })
  })
}

export const sortServicesTransformer = (values, dispatch, state) => {
  return values.update('selectedServices', connections => {
    if (!connections) {
      return
    }

    return connections.sort((con1, con2) => {
      if (con1.get('modifiedBy') && con2.get('modifiedBy') || !con1.get('modifiedBy') && !con2.get('modifiedBy')) {
        return 0
      }
      if (con1.get('modifiedBy') && !con2.get('modifiedBy')) { return -1 }
      if (!con1.get('modifiedBy') && con2.get('modifiedBy')) { return 1 }
    }).map(convertDateTime)
  })
}

export const sortChannelsTransformer = (values, dispatch, state) => {
  return values.update('selectedChannels', connections => {
    if (!connections) {
      return
    }

    return connections.sort((con1, con2) => {
      if (con1.get('modifiedBy') && con2.get('modifiedBy') || !con1.get('modifiedBy') && !con2.get('modifiedBy')) {
        return 0
      }
      if (con1.get('modifiedBy') && !con2.get('modifiedBy')) { return -1 }
      if (!con1.get('modifiedBy') && con2.get('modifiedBy')) { return 1 }
    }).map(convertDateTime)
  })
}

export const basicInformationTransformer = (values, dispatch, state) => {
  return values.update('selectedConnections', connections => {
    return connections.map(connection => {
      connection = connection.updateIn(['basicInformation','description'], Mappers.languageMap(editorStateTransformer))
      connection = connection.updateIn(['basicInformation','additionalInformation'], Mappers.languageMap(editorStateTransformer))
      return connection
    })
  })
}

export const serviceCollectionConnectionsTransformer = (values, dispatch, state) => {
  values = values.update('selectedServices', connections => {
    if (!connections) {
      return
    }
    return connections.map(connection => {
      if (connection.get('organizationId') == null){
        const organizationId = connection.getIn(['organization', 'id']) || connection.get('organization') || null 
        connection = connection.update('organizationId', x => x || organizationId)
      }
      return connection.delete('organization')
    })
  })

  values = values.update('selectedChannels', connections => {
    if (!connections) {
      return
    }
    return connections.map(connection => {
      if (connection.get('organizationId') == null){
        const organizationId = connection.getIn(['organization', 'id']) || connection.get('organization') || null 
        connection = connection.update('organizationId', x => x || organizationId)
      }
      return connection.delete('organization')
    })
  })

  return values
}