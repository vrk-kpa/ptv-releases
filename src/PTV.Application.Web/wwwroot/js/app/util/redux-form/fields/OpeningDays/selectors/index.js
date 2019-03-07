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
import { Map, List } from 'immutable'
import { formValueSelector } from 'redux-form/immutable'
import { daysOfTheWeek } from 'enums'
import _ from 'lodash'
import moment from 'moment'

const getOpeningDays = (formName, formPath) => createSelector(
  state => formValueSelector(formName)(state, `${formPath}.dailyOpeningHours`),
  formValues => formValues || Map()
)
export const getOpeningDaysNames = (formName, index) => createSelector(
  getOpeningDays(formName, index),
  openingDays => {
    const intervalNames = daysOfTheWeek.reduce((acc, dayName) => {
      const intervals = openingDays.getIn([dayName, 'intervals'])
      if (intervals && intervals.size !== 0) {
        intervals.forEach((interval, index) => {
          acc = acc
            .push(`dailyOpeningHours.${dayName}.intervals[${index}].from`)
            .push(`dailyOpeningHours.${dayName}.intervals[${index}].to`)
        })
      }
      return acc
    }, List())
    const result = _.uniq([
      'isOpenNonStop',
      ...daysOfTheWeek.map(dayName => `dailyOpeningHours.${dayName}`),
      ...daysOfTheWeek.map(dayName => `dailyOpeningHours.${dayName}.active`),
      ...daysOfTheWeek.map(dayName => `dailyOpeningHours.${dayName}.intervals[0].from`),
      ...daysOfTheWeek.map(dayName => `dailyOpeningHours.${dayName}.intervals[0].to`),
      ...intervalNames.toJS()
    ])
    return result
  }
)

const getOpeningDaysIsPeriod = (formName, formPath) => createSelector(
  state => formValueSelector(formName)(state, `${formPath}.isPeriod`),
  formValues => formValues || false
)
const getOpeningDaysFrom = (formName, formPath) => createSelector(
  state => formValueSelector(formName)(state, `${formPath}.dateFrom`),
  formValues => formValues
)
const getOpeningDaysTo = (formName, formPath) => createSelector(
  state => formValueSelector(formName)(state, `${formPath}.dateTo`),
  formValues => formValues
)

export const getOpeningIntervalDaysNames = (formName, formPath) => createSelector(
  [ getOpeningDaysIsPeriod(formName, formPath),
    getOpeningDaysFrom(formName, formPath),
    getOpeningDaysTo(formName, formPath)],
  (isPeriod, from, to) => {
    if (isPeriod && from && to) {
      const fromDate = moment.utc(from).startOf('d')
      const toDate = moment.utc(to).startOf('d')
      // if period is longer than 6 days, is not needed to filter days
      // check for duration should be less than 6 because start of the day is taken for calculation
      if (moment.duration(toDate - fromDate).asDays() < 6) {
        const dayFrom = fromDate.isoWeekday() - 1
        const dayTo = toDate.isoWeekday() - 1
        let resultDays = daysOfTheWeek.map((dayName, dayNumber) =>
          dayFrom < dayTo
            ? dayFrom <= dayNumber && dayTo >= dayNumber && dayName
            : (dayFrom <= dayNumber || dayTo >= dayNumber) && dayName
        ).filter(x => x)
        return resultDays
      }
    }
    return daysOfTheWeek
  }
)

