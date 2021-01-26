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
import React from 'react'
import PropTypes from 'prop-types'
import Holiday from '../Holiday'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { change } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import styles from '../styles.scss'
import { List, fromJS } from 'immutable'
import { getIntervalHolidaysNames } from '../selectors'
import { EnumsSelectors } from 'selectors'
import HolidayInfo from '../HolidayInfo'

const defaultTime = { from: 28800000, to: 57600000 }

const RenderHolidays = ({
  holidayHours,
  updateUI,
  names,
  isReadOnly,
  setDayIntervals,
  formName,
  isCompareMode,
  compare,
  availableHolidayNames,
  holidayEntities,
  holidaysNames
}) => {
  return (
    <div className={styles.holidayWrap}>
      <HolidayInfo />
      {holidayEntities.map((holidayEntity, dayIndex) => {
        const holidayName = holidayEntity.get('code')
        const { active, intervals } = holidayHours[holidayName]
        const handleOnActivate = (...args) => {
          active.input.onChange(...args)
          let previousTimes = [defaultTime]
          if (dayIndex > 0) {
            const previousDaysOfTheWeek = holidaysNames.slice(0, dayIndex).reverse()
            const nearestPreviousDay = previousDaysOfTheWeek.find(day => holidayHours[day].active.input.value)
            holidayHours[nearestPreviousDay] &&
            holidayHours[nearestPreviousDay].intervals.map((interval, index) => {
              if (
                interval.from.input.value &&
                interval.to.input.value
              ) {
                previousTimes[index] = {
                  from: interval.from.input.value,
                  to: interval.to.input.value
                }
              }
            })
          }
          const checkboxValue = args[1]
          setDayIntervals(
            formName,
            `${holidayHours[holidayName].input.name}.intervals`,
            checkboxValue ? fromJS(previousTimes) : List()
          )
        }
        const isActive = active.input.value && availableHolidayNames.includes(holidayName)
        const shouldShow = !isReadOnly || (
          isReadOnly &&
          intervals.length !== 0 &&
          !!intervals[0].from.input.value &&
          !!intervals[0].to.input.value
        )
        return shouldShow
          ? <Holiday
            key={holidayName}
            intervals={intervals}
            holidayName={holidayEntity.get('code')}
            holidayNameId={holidayEntity.get('id')}
            onActivate={handleOnActivate}
            isCompareMode={isCompareMode}
            compare={compare}
            checked={isActive}
            disabled={!availableHolidayNames.includes(holidayName)}
          />
          : null
      })}
    </div>
  )
}
RenderHolidays.propTypes = {
  holidayHours: PropTypes.object,
  isReadOnly: PropTypes.bool,
  updateUI: PropTypes.func,
  names: PropTypes.arrayOf(PropTypes.string),
  setDayIntervals: PropTypes.func,
  formName: PropTypes.string,
  compare: PropTypes.bool,
  isCompareMode: PropTypes.bool,
  availableHolidayNames: PropTypes.array,
  holidayEntities: PropTypes.array,
  holidaysNames: PropTypes.array
}

export default compose(
  injectFormName,
  connect((state, { formName, path }) => {
    const holidays = EnumsSelectors.holidays.getEntities(state)
    return {
      availableHolidayNames: getIntervalHolidaysNames(formName, path)(state),
      holidayEntities:holidays.toArray(),
      holidaysNames:holidays.map(x => x.get('code')).toArray()
    }
  }, {
    setDayIntervals: change
  }),
  withFormStates
)(RenderHolidays)
