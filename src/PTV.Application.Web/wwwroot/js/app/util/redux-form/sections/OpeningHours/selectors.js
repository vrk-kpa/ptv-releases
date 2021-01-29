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
import { createSelector } from 'reselect'
import { List, Map } from 'immutable'
import {
  getContentLanguageCode,
  getSelectedComparisionLanguageCode
} from 'selectors/selections'
import { daysOfTheWeek } from 'enums'
import {
  createTranslatedListSelector,
  createTranslatedIdSelector
} from 'appComponents/Localize/selectors'
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
const getDateFrom = createSelector(
  getItem,
  openingHours => openingHours.get('dateFrom')
)
const getDateTo = createSelector(
  getItem,
  openingHours => openingHours.get('dateTo')
)
const getDayFrom = createSelector(
  getItem,
  openingHours => openingHours.get('dayFrom')
)
const getDayTo = createSelector(
  getItem,
  openingHours => openingHours.get('dayTo')
)
const getTimeFrom = createSelector(
  getItem,
  openingHours => openingHours.get('timeFrom')
)
const getTimeTo = createSelector(
  getItem,
  openingHours => openingHours.get('timeTo')
)
const getClosedForPeriod = createSelector(
  getItem,
  openingHours => !!openingHours.get('closedForPeriod')
)

const getOpeningHours = createSelector(
  getItem,
  item => item.get('dailyOpeningHours') || List()
)

const getIsOpeningNonStop = createSelector(
  getItem,
  item => item.get('isOpenNonStop') || false
)

const getIsReservationing = createSelector(
  getItem,
  item => item.get('isReservation') || false
)

const isOpenNonstopTranslationId = 'Containers.Channels.Common.OpeningHours.Nonstop.Title'
const isReservationTranslationId = 'Containers.Channels.Common.OpeningHours.Reservation.Title'
const isClosedTranslationId = 'Containers.Channels.Common.OpeningHours.ValidityType.ClosedMessage'
const isOpenTranslationId = 'Containers.Channels.Common.OpeningHours.ValidityType.OpenMessage'

const getTranslatedIsOpenNonstop = createTranslatedIdSelector(
  () => isOpenNonstopTranslationId,
  {
    languageTranslationType: languageTranslationTypes.locale
  }
)

const getTranslatedIsReservation = createTranslatedIdSelector(
  () => isReservationTranslationId,
  {
    languageTranslationType: languageTranslationTypes.locale
  }
)

const getTranslatedIsClosed = createTranslatedIdSelector(
  () => isClosedTranslationId,
  {
    languageTranslationType: languageTranslationTypes.locale
  }
)

const getTranslatedIsOpen = createTranslatedIdSelector(
  () => isOpenTranslationId,
  {
    languageTranslationType: languageTranslationTypes.locale
  }
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

const dayShortNamesTranslationMap = daysOfTheWeek.map(dayName => ({
  id: `Containers.Channels.OpeningHours.${startCase(dayName.substring(0, 2))}`,
  name: dayName,
  dayName
}))
const getTranslatedDayShortNames = createTranslatedListSelector(
  () => dayShortNamesTranslationMap,
  {
    languageTranslationType: languageTranslationTypes.data
  }
)
const getTranslatedDayShortNamesMap = createSelector(
  getTranslatedDayShortNames,
  translatedName => translatedName
    .reduce((acc, curr) => acc.set(curr.dayName, curr.name), Map())
)

const getDateFormatString = () => 'DD.MM.YYYY'
const getTimeFormatString = () => 'HH:mm'

const getValidity = (dateFrom, dateTo) => {
  const dateFormatString = getDateFormatString()
  const fromDateMoment = dateFrom && moment(dateFrom)
  const toDateMoment = dateTo && moment(dateTo)
  let validity = ''
  if (fromDateMoment && fromDateMoment.isValid()) {
    validity += fromDateMoment.format(dateFormatString)
  }
  if (toDateMoment && toDateMoment.isValid()) {
    validity += ' - ' + toDateMoment.format(dateFormatString)
  }
  return validity
}

export const getOpeningHoursNormalTitle = createSelector(
  [
    getTitle,
    getOpeningHours,
    getTranslatedDayNamesMap,
    getIsPeriod,
    getDateFrom,
    getDateTo,
    getIsOpeningNonStop,
    getIsReservationing,
    getTranslatedIsOpenNonstop,
    getTranslatedIsReservation
  ],
  (
    title,
    openingHours,
    trasnalatedDaysName,
    isPeriod,
    dateFrom,
    dateTo,
    isOpeningNonStop,
    isReservationing,
    translatedIsOpenNonstop,
    translatedIsReservation
  ) => {
    const dayFormatString = 'HH:mm'
    openingHours = openingHours.map((oh, name) => oh.set('sortIndex', daysOfTheWeek.indexOf(name)))
    const validDays = isOpeningNonStop || isReservationing ? new List() : openingHours.filter(day => day && day.get('active')).sortBy(oh => oh.get('sortIndex'))
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
                if (Number.isInteger(fromTimeValue) && Number.isInteger(toTimeValue)) {
                  const fromTimeMoment = moment.utc(fromTimeValue)
                  const toTimeMoment = moment.utc(toTimeValue)
                  if (fromTimeMoment.isValid() && toTimeMoment.isValid()) {
                    const toTimeFormatted = toTimeValue === 0 ? '24:00' : toTimeMoment.format(dayFormatString)
                    acc = acc.push(
                      fromTimeMoment.format(dayFormatString) +
                      '-' +
                      toTimeFormatted
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
    const validity = isPeriod && getValidity(dateFrom, dateTo)
    const isOpenNonstop = isOpeningNonStop && translatedIsOpenNonstop
    const isReservation = isReservationing && translatedIsReservation
    return [
      title,
      validity,
      isOpenNonstop,
      isReservation,
      days
    ].filter(x => !!x).join(', ')
  }
)

const getDateTimeInfo = (isPeriod, date, time, day, translatedDayNames) => {
  const formattedDate = date && isPeriod && moment(date).isValid() && moment(date).format(getDateFormatString())
  const dayToFormat = day && translatedDayNames.get(day)
  const formattedDay = dayToFormat && dayToFormat.charAt().toUpperCase() + dayToFormat.slice(1)
  const formattedTime = Number.isInteger(time) &&
    moment.utc(time).isValid() && moment.utc(time).format(getTimeFormatString())
  return [
    formattedDate,
    formattedDay,
    formattedTime
  ].filter(x => !!x).join(' ')
}

export const getOpeningHoursSpecialTitle = createSelector(
  [
    getTitle,
    getIsPeriod,
    getDateFrom,
    getDateTo,
    getDayFrom,
    getDayTo,
    getTimeFrom,
    getTimeTo,
    getTranslatedDayShortNamesMap
  ],
  (
    title,
    isPeriod,
    dateFrom,
    dateTo,
    dayFrom,
    dayTo,
    timeFrom,
    timeTo,
    translatedDayNames
  ) => {
    const beginning = getDateTimeInfo(isPeriod, dateFrom, timeFrom, dayFrom, translatedDayNames)
    const ending = getDateTimeInfo(isPeriod, dateTo, timeTo, dayTo, translatedDayNames)
    return [title, [beginning, ending].filter(x => x).join(' - ')].filter(x => x).join(', ')
  }
)

export const getOpeningHoursExceptionalTitle = createSelector(
  [
    getTitle,
    getIsPeriod,
    getDateFrom,
    getDateTo,
    getTimeFrom,
    getTimeTo,
    getClosedForPeriod,
    getTranslatedIsClosed,
    getTranslatedIsOpen
  ],
  (
    title,
    isPeriod,
    dateFrom,
    dateTo,
    timeFrom,
    timeTo,
    closedForPeriod,
    translatedIsClosed,
    translatedIsOpen
  ) => {
    const prefix = closedForPeriod && translatedIsClosed || translatedIsOpen
    const beginning = getDateTimeInfo(true, dateFrom, timeFrom)
    const ending = getDateTimeInfo(isPeriod, dateTo, timeTo)
    return [prefix, [beginning, ending].filter(x => x).join(' - '), title].filter(x => x).join(', ')
  }
)
