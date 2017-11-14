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
import { Map, Set, List } from 'immutable'
import * as BaseSelectors from '../base'

export const getEntities = createSelector(
    BaseSelectors.getCommon,
    common => common.get('entities') || Map()
)

export const getEnums = createSelector(
    BaseSelectors.getCommon,
    common => common.get('enums') || Map()
)

const createGetEntitiesSelector = (enumName, entityName) => createSelector(
  [getEntities, getEnums],
  (entities, enums) => BaseSelectors.getEntitiesForIds(
    entities.get(entityName),
    enums.get(enumName),
    List())
)

const createGetEntitiesMapSelector = (getEntitiesSelector) => createSelector(
  getEntitiesSelector,
  entities => entities.reduce((result, item) => result.set(item.get('id'), item), Map())
)

const createGetEnumsSelector = enumName => createSelector(
  getEnums,
  enums => enums.get(enumName) || List()
)

const createGetIsEnumLoadedSelector = enumName => createSelector(
  getEnums,
  enums => !!enums.get(enumName)
)

const enums = Map({
  areaInformationTypes: 'areaInformationTypes',
  areaTypes: 'areaTypes',
  businessRegions: 'businessRegions',
  chargeTypes: 'chargeTypes',
  dialCodes: 'dialCodes',
  digitalAuthorizations: 'digitalAuthorizations',
  fundingTypes: 'fundingTypes',
  hospitalRegions: 'hospitalRegions',
  industrialClasses: 'industrialClasses',
  keywords: 'keywords',
  languages: 'languages',
  laws: 'laws',
  lifeEvents: 'lifeEvents',
  municipalities: 'municipalities',
  organizations: 'organizations',
  organizationTypes: 'organizationTypes',
  phoneNumberTypes: 'phoneNumberTypes',
  printableFormUrlTypes: 'printableFormUrlTypes',
  postalCodes: 'postalCodes',
  provinces: 'provinces',
  provisionTypes: 'provisionTypes',
  publishingStatuses: 'publishingStatuses',
  serviceChannelConnectionTypes: 'serviceChannelConnectionTypes',
  serviceClasses: 'serviceClasses',
  serviceTypes: 'serviceTypes',
  targetGroups: 'targetGroups',
  topLifeEvents: 'lifeEvents',
  topServiceClasses: 'serviceClasses',
  topTargetGroups: 'targetGroups',
  topDigitalAuthorizations: 'digitalAuthorizations',
  translationLanguages: 'languages',
  organizationRoles: 'organizationRoles',
  userOrganizations: 'organizations'
})

const enumsSelectors = {}

enums.map((value, key) => {
  const getEnums = createGetEnumsSelector(key)
  const getEntities = createGetEntitiesSelector(key, value)
  const getEntitiesMap = createGetEntitiesMapSelector(getEntities)
  const getIsEnumLoaded = createGetIsEnumLoadedSelector(key)

  enumsSelectors[key] = {
    getEntities,
    getEntitiesMap,
    getEnums,
    getIsEnumLoaded
  }
})

export default enumsSelectors
