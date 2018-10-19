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
import entitySelector, { getEntityAvailableLanguages, getSelectedEntityType } from 'selectors/entities/entities'
import { getForm } from 'selectors/base'
import { getContentLanguageCode } from 'selectors/selections'
import { formTypesEnum } from 'enums'
import { EntitySelectors } from 'selectors'
import { List } from 'immutable'

export const getLanguages = createSelector(
  [getEntityAvailableLanguages, entitySelector.languages.getEntities],
  (languagesAvailability, languages) => {
    const result = languagesAvailability.map(language => {
      const languageId = language.get('languageId')
      return languages.get(languageId)
    })
    return result
  }
)

export const getFormNameFieldValue = formName => createSelector(
  [getForm, getContentLanguageCode],
  (formStates, contentLanguageCode) => {
    // TBD -> unite name and path for Name field
    if (formName === formTypesEnum.ELECTRONICCHANNELFORM) {
      return formStates.getIn([formName, 'values', 'basicInfo', 'name', contentLanguageCode]) || ''
    } else {
      return formStates.getIn([formName, 'values', 'name', contentLanguageCode]) || ''
    }
  }
)

export const getArchivedStatusesId = createSelector(
  EntitySelectors.publishingStatuses.getEntities,
  (publishingStatuses) => (publishingStatuses.filter(ps => {
    const status = ps.get('code').toLowerCase()
    return status === 'deleted' || status === 'oldpublished'
  }) ||
    List()).map(x => x.get('id'))
)

export const getEntityPublishingStatus = createSelector(
  EntitySelectors.getEntity,
  (entity) => entity.get('publishingStatus')
)

export const getIsEntityArchived = createSelector(
  [getEntityPublishingStatus, getArchivedStatusesId],
  (entityPs, archivedPS) => archivedPS.includes(entityPs)
)

export const getIsEntityWithConnections = createSelector(
  getSelectedEntityType,
  entityType => (
    entityType !== 'organizations' &&
    entityType !== 'serviceLocations'
  )
)
