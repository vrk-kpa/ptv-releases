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
import { formValueSelector, change } from 'redux-form/immutable'
import { formTypesEnum, formEntityTypes } from 'enums'
import { EntitySelectors } from 'selectors'
import { Map } from 'immutable'

const copyNewLanguagePhoneFax = ({ code, store, formName, path }) => {
  const state = store.getState()
  const dispatch = store.dispatch
  const existingLanguageAvailabilities = formValueSelector(formName)(state, 'languagesAvailabilities')
  const templateLanguageAvailability = existingLanguageAvailabilities.filter(la => la.get('code') === 'fi').first() ||
    existingLanguageAvailabilities.filter(la => la.get('code') !== '?').first()

  if (!templateLanguageAvailability || !templateLanguageAvailability.size) {
    return
  }

  const templateLanguageCode = templateLanguageAvailability.get('code')
  const formPhones = formValueSelector(formName)(state, path)
  const templatePhones = formPhones && formPhones.get(templateLanguageCode)

  if (!templatePhones || !templatePhones.size) {
    return
  }

  const templateWithoutId = templatePhones.map(p => p.set('id', null))
  dispatch(change(formName, `${path}.${code}`, templateWithoutId))
}

export const setPhoneForServiceLocation = (code, store) =>
  (copyNewLanguagePhoneFax({
    code,
    store,
    formName: formTypesEnum.SERVICELOCATIONFORM,
    path: 'phoneNumbers'
  }) && copyNewLanguagePhoneFax({
      code,
      store,
      formName: formTypesEnum.SERVICELOCATIONFORM,
      path: 'faxNumbers'
    }))

export const setPhoneForEChannel = (code, store) =>
  copyNewLanguagePhoneFax({
    code,
    store,
    formName: formTypesEnum.ELECTRONICCHANNELFORM,
    path: 'phoneNumbers'
  })

export const setPhoneForWebPage = (code, store) =>
  copyNewLanguagePhoneFax({
    code,
    store,
    formName: formTypesEnum.WEBPAGEFORM,
    path: 'phoneNumbers'
  })

export const setPhoneForPrintableForm = (code, store) =>
  copyNewLanguagePhoneFax({
    code,
    store,
    formName: formTypesEnum.PRINTABLEFORM,
    path: 'phoneNumbers'
  })

export const setPhoneForPhoneChannel = (code, store) =>
  copyNewLanguagePhoneFax({
    code,
    store,
    formName: formTypesEnum.PHONECHANNELFORM,
    path: 'phoneNumbers'
  })

const splitPath = fullPath => {
  const dotParts = fullPath.split('.')
  const prefix = dotParts[0]
  const bracketParts = dotParts[1].split('[')
  return {
    prefix,
    code: bracketParts[0],
    index: `[${bracketParts[1]}`
  }
}

export const polyfillFieldValue = ({ dispatch, formName, path }) => () => {
  if (!path) {
    return
  }

  dispatch(({ dispatch, getState }) => {
    const state = getState()
    const { prefix, code, index } = splitPath(path)
    const entityId = formValueSelector(formName)(state, 'id')
    const entityType = formEntityTypes[formName] || ''
    const entity = entityType && entityId &&
      EntitySelectors[entityType].getEntity(state, { id: entityId, type: entityType }) || Map()

    const entityPhones = entity.getIn([prefix, code]) || Map()
    const formPhones = formValueSelector(formName)(state, `${prefix}.${code}`) || Map()

    // User is not adding new phone number for the current entity
    if (formPhones.size <= entityPhones.size) {
      return
    }
    const formValue = formValueSelector(formName)(state, path)
    const existingLanguages = formValueSelector(formName)(state, 'languagesAvailabilities')
    const otherLanguages = existingLanguages && existingLanguages.filter(la => la.get('code') !== code && la.get('code') !== '?')

    // Do nothing, if other languages do not exist.
    if (!otherLanguages || !otherLanguages.size) {
      return
    }

    otherLanguages.forEach(language => {
      const otherCode = language.get('code')
      const otherPath = `${prefix}.${otherCode}${index}`
      const otherValue = formValueSelector(formName)(state, otherPath) || Map()
      const merged = formValue.merge(otherValue).set('id', '')
      dispatch(change(formName, otherPath, merged))
    })
  })
}
