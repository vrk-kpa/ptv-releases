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
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { createSelector } from 'reselect'
import { Map, List } from 'immutable'
import { getFormValueWithPath } from 'selectors/base'
import { createTranslatedIdSelector, createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'

export const getAreaTypeAreaType = createSelector(
  EntitySelectors.areaInformationTypes.getEntities,
  areaInformationTypes => areaInformationTypes.filter(ati =>
    ati.get('code').toLowerCase() === 'areatype').first() ||
    Map()
)

export const getAreaTypeAreaTypeId = createSelector(
  getAreaTypeAreaType,
  areaTypeAreaType => areaTypeAreaType.get('id') || null
)

// form selectors
const getParentPath = props => {
  return props.parentPath ? (props.parentPath + '.') : ''
}

export const getFormAreaInformation = createSelector(
  getFormValueWithPath(props => getParentPath(props) + 'areaInformation'),
  values => values || Map()
)

export const getFormAreaInformationTypeId = createSelector(
  getFormAreaInformation,
  areaInformation => areaInformation.get('areaInformationType') || null
)

export const getIsTreeActive = createSelector(
  [getAreaTypeAreaTypeId, getFormAreaInformationTypeId],
  (areaTypeAreaTypeId, formAreaInformationTypeId) => areaTypeAreaTypeId === formAreaInformationTypeId
)

const getData = (_, { data }) => data

// AREA INFORMATION
const getAreaInformation = createSelector(
  getData,
  data => data || Map()
)
// AREA INFORMATION TYPE
const getAreaInformationType = createSelector(
  getAreaInformation,
  area => area.get('areaInformationType') || null
)
export const getTranslatedAreaInformationType = createTranslatedIdSelector(
  getAreaInformationType, {
    languageTranslationType: languageTranslationTypes.both
  }
)
// AREA MUNICIPALITIES
const getAreaInformationMunicipalities = createSelector(
  getAreaInformation,
  area => area.get('municipalities') || List()
)
const getAreaInformationMunicipalitiesIds = createSelector(
  getAreaInformationMunicipalities,
  ids => ids.map(id => Map({ id }))
)
const getTranslatedAreaInformationMunicipalities = createTranslatedListSelector(
  getAreaInformationMunicipalitiesIds, {
    languageTranslationType: languageTranslationTypes.both
  }
)
export const getTranslatedAreaInformationMunicipalitiesTitle = createSelector(
  getTranslatedAreaInformationMunicipalities,
  items => items.map(x => x.get('name')).join(', ')
)

const getAreaInformationProvinces = createSelector(
  getAreaInformation,
  area => area.get('provinces') || List()
)
const getAreaInformationProvincesIds = createSelector(
  getAreaInformationProvinces,
  ids => ids.map(id => Map({ id }))
)
const getTranslatedAreaInformationProvinces = createTranslatedListSelector(
  getAreaInformationProvincesIds, {
    languageTranslationType: languageTranslationTypes.both
  }
)
export const getTranslatedAreaInformationProvincesTitle = createSelector(
  getTranslatedAreaInformationProvinces,
  items => items.map(x => x.get('name')).join(', ')
)

const getAreaInformationBusinessRegions = createSelector(
  getAreaInformation,
  area => area.get('businessRegions') || List()
)
const getAreaInformationBusinessRegionsIds = createSelector(
  getAreaInformationBusinessRegions,
  ids => ids.map(id => Map({ id }))
)
const getTranslatedAreaInformationBusinessRegions = createTranslatedListSelector(
  getAreaInformationBusinessRegionsIds, {
    languageTranslationType: languageTranslationTypes.both
  }
)
export const getTranslatedAreaInformationBusinessRegionsTitle = createSelector(
  getTranslatedAreaInformationBusinessRegions,
  items => items.map(x => x.get('name')).join(', ')
)

const getAreaInformationHospitalRegions = createSelector(
  getAreaInformation,
  area => area.get('hospitalRegions') || List()
)
const getAreaInformationHospitalRegionsIds = createSelector(
  getAreaInformationHospitalRegions,
  ids => ids.map(id => Map({ id }))
)
const getTranslatedAreaInformationHospitalRegions = createTranslatedListSelector(
  getAreaInformationHospitalRegionsIds, {
    languageTranslationType: languageTranslationTypes.both
  }
)
export const getTranslatedAreaInformationHospitalRegionsTitle = createSelector(
  getTranslatedAreaInformationHospitalRegions,
  items => items.map(x => x.get('name')).join(', ')
)

const getAreaInformationTypeLimitedArea = createSelector(
  EnumsSelectors.areaInformationTypes.getEntities,
  areaInformationTypes => areaInformationTypes.filter(st =>
    st.get('code').toLowerCase() === 'areatype'
  ).first() || Map()
)

export const getIsLimitedAreaInformationType = createSelector(
  [
    getAreaInformationType,
    getAreaInformationTypeLimitedArea
  ],
  (id, limitedArea) => limitedArea.get('id') === id
)
