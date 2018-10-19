import { createSelector } from 'reselect'
import { List } from 'immutable'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'

const getData = (_, { data }) => data
const getIsInCompareMode = (_, { compare }) => !!compare

const getLanguageCode = createSelector(
  [
    getContentLanguageCode,
    getSelectedComparisionLanguageCode,
    getIsInCompareMode
  ],
  (language, comparisionLanguageCode, compare) => {
    return compare && comparisionLanguageCode || language || 'fi'
  }
)

// SERVICE COLLECTIONS
const getServiceCollectionsData = createSelector(
  getData,
  data => data || List()
)

export const getServiceCollections = createSelector(
  [
    getServiceCollectionsData,
    getLanguageCode
  ],
  (items, lang) => items.map(item => item.getIn(['name', lang]) || '')
)

export const getServiceCollectionTitle = createSelector(
  getServiceCollections,
  items => items.join(', ')
)
