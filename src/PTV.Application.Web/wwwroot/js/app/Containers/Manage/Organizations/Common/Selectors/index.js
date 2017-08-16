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
import * as CommonSelectors from '../../../../Common/Selectors'
import { Map, List } from 'immutable'
// types
import { addressTypes } from '../../../../Common/Helpers/types'

// Shared
export const getOrganizationEntities = createSelector(
    CommonSelectors.getEntities,
    entities => entities.get('organizations') || Map()
)

export const getOrganizations = createSelector(
     [getOrganizationEntities, CommonSelectors.getLanguageParameter],
    (organizations, language) => organizations.map((organization) => organization.get(language))
)

export const getOrganizationForId = createSelector(
    [getOrganizationEntities, CommonSelectors.getIdFromProps],
    (entity, id) => entity.get(id) || Map()
)

export const getOrganizationNameForId = createSelector(
    getOrganizationForId,
    organistion => organistion.get('name')
)

export const getOrganizationsJS = createSelector(
    getOrganizations,
    organizations => CommonSelectors.getJS(organizations, [])
)

export const getTopOrganizations = createSelector(
    CommonSelectors.getEnums,
    results => results.get('organizations') || List()
)

export const getOrganizationsObjectArray = createSelector(
    [getOrganizationEntities, getTopOrganizations, CommonSelectors.getParameterFromProps('withoutOrganizationId')],
    (organizations, ids, wId) => CommonSelectors.getObjectArray(ids
        .filter(id => id !== wId)
        .map(id => organizations.get(id))
        .sortBy(x => x.get('name'))
    )
)

export const getTopAvailableOrganizations = createSelector(
    CommonSelectors.getEnums,
    results => results.get('availableOrganizations') || List()
)

export const getAvailableOrganizationsObjectArray = createSelector(
    [getOrganizationEntities, getTopAvailableOrganizations, CommonSelectors.getParameterFromProps('withoutOrganizationId')],
    (organizations, ids, wId) => CommonSelectors.getObjectArray(ids
        .filter(id => id !== wId)
        .map(id => organizations.get(id))
        .sortBy(x => x.get('name'))
    )
)

export const getFilteredOrganizations = createSelector(
    CommonSelectors.getEntities,
    entities => entities.get('filteredOrganizations') || List()
)

export const getFilteredOrganization = createSelector(
    [getFilteredOrganizations, CommonSelectors.getIdFromProps],
    (entity, id) => entity.get(id)
)

export const getOrganizationWithFiltered = createSelector(
    [getOrganizationForId, getFilteredOrganization],
    (entity, filtered) => entity.size > 0 ? entity : filtered
)

export const getTopFilteredOrganizations = createSelector(
    CommonSelectors.getResults,
    results => results.get('filteredOrganizations') || null
)
// Shared

// Org related
export const serviceKeyToState = 'organization'
export const getOrganizationId = createSelector(
    CommonSelectors.getPageModeState,
    pageState => CommonSelectors.getEntityId(pageState.get(serviceKeyToState))
)

export const getOrganization = createSelector(
    [getOrganizations, getOrganizationId],
    (organizations, organizationId) => organizations.get(organizationId) || Map()
)

export const getOrganizationEntity = createSelector(
    [getOrganizationEntities, getOrganizationId],
    (organizationEntities, organizationId) => organizationEntities.get(organizationId) || Map()
)

export const getPublishingStatus = createSelector(
    getOrganizationEntity,
    organizationEntity => organizationEntity.get('publishingStatusId') || Map()
)

export const getUnificRootId = createSelector(
    getOrganization,
    organization => organization.get('unificRootId') || Map()
)

export const getOrganizationParentId = createSelector(
    getOrganization,
    organization => organization.get('parentId') || null
)

export const getSubOrganizationSelected = createSelector(
    getOrganization,
    organization => organization.get('isSubOrganizationSelected')
)

export const getIsSubOrganizationSelected = createSelector(
    getSubOrganizationSelected,
    isSubOrganization => (isSubOrganization !== undefined ? isSubOrganization : false)
)

export const getMunicipalityCode = createSelector(
    getOrganization,
    entities => entities.get('municipalityCode') || ''
)

export const getMunicipalities = createSelector(
    CommonSelectors.getEntities,
    entities => entities.get('municipalities') || Map()
)

export const getMunicipalityId = createSelector(
    getOrganization,
    entities => entities.get('municipality') || null
)

export const getMunicipalityEntity = createSelector(
    [getMunicipalities, getMunicipalityId],
    (entities, id) => entities.get(id) || null
)

export const getMunicipalityJS = createSelector(
    getMunicipalityEntity,
    entity => entity ? CommonSelectors.getJS(entity) : null
)

// Org description container
export const getName = createSelector(
    getOrganization,
    entities => entities.get('organizationName') || ''
)

export const getAlternateName = createSelector(
    getOrganization,
    entities => entities.get('organizationAlternateName') || ''
)

export const getDescription = createSelector(
    getOrganization,
    entities => entities.get('description') || ''
)

export const getOrganizationStringId = createSelector(
    getOrganization,
    entities => entities.get('organizationId') || ''
)

export const isAlternateNameUsedAsDisplayNameSelected = createSelector(
    getOrganization,
    entities => entities.get('isAlternateNameUsedAsDisplayName') || false
)

export const getBusinessId = createSelector(
    getOrganization,
    entities => entities.get('business') || null
)

export const getShowContacts = createSelector(
    getOrganization,
    organization => organization.get('showContacts') || Map()
)

export const ShowPostalAddress = createSelector(
    getOrganization,
    organization => organization.get('ShowPostalAddress') || Map()
)

export const ShowVisitingAddress = createSelector(
    getOrganization,
    organization => organization.get('ShowVisitingAddress') || Map()
)

export const getBusiness = createSelector(
    CommonSelectors.getEntities,
    entities => entities.get('business') || Map()
)

export const getBusinessEntity = createSelector(
    [getBusiness, getBusinessId],
    (entities, id) => entities.get(id) || null
)

// Org type
export const getOrganizationTypes = createSelector(
    CommonSelectors.getEntities,
    entities => entities.get('organizationTypes') || List()
)

export const getOrganizationTypesIds = createSelector(
    CommonSelectors.getEnums,
    entities => entities.get('organizationTypes') || List()
)

export const getOrganizationTypesObjectArray = createSelector(
    getOrganizationTypes,
    organizationTypes =>
    CommonSelectors.getObjectArray(organizationTypes.map(x => x.set('disabled', x.get('isDisabled'))))
)

export const getOrganizationTypesJS = createSelector(
    getOrganizationTypes,
    organizationTypes => CommonSelectors.getJS(organizationTypes, [])
)

export const getOrganizationTypeMap = createSelector(
    [getOrganizationTypes, CommonSelectors.getParameters],
    (organizationTypes, code) => organizationTypes
    .filter(chT => chT.get('code').toLowerCase() === code).first() || Map()
)

export const getOrganizationTypeForCode = createSelector(
    getOrganizationTypeMap,
    organizationType => organizationType.get('id') || ''
)

export const getOrganizationType = createSelector(
    getOrganization,
    entities => entities.get('organizationTypeId') || null
)

export const getOrganizationTypeSelector = code => createSelector(
    getOrganizationTypes,
    orgTypes => {
      return orgTypes
        .filter(chT => chT.get('code').toLowerCase() === code).first() || Map()
    }
)

const getOrganizationTypeMunicipalityId = CommonSelectors.getIdSelector(
    getOrganizationTypeSelector('municipality'))

export const isMunicipalityTypeSelected = createSelector(
    [getOrganizationType, getOrganizationTypeMunicipalityId],
    (selectedId, municipalityId) => selectedId === municipalityId
)

export const getIsOrganizationTypeSelected = createSelector(
    [getOrganizationType, CommonSelectors.getIdFromProps],
    (selectedOrganizationType, id) => selectedOrganizationType === id
)

const getCodeSelector = selector => createSelector(
  selector,
  entity => entity.get('code')
)

const getSelectedOrganizationType = createSelector(
    [getOrganizationTypes, getOrganizationType],
    (organizationTypes, id) => organizationTypes.get(id) || Map()
)

const getSelectedOrganizationTypeCode = getCodeSelector(getSelectedOrganizationType)

const getIsOrganizationTypesSelected = types => createSelector(
  getSelectedOrganizationTypeCode,
  code => List(types).includes(code)
)

// Org Area information
export const getAreaInformationType = createSelector(
    getOrganization,
    organization => organization.get('areaInformationType') || ''
)

export const getIsFilteredCodeSelected = getIsOrganizationTypesSelected(['State', 'Organization', 'Company'])

const getAreaInformationTypeWholeCountryId = CommonSelectors.getIdSelector(CommonSelectors.getDefaultAreaInformationTypeSelector('wholecountry'))
const getAreaInformationTypeAreaTypeId = CommonSelectors.getIdSelector(CommonSelectors.getDefaultAreaInformationTypeSelector('areatype'))

const getSelectedAreaInformationTypeId = createSelector(
    [getOrganization, CommonSelectors.getAreaInformationTypeId],
    (organization, defaultAreaInformationType) =>
    organization.get('areaInformationTypeId') || null
)

const getSelectedAreaInformationType = createSelector(
  [CommonSelectors.getAreaInformationTypes, getSelectedAreaInformationTypeId],
  (types, areaInformationAreaTypeId) => types.get(areaInformationAreaTypeId) || Map()
)

export const getAreaInformationTypeId = createSelector(
  [
    getSelectedAreaInformationTypeId,
    getIsFilteredCodeSelected,
    CommonSelectors.getAreaInformationTypeId,
    getSelectedOrganizationTypeCode,
    getAreaInformationTypeWholeCountryId,
    getAreaInformationTypeAreaTypeId
  ],
  (
      selectedInfoAreaTypeId,
      filteredCodeSelected,
      defaultAreaInformationType,
      organizationTypeCode,
      aitWholeCountryId,
      aitAreaTypeId
  ) =>
      (filteredCodeSelected
          ? selectedInfoAreaTypeId
          : organizationTypeCode === 'Municipality'
              ? null
              : aitAreaTypeId
    ) || aitWholeCountryId
)

export const getAreaTypeId = createSelector(
    getOrganization,
    organization => organization.get('areaTypeId') || null
)

export const getSelectedAreaMunicipality = createSelector(
    getOrganization,
    organization => organization.get('areaMunicipality') || List()
)

export const getSelectedAreaBusinessRegions = createSelector(
    getOrganization,
    organization => organization.get('areaBusinessRegions') || List()
)

export const getSelectedAreaHospitalRegions = createSelector(
    getOrganization,
    organization => organization.get('areaHospitalRegions') || List()
)

export const getSelectedAreaProvince = createSelector(
    getOrganization,
    organization => organization.get('areaProvince') || List()
)

// Org emails
export const getOrganizationEmails = createSelector(
    getOrganization,
    organization => organization.get('emails') || Map()
)

export const getOrganizationEmailEntities = createSelector(
    [CommonSelectors.getEmails, getOrganizationEmails],
    (emails, organizationEmails) => CommonSelectors.getEntitiesForIds(emails, organizationEmails) || List()
)

// Org phone numbers
export const getPhoneNumbers = createSelector(
    CommonSelectors.getEntities,
    entities => entities.get('phoneNumbers') || Map()
)

export const getOrganizationPhoneNumbers = createSelector(
    getOrganization,
    organization => organization.get('phoneNumbers') || Map()
)

export const getOrganizationPhoneNumberEntities = createSelector(
    [getPhoneNumbers, getOrganizationPhoneNumbers],
    (phoneNumbers, organizationPhoneNumbers) =>
    CommonSelectors.getEntitiesForIds(phoneNumbers, organizationPhoneNumbers) || List()
)

// Org webPages
export const getWebPages = createSelector(
    CommonSelectors.getEntities,
    entities => entities.get('webPages') || Map()
)

export const getOrganizationWebPages = createSelector(
    getOrganization,
    entities => entities.get('webPages') || Map()
)

export const getOrganizationWebPageEntities = createSelector(
    [getWebPages, getOrganizationWebPages],
    (webPages, organizationWebPages) => CommonSelectors.getEntitiesForIds(webPages, organizationWebPages) || List()
)

export const getSortedOrganizationWebPages = createSelector(
    getOrganizationWebPageEntities,
    webPages => webPages.sortBy(webPage => webPage.get('orderNumber')).map(webPage => webPage.get('id')) || List()
)

// Org addresses
export const getPostalAddresses = createSelector(
    getOrganization,
    organization => organization.get(addressTypes.POSTAL) || List()
)

export const getVisitingAddresses = createSelector(
    getOrganization,
    organization => organization.get(addressTypes.VISITING) || List()
)

export const getOrganizationPostalAddressEntities = createSelector(
    [CommonSelectors.getAddressesForModel, getPostalAddresses],
    (addresses, postalAddresses) => CommonSelectors.getEntitiesForIds(addresses, postalAddresses) || List()
)

export const getOrganizationVisitingAddressEntities = createSelector(
    [CommonSelectors.getAddressesForModel, getVisitingAddresses],
    (addresses, visitingAdresses) => CommonSelectors.getEntitiesForIds(addresses, visitingAdresses) || List()
)

// Areas
export const getSelectedAreaMunicipalitiesJS = createSelector(
    [getMunicipalities, getSelectedAreaMunicipality],
    (allAreaMunicipalities, selectedAreaMunicipalities) => selectedAreaMunicipalities
      ? CommonSelectors.getJS(selectedAreaMunicipalities.map(id => allAreaMunicipalities.get(id)))
      : null
)
export const getOrderedSelectedAreaMunicipalitiesJS = createSelector(
    [getMunicipalities, getSelectedAreaMunicipality],
    (allAreaMunicipalities, selectedAreaMunicipalities) => selectedAreaMunicipalities
      ? CommonSelectors.getJS(
          selectedAreaMunicipalities.map(
            id => allAreaMunicipalities.get(id)
          ).sortBy(x => x.get('name'))
        )
      : null
)

export const getSelectedProvinciesJS = createSelector(
    [CommonSelectors.getProvincies, getSelectedAreaProvince],
    (allProvincies, selectedProvincies) => selectedProvincies
      ? CommonSelectors.getJS(selectedProvincies.map(id => allProvincies.get(id)))
      : null
)
export const getOrderedSelectedProvinciesJS = createSelector(
    [CommonSelectors.getProvincies, getSelectedAreaProvince],
    (allProvincies, selectedProvincies) => selectedProvincies
      ? CommonSelectors.getJS(
          selectedProvincies.map(
            id => allProvincies.get(id)
          ).sortBy(x => x.get('name'))
        )
      : null
)

export const getSelectedBusinessRegionsJS = createSelector(
    [CommonSelectors.getBusinessRegions, getSelectedAreaBusinessRegions],
    (allBusinessRegions, selectedBusinessRegions) => selectedBusinessRegions
      ? CommonSelectors.getJS(selectedBusinessRegions.map(id => allBusinessRegions.get(id)))
      : null
)
export const getOrderedSelectedBusinessRegionsJS = createSelector(
    [CommonSelectors.getBusinessRegions, getSelectedAreaBusinessRegions],
    (allBusinessRegions, selectedBusinessRegions) => selectedBusinessRegions
      ? CommonSelectors.getJS(
          selectedBusinessRegions.map(
            id => allBusinessRegions.get(id)
          ).sortBy(x => x.get('name'))
        )
      : null
)

export const getSelectedHospitalRegionsJS = createSelector(
    [CommonSelectors.getHospitalRegions, getSelectedAreaHospitalRegions],
    (allHospitalRegions, selectedHospitalRegions) => selectedHospitalRegions
      ? CommonSelectors.getJS(selectedHospitalRegions.map(id => allHospitalRegions.get(id)))
      : null
)
export const getOrderedSelectedHospitalRegionsJS = createSelector(
    [CommonSelectors.getHospitalRegions, getSelectedAreaHospitalRegions],
    (allHospitalRegions, selectedHospitalRegions) => selectedHospitalRegions
      ? CommonSelectors.getJS(
          selectedHospitalRegions.map(
            id => allHospitalRegions.get(id)
          ).sortBy(x => x.get('name'))
        )
      : null
)

export const getSelectedAreaCount = createSelector(
  [getSelectedAreaMunicipality,
    getSelectedAreaBusinessRegions,
    getSelectedAreaHospitalRegions,
    getSelectedAreaProvince],
    (municipality, business, hospital, provicies) => municipality.size + business.size + hospital.size + provicies.size
)

const getAreaTypeMunicipalityId = CommonSelectors.getIdSelector(
    CommonSelectors.getAreaTypeSelector('municipality'))
const getAreaTypeBusinessRegionsId = CommonSelectors.getIdSelector(
    CommonSelectors.getAreaTypeSelector('businessregions'))
const getAreaTypeHospitalRegionsId = CommonSelectors.getIdSelector(
    CommonSelectors.getAreaTypeSelector('hospitalregions'))
const getAreaTypeProvinceId = CommonSelectors.getIdSelector(
    CommonSelectors.getAreaTypeSelector('province'))

export const getSelectedAreaType = createSelector(
  [getAreaTypeId, getSelectedAreaMunicipality, getAreaTypeMunicipalityId,
    getSelectedAreaBusinessRegions, getAreaTypeBusinessRegionsId,
    getSelectedAreaHospitalRegions, getAreaTypeHospitalRegionsId,
    getSelectedAreaProvince, getAreaTypeProvinceId],
    (areaTypeId,
    municipalities, municipalityType,
    businessRegions, businessType,
    hospitalRegions, hospitalType,
    provincies, provinceType) =>
    municipalities.size && municipalityType ||
    provincies.size && provinceType ||
    businessRegions.size && businessType ||
    hospitalRegions.size && hospitalType ||
    areaTypeId
)


