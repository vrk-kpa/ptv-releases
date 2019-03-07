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
// import { createSelector } from 'reselect'
import createCachedSelector from 're-reselect'
import EntitySelectors from 'selectors/entities'
import { getParameterFromProps, getFormValue } from 'selectors/base'
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
  (errors, entityPrefix) => errors
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
