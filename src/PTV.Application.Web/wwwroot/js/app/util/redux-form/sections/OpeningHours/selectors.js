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
import { List, Map } from 'immutable'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import { daysOfTheWeek } from 'enums'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'
import { startCase } from 'lodash'
import moment from 'moment'

const getItems = (_, { items }) => items
const getIndex = (_, { index }) => index
const getIsInCompareMode = (_, { compare }) => !!compare

const getLanguageCode = createSelector(
  [
    getContentLanguageCode,
    getSelectedComparisionLanguageCode,
    getIsInCompareMode
  ],
  (language, comparisionLanguageCode, compare) => {
    return compare && comparisionLanguageCode || language || 'fi'
  }
)

const getItem = createSelector(
  [getIndex, getItems],
  (index, items) => items && items.get(index) || Map()
)

const getTitle = createSelector(
  [getItem, getLanguageCode],
  (openingHours, languageCode) => openingHours.getIn(['title', languageCode])
)
const getIsPeriod = createSelector(
  getItem,
  openingHours => !!openingHours.get('isPeriod')
)

const getOpeningHours = createSelector(
  getItem,
  item => item.get('dailyOpeningHours') || List()
)

const getIsOpeningNonStop = createSelector(
  getItem,
  item => item.get('isOpenNonStop') || false
)

const dayNamesTranslationMap = daysOfTheWeek.map(dayName => ({
  id: `Containers.Channels.OpeningHours.${startCase(dayName)}`,
  name: dayName,
  dayName
}))
const getTranslatedDayNames = createTranslatedListSelector(
  () => dayNamesTranslationMap,
  {
    languageTranslationType: languageTranslationTypes.data
  }
)
const getTranslatedDayNamesMap = createSelector(
  getTranslatedDayNames,
  translatedName => translatedName
    .reduce((acc, curr) => acc.set(curr.dayName, curr.name), Map())
)

const isValidityTypeTranslationMap = [
  {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.Onward',
    name: 'onward',
    type: 'onward'
  },
  {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.Period',
    name: 'period',
    type: 'period'
  }
]
const getTranslatedValidityType = createTranslatedListSelector(
  () => isValidityTypeTranslationMap,
  {
    languageTranslationType: languageTranslationTypes.locale
  }
)
const getTranslatedValidityTypesMap = createSelector(
  getTranslatedValidityType,
  translatedName => translatedName
    .reduce((acc, curr) => acc.set(curr.type, curr.name), Map())
)

export const getItemTitle = createSelector(
  [
    getTitle,
    getOpeningHours,
    getTranslatedDayNamesMap,
    getIsPeriod,
    getTranslatedValidityTypesMap,
    getIsOpeningNonStop
  ],
  (
    title,
    openingHours,
    trasnalatedDaysName,
    isPeriod,
    translatedValidityTypes,
    isOpeningNonStop
  ) => {
    const dayFormatString = 'HH:mm'
    openingHours = openingHours.map((oh, name) => oh.set('sortIndex', daysOfTheWeek.indexOf(name)))
    const validDays = isOpeningNonStop ? new List() : openingHours.filter(day => day && day.get('active')).sortBy(oh => oh.get('sortIndex'))
    const days =
    validDays.reduce((acc, curr, dayName) => {
      return acc.push(
        trasnalatedDaysName.get(dayName) +
          ' ' +
          curr
            .get('intervals')
            .reduce((acc, curr) => {
              if (curr && Map.isMap(curr)) {
                const fromTimeValue = curr.get('from')
                const toTimeValue = curr.get('to')
                if (fromTimeValue && toTimeValue) {
                  const fromTimeMoment = moment.utc(fromTimeValue)
                  const toTimeMoment = moment.utc(toTimeValue)
                  if (fromTimeMoment.isValid() && toTimeMoment.isValid()) {
                    acc = acc.push(
                      fromTimeMoment.format(dayFormatString) +
                      '-' +
                      toTimeMoment.format(dayFormatString)
                    )
                  }
                }
              }
              return acc
            }, List())
            .join(' / ')
      )
    }, List())
      .join(', ')
    const validityType = isPeriod
      ? translatedValidityTypes.get('period')
      : translatedValidityTypes.get('onward')
    return [
      title,
      validityType,
      days
    ].filter(x => !!x).join(', ')
  }
)
