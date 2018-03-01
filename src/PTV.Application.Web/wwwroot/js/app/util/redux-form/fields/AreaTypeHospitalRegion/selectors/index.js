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
import { Map, List } from 'immutable'
import {
  getEntitiesForIds,
  getIdFromProps,
  getParameterFromProps
} from 'selectors/base'

export const getHospitalRegionIds = createSelector(
    EnumsSelectors.hospitalRegions.getEnums,
    hospitalRegions => hospitalRegions || List()
)

export const getHospitalRegion = createSelector(
    [EntitySelectors.hospitalRegions.getEntities, getIdFromProps],
    (entities, id) => entities.get(id) || Map()
)

const createHospitalRegionsSelector = idsSelector => createSelector(
    [EntitySelectors.hospitalRegions.getEntities, idsSelector],
    (hospitalRegions, ids) => getEntitiesForIds(hospitalRegions, List(ids), List())
)

export const getHospitalRegions = createHospitalRegionsSelector(getHospitalRegionIds)

export const getHospitalRegionsForIds = createHospitalRegionsSelector(getParameterFromProps('ids'))

export const getHospitalRegionsForIdsJs = createSelector(
  getHospitalRegionsForIds,
  list => list.map(x => ({ value: x.get('id'), label: x.get('name') })).toArray()
)
