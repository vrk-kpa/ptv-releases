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
import { EntitySelectors } from 'selectors'
import { createSelector } from 'reselect'
import { getFormValueWithPath } from 'selectors/base'
import { combinePathAndField } from 'util/redux-form/util'
import { List } from 'immutable'

const getStreetJS = createSelector(
  EntitySelectors.streets.getEntity,
  street => street.toJS()
)

export const getStreetDefaultOptionsJS = createSelector(
  getStreetJS,
  street => [street]
)

export const getStreetOptionsJS = createSelector(
  EntitySelectors.streets.getEntities,
  streets => streets.map(street => ({
    value: street.get('id'),
    label: street.get('defaultTranslation'),
    municipalityId: street.get('municipalityId')
  }))
)

export const getFormPostalCodeId = createSelector(
  getFormValueWithPath((props) => combinePathAndField(props.path, 'postalCode')),
  id => id
)

export const getFormStreetNumber = createSelector(
  getFormValueWithPath(props => combinePathAndField(props.path, 'streetNumber')),
  value => value
)

export const getLanguageId = languageCode => createSelector(
  EntitySelectors.languages.getEntities,
  languages => {
    const languagesList = languages && languages.toList() || List()
    const languageEntity = languagesList.filter((value, key) => value.get('code') === languageCode).first()
    return languageEntity && languageEntity.get('id')
  }
)
