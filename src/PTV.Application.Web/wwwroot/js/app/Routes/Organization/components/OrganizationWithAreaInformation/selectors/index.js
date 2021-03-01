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
import { Map } from 'immutable'
import { getFormValue } from 'selectors/base'
import { getSelectedEntityId } from 'selectors/entities/entities'
import { EntitySelectors } from 'selectors'
// form selectors
const getFormOrganizationTypeId = createSelector(
  getFormValue('organizationType'),
  values => values || ''
)

const getMunicipalityOrganizationType = createSelector(
  EntitySelectors.organizationTypes.getEntities,
  organizationType => (
    organizationType.filter(aT => aT.get('code').toLowerCase() === 'municipality')
      .first() || Map()).get('id') || ''
)

const getRegionalOrganizationType = createSelector(
  EntitySelectors.organizationTypes.getEntities,
  organizationType => (
    organizationType.filter(aT => aT.get('code').toLowerCase() === 'regionalorganization')
      .first() || Map()).get('id') || ''
)

export const getIsOrganizationTypeMunicipalitySelected = createSelector(
  [getFormOrganizationTypeId, getMunicipalityOrganizationType],
  (typeId, municipalityId) => typeId === municipalityId
)

export const getIsOrganizationTypeRegionalSelected = createSelector(
  [getFormOrganizationTypeId, getRegionalOrganizationType],
  (typeId, regionalId) => typeId === regionalId
)

export const getIsAreaInformationVisible = createSelector(
  [getFormOrganizationTypeId, getMunicipalityOrganizationType],
  (typeId, municipalityId) => typeId !== municipalityId && typeId !== ''
)

export const getIsAreaInformationWarningVisible = createSelector(
  [getIsAreaInformationVisible, getSelectedEntityId],
  (areaIsSwown, entityId) => areaIsSwown && entityId
)

export const getProvinceAreaType = createSelector(
  EntitySelectors.areaTypes.getEntities,
  areaInformationType => (
    areaInformationType.filter(aT => aT.get('code').toLowerCase() === 'province')
      .first() || Map()).get('id') || ''
)

export const getRegionOrganizationType = createSelector(
  EntitySelectors.organizationTypes.getEntities,
  organizationType => (
    organizationType.filter(aT => aT.get('code').toLowerCase() === 'region')
      .first() || Map()).get('id') || ''
)

export const getIsOrganizationTypeRegionSelected = createSelector(
  [getFormOrganizationTypeId, getRegionOrganizationType],
  (typeId, regionId) => typeId === regionId
)

