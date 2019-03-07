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
import { EnumsSelectors, EntitySelectors } from 'selectors'
import { createSelector } from 'reselect'
import { Map, List, OrderedSet } from 'immutable'
import { getIdFromProps, getFormValue } from 'selectors/base'

const getIsIdDefined = createSelector(
  getIdFromProps,
  id => {
    // console.log(id)
    return id != null
  }
)

export const getTopTargetGroups = createSelector(
  EnumsSelectors.topTargetGroups.getEnums,
  topTargetGroups => topTargetGroups || List()
)

export const getTopTargetGroupsEntities = createSelector(
  EnumsSelectors.topTargetGroups.getEntitiesMap,
  topTargetGroups => topTargetGroups
)

export const getTargetGroup = createSelector(
  [EntitySelectors.targetGroups.getEntities, getIdFromProps],
  (entities, id) => entities.get(id) || Map()
)

export const getTargetGroupsChildren = createSelector(
  getTargetGroup,
  entity => entity.get('children') || Map()
)

export const getTargetGroupsIds = createSelector(
  [getTopTargetGroups, getTargetGroupsChildren, getIsIdDefined],
  (top, children, returnChildren) => {
    // console.log(returnChildren, children, top)
    return returnChildren ? children : top
  }
)

export const getFormGeneralDescriptionId = createSelector(
  getFormValue('generalDescriptionId'),
  values => values || ''
)

export const getFormTargetGroups = createSelector(
  getFormValue('targetGroups'),
  values => values || List()
)

export const getFormTargetGroupsOrderedSet = createSelector(
  getFormTargetGroups,
  targetGroups => targetGroups.toOrderedSet()
)

export const getFormOverrideTargetGroups = createSelector(
  getFormValue('overrideTargetGroups'),
  values => values || List()
)

export const getFormOverrideTargetGroupsOS = createSelector(
  getFormOverrideTargetGroups,
  overrideTargetGroups => overrideTargetGroups.toOrderedSet()
)

export const getIsGeneralDescriptionAttached = createSelector(
  getFormGeneralDescriptionId,
  generalDescriptionId => !!generalDescriptionId || false
)

export const getGeneralDescription = createSelector(
  [EntitySelectors.generalDescriptions.getEntities, getFormGeneralDescriptionId],
  (generalDescriptions, generalDescriptionId) => generalDescriptions.get(generalDescriptionId) || Map()
)

export const getGeneralDescriptionTargetGroups = createSelector(
  getGeneralDescription,
  generalDescription => generalDescription.get('targetGroups') || List()
)

export const getGeneralDescriptionTargetGroupsIds = createSelector(
  getGeneralDescriptionTargetGroups,
  targetGroups => targetGroups.toOrderedSet()
)

export const anyChildrenChecked = createSelector(
  [getTargetGroupsChildren, getGeneralDescriptionTargetGroupsIds, getFormTargetGroups, getFormOverrideTargetGroups],
  (childrenIds, gdTgIds, formTgIds, formOverIds) => {
    const userChecked = !!childrenIds.filter(x => formTgIds.includes(x)).size
    const genChecked = !!childrenIds.filter(x => gdTgIds.includes(x) && !formOverIds.includes(x)).size
    return userChecked || genChecked
  }
)
