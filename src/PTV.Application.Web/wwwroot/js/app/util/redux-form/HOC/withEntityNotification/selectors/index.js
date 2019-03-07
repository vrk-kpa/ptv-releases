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
import { EntitySelectors } from 'selectors'
import { List } from 'immutable'
import { getEntitiesForIds } from 'selectors/base'

export const getIsOrganizationLanguageWarningVisible = createSelector(
  EntitySelectors.getEntity,
  entity =>
    (entity.get('missingLanguages') && entity.get('missingLanguages').size > 0) || false
)

const getMissingLanguagesIds = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('missingLanguages') || List()
)

export const getMissingLanguages = createSelector(
  getMissingLanguagesIds, EntitySelectors.languages.getEntities,
  (missingLanguagesIds, languagesEntities) => getEntitiesForIds(languagesEntities, missingLanguagesIds)
)

export const getIsRemoveConnectionWarningVisible = createSelector(
  EntitySelectors.getEntity,
  entity =>
    (entity.get('disconnectedConnections') && entity.get('disconnectedConnections').size > 0) || false
)

export const getWarningChannels = createSelector(
  EntitySelectors.getEntity,
  entity =>
    entity.get('disconnectedConnections') || List()
)

export const getNotificationIds = createSelector(
  getWarningChannels,
  channels =>
    channels.map((channel) => channel.get('notificationId')).toJS()
)

export const getIsExpireWarningVisible = createSelector(
  EntitySelectors.getEntity,
  entity =>
    entity.get('isWarningVisible') || false
)

const getEntityLanguagesAvailabilities = createSelector(
  EntitySelectors.getEntity,
  entity => entity.get('languagesAvailabilities') || List()
)

export const getIsTimedPublishInfoVisible = createSelector(
  getEntityLanguagesAvailabilities,
  languagesAvailabilities => languagesAvailabilities.some(la => la.get('validFrom'))
)

export const getPublishOn = createSelector(
  getEntityLanguagesAvailabilities,
  languagesAvailabilities => {
    const sorted = languagesAvailabilities
      .map(la => la.get('validFrom'))
      .filter(x => x)
      .sort() || List()
    return sorted.get(0)
  }
)
