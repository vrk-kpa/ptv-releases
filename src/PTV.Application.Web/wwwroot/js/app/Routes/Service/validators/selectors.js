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
import { getParameterFromProps, getParameters } from 'selectors/base'
import { getContentLanguageCode } from 'selectors/selections'
import Entities, { getEntity } from 'selectors/entities/entities'
import { createSelector } from 'reselect'
import { Map, List } from 'immutable'

const mappings = Map({
  FundingTypeId: 'fundingType',
  ShortDescription: 'shortDescription',
  Description: 'description'
})

const getEntityAvailableLanguages = createSelector(
  getEntity,
  entity => entity.get('languagesAvailabilities') || List()
)

const getEntityAvailableLanguagesByCode = createSelector(
  [getEntityAvailableLanguages, Entities.languages.getEntities],
  (entitylanguages, languages) => entitylanguages.reduce(
    (prev, curr) => prev.set(languages.getIn([curr.get('languageId'), 'code']), curr),
    Map()
  )
)

const getEntityAvailableLanguage = createSelector(
  [getEntityAvailableLanguagesByCode, getContentLanguageCode],
  (languages, languageCode) => languages.get(languageCode) || Map()
)

const createLanguagePropertySelector = (property, defaultValue) => createSelector(
  getEntityAvailableLanguage,
  language => {
    const value = language.get(property)
    console.log(property, value, defaultValue, language)
    return value == null ? defaultValue : value
  }
)

export const getLanguageStatusId = createLanguagePropertySelector('statusId')

const getValidatedFields = createLanguagePropertySelector('validatedFields', List())

export const getValidationMessages = createSelector(
  getValidatedFields,
  fields => fields.map(f => mappings.get(f.get('key')) || f.get('key'))
  //    Map({
  //   label: { id: f.get('key'), defaultMessage: f.get('key')},
  //   message: { id: 'Components.Validators.IsRequired.Message', defaultMessage: 'Täytä kentän tiedot.'}
  // }
  // )
// )
)
