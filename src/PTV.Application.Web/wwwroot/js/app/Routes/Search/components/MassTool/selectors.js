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
  getFormValue
} from 'selectors/base'
import { getFormValues } from 'redux-form/immutable'
import { Map, List } from 'immutable'
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { getIsRegionUserOrganization } from 'selectors/userInfo'
import {
  getEntityInfo,
  formTypesEnum,
  massToolTypes,
  permisionTypes,
  securityOrganizationCheckTypes,
  entityTypesEnum
} from 'enums'
import { getJSUIStateByKey } from 'util/withState/withState'
import {
  // getPublishingStatusDraftId,
  // getPublishingStatusModifiedId,
  getPublishingStatusDeletedId,
  getPublishingStatusOldPublishedId,
  getPublishingStatusPublishedId
} from 'selectors/common'
import { getIsAccessible } from 'appComponents/Security/selectors'

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

const getSelectedLanguages = createSelector(
  getSelectedEntities,
  selected => selected.map(entity => entity.get('languagesAvailabilities') || List())
)

export const getSelectedLanguageVersionsCount = createSelector(
  getSelectedLanguages,
  EntitySelectors.languages.getEntities,
  getFormValue('languages'),
  (selected, languages, filterLanguages) => selected.reduce(
    (sum, lang) => sum + lang.size, 0)
  // (sum, lang) => sum + lang.filter(
  //   l => !filterLanguages.includes(languages.getIn([l.get('languageId'), 'code']))
  // ).size, 0)
)

export const getSelectedCount = createSelector(
  getSelected,
  selected => selected.size || 0
)

// const getFilterLanguages = createSelector(
//   getFormValue('type'),
//   getFormValue('languages'),
//   getTranslatableLanguages,
//   (massToolType, selectedLanguages, translatableLanguages) => {
//     if (massToolType === massToolTypes.PUBLISH && selectedLanguages.size > 0) {
//       return selectedLanguages
//     }
//     return translatableLanguages
//   }
// )

export const getEntitiesForReview = createSelector(
  getSelectedEntities,
  EntitySelectors.languages.getEntities,
  // getFilterLanguages,
  (
    selected,
    languages,
    // filterLanguages
  ) => selected.reduce(
    (reviewList, entity) =>
      reviewList.concat(
        entity
          .get('languagesAvailabilities')
          // .filter(l => filterLanguages.includes(languages.getIn([l.get('languageId'), 'code'])))
          .map(lang => Map({
            id: entity.get('id'),
            unificRootId: entity.get('unificRootId'),
            language: languages.getIn([lang.get('languageId'), 'code']),
            languageId: lang.get('languageId'),
            meta: entity.get('meta')
          }))),
    List()
  )
)

export const getEntitiesForReviewCount = createSelector(
  getEntitiesForReview,
  entities => entities.size || 0
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
  [massToolTypes.COPY]: permisionTypes.create
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

export const getDisabledForRow = createCachedSelector(
  getPublishingStatusDeletedId,
  getPublishingStatusOldPublishedId,
  getPublishingStatusPublishedId,
  getParameterFromProps('languagesAvailabilities'),
  getParameterFromProps('publishingStatusId'),
  getParameterFromProps('massToolType'),
  getIsEntityAccessible,
  (deletedId, oldPublishedId, publishedId, la, entityStatusId, massToolType, isAccessible) => {
    switch (massToolType) {
      case massToolTypes.PUBLISH:
        return !isAccessible ||
          entityStatusId === deletedId ||
          entityStatusId === oldPublishedId // ||
          // la.every(v => v.statusId === publishedId)
      case massToolTypes.ARCHIVE:
        return !isAccessible ||
          entityStatusId === deletedId ||
          entityStatusId === oldPublishedId
      case massToolTypes.COPY:
        return !isAccessible
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
  getFormValues(formTypesEnum.MASSTOOLSELECTIONFORM),
  formValues => (formValues && formValues.get('selected')) || Map()
)
