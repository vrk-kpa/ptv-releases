/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the 'Software'), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
import React, { PureComponent, PropTypes } from 'react'
import { compose } from 'redux'
import { DailyOpeningHour } from 'util/redux-form/sections'
import { asSection } from 'util/redux-form/HOC'
import styles from './styles.scss'

export const daysOfTheWeek = [
  'monday',
  'tuesday',
  'wednesday',
  'thursday',
  'friday',
  'saturday',
  'sunday'
]

const DailyHoursList = props => (
  <div>
    {daysOfTheWeek.map((dayName, index) => (
      <DailyOpeningHour
        key={index}
        name={dayName}
        dayName={dayName}
        {...props}
      />
    ))}
  </div>
)

const DailyHoursListSection = compose(
  asSection('dailyOpeningHours')
)(DailyHoursList)

const DailyOpeningHours = ({
  children,
  ...rest
}) => (
  <div className={styles.dailyOpeningHours}>
    <div className='row mb-2'>
      <div className='col-md-24'>
        {children}
      </div>
    </div>
    <DailyHoursListSection {...rest} />
  </div>
)
DailyOpeningHours.propTypes = {
  index: PropTypes.number,
  children: PropTypes.any
}

export default DailyOpeningHours
