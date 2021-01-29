/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import {
  getApiCalls
} from 'selectors/base'
import { formTypesEnum } from 'enums'
// import { EntitySelectors } from 'selectors'
import { Map, List } from 'immutable'
import Entities from 'selectors/entities/entities'
import { getIdFromProps, getUiByKey } from 'selectors/base'

const getMappingSearch = createSelector(
  getApiCalls,
  apiCalls => apiCalls.get(formTypesEnum.ADMINMAPPINGFORM) || Map()
)

export const getMappingIsFound = createSelector(
  getMappingSearch,
  apiCallMapping => apiCallMapping.size > 0
)

export const getMappingIsFetching = createSelector(
  getMappingSearch,
  apiCallMapping => apiCallMapping.getIn(['search', 'isFetching']) || false
)

const getMappingResult = createSelector(
  getMappingSearch,
  apiCallMapping => apiCallMapping.getIn(['search', 'result']) || Map()
)

const getMappingResultIds = createSelector(
  getMappingResult,
  mappingResult => mappingResult.get('data') || List()
)

const getMappingResults = createSelector(
  [getMappingResultIds, Entities.sahaMappings.getEntities],
  (mappingResultIds, mappings) =>
    mappingResultIds.map(id => mappings.get(id)) || List()
)

export const getResultsJS = createSelector(getMappingResults, results => results.toJS())

export const getPtvId = createSelector(
  getUiByKey,
  component => component.get('ptvId') || null
)

export const getAvailableLanguages = createSelector(
  [getPtvId, Entities.sahaMappings.getEntities],
  (id, organizations) => {
    const org = organizations.filter(o => o.get('ptvOrganizationId') === id).first() || Map()
    const langs = org.get('languagesAvailabilities')
    return langs && langs.toJS() || null
  }
)
