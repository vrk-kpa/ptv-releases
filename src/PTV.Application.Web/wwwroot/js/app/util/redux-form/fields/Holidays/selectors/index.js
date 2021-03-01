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
import { Map, List } from 'immutable'
import { formValueSelector } from 'redux-form/immutable'
import _ from 'lodash'
import moment from 'moment'
import { EnumsSelectors } from 'selectors'

export const getHolidays = (formName, formPath) => createSelector(
  state => formValueSelector(formName)(state, `${formPath}.holidayHours`),
  formValues => formValues || Map()
)

const getHolidayNameArray = () => createSelector(
  state => EnumsSelectors.holidays.getEntities(state),
  holidays => holidays.map(h => h.get('code')).toArray() || []
)
export const getHolidaysNames = (formName, index) => createSelector(
  getHolidays(formName, index),
  getHolidayNameArray(),
  (holidays, names) => {
    const intervalNames = names.reduce((acc, dayName) => {
      const intervals = holidays.getIn([dayName, 'intervals'])
      if (intervals && intervals.size !== 0) {
        intervals.forEach((interval, index) => {
          acc = acc
            .push(`holidayHours.${dayName}.intervals[${index}].from`)
            .push(`holidayHours.${dayName}.intervals[${index}].to`)
        })
      }
      return acc
    }, List())
    const result = _.uniq([
      ...names.map(dayName => `holidayHours.${dayName}`),
      ...names.map(dayName => `holidayHours.${dayName}.active`),
      ...names.map(dayName => `holidayHours.${dayName}.intervals[0].from`),
      ...names.map(dayName => `holidayHours.${dayName}.intervals[0].to`),
      ...names.map(dayName => `holidayHours.${dayName}.type`),
      ...intervalNames.toJS()
    ])
    return result
  }
)

const getHolidaysFrom = (formName, formPath) => createSelector(
  state => formValueSelector(formName)(state, `${formPath}.dateFrom`),
  formValues => formValues
)
const getHolidaysTo = (formName, formPath) => createSelector(
  state => formValueSelector(formName)(state, `${formPath}.dateTo`),
  formValues => formValues
)

export const isHolidayOpen = (formName, formPath) => createSelector(
  state => formValueSelector(formName)(state, `${formPath}.type`),
  type => type && type === 'open'
)

export const getIntervalHolidaysNames = (formName, formPath) => createSelector(
  [ getHolidaysFrom(formName, formPath),
    getHolidaysTo(formName, formPath),
    getHolidayNameArray()],
  (from, to, names) => {
    if (from && to) {
      const fromDate = moment.utc(from).startOf('d')
      const toDate = moment.utc(to).startOf('d')
      // if period is longer than 6 days, is not needed to filter days
      // check for duration should be less than 6 because start of the day is taken for calculation
      if (moment.duration(toDate - fromDate).asDays() < 6) {
        const dayFrom = fromDate.isoWeekday() - 1
        const dayTo = toDate.isoWeekday() - 1
        let resultDays = names.map((dayName, dayNumber) =>
          dayFrom < dayTo
            ? dayFrom <= dayNumber && dayTo >= dayNumber && dayName
            : (dayFrom <= dayNumber || dayTo >= dayNumber) && dayName
        ).filter(x => x)
        return resultDays
      }
    }
    return names
  }
)

