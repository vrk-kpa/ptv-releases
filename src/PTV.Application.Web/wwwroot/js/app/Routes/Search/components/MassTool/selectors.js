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
import createCachedSelector from 're-reselect'
import {
  getParameterFromProps,
  getFormValue,
  getUiSortingData
} from 'selectors/base'
import { Map, List, Set } from 'immutable'
import { EntitySelectors } from 'selectors'
import { getIsRegionUserOrganization, getUserRoleName } from 'selectors/userInfo'
import {
  getEntityInfo,
  formTypesEnum,
  massToolTypes,
  permisionTypes,
  securityOrganizationCheckTypes,
  entityTypesEnum,
  taskTypesEnum
} from 'enums'
import { getJSUIStateByKey } from 'util/withState/withState'
import {
  getPublishingStatusDeletedId,
  getPublishingStatusOldPublishedId,
  getPublishingStatusPublishedId,
  getPublishingStatusDraftId,
  getPublishingStatusModifiedId,
  getTranslationLanguageCodes
} from 'selectors/common'
import { getIsAccessible } from 'appComponents/Security/selectors'
import { getDomainSearchResults } from 'Routes/Search/selectors'
import { getTasksServicesEntities } from 'Routes/Tasks/selectors'

import { sortUIData } from 'util/helpers'
import { messages } from './messages'

const getSelected = createSelector(
  getParameterFromProps('selected'),
  selected =>
    selected && selected.filter(x => x) || Map()
)

const getSelectedEntities = createSelector(
  getSelected,
  EntitySelectors.getEntities,
  (selected, entities) => selected.map((v, unificRootId) => {
    const meta = getEntityInfo(v.get('entityType'), v.get('subEntityType'))
    return (entities.getIn([meta.type, v.get('id')]) || Map()).set('meta', Map(meta))
  })
)

const getSelectedEntitiesJS = createSelector(
  getSelectedEntities,
  EntitySelectors.getEntities,
  getTranslationLanguageCodes,
  EntitySelectors.previousInfos.getEntities,
  (selected, entities, languages, previousInfosEntities) => {
    const list = selected.map((x, index) => {
      const type = x.getIn(['meta', 'type'])
      const id = x.get('id')
      const unificRootId = x.get('unificRootId')
      const names = entities.getIn([type, id, 'name']) || Map()
      const language = languages.find(lang => names.get(lang))
      const previousInfo = previousInfosEntities.get(unificRootId) || Map()
      const lastModifiedExist = previousInfo && previousInfo.get('lastModifiedId') || false
      return {
        index,
        id,
        unificRootId: unificRootId,
        type,
        name: names.get(language) || '',
        languagesAvailabilities: x.get('languagesAvailabilities').toJS(),
        language,
        canBeRestore: !lastModifiedExist,
        publishingStatusId: x.get('publishingStatusId')
      }
    }).filter(x => x) || List()
    return list.toArray()
  }
)

export const getSortedSelectedEntitiesJS = createSelector(
  getSelectedEntitiesJS,
  getUiSortingData,
  (selected, uiSortingData) => {
    const column = uiSortingData.get('column')
    const direction = uiSortingData.get('sortDirection')
    return sortUIData(selected, column, direction)
  }
)

const getSelectedLanguages = createSelector(
  getSelectedEntities,
  selected => selected.map(entity => entity.get('languagesAvailabilities') || List())
)

export const getSelectedLanguageVersionsCount = createSelector(
  getSelectedLanguages,
  EntitySelectors.languages.getEntities,
  getFormValue('languages'),
  (selected, languages, filterLanguages) => selected.reduce(
    (sum, lang) =>
      filterLanguages && filterLanguages.size
        ? sum + lang.count(l => filterLanguages.has(languages.getIn([l.get('languageId'), 'code'])))
        : sum + lang.size
    , 0)
)

// export const getSelectedCount = createSelector(
//   getSelected,
//   selected => selected.size || 0
// )

export const getSelectedCount = createSelector(
  getSelectedLanguages,
  EntitySelectors.languages.getEntities,
  getFormValue('languages'),
  (selected, languages, filterLanguages) => {
    if (filterLanguages && filterLanguages.size) {
      return selected.reduce(
        (sum, lang) => {
          return lang.count(l => filterLanguages.has(languages.getIn([l.get('languageId'), 'code'])))
            ? sum + 1
            : sum
        }, 0)
    }
    return selected.size || 0
  }
)

// const getFilteredEntityLanguages = (languagesAvailabilities, filterLanguages, languages) =>
//   filterLanguages && filterLanguages.size
//     ? languagesAvailabilities.filter(l => filterLanguages.has(languages.getIn([l.get('languageId'), 'code'])))
//     : languagesAvailabilities

export const getEntitiesForReview = createSelector(
  getSelectedEntities,
  EntitySelectors.languages.getEntities,
  getFormValue('languages'),
  (
    selected,
    languages,
    filterLanguages
  ) => {
    let index = 0
    let step = 0
    return selected.reduce(
      (reviewList, entity) => reviewList.concat(
      // getFilteredEntityLanguages(entity.get('languagesAvailabilities'), filterLanguages, languages)
        entity.get('languagesAvailabilities')
          .map(lang => {
            const languageId = lang.get('languageId')
            const language = languages.getIn([languageId, 'code'])
            const useForReview = !filterLanguages || !filterLanguages.size || filterLanguages.has(language)
            const unificRootId = entity.get('unificRootId')
            return Map({
              id: entity.get('id'),
              index: index++,
              unificRootId,
              language,
              languageId,
              meta: entity.get('meta'),
              useForReview,
              step: useForReview ? step++ : null
            })
          })
      ),
      List()
    )
  }
)

const initialState = Map()

const getMassToolState = state =>
  getJSUIStateByKey(state, { initialState }, formTypesEnum.MASSTOOLFORM)

export const getIsMassToolActive = createSelector(
  getMassToolState,
  massTool => massTool && massTool.isVisible || false
)

const massToolPermisions = {
  [massToolTypes.PUBLISH]: permisionTypes.publish,
  [massToolTypes.ARCHIVE]: permisionTypes.delete,
  [massToolTypes.COPY]: permisionTypes.create,
  [massToolTypes.RESTORE]: permisionTypes.update
}

const getOrganizationCheckType = (entityType, massToolType, isRegion) => {
  // if (massToolType === massToolTypes.COPY && isRegion) {
  if (massToolType === massToolTypes.COPY) {
    return securityOrganizationCheckTypes.ownOrganization
  }
  return entityType === entityTypesEnum.GENERALDESCRIPTIONS
    ? securityOrganizationCheckTypes.otherOrganization
    : securityOrganizationCheckTypes.byOrganization
}

export const getIsEntityAccessible = (state, props) => {
  const { entityType, subEntityType, massToolType, organizationId, unificRootId } = props
  const info = getEntityInfo(entityType, subEntityType)
  const isRegion = getIsRegionUserOrganization(state)
  // console.log(permisionTypes[massToolType], info, props, isRegion)
  return getIsAccessible(state, {
    domain: `${info.type}MassTool`,
    permisionType: massToolPermisions[massToolType],
    organization: info.type === entityTypesEnum.ORGANIZATIONS ? unificRootId : organizationId,
    formName: 'unknown',
    checkOrganization: getOrganizationCheckType(info.type, massToolType, isRegion)
  })
}

const getIsUserEeva = createSelector(
  getUserRoleName,
  userRoleName => userRoleName === 'Eeva'
)

export const getDisabledForRow = createCachedSelector(
  getPublishingStatusDeletedId,
  getPublishingStatusOldPublishedId,
  getPublishingStatusPublishedId,
  getPublishingStatusDraftId,
  getPublishingStatusModifiedId,
  getParameterFromProps('publishingStatusId'),
  getParameterFromProps('massToolType'),
  getIsEntityAccessible,
  getParameterFromProps('hasTranslationOrder'),
  getParameterFromProps('hasAstiConnection'),
  getIsUserEeva,
  (
    deletedId,
    oldPublishedId,
    publishedId,
    draftId,
    modifiedId,
    entityStatusId,
    massToolType,
    isAccessible,
    hasTranslationOrder,
    hasAstiConnection,
    isUserEeva
  ) => {
    switch (massToolType) {
      case massToolTypes.PUBLISH:
        return hasTranslationOrder ||
          !isAccessible ||
          entityStatusId === deletedId ||
          entityStatusId === oldPublishedId
      case massToolTypes.ARCHIVE:
        return !isAccessible ||
          entityStatusId === deletedId ||
          entityStatusId === oldPublishedId ||
          (hasAstiConnection && !isUserEeva)
      case massToolTypes.COPY:
        return !isAccessible ||
        entityStatusId === deletedId ||
        entityStatusId === oldPublishedId
      case massToolTypes.RESTORE:
        return !isAccessible ||
          entityStatusId === draftId ||
          entityStatusId === modifiedId ||
          entityStatusId === publishedId
    }
    return false
  }
)(
  getParameterFromProps('id')
)

export const getMassToolType = createSelector(
  getFormValue('type'),
  type => type
)

export const getSelectedEntitiesForMassOperation = createSelector(
  getFormValue('selected'),
  selected => selected || Map()
)

const restoreModifiedExists = Map({ message: messages.contentExistInModifiedVersion })

export const getErrors = createSelector(
  getParameterFromProps('canBeRestore'),
  getMassToolType,
  (canBeRestored, massToolType) => massToolType === massToolTypes.RESTORE &&
    !canBeRestored ? List([restoreModifiedExists]) : List()
)

export const getErrorTitle = createSelector(
  getMassToolType,
  massToolType => massToolType === massToolTypes.RESTORE && messages.contentCannotBeRestore || null
)

export const getApprovedId = createSelector(
  getMassToolType,
  getParameterFromProps('id'),
  getParameterFromProps('unificRootId'),
  (massToolType, id, unificRootId) => massToolType === massToolTypes.ARCHIVE ? id : unificRootId
)

const getSearchedEntities = createSelector(
  getParameterFromProps('taskType'),
  getTasksServicesEntities,
  getDomainSearchResults,
  (taskType, tasksSearchResults, frontPageSearchResults) => {
    switch (taskType) {
      case taskTypesEnum.OUTDATEDDRAFTSERVICES:
      case taskTypesEnum.OUTDATEDPUBLISHEDSERVICES:
      case taskTypesEnum.OUTDATEDDRAFTCHANNELS:
      case taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS:
        return tasksSearchResults
      default:
        return frontPageSearchResults
    }
  }
)

const getSearchIdsSet = createSelector(
  getSearchedEntities,
  getMassToolType,
  (entities, massToolType) => {
    const id = massToolType === massToolTypes.ARCHIVE ? 'id' : 'unificRootId'
    return entities.reduce((ids, entity) => ids.add(entity.get(id)), Set())
  }
)

const getDisablededIdsSet = (state, props) => {
  const { formName, taskType } = props
  const massToolType = getMassToolType(state, { formName })
  const searchEntities = getSearchedEntities(state, { taskType })
  const idList = searchEntities.reduce((ids, entity, key) => {
    const publishingStatusId = entity.get('publishingStatusId')
    const hasTranslationOrder = entity.get('hasTranslationOrder')
    const hasAstiConnection = entity.get('hasAstiConnection')
    const entityType = entity.get('entityType')
    const subEntityType = entity.get('subEntityType')
    const organizationId = entity.get('organizationId')
    const unificRootId = entity.get('unificRootId')
    const id = entity.get('id')
    const isUserEeva = getIsUserEeva(state, props)

    const condition = getDisabledForRow(state, {
      id,
      massToolType,
      publishingStatusId,
      hasTranslationOrder,
      hasAstiConnection,
      entityType,
      subEntityType,
      organizationId,
      unificRootId,
      isUserEeva
    })
    const identifierName = massToolType === massToolTypes.ARCHIVE ? 'id' : 'unificRootId'
    return ids.push({ id: entity.get(identifierName), disabled: condition })
  }, List())
  const disabledSet = idList.filter(item => item.disabled).map(item => item.id).toSet()
  const enabledSet = idList.filter(item => !item.disabled).map(item => item.id).toSet()
  return disabledSet.subtract(disabledSet.intersect(enabledSet))
}

const getSelectedIdsSet = createSelector(
  getSelectedEntitiesForMassOperation,
  selected => selected.map((entity, key) => entity && key).toSet()
)

const getAvailableSet = createSelector(
  getSearchIdsSet,
  getDisablededIdsSet,
  (searched, disabled) => searched.subtract(disabled)
)

export const getIsAnyAvailable = createSelector(
  getAvailableSet,
  available => available.size > 0
)

export const getIsBatchCheckboxChecked = createSelector(
  getAvailableSet,
  getSelectedIdsSet,
  (available, selected) => selected.filter(x => x).size > 0 && available.isSubset(selected)
)
