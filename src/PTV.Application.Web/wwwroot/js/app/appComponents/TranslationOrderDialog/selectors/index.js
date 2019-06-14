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
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { createSelector } from 'reselect'
import { getContentLanguageId } from 'selectors/selections'
import { getUserName, getUserEmail } from 'selectors/userInfo'
import { getSelectedEntityId } from 'selectors/entities/entities'
import { fromJS, Set } from 'immutable'
import { getFormValues } from 'redux-form/immutable'
import { formTypesEnum } from 'enums'

export const isRequiredLanguageSelected = createSelector(
  getFormValues(formTypesEnum.TRANSLATIONORDERFORM),
  formValues => formValues && formValues.get('requiredLanguages').size > 0 || false
)

export const getTranslationCompanyId = createSelector(
  EnumsSelectors.translationCompanies.getEnums,
  companies => companies.first() || null
)

export const getTranslationCompany = createSelector(
  EntitySelectors.translationCompanies.getEntities,
  companies => companies.first() || null
)

export const getTranslationOrder = createSelector(
  [
    getSelectedEntityId,
    getUserName,
    getUserEmail,
    getContentLanguageId
  ], (
    entityId,
    senderName,
    senderEmail,
    sourceLanguage
  ) => fromJS({
    entityId,
    senderName,
    senderEmail,
    sourceLanguage,
    requiredLanguages: Set()
  })
)
