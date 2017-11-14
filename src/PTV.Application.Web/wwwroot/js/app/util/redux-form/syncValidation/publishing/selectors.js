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
import { getParameterFromProps } from 'selectors/base'
import { getContentLanguageCode } from 'selectors/selections'
import Entities, { getEntity, getSelectedEntityId } from 'selectors/entities/entities'
import { createSelector } from 'reselect'
import { Map, List, fromJS, Iterable } from 'immutable'
import { normalize, schema } from 'normalizr'
import CommonMessages from 'util/redux-form/messages'
import { createGetFlatFormValidationErrors } from 'util/redux-form/selectors/errorMessages'

const mappings = Map({
  WebpageUrl: 'urlAddress',
  AttachmentWebaddress: 'urlAddress',
  PrintableFormChannelUrl: 'urlAddress',
  StreetAddressStreetName: 'street',
  StreetsAddressPostalCodeId: 'postalCode',
  PostOfficeBoxAddressName: 'poBox',
  PostOfficeBoxAddressPostalCodeId: 'postalCode',
  ForeignAddressTextName: 'foreignAddressText',
  FaxNumber: 'phoneNumber'
})

const getMappings = (key, keyMappings) => keyMappings.get(key) || key

const pathMappings = Map({
  areaType: 'areaInformation'
})

const getEntityAvailableLanguages = createSelector(
  getEntity,
  entity => entity.get('languagesAvailabilities') || List()
)

const getEntityAvailableLanguageIndexByCode = createSelector(
  [getEntityAvailableLanguages, Entities.languages.getEntities],
  (entitylanguages, languages) => entitylanguages.reduce(
    (prev, curr, index) => prev.set(languages.getIn([curr.get('languageId'), 'code']), index),
    Map()
  )
)

const getContentLanguageIndex = createSelector(
  [getEntityAvailableLanguageIndexByCode, getContentLanguageCode],
  (languages, languageCode) => languages.get(languageCode) || 0
)

const getEntityAvailableLanguage = createSelector(
  [getEntity, getParameterFromProps('rowIndex'), getContentLanguageIndex],
  (entity, row, contentIndex) => {
    // console.log(entity, row, a)
    return entity.getIn(['languagesAvailabilities', row != null ? row : contentIndex]) || Map()
  }
)

const createLanguagePropertySelector = (property, defaultValue) => createSelector(
  getEntityAvailableLanguage,
  language => {
    const value = language.get(property)
    return value == null ? defaultValue : value
  }
)

const getValidatedFields = createLanguagePropertySelector('validatedFields', List())

const filteredPaths = [
  'ElectronicChannel',
  'ServiceLocationChannel',
  'PrintableFormChannel',
  'WebpageChannel'
]

const createGetValidations = validatedFieldsSelector => createSelector(
  [validatedFieldsSelector, getSelectedEntityId],
  (fields, id) => fields.reduce((prev, curr) =>
    prev.set(
      getMappings(curr.get('key'), mappings),
      curr.get('validationPaths').filter(x =>
        id !== x.get('id') &&
        filteredPaths.indexOf(x.get('name')) < 0
      )
    ),
    Map()
  )
)

const errorSchema = new schema.Entity('errors', {}, {
  processStrategy: (entity, parent, key) => {
    entity.key = key
    return entity
  }
})

const errors = new schema.Values(new schema.Array(errorSchema))

export const createGetNormalizedErrors = validatedFieldsSelector => createSelector(
  createGetValidations(validatedFieldsSelector),
  validation => validation.size ? fromJS(normalize(validation.toJS(), errors)) : null
)

export const getNormalizedErrors = createGetNormalizedErrors(getValidatedFields)

export const createErrors = (values, normalized, errorKey, path = [], onClickAction) => {
  const val = values.map((v, k) => {
    const err = normalized.getIn(['result', k])
    if (err) {
      // console.log('err', errorKey, k, err.toJS(), path)
      if ((err.size === 0 && path.length === 0) ||
        k === errorKey ||
        (pathMappings.get(k) === path.join('.'))) {
        const label = CommonMessages[k] || k
        // console.log('err', errorKey, label, k, err.toJS(), path)
        return Map({
          label: label,
          message: { id: 'Components.Validators.IsRequired.Message' },
          onClick: onClickAction
        })
      }
    }

    if (Iterable.isIterable(v)) {
      const id = v.get('id')
      const errEntity = id && normalized.getIn(['entities', 'errors', id]) || null
      const result = createErrors(v, normalized, errEntity && errEntity.get('key') || errorKey, [...path, k], onClickAction)
      if (result) {
        const filtered = result.filter(x => x)
        return filtered.size ? filtered : null
      }
    }
    return null
  })
  return val
}

const getValues = createSelector(
  [state => state, getParameterFromProps('formName')],
  (state, formName) => state.getIn(['form', formName, 'values'])
)

export const getPublishingValidationErrors = createSelector(
  [getNormalizedErrors, getValues, getParameterFromProps('onClick')],
  (normalized, values, onClick) =>
    normalized && createErrors(values.delete('languagesAvailabilities'), normalized, null, [], onClick) || null
)

export const getValidationMessages = createGetFlatFormValidationErrors(getPublishingValidationErrors)
