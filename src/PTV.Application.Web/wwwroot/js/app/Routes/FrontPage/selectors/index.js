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
import { getApiCalls } from 'selectors/base'
import { permisionTypes } from 'enums'
import { getIsAccessible } from 'appComponents/Security/selectors'
import { getTasksSummaryCount } from 'Routes/Tasks/selectors'
import { getNotificationsCount } from 'Routes/Tasks/selectors/notifications'
import { Map } from 'immutable'

export const getFrontPageSearch = createSelector(
  getApiCalls,
  apiCalls => apiCalls.get('frontPageSearch') || Map()
)

export const getFrontPageSearchForm = createSelector(
  getFrontPageSearch,
  frontPageSearch => frontPageSearch.get('form') || Map()
)

export const getFrontPageSearchFormResult = createSelector(
  getFrontPageSearchForm,
  frontPageSearch => frontPageSearch.get('result') || Map()
)

export const getAreFrontPageDataLoaded = createSelector(
  [getFrontPageSearchForm, getFrontPageSearchFormResult],
  (frontPage, frontPageResults) => frontPage.get('areDataValid') && frontPageResults.size > 0
)

export const getIsNavigationItemVisible = (state, { domain }) => {
  return !domain ? true : getIsAccessible(state, {
    domain,
    permisionType: permisionTypes.read,
    formName: 'unknown'
  })
}

const getIsAdminTasksNavigationItemVisible = state => {
  return getIsAccessible(state, {
    domain: 'adminSection',
    permisionType: permisionTypes.read,
    formName: 'unknown'
  })
}

export const getVisibleNavigationItemCount = createSelector(
  getIsAdminTasksNavigationItemVisible,
  adminTasks => {
    const navigationItemsVisibility = [
      true,
      true,
      true,
      adminTasks
    ]
    return navigationItemsVisibility.filter(item => item).length
  }
)

export const getTasksSectionTotalCount = createSelector(
  getTasksSummaryCount,
  getNotificationsCount,
  (tasksCount, notificationsCount) => tasksCount + notificationsCount
)
