import { EntitySelectors } from 'selectors'
import { createSelector } from 'reselect'
import { List } from 'immutable'
import { getObjectArray } from 'selectors/base'
import { createTranslatedIdSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'

export const getServiceChannelConnectionTypesWithoutCommonFor = createSelector(
  EntitySelectors.serviceChannelConnectionTypes.getEntities,
  serviceChannelConnectionTypes => serviceChannelConnectionTypes.filter(
    sct => sct.get('code') !== 'CommonFor'
  ) || List()
)

export const getServiceChannelConnectionTypesObjectArray = createSelector(
  getServiceChannelConnectionTypesWithoutCommonFor,
  serviceChannelConnectionTypes => getObjectArray(serviceChannelConnectionTypes)
)

const getConnectionTypeData = (_, { data }) => data

export const getConnectionTypeTitle = createTranslatedIdSelector(
  getConnectionTypeData, {
    languageTranslationType: languageTranslationTypes.both
  }
)
