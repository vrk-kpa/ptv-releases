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
import { getEntity } from 'selectors/entities/entities'
import { List, Map } from 'immutable'
import { getFormValue, getParameterFromProps } from 'selectors/base'
import { getFormValues } from 'redux-form/immutable'
import { formTypesEnum } from 'enums'
import { EntitySelectors } from 'selectors'

export const getLanguageAvailabilities = createSelector(
  getFormValue('languagesAvailabilities'),
  languages => languages || List()
)

const getReviewedRecords = createSelector(
  getFormValues(formTypesEnum.MASSTOOLFORM),
  formValues => {
    return formValues && formValues.get('review') || List()
  }
)

const getReviewedEntitiesApprovedLanguages = createSelector(
  getReviewedRecords,
  records => {
    const approvedRecords = records.filter(record => record.get('approved'))
    return approvedRecords.reduce((acc, curr) => {
      const key = curr.get('id')
      const value = curr.get('language')
      return acc.update(key, (x = List()) => x.push(value))
    }, Map())
  }
)

export const getCurrentEntityApprovedLanguages = createSelector(
  getReviewedEntitiesApprovedLanguages,
  getEntity,
  (all, selected) => all.get(selected.get('id')) || List()
)

export const getLanguageEntity = createSelector(
  EntitySelectors.languages.getEntities,
  getParameterFromProps('languageId'),
  (languages, id) => languages.get(id) || Map()
)

// export const getCompactOptions = createSelector(
//   getLanguageAvailabilities,
//   languages => {

//   }
// )
