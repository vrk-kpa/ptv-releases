import { EnumsSelectors } from 'selectors'
import { createSelector } from 'reselect'
import { getObjectArray } from 'selectors/base'

export const getAreaTypesObjectArray = createSelector(
    EnumsSelectors.areaTypes.getEntities,
    areaTypes => getObjectArray(areaTypes)
)
