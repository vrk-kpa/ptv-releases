/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
*
* Permission is hereby granted, free of areaInformation, to any person obtaining a copy
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
import { EntitySelectors } from 'selectors'
import { createSelector } from 'reselect'
import { Map, List } from 'immutable'
import { getFormValueWithPath, getParameterFromProps } from 'selectors/base'

// form selectors
// const getParentPath = createSelector(
//   getParameterFromProps('parentPath'),
//   path => path ? (path + '.') : ''
// )
const getParentPath = props => props.parentPath ? (props.parentPath + '.') : ''

export const getFormAreaInformation = createSelector(
    getFormValueWithPath(props => getParentPath(props) + 'areaInformation'),
    values => values || Map()
)

export const getFormAreaTypeId = createSelector(
    getFormAreaInformation,
    areaInformation => areaInformation.get('areaType') || ''
)

export const getFormAreaType = createSelector(
    [EntitySelectors.areaTypes.getEntities, getFormAreaTypeId],
    (areaTypes, formAreaTypeId) => areaTypes.get(formAreaTypeId) || Map()
)

export const getFormAreaTypeCode = createSelector(
    getFormAreaType,
    formAreaType => formAreaType.get('code') || ''
)

export const getFormAreaTypeCodeLower = createSelector(
    getFormAreaTypeCode,
    formAreaTypeCode => formAreaTypeCode.toLowerCase() || ''
)

export const getSelectedAreasCount = createSelector(
    getFormAreaInformation,
    areaInformation => List(['businessRegions', 'municipalities', 'hospitalRegions', 'provinces'])
      .reduce((result, type) => result + (areaInformation.get(type) || Map()).size, 0)
)

export const getAreaInformationTypeAreaType = createSelector(
  EntitySelectors.areaInformationTypes.getEntities,
  aiTypes => aiTypes.find(st => st.get('code').toLowerCase() === 'areatype') || Map()
)

export const getAreaInformationTypeAreaTypeId = createSelector(
  getAreaInformationTypeAreaType,
  aiTypeAreaTypeId => {
    return aiTypeAreaTypeId.get('id') || ''
  }
)

export const getSelectedAreaInformationTypeId = createSelector(
  getFormAreaInformation,
  areaInformation => {
    return areaInformation.get('areaInformationType') || ''
  }
)

export const getIsAreaInformationTypeAreaTypeSelected = createSelector(
  [getSelectedAreaInformationTypeId, getAreaInformationTypeAreaTypeId],
  (selectedAiTypeId, aiTypeAreaTypeId) => {
    return selectedAiTypeId === aiTypeAreaTypeId
  }
)
