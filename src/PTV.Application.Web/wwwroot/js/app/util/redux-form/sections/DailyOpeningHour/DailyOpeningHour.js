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
import { compose } from 'redux'
import { connect } from 'react-redux'
import withState from 'util/withState'
import { FormSection, Field, formValueSelector, change } from 'redux-form/immutable'
import { RenderCheckBox, RenderClickIcon } from 'util/redux-form/renders'
import { TimeSelect } from 'util/redux-form/fields'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { Map } from 'immutable'
import styles from './styles.scss'
import { daysOfTheWeek } from 'enums'

const messages = defineMessages({
  monday: {
    id: 'Containers.Channels.OpeningHours.Monday',
    defaultMessage: 'Maanantai'
  },
  tuesday: {
    id: 'Containers.Channels.OpeningHours.Tuesday',
    defaultMessage: 'Tiistai'
  },
  wednesday: {
    id: 'Containers.Channels.OpeningHours.Wednesday',
    defaultMessage: 'Keskiviikko'
  },
  thursday: {
    id: 'Containers.Channels.OpeningHours.Thursday',
    defaultMessage: 'Torstai'
  },
  friday: {
    id: 'Containers.Channels.OpeningHours.Friday',
    defaultMessage: 'Perjantai'
  },
  saturday: {
    id: 'Containers.Channels.OpeningHours.Saturday',
    defaultMessage: 'Lauantai'
  },
  sunday: {
    id: 'Containers.Channels.OpeningHours.Sunday',
    defaultMessage: 'Sunnuntai'
  },
  hoursConjunction: {
    id: 'Util.ReduxForm.DailyOpeningHour.HoursConjunction',
    defaultMessage: 'ja'
  }
})

class DailyOpeningHour extends FormSection {
  static defaultProps = {
    name: 'dailyOpeningHour'
  }

  handleOnChangeDay = (_, checkbox) => {
    const {
      formName,
      dayName,
      index,
      dailyOpeningHourValues
    } = this.props
    const _getClosestOpeningDay = dayName => {
      // Take all days in weak before `dayName` //
      const daysOfTheWeekRange = daysOfTheWeek
        .slice(0, daysOfTheWeek.indexOf(dayName))
      for (let dayIndex = daysOfTheWeekRange.length - 1; dayIndex >= 0; --dayIndex) {
        const dayName = daysOfTheWeek[dayIndex]
        const openingDay = dailyOpeningHourValues.get(dayName)
        if (openingDay) {
          return openingDay
        }
      }
    }
    const path = `openingHours.normalOpeningHours[${index}].dailyOpeningHours.${dayName}`
    // Set previously set value //
    if (checkbox) {
      // We have some filled values above `dayName` note: use some //
      const isDailyOpeningHoursValuesEmpty = typeof dailyOpeningHourValues === 'undefined' ||
        dailyOpeningHourValues.every(day => {
          // day used as quick fix to filter isOpenNonStop
          return day && day.get && !day.get('from') &&
            !day.get('to') &&
            !day.get('isSelected')
        })
      if (!isDailyOpeningHoursValuesEmpty) {
        const closestOpeningDay = _getClosestOpeningDay(dayName)
        this.props.change(formName, path, closestOpeningDay)
        // All values are empty note: use every //
      } else {
        const fromPrevious = this.props.previousValues.from
        const toPrevious = this.props.previousValues.to
        this.props.change(formName, path, Map({
          from: fromPrevious,
          to: toPrevious
        }))
      }
    } else { // Reset values (previous value is kept in ui state)
      this.props.change(formName, path, Map({
        from: null,
        to: null,
        isSelected: false
      }))
    }
  }

  handleTimeSelectOnChange = fieldName => (_, value) => {
    const {
      formName,
      dayName,
      change,
      index
    } = this.props
    const path = `openingHours.normalOpeningHours[${index}].dailyOpeningHours.${dayName}`
    // Set checkbox to true and show add extra hours icon on every change //
    change(formName, `${path}.isSelected`, true)
    // value === null means clear `x` was clicked //
    if (!value) {
      value = {
        from: 28800000,
        to: 57600000
      }[fieldName] || null
      change(formName, `${path}.${fieldName}`, value)
      this.props.updateUI({ isManuallyFilled: false })
    } else {
      this.props.updateUI({
        previousValues: {
          ...this.props.previousValues,
          [fieldName]: value
        }
      })
      this.props.updateUI({ isManuallyFilled: true })
    }
  }
  render () {
    const {
      dayName,
      isOpenNonStop,
      intl: { formatMessage },
      timeFrom,
      timeTo,
      timeFromExtra,
      isSelected,
      areExtraHoursVisible,
      isManuallyFilled
    } = this.props
    return (
      <div className='row align-items-center mb-2'>
        <div className='col-md-5 mb-2 mb-md-0'>
          <Field
            name='isSelected'
            component={RenderCheckBox}
            label={formatMessage(messages[dayName])}
            type='checkbox'
            onChange={this.handleOnChangeDay}
            disabled={isOpenNonStop}
          />
        </div>
        <div className='col-md-19'>
          <div className='row align-items-center no-gutters'>
            <div className='col-md-11'>
              <div className={styles.dailyOpeningHour}>
                <div className={styles.dailyFrom}>
                  <TimeSelect
                    onChange={this.handleTimeSelectOnChange('from')}
                    name={'from'}
                    resetValue={28800000}
                    clearable={isManuallyFilled}
                    disabled={isOpenNonStop}
                  />
                </div>
                <div className={styles.dailyTo}>
                  <TimeSelect
                    onChange={this.handleTimeSelectOnChange('to')}
                    name={'to'}
                    resetValue={57600000}
                    clearable={isManuallyFilled}
                    filterFrom={timeFrom}
                    disabled={isOpenNonStop}
                  />
                </div>
              </div>
            </div>
            {isSelected &&
              <div className='col-md-13'>
                {!areExtraHoursVisible
                ? <Field
                  name='isExtra'
                  iconType='icon-plus'
                  component={RenderClickIcon}
                  setValue
                />
                  : <div className='row align-items-center no-gutters'>
                    <div className='col-md-2'>
                      <div className={styles.hoursConjunction}>
                        {formatMessage(messages.hoursConjunction)}
                      </div>
                    </div>
                    <div className='col-md-20'>
                      <div className={styles.dailyOpeningHour}>
                        <div className={styles.dailyFrom}>
                          <TimeSelect
                            name={'fromExtra'}
                            filterFrom={timeTo}
                            disabled={isOpenNonStop}
                          />
                        </div>
                        <div className={styles.dailyTo}>
                          <TimeSelect
                            name={'toExtra'}
                            filterFrom={timeFromExtra}
                            disabled={isOpenNonStop}
                          />
                        </div>
                      </div>
                    </div>
                    <div className='col-md-2'>
                      <Field
                        name='isExtra'
                        iconType='icon-close'
                        component={RenderClickIcon}
                        setValue={false}
                      />
                    </div>
                  </div>}
              </div>
            }
          </div>
        </div>
      </div>
    )
  }
}
DailyOpeningHour.propTypes = {
  updateUI: PropTypes.func,
  dailyOpeningHourValues: PropTypes.object,
  dayName: PropTypes.string,
  intl: intlShape
}

export default compose(
  connect(
    (state, { index, formName, dayName }) => {
      const dailyOpeningHoursValueSelector = formValueSelector(formName)
      const path = `openingHours.normalOpeningHours[${index}]`
      const dayilyHoursPath = `openingHours.normalOpeningHours[${index}].dailyOpeningHours`
      const dailyOpeningHourValues = dailyOpeningHoursValueSelector(state, dayilyHoursPath)
      return {
        dailyOpeningHourValues,
        isOpenNonStop: dailyOpeningHoursValueSelector(state, `${path}.isOpenNonStop`),
        isSelected: dailyOpeningHoursValueSelector(state, `${dayilyHoursPath}.${dayName}.isSelected`),
        areExtraHoursVisible: dailyOpeningHoursValueSelector(state, `${dayilyHoursPath}.${dayName}.isExtra`)
        // timeFrom: dailyOpeningHoursValueSelector(state, `${path}.dailyOpeningHours.${dayName}.from`),
        // timeTo: dailyOpeningHoursValueSelector(state, `${path}.dailyOpeningHours.${dayName}.to`),
        // timeFromExtra: dailyOpeningHoursValueSelector(state, `${path}.dailyOpeningHours.${dayName}.fromExtra`)
      }
    }, {
      change
    }
  ),
  withState({
    initialState: {
      isManuallyFilled: false,
      previousValues: {
        from: 28800000, // 8:00
        to: 57600000 // 16:00
      }
    }
  }),
  injectIntl,
)(DailyOpeningHour)
