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
import { daysOfTheWeek, initialNames } from '../OpeningDays'
import { IsOpenNonStop } from 'util/redux-form/fields'
import WeekDay from '../WeekDay'
import { withFormStates } from 'util/redux-form/HOC'
import { compose } from 'redux'
import styles from '../styles.scss'

const defaultTime = { from: 28800000, to: 57600000 }

const RenderOpeningDays = ({
  dailyOpeningHours,
  isOpenNonStop,
  updateUI,
  names,
  isReadOnly
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
          />
        </div>
      </div>
      {!isOpenNonStopValue && daysOfTheWeek.map(dayName => {
        const { active, intervals } = dailyOpeningHours[dayName]
        const handleOnActivate = (...args) => {
          active.input.onChange(...args)
          const checkboxValue = args[1]
          intervals[0].from.input.onChange(
            checkboxValue
              ? defaultTime.from
              : null
          )
          intervals[0].to.input.onChange(
            checkboxValue
              ? defaultTime.to
              : null
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
            />
          : null
      })}
    </div>
  )
}
RenderOpeningDays.propTypes = {
  openingDays: PropTypes.object,
  updateUI: PropTypes.func,
  names: PropTypes.arrayOf(PropTypes.string)
}

export default compose(
  withFormStates
)(RenderOpeningDays)
