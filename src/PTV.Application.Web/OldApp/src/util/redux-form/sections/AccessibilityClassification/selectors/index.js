/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { EntitySelectors, EnumsSelectors } from 'selectors'
import { createSelector } from 'reselect'
import { createTranslatedIdSelector } from 'appComponents/Localize/selectors'
import { Map } from 'immutable'
import { getFormValueWithPath } from 'selectors/base'
import { languageTranslationTypes } from 'appComponents/Localize'

export const getAccessibilityClassificationCode = createSelector(
  EntitySelectors.accessibilityClassificationLevelTypes.getEntity,
  entity => entity.get('code')
)

export const getFormAccessibilityClassificationLevelTypeId = createSelector(
  getFormValueWithPath((props) => `${props.path}.accessibilityClassificationLevelType`),
  (value) => value || ''
)

export const getFormWcagLevelTypeId = createSelector(
  getFormValueWithPath((props) => `${props.path}.wcagLevelType`),
  (value) => value || ''
)

//FullyCompliant
const getAccessibilityClassificationLevelTypeFullyCompliantId = createSelector(
  EnumsSelectors.accessibilityClassificationLevelTypes.getEntities,
  accessibilityClassificationLevelTypes => {
    return accessibilityClassificationLevelTypes.filter(acType =>
      acType.get('code').toLowerCase() === 'fullycompliant'
    ).first() || Map()
  }
)

//PartiallyCompliant
const getAccessibilityClassificationLevelTypePartiallyCompliantId = createSelector(
  EnumsSelectors.accessibilityClassificationLevelTypes.getEntities,
  accessibilityClassificationLevelTypes => {
    return accessibilityClassificationLevelTypes.filter(acType =>
      acType.get('code').toLowerCase() === 'partiallycompliant'
    ).first() || Map()
  }
)

//NonCompliant
const getAccessibilityClassificationLevelTypeNonComplaintId = createSelector(
  EnumsSelectors.accessibilityClassificationLevelTypes.getEntities,
  accessibilityClassificationLevelTypes => {
    return accessibilityClassificationLevelTypes.filter(acType =>
      acType.get('code').toLowerCase() === 'noncompliant'
    ).first() || Map()
  }
)

//Unkown
const getAccessibilityClassificationLevelTypeUnkownId = createSelector(
  EnumsSelectors.accessibilityClassificationLevelTypes.getEntities,
  accessibilityClassificationLevelTypes => {
    return accessibilityClassificationLevelTypes.filter(acType =>
      acType.get('code').toLowerCase() === 'unknown'
    ).first() || Map()
  }
)

export const getIsWcagLevelTypeVisible = createSelector(
  [
    getAccessibilityClassificationLevelTypeFullyCompliantId,
    getAccessibilityClassificationLevelTypePartiallyCompliantId,
    getFormAccessibilityClassificationLevelTypeId
  ],
  (
    accessibilityClassificationLevelTypeFullyCompliantId,
    accessibilityClassificationLevelTypePartiallyCompliantId,
    getSelectedAccessClasificationLevelTypeId) => {
    return accessibilityClassificationLevelTypeFullyCompliantId.get('id') === getSelectedAccessClasificationLevelTypeId ||
      accessibilityClassificationLevelTypePartiallyCompliantId.get('id') === getSelectedAccessClasificationLevelTypeId
  }
)

export const getIsNameAndUrlAddressVisible = createSelector(
  [
    getAccessibilityClassificationLevelTypeFullyCompliantId,
    getAccessibilityClassificationLevelTypePartiallyCompliantId,
    getAccessibilityClassificationLevelTypeNonComplaintId,
    getFormAccessibilityClassificationLevelTypeId
  ],
  (
    accessibilityClassificationLevelTypeFullyCompliantId,
    accessibilityClassificationLevelTypePartiallyCompliantId,
    accessibilityClassificationLevelTypeNonComplaintId,
    getSelectedAccessClasificationLevelTypeId) => {
    return accessibilityClassificationLevelTypeFullyCompliantId.get('id') === getSelectedAccessClasificationLevelTypeId ||
      accessibilityClassificationLevelTypePartiallyCompliantId.get('id') === getSelectedAccessClasificationLevelTypeId ||
      accessibilityClassificationLevelTypeNonComplaintId.get('id') === getSelectedAccessClasificationLevelTypeId
  }
)

export const getIsAccessibilityClassificationLevelTypeNonComplaintSelected = createSelector(
  [
    getAccessibilityClassificationLevelTypeNonComplaintId,
    getFormAccessibilityClassificationLevelTypeId
  ],
  (accessibilityClassificationLevelTypeNonComplaintId, getSelectedAccessClasificationLevelTypeId) => {
    return accessibilityClassificationLevelTypeNonComplaintId.get('id') === getSelectedAccessClasificationLevelTypeId
  }
)

export const getIsAccessibilityClassificationLevelTypeUnkownSelected = createSelector(
  [
    getAccessibilityClassificationLevelTypeUnkownId,
    getFormAccessibilityClassificationLevelTypeId
  ],
  (accessibilityClassificationLevelTypeUnkownId, getSelectedAccessClasificationLevelTypeId) => {
    return accessibilityClassificationLevelTypeUnkownId.get('id') === getSelectedAccessClasificationLevelTypeId
  }
)

//PREVIEW DATA Titles selecting from data
const getData = (_, { data }) => data

const getDataAccessibilityClassification = createSelector(
  getData,
  data => data || Map()
)

export const getDataAccessibilityClassificationLevelTypeId = createSelector(
  [
    getDataAccessibilityClassification,
    getAccessibilityClassificationLevelTypeUnkownId
  ],
  (value,
    defaultValue
  ) => {
    return value.get('accessibilityClassificationLevelType') || defaultValue.get('id') || ''
  }
)

export const getDataWcagLevelTypeId = createSelector(
  getDataAccessibilityClassification,
  (value) => value.get('wcagLevelType') || ''
)

export const getDataUrlAddress = createSelector(
  getDataAccessibilityClassification,
  (value) => value.get('urlAddress') || ''
)

export const getDataName = createSelector(
  getDataAccessibilityClassification,
  (value) => value.get('name') || ''
)

export const getIsFullyOrPartiallyCompliantLevelTypeSelected = createSelector(
  [
    getAccessibilityClassificationLevelTypeFullyCompliantId,
    getAccessibilityClassificationLevelTypePartiallyCompliantId,
    getDataAccessibilityClassificationLevelTypeId
  ],
  (
    accessibilityClassificationLevelTypeFullyCompliantId,
    accessibilityClassificationLevelTypePartiallyCompliantId,
    getSelectedAccessClasificationLevelTypeId) => {
    return accessibilityClassificationLevelTypeFullyCompliantId.get('id') === getSelectedAccessClasificationLevelTypeId ||
      accessibilityClassificationLevelTypePartiallyCompliantId.get('id') === getSelectedAccessClasificationLevelTypeId
  }
)

export const getIsNotCompliantLevelTypeSelected = createSelector(
  [
    getAccessibilityClassificationLevelTypeNonComplaintId,
    getDataAccessibilityClassificationLevelTypeId
  ],
  (accessibilityClassificationLevelTypeNonComplaintId,
    getSelectedAccessClasificationLevelTypeId) => {
    return accessibilityClassificationLevelTypeNonComplaintId.get('id') === getSelectedAccessClasificationLevelTypeId
  }
)

export const getIsUnkownLevelTypeSelected = createSelector(
  [
    getAccessibilityClassificationLevelTypeUnkownId,
    getDataAccessibilityClassificationLevelTypeId
  ],
  (accessibilityClassificationLevelTypeUnkownId, getSelectedAccessClasificationLevelTypeId) => {
    return accessibilityClassificationLevelTypeUnkownId.get('id') === getSelectedAccessClasificationLevelTypeId
  }
)

export const getTranslatedAccessibilityClassificationLevelType = createTranslatedIdSelector(
  getDataAccessibilityClassificationLevelTypeId, {
    languageTranslationType: languageTranslationTypes.both
  }
)

export const getTranslatedWcagLevelType = createTranslatedIdSelector(
  getDataWcagLevelTypeId, {
    languageTranslationType: languageTranslationTypes.both
  }
)

export const getAccessibilityClasificationTitleWithAllValues = createSelector(
  [
    getTranslatedAccessibilityClassificationLevelType,
    getTranslatedWcagLevelType,
    getDataName,
    getDataUrlAddress
  ],
  (aclevelType, wcagLevelType, name, urlAddress) => {
    const infoString = [aclevelType, wcagLevelType, name, urlAddress].filter(x => x)
    return infoString.length > 0 && infoString.join(', ') || null
  }
)

export const getAccessibilityClasificationTitleWithoutWcaglevelType = createSelector(
  [
    getTranslatedAccessibilityClassificationLevelType,
    getDataName,
    getDataUrlAddress
  ],
  (aclevelType, name, urlAddress) => {
    const infoString = [aclevelType, name, urlAddress].filter(x => x)
    return infoString.length > 0 && infoString.join(', ') || null
  }
)
