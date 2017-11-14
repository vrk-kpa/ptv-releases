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
import { Map, List } from 'immutable'
import { EditorState } from 'draft-js'
import {
  createIsFetchingSelector,
  getApiCalls,
  getEntitiesForIds,
  getIdFromProps,
  getFormValue,
  getParameterFromProps
} from 'selectors/base'
import {
    getContentLanguageCode
} from 'selectors/selections'

const getIsIdDefined = createSelector(
  getIdFromProps,
  id => id != null
)

export const getOntologyTerm = createSelector(
    [EntitySelectors.ontologyTerms.getEntities, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
)

export const getOntologyTermChildren = createSelector(
    getOntologyTerm,
    entity => entity.get('children') || List()
)

const getSearch = createSelector(
  [getApiCalls, getParameterFromProps('searchValue')],
  (api, searchValue) => api.getIn(['OntologyTerm', searchValue && searchValue.value, 'search']) || Map()
)

const getAnnotation = createSelector(
    getApiCalls,
    api => api.getIn(['OntologyTerm', 'searchAnnotations']) || Map()
  )

export const getIsFetching = createIsFetchingSelector(getSearch)
export const annotationIsFetching = createIsFetchingSelector(getAnnotation)

const getFilteredIds = createSelector(
  [getSearch, getParameterFromProps('searchValue')],
  (search, searchValue) => searchValue && search.get('result') || List()
)

export const getOntologyTermIds = createSelector(
    [getFilteredIds, getOntologyTermChildren, getIsIdDefined],
    (top, children, returnChildren) => returnChildren ? children : top
)

export const getFormGeneralDescriptionId = createSelector(
    getFormValue('generalDescriptionId'),
    values => values || ''
)

export const getIsGeneralDescriptionAttached = createSelector(
    getFormGeneralDescriptionId,
    generalDescriptionId => !!generalDescriptionId || false
)

export const getGeneralDescription = createSelector(
    [EntitySelectors.generalDescriptions.getEntities, getFormGeneralDescriptionId],
    (generalDescriptions, generalDescriptionId) => generalDescriptions.get(generalDescriptionId) || Map()
)

export const getGeneralDescriptionOntologyTermsIds = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('ontologyTerms') || List()
)

export const getGeneralDescriptionOntologyTerms = createSelector(
  [EntitySelectors.ontologyTerms.getEntities,
    getGeneralDescriptionOntologyTermsIds],
    (ontologyTerms, ids) => getEntitiesForIds(ontologyTerms, ids, List())
)

export const getOntologyTermsForIds = createSelector(
  [EntitySelectors.ontologyTerms.getEntities,
    getParameterFromProps('ids'),
    getGeneralDescriptionOntologyTermsIds],
    (ontologyTerms, ids, idsFromGD) =>
      getEntitiesForIds(ontologyTerms, List(ids.filter(id => !idsFromGD.includes(id))), List())
)

export const getOntologyTermsForIdsJs = createSelector(
  [getOntologyTermsForIds, getGeneralDescriptionOntologyTerms, getParameterFromProps('disabledAll')],
  (list, fromGD, disabledAll) => fromGD.map(x => ({ value: x.get('id'), label: x.get('name'), clearableValue:false })).concat(
        list.map(x => ({ value: x.get('id'), label: x.get('name'), clearableValue: !disabledAll }))
      ).toArray()
)

export const getAnnotationTermsForIds = createSelector(
  [EntitySelectors.ontologyTerms.getEntities,
    getParameterFromProps('ids'),
    getGeneralDescriptionOntologyTermsIds,
    getFormValue('ontologyTerms')],
      (ontologyTerms, ids, idsFromGD, idsFromOT) =>
       getEntitiesForIds(ontologyTerms, List(ids.filter(id => !idsFromGD.includes(id) && !idsFromOT.includes(id))), List())
  )
export const getAnnotationTermsForIdsJs = createSelector(
    [getAnnotationTermsForIds, getParameterFromProps('disabledAll')],
    (list, disabledAll) =>
       list.map(x => ({ value: x.get('id'), label: x.get('name'), clearableValue: !disabledAll })).toArray()
  )

export const isAnyAnnotationTerm = createSelector(
  [getFormValue('annotationTerms'),
    getFormValue('ontologyTerms'),
    getGeneralDescriptionOntologyTermsIds],
    (atIds, otIds, gdIds) => {
      return List(atIds.filter(id => !gdIds.includes(id) && !otIds.includes(id))).size > 0
    }
)

export const getSelectedTargetGroupsNames = createSelector(
    [getFormValue('targetGroups'), EntitySelectors.translatedItems.getEntities, getContentLanguageCode],
    (ids, translations, languageCode) => List(ids.map(id => translations.getIn([id, 'texts', languageCode])))
)

export const getSelectedServiceClassesNames = createSelector(
    [getFormValue('serviceClasses'), EntitySelectors.translatedItems.getEntities, getContentLanguageCode],
    (ids, translations, languageCode) => List(ids.map(id => translations.getIn([id, 'texts', languageCode])))
)

export const getSelectedLifeEventsNames = createSelector(
    [getFormValue('lifeEvents'), EntitySelectors.translatedItems.getEntities, getContentLanguageCode],
    (ids, translations, languageCode) => List(ids.map(id => translations.getIn([id, 'texts', languageCode])))
)

export const getSelectedIndustrialClassesNames = createSelector(
    [getFormValue('industrialClasses'), EntitySelectors.translatedItems.getEntities, getContentLanguageCode],
    (ids, translations, languageCode) => List(ids.map(id => translations.getIn([id, 'texts', languageCode])))
)

export const getTargetGroupSelector = code => createSelector(
    EntitySelectors.targetGroups.getEntities,
    targetGroups => {
      return targetGroups
        .filter(chT => chT.get('code').toLowerCase() === code).first() || Map()
    }
)

export const getIdSelector = entitySelector => createSelector(
    entitySelector,
    entity => entity.get('id') || null
)
const getTagetGroupKR2Id = getIdSelector(getTargetGroupSelector('kr2'))
const getTagetGroupKR1Id = getIdSelector(getTargetGroupSelector('kr1'))

export const getIsSelectedTargetGroupKR1 = createSelector(
    [getFormValue('targetGroups'), getTagetGroupKR1Id],
    (selectedTargetGroups, id) => selectedTargetGroups.includes(id)
)

export const getIsSelectedTargetGroupKR2 = createSelector(
    [getFormValue('targetGroups'), getTagetGroupKR2Id],
    (selectedTargetGroups, id) => selectedTargetGroups.includes(id)
)

export const getName = createSelector(
    [getFormValue('name'), getContentLanguageCode],
    (name, lang) => name.get(lang) || ''
)

export const getShortDescription = createSelector(
    [getFormValue('shortDescription'), getContentLanguageCode],
    (shortDescription, lang) => shortDescription.get(lang) || ''
)
export const getDescription = createSelector(
    [getFormValue('description'), getContentLanguageCode],
    (description, lang) => description.get(lang) || EditorState.createEmpty()
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
          shortDescriptions,
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
          name : name,
          shortDescriptions,
          description: description.getCurrentContent().getPlainText(),
          targetGroups,
          serviceClasses,
          lifeEvents: useLifeEvents ? lifeEvents : [],
          industrialClasses: useIndustrialClasses ? industrialClasses : [],
          languageCode
        }
      )
  )
