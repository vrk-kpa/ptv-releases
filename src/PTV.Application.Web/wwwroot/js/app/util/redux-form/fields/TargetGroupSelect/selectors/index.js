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
import { EnumsSelectors, EntitySelectors } from 'selectors'
import { getArray } from 'selectors/base'
import { List } from 'immutable'

export const getTopTargetGroupsJS = createSelector(
  EnumsSelectors.targetGroups.getEntities,
  targetGroups =>
    getArray(targetGroups
      .map(targetGroup => {
        return {
          value: targetGroup.get('id'),
          label: targetGroup.get('name')
        }
      })
    )
)

export const getTargetGroupsJS = createSelector(
  getTopTargetGroupsJS, EntitySelectors.targetGroups.getEntities,
  (topTG, allTG) => {
    let listTG = List()
    topTG.map(top => {
      listTG = listTG.push(top)
      const sub = allTG.getIn([top.value, 'children'])
      listTG = listTG.concat(sub.map(s => {
        return {
          value: allTG.get(s).get('id'),
          label: allTG.get(s).get('name')
        }
      }))
    })
    return listTG.toArray()
  }
)
