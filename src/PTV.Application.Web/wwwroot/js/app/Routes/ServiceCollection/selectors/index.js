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
import { Map, OrderedMap } from 'immutable'
import { getApiCalls,
  getParameterFromProps
} from 'selectors/base'
import { EntitySelectors } from 'selectors'
import { EditorState, convertFromRaw } from 'draft-js'
import {
  getOrganization,
  createEntityPropertySelector
} from 'selectors/common'
import {
  createFilteredField,
  getSelectedOrCopyEntity,
  getLanguageAvailabilities
} from 'selectors/copyEntity'
import { getUserOrganization } from 'selectors/userInfo'

export const getServiceCollectionDescription = createSelector(
  createFilteredField(createEntityPropertySelector(getSelectedOrCopyEntity, 'description', Map())),
  field => {
    try {
      return field.map(l =>
        l && EditorState.createWithContent(convertFromRaw(JSON.parse(l)))
      ) || Map()
    } catch (e) {
      return Map()
    }
  }
)

const getTemplateId = createSelector(
  getParameterFromProps('templateId'),
  templateId => templateId
)

const getTemplateOrganizationId = createSelector(
  getParameterFromProps('templateId'),
  getSelectedOrCopyEntity,
  (templateId, entity) => templateId && entity.get('organizationId') || null
)

const getServiceCollectionOrganization = createSelector(
  [getOrganization, getUserOrganization, getTemplateId],
  (organization, userOrganization, templateId) => templateId ? userOrganization : organization
)

export const getServiceCollection = createSelector(
  [EntitySelectors.getEntity,
    getServiceCollectionDescription,
    getLanguageAvailabilities,
    getServiceCollectionOrganization,
    getTemplateId,
    getTemplateOrganizationId
  ],
  (entity,
    description,
    languagesAvailabilities,
    organization,
    templateId,
    templateOrganizationId
  ) => OrderedMap().mergeDeep({
    id: entity.get('id') || null,
    languagesAvailabilities,
    templateId,
    templateOrganizationId,
    name: entity.get('name') || Map(),
    description,
    organization
  })
)
