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
import { Map, List } from 'immutable'
import { EntitySelectors, EnumsSelectors } from 'selectors'
import {
  getEntitiesForIds,
  getParameterFromProps
} from 'selectors/base'
import { getSelectedEntityId } from 'selectors/entities/entities'
import { getContentLanguageId } from 'selectors/selections'

export const getTranslationEntity = createSelector(
  [EntitySelectors.translations.getEntities, getSelectedEntityId],
  (entities, id) => entities.get(id) || Map()
)

export const getTranslationOrderStatesIds = createSelector(
  getTranslationEntity,
  entity => entity.get('translationOrderStates') || List()
)

export const getTranslationOrderStateEntities = createSelector(
  [getTranslationOrderStatesIds, EntitySelectors.translationOrderStates.getEntities],
  (ids, entities) => ids.size > 0 &&
    ids.map(v => getEntitiesForIds(entities, v, List())) || Map()
)

export const getTranslationStatesWithOrderEntities = createSelector(
  [getTranslationOrderStatesIds,
    EntitySelectors.translationOrderStates.getEntities,
    EntitySelectors.translationOrders.getEntities],
  (orderStateIds, orderStateEntities, orderEntities) => {
    const result = getEntitiesForIds(orderStateEntities, orderStateIds, List())
      .map(os => os.set('translationOrder', orderEntities && orderEntities.get(os.get('translationOrder') || Map())))
    const lastIds = result.groupBy(order => order.getIn(['translationOrder', 'targetLanguage'])).map(group => group.first().get('id'))
    return result.map(order => order.set('lastOrder', lastIds.contains(order.get('id'))))
  }
)

export const isTranslationReceived = createSelector(
  [EntitySelectors.translationStateTypes.getEntities,
    getParameterFromProps('stateTypeId')],
  (orderStates, id) => {
    const state = orderStates.get(id)
    return state && (state.get('code').toLowerCase() === 'arrived') || false
  }
)

export const isTranslationFailForInvestigationState = createSelector(
  [EntitySelectors.translationStateTypes.getEntities,
    getParameterFromProps('stateTypeId')],
  (orderStates, id) => {
    const state = orderStates.get(id)
    return state && (state.get('code').toLowerCase() === 'failforinvestigation') || false
  }
)

// in orderForm
export const getTargetLanguagesInUse = createSelector(
  getTranslationEntity,
  entity => {
    return entity.get('targetLanguagesInUse') || List()
  }
)

// TranslationOrderForm
export const getRequiredLanguages = createSelector(
  [EnumsSelectors.translationOrderLanguages.getEntities, getTargetLanguagesInUse, getContentLanguageId],
  (languagesEntities, languagesInUseIds, contentLanguageId) => {
    return languagesEntities.map(language => ({
      label: language.get('code'),
      code: language.get('code'),
      id: language.get('id'),
      disabledItem: languagesInUseIds.includes(language.get('id')) || contentLanguageId === language.get('id'),
      isSelected: languagesInUseIds.size !== 3 && languagesInUseIds.includes(language.get('id')) && contentLanguageId !== language.get('id')
    }))
  }
)

// TranslationOrderForm
export const getRequiredLanguage = createSelector(
  [EnumsSelectors.translationOrderLanguages.getEntities, getParameterFromProps('selectedLang')],
  (languagesEntities, languageId) => {
    return languagesEntities.map(language => ({
      label: language.get('code'),
      code: language.get('code'),
      id: language.get('id'),
      disabledItem: true,
      isSelected: language.get('id') === languageId
    }))
  }
)

// TranslationOrderForm
export const getSourceLanguages = createSelector(
  [EnumsSelectors.translationOrderLanguages.getEntities, getContentLanguageId],
  (languagesEntities, contentLanguageId) => {
    return languagesEntities.map(language => ({
      label: language.get('code'),
      code: language.get('code'),
      value: language.get('id'),
      checked: language.get('id') === contentLanguageId,
      disabledItem: language.get('id') !== contentLanguageId
    }))
  }
)
