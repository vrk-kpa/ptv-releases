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
// import { getContentLanguageCode } from 'selectors/selections'
import Entities, { getEntity } from 'selectors/entities/entities'
import { createSelector } from 'reselect'
import { Map, Set, List, fromJS, Iterable } from 'immutable'
import { normalize, schema } from 'normalizr'
import CommonMessages from 'util/redux-form/messages'
import { createGetFlatFormValidationErrors } from 'util/redux-form/selectors/errorMessages'
import mergeWith from 'lodash/mergeWith'
import { getFormInitialValues } from 'redux-form/immutable'
import validationMessages from 'util/redux-form/validators/messages'
import { validationMessageTypes } from 'util/redux-form/validators/types'
import { createError } from 'util/redux-form/validators/isValid'
import { merger } from 'Middleware'

const mappings = Map({
  WebpageUrl: 'urlAddress',
  AttachmentWebaddress: 'urlAddress',
  PrintableFormChannelUrl: 'urlAddress',
  StreetAddressStreetId: 'street',
  StreetAddressMunicipalitId: 'municipality',
  StreetsAddressPostalCodeId: 'postalCode',
  PostOfficeBoxAddressName: 'poBox',
  PostOfficeBoxAddressPostalCodeId: 'postalCode',
  ForeignAddressTextName: 'foreignAddressText',
  FaxNumber: 'phoneNumber',
  OtherAddressCoordinates: 'coordinates',
  OtherAddressAdditionalInformation: 'additionalInformation',
  Coordinates: 'coordinates'
})

const getMappings = (key, keyMappings) => keyMappings.get(key) || key

// const pathMappings = Map({
//   areaType: 'areaInformation'
// })

const getEntityAvailableLanguages = createSelector(
  getEntity,
  entity => entity.get('languagesAvailabilities') || List()
)

const filteredPaths = [
  'ElectronicChannel',
  'ServiceLocationChannel',
  'PrintableFormChannel',
  'WebpageChannel',
  'Organization'
]

const errorTypes = {
  mandatoryField: { error: 'isRequired' },
  publishedMandatoryField: { error: 'invalidPublishingStatus' },
  publishedOrganizationLanguageMandatoryField: { error: 'invalidPublishingOrganizationLanguageStatus' },
  valueIsSame: { error: 'isEqual' },
  maxLimit: {
    error: 'itemMaxCountExceeded',
    getOptions: (key) => ({
      limit: key === 'ontologyTerms' ? 10 : 4
    })
  }
}
const getValidationError = (lang, languages) =>
  (lang.get('validatedFields') || List()).map(v =>
    v.get('validationPaths').reduce(
      (res, vp) => {
        if (!res.id || !filteredPaths.includes(vp.get('name'))) {
          res.id = vp.get('id')
        }
        return res
      }, {
        fields: {
          [getMappings(v.get('key'), mappings)]: {
            [v.get('errorType') || 'mandatoryField']:
              Set([languages.getIn([lang.get('languageId'), 'code']) || lang.get('languageId')])
          }
        }
      }
    )
  )

const getValidationErrors = createSelector(
  [getEntityAvailableLanguages, Entities.languages.getEntities],
  (langs, languageEntities) =>
    langs
      .map(l => getValidationError(l, languageEntities))
      .reduce((r, c) => {
        c.map(x => r.push(x))
        return r
      }, [])
)

const mergerJs = (a, b) => {
  if ((typeof a === 'object') && (typeof b === 'object')) {
    if (a.union) {
      return a.union(b)
    } else {
      return mergeWith(a, b, mergerJs)
    }
  }
  return b
}

const validationSchema = new schema.Array(new schema.Entity('errors', {}, {
  mergeStrategy: (a, b) => mergeWith(a, b, mergerJs)
}))

const getNormalizedErrors = createSelector(
  getValidationErrors,
  validation => validation && validation.length && fromJS(normalize(validation, validationSchema)) || null
)

export const createErrors = (values, normalized, onClickAction, parentId = null) => {
  const id = values.get('id') || parentId
  // console.log('id: ', id, path)
  const val = values.map((v, k) => {
    const err = normalized.getIn(['entities', 'errors', id, 'fields', k])
    // console.log(err, normalized.toJS(), id, k)
    if (err && err.size > 0) {
      const label = CommonMessages[k] || k
      // console.log('errmapped', err, label, k, err.toJS(), v)
      return err.reduce((error, languages, errorType) => {
        const mappedErrorType = errorTypes[errorType]
        const currentError = fromJS(createError(
          label,
          validationMessages[mappedErrorType.error],
          languages,
          typeof mappedErrorType.getOptions === 'function' ? mappedErrorType.getOptions(k) : null,
          { type: validationMessageTypes.asError, onClick: onClickAction }
        ))

        // Map({
        //   label: label,
        //   message: validationMessages[mappedErrorType.error],
        //   languages: languages,
        //   type: validationMessageTypes.asError,
        //   options: typeof mappedErrorType.getOptions === 'function' ? mappedErrorType.getOptions(k) : null,
        //   onClick: onClickAction
        // })
        if (error) {
          // return Iterable.isList(error) ? error.push(currentError) : List(currentError)
          return error.mergeWith(merger, error)
        }
        return currentError
      }, null)
    }

    if (Iterable.isIterable(v)) {
      const result = createErrors(v, normalized, onClickAction, id)
      if (result) {
        const filtered = result.filter(x => x)
        return filtered.size
          ? (Iterable.isIndexed(result) && result || filtered)
          : null
      }
    }
    return null
  })
  return val
}

const getValues = createSelector(
  [state => state, getParameterFromProps('formName')],
  (state, formName) => getFormInitialValues(formName)(state)
)

export const getPublishingValidationErrors = createSelector(
  [getNormalizedErrors, getValues, getParameterFromProps('onClick')],
  (normalized, values, onClick) =>
    normalized && createErrors(values.delete('languagesAvailabilities'), normalized, onClick) || null
)

export const getValidationMessages = createGetFlatFormValidationErrors(getPublishingValidationErrors)
