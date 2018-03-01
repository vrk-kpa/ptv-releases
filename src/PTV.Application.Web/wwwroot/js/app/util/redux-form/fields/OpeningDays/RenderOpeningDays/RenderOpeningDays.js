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
import React from 'react'
import PropTypes from 'prop-types'
import { daysOfTheWeek } from '../OpeningDays'
import { IsOpenNonStop } from 'util/redux-form/fields'
import WeekDay from '../WeekDay'
import { withFormStates, injectFormName } from 'util/redux-form/HOC'
import { change } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import styles from '../styles.scss'
import { List, fromJS } from 'immutable'

const defaultTime = { from: 28800000, to: 57600000 }

const RenderOpeningDays = ({
  dailyOpeningHours,
  isOpenNonStop,
  updateUI,
  names,
  isReadOnly,
  setDayIntervals,
  formName,
  isCompareMode,
  compare
}) => {
  const { input: { value: isOpenNonStopValue } } = isOpenNonStop
  const handleOnOpenNonStopChange = (_, value) => {}
  return (
    <div className={styles.openingItem}>
      <div className='row'>
        <div className='col-lg-8'>
          <IsOpenNonStop
            name='isOpenNonStop'
            onChange={handleOnOpenNonStopChange}
            isCompareMode={isCompareMode}
            compare={compare}
          />
        </div>
      </div>
      {!isOpenNonStopValue && daysOfTheWeek.map((dayName, dayIndex) => {
        const { active, intervals } = dailyOpeningHours[dayName]
        const handleOnActivate = (...args) => {
          active.input.onChange(...args)
          let previousTimes = [defaultTime]
          if (dayIndex > 0) {
            const previousDaysOfTheWeek = daysOfTheWeek.slice(0, dayIndex).reverse()
            const nearestPreviousDay = previousDaysOfTheWeek.find(day => dailyOpeningHours[day].active.input.value)
            dailyOpeningHours[nearestPreviousDay] &&
            dailyOpeningHours[nearestPreviousDay].intervals.map((interval, index) => {
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
            `${dailyOpeningHours[dayName].input.name}.intervals`,
            checkboxValue ? fromJS(previousTimes) : List()
          )
        }
        const isActive = active.input.value
        const shouldShow = !isReadOnly || (
          isReadOnly &&
          intervals.length !== 0 &&
          !!intervals[0].from.input.value &&
          !!intervals[0].to.input.value
        )
        return shouldShow
          ? <WeekDay
            key={dayName}
            intervals={intervals}
            dayName={dayName}
            openingDays={dailyOpeningHours}
            onActivate={handleOnActivate}
            isActive={isActive}
            isCompareMode={isCompareMode}
            compare={compare}
          />
          : null
      })}
    </div>
  )
}
RenderOpeningDays.propTypes = {
  dailyOpeningHours: PropTypes.object,
  isOpenNonStop: PropTypes.object,
  isReadOnly: PropTypes.bool,
  updateUI: PropTypes.func,
  names: PropTypes.arrayOf(PropTypes.string),
  setDayIntervals: PropTypes.func,
  formName: PropTypes.string,
  compare: PropTypes.bool,
  isCompareMode: PropTypes.bool
}

export default compose(
  injectFormName,
  connect(null, {
    setDayIntervals: change
  }),
  withFormStates
)(RenderOpeningDays)
