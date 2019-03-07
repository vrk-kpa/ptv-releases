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
import { EntitySelectors } from 'selectors'
import { createSelector } from 'reselect'
import { Map } from 'immutable'
import { getFormValue } from 'selectors/base'

export const getAreaTypeAreaType = createSelector(
    EntitySelectors.areaInformationTypes.getEntities,
    areaInformationTypes =>
      areaInformationTypes.filter(ati => ati.get('code').toLowerCase() === 'areatype').first() || Map()
)

export const getAreaTypeAreaTypeId = createSelector(
    getAreaTypeAreaType,
    areaTypeAreaType => areaTypeAreaType.get('id') || ''
)

// form selectors
export const getFormAreaInformation = createSelector(
    getFormValue('areaInformation'),
    values => values || Map()
)

export const getFormAreaInformationTypeId = createSelector(
    getFormAreaInformation,
    areaInformation => areaInformation.get('areaInformationType') || ''
)

export const getIsTreeActive = createSelector(
    [getAreaTypeAreaTypeId, getFormAreaInformationTypeId],
    (areaTypeAreaTypeId, formAreaInformationTypeId) => areaTypeAreaTypeId === formAreaInformationTypeId
)
