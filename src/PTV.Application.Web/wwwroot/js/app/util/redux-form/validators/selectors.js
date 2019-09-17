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
import { EntitySelectors } from 'selectors'
import { getParameterFromProps, getEntitiesForIds } from 'selectors/base'
import { getEntity } from 'selectors/entities/entities'
import { Map, OrderedSet } from 'immutable'

const getGDValue = createSelector(
  getEntity,
  getParameterFromProps('field'),
  getParameterFromProps('defaultValue'),
  getParameterFromProps('id'),
  (generalDescription, field, defaultValue, gdID) => gdID && generalDescription.get(field) || defaultValue || Map()
)

export const getGDLocalizedValue = createSelector(
  getGDValue,
  getParameterFromProps('language'),
  (value, language) => value.get(language) || null
)

export const getServiceClassesWithGD = createSelector(
  getGDValue,
  getParameterFromProps('serviceClassesServiceIds'),
  EntitySelectors.serviceClasses.getEntities,
  (gdSCIds, serviceSCIds, serviceClasses) => {
    const ids = (gdSCIds.toOrderedSet()).union(serviceSCIds)
    return getEntitiesForIds(serviceClasses, ids, OrderedSet())
  }
)

export const getTotalCount = createSelector(
  getGDValue,
  getParameterFromProps('items'),
  (gdValue, value) => {
    const ids = (gdValue.toOrderedSet()).union(value)
    return ids.size
  }
)
