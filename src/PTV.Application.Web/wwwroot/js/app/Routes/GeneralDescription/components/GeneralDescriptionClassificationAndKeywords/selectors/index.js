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
import { Map, List, OrderedSet } from 'immutable'
import { getFormValue } from 'selectors/base'
import { EntitySelectors, EnumsSelectors } from 'selectors'
// form selectors

export const getTargetGrougKR2 = createSelector(
  EnumsSelectors.targetGroups.getEntities,
  targetGroups => targetGroups.find(tg => tg.get('code') === 'KR2') || Map()
)

export const getTargetGrougKR2Id = createSelector(
  getTargetGrougKR2,
  targetGroup => targetGroup.get('id') || ''
)

export const getTargetGrougKR1 = createSelector(
  EnumsSelectors.targetGroups.getEntities,
  targetGroups => targetGroups.find(tg => tg.get('code') === 'KR1') || Map()
)

export const getTargetGrougKR1Id = createSelector(
  getTargetGrougKR1,
  targetGroup => targetGroup.get('id') || ''
)

export const getFormTargetGroups = createSelector(
    getFormValue('targetGroups'),
    values => values || Map()
)

export const getFormGeneralDescriptionId = createSelector(
    getFormValue('generalDescriptionId'),
    values => values || ''
)

export const getFormOverrideTargetGroups = createSelector(
    getFormValue('overrideTargetGroups'),
    values => values || OrderedSet()
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

export const getIsKR1Selected = createSelector(
  [getFormTargetGroups,
    getTargetGrougKR1Id,
    getIsGeneralDescriptionAttached,
    getGeneralDescriptionTargetGroups,
    getFormOverrideTargetGroups],
    (targetGroups, kr1Id, isGD, gdTG, oTG) =>
       oTG.get(kr1Id) ? false : isGD && gdTG.includes(kr1Id) || targetGroups.includes(kr1Id) || false
)

export const getIsKR2Selected = createSelector(
  [getFormTargetGroups,
    getTargetGrougKR2Id,
    getIsGeneralDescriptionAttached,
    getGeneralDescriptionTargetGroups,
    getFormOverrideTargetGroups],
    (targetGroups, kr2Id, isGD, gdTG, oTG) =>
      oTG.get(kr2Id) ? false : isGD && gdTG.includes(kr2Id) || targetGroups.includes(kr2Id) || false
)
