import { EnumsSelectors } from 'selectors'
import { createSelector } from 'reselect'
import { getObjectArray } from 'selectors/base'

export const getAreaInformationTypesObjectArray = createSelector(
    EnumsSelectors.areaInformationTypes.getEntities,
    areaInformationTypes => getObjectArray(areaInformationTypes)
)
