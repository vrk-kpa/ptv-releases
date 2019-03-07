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
import { EntitySelectors } from 'selectors'
import { createSelector } from 'reselect'
import { Map, List, OrderedSet } from 'immutable'
import { EditorState } from 'draft-js'
import {
  createIsFetchingSelector,
  getApiCalls,
  getIdFromProps,
  getFormValue,
  getParameterFromProps
} from 'selectors/base'
import {
  getContentLanguageCode
} from 'selectors/selections'
import {
  createListLabelValueSelectorJS,
  createListLabelValueSelectorDisabledJS,
  createEntityListWithDefault
} from 'selectors/factory'
import { getGeneralDescriptionTargetGroups, getFormOverrideTargetGroups } from '../../TargetGroupTree/selectors'
import { getGeneralDescriptionServiceClassesIds } from '../../ServiceClassTree/selectors'

const getIsIdDefined = createSelector(
  getIdFromProps,
  id => id != null
)

export const getOntologyTerm = createSelector(
  [EntitySelectors.ontologyTerms.getEntities, getIdFromProps],
  (entities, id) => entities.get(id) || Map()
)

const getOntologyTermChildren = createSelector(
  getOntologyTerm,
  entity => entity.get('children') || List()
)

const getSearch = createSelector(
  [getApiCalls, getParameterFromProps('searchValue')],
  (api, searchValue) =>
    api.getIn(['OntologyTerm', searchValue && (searchValue.value || searchValue), 'search']) || Map()
)

const getAnnotation = createSelector(
  getApiCalls,
  api => api.getIn(['OntologyTerm', 'searchAnnotations']) || Map()
)

export const getIsFetching = createIsFetchingSelector(getSearch)
export const getAnnotationIsFetching = createIsFetchingSelector(getAnnotation)

const getFilteredIds = createSelector(
  [getSearch, getParameterFromProps('searchValue')],
  (search, searchValue) => searchValue && search.get('result') || List()
)

export const getIsAnyOntologyTermFound = createSelector(
  getFilteredIds,
  ids => ids.size
)

const getRootIds = createSelector(
  getFilteredIds,
  getParameterFromProps('value'),
  getParameterFromProps('showSelected'),
  getParameterFromProps('filterSelected'),
  (searchedIds, selected, showSelected, filterSelected) =>
    showSelected
      ? selected.toList()
      : (filterSelected && searchedIds.filter(id => !selected.has(id)) || searchedIds) || List()
)

export const getOntologyTermIds = createSelector(
  [getRootIds, getOntologyTermChildren, getIsIdDefined],
  (top, children, returnChildren) => returnChildren ? children : top
)

const getFormGeneralDescriptionId = createSelector(
  getFormValue('generalDescriptionId'),
  values => values || ''
)

const getGeneralDescription = createSelector(
  [EntitySelectors.generalDescriptions.getEntities, getFormGeneralDescriptionId],
  (generalDescriptions, generalDescriptionId) => generalDescriptions.get(generalDescriptionId) || Map()
)

export const getGeneralDescriptionOntologyTermsIds = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('ontologyTerms') || List()
)

export const getGeneralDescriptionOntologyTermsCount = createSelector(
  getGeneralDescriptionOntologyTermsIds,
  generalDescriptions => generalDescriptions.size
)

const getGeneralDescriptionOntologyTerms = createEntityListWithDefault(
  EntitySelectors.ontologyTerms.getEntities,
  getGeneralDescriptionOntologyTermsIds
)

const getSelectedOntologyTermIds = createSelector([
  getParameterFromProps('ids'),
  getGeneralDescriptionOntologyTermsIds
], (ids, idsFromGD) =>
  ids.filter(id => !idsFromGD.includes(id))
)

const getOntologyTermsForIds = createEntityListWithDefault(
  EntitySelectors.ontologyTerms.getEntities,
  getSelectedOntologyTermIds
)

export const getGDOntologyTermsForIdsJs = createListLabelValueSelectorDisabledJS(
  getGeneralDescriptionOntologyTerms
)

export const getOntologyTermsForIdsJs = createListLabelValueSelectorJS(
  getOntologyTermsForIds,
  getParameterFromProps('disabledAll')
)

const getReadOnlyOntologyThermsIds = createSelector(
  [getFormValue('ontologyTerms'), getGeneralDescriptionOntologyTermsIds],
  (idsOT, idsGDOT) => idsOT.union(idsGDOT)
)

export const getReadOnlyOntologyThermsIdsJs = createListLabelValueSelectorDisabledJS(
  createEntityListWithDefault(EntitySelectors.ontologyTerms.getEntities, getReadOnlyOntologyThermsIds)
)

export const getFormOntologyTermsIds = createSelector(
  getFormValue('ontologyTerms'),
  ids => {
    return ids || OrderedSet()
  }
)

export const getFormAnnotationTermsIds = createSelector(
  getFormValue('annotationTerms'),
  ids => ids || OrderedSet()
)

export const getFormAnnotationTermsCount = createSelector(
  getFormAnnotationTermsIds,
  ids => ids.size
)

export const getAnnotationTermsForIds = createEntityListWithDefault(
  EntitySelectors.ontologyTerms.getEntities,
  getFormAnnotationTermsIds
)

export const getAnnotationTermsForIdsJs = createListLabelValueSelectorJS(
  getAnnotationTermsForIds,
  getParameterFromProps('disabledAll')
)

export const getAnnotationTerm = createSelector(
  [EntitySelectors.ontologyTerms.getEntities, getIdFromProps],
  (entities, node) => {
    const id = node.get('id')
    return id && entities.get(id) || Map()
  }
)

export const getIsAnyOntologyTerm = createSelector(
  [getFormValue('ontologyTerms'), getGeneralDescriptionOntologyTermsIds],
  (otIds, gdIds) => {
    return List(otIds.filter(id => !gdIds.includes(id))).size > 0
  }
)

export const getOntologyTermsCount = createSelector(
  [getFormValue('ontologyTerms'), getGeneralDescriptionOntologyTermsIds],
  (otIds, gdIds) => {
    return List(otIds.filter(id => !gdIds.includes(id))).size
  }
)

export const getNonAnnotationTermIds = createSelector(
  [getFormValue('ontologyTerms'),
    getGeneralDescriptionOntologyTermsIds],
  (otIds, gdIds) => {
    return List(otIds.union(gdIds)).toArray()
  }
)

const getSelectedTargetGroupsNames = createSelector(
  [getFormValue('targetGroups'), EntitySelectors.translatedItems.getEntities, getContentLanguageCode],
  (ids, translations, languageCode) => List(ids.map(id => translations.getIn([id, 'texts', languageCode])))
)

const getSelectedServiceClassesNames = createSelector(
  [getFormValue('serviceClasses'), EntitySelectors.translatedItems.getEntities, getContentLanguageCode],
  (ids, translations, languageCode) => List(ids.map(id => translations.getIn([id, 'texts', languageCode])))
)

const getSelectedLifeEventsNames = createSelector(
  [getFormValue('lifeEvents'), EntitySelectors.translatedItems.getEntities, getContentLanguageCode],
  (ids, translations, languageCode) => List(ids.map(id => translations.getIn([id, 'texts', languageCode])))
)

const getSelectedIndustrialClassesNames = createSelector(
  [getFormValue('industrialClasses'), EntitySelectors.translatedItems.getEntities, getContentLanguageCode],
  (ids, translations, languageCode) => List(ids.map(id => translations.getIn([id, 'texts', languageCode])))
)

const getTargetGroupSelector = code => createSelector(
  EntitySelectors.targetGroups.getEntities,
  targetGroups => {
    return targetGroups
      .filter(chT => chT.get('code').toLowerCase() === code).first() || Map()
  }
)

const getIdSelector = entitySelector => createSelector(
  entitySelector,
  entity => entity.get('id') || null
)
const getTagetGroupKR2Id = getIdSelector(getTargetGroupSelector('kr2'))
const getTagetGroupKR1Id = getIdSelector(getTargetGroupSelector('kr1'))

const getIsSelectedTargetGroupKR1 = createSelector(
  [getFormValue('targetGroups'), getTagetGroupKR1Id],
  (selectedTargetGroups, id) => selectedTargetGroups.includes(id)
)

const getIsSelectedTargetGroupKR2 = createSelector(
  [getFormValue('targetGroups'), getTagetGroupKR2Id],
  (selectedTargetGroups, id) => selectedTargetGroups.includes(id)
)

const getName = createSelector(
  [getFormValue('name'), getContentLanguageCode],
  (name, lang) => name.get(lang) || ''
)

const getShortDescription = createSelector(
  [getFormValue('shortDescription'), getContentLanguageCode],
  (shortDescription, lang) => shortDescription.get(lang) || ''
)
const getDescription = createSelector(
  [getFormValue('description'), getContentLanguageCode],
  (description, lang) => description.get(lang) || EditorState.createEmpty()
)

export const getIsDescriptionFilled = createSelector(
  getDescription,
  raw => raw.getCurrentContent().hasText() || false
)

export const getIsTargetGroupFilled = createSelector(
  [getSelectedTargetGroupsNames, getGeneralDescriptionTargetGroups, getFormOverrideTargetGroups],
  (selectedTg, gdSelectedTg, overrideTg) => selectedTg.size !== 0 || (gdSelectedTg.size - overrideTg.size) !== 0
)
export const getIsServiceClassFilled = createSelector(
  [getSelectedServiceClassesNames, getGeneralDescriptionServiceClassesIds],
  (selectedSc, gdSelectedSc) => selectedSc.size !== 0 || gdSelectedSc.size !== 0
)

export const getHasAllRequiredAnnotationsFiedsFilled = createSelector(
  [
    getName,
    getShortDescription,
    getIsDescriptionFilled,
    getIsTargetGroupFilled,
    getIsServiceClassFilled
  ],
  (
    name,
    shortDescription,
    isDescriptionFilled,
    isTargetGroupFilled,
    isServiceClassFilled
  ) => {
    const isNameFilled = !!name
    const isShortDescriptionFilled = !!shortDescription
    return (
      isNameFilled &&
      isShortDescriptionFilled &&
      isDescriptionFilled &&
      isTargetGroupFilled &&
      isServiceClassFilled
    )
  }
)

export const getAnnotationData = createSelector(
  [
    getFormValue('id'),
    getName,
    getShortDescription,
    getDescription,
    getSelectedTargetGroupsNames,
    getSelectedServiceClassesNames,
    getSelectedLifeEventsNames,
    getSelectedIndustrialClassesNames,
    getContentLanguageCode,
    getIsSelectedTargetGroupKR1,
    getIsSelectedTargetGroupKR2
  ],
  (
    id,
    name,
    shortDescription,
    description,
    targetGroups,
    serviceClasses,
    lifeEvents,
    industrialClasses,
    languageCode,
    useLifeEvents,
    useIndustrialClasses
  ) => (
    {
      id,
      name: name,
      shortDescription,
      description: description.getCurrentContent().getPlainText(),
      targetGroups,
      serviceClasses,
      lifeEvents: useLifeEvents ? lifeEvents : [],
      industrialClasses: useIndustrialClasses ? industrialClasses : [],
      languageCode
    }
  )
)
