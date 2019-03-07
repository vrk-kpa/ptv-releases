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
import {
  getUserName,
  getUserSurname,
  getUserEmail,
  getUserOrganization
} from 'selectors/userInfo'
import EntitySelectors from 'selectors/entities'
import { getFormValues } from 'redux-form/immutable'
import { formTypesEnum } from 'enums'
import { List } from 'immutable'

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

export const getHeaderOrganization = createSelector(
  [
    getUserOrganization,
    EntitySelectors.organizations.getEntities
  ], (organizationId, organizations) => (
    {
      organizationName: organizations.getIn([organizationId, 'displayName']) || ''
    }
  )
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
