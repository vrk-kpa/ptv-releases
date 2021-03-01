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
import { Map } from 'immutable'

const createValues = (key, values, language, propertyMapping) => (
  values.map((value) => {
    const result = {
      language,
      _key: value && `${key}.${language}.${value.get('order')}`
    }
    Object.keys(propertyMapping).map(key => {
      result[propertyMapping[key]] = value && value.get(key) || ''
    })
    return result
  }).filter(value => value._key)
)

export const getMappedCollectionsData = (entity, collectionFields) =>
  entity
    .filter((x, key) => Object.keys(collectionFields).includes(key))
    .reduce((fields, x, key) => {
      if (x) {
        return fields.concat(
          x.map((values, language) => {
            let result = {
              [key]: createValues(
                key,
                values,
                language,
                collectionFields[key]
              )
            }
            return result
          }).toArray())
      }
      return fields
    }, [])

/**************************************/
/* address collections                */
/**************************************/
const createAddressValues = (key, values, options) => {
  const streetType = values.get('streetType')
  const valueString = typeof options.value === 'function' ? options.value(streetType) : options.value
  const val = values.get(valueString)

  const addressSection = streetType === 'PostOfficeBox'
    ? 'postOfficeBoxAddress'
    : streetType === 'NoAddress'
      ? 'deliveryAddressInText'
      : 'streetAddress'
  if (val) {
    let address = Map()
    if (streetType !== 'NoAddress') {
      address = val.map((x, language) => ({
        type: options.type,
        subType: streetType,
        [addressSection]: {
          [valueString] : [{
            [options.property]: x,
            language
          }]
        },
        _key: `${key}.${language}.${values.get('order')}`
      }))
    } else {
      address = val.map((x, language) => ({
        type: options.type,
        subType: streetType,
        [addressSection]: [{
          [options.property]: x,
          language
        }],
        _key: `${key}.${language}.${values.get('order')}`
      }))
    }

    return {
      [options.collection || key]:address.toArray()
    }
  }
}

export const getAddressCollectionsData = (entity, addressFields) =>
  entity
    .filter((x, key) => Object.keys(addressFields).includes(key))
    .reduce((fields, x, key) => {
      if (x) {
        return fields.concat(x.filter(value => value).map((values) => {
          let result = createAddressValues(key, values, addressFields[key])
          return result
        }).filter(x => x).toArray())
      }
      return fields
    }, [])

/**************************************/
/* localized collections          */
/**************************************/
const createLocalizedValues = (key, values, options) => {
  const val = values.get(options.value)
  if (val) {
    const result = val.map((x, language) => ({
      additionalInformation : [{
        [options.property]: x,
        language
      }],
      _key: `${key}.${language}.${values.get('order')}`,
      ...options.rest
    }))
    return {
      [options.collection || key]:result.toArray()
    }
  }
}

export const getLocalizedCollectionsData = (entity, localizedFields) =>
  entity
    .filter((x, key) => Object.keys(localizedFields).includes(key))
    .reduce((fields, x, key) => {
      if (x) {
        return fields.concat(x.filter(value => value).map((values) => {
          let result = createLocalizedValues(key, values, localizedFields[key])
          return result
        }).toArray())
      }
      return fields.filter(x=>x)
    }, [])

/**************************************/
/* Names                              */
/**************************************/
export const getNamesForQa = (entity, nameFields, keyPrefix) =>
  entity
    .filter((x, key) => Object.keys(nameFields).includes(key))
    .reduce((names, x, key) => {
      if (x) {
        const isJSON = nameFields[key].json
        return names.concat(
          x.map((value, language) => ({
            language,
            type: nameFields[key].type,
            value: !isJSON
              ? value
              : value && value.getCurrentContent().getPlainText(' '),
            _key: keyPrefix && `${keyPrefix}.${nameFields[key].type}.${language}`
          })).toArray()
        )
      }
      return names
    }, [])

/****************************************/
/* Descriptions                         */
/****************************************/

export const getDescriptionsForQa = (entity, convertValue = (value) => value, descriptionFields, keyPrefix) =>
  entity
    .filter((x, key) => Object.keys(descriptionFields).includes(key))
    .reduce((descriptions, x, key) => {
      if (x) {
        const isJSON = descriptionFields[key].json
        return descriptions.concat(
          x.map((value, language) => ({
            language,
            type: descriptionFields[key].type,
            value: !isJSON
              ? value
              : value && convertValue(value).getPlainText(' '),
            _key: keyPrefix && `${keyPrefix}.${descriptionFields[key].type}.${language}`
          })).toArray()
        )
      }
      return descriptions
    }, [])
