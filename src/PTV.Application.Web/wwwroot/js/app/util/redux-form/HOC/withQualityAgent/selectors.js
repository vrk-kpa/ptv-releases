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
import createCachedSelector from 're-reselect'
import EntitySelectors from 'selectors/entities'
import { getParameterFromProps, getFormValue, getApiCalls, createIsFetchingSelector } from 'selectors/base'
import { Map, List } from 'immutable'
import messages from './messages'

const getRulesErrors = createCachedSelector(
  EntitySelectors.qualityErrors.getEntities,
  getParameterFromProps('fieldId'),
  getFormValue('unificRootId'),
  getFormValue('alternativeId'),
  (entities, fieldId, id, alternativeId) =>
    entities.getIn([id || alternativeId, fieldId, 'rules']) || Map()
)(
  getParameterFromProps('fieldId', '')
)

// const getRulesErrors = createSelector(
//  EntitySelectors.qualityErrors.getEntity,
//  errors => errors.get('rules') || List()
// )

const getMessage = (error, entityPrefix) =>
  messages[`${entityPrefix}rule${error.get('ruleId')}`] ||
  messages[`rule${error.get('ruleId')}`] ||
  error.get('explanation')

export const getQualityErrors = createCachedSelector(
  getRulesErrors,
  getParameterFromProps('entityPrefix'),
  (errors, entityPrefix) =>
    errors
      .filter(x => !x.get('passed'))
      .map(x => Map({
        id: x.get('ruleId'),
        title: getMessage(x, entityPrefix),
        subMessages: (x.getIn(['info', 'matches']) || List())
          .map(sub => Map({
            id: sub.get('start'),
            title: sub.get('explanation')
          }))
      }))
      .toList()
)(
  getParameterFromProps('fieldId', '')
)

const getRulesErrosForHighlighting = createSelector(
  getRulesErrors,
  errors => errors
    .filter(x => !x.get('passed') && (x.get('ruleId') === '9' || x.get('ruleId') === '10'))
)

export const getHighlightErrors = createCachedSelector(
  getRulesErrosForHighlighting,
  errors => errors
    .map(x => x.getIn(['info', 'matches']))
    .toList()
    .flatten(true)
)(
  getParameterFromProps('fieldId', '')
)

export const getProcessedData = createCachedSelector(
  getRulesErrosForHighlighting,
  errors => errors
    .map(x => x.getIn(['processed', 'data']))
    .first()
)(
  getParameterFromProps('fieldId', '')
)

const getQualityAgentApiCalls = createSelector(
  [getApiCalls],
  (api) => api.getIn(['services', 'quality']) || Map()
)

export const getQualityAgentIsFetching = createIsFetchingSelector(getQualityAgentApiCalls)

export const getLanguageHasQualityAgentError = createSelector(
  EntitySelectors.qualityErrors.getEntity,
  getParameterFromProps('languageCode'),
  (entity, langCode) => {
    return entity &&
      langCode &&
      entity.some(field => field.get('fieldId').includes('.' + langCode) &&
        field.get('rules').some(rule => !rule.get('passed'))) || false
  }
)