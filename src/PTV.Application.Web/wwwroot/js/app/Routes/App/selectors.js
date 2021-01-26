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
  getUserName,
  getUserSurname,
  getUserEmail,
  getUserOrganization,
  getUserInfoIsFetching
} from 'selectors/userInfo'
import EntitySelectors from 'selectors/entities'
import { getFormValues } from 'redux-form/immutable'
import { formTypesEnum } from 'enums'
import { List } from 'immutable'
import { getEnumTypesIsFetching } from 'selectors/base'
import { createTranslatedIdSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'

export const getFooterUserName = createSelector([
  getUserName,
  getUserSurname,
  getUserEmail
], (
  name,
  surname,
  email
) => (name || surname)
  ? (name + ' ' + surname).trim()
  : email
)

const getTranslatedOrganization = createTranslatedIdSelector(
  getUserOrganization, {
    languageTranslationType: languageTranslationTypes.locale
  }
)

export const getHeaderOrganization = createSelector(
  getTranslatedOrganization,
  organization => ({ organizationName: organization })
)

export const getHeaderUser = createSelector(
  [
    getUserName,
    getUserSurname
  ], (name, surname) => (
    {
      firstName: name,
      lastName: surname
    }
  )
)

export const getSelectedEntitiesForMassTool = createSelector(
  getFormValues(formTypesEnum.MASSTOOLSELECTIONFORM),
  formValues => formValues && formValues.get('selected') || List()
)

export const getIsMassSelectionInProgress = createSelector(
  getSelectedEntitiesForMassTool,
  entities => entities.size > 0
)

export const getIsPreloaderVisible = createSelector(
  [getEnumTypesIsFetching, getUserInfoIsFetching],
  (enumTypesIsFetching, userInfoIsFetching) => userInfoIsFetching || enumTypesIsFetching
)
