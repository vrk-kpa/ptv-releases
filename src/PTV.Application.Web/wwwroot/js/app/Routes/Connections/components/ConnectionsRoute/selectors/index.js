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
import { getUserOrganization } from 'selectors/userInfo'
import { getSelectedLanguage } from 'Intl/Selectors'
import { getUiSortingData, getParameterFromProps } from 'selectors/base'

const getSortingData = createSelector(
  getUiSortingData,
  (sortingData) => sortingData.get('column') === 'serviceType' ? sortingData.set('column', 'subEntityType') : sortingData
)

export const getSearchOrganizationValues = createSelector(
  getUserOrganization,
  getSelectedLanguage,
  getParameterFromProps('pageNumber'),
  getSortingData,
  (organizationId, language, pageNumber, sortingData) => ({
    organizationId,
    language,
    pageNumber: pageNumber || 0,
    sortData: sortingData.size > 0 ? [sortingData] : []
  })
)

export const getAllSearchOrganizationValues = createSelector(
  [getUserOrganization, getParameterFromProps('count')],
  (organizationId, count) => ({
    organizationId,
    maxPageCount: count
  })
)
