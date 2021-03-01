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
import { getApiCalls, getEntitiesForIds, getParameterFromProps, getFormValue } from 'selectors/base'
import { Map, List } from 'immutable'
import { EntitySelectors } from 'selectors'
import { EditorState, convertFromRaw } from 'draft-js'

export const getApiCallForInstructions = createSelector(
  getApiCalls,
  apiCalls => apiCalls.get('currentIssuesPage') || Map()
)

export const getApiCallForInstructionsForm = createSelector(
  getApiCallForInstructions,
  currentIssues => currentIssues.get('form') || Map()
)

export const getInstructionsIsFetching = createSelector(
  getApiCallForInstructionsForm,
  form => form.get('isFetching') || false
)

export const getInstructionsData = createSelector(
  [getApiCallForInstructionsForm, getParameterFromProps('result')],
  (form, result) => result || form.get('result') || List()
)

export const getInstructions = createSelector(
  [EntitySelectors.instructions.getEntities, getInstructionsData],
  (entitiesInstructions, instructionsIds) => getEntitiesForIds(entitiesInstructions, instructionsIds) || List()
)

export const getLastInstruction = createSelector(
  getInstructions,
  instructions => instructions.first() || Map()
)

export const getPreviousInstruction = createSelector(
  getInstructions,
  instructions => instructions.last() || Map()
)

export const getLastFormInstruction = createSelector(
  getFormValue('lastInfo'),
  instructions => instructions || Map()
)
export const getPreviousFormInstruction = createSelector(
  getFormValue('previousInfo'),
  instructions => instructions || Map()
)

export const getLastData = createSelector(
  getLastInstruction,
  entity => {
    try {
      return entity.get('environmentInstructions') &&
        EditorState.createWithContent(convertFromRaw(JSON.parse(entity.get('environmentInstructions'))))
    } catch (e) {
      return Map()
    }
  }
)

export const getPreviousData = createSelector(
  getPreviousInstruction,
  entity => {
    try {
      return entity.get('environmentInstructions') &&
        EditorState.createWithContent(convertFromRaw(JSON.parse(entity.get('environmentInstructions'))))
    } catch (e) {
      return Map()
    }
  }
)

export const getCurrentIssue = createSelector(
  [ getLastData,
    getPreviousData,
    getLastInstruction,
    getPreviousInstruction
  ],
  (lastData,
    previousData,
    lastInfo,
    previousInfo
  ) => ({
    environmentInstructions : lastData,
    previousData,
    lastInfo,
    previousInfo
  })
)
